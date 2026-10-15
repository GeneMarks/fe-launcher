using Cabby.Client.Engine.IO;
using Cabby.Client.Engine.JobObjects;
using Cabby.Client.Engine.Processes;
using Cabby.Client.Engine.Processes.Runner;
using Cabby.Client.Engine.Scheduling;
using Cabby.Client.Engine.Sessions;
using Cabby.Client.Shared.Contracts.IO;
using Cabby.Client.Shared.Contracts.Sessions;
using Cabby.Client.Shared.Contracts.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cabby.Client.Engine
{
    public static class EngineServiceCollectionExtensions
    {
        public static IServiceCollection AddEngineServices(this IServiceCollection services)
        {
            // Public services
            services.AddSingleton<IPathResolver, PathResolver>();
            services.AddSingleton<ISessionOrchestrator, SessionOrchestrator>();
            services.AddSingleton<ISessionLoggerScopeProvider, SessionLoggerScopeProvider>();
            services.AddSingleton<ISettingsStore, JsonSettingsStore>();
            services.AddScoped<IStartupSessionInitializer, StartupSessionInitializer>();

            // Internal services
            services.AddSingleton<ProcessFactory>();
            services.AddSingleton<ProcessRunner>();
            services.AddSingleton<JobObjectManager>();
            services.AddHostedService<StartupTaskScheduler>();

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
