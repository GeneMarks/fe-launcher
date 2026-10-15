using Cabby.Client.Shared.Contracts.UI;
using Cabby.Client.Shell.Notifications;
using Cabby.Client.Shell.Tray;
using Microsoft.Extensions.DependencyInjection;

namespace Cabby.Client.Shell
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
