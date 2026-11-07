using Cabby.Client.Shared.Contracts.Sessions;

namespace Cabby.Client.Engine.Sessions
{
    internal sealed class SessionStatusUpdater(SessionStatusProvider sessionStatusProvider)
    {
        public void UpdateSession(SessionDto newSession) => sessionStatusProvider.UpdateSession(newSession);

        public void UpdateStatus(SessionStatus newStatus) => sessionStatusProvider.UpdateStatus(newStatus);

        public void ResetState() => sessionStatusProvider.ResetState();
    }
}
