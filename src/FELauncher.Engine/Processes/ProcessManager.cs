using FELauncher.Engine.Sessions;
using Microsoft.Extensions.Logging;

namespace FELauncher.Engine.Processes
{
    internal sealed class ProcessManager(
        ILogger<ProcessManager> logger,
        IJobObjectManager jobObjectManager) : IProcessManager
    {
        public event EventHandler<ProcessExitedEventArgs>? ProcessExited;

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

        private void OnProcessExited(object? sender, EventArgs e)
        {
            if (_running.Count == 0) return;

            var proc = (Win32Process)sender!;

            var felProcess = _running.Find((p) => p.Process == proc);
            if (felProcess is null) return;

            _running.Remove(felProcess);
            ProcessExited?.Invoke(this, new ProcessExitedEventArgs
            {
                NotifyOnExit     = felProcess.NotifyOnExit,
                EndSessionOnExit = felProcess.EndSessionOnExit
            });
        }
    }
}
