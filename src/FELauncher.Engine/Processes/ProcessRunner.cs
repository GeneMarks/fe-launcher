using FELauncher.Engine.JobObjects;
using FELauncher.Engine.Processes.Logging;
using FELauncher.Engine.Settings;
using FELauncher.Shared.Contracts;
using Microsoft.Extensions.Logging;

namespace FELauncher.Engine.Processes
{
    internal sealed class ProcessRunner(
        ILogger<ProcessRunner> logger,
        ISessionLoggerScopeProvider sessionLoggerScopeProvider,
        ProcessFactory factory,
        JobObjectManager jobObjectManager)
    {
        public event EventHandler<RunnerProcessExitedEventArgs>? RunnerProcessExited;

        private readonly Dictionary<Process, ProcessSettings> _running = [];

        public async Task RunAsync(ProcessSettings processSettings, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();

            var process = factory.Create(processSettings.Path, processSettings.Arguments);
            await StartAndTrackAsync(process, processSettings, ct);
        }

        public async Task RunAsync(IList<ProcessSettings> processSettingsList, CancellationToken ct = default)
        {
            foreach (var processSettings in processSettingsList)
            {
                await RunAsync(processSettings, ct);
            }
        }

        private async Task StartAndTrackAsync(
            Process process,
            ProcessSettings processSettings,
            CancellationToken ct = default)
        {
            _running.Add(process, processSettings);
            process.Exited += OnProcessExited;

            var delayTime = TimeSpan.FromSeconds(processSettings.DelaySeconds);
            if (delayTime > TimeSpan.Zero)
            {
                await Task.Delay(delayTime, ct);
            }

            process.StartInJob(jobObjectManager.SafeJobHandle);
        }

        private void OnProcessExited(object? sender, ProcessExitedEventArgs e)
        {
            if (_running.Count == 0) return;

            var senderProc = (Process)sender!;
            senderProc.Exited -= OnProcessExited;

            if (!_running.TryGetValue(senderProc, out var processSettings)) return;

            _running.Remove(senderProc);

            using var sessionScope = sessionLoggerScopeProvider.BeginSessionScope(logger);
            logger.ProcessExited(e.ProcessId, e.ProcessPath, e.ExitCode);

            RunnerProcessExited?.Invoke(this, new RunnerProcessExitedEventArgs
            {
                ProcessName      = e.ProcessName,
                NotifyOnExit     = processSettings.NotifyOnExit,
                EndSessionOnExit = processSettings.EndSessionOnExit
            });
        }
    }
}
