using FELauncher.Engine.Processes.Logging;
using FELauncher.Shared.Contracts.Engine;
using Microsoft.Extensions.Logging;

namespace FELauncher.Engine.Processes
{
    internal sealed class ProcessFactory(
        ILogger<ProcessFactory> logger,
        ILogger<Process> win32ProcessLogger,
        IPathResolver pathResolver)
    {
        /// <summary>
        /// Attempts to validate the provided executable path and arguments and, if successful, creates
        /// a <see cref="Process"/> configured to start from the executable's directory.
        /// </summary>
        /// <param name="executablePath">The configured path to the executable.</param>
        /// <param name="arguments">The command-line arguments used when starting the executable.</param>
        /// <param name="process">
        /// Contains the created <see cref="Process"/> instance if the method returns <c>true</c>,
        /// otherwise contains <c>null</c>.
        /// </param>
        /// <param name="processCreationFailure">
        /// Contains information describing why process creation failed if the method returns <c>false</c>,
        /// otherwise contains <c>null</c>.
        /// </param>
        /// <returns>
        /// <c>true</c> if the process was successfully validated and created, otherwise <c>false</c>.
        /// </returns>
        public bool TryCreate(
            string? executablePath, string? arguments,
            out Process? process, out ProcessCreationFailure? processCreationFailure)
        {
            process = null;
            processCreationFailure = null;

            var path = pathResolver.ResolvePath(executablePath);

            if (string.IsNullOrEmpty(path))
            {
                logger.EmptyPath();
                processCreationFailure = new ProcessCreationFailure(ProcessCreationFailureReason.EmptyPath);
                return false;
            }

            var fileName = Path.GetFileName(path);

            if (Path.GetExtension(path) is not EngineConstants.ExecutableExtension)
            {
                logger.InvalidFileExt(path, EngineConstants.ExecutableExtension);
                processCreationFailure = new ProcessCreationFailure(ProcessCreationFailureReason.InvalidFileExt, fileName);
                return false;
            }

            if (!File.Exists(path))
            {
                logger.FileNotPresent(path);
                processCreationFailure = new ProcessCreationFailure(ProcessCreationFailureReason.FileNotPresent, fileName);
                return false;
            }

            var pathWithArgs = string.IsNullOrEmpty(arguments) ? path : path + " " + arguments.Trim();
            var workingDir = Path.GetDirectoryName(path);
            var prettyName = Path.GetFileNameWithoutExtension(path);

            process = new Process(win32ProcessLogger, pathWithArgs, workingDir, prettyName);
            return true;
        }
    }
}
