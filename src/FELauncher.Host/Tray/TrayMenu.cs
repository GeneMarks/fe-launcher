using System.Drawing;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;

namespace FELauncher.Host.Tray
{
    internal static class TrayMenu
    {
        private struct Items
        {
            public const nuint Launch              = 1000;
            public const nuint EndSession          = 1010;
            public const nuint InstallDependencies = 1020;
            public const nuint Options             = 1030;
            public const nuint CheckUpdates        = 1040;
            public const nuint Exit                = 1050;
        }

        public static void ShowMenu(HWND hWnd, TrayController controller)
        {
            Point pt;
            PInvoke.GetCursorPos(out pt);

            using var menu = PInvoke.CreatePopupMenu_SafeHandle();

            PopulateMenu(menu, controller.IsSessionActive);

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

        private static void PopulateMenu(DestroyMenuSafeHandle menu, bool isSessionActive)
        {
            AppendMenuItem(menu,
                isSessionActive ? Items.EndSession : Items.Launch,
                isSessionActive ? "End session" : "Launch");
            AppendMenuItem(menu);
            AppendMenuItem(menu, Items.InstallDependencies, "Install dependencies", isSessionActive);
            AppendMenuItem(menu, Items.Options, "Options", isSessionActive);
            AppendMenuItem(menu);
            AppendMenuItem(menu, Items.CheckUpdates, "Check updates", false);
            AppendMenuItem(menu);
            AppendMenuItem(menu, Items.Exit, "Exit", isSessionActive);
        }
        
        private static void AppendMenuItem(DestroyMenuSafeHandle menu,
            nuint id, string label, bool disabled = false)
        {
            var flags = MENU_ITEM_FLAGS.MF_STRING | (disabled ? MENU_ITEM_FLAGS.MF_DISABLED : 0);
            PInvoke.AppendMenu( menu, flags, id, label);
        }
        
        private static void AppendMenuItem(DestroyMenuSafeHandle menu)
        {
            PInvoke.AppendMenu(menu, MENU_ITEM_FLAGS.MF_SEPARATOR, 0, null);
        }

        private static void HandleMenuChoice(int choice, TrayController controller)
        {
            switch ((nuint)choice)
            {
                case Items.Launch:
                    controller.LaunchFrontend();
                    break;

                case Items.EndSession:
                    controller.EndSession();
                    break;

                case Items.InstallDependencies:
                    break;

                case Items.Options:
                    break;

                case Items.CheckUpdates:
                    TrayController.CheckUpdates();
                    break;

                case Items.Exit:
                    controller.Exit();
                    break;

                default:
                    break;
            }
        }
    }
}
