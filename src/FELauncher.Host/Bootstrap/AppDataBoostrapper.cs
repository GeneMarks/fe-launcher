using FELauncher.Engine.Settings;
using FELauncher.Host.Exceptions;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FELauncher.Host.Bootstrap
{
    public static class AppDataBootstrapper
    {
        private static readonly JsonSerializerOptions jsonOptions = new()
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        public static void EnsureAppDataInitialized()
        {
            EnsureAppDataDirectoryExists();
            EnsureSettingsFileExists();
        }

        private static void EnsureAppDataDirectoryExists()
        {
            const string appData = HostPaths.AppDataDirectory;

            static bool AppDataDirectoryExists() => Directory.Exists(appData);

            if (AppDataDirectoryExists()) return;

            try
            {
                _ = Directory.CreateDirectory(appData);
            }
            catch (Exception ex)
            {
                throw new AppDataBootstrapException(
                    $"Failed to create appdata directory '{appData}'.", ex);
            }
        }

        private static void EnsureSettingsFileExists()
        {
            var settingsFile = HostPaths.SettingsFile;

            if (Path.Exists(settingsFile)) return;

            try
            {
                var feLauncherSettings = new FELauncherSettings();

                var settings = new
                {
                    FELauncher = feLauncherSettings
                };

                var serializedSettings = JsonSerializer.Serialize(settings, jsonOptions);

                // Use temporary file in case writing fails
                var tmp = Path.GetTempFileName();
                File.WriteAllText(tmp, serializedSettings);
                File.Move(tmp, settingsFile, overwrite: true);
            }
            catch (Exception ex)
            {
                throw new AppDataBootstrapException(
                    $"Failed to create settings file '{settingsFile}'.", ex);
            }
        }
    }
}