namespace FELauncher.Engine.Processes
{
    public sealed record FELProcess(
        Win32Process Process,
        int DelaySeconds,
        bool NotifyOnExit,
        bool EndSessionOnExit) { }
}
