using FELauncher.Engine.Exceptions;
using FELauncher.Engine.Processes;
using FELauncher.Engine.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FELauncher.Engine.Sessions
{
    internal sealed class SessionManager(
        ILogger<SessionManager> logger,
        ISessionLoggerScopeProvider sessionLoggerScopeProvider,
        IOptionsMonitor<FELauncherSettings> settings,
        JobObjectManager jobObjectManager,
        FELProcessManager processManager,
        PreProcessRunner preProcessRunner,
        FrontendRunner frontendRunner) : ISessionManager
    {
        private Session? _session;
        private FELauncherSettings? _sessionSettings;
        private CancellationTokenSource? _sessionCts;
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

                _sessionCts = new CancellationTokenSource();
            }

            var ct = _sessionCts.Token;

            sessionLoggerScopeProvider.SetCurrentSessionId(_session.Id);
            using var sessionScope = sessionLoggerScopeProvider.BeginSessionScope(logger);

            try
            {
                _sessionSettings = settings.CurrentValue;

                jobObjectManager.ResetJobObject();
                processManager.FELProcessExited += OnFELProcessExited;

                /* Pre-hooks */
                // session.Status = SessionStatus.RunningPreHooks;

                lock (_sessionLock)
                {
                    _session.Status = SessionStatus.RunningPreProcesses;
                }
                await preProcessRunner.RunAsync(_sessionSettings.PreProcesses, ct);

                lock (_sessionLock)
                {
                    _session.Status = SessionStatus.RunningFrontend;
                }
                await frontendRunner.RunAsync(_sessionSettings.Frontend, ct);

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
                // toast here
                lock (_sessionLock)
                {
                    _session.Status = SessionStatus.Failed;
                }
            }
            catch (Win32ProcessCreationException ex)
            {
                // cancel session run
                // toast here
                lock (_sessionLock)
                {
                    _session.Status = SessionStatus.Failed;
                }
            }
            catch (Exception ex)
            {
                // cancel session run
                // log unknown session
                // toast here
                lock (_sessionLock)
                {
                    _session.Status = SessionStatus.Failed;
                }
            }
            finally
            {
                _sessionCts.Dispose();
                _sessionSettings = null;
                _sessionCts = null;
                sessionLoggerScopeProvider.SetCurrentSessionId(null);
            }
        }

        public void RequestEndSession()
        {
            lock (_sessionLock)
            {
                if (_session?.CanRequestStop != true) return;
                _sessionCts?.Cancel();
            }
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

        private void OnFELProcessExited(object? sender, FELProcessExitedEventArgs e)
        {
            //if (e.EndSessionOnExit) RequestEndSession();
        }
    }
}
