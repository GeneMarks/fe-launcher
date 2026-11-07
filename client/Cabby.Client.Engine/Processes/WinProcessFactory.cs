using Cabby.Client.Engine.Processes.Logging;
using Cabby.Client.Shared.Contracts.Core;
using Cabby.Client.Shared.Contracts.Processes;
using Microsoft.Extensions.Logging;

namespace Cabby.Client.Engine.Processes
{
    internal sealed class WinProcessFactory(
        ILogger<WinProcessFactory> logger,
        ILogger<WinProcess> winProcessLogger,
        IPathResolver pathResolver)
    {
        /// <summary>
        /// Attempts to validate the provided executable path and arguments and, if successful, creates
        /// a <see cref="WinProcess"/> configured to start from the executable's directory.
        /// </summary>
        /// <param name="processDto">A reference to the <see cref="ProcessDto"/>.</param>
        /// <param name="processCompletionCallback">The <see cref="Action<ProcessCompletionContext>"/> containing the callback that will execute process exit.</param>
        /// <param name="process">
        /// Contains the created <see cref="WinProcess"/> instance if the method returns <c>true</c>,
        /// otherwise contains <c>null</c>.
        /// </param>
        /// <param name="failureReason">
        /// Contains the reason why process creation failed if the method returns <c>false</c>,
        /// otherwise contains <c>null</c>.
        /// </param>
        /// <returns>
        /// <c>true</c> if the process was successfully validated and created, otherwise <c>false</c>.
        /// </returns>
        public bool TryCreate(
            ProcessDto processDto,
            Action<ProcessCompletionContext> processCompletionCallback, 
            out WinProcess? process,
            out string? failureReason)
        {
            process = null;
            failureReason = null;

            var path = pathResolver.ResolvePath(processDto.Target);

            if (string.IsNullOrEmpty(path))
            {
                logger.EmptyPath();
                failureReason = "The provided executable path is empty.";
                return false;
            }

            if (Path.GetExtension(path) is not EngineConstants.ExecutableExtension)
            {
                logger.InvalidFileExt(path, EngineConstants.ExecutableExtension);
                failureReason = $"The file '{path}' is not a valid executable.\nRequired file type: {EngineConstants.ExecutableExtension}";
                return false;
            }

            if (!File.Exists(path))
            {
                logger.FileNotPresent(path);
                failureReason = $"The file '{path}' does not exist.";
                return false;
            }

            var arguments = processDto.Arguments;
            var pathWithArgs = string.IsNullOrEmpty(arguments) ? path : path + " " + arguments.Trim();
            var workingDir = Path.GetDirectoryName(path);
            var prettyName = Path.GetFileNameWithoutExtension(path);

            process = new WinProcess()
            {
                ProcessDto = processDto,
                Logger = winProcessLogger,
                PathWithArgs = pathWithArgs,
                WorkingDir = workingDir,
                PrettyName = prettyName,
                ProcessCompletionCallback = processCompletionCallback
            };

            return true;
        }
    }
}
