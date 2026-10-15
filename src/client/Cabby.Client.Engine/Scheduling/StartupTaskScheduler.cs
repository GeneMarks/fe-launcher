using Cabby.Client.Engine.Scheduling.Logging;
using Cabby.Client.Shared.Contracts.Settings;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Win32.TaskScheduler;

namespace Cabby.Client.Engine.Scheduling
{
    internal sealed class StartupTaskScheduler(
        ILogger<StartupTaskScheduler> logger,
        IOptionsMonitor<CabbySettings> settings) : IHostedService, IDisposable
    {
        private bool _startWithWindowsOptionSnapshot;
        private IDisposable? _onChangeToken;
        private static readonly string taskName = "CabbyStartupTask";

        public async System.Threading.Tasks.Task StartAsync(CancellationToken ct = default)
        {
            _startWithWindowsOptionSnapshot = settings.CurrentValue.StartWithWindows;
            _onChangeToken = settings.OnChange(OnSettingsChange);
        }

        public async System.Threading.Tasks.Task StopAsync(CancellationToken ct = default)
        {
            // Ensure task is synced during graceful shutdown.
            SyncTaskState(_startWithWindowsOptionSnapshot);

            _onChangeToken?.Dispose();
        }

        private void OnSettingsChange(CabbySettings settings, string? name)
        {
            bool startWithWindows = settings.StartWithWindows;

            if (_startWithWindowsOptionSnapshot == startWithWindows) return;

            _startWithWindowsOptionSnapshot = startWithWindows;
            SyncTaskState(startWithWindows);
        }

        private void RegisterTaskIfNotExists()
        {
            if (TaskService.Instance.GetTask(taskName) != null)
            {
                logger.StartupTaskAlreadyExists(taskName);
                return;
            }

            logger.RegisteringTask(taskName);

            TaskDefinition td = TaskService.Instance.NewTask();

            td.Principal.LogonType = TaskLogonType.InteractiveToken;
            td.Principal.RunLevel = TaskRunLevel.Highest;

            td.Triggers.Add(new LogonTrigger());

            td.Actions.Add(
                path: Environment.ProcessPath,
                workingDirectory: Path.GetDirectoryName(Environment.ProcessPath));

            TaskService.Instance.RootFolder.RegisterTaskDefinition(taskName, td);
        }

        private void EnsureTaskIsDeleted()
        {
            logger.EnsuringTaskIsDeleted(taskName);
            TaskService.Instance.RootFolder.DeleteTask(taskName, false);
        }

        private void SyncTaskState(bool registerTask)
        {
            if (registerTask)
            {
                RegisterTaskIfNotExists();
            }
            else
            {
                EnsureTaskIsDeleted();
            }
        }

        public void Dispose()
            => _onChangeToken?.Dispose();
    }
}
