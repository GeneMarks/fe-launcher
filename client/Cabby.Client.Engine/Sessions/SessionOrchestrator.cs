using Cabby.Client.Engine.Exceptions;
using Cabby.Client.Engine.Networking;
using Cabby.Client.Engine.Processes;
using Cabby.Client.Engine.Sessions.Logging;
using Cabby.Client.Shared.Contracts.Network;
using Cabby.Client.Shared.Contracts.Processes;
using Cabby.Client.Shared.Contracts.Sessions;
using Microsoft.Extensions.Logging;

namespace Cabby.Client.Engine.Sessions
{
    internal sealed class SessionOrchestrator(
        ILogger<SessionOrchestrator> logger,
        ServerMessageDispatcher serverMessageDispatcher,
        SessionStatusProvider sessionStatusProvider,
        SessionStatusUpdater sessionStatusUpdater,
        SessionLoggerScopeProvider sessionLoggerScopeProvider,
        JobObjectManager jobObjectManager,
        ProcessDispatcher processDispatcher)
    {
        private SessionDto? _currentSession;
        private CancellationTokenSource? _sessionCts;
        private TaskCompletionSource<bool>? _frontendCompletionTcs;
        private bool _frontendWasSuccessful;

        public async Task StartNewSessionAsync(SessionDto sessionDto)
        {
            if (!sessionStatusProvider.IsIdle)
            {
                logger.CannotStartMultipleSessions();
                await serverMessageDispatcher.SendAsync(WebSocketMessageTypes.Outbound.CannotStartMultipleSessions);
                return;
            }

            _currentSession = sessionDto;
            logger.StartingNewSession(_currentSession.Id);
            sessionStatusUpdater.UpdateSession(_currentSession);
            sessionStatusUpdater.UpdateStatus(SessionStatus.Pending);
            await DispatchCurrentSessionStatusAsync();

            _sessionCts = new CancellationTokenSource();
            var sessionCt = _sessionCts.Token;

            sessionLoggerScopeProvider.SetCurrentSessionId(_currentSession.Id);
            using var sessionScope = sessionLoggerScopeProvider.BeginSessionScope(logger);

            try
            {
                jobObjectManager.ResetJobObject();

                /* Pre-Processes */
                sessionStatusUpdater.UpdateStatus(SessionStatus.RunningPreProcesses);
                await DispatchCurrentSessionStatusAsync();
                logger.RunningPreProcesses(_currentSession.PreProcesses.Length);

                foreach (var preProcess in _currentSession.PreProcesses)
                {
                    await processDispatcher.DispatchAsync(preProcess, OnProcessExited, sessionCt);
                }

                /* Frontend */
                sessionStatusUpdater.UpdateStatus(SessionStatus.RunningFrontend);
                await DispatchCurrentSessionStatusAsync();
                logger.StartingFrontend();

                _frontendCompletionTcs = new TaskCompletionSource<bool>();
                await processDispatcher.DispatchAsync(_currentSession.MainProcess, OnProcessExited, sessionCt);
                await _frontendCompletionTcs.Task.WaitAsync(sessionCt);

                /* Post-Processes */
                if (!_frontendWasSuccessful)
                {
                    await StopSessionAsync(SessionStatus.Failed);
                    return;
                }

                sessionStatusUpdater.UpdateStatus(SessionStatus.RunningPostProcesses);
                await DispatchCurrentSessionStatusAsync();
                logger.RunningPostProcesses(_currentSession!.PostProcesses.Length);

                foreach (var postProcess in _currentSession.PostProcesses)
                {
                    await processDispatcher.DispatchAsync(postProcess, OnProcessExited, _sessionCts!.Token);
                }

                sessionStatusUpdater.UpdateStatus(SessionStatus.Completed);
                await DispatchCurrentSessionStatusAsync();
            }
            catch (OperationCanceledException) when (sessionCt.IsCancellationRequested)
            {
                logger.StoppingSession();
                await StopSessionAsync(SessionStatus.Aborted);
            }
            catch (WinApiException ex)
            {
                logger.FatalSessionError(ex.GetType().Name);
                await StopSessionAsync(SessionStatus.Failed);
                await serverMessageDispatcher.SendAsync(WebSocketMessageTypes.Outbound.SessionFailed);
            }
            catch (Exception ex)
            {
                logger.UnknownFatalSessionError(ex);
                await StopSessionAsync(SessionStatus.Failed);
                await serverMessageDispatcher.SendAsync(WebSocketMessageTypes.Outbound.SessionFailed);
            }
            finally
            {
                var finalSessionStatus = sessionStatusProvider.CurrentStatus;
                var runtime = sessionStatusProvider.ElapsedTime;
                var prettyRuntime = $"{(int)runtime.TotalHours} hrs {runtime.Minutes:00} min {runtime.Seconds:00} sec";
                logger.SessionFinalRuntime(_currentSession.Id, finalSessionStatus, prettyRuntime);

                sessionStatusUpdater.ResetState();
                _sessionCts?.Dispose();
                _sessionCts = null;
                _currentSession = null;
                _frontendCompletionTcs = null;
                _frontendWasSuccessful = false;

                await serverMessageDispatcher.SendAsync(WebSocketMessageTypes.Outbound.ReadyForSession);
            }
        }

        public void RequestCancelSession()
        {
            if (sessionStatusProvider.IsCancellable)
            {
                _sessionCts?.Cancel();
            }
        }

        private async Task StopSessionAsync(SessionStatus intendedCompletionStatus)
        {
            sessionStatusUpdater.UpdateStatus(SessionStatus.Stopping);

            var completionStatus = intendedCompletionStatus;
            try
            {
                // First attempt to close windows in the job object gracefully
                logger.AttemptingToCloseProcessWindows();
                await jobObjectManager.AttemptCloseWindowsInJobAsync(_currentSession!.GracefulCloseWindowsTimeoutMs);

                // Then always terminate the job object to ensure all processes are cleaned up
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
                await serverMessageDispatcher.SendAsync(WebSocketMessageTypes.Outbound.SessionFailed);
            }
            finally
            {
                sessionStatusUpdater.UpdateStatus(completionStatus);
                await DispatchCurrentSessionStatusAsync();
            }
        }

        private async Task OnProcessStarted(object? sender, ProcessStartedEventArgs e)
        {
            await DispatchProcessStatusUpdateAsync(e.ProcessDto, ProcessStatus.Running);
        }

        private async Task OnProcessExited(ProcessCompletionContext context)
        {
            if (!context.WasSuccessful)
            {
                // TODO: stuff
            }

            await DispatchProcessStatusUpdateAsync(
                context.ProcessDto,
                context.WasSuccessful ? ProcessStatus.Completed : ProcessStatus.Failed);

            if (context.ProcessDto.Phase == ProcessPhase.Main)
            {
                _frontendWasSuccessful = context.WasSuccessful;
                _frontendCompletionTcs?.TrySetResult(true);
            }
        }

        private async Task DispatchProcessStatusUpdateAsync(ProcessDto processDto, ProcessStatus processStatus)
        {
            await serverMessageDispatcher.SendAsync(
                WebSocketMessageTypes.Outbound.ProcessStatusUpdate,
                new ProcessStatusUpdateDto
                {
                    Id = processDto.Id,
                    SessionId = processDto.SessionId,
                    Status = processStatus
                },
                _sessionCts?.Token ?? default);
        }

        private async Task DispatchCurrentSessionStatusAsync()
        {
            if (_currentSession is null)
            {
                // TODO: Logging
                return;
            }

            var sessionStatusUpdate = new SessionStatusUpdateDto
            {
                Id = _currentSession.Id,
                Status = _currentSession.Status
            };

            await serverMessageDispatcher.SendAsync(
                WebSocketMessageTypes.Outbound.SessionStatusUpdate,
                sessionStatusUpdate,
                _sessionCts?.Token ?? default);
        }
    }
}
