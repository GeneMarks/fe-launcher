using Cabby.Client.Engine.Configuration;
using Cabby.Client.Engine.Core;
using Cabby.Client.Engine.Processes;
using Cabby.Client.Engine.Processes.Runner;
using Cabby.Client.Engine.TaskScheduling;
using Cabby.Client.Engine.Sessions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Cabby.Client.Shared.Contracts.Configuration;
using Cabby.Client.Engine.Processes.JobObjects;

namespace Cabby.Client.Engine
{
    public static class EngineServiceCollectionExtensions
    {
        public static IServiceCollection AddEngineServices(this IServiceCollection services)
        {
            // Public services
            services.AddSingleton<PathResolver>();
            services.AddSingleton<SessionOrchestrator>();
            services.AddSingleton<ISettingsStore, JsonSettingsStore>();

            // Internal services
            services.AddSingleton<ProcessFactory>();
            services.AddSingleton<ProcessRunner>();
            services.AddSingleton<JobObjectManager>();
            services.AddSingleton<SessionLoggerScopeProvider>();
            services.AddSingleton<StartupTaskScheduler>();

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
