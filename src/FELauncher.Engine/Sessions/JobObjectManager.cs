using FELauncher.Engine.Exceptions;
using FELauncher.Engine.Sessions.Logging;
using Microsoft.Extensions.Logging;
using Microsoft.Win32.SafeHandles;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.System.JobObjects;

namespace FELauncher.Engine.Sessions
{
    /// <summary>
    /// Manages the creation, termination, and lifetime of a win32 job object.
    /// </summary>
    internal sealed class JobObjectManager(ILogger<JobObjectManager> logger) : IDisposable
    {
        private SafeFileHandle? _safeJobHandle;
        private SafeFileHandle? _safeCompletionPortHandle;

        /// <summary>
        /// Gets the safe handle for the currently active job object.
        /// </summary>
        /// <remarks>
        /// The job object must be initialized (via <see cref="ResetJobObject"/>) before accessing this property.
        /// </remarks>
        /// <exception cref="JobObjectException">
        /// Thrown when the job object has not been initialized or the underlying handle is invalid.
        /// </exception>
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

        /// <summary>
        /// Closes and releases the manager's existing job object and assigns a new one.
        /// </summary>
        /// <exception cref="JobObjectException">Thrown when job object creation fails due to a Win32 error.</exception>
        public void ResetJobObject()
        {
            ReleaseJobObject();
            _safeJobHandle = CreateJobObject();
        }

        /// <summary>
        /// Asynchronously waits until the current job object has no remaining active processes.
        /// </summary>
        /// <remarks>
        /// Internally sets up an I/O completion port for the job object and blocks on completion status on a background thread.
        /// </remarks>
        /// <returns>
        /// A task that completes when there are no more active processes in the current job object.
        /// </returns>
        /// <exception cref="JobObjectException">
        /// Thrown when the job object has not been initialized, the handle is invalid, or completion-port setup fails due to a Win32 error.
        /// </exception>
        /// <exception cref="OperationCanceledException">
        /// Thrown when <paramref name="ct"/> is canceled while waiting.
        /// </exception>
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

        /// <summary>
        /// Terminates the current job object if it is valid.
        /// </summary>
        /// <exception cref="JobObjectException">Thrown when job object termination fails due to a Win32Error.</exception>
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

        // Must be executed on separate thread.
        // Queued completion status loop is blocking.
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
