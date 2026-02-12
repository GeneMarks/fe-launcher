using FELauncher.Engine.IO;
using FELauncher.Host.Exceptions;
using FELauncher.Shared;
using FELauncher.Shared.Contracts.Settings;

namespace FELauncher.Host.Bootstrap
{
    public static class AppDataBootstrapper
    {
        public static void EnsureAppDataInitialized()
        {
            EnsurePathExists(AppPaths.AppDataDirectory);
            EnsurePathExists(AppPaths.HooksDirectory);
            EnsurePathExists(AppPaths.DependenciesDirectory);

            EnsureSettingsFileExists();
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
                JsonSettingsStore.SaveSettings(new FELauncherSettings(), settingsFile);
            }
            catch (Exception ex)
            {
                throw new AppDataBootstrapException($"Failed to create settings file '{settingsFile}'.", ex);
            }
        }
    }
}