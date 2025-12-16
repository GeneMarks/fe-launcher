using FELauncher.Engine.Exceptions;
using FELauncher.Engine.IO;
using Microsoft.Extensions.Logging;

namespace FELauncher.Engine.Processes
{
    internal sealed class ProcessFactory(
        ILogger<ProcessFactory> logger,
        ILogger<Win32Process> win32ProcessLogger,
        IPathResolver pathResolver) : IProcessFactory
    {
        public Win32Process Create(string? executablePath, string? arguments)
        {
            var path = pathResolver.ResolvePath(executablePath);

            if (String.IsNullOrEmpty(path))
            {
                logger.EmptyPath();
                throw new ProcessCreationException("Executable path is null or empty.");
            }

            if (Path.GetExtension(path) is not EngineConstants.ExecutableExtension)
            {
                logger.InvalidFileExt(path, EngineConstants.ExecutableExtension);
                throw new ProcessCreationException($"Executable '{path}' does not contain required file extension: {EngineConstants.ExecutableExtension}");
            }

            if (!File.Exists(path))
            {
                logger.FileNotPresent(path);
                throw new ProcessCreationException($"The specified file '{path}' does not exist.");
            }

            var pathWithArgs = String.IsNullOrEmpty(arguments) ? path : path + " " + arguments.Trim();
            var workingDir = Path.GetDirectoryName(path);

            return new Win32Process(win32ProcessLogger, pathWithArgs, workingDir);
        }
    }
}
