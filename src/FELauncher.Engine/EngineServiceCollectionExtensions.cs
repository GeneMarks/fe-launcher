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
            services.AddSingleton<IPathResolver, PathResolver>();
            services.AddSingleton<IProcessFactory, ProcessFactory>();
            services.AddSingleton<IProcessManager, ProcessManager>();
            services.AddSingleton<IJobObjectManager, JobObjectManager>();
            services.AddSingleton<IPreProcessRunner, PreProcessRunner>();
            services.AddSingleton<IFrontendRunner, FrontendRunner>();

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
