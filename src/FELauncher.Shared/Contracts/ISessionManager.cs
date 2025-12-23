namespace FELauncher.Shared.Contracts
{
    public interface ISessionManager
    {
        bool IsSessionActive { get; }
        bool CanEndSession { get; }

        /// <summary>
        /// Creates and runs a new session if an active one isn't already being managed.
        /// </summary>
        Task StartNewSessionAsync();

        /// <summary>
        /// Requests cancellation from the cancellation token source if the session is in a stoppable state.
        /// </summary>
        void RequestEndSession();
    }
}
