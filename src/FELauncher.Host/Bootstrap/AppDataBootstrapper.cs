using FELauncher.Engine.Settings;
using FELauncher.Host.Exceptions;
using FELauncher.Shared;
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
            EnsurePathExists(AppPaths.AppDataDirectory);
            EnsurePathExists(AppPaths.AssetsDirectory);
            EnsurePathExists(AppPaths.HooksDirectory);
            EnsurePathExists(AppPaths.DependenciesDirectory);

            EnsureSettingsFileExists();
            EnsureNotificationImageFileExists();
        }

        private static void EnsurePathExists(string path)
        {
            if (Directory.Exists(path)) return;

            try
            {
                _ = Directory.CreateDirectory(path);
            }
            catch (Exception ex)
            {
                throw new AppDataBootstrapException($"Failed to create directory '{path}'.", ex);
            }
        }

        private static void EnsureSettingsFileExists()
        {
            var settingsFile = AppPaths.SettingsFile;

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
                throw new AppDataBootstrapException($"Failed to create settings file '{settingsFile}'.", ex);
            }
        }

        private static void EnsureNotificationImageFileExists()
        {
            var imageFile = AppPaths.NotificationImageFile;

            try
            {
                if (File.Exists(imageFile)) return;

                var assembly = typeof(AppConstants).Assembly;
                using var resource = assembly.GetManifestResourceStream(AppConstants.NotificationImageResource);
                using var file = new FileStream(imageFile, FileMode.Create, FileAccess.Write);
                resource!.CopyTo(file);
            }
            catch (Exception ex)
            {
                throw new AppDataBootstrapException($"Failed to create asset file '{imageFile}'.", ex);
            }
        }
    }
}