using FELauncher.Engine.Exceptions;
using FELauncher.Engine.Processes;
using Microsoft.Extensions.Logging;

namespace FELauncher.Engine.Sessions
{
    public sealed class SessionRunner : ISessionRunner
    {
        private readonly ILogger<SessionRunner> _logger;
        private readonly IJobObjectManager _jobObjectManager;
        private readonly IPreProcessRunner _preProcessRunner;
        private readonly IFrontendRunner _frontendRunner;

        public SessionRunner(
            ILogger<SessionRunner> logger,
            IJobObjectManager jobObjectManager,
            IPreProcessRunner preProcessRunner,
            IFrontendRunner frontendRunner)
        {
            _logger = logger;
            _jobObjectManager = jobObjectManager;
            _preProcessRunner = preProcessRunner;
            _frontendRunner = frontendRunner;
        }

        public async Task RunAsync(Session session)
        {
            try
            {
                session.Status = SessionStatus.Starting;

                _jobObjectManager.ResetJobObject();

                /* Pre-hooks */
                // session.Status = SessionStatus.RunningPreHooks;

                /* Preprocesses */
                session.Status = SessionStatus.RunningPreProcesses;
                _preProcessRunner.PreProcessExited += OnPreProcessExited;
                await _preProcessRunner.RunAsync();

                /* Frontend */
                session.Status = SessionStatus.RunningFrontend;
                _frontendRunner.FrontendProcessExited += OnFrontendProcessExited;
                await _frontendRunner.RunAsync();

                /* Post-hooks */
                // session.Status = SessionStatus.RunningPostHooks;

                session.Status = SessionStatus.Completed;
            }
            catch (JobObjectException ex)
            {
                // cancel session run
                // toast here
            }
            catch (ProcessCreationException ex)
            {
                // cancel session run
                // toast here
            }
            catch (AssignProcessToJobObjectException ex)
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
        }

        private void OnPreProcessExited(object? sender, PreProcessExitedEventArgs e)
        {

        }

        private void OnFrontendProcessExited(object? sender, FrontendProcessExitedEventArgs e)
        {

        }
    }
}
