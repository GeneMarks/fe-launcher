using Cabby.Client.Shared.Contracts.Configuration;

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
