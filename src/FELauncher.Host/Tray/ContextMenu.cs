using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;

namespace FELauncher.Host.Tray
{
    public class ContextMenu
    {
        private HWND _hWnd;

        public  ContextMenu()
        {
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
            }
        }

        public void ShowMenu()
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
            PInvoke.SetForegroundWindow(_hWnd);

            int choice = PInvoke.TrackPopupMenu(
                menu,
                TRACK_POPUP_MENU_FLAGS.TPM_LEFTALIGN
                | TRACK_POPUP_MENU_FLAGS.TPM_RIGHTBUTTON
                | TRACK_POPUP_MENU_FLAGS.TPM_RETURNCMD,
                pt.X,
                pt.Y,
                _hWnd,
                null);

            HandleMenuChoice(choice);
        }

        private void HandleMenuChoice(int choice)
        {
            switch (choice)
            {
                case 1001:
                    break;

                case 1002:
                    break;

                case 1003:
                    break;

                case 0:
                    break;
            }
        }
    }
}
