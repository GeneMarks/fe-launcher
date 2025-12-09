using System.Diagnostics;

namespace FELauncher.Engine.Processes
{
    internal sealed record FELPreProcess(
        Process Process,
        int DelaySeconds,
        bool NotifyOnExit,
        bool EndSessionOnExit) : FELProcess(Process) { }
}
