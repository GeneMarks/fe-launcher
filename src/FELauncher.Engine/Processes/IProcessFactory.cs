using System.Diagnostics;

namespace FELauncher.Engine.Processes
{
    /// <summary>
    /// Factory that produces system Process objects.
    /// </summary>
    internal interface IProcessFactory
    {
        /// <summary>
        /// Create a system process based on the provided path and arguments.
        /// Verifies that the provided path is valid before creation.
        /// <br /><br />
        /// The process's working directory is always set to the executable path's directory.
        /// <br />
        /// Event raising is always enabled for the created process.
        /// </summary>
        /// <param name="executablePath">The system path to a valid executable file.</param>
        /// <param name="arguments">The command-line arguments used when starting the executable.</param>
        /// <returns>A verified system Process object.</returns>
        /// <exception cref="ProcessCreationException">Thrown if factory is unable to create a Process from an invalid path.</exception>
        Process Create(string? executablePath, string? arguments);
    }
}
