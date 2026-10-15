using Cabby.Client.Shared.Contracts.Settings;

namespace Cabby.Client.Shared.Contracts.Sessions
{
    public interface IStartupSessionInitializer
    {
        /// <summary>
        /// Checks if <see cref="CabbySettings.AutoLaunchSession"/> is true and starts a new session accordingly.
        /// </summary>
        void LaunchStartupSessionIfEnabled();
    }
}
