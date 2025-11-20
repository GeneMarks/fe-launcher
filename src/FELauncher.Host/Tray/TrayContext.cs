using System.Reflection;

namespace FELauncher.Host.Tray
{
    public class TrayContext : ApplicationContext
    {
        private NotifyIcon _notifyIcon;
        private ContextMenuStrip _contextMenu;

        private readonly ITrayController _controller;

        public TrayContext(ITrayController controller)
        {
            _controller = controller;

            _contextMenu = new ContextMenuStrip();

            _contextMenu.Items.Clear();
            _contextMenu.Items.Add("Launch Frontend", null, (s, e) => _controller.LaunchFrontend());
            _contextMenu.Items.Add("Options");
            _contextMenu.Items.Add("-");
            _contextMenu.Items.Add("Exit", null, (s, e) => _controller.Exit());

            // _contextMenu.Items[0].Font = new Font(_contextMenu.Items[0].Font, _contextMenu.Items[0].Font.Style | FontStyle.Bold);

            _notifyIcon = new NotifyIcon
            {
                Icon = LoadEmbeddedIcon("FELauncher.Host.Assets.win_ico_16.ico"),
                Text = "FE Launcher",
                Visible = true
            };

            _notifyIcon.MouseDoubleClick += new MouseEventHandler(notifyIcon_DoubleClick);
            _notifyIcon.ContextMenuStrip = _contextMenu;
        }

        private void notifyIcon_DoubleClick(object Sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;

            _controller.LaunchFrontend();
        }

        private Icon LoadEmbeddedIcon(string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            
            using var stream = assembly.GetManifestResourceStream(resourceName);
            return new Icon(stream);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _notifyIcon.Visible = false;
                _notifyIcon.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
