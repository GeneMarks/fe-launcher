namespace FELauncher.Engine.Processes
{
    internal sealed class Win32ProcessExitedEventArgs : EventArgs
    {
        public uint ExitCode { get; init; }
    }
}
