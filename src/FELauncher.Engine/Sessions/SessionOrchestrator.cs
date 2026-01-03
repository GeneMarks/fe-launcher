using FELauncher.Engine.Exceptions;
using FELauncher.Engine.JobObjects;
using FELauncher.Engine.Processes;
using FELauncher.Engine.Processes.Runner;
using FELauncher.Engine.Sessions.Logging;
using FELauncher.Engine.Settings;
using FELauncher.Shared.Contracts;
using FELauncher.Shared.Contracts.Sessions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FELauncher.Engine.Sessions
{
    internal sealed class SessionOrchestrator(
        ILogger<SessionOrchestrator> logger,
        ISessionLoggerScopeProvider sessionLoggerScopeProvider,
        IOptionsMonitor<FELauncherSettings> settings,
        INotifier notifier,
        JobObjectManager jobObjectManager,
        ProcessRunner processRunner) : ISessionOrchestrator
    {
        private Session? _session;
        private FELauncherSettings? _sessionSettings;
        private CancellationTokenSource? _sessionCts;
        private readonly Lock _sessionLock = new();

        public SessionStateSnapshot GetSessionState()
        {
            Session? s;
            lock (_sessionLock) s = _session;

            return new SessionStateSnapshot(
                Id: s?.Id,
                Status: s?.Status ?? SessionStatus.Created,
                IsActive: s?.IsActive ?? false,
                CanRequestStop: s?.CanRequestStop ?? false);
        }

        public async Task StartNewSessionAsync()
        {
            CancellationToken ct;

            lock (_sessionLock)
            {
                if (_session?.IsActive == true)
                {
                    logger.CannotStartMultipleSessions();
                    return;
                }

                _session = new()
                {
                    Status = SessionStatus.Starting
                };

                _sessionCts = new CancellationTokenSource();
                ct = _sessionCts.Token;

                logger.StartingNewSession(_session.Id);
            }

            sessionLoggerScopeProvider.SetCurrentSessionId(_session.Id);
            using var sessionScope = sessionLoggerScopeProvider.BeginSessionScope(logger);

            try
            {
                _sessionSettings = settings.CurrentValue;

                ProcessRun? preProcessRun;
                ProcessRun? frontendRun;
                List<string> processRunBuildErrors = [];

                if (!processRunner.TryBuildProcessRun(
                    _sessionSettings.PreProcesses, 
                    out preProcessRun,
                    out var preProcessBuildFailures, ct))
                {
                    foreach (var fail in preProcessBuildFailures)
                    {
                        processRunBuildErrors.Add(GetProcessRunBuildFailMessage(fail));
                    }
                }

                if (!processRunner.TryBuildProcessRun(
                    _sessionSettings.Frontend,
                    out frontendRun,
                    out var frontendBuildFailure, ct))
                {
                    processRunBuildErrors.Add(GetProcessRunBuildFailMessage(frontendBuildFailure!));
                }

                if (processRunBuildErrors.Count > 0)
                {
                    foreach (var error in processRunBuildErrors)
                    {
                        notifier.Notify("Process Configuration Error", error);
                    }

                    lock (_sessionLock) _session.Status = SessionStatus.Failed;
                    return;
                }

                jobObjectManager.ResetJobObject();
                processRunner.RunnerProcessExited += OnRunnerProcessExited;

                /* Pre-hooks */
                // session.Status = SessionStatus.RunningPreHooks;

                lock (_sessionLock) _session.Status = SessionStatus.RunningPreProcesses;

                logger.RunningPreProcesses(_sessionSettings.PreProcesses.Count);
                await processRunner.RunAsync(preProcessRun!, ct);

                lock (_sessionLock) _session.Status = SessionStatus.RunningFrontend;

                logger.StartingFrontend();
                await processRunner.RunAsync(frontendRun!, ct);

                logger.WaitingForSessionCompletion();
                await jobObjectManager.WaitForJobObjectCompletionAsync(ct);

                // This is going to have to run no matter what..
                /* Post-hooks */
                // session.Status = SessionStatus.RunningPostHooks;

                lock (_sessionLock) _session.Status = SessionStatus.Completed;
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested)
            {
                logger.StoppingSession();
                await StopSessionAsync(SessionStatus.Aborted);
            }
            catch (Exception ex) when (
                ex is JobObjectException
                || ex is ProcessException)
            {
                logger.FatalSessionError(ex.GetType().Name);
                notifier.Notify("Unexpected Error", "Ending the current session. Please see log for more details.");
                await StopSessionAsync(SessionStatus.Failed);
            }
            catch (Exception ex)
            {
                logger.UnknownFatalSessionError(ex);
                notifier.Notify("Unexpected Error", "Ending the current session. Please see log for more details.");
                await StopSessionAsync(SessionStatus.Failed);
            }
            finally
            {
                processRunner.RunnerProcessExited -= OnRunnerProcessExited;
                processRunner.Reset();

                lock (_sessionLock)
                {
                    logger.SessionFinalRuntime(_session.Id, _session.Status, _session.PrettyRuntime);

                    _sessionCts.Dispose();
                    _sessionCts = null;
                    _sessionSettings = null;
                }

                sessionLoggerScopeProvider.SetCurrentSessionId(null);
            }
        }

        public void RequestCancelSession()
        {
            CancellationTokenSource? cts;

            lock (_sessionLock)
            {
                if (_session?.CanRequestStop != true) return;
                cts = _sessionCts;
            }

            cts?.Cancel();
        }

        private async Task StopSessionAsync(SessionStatus intendedCompletionStatus)
        {
            const int timeoutCtsSeconds = 10;

            lock (_sessionLock)
            {
                if (_session is null)
                {
                    logger.TriedToStopNullSession();
                    return;
                }

                _session.Status = SessionStatus.Stopping;
            }

            logger.AttemptingToCloseProcessWindows();
            await jobObjectManager.AttemptCloseWindowsInJobAsync(
                _sessionSettings?.EndSessionGracePeriod ?? 0);

            var completionStatus = intendedCompletionStatus;

            try
            {
                logger.TerminatingProcesses();
                jobObjectManager.TerminateJobObject();

                // Accept the job object as complete no matter what after x seconds to avoid hangs
                using var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(timeoutCtsSeconds));
                await jobObjectManager.WaitForJobObjectCompletionAsync(timeoutCts.Token);
            }
            catch (JobObjectException ex)
            {
                logger.AbandoningSessionDueToJobObjectError(ex);
                completionStatus = SessionStatus.Failed;
            }
            finally
            {
                var isCompletionStatus = completionStatus is (
                      SessionStatus.Completed
                   or SessionStatus.Failed
                   or SessionStatus.Aborted);

                lock (_sessionLock) _session.Status = isCompletionStatus
                        ? completionStatus
                        : SessionStatus.Completed;
            }
        }

        private void OnRunnerProcessExited(object? _, RunnerProcessExitedEventArgs e)
        {
            var endSessionOnExit = e.EndSessionOnExit;
            var processName = e.ProcessName;

            lock (_sessionLock)
            {
                // Don't do anything when not in a cancelable active session
                if (_session?.CanRequestStop != true
                    || _sessionCts?.Token.IsCancellationRequested == true) return;

                if (endSessionOnExit)
                {
                    _sessionCts!.Cancel();
                }
            }

            if (endSessionOnExit)
            {
                notifier.Notify(
                    "Ending Session",
                    $"Ending the current session because '{processName}' has terminated.");

                return;
            }

            if (e.NotifyOnExit)
            {
                notifier.Notify(
                    "Process Exited",
                    $"'{processName}' has terminated.");
            }
        }

        private static string GetProcessRunBuildFailMessage(ProcessCreationFailure failure)
        {
            return failure.Reason switch
            {
                ProcessCreationFailureReason.EmptyPath
                    => $"A provided executable path is empty.\nPlease verify settings.",

                ProcessCreationFailureReason.InvalidFileExt
                    => $"'{failure.FileName}' is not a valid executable file.\nPlease verify settings.",

                ProcessCreationFailureReason.FileNotPresent
                    => $"'{failure.FileName}' does not exist.\nPlease verify settings.",

                _ => String.Empty,
            };
        }
    }
}
