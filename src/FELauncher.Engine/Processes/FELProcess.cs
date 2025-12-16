namespace FELauncher.Engine.Processes
{
    internal sealed record FELProcess(
        Win32Process Process,
        int DelaySeconds,
        bool NotifyOnExit,
        bool EndSessionOnExit) { }
}
