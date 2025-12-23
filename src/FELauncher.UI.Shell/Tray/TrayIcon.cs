using FELauncher.Shared;
using System.Drawing;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.Shell;
using Windows.Win32.UI.WindowsAndMessaging;

namespace FELauncher.UI.Shell.Tray
{
    internal sealed class TrayIcon : IDisposable
    {
        private readonly HICON _hIcon;
        private readonly NOTIFYICONDATAW _iconData;

        public TrayIcon(HWND windowHandle, uint callbackMsg)
        {
            _hIcon = LoadEmbeddedIcon(AppConstants.TrayIconResource);

            unsafe
            {
                _iconData.cbSize             = (uint)sizeof(NOTIFYICONDATAW);
                _iconData.hWnd               = windowHandle;
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
            var assembly = typeof(AppConstants).Assembly;
            using var resource = assembly.GetManifestResourceStream(resourceName);

            Icon icon = new Icon(resource!);

            HICON hicon = (HICON)icon.Handle;
            return PInvoke.CopyIcon(hicon);
        }

        public void AddIcon()
        {
            _ = PInvoke.Shell_NotifyIcon(NOTIFY_ICON_MESSAGE.NIM_ADD, _iconData);
            _ = PInvoke.Shell_NotifyIcon(NOTIFY_ICON_MESSAGE.NIM_SETVERSION, _iconData);
        }

        public void Dispose()
        {
            _ = PInvoke.Shell_NotifyIcon(NOTIFY_ICON_MESSAGE.NIM_DELETE, _iconData);
            _ = PInvoke.DestroyIcon(_hIcon);
        }
    }
}
