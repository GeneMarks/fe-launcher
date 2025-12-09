using FELauncher.Engine.Exceptions;
using FELauncher.Engine.IO;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace FELauncher.Engine.Processes
{
    internal sealed class ProcessFactory : IProcessFactory
    {
        private readonly ILogger<ProcessFactory> _logger;
        private readonly PathResolver _pathResolver;

        public ProcessFactory(ILogger<ProcessFactory> logger, PathResolver pathResolver)
        {
            _logger = logger;
            _pathResolver = pathResolver;
        }

        public Process Create(string? executablePath, string? arguments)
        {
            var path = _pathResolver.ResolvePath(executablePath);

            if (String.IsNullOrEmpty(path))
            {
                _logger.EmptyPath();
                throw new ProcessCreationException("Executable path is null or empty.");
            }

            if (Path.GetExtension(path) is not EngineConstants.ExecutableExtension)
            {
                _logger.InvalidFileExt(path, EngineConstants.ExecutableExtension);
                throw new ProcessCreationException($"Executable '{path}' does not contain required file extension: {EngineConstants.ExecutableExtension}");
            }

            if (!File.Exists(path))
            {
                _logger.FileNotPresent(path);
                throw new ProcessCreationException($"The specified file '{path}' does not exist.");
            }

            var fileName = Path.GetFileName(path);
            var workingDir = Path.GetDirectoryName(path);

            var startInfo = new ProcessStartInfo()
            {
                FileName         = fileName,
                WorkingDirectory = workingDir,
                UseShellExecute  = false,
                Arguments        = arguments
            };

            return new Process()
            {
                StartInfo = startInfo,
                EnableRaisingEvents = true
            };
        }
    }
}
