using FELauncher.Engine.Processes;
using FELauncher.Engine.Settings;
using Microsoft.Extensions.Logging;

namespace FELauncher.Engine.Sessions
{
    internal sealed class FrontendRunner(
        ILogger<FrontendRunner> logger,
        Win32ProcessFactory factory,
        FELProcessManager processManager)
    {
        public async Task RunAsync(FrontendSettings frontendSettings, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();

            var process = factory.Create(frontendSettings.Path, frontendSettings.Arguments);

            var felFrontendProcess = new FELProcess(
                process,
                frontendSettings.DelaySeconds,
                true, // todo: make user-assignable?
                true); // Always request to end session on frontend exit.

            await processManager.StartAndTrackAsync(felFrontendProcess, ct);
        }
    }
}
