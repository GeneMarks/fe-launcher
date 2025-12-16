namespace FELauncher.Engine.Processes
{
    public sealed class Win32ProcessExitedArgs : EventArgs
    {
        public uint ExitCode { get; init; }
    }
}
