using FELauncher.Engine.Processes.Logging;
using FELauncher.Engine.Sessions;
using Microsoft.Extensions.Logging;

namespace FELauncher.Engine.Processes
{
    internal sealed class FELProcessManager(
        ILogger<FELProcessManager> logger,
        ISessionLoggerScopeProvider sessionLoggerScopeProvider,
        JobObjectManager jobObjectManager)
    {
        public event EventHandler<FELProcessExitedEventArgs>? FELProcessExited;

        private readonly List<FELProcess> _running = [];

        public async Task StartAndTrackAsync(
            FELProcess felProcess,
            CancellationToken ct = default)
        {
            var proc = felProcess.Process;

            _running.Add(felProcess);

            proc.Exited += OnProcessExited;

            var delayTime = TimeSpan.FromSeconds(felProcess.DelaySeconds);
            if (delayTime > TimeSpan.Zero)
            {
                await Task.Delay(delayTime, ct);
            }

            proc.StartInJob(jobObjectManager.SafeJobHandle);
        }

        private void OnProcessExited(object? sender, Win32ProcessExitedEventArgs e)
        {
            if (_running.Count == 0) return;

            var proc = (Win32Process)sender!;
            proc.Exited -= OnProcessExited;

            var felProcess = _running.Find((p) => p.Process == proc);
            if (felProcess is null) return;

            _running.Remove(felProcess);

            using var sessionScope = sessionLoggerScopeProvider.BeginSessionScope(logger);
            logger.Win32ProcessExited(e.ProcessId, e.ProcessPath, e.ExitCode);

            FELProcessExited?.Invoke(this, new FELProcessExitedEventArgs
            {
                NotifyOnExit     = felProcess.NotifyOnExit,
                EndSessionOnExit = felProcess.EndSessionOnExit
            });
        }
    }
}
