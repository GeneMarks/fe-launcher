using FELauncher.Shared.Contracts.UI.Desktop;
using FELauncher.UI.Desktop.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FELauncher.UI.Desktop
{
    public static class DesktopServiceCollectionExtensions
    {
        public static IServiceCollection AddDesktopServices(this IServiceCollection services)
        {
            services.AddSingleton<WpfService>();
            services.AddHostedService(sp => sp.GetRequiredService<WpfService>());

            services.AddSingleton<ISettingsWindowService, SettingsWindowService>();

            return services;
        }
    }
}
