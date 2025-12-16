namespace FELauncher.Engine.Processes
{
    public interface IProcessManager
    {
        event EventHandler<ProcessExitedEventArgs>? ProcessExited;

        /// <summary>
        /// Coordinates the steps of starting a FELProcess, including registering the
        /// Exited event, adding a delay, and starting its Win32Process.
        /// </summary>
        /// <exception cref="JobObjectException">Thrown if IJobObjectManager doesn't have a valid job handle.</exception>
        /// <exception cref="Win32ProcessException">Thrown if Win32Process failed to start.</exception>
        Task StartAndTrackAsync(FELProcess felProcess, CancellationToken ct = default);
    }
}
