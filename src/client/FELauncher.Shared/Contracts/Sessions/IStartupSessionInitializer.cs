namespace FELauncher.Shared.Contracts.Sessions
{
    public interface IStartupSessionInitializer
    {
        /// <summary>
        /// Checks if <see cref="Settings.FELauncherSettings.AutoLaunchSession"/> is true and starts a new session accordingly.
        /// </summary>
        void LaunchStartupSessionIfEnabled();
    }
}
