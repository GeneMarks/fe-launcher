using System.Runtime.InteropServices;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;

namespace FELauncher.Host.Tray
{
    internal class TrayWindowHost : IDisposable
    {
        private readonly ITrayController _controller;
        private TrayIcon? _notifyIcon;
        private HWND _hWnd;
        private const uint WM_TRAYICON = 0x800;
        private const string ClassName = "FELauncherTrayWnd";
        private IntPtr _classNamePtr;

        public TrayWindowHost(ITrayController controller)
        {
            _controller = controller;
        }

        /* Must be executed on separate thread.
           Message loop is blocking. */
        public void Run()
        {
            _classNamePtr = Marshal.StringToHGlobalUni(ClassName);
            RegisterClass();
            _hWnd = CreateMessageWindow();

            _notifyIcon = new TrayIcon(_hWnd, WM_TRAYICON);
            _notifyIcon.AddIcon();

            MSG msg;
            while (PInvoke.GetMessage(out msg, HWND.Null, 0, 0)) // Don't use _hWnd so WM_QUIT works
            {
                PInvoke.TranslateMessage(msg);
                PInvoke.DispatchMessage(msg);
            }
        }

        public void Stop()
        {
            if (_hWnd == HWND.Null) return;

            PInvoke.PostMessage(_hWnd, PInvoke.WM_CLOSE, 0, 0);
        }

        private unsafe void RegisterClass()
        {
            WNDCLASSEXW wc = new()
            {
                cbSize        = (uint)sizeof(WNDCLASSEXW),
                lpfnWndProc   = WndProc,
                lpszClassName = new PCWSTR((char*)_classNamePtr)
            };

            PInvoke.RegisterClassEx(wc);
        }
        
        private unsafe HWND CreateMessageWindow()
        {
            return PInvoke.CreateWindowEx(
                0,
                ClassName,
                string.Empty,
                0,
                0, 0, 0, 0,
                HWND.Null,
                null, null, null);
        }

        private LRESULT WndProc(
            HWND hWnd,
            uint uMsg,
            WPARAM wParam,
            LPARAM lParam)
        {
            switch (uMsg)
            {
                case PInvoke.WM_CLOSE:
                    _notifyIcon?.Dispose();
                    _notifyIcon = null;

                    PInvoke.DestroyWindow(hWnd);
                    break;

                case PInvoke.WM_DESTROY:
                    PInvoke.PostQuitMessage(0);
                    break;

                case WM_TRAYICON:
                {
                    uint code = (uint)lParam.Value;

                    switch (code)
                    {
                        case PInvoke.WM_LBUTTONDBLCLK:
                            _controller.LaunchFrontend();
                            break;

                        case PInvoke.WM_RBUTTONUP:
                            TrayMenu.ShowMenu(hWnd, _controller);
                            break;
                    }
                    break;
                }
            }

            return PInvoke.DefWindowProc(hWnd, uMsg, wParam, lParam);
        }

        public void Dispose()
        {
            if (_classNamePtr !=  IntPtr.Zero)
            {
                Marshal.FreeHGlobal(_classNamePtr);
                _classNamePtr = IntPtr.Zero;
            }
        }
    }
}
