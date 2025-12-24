using FELauncher.Engine.Exceptions;
using FELauncher.Engine.JobObjects;
using FELauncher.Engine.Processes;
using FELauncher.Engine.Settings;
using FELauncher.Shared.Contracts;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FELauncher.Engine.Sessions
{
    internal sealed class SessionManager(
        ILogger<SessionManager> logger,
        ISessionLoggerScopeProvider sessionLoggerScopeProvider,
        IOptionsMonitor<FELauncherSettings> settings,
        INotifier notifier,
        JobObjectManager jobObjectManager,
        ProcessRunner processRunner) : ISessionManager
    {
        private Session? _session;
        private FELauncherSettings? _sessionSettings;
        private CancellationTokenSource? _sessionCts;
        private bool _shutdownRequested;
        private readonly Lock _sessionLock = new();

        public bool IsSessionActive
        {
            get
            {
                lock (_sessionLock)
                {
                    return _session?.IsActive == true;
                }
            }
        }

        public bool CanEndSession
        {
            get
            {
                lock (_sessionLock)
                {
                    return _session?.CanRequestStop == true;
                }
            }
        }

        public async Task StartNewSessionAsync()
        {
            CancellationToken ct;

            lock (_sessionLock)
            {
                if (_session?.IsActive == true)
                {
                    // cannot start new session when current session is active
                    // todo: log, possibly toast
                    return;
                }

                _session = new()
                {
                    Status = SessionStatus.Starting
                };

                _shutdownRequested = false;
                _sessionCts = new CancellationTokenSource();
                ct = _sessionCts.Token;
            }

            sessionLoggerScopeProvider.SetCurrentSessionId(_session.Id);
            using var sessionScope = sessionLoggerScopeProvider.BeginSessionScope(logger);

            try
            {
                _sessionSettings = settings.CurrentValue;

                processRunner.RunnerProcessExited += OnRunnerProcessExited;
                jobObjectManager.ResetJobObject();

                /* Pre-hooks */
                // session.Status = SessionStatus.RunningPreHooks;

                lock (_sessionLock)
                {
                    _session.Status = SessionStatus.RunningPreProcesses;
                }
                await processRunner.RunAsync(_sessionSettings.PreProcesses, ct);

                lock (_sessionLock)
                {
                    _session.Status = SessionStatus.RunningFrontend;
                }
                await processRunner.RunAsync(_sessionSettings.Frontend, ct);

                // Wait for all processes to exit naturally.
                await jobObjectManager.WaitForJobObjectCompletionAsync(ct);

                /* Post-hooks */
                // session.Status = SessionStatus.RunningPostHooks;

                lock (_sessionLock)
                {
                    _session.Status = SessionStatus.Completed;
                }
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested)
            {
                await StopSessionAsync();
                lock (_sessionLock)
                {
                    _session.Status = SessionStatus.Aborted;
                }
            }
            catch (JobObjectException ex)
            {
                // cancel session run
                lock (_sessionLock)
                {
                    _session.Status = SessionStatus.Failed;
                }
                // log here
                // toast here or task dialog?
            }
            catch (ProcessCreationException ex)
            {
                // cancel session run
                lock (_sessionLock)
                {
                    _session.Status = SessionStatus.Failed;
                }
                // log here
                // toast here or task dialog?
            }
            catch (Exception ex)
            {
                await StopSessionAsync();
                lock (_sessionLock)
                {
                    _session.Status = SessionStatus.Failed;
                }
                // log here
                // toast here or task dialog?
            }
            finally
            {
                processRunner.RunnerProcessExited -= OnRunnerProcessExited;

                lock (_sessionLock)
                {
                    _sessionCts.Dispose();
                    _sessionSettings = null;
                    _sessionCts = null;
                }
                // sessionLoggerScopeProvider.SetCurrentSessionId(null);
            }
        }

        public void RequestEndSession()
        {
            CancellationTokenSource? cts;

            lock (_sessionLock)
            {
                if (_session?.CanRequestStop != true) return;

                _shutdownRequested = true;
                cts = _sessionCts;
            }

            cts?.Cancel();
        }

        private async Task StopSessionAsync()
        {
            lock (_sessionLock)
            {
                if (_session is null)
                {
                    // log here
                    return;
                }

                if (!_session.CanRequestStop)
                {
                    // log here
                    return;
                }

                _session.Status = SessionStatus.Stopping;
            }

            await jobObjectManager.AttemptCloseWindowsInJobAsync(
                _sessionSettings?.EndSessionGracePeriod ?? 0);
            try
            {
                jobObjectManager.TerminateJobObject();
            }
            catch { /* Maybe do something here */ }
        }

        private void OnRunnerProcessExited(object? sender, RunnerProcessExitedEventArgs e)
        {
            bool shouldEndSession;

            lock (_sessionLock)
            {
                // Don't do anything when not in a cancelable active session
                if (_session?.CanRequestStop != true || _shutdownRequested) return;

                shouldEndSession = e.EndSessionOnExit && _shutdownRequested == false;

                if (shouldEndSession) _shutdownRequested = true;
            }

            if (shouldEndSession)
            {
                RequestEndSession();
                notifier.Notify(
                    "Ending Session",
                    $"Ending the current session because '{e.ProcessName}' has terminated.");
                return;
            }

            if (e.NotifyOnExit)
            {
                notifier.Notify(
                    "Process Exited",
                    $"'{e.ProcessName}' has terminated.");
            }
        }
    }
}
