using FELauncher.Engine.Processes;
using FELauncher.Engine.Settings;
using Microsoft.Extensions.Options;
using System.Reflection;

namespace FELauncher.Host.Tray
{
    public class TrayManager
    {
        private NotifyIcon _notifyIcon;
        private readonly IProcessManager _processManager;
        private readonly FrontendSettings _frontendSettings;

        public TrayManager(IProcessManager processManager, IOptionsMonitor<FrontendSettings> frontendSettings)
        {
            _processManager = processManager;
            _frontendSettings = frontendSettings.CurrentValue;

            _notifyIcon = new NotifyIcon
            {
                Icon = LoadEmbeddedIcon("FELauncher.Host.Assets.win_ico_16.ico"),
                Text = "FE Launcher",
                Visible = true
            };

            _notifyIcon.MouseDoubleClick += new MouseEventHandler(notifyIcon_DoubleClick);
        }

        private void notifyIcon_DoubleClick(object Sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;

            _processManager.StartProcess(_frontendSettings.FrontendPath);
        }

        private Icon LoadEmbeddedIcon(string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            
            using var stream = assembly.GetManifestResourceStream(resourceName);
            return new Icon(stream);
        }

    }
}
