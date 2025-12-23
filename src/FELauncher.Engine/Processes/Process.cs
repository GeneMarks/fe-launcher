using FELauncher.Engine.Exceptions;
using FELauncher.Engine.Processes.Logging;
using Microsoft.Extensions.Logging;
using Microsoft.Win32.SafeHandles;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.System.Threading;

namespace FELauncher.Engine.Processes
{
    internal sealed class Process : IDisposable
    {
        public event EventHandler<ProcessExitedEventArgs>? Exited;

        private readonly ILogger<Process> _logger;

        private readonly string _pathWithArgs;
        private readonly string? _workingDir;
        private readonly string _prettyName;
        private SafeFileHandle? _safeProcHandle;
        private SafeFileHandle? _safeWaitHandle;
        private uint _pid;

        public Process(
            ILogger<Process> logger,
            string pathWithArgs,
            string? workingDir,
            string prettyName)
        {
            _logger = logger;
            _pathWithArgs = pathWithArgs;
            _workingDir = workingDir;
            _prettyName = prettyName;
        }

        /// <summary>
        /// Creates a native windows process in the provided job handle and registers a wait callback.
        /// </summary>
        /// <param name="safeJobHandle">Handle to a job object the process will be assigned to.</param>
        /// <exception cref="ProcessException">
        /// Thrown when process creation or wait registration fails due to a Win32 error.
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

                    _logger.FailedToInitializeAttributeList(_pathWithArgs, errorCode, win32Ex);
                    throw new ProcessException($"Failed to initialize process attribute list for process with path '{_pathWithArgs}'.", win32Ex);
                }

                HANDLE jobHandle = (HANDLE)safeJobHandle.DangerousGetHandle();

                if (!PInvoke.UpdateProcThreadAttribute(
                    list, 0, PInvoke.PROC_THREAD_ATTRIBUTE_JOB_LIST, &jobHandle, (nuint)sizeof(HANDLE)))
                {
                    var errorCode = Marshal.GetLastPInvokeError();
                    var win32Ex = new Win32Exception(errorCode);

                    _logger.FailedToUpdateAttributeList(_pathWithArgs, errorCode, win32Ex);
                    throw new ProcessException($"Failed to update process attribute list for process with path '{_pathWithArgs}'.", win32Ex);
                }

                STARTUPINFOEXW siex = new()
                {
                    lpAttributeList = list
                };
                siex.StartupInfo.cb = (uint)sizeof(STARTUPINFOEXW);

                char[] cmd = (_pathWithArgs + '\0').ToCharArray();
                Span<char> lpCommandLine = cmd;
                PROCESS_INFORMATION pi;

                if (!PInvoke.CreateProcess(
                    null, ref lpCommandLine, null, null, false,
                    PROCESS_CREATION_FLAGS.EXTENDED_STARTUPINFO_PRESENT,
                    null, _workingDir, in siex.StartupInfo, out pi))
                {
                    var errorCode = Marshal.GetLastPInvokeError();
                    var win32Ex = new Win32Exception(errorCode);

                    _logger.FailedToCreateProcess(_pathWithArgs, errorCode, win32Ex);
                    throw new ProcessException($"Failed to create process with path '{_pathWithArgs}'.", win32Ex);
                }

                _safeProcHandle = new SafeFileHandle(pi.hProcess, true);
                _pid = PInvoke.GetProcessId(_safeProcHandle);

                if (!PInvoke.RegisterWaitForSingleObject(
                    out _safeWaitHandle, _safeProcHandle, new WAITORTIMERCALLBACK(WaitProc),
                    null, PInvoke.INFINITE, WORKER_THREAD_FLAGS.WT_EXECUTEONLYONCE))
                {
                    var errorCode = Marshal.GetLastPInvokeError();
                    var win32Ex = new Win32Exception(errorCode);

                    _logger.FailedToRegisterWaitOperation(_pid, _pathWithArgs, errorCode, win32Ex);
                    throw new ProcessException($"Failed to register wait operation for pid {_pid} ({_pathWithArgs}).", win32Ex);
                }
            }
            finally
            {
                PInvoke.DeleteProcThreadAttributeList(list);
                Marshal.FreeHGlobal(listBuffer);
            }
        }

        private unsafe void WaitProc(void* context, BOOLEAN timerOrWaitFired)
        {
            uint exitCode;
            if (!PInvoke.GetExitCodeProcess(_safeProcHandle, out exitCode))
            {
                exitCode = 0; // todo: handle function failure differently
            }

            Exited?.Invoke(this, new ProcessExitedEventArgs()
            {
                ProcessId   = _pid,
                ProcessPath = _pathWithArgs,
                ProcessName = _prettyName,
                ExitCode    = exitCode
            });
        }

        private void CleanupWaitHandle()
        {
            if (_safeWaitHandle is not null)
            {
                _ = PInvoke.UnregisterWait(_safeWaitHandle);

                _safeWaitHandle.Dispose();
                _safeWaitHandle = null;
            }
        }

        public void Dispose()
        {
            CleanupWaitHandle();
            _safeProcHandle?.Dispose();
        }
    }
}
