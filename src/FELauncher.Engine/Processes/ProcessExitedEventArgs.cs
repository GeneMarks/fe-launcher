namespace FELauncher.Engine.Processes
{
    public sealed class ProcessExitedEventArgs : EventArgs
    {
        public bool NotifyOnExit { get; init; }
        public bool EndSessionOnExit { get; init; }
    }
}
