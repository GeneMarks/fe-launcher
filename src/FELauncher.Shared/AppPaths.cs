namespace FELauncher.Shared
{
    public static class AppPaths
    {
        public const string AppDataDirectory                = "felauncher";
        public static readonly string AssetsDirectory       = Path.Combine(AppDataDirectory, "assets");
        public static readonly string HooksDirectory        = Path.Combine(AppDataDirectory, "hooks");
        public static readonly string DependenciesDirectory = Path.Combine(AppDataDirectory, "dependencies");

        public static readonly string SettingsFile          = Path.Combine(AppDataDirectory, "settings.json");
        public static readonly string LogFile               = Path.Combine(AppDataDirectory, "felauncher.log");
        public static readonly string NotificationImageFile = Path.Combine(AssetsDirectory, "cade_notify.png");
    }
}
