namespace FELauncher.Engine.Processes
{
    internal sealed class Win32ProcessExitedEventArgs : EventArgs
    {
        public uint ProcessId { get; init; }
        public uint ExitCode { get; init; }
    }
}
