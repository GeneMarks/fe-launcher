namespace FELauncher.Shared.Contracts
{
    public interface ISessionOrchestrator
    {
        bool IsSessionActive { get; }
        bool CanEndSession { get; }

        /// <summary>
        /// Creates and runs a new session if an active one isn't already being orchestrated.
        /// </summary>
        Task StartNewSessionAsync();

        /// <summary>
        /// Requests cancellation from the cancellation token source if the session is in a cancelable state.
        /// </summary>
        void RequestEndSession();
    }
}
