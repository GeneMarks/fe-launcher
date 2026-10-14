using Windows.Win32;
using Windows.Win32.UI.Controls;

namespace FELauncher.UI.Shell.TaskDialog
{
    public static class ErrorDialog
    {
        public static unsafe void ShowFatal(string title, string content, Exception? ex = null)
        {
            string? details = ex?.ToString();

            fixed (char* pTitle            = title)
            fixed (char* pMain             = "A fatal error occurred")
            fixed (char* pContent          = content)
            fixed (char* pDetails          = details)
            fixed (char* pCollapsedControl = "Show details")
            fixed (char* pExpandedControl  = "Hide details")
            {
                TASKDIALOGCONFIG config = new()
                {
                    cbSize                  = (uint)sizeof(TASKDIALOGCONFIG),
                    pszWindowTitle          = pTitle,
                    pszMainInstruction      = pMain,
                    pszContent              = pContent,
                    pszExpandedInformation  = details is null ? null : pDetails,
                    pszCollapsedControlText = pCollapsedControl,
                    pszExpandedControlText  = pExpandedControl,
                    cxWidth                 = 0,
                    dwCommonButtons         = TASKDIALOG_COMMON_BUTTON_FLAGS.TDCBF_CLOSE_BUTTON,
                    dwFlags                 = TASKDIALOG_FLAGS.TDF_ENABLE_HYPERLINKS
                                            | TASKDIALOG_FLAGS.TDF_ALLOW_DIALOG_CANCELLATION
                                            | TASKDIALOG_FLAGS.TDF_SIZE_TO_CONTENT
                };
                config.Anonymous1.pszMainIcon = PInvoke.TD_ERROR_ICON;

                _ = PInvoke.TaskDialogIndirect(in config, out _, out _, out _);
            }
        }
    }
}
