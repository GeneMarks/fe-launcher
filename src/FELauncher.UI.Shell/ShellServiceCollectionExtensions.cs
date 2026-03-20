using FELauncher.Shared.Contracts.UI;
using FELauncher.UI.Shell.Notifications;
using FELauncher.UI.Shell.Tray;
using Microsoft.Extensions.DependencyInjection;

namespace FELauncher.UI.Shell
{
    public static class ShellServiceCollectionExtensions
    {
        public static IServiceCollection AddShellServices(this IServiceCollection services)
        {
            services.AddSingleton<TrayActionHandler>();
            services.AddHostedService<TrayService>();
            services.AddSingleton<INotifier, Notifier>();

            return services;
        }
    }
}
