namespace FELauncher.Engine.Processes
{
    public interface IPreProcessRunner
    {
        event EventHandler<PreProcessExitedEventArgs> PreProcessExited;

        /// <summary>
        /// Creates and runs processes using the user's list of PreProcessSettings.
        /// <br />
        /// Each process is added to the current job object.
        /// </summary>
        /// <exception cref="ProcessCreationException">Thrown by IProcessFactory if Process creation unsuccessful.</exception>
        /// <exception cref="JobObjectException">Thrown by IJobObjectManager if unable to assign Process to job object.</exception>
        Task RunAsync();
    }
}
