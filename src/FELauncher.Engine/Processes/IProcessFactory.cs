namespace FELauncher.Engine.Processes
{
    /// <summary>
    /// Factory that produces Win32Process objects.
    /// </summary>
    internal interface IProcessFactory
    {
        /// <summary>
        /// Create a win32 process based on the provided path and arguments.
        /// <br />
        /// Verifies that the provided path is valid before creation.
        /// <br /><br />
        /// The process's working directory is always set to the executable path's directory.
        /// </summary>
        /// <param name="executablePath">The system path to a valid executable file.</param>
        /// <param name="arguments">The command-line arguments used when starting the executable.</param>
        /// <returns>A verified win32 process object.</returns>
        /// <exception cref="Win32ProcessCreationException">Thrown if factory is unable to create a win32 process from an invalid path.</exception>
        Win32Process Create(string? executablePath, string? arguments);
    }
}
