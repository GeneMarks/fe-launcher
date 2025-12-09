using FELauncher.Engine.Exceptions;
using Microsoft.Extensions.Logging;
using Microsoft.Win32.SafeHandles;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Windows.Win32;

namespace FELauncher.Engine.Sessions
{
    internal sealed class JobObjectManager : IJobObjectManager, IDisposable
    {
        private SafeFileHandle? _hJobObject;

        private readonly ILogger<JobObjectManager> _logger;

        public JobObjectManager(ILogger<JobObjectManager> logger)
        {
            _logger = logger;
        }

        public void ResetJobObject()
        {
            ReleaseJobObject();
            _hJobObject = CreateJobObject();
        }

        public void AssignToJobObject(Process process)
        {
            SafeFileHandle hProcess = new(process.Handle, ownsHandle: false); // Process object already owns handle.

            // Return value is 0 if function fails.
            if (!PInvoke.AssignProcessToJobObject(_hJobObject, hProcess))
            {
                var win32Message = Marshal.GetLastPInvokeErrorMessage();
                var win32ErrCode = Marshal.GetLastPInvokeError();
                var win32Ex = new Win32Exception(win32ErrCode);

                _logger.FailedToAssignProcessToJobObject(process, win32Message);
                throw new JobObjectException($"Failed to assign process '{process}' to job object.", win32Ex);
            }
        }

        public void TerminateJobObject()
        {
            if (_hJobObject is null || _hJobObject.IsInvalid) return; // todo: log here?

            // Return value is 0 if function fails.
            if (!PInvoke.TerminateJobObject(_hJobObject, 0))
            {
                var win32Message = Marshal.GetLastPInvokeErrorMessage();
                var win32ErrCode = Marshal.GetLastPInvokeError();
                var win32Ex = new Win32Exception(win32ErrCode);

                _logger.FailedToTerminateJobObject(win32Message);
                throw new JobObjectException($"Failed to terminate job object.", win32Ex);
            }
        }

        private SafeFileHandle CreateJobObject()
        {
            SafeFileHandle hJobObject = PInvoke.CreateJobObject(lpJobAttributes: null, lpName: null);

            if (hJobObject.IsInvalid)
            {
                var win32Message = Marshal.GetLastPInvokeErrorMessage();
                var win32ErrCode = Marshal.GetLastPInvokeError();
                var win32Ex = new Win32Exception(win32ErrCode);

                _logger.FailedToCreateJobObject(win32Message);
                throw new JobObjectException("Failed to create job object.", win32Ex);
            }

            return hJobObject;
        }

        private void ReleaseJobObject()
        {
            if (_hJobObject is not null && !_hJobObject.IsInvalid)
            {
                _hJobObject.Close();
                _hJobObject = null;
            }
        }

        public void Dispose()
            => ReleaseJobObject();
    }
}
