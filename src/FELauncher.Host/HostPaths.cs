using System.IO;

namespace FELauncher.Host
{
    internal static class HostPaths
    {
        public const string AppDataDirectory       = "felauncher";
        public static readonly string SettingsFile = Path.Combine(AppDataDirectory, "felauncher.json");
        public static readonly string LogFile      = Path.Combine(AppDataDirectory, "felauncher.log");
    }
}
