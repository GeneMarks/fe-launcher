using FELauncher.Shared;
using FELauncher.Shared.Contracts.Engine;
using Microsoft.Toolkit.Uwp.Notifications;

namespace FELauncher.UI.Shell.Notifications
{
    internal sealed class Notifier : INotifier
    {
        public void Notify(string title, string body)
        {
            new ToastContentBuilder()
                .AddText(title)
                .AddText(body)
                .Show();
        }
    }
}
