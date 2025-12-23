namespace FELauncher.Engine.Processes
{
    internal sealed class ProcessExitedEventArgs : EventArgs
    {
        public required uint ProcessId { get; init; }
        public required string ProcessPath { get; init; }
        public required string ProcessName { get; init; }
        public required uint ExitCode { get; init; }
    }
}
