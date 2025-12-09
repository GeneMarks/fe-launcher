using System.Diagnostics;

namespace FELauncher.Engine.Sessions
{
    /// <summary>
    /// Manages the creation, process assigment, termination, and lifetime of a win32 job object.
    /// </summary>
    public interface IJobObjectManager
    {
        /// <summary>
        /// Closes and releases the manager's existing job object and assigns a new one.
        /// </summary>
        /// <exception cref="JobObjectException">Thrown if there was an os error during job object creation.</exception>
        void ResetJobObject();

        /// <summary>
        /// Assigns a system process to the current job object.
        /// </summary>
        /// <param name="process">The system process to be assigned to the job object.</param>
        /// <exception cref="JobObjectException">Thrown if there was an os error trying to assign the process to the job object.</exception>
        void AssignToJobObject(Process process);

        /// <summary>
        /// Terminates the current job object if it is valid.
        /// </summary>
        /// <exception cref="JobObjectException">Thrown if there was an os error during job object termination.</exception>
        void TerminateJobObject();
    }
}
