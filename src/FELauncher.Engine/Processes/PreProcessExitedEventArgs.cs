namespace FELauncher.Engine.Processes
{
    public sealed class PreProcessExitedEventArgs : EventArgs
    {
        public int ExitCode { get; init; }
        public bool NotifyOnExit { get; init; }
        public bool EndSessionOnExit { get; init; }
    }
}
