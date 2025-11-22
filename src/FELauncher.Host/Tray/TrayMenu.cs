using System.Drawing;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;

namespace FELauncher.Host.Tray
{
    internal class TrayMenu
    {
        private ITrayController _trayController;

        public TrayMenu(ITrayController trayController)
        {
            _trayController = trayController;
        }

        public void ShowMenu(HWND hWnd)
        {
            Point pt;
            PInvoke.GetCursorPos(out pt);

            using var menu = PInvoke.CreatePopupMenu_SafeHandle();

            PInvoke.AppendMenu(menu, MENU_ITEM_FLAGS.MF_STRING, 1001, "Launch");
            PInvoke.AppendMenu(menu, MENU_ITEM_FLAGS.MF_STRING, 1002, "Options");
            PInvoke.AppendMenu(menu, MENU_ITEM_FLAGS.MF_SEPARATOR, 0, null);
            PInvoke.AppendMenu(menu, MENU_ITEM_FLAGS.MF_STRING, 1003, "Exit");

            // Foreground must be set to menu's HWND,
            // otherwise menu doesn't close when clicking outside it.
            PInvoke.SetForegroundWindow(hWnd);

            int choice = PInvoke.TrackPopupMenu(
                menu,
                TRACK_POPUP_MENU_FLAGS.TPM_LEFTALIGN
                | TRACK_POPUP_MENU_FLAGS.TPM_RIGHTBUTTON
                | TRACK_POPUP_MENU_FLAGS.TPM_RETURNCMD,
                pt.X,
                pt.Y,
                hWnd,
                null);

            HandleMenuChoice(choice);
        }

        private void HandleMenuChoice(int choice)
        {
            switch (choice)
            {
                // Launch
                case 1001:
                    _trayController.LaunchFrontend();
                    break;

                // Options
                case 1002:
                    _trayController.OpenSettings();
                    break;

                // Exit
                case 1003:
                    _trayController.Exit();
                    break;

                case 0:
                    break;
            }
        }
    }
}
