using System.Runtime.InteropServices;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;

namespace FELauncher.UI.Shell.Tray
{
    internal sealed class TrayWindowHost(TrayActionHandler handler) : IDisposable
    {
        private TrayIcon? _notifyIcon;
        private HWND _windowHandle;
        private WNDPROC? _wndProc;
        private const uint WM_TRAYICON = 0x800;
        private const string ClassName = "FELauncherTrayWnd";
        private IntPtr _classNamePtr;

        // Must be executed on separate thread.
        // Message loop is blocking.
        public void Run()
        {
            _classNamePtr = Marshal.StringToHGlobalUni(ClassName);
            RegisterClass();
            _windowHandle = CreateMessageWindow();

            _notifyIcon = new TrayIcon(_windowHandle, WM_TRAYICON);
            _notifyIcon.AddIcon();

            MSG msg;
            while (PInvoke.GetMessage(out msg, HWND.Null, 0, 0)) // Don't use _windowHandle so WM_QUIT works
            {
                PInvoke.TranslateMessage(msg);
                PInvoke.DispatchMessage(msg);
            }
        }

        public void Stop()
        {
            if (_windowHandle == HWND.Null) return;

            _ = PInvoke.PostMessage(_windowHandle, PInvoke.WM_CLOSE, 0, 0);
        }

        private unsafe void RegisterClass()
        {
            _wndProc = WndProc;

            WNDCLASSEXW wc = new()
            {
                cbSize        = (uint)sizeof(WNDCLASSEXW),
                lpfnWndProc   = _wndProc,
                lpszClassName = new PCWSTR((char*)_classNamePtr)
            };

            _ = PInvoke.RegisterClassEx(wc);
        }
        
        private static unsafe HWND CreateMessageWindow()
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
            HWND windowHandle,
            uint message,
            WPARAM wParam,
            LPARAM lParam)
        {
            switch (message)
            {
                case PInvoke.WM_CLOSE:
                    _notifyIcon?.Dispose();
                    _notifyIcon = null;

                    _ = PInvoke.DestroyWindow(windowHandle);
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
                            handler.LaunchFrontend();
                            break;

                        case PInvoke.WM_RBUTTONUP:
                            TrayMenu.ShowMenu(windowHandle, handler);
                            break;
                    }
                    break;
                }
            }

            return PInvoke.DefWindowProc(windowHandle, message, wParam, lParam);
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
