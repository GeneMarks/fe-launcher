namespace FELauncher.Engine.Processes
{
    internal sealed class FELProcessExitedEventArgs : EventArgs
    {
        public bool NotifyOnExit { get; init; }
        public bool EndSessionOnExit { get; init; }
    }
}
