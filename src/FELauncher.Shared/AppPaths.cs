namespace FELauncher.Shared
{
    public static class AppPaths
    {
        public const string AppDataDirectory                = "felauncher";
        public static readonly string HooksDirectory        = Path.Combine(AppDataDirectory, "hooks");

        public static readonly string SettingsFile          = Path.Combine(AppDataDirectory, "settings.json");
        public static readonly string LogFile               = Path.Combine(AppDataDirectory, "felauncher.log");
    }
}
