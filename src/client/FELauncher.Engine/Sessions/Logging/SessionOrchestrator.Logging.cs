using FELauncher.Shared.Contracts.Sessions;
using Microsoft.Extensions.Logging;

namespace FELauncher.Engine.Sessions.Logging
{
    internal static partial class SessionOrchestratorLogging
    {
        [LoggerMessage(
            Level = LogLevel.Error,
            Message = "An unexpected ({ExceptionType}) error has occurred. Ending the current session...")]
        public static partial void FatalSessionError(this ILogger logger, string exceptionType);

        [LoggerMessage(
            Level = LogLevel.Error,
            Message = "An unexpected error has occurred. Ending the current session...")]
        public static partial void UnknownFatalSessionError(this ILogger logger, Exception? ex = null);

        [LoggerMessage(
            Level = LogLevel.Error,
            Message = "Abandoning session due to an unexpected job object error that occurred during session shutdown.")]
        public static partial void AbandoningSessionDueToJobObjectError(this ILogger logger, Exception? ex = null);

        [LoggerMessage(
            Level = LogLevel.Warning,
            Message = "Tried to stop null session object. Returning.")]
        public static partial void TriedToStopNullSession(this ILogger logger);

        [LoggerMessage(
            Level = LogLevel.Information,
            Message = "Starting new session with ID {SessionId}.")]
        public static partial void StartingNewSession(this ILogger logger, string sessionId);

        [LoggerMessage(
            Level = LogLevel.Information,
            Message = "Running ({Count}) pre-processes...")]
        public static partial void RunningPreProcesses(this ILogger logger, int count);

        [LoggerMessage(
            Level = LogLevel.Information,
            Message = "Launching frontend...")]
        public static partial void StartingFrontend(this ILogger logger);

        [LoggerMessage(
            Level = LogLevel.Information,
            Message = "Waiting for session completion...")]
        public static partial void WaitingForSessionCompletion(this ILogger logger);

        [LoggerMessage(
            Level = LogLevel.Information,
            Message = "Stopping the current session...")]
        public static partial void StoppingSession(this ILogger logger);

        [LoggerMessage(
            Level = LogLevel.Information,
            Message = "Attempting to gracefully close all process windows...")]
        public static partial void AttemptingToCloseProcessWindows(this ILogger logger);

        [LoggerMessage(
            Level = LogLevel.Information,
            Message = "Forcefully terminating remaining processes...")]
        public static partial void TerminatingProcesses(this ILogger logger);

        [LoggerMessage(
            Level = LogLevel.Information,
            Message = "Session {SessionId} ended. (Status: {CompletionStatus}, Total runtime: {Runtime})")]
        public static partial void SessionFinalRuntime(this ILogger logger, string sessionId, SessionStatus completionStatus, string runtime);

        [LoggerMessage(
            Level = LogLevel.Debug,
            Message = "Cannot start another session when one is already active.")]
        public static partial void CannotStartMultipleSessions(this ILogger logger);
    }
}
