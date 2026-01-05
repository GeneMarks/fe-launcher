using FELauncher.Engine.JobObjects;
using FELauncher.Engine.Processes.Logging;
using FELauncher.Engine.Settings;
using FELauncher.Shared.Contracts.Sessions;
using Microsoft.Extensions.Logging;

namespace FELauncher.Engine.Processes.Runner
{
    internal sealed class ProcessRunner(
        ILogger<ProcessRunner> logger,
        ISessionLoggerScopeProvider sessionLoggerScopeProvider,
        ProcessFactory factory,
        JobObjectManager jobObjectManager)
    {
        public event EventHandler<RunnerProcessExitedEventArgs>? RunnerProcessExited;

        private readonly Dictionary<Process, ProcessRunItem> _running = [];
        private readonly Lock _runningLock = new();

        public bool TryBuildProcessRun(
            ProcessSettings settings,
            out ProcessRun? run,
            out ProcessCreationFailure? failure,
            CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();

            run = null;
            failure = null;

            if (!factory.TryCreate(settings.Path, settings.Arguments,
                out var process, out var processCreationFailure))
            {
                failure = processCreationFailure!;
                return false;
            }

            var item = new ProcessRunItem(process!, settings);
            run = new([item]);
            return true;
        }

        public bool TryBuildProcessRun(
            IList<ProcessSettings> settingsList,
            out ProcessRun? run,
            out IList<ProcessCreationFailure> failures,
            CancellationToken ct = default)
        {
            run = null;
            failures = [];
            List<ProcessRunItem> items = [];

            bool success = true;

            foreach (var settings in settingsList)
            {
                ct.ThrowIfCancellationRequested();

                if (!factory.TryCreate(settings.Path, settings.Arguments,
                    out var process, out var processCreationFailure))
                {
                    failures.Add(processCreationFailure!);
                    success = false;
                    continue;
                }

                items.Add(new(process!, settings));
            }

            if (success)
            {
                run = new(items);
            }

            return success;
        }

        public async Task RunAsync(ProcessRun processRun, CancellationToken ct = default)
        {
            var items = processRun.Items;

            if (items.Count == 0)
            {
                logger.NoProcessesInProcessRun();
                return;
            }

            foreach (var item in items)
            {
                ct.ThrowIfCancellationRequested();
                await StartProcessItemAsync(item, ct);
            }
        }

        public void Reset()
        {
            lock (_runningLock)
            {
                foreach (var p in _running.Keys)
                {
                    p.Exited -= OnProcessExited;
                }

                _running.Clear();
            }
        }

        private async Task StartProcessItemAsync(
            ProcessRunItem item,
            CancellationToken ct = default)
        {
            var process = item.Process;
            var processSettings = item.ProcessSettings;

            var delaySeconds = processSettings.DelaySeconds;
            var delayTime = TimeSpan.FromSeconds(delaySeconds);
            if (delayTime > TimeSpan.Zero)
            {
                logger.WaitingDelay(delaySeconds, process.PrettyName, process.PathWithArgs);
                await Task.Delay(delayTime, ct);
            }

            lock (_runningLock)
            {
                _running.Add(process, item);
                process.Exited += OnProcessExited;
            }

            try
            {
                ct.ThrowIfCancellationRequested();

                logger.StartingProcess(process.PrettyName, process.PathWithArgs);
                process.StartInJob(jobObjectManager.SafeJobHandle);
            }
            catch
            {
                lock (_runningLock)
                {
                    process.Exited -= OnProcessExited;
                    _running.Remove(process);
                }
                throw;
            }
        }

        private void OnProcessExited(object? sender, ProcessExitedEventArgs e)
        {
            var process = (Process)sender!;

            ProcessRunItem? item;
            lock (_runningLock)
            {
                if (!_running.TryGetValue(process, out item)) return;

                _running.Remove(process);
                process.Exited -= OnProcessExited;
            }

            process.Dispose();

            using var sessionScope = sessionLoggerScopeProvider.BeginSessionScope(logger);
            logger.ProcessExited(e.ProcessId, e.ProcessPathWithArgs, e.ExitCode);

            var processSettings = item.ProcessSettings;

            RunnerProcessExited?.Invoke(this, new RunnerProcessExitedEventArgs
            {
                ProcessName      = e.ProcessName,
                NotifyOnExit     = processSettings.NotifyOnExit,
                EndSessionOnExit = processSettings.EndSessionOnExit
            });
        }
    }
}
