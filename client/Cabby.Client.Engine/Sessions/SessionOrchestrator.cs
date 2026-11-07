using Cabby.Client.Engine.Exceptions;
using Cabby.Client.Engine.Processes;
using Cabby.Client.Engine.Sessions.Logging;
using Cabby.Client.Shared.Contracts.Processes;
using Cabby.Client.Shared.Contracts.Sessions;
using Microsoft.Extensions.Logging;

namespace Cabby.Client.Engine.Sessions
{
    internal sealed class SessionOrchestrator(
        ILogger<SessionOrchestrator> logger,
        SessionStatusProvider sessionStatusProvider,
        SessionStatusUpdater sessionStatusUpdater,
        SessionLoggerScopeProvider sessionLoggerScopeProvider,
        JobObjectManager jobObjectManager,
        ProcessDispatcher processDispatcher)
    {
        private SessionDto? _currentSession;
        private CancellationTokenSource? _sessionCts;

        public async Task StartNewSessionAsync(SessionDto sessionDto)
        {
            if (!sessionStatusProvider.IsIdle)
            {
                logger.CannotStartMultipleSessions();
                // TODO: Notify server
                return;
            }

            _currentSession = sessionDto;
            logger.StartingNewSession(_currentSession.Id);
            sessionStatusUpdater.UpdateSession(_currentSession);
            sessionStatusUpdater.UpdateStatus(SessionStatus.Pending);

            _sessionCts = new CancellationTokenSource();
            var sessionCt = _sessionCts.Token;

            sessionLoggerScopeProvider.SetCurrentSessionId(_currentSession.Id);
            using var sessionScope = sessionLoggerScopeProvider.BeginSessionScope(logger);

            try
            {
                jobObjectManager.ResetJobObject();

                /* Pre-Processes */
                sessionStatusUpdater.UpdateStatus(SessionStatus.RunningPreProcesses);
                logger.RunningPreProcesses(_currentSession.PreProcesses.Length);

                foreach (var preProcess in _currentSession.PreProcesses)
                {
                    await processDispatcher.DispatchAsync(preProcess, OnProcessExited, sessionCt);
                }

                /* Frontend */
                sessionStatusUpdater.UpdateStatus(SessionStatus.RunningFrontend);
                logger.StartingFrontend();

                await processDispatcher.DispatchAsync(_currentSession.MainProcess, OnProcessExited, sessionCt);

                logger.WaitingForSessionCompletion();
                await jobObjectManager.WaitForJobObjectCompletionAsync(sessionCt);

                /* Post-Processes */
                sessionStatusUpdater.UpdateStatus(SessionStatus.RunningPostProcesses);
                logger.RunningPostProcesses(_currentSession.PostProcesses.Length);

                foreach (var postProcess in _currentSession.PostProcesses)
                {
                    await processDispatcher.DispatchAsync(postProcess, OnProcessExited, sessionCt);
                }

                // TODO: Await the job object completion again for post-processes?

                /* Completion */
                sessionStatusUpdater.UpdateStatus(SessionStatus.Completed);
            }
            catch (OperationCanceledException) when (sessionCt.IsCancellationRequested)
            {
                logger.StoppingSession();
                await StopSessionAsync(SessionStatus.Aborted);
            }
            catch (WinApiException ex)
            {
                logger.FatalSessionError(ex.GetType().Name);
                // notifier.Notify("Unexpected Error", "Ending the current session. Please see log for more details.");
                // TODO: Notify server
                await StopSessionAsync(SessionStatus.Failed);
            }
            catch (Exception ex)
            {
                logger.UnknownFatalSessionError(ex);
                // notifier.Notify("Unexpected Error", "Ending the current session. Please see log for more details.");
                // TODO: Notify server

                await StopSessionAsync(SessionStatus.Failed);
            }
            finally
            {
                var runtime = sessionStatusProvider.ElapsedTime;
                var prettyRuntime = $"{(int)runtime.TotalHours} hrs {runtime.Minutes:00} min {runtime.Seconds:00} sec";
                // TODO: Don't get status from the SessionDto?
                logger.SessionFinalRuntime(_currentSession.Id, _currentSession.Status, prettyRuntime);

                // TODO: Notify server

                sessionStatusUpdater.ResetState();
                _sessionCts?.Dispose();
                _sessionCts = null;
                _currentSession = null;
            }
        }

        public void RequestCancelSession()
        {
            CancellationTokenSource? cts = null;

            if (sessionStatusProvider.IsActive)
            {
                cts = _sessionCts;
            }

            cts?.Cancel();
        }

        private async Task StopSessionAsync(SessionStatus intendedCompletionStatus)
        {
            sessionStatusUpdater.UpdateStatus(SessionStatus.Stopping);

            // First attempt to close windows in the job object gracefully
            logger.AttemptingToCloseProcessWindows();
            await jobObjectManager.AttemptCloseWindowsInJobAsync(_currentSession!.GracefulCloseWindowsTimeoutMs);

            var completionStatus = intendedCompletionStatus;
            // Then always terminate the job object to ensure all processes are cleaned up
            try
            {
                logger.TerminatingProcesses();
                jobObjectManager.TerminateJobObject();

                // Accept the job object as complete after 10 seconds to avoid hangs
                // TODO: Make this configurable?
                using var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
                await jobObjectManager.WaitForJobObjectCompletionAsync(timeoutCts.Token);
            }
            catch (WinApiException ex)
            {
                logger.AbandoningSessionDueToWinError(ex);
                completionStatus = SessionStatus.Failed;
            }
            finally
            {
                sessionStatusUpdater.UpdateStatus(completionStatus);
            }
        }

        private void OnProcessExited(ProcessCompletionContext context)
        {
            switch (context.ProcessDto.ExitBehavior)
            {
                case ProcessExitBehavior.None:
                    break;

                case ProcessExitBehavior.EndSession:
                    _sessionCts!.Cancel();
                    break;

                case ProcessExitBehavior.RestartSession:
                    // Don't do anything when not in a cancelable active session
                    if (!sessionStatusProvider.IsActive || _sessionCts?.Token.IsCancellationRequested == true) return;
                    // TODO
                    break;

                case ProcessExitBehavior.RestartSelf:
                    // TODO
                    break;
            }
        }
    }
}
