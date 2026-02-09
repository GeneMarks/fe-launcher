namespace FELauncher.Shared
{
    public static class AppConstants
    {
        public const string MutexName       = @"Global\925bac46-79de-4db8-8b09-0ba589ce99ee";
        public const long LogFileSizeLimit  = (long)1e+8; // 100mb
        public const int LogFileCountLimit  = 10;
        public const string CheckUpdatesUrl = "https://github.com/GeneMarks/fe-launcher/releases";

        public const string TrayIconResource          = "FELauncher.Shared.Assets.win_ico_16.ico";
    }
}
