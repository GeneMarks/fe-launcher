using System.Drawing;
using System.Reflection;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.Shell;
using Windows.Win32.UI.WindowsAndMessaging;

namespace FELauncher.Host.Tray
{
    internal class TrayIcon
    {
        static readonly uint WM_TRAYICON = 0x800 + 1;

        private Guid _guid;
        private HWND _hWnd;
        private ITrayController _trayController;
        private TrayMenu _trayMenu;

        public TrayIcon(ITrayController trayController, TrayMenu trayMenu)
        {
            _trayController = trayController;
            _trayMenu = trayMenu;

            _guid = Guid.NewGuid();

            unsafe
            {
                _hWnd = PInvoke.CreateWindowEx(
                    0,
                    "Static",
                    "",
                    WINDOW_STYLE.WS_POPUP,
                    0, 0, 0, 0,
                    HWND.Null,
                    null,
                    null,
                    null);

                PInvoke.SetWindowSubclass(_hWnd, TrayIconSubclassProc, 1, 0);
            }
        }

        unsafe public bool AddNotifyIcon()
        {
            NOTIFYICONDATAW nid = new NOTIFYICONDATAW();

            nid.cbSize = (uint)sizeof(NOTIFYICONDATAW);
            nid.hWnd = _hWnd;
            nid.uCallbackMessage = WM_TRAYICON;
            nid.guidItem = _guid;
            nid.hIcon = LoadEmbeddedIcon("FELauncher.Host.Assets.win_ico_16.ico");
            nid.szTip = "FE Launcher";
            nid.uFlags = NOTIFY_ICON_DATA_FLAGS.NIF_MESSAGE
                       | NOTIFY_ICON_DATA_FLAGS.NIF_ICON
                       | NOTIFY_ICON_DATA_FLAGS.NIF_TIP
                       | NOTIFY_ICON_DATA_FLAGS.NIF_GUID
                       | NOTIFY_ICON_DATA_FLAGS.NIF_SHOWTIP;

            PInvoke.Shell_NotifyIcon(NOTIFY_ICON_MESSAGE.NIM_ADD, &nid);

            nid.Anonymous.uVersion = 4;
            return PInvoke.Shell_NotifyIcon(NOTIFY_ICON_MESSAGE.NIM_SETVERSION, &nid);
        }

        unsafe private LRESULT TrayIconSubclassProc(
            HWND hWnd,
            uint uMsg,
            WPARAM wParam,
            LPARAM lParam,
            nuint uIdSubclass,
            nuint dwRefData)
        {
            if (uMsg == WM_TRAYICON)
            {
                uint eventId = (uint)lParam.Value;
                switch (eventId)
                {
                    case PInvoke.WM_LBUTTONDBLCLK:
                        _trayController.LaunchFrontend();
                        break;

                    case PInvoke.WM_RBUTTONUP:
                        _trayMenu.ShowMenu(_hWnd);
                        break;
                }
            }

            return PInvoke.DefSubclassProc(hWnd, uMsg, wParam, lParam);
        }

        unsafe private HICON LoadEmbeddedIcon(string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            using var stream = assembly.GetManifestResourceStream(resourceName);

            Icon icon = new Icon(stream);

            IntPtr hicon = icon.Handle;
            return (HICON)hicon;
        }
    }
}
