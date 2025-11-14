using System.Reflection;

namespace FELauncher.Host.Tray
{
    public class NotifyIconManager
    {
        private NotifyIcon notifyIcon;

        public NotifyIconManager()
        {
            this.notifyIcon = new NotifyIcon
            {
                Icon = LoadEmbeddedIcon("FELauncher.Host.Assets.win_ico_16.ico"),
                Text = "Test",
                Visible = true
            };

        }

        private Icon LoadEmbeddedIcon(string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            
            using var stream = assembly.GetManifestResourceStream(resourceName);
            return new Icon(stream);
        }

    }
}
