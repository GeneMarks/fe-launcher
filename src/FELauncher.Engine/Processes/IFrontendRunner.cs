using FELauncher.Engine.Settings;

namespace FELauncher.Engine.Processes
{
    public interface IFrontendRunner
    {
        /// <summary>
        /// Creates a FELProcess using the user's Frontend settings and sends it to ProcessManager to start and track.
        /// </summary>
        /// <exception cref="ProcessCreationException">Thrown by IProcessFactory if Win32Process creation is unsuccessful.</exception>
        /// <exception cref="Win32ProcessException">Thrown by IProcessManager if starting the Win32Process is unsuccessful.</exception>
        Task RunAsync(FrontendSettings settings, CancellationToken ct = default);
    }
}
