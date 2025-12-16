using FELauncher.Engine.Exceptions;
using FELauncher.Engine.Processes;
using FELauncher.Engine.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FELauncher.Engine.Sessions
{
    internal sealed class SessionManager(
        ILogger<SessionManager> logger,
        IOptionsMonitor<FELauncherSettings> settings,
        JobObjectManager jobObjectManager,
        FELProcessManager processManager,
        PreProcessRunner preProcessRunner,
        FrontendRunner frontendRunner) : ISessionManager
    {
        private Session? _session;
        private CancellationTokenSource? _cancellationTokenSource;
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

        public CancellationTokenSource CancellationTokenSource
        {
            get
            {
                lock (_sessionLock)
                {
                    if (_session?.IsActive == true && _cancellationTokenSource is not null)
                    {
                        return _cancellationTokenSource;
                    }

                    _cancellationTokenSource?.Dispose();
                    _cancellationTokenSource = new CancellationTokenSource();
                    return _cancellationTokenSource;
                }
            }
        }

        public async Task StartNewSessionAsync(CancellationToken ct = default)
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
            }

            try
            {
                ct.ThrowIfCancellationRequested();

                FELauncherSettings sessionSettings = settings.CurrentValue;

                jobObjectManager.ResetJobObject();
                processManager.FELProcessExited += OnFELProcessExited;

                /* Pre-hooks */
                // session.Status = SessionStatus.RunningPreHooks;

                lock (_sessionLock)
                {
                    _session.Status = SessionStatus.RunningPreProcesses;
                }
                await preProcessRunner.RunAsync(sessionSettings.PreProcesses, ct);

                lock (_sessionLock)
                {
                    _session.Status = SessionStatus.RunningFrontend;
                }
                await frontendRunner.RunAsync(sessionSettings.Frontend, ct);

                // Wait for all processes to exit naturally.
                await jobObjectManager.WaitForJobObjectCompletionAsync(ct);

                /* Post-hooks */
                // session.Status = SessionStatus.RunningPostHooks;

                lock (_sessionLock)
                {
                    _session.Status = SessionStatus.Completed;
                }
            }
            catch (OperationCanceledException ex) when (ct.IsCancellationRequested)
            {
                lock (_sessionLock)
                {
                    _session.Status = SessionStatus.Aborted;
                }
            }
            catch (JobObjectException ex)
            {
                // cancel session run
                // toast here
            }
            catch (Win32ProcessCreationException ex)
            {
                // cancel session run
                // toast here
            }
            catch (Exception ex)
            {
                // cancel session run
                // log unknown session
                // toast here
            }
            finally
            {

            }
        }

        private void OnFELProcessExited(object? sender, FELProcessExitedEventArgs e)
        {

        }
    }
}
