using FELauncher.Engine.Settings;
using System.IO;
using System.Text.Json;

namespace FELauncher.Host.Bootstrap
{
    public static class FELauncherSettingsBootstrapper
    {
        public static void EnsureSettingsFileExists(string settingsFile)
        {
            if (Path.Exists(settingsFile)) return;

            var bootstrapSettings = new FELauncherSettings();

            var serializedSettings = JsonSerializer.Serialize(bootstrapSettings, new JsonSerializerOptions
            {
                WriteIndented = true
            });

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