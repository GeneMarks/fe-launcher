using FELauncher.Engine.IO;
using FELauncher.Engine.JobObjects;
using FELauncher.Engine.Processes;
using FELauncher.Engine.Sessions;
using FELauncher.Engine.Settings;
using FELauncher.Shared.Contracts;
using FELauncher.Shared.Contracts.Sessions;
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

            // Internal services
            services.AddSingleton<ProcessFactory>();
            services.AddSingleton<ProcessRunner>();
            services.AddSingleton<JobObjectManager>();

            return services;
        }

        public static IServiceCollection ConfigureEngineSettings(
            this IServiceCollection services,
            IConfiguration config)
        {
            services.Configure<FELauncherSettings>(config.GetSection("FELauncher"));

            return services;
        }
    }
}
