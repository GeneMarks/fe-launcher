using System.Drawing;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;

namespace FELauncher.UI.Shell.Tray
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

        public static void ShowMenu(HWND windowHandle, TrayActionHandler handler)
        {
            Point pt;
            PInvoke.GetCursorPos(out pt);

            using var menu = PInvoke.CreatePopupMenu_SafeHandle();

            PopulateMenu(menu, handler.IsSessionActive, handler.CanEndSession);

            // Foreground must be set to current HWND,
            // otherwise menu doesn't close when clicking outside it.
            _ = PInvoke.SetForegroundWindow(windowHandle);

            int choice = PInvoke.TrackPopupMenuEx(
                menu,
                (uint)TRACK_POPUP_MENU_FLAGS.TPM_LEFTALIGN
              | (uint)TRACK_POPUP_MENU_FLAGS.TPM_RIGHTBUTTON
              | (uint)TRACK_POPUP_MENU_FLAGS.TPM_RETURNCMD,
                pt.X,
                pt.Y,
                windowHandle,
                null);

            HandleMenuChoice(choice, handler);
        }

        private static void PopulateMenu(
            DestroyMenuSafeHandle menu,
            bool isSessionActive,
            bool canEndSession)
        {
            AppendMenuItem(menu,
                isSessionActive ? Items.EndSession : Items.Launch,
                isSessionActive ? "End session" : "Launch",
                disabled: isSessionActive && !canEndSession);
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
            _ = PInvoke.AppendMenu( menu, flags, id, label);
        }
        
        private static void AppendMenuItem(DestroyMenuSafeHandle menu)
        {
            _ = PInvoke.AppendMenu(menu, MENU_ITEM_FLAGS.MF_SEPARATOR, 0, null);
        }

        private static void HandleMenuChoice(int choice, TrayActionHandler handler)
        {
            switch ((nuint)choice)
            {
                case Items.Launch:
                    handler.LaunchFrontend();
                    break;

                case Items.EndSession:
                    handler.EndSession();
                    break;

                case Items.InstallDependencies:
                    break;

                case Items.Options:
                    break;

                case Items.CheckUpdates:
                    TrayActionHandler.CheckUpdates();
                    break;

                case Items.Exit:
                    handler.Exit();
                    break;

                default:
                    break;
            }
        }
    }
}
