using FELauncher.Engine.Settings;
using System.IO;
using System.Text.Json;

namespace FELauncher.Host.Bootstrap
{
    public static class FELauncherSettingsBootstrapper
    {
        private static readonly JsonSerializerOptions jsonOptions = new()
        {
            WriteIndented = true
        };

        public static void EnsureSettingsFileExists(string settingsFile)
        {
            if (Path.Exists(settingsFile)) return;

            var feLauncherSettings = new FELauncherSettings();

            var settings = new
            {
                FELauncher = feLauncherSettings
            };

            var serializedSettings = JsonSerializer.Serialize(settings, jsonOptions);

            // todo: error handling
            try
            {
                File.WriteAllText(settingsFile, serializedSettings);
            }
            catch
            {

            }
        }
    }
}