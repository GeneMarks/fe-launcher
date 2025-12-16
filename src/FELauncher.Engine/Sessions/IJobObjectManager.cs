using Microsoft.Win32.SafeHandles;

namespace FELauncher.Engine.Sessions
{
    /// <summary>
    /// Manages the creation, termination, and lifetime of a win32 job object.
    /// </summary>
    public interface IJobObjectManager
    {
        /// <summary>
        /// Job handle is publicly accessible for use in win32 process creation.
        /// <br />
        /// A job object must be initialized, or accessing this public property
        /// results in an exception.
        /// </summary>
        /// <exception cref="JobObjectException">Thrown if a caller tries to access a null job handle.</exception>
        public SafeFileHandle SafeJobHandle { get; }

        /// <summary>
        /// Closes and releases the manager's existing job object and assigns a new one.
        /// </summary>
        /// <exception cref="JobObjectException">Thrown if there was an os error during job object creation.</exception>
        void ResetJobObject();

        /// <summary>
        /// Terminates the current job object if it is valid.
        /// </summary>
        /// <exception cref="JobObjectException">Thrown if there was an os error during job object termination.</exception>
        void TerminateJobObject();

        /// <summary>
        /// Creates an IOCompletionPort for the current job object and listens for completion status in a Task.
        /// </summary>
        /// <returns>Task that completes when there are no more active processes in the current job object.</returns>
        /// <exception cref="JobObjectException">Thrown if current job handle is null or invalid or if there was an os error during completion port setup.</exception>
        Task WaitForJobObjectCompletionAsync(CancellationToken ct);
    }
}
