using FELauncher.Engine.Settings;
using Microsoft.Extensions.Logging;

namespace FELauncher.Engine.Processes
{
    internal sealed class PreProcessRunner(
        ILogger<PreProcessRunner> logger,
        IProcessFactory factory,
        IProcessManager processManager) : IPreProcessRunner
    {
        public async Task RunAsync(IList<PreProcessSettings> preProcessSettings, CancellationToken ct = default)
        {
            if (preProcessSettings.Count == 0)
            {
                // todo: log here?
                return;
            }

            foreach (var preProcess in preProcessSettings)
            {
                ct.ThrowIfCancellationRequested();

                var process = factory.Create(preProcess.Path, preProcess.Arguments);

                var felPreProcess = new FELProcess(
                    process,
                    preProcess.DelaySeconds,
                    preProcess.NotifyOnExit,
                    preProcess.EndSessionOnExit);

                await processManager.StartAndTrackAsync(felPreProcess, ct);
            }
        }
    }
}
