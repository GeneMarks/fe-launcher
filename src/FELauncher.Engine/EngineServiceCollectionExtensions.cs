using FELauncher.Engine.IO;
using FELauncher.Engine.Processes;
using FELauncher.Engine.Sessions;
using FELauncher.Engine.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FELauncher.Engine
{
    public static class EngineServiceCollectionExtensions
    {
        public static IServiceCollection AddEngineServices(this IServiceCollection services)
        {
            // Public services
            services.AddSingleton<ISessionManager, SessionManager>();

            // Internal services
            services.AddSingleton<PathResolver>();
            services.AddSingleton<Win32ProcessFactory>();
            services.AddSingleton<FELProcessManager>();
            services.AddSingleton<JobObjectManager>();
            services.AddSingleton<PreProcessRunner>();
            services.AddSingleton<FrontendRunner>();

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
