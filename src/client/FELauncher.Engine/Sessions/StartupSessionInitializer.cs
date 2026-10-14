using FELauncher.Engine.Sessions.Logging;
using FELauncher.Shared.Contracts.Sessions;
using FELauncher.Shared.Contracts.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FELauncher.Engine.Sessions
{
    internal sealed class StartupSessionInitializer(
        ILogger<StartupSessionInitializer> logger,
        IOptions<FELauncherSettings> settings,
        ISessionOrchestrator sessionOrchestrator) : IStartupSessionInitializer
    {
        public void LaunchStartupSessionIfEnabled()
        {
            if (!settings.Value.AutoLaunchSession)
            {
                logger.StartupSessionDisabled();
                return;
            }

            logger.StartupSessionEnabled();
            _ = sessionOrchestrator.StartNewSessionAsync();
        }
    }
}
