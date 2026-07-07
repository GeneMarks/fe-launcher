using FELauncher.Engine.IO;
using FELauncher.Engine.JobObjects;
using FELauncher.Engine.Processes;
using FELauncher.Engine.Processes.Runner;
using FELauncher.Engine.Scheduling;
using FELauncher.Engine.Sessions;
using FELauncher.Shared.Contracts.IO;
using FELauncher.Shared.Contracts.Sessions;
using FELauncher.Shared.Contracts.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FELauncher.Engine
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
            services.Configure<FELauncherSettings>(config);

            return services;
        }
    }
}
