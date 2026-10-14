namespace FELauncher.Engine.Processes.Runner
{
    internal sealed class RunnerProcessExitedEventArgs : EventArgs
    {
        public required string ProcessName { get; init; }
        public required bool NotifyOnExit { get; init; }
        public required bool EndSessionOnExit { get; init; }
    }
}
