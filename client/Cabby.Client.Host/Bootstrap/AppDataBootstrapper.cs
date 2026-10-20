using Cabby.Client.Shared;
using Cabby.Client.Host.Exceptions;
using Cabby.Client.Shared.Contracts.Configuration;
using Cabby.Client.Engine.Configuration;

namespace Cabby.Client.Host.Bootstrap
{
    public static class AppDataBootstrapper
    {
        public static void EnsureAppDataInitialized()
        {
            EnsurePathExists(AppPaths.AppDataDirectory);
            EnsurePathExists(AppPaths.HooksDirectory);

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
                JsonSettingsStore.SaveSettings(new CabbySettings(), settingsFile);
            }
            catch (Exception ex)
            {
                throw new AppDataBootstrapException($"Failed to create settings file '{settingsFile}'.", ex);
            }
        }
    }
}