using FELauncher.Engine.Processes;
using System.Reflection;

namespace FELauncher.Host.Tray
{
    public class NotifyIconManager
    {
        private NotifyIcon _notifyIcon;
        private readonly IProcessManager _processManager;

        public NotifyIconManager(IProcessManager processManager)
        {
            _processManager = processManager;

            _notifyIcon = new NotifyIcon
            {
                Icon = LoadEmbeddedIcon("FELauncher.Host.Assets.win_ico_16.ico"),
                Text = "Test",
                Visible = true
            };

            _notifyIcon.Click += new EventHandler(notifyIcon_Click);

        }

        private void notifyIcon_Click(object Sender, EventArgs e)
        {
            _processManager.StartProcess("notepad.exe");
        }

        private Icon LoadEmbeddedIcon(string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            
            using var stream = assembly.GetManifestResourceStream(resourceName);
            return new Icon(stream);
        }

    }
}
