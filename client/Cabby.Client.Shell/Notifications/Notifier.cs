using Cabby.Client.Shared.Contracts.UI;
using Microsoft.Toolkit.Uwp.Notifications;

namespace Cabby.Client.Shell.Notifications
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
