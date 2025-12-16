using FELauncher.Engine.Exceptions;
using Microsoft.Extensions.Logging;
using Microsoft.Win32.SafeHandles;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.System.JobObjects;

namespace FELauncher.Engine.Sessions
{
    internal sealed class JobObjectManager(ILogger<JobObjectManager> logger) : IJobObjectManager, IDisposable
    {
        private SafeFileHandle? _safeJobHandle;
        private SafeFileHandle? _safeCompletionPortHandle;

        public SafeFileHandle SafeJobHandle
        {
            get
            {
                if (_safeJobHandle is null || _safeJobHandle.IsInvalid)
                {
                    logger.TriedToAccessNullOrInvalidJobHandle();
                    throw new JobObjectException("Tried to access null or invalid job handle.");
                }

                return _safeJobHandle;
            }
        }

        public void ResetJobObject()
        {
            ReleaseJobObject();
            _safeJobHandle = CreateJobObject();
        }

        public async Task WaitForJobObjectCompletionAsync(CancellationToken ct)
        {
            if (_safeJobHandle is null || _safeJobHandle.IsInvalid)
            {
                logger.TriedToWaitNullOrInvalidJobHandle();
                throw new JobObjectException("Tried to wait for completion of null or invalid job handle.");
            }

            SetupIOCompletionPort();

            try
            {
                await Task.Run(() => WaitForCompletionStatus(ct));
            }
            finally
            {
                ReleaseJobObject();
            }
        }

        public void TerminateJobObject()
        {
            if (_safeJobHandle is null || _safeJobHandle.IsInvalid)
            {
                logger.TerminationUnecessary();
                return;
            }

            if (!PInvoke.TerminateJobObject(_safeJobHandle, 0))
            {
                var errorCode = Marshal.GetLastPInvokeError();
                var win32Ex = new Win32Exception(errorCode);

                logger.FailedToTerminateJobObject(errorCode, win32Ex.Message);
                throw new JobObjectException($"Failed to terminate job object.", win32Ex);
            }
        }
            
        private SafeFileHandle CreateJobObject()
        {
            SafeFileHandle safeJobHandle = PInvoke.CreateJobObject(null, null);

            if (safeJobHandle.IsInvalid)
            {
                var errorCode = Marshal.GetLastPInvokeError();
                var win32Ex = new Win32Exception(errorCode);

                logger.FailedToCreateJobObject(errorCode, win32Ex.Message);
                throw new JobObjectException("Failed to create job object.", win32Ex);
            }

            return safeJobHandle;
        }

        private unsafe void SetupIOCompletionPort()
        {
            _safeCompletionPortHandle = PInvoke.CreateIoCompletionPort(
                new SafeFileHandle(HANDLE.INVALID_HANDLE_VALUE, false), null, 0, 1);

            if (_safeCompletionPortHandle is null || _safeCompletionPortHandle.IsInvalid)
            {
                var errorCode = Marshal.GetLastPInvokeError();
                var win32Ex = new Win32Exception(errorCode);

                logger.FailedToCreateIoCompletionPort(errorCode, win32Ex.Message);
                throw new JobObjectException("Failed to create IO completion port.", win32Ex);
            }

            var jobHandle = _safeJobHandle!.DangerousGetHandle();
            JOBOBJECT_ASSOCIATE_COMPLETION_PORT port = new()
            {
                CompletionKey = (void*)jobHandle,
                CompletionPort = (HANDLE)_safeCompletionPortHandle.DangerousGetHandle()
            };

            if (!PInvoke.SetInformationJobObject(
                (HANDLE)jobHandle,
                JOBOBJECTINFOCLASS.JobObjectAssociateCompletionPortInformation,
                &port, (uint)sizeof(JOBOBJECT_ASSOCIATE_COMPLETION_PORT)))
            {
                var errorCode = Marshal.GetLastPInvokeError();
                var win32Ex = new Win32Exception(errorCode);

                logger.FailedToSetJobObjectLimits(errorCode, win32Ex.Message);
                throw new JobObjectException("Failed to set job object limits.", win32Ex);
            }
        }

        /// <summary>
        /// Must be executed on separate thread.
        /// <br />
        /// Queued completion status loop is blocking.
        /// </summary>
        private unsafe void WaitForCompletionStatus(CancellationToken ct)
        {
            using var reg = ct.Register(() =>
                // Posts a dummy completion packet to break loop
                PInvoke.PostQueuedCompletionStatus(_safeCompletionPortHandle, 0, 0, null));

            uint completionCode;
            nuint completionKey;
            NativeOverlapped* overlapped;

            while (true)
            {
                if (!PInvoke.GetQueuedCompletionStatus(
                    _safeCompletionPortHandle,
                    out completionCode, out completionKey, out overlapped,
                    PInvoke.INFINITE))
                {
                    logger.FailedToGetCompletionStatus();
                    return;
                }

                // Cancel if dummy completion packet received
                if (completionCode == 0 && completionKey == 0)
                {
                    ct.ThrowIfCancellationRequested();
                }

                if ((HANDLE)completionKey == (HANDLE)_safeJobHandle!.DangerousGetHandle()
                    && completionCode == PInvoke.JOB_OBJECT_MSG_ACTIVE_PROCESS_ZERO)
                {
                    return;
                }
            }
        }

        private void ReleaseJobObject()
        {
            _safeJobHandle?.Dispose();
            _safeJobHandle = null;
            _safeCompletionPortHandle?.Dispose();
            _safeCompletionPortHandle = null;
        }

        public void Dispose()
            => ReleaseJobObject();
    }
}
