using FELauncher.Shared.Contracts.UI.Desktop;
using FELauncher.Shared.Contracts.UI.Desktop.Windows;
using FELauncher.UI.Desktop.Services.Infrastructure;
using FELauncher.UI.Desktop.Services.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace FELauncher.UI.Desktop
{
    public static class DesktopServiceCollectionExtensions
    {
        public static IServiceCollection AddDesktopServices(this IServiceCollection services)
        {
            // Public services
            services.AddSingleton<ISettingsWindowService, SettingsWindowService>();

            // Internal services
            services.AddSingleton<WpfService>();
            services.AddHostedService(sp => sp.GetRequiredService<WpfService>());
            services.AddSingleton<IUiDispatcher, WpfDispatcher>();

            return services;
        }
    }
}
