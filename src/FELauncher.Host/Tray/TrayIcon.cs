using System.Drawing;
using System.Reflection;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.Shell;
using Windows.Win32.UI.WindowsAndMessaging;

namespace FELauncher.Host.Tray
{
    internal sealed class TrayIcon : IDisposable
    {
        private readonly HICON _hIcon;
        private readonly NOTIFYICONDATAW _iconData;

        public TrayIcon(HWND hWnd, uint callbackMsg)
        {
            _hIcon = LoadEmbeddedIcon(HostConstants.EmbeddedIcon);

            unsafe
            {
                _iconData.cbSize             = (uint)sizeof(NOTIFYICONDATAW);
                _iconData.hWnd               = hWnd;
                _iconData.uID                = 1;
                _iconData.uCallbackMessage   = callbackMsg;
                _iconData.hIcon              = _hIcon;
                _iconData.szTip              = "FE Launcher";
                _iconData.Anonymous.uVersion = 3; // todo: upgrade to v4?
                _iconData.uFlags             = NOTIFY_ICON_DATA_FLAGS.NIF_MESSAGE
                                             | NOTIFY_ICON_DATA_FLAGS.NIF_ICON
                                             | NOTIFY_ICON_DATA_FLAGS.NIF_TIP;
            }
        }

        private static HICON LoadEmbeddedIcon(string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            using var stream = assembly.GetManifestResourceStream(resourceName);

            Icon icon = new Icon(stream!);

            HICON hicon = (HICON)icon.Handle;
            return PInvoke.CopyIcon(hicon);
        }

        public void AddIcon()
        {
            PInvoke.Shell_NotifyIcon(NOTIFY_ICON_MESSAGE.NIM_ADD, _iconData);
            PInvoke.Shell_NotifyIcon(NOTIFY_ICON_MESSAGE.NIM_SETVERSION, _iconData);
        }

        public void Dispose()
        {
            PInvoke.Shell_NotifyIcon(NOTIFY_ICON_MESSAGE.NIM_DELETE, _iconData);
            PInvoke.DestroyIcon(_hIcon);
        }
    }
}
