using FELauncher.Engine.Exceptions;
using FELauncher.Engine.Processes.Logging;
using FELauncher.Shared.Contracts;
using Microsoft.Extensions.Logging;

namespace FELauncher.Engine.Processes
{
    internal sealed class ProcessFactory(
        ILogger<ProcessFactory> logger,
        ILogger<Process> win32ProcessLogger,
        IPathResolver pathResolver)
    {
        /// <summary>
        /// Validates the executable path and arguments and returns a <see cref="Process"/> configured to start
        /// from the executable's directory as its working directory.
        /// </summary>
        /// <param name="executablePath">The system path to a valid executable file.</param>
        /// <param name="arguments">The command-line arguments used when starting the executable.</param>
        /// <returns>A configured <see cref="Process"/> instance.</returns>
        /// <exception cref="ProcessCreationException">
        /// Thrown when the path is null/empty, does not have the required extension, or does not exist.
        /// </exception>
        public Process Create(string? executablePath, string? arguments)
        {
            var path = pathResolver.ResolvePath(executablePath);

            if (string.IsNullOrEmpty(path))
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

            var pathWithArgs = string.IsNullOrEmpty(arguments) ? path : path + " " + arguments.Trim();
            var workingDir = Path.GetDirectoryName(path);
            var prettyName = Path.GetFileNameWithoutExtension(path);

            return new Process(win32ProcessLogger, pathWithArgs, workingDir, prettyName);
        }
    }
}
