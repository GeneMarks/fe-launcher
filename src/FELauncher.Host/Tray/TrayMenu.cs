using System.Drawing;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;

namespace FELauncher.Host.Tray
{
    internal static class TrayMenu
    {
        public static void ShowMenu(HWND hWnd, ITrayController controller)
        {
            Point pt;
            PInvoke.GetCursorPos(out pt);

            using var menu = PInvoke.CreatePopupMenu_SafeHandle();

            PInvoke.AppendMenu(menu, MENU_ITEM_FLAGS.MF_STRING, 1001, "Launch");
            PInvoke.AppendMenu(menu, MENU_ITEM_FLAGS.MF_SEPARATOR, 0, null);
            PInvoke.AppendMenu(menu, MENU_ITEM_FLAGS.MF_STRING, 1004, "Install Dependencies");
            PInvoke.AppendMenu(menu, MENU_ITEM_FLAGS.MF_STRING, 1002, "Options");
            PInvoke.AppendMenu(menu, MENU_ITEM_FLAGS.MF_SEPARATOR, 0, null);
            PInvoke.AppendMenu(menu, MENU_ITEM_FLAGS.MF_STRING, 1003, "Exit");

            // Foreground must be set to current HWND,
            // otherwise menu doesn't close when clicking outside it.
            PInvoke.SetForegroundWindow(hWnd);

            int choice = PInvoke.TrackPopupMenuEx(
                menu,
                (uint)TRACK_POPUP_MENU_FLAGS.TPM_LEFTALIGN
              | (uint)TRACK_POPUP_MENU_FLAGS.TPM_RIGHTBUTTON
              | (uint)TRACK_POPUP_MENU_FLAGS.TPM_RETURNCMD,
                pt.X,
                pt.Y,
                hWnd,
                null);

            HandleMenuChoice(choice, controller);
        }

        private static void HandleMenuChoice(int choice, ITrayController controller)
        {
            switch (choice)
            {
                // Launch
                case 1001:
                    controller.LaunchFrontend();
                    break;

                // Install Dependencies
                case 1004:
                    break;

                // Options
                case 1002:
                    break;

                // Exit
                case 1003:
                    controller.Exit();
                    break;

                case 0:
                    break;
            }
        }
    }
}
