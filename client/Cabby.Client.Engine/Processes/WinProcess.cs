using Cabby.Client.Engine.Exceptions;
using Cabby.Client.Engine.Processes.Logging;
using Cabby.Client.Shared.Contracts.Processes;
using Microsoft.Extensions.Logging;
using Microsoft.Win32.SafeHandles;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.System.Threading;

namespace Cabby.Client.Engine.Processes
{
    internal sealed class WinProcess : IDisposable
    {
        private uint _pid;
        private SafeFileHandle? _safeProcHandle;
        private WaitHandle? _waitHandle;
        private RegisteredWaitHandle? _registeredWaitHandle;
        private readonly WaitOrTimerCallback _waitCallback;

        public required ILogger<WinProcess> Logger { private get; init; }
        public required ProcessDto ProcessDto { get; init; }
        public string? WorkingDir { private get; init; }
        public required string PathWithArgs { get; init; }
        public required string PrettyName { get; init; }

        public required Action<ProcessCompletionContext> ProcessCompletionCallback { private get; init; }

        public WinProcess()
        {
            _waitCallback = WaitCallback;
        }

        /// <summary>
        /// Creates a native Windows process in the provided job handle and registers a wait callback.
        /// </summary>
        /// <param name="safeJobHandle">Handle to a job object the process will be assigned to.</param>
        /// <exception cref="ProcessException">
        /// Thrown when process creation fails due to a Win32 error.
        /// </exception>
        public unsafe void StartInJob(SafeFileHandle safeJobHandle)
        {
            nuint size;
            // First call used to assign size.
            // Errors intentionally. Function return will always be zero.
            _ = PInvoke.InitializeProcThreadAttributeList(LPPROC_THREAD_ATTRIBUTE_LIST.Null, 1, 0, &size);

            nint listBuffer = Marshal.AllocHGlobal((nint)size);
            var list = (LPPROC_THREAD_ATTRIBUTE_LIST)listBuffer;

            try
            {
                if (!PInvoke.InitializeProcThreadAttributeList(list, 1, 0, &size))
                {
                    var errorCode = Marshal.GetLastPInvokeError();
                    var win32Ex = new Win32Exception(errorCode);

                    Logger.FailedToInitializeAttributeList(PathWithArgs, errorCode, win32Ex);
                    throw new ProcessException($"Failed to initialize process attribute list for process with path '{PathWithArgs}'.", win32Ex);
                }

                HANDLE jobHandle = (HANDLE)safeJobHandle.DangerousGetHandle();

                if (!PInvoke.UpdateProcThreadAttribute(
                    list, 0, PInvoke.PROC_THREAD_ATTRIBUTE_JOB_LIST, &jobHandle, (nuint)sizeof(HANDLE)))
                {
                    var errorCode = Marshal.GetLastPInvokeError();
                    var win32Ex = new Win32Exception(errorCode);

                    Logger.FailedToUpdateAttributeList(PathWithArgs, errorCode, win32Ex);
                    throw new ProcessException($"Failed to update process attribute list for process with path '{PathWithArgs}'.", win32Ex);
                }

                STARTUPINFOEXW siex = new()
                {
                    lpAttributeList = list
                };
                siex.StartupInfo.cb = (uint)sizeof(STARTUPINFOEXW);

                char[] cmd = (PathWithArgs + '\0').ToCharArray();
                Span<char> lpCommandLine = cmd;
                PROCESS_INFORMATION pi;

                if (!PInvoke.CreateProcess(
                    null, ref lpCommandLine, null, null, false,
                    PROCESS_CREATION_FLAGS.EXTENDED_STARTUPINFO_PRESENT,
                    null, WorkingDir, in siex.StartupInfo, out pi))
                {
                    var errorCode = Marshal.GetLastPInvokeError();
                    var win32Ex = new Win32Exception(errorCode);

                    Logger.FailedToCreateProcess(PathWithArgs, errorCode, win32Ex);
                    throw new ProcessException($"Failed to create process with path '{PathWithArgs}'.", win32Ex);
                }

                _safeProcHandle = new SafeFileHandle(pi.hProcess, true);
                _pid = PInvoke.GetProcessId(_safeProcHandle);
                
                _ = PInvoke.CloseHandle(pi.hThread); // Not using this. Handle must be closed.

                RegisterWaitHandle();
            }
            finally
            {
                PInvoke.DeleteProcThreadAttributeList(list);
                Marshal.FreeHGlobal(listBuffer);
            }
        }

        private void RegisterWaitHandle()
        {
            _waitHandle = new EventWaitHandle(false, EventResetMode.AutoReset)
            {
                SafeWaitHandle = new(_safeProcHandle!.DangerousGetHandle(), false)
            };

            _registeredWaitHandle = ThreadPool.RegisterWaitForSingleObject(
                _waitHandle, _waitCallback, null, -1, true);
        }

        private void WaitCallback(object? state, bool timedOut)
        {
            uint exitCode;
            if (!PInvoke.GetExitCodeProcess(_safeProcHandle, out exitCode))
            {
                exitCode = 0; // TODO: Handle function failure differently?
            }

            Logger.ProcessExited(_pid, PathWithArgs, PrettyName, exitCode);

            var completionContext = new ProcessCompletionContext
            {
                ProcessDto = ProcessDto,
                WasSuccessful = exitCode == 0,
                FailureMessage = exitCode == 0 ? null : $"Process '{PrettyName}' exited unexpectedly with exit code: {exitCode}."
            };

            ProcessCompletionCallback(completionContext);
        }

        public void Dispose()
        {
            _registeredWaitHandle?.Unregister(null);
            _registeredWaitHandle = null;

            _waitHandle?.Dispose();
            _waitHandle = null;
            _safeProcHandle?.Dispose();
            _safeProcHandle = null;
        }
    }
}