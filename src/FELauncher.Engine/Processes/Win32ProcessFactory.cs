using FELauncher.Engine.Exceptions;
using FELauncher.Engine.IO;
using FELauncher.Engine.Processes.Logging;
using Microsoft.Extensions.Logging;

namespace FELauncher.Engine.Processes
{
    internal sealed class Win32ProcessFactory(
        ILogger<Win32ProcessFactory> logger,
        ILogger<Win32Process> win32ProcessLogger,
        PathResolver pathResolver)
    {
        /// <summary>
        /// Validates the executable path and arguments and returns a <see cref="Win32Process"/> configured to start
        /// from the executable's directory as its working directory.
        /// </summary>
        /// <param name="executablePath">The system path to a valid executable file.</param>
        /// <param name="arguments">The command-line arguments used when starting the executable.</param>
        /// <returns>A configured <see cref="Win32Process"/> instance.</returns>
        /// <exception cref="Win32ProcessCreationException">
        /// Thrown when the path is null/empty, does not have the required extension, or does not exist.
        /// </exception>
        public Win32Process Create(string? executablePath, string? arguments)
        {
            var path = pathResolver.ResolvePath(executablePath);

            if (String.IsNullOrEmpty(path))
            {
                logger.EmptyPath();
                throw new Win32ProcessCreationException("Executable path is null or empty.");
            }

            if (Path.GetExtension(path) is not EngineConstants.ExecutableExtension)
            {
                logger.InvalidFileExt(path, EngineConstants.ExecutableExtension);
                throw new Win32ProcessCreationException($"Executable '{path}' does not contain required file extension: {EngineConstants.ExecutableExtension}");
            }

            if (!File.Exists(path))
            {
                logger.FileNotPresent(path);
                throw new Win32ProcessCreationException($"The specified file '{path}' does not exist.");
            }

            var pathWithArgs = String.IsNullOrEmpty(arguments) ? path : path + " " + arguments.Trim();
            var workingDir = Path.GetDirectoryName(path);

            return new Win32Process(win32ProcessLogger, pathWithArgs, workingDir);
        }
    }
}
