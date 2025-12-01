using FELauncher.Engine.IO;
using FELauncher.Engine.Logging;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using Windows.Win32;
using Windows.Win32.Foundation;

namespace FELauncher.Engine.Processes
{
    public sealed class ProcessManager : IProcessManager
    {
        private readonly ILogger<ProcessManager> _logger;
        private readonly IPathResolver _pathResolver;

        public ProcessManager(ILogger<ProcessManager> logger, IPathResolver pathResolver)
        {
            _logger = logger;
            _pathResolver = pathResolver;
        }

        public ProcessStartResult StartProcess(string? executablePath, string? arguments)
        {
            var path = _pathResolver.ResolvePath(executablePath);

            if (String.IsNullOrEmpty(path))
            {
                _logger.Processes_InvalidPath(path ?? String.Empty);
                return ProcessStartResult.Fail("Path to executable invalid or empty.");
            }

            if (Path.GetExtension(path) is not EngineDefaults.ExecutableExtension)
            {
                _logger.Processes_InvalidFileExt(path, EngineDefaults.ExecutableExtension);
                return ProcessStartResult.Fail("The specified file is not a valid executable.");
            }

            if (!File.Exists(path))
            {
                _logger.Processes_FileNotPresent(path);
                return ProcessStartResult.Fail("The specified executable does not exist.");
            }

            var fileName = Path.GetFileName(path);
            var workingDir = Path.GetDirectoryName(path);

            var startInfo = new ProcessStartInfo(path)
            {
                WorkingDirectory = workingDir,
                UseShellExecute = false,
                Arguments = arguments
            };


            // todo:
            // job object
            // handle
            // add completion action to handle
            // return processstartresult.success
            try
            {
                var process = Process.Start(startInfo);

                if (process is null)
                {
                    _logger.Processes_FailedToStartUnknown(fileName);
                    return ProcessStartResult.Fail("The process failed to start for an unknown reason.\nCheck logs for more details.");
                }
            }
            catch (Exception ex)
            {
                _logger.Processes_FailedToStart(fileName, ex.Message);
                return ProcessStartResult.Fail("The process failed to start.\nCheck logs for more details.");
            }
        }
    }
}