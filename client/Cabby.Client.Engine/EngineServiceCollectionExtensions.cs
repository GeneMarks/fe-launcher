using Cabby.Client.Engine.Configuration;
using Cabby.Client.Engine.Core;
using Cabby.Client.Engine.Processes;
using Cabby.Client.Engine.TaskScheduling;
using Cabby.Client.Engine.Sessions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Cabby.Client.Shared.Contracts.Configuration;
using Cabby.Client.Shared.Contracts.Sessions;
using Cabby.Client.Shared.Contracts.Core;
using Cabby.Client.Engine.Commands;
using Cabby.Client.Engine.Networking;

namespace Cabby.Client.Engine
{
    public static class EngineServiceCollectionExtensions
    {
        public static IServiceCollection AddEngineServices(this IServiceCollection services)
        {
            /* Internal services */

            // Networking
            services.AddSingleton<WebSocketConnection>();
            services.AddSingleton<ServerMessageDispatcher>();
            services.AddSingleton<CommandDispatcher>();

            // Sessions
            services.AddSingleton<SessionOrchestrator>();
            services.AddSingleton<SessionStatusProvider>();
            services.AddSingleton<SessionStatusUpdater>();
            services.AddSingleton<SessionLoggerScopeProvider>();

            // Processes
            services.AddSingleton<JobObjectManager>();
            services.AddSingleton<ProcessDispatcher>();
            services.AddSingleton<IProcessRunner, WinProcessRunner>();
            services.AddSingleton<WinProcessFactory>();

            // TaskScheduling
            services.AddSingleton<StartupTaskScheduler>();


            /* Public services */
            services.AddSingleton<IPathResolver, PathResolver>();
            services.AddSingleton<ISettingsStore, JsonSettingsStore>();
            services.AddSingleton<ISessionStatusProvider>(sp => sp.GetRequiredService<SessionStatusProvider>());

            return services;
        }

        public static IServiceCollection ConfigureEngineSettings(
            this IServiceCollection services,
            IConfiguration config)
        {
            services.Configure<CabbySettings>(config);

            return services;
        }
    }
}
