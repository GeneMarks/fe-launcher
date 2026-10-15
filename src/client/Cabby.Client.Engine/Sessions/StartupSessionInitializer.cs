using Cabby.Client.Engine.Sessions.Logging;
using Cabby.Client.Shared.Contracts.Sessions;
using Cabby.Client.Shared.Contracts.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Cabby.Client.Engine.Sessions
{
    internal sealed class StartupSessionInitializer(
        ILogger<StartupSessionInitializer> logger,
        IOptions<CabbySettings> settings,
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
