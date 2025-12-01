namespace FELauncher.Engine.Processes
{
	public interface IProcessManager
	{
		/// <summary>
		/// Attempt to create a process from a string path with arguments and execute it.
		/// Return a result with a handle to the successfully started process.
		/// </summary>
	    ProcessStartResult StartProcess(string? executablePath, string? arguments);

		/// <summary>
		/// Attempt to kill a process.
		/// Return true if process successfully stopped.
		/// </summary>
		bool KillProcess(ProcessHandle processHandle);
	}
}