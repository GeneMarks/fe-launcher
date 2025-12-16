namespace FELauncher.Engine.Sessions
{
    public interface ISessionManager
    {
        bool IsSessionActive { get; }

        /// <summary>
        /// Returns the CancellationTokenSource for the active session, or
        /// a new instance if the session is null or inactive.
        /// </summary>
        CancellationTokenSource CancellationTokenSource { get; }

        /// <summary>
        /// Creates and runs a new session if an active one isn't already being managed.
        /// </summary>
        Task StartNewSessionAsync(CancellationToken ct = default);
    }
}
