using FELauncher.Engine.Sessions;
using FELauncher.Engine.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace FELauncher.Engine.Processes
{
    internal sealed class PreProcessRunner : IPreProcessRunner
    {
        public event EventHandler<PreProcessExitedEventArgs>? PreProcessExited;

        private readonly List<FELPreProcess> _running = [];
        private readonly IList<PreProcessSettings>? _preProcessSettings;

        private readonly ILogger<PreProcessRunner> _logger;
        private readonly IOptionsSnapshot<FELauncherSettings> _settings;
        private readonly IProcessFactory _factory;
        private readonly IJobObjectManager _jobObjectManager;

        public PreProcessRunner(
            ILogger<PreProcessRunner> logger,
            IOptionsSnapshot<FELauncherSettings> settings,
            IProcessFactory factory,
            IJobObjectManager jobObjectManager)
        {
            _logger = logger;
            _settings = settings;
            _factory = factory;
            _jobObjectManager = jobObjectManager;

            _preProcessSettings = _settings.Value.PreProcesses;
        }

        public async Task RunAsync()
        {
            if (_preProcessSettings is null
             || _preProcessSettings.Count == 0) return;

            var felPreProcesses = CreateFELPreProcesses();
            _running.AddRange(felPreProcesses);
            
            foreach (var felPreProcess in felPreProcesses)
            {
                var proc = felPreProcess.Process;

                proc.Exited += OnProcessExited;

                var delayTime = TimeSpan.FromSeconds(felPreProcess.DelaySeconds);
                if (delayTime > TimeSpan.Zero)
                {
                    await Task.Delay(delayTime);
                }

                proc.Start();
                // Theoretical race condition here where process immediately terminates
                // or spawns children before job object assigment.
                //
                // Monitor real-world impact.
                _jobObjectManager.AssignToJobObject(proc);
            }
        }

        private List<FELPreProcess> CreateFELPreProcesses()
        {
            var processes = new List<FELPreProcess>();
            foreach (var preProcess in _preProcessSettings!)
            {
                var process = _factory.Create(preProcess.Path, preProcess.Arguments);

                var felPreProcess = new FELPreProcess(
                    process,
                    preProcess.DelaySeconds,
                    preProcess.NotifyOnExit,
                    preProcess.EndSessionOnExit);

                processes.Add(felPreProcess);
            }

            return processes;
        }

        private void OnProcessExited(object? sender, EventArgs e)
        {
            if (_running.Count == 0) return;

            var proc = (Process)sender!;

            var felPreProcess = _running.Find((p) => p.Process == proc);
            if (felPreProcess is null) return;

            int exitCode;
            try
            {
                exitCode = proc.ExitCode;
            }
            catch
            {
                exitCode = -1;
            }

            _running.Remove(felPreProcess);
            PreProcessExited?.Invoke(this, new PreProcessExitedEventArgs
            {
                ExitCode         = exitCode,
                NotifyOnExit     = felPreProcess.NotifyOnExit,
                EndSessionOnExit = felPreProcess.EndSessionOnExit
            });
        }
    }
}
