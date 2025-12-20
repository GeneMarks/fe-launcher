namespace FELauncher.Engine.Processes
{
    internal sealed class Win32ProcessExitedEventArgs : EventArgs
    {
        public required uint ProcessId { get; init; }
        public required string ProcessPath { get; init; }
        public required uint ExitCode { get; init; }
    }
}
