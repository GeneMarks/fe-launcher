using System.Diagnostics;

namespace FELauncher.Engine.Processes
{
    public sealed class ProcessHandle
    {
        public required Process FELProcess { get; init; }
        public required Task Completion { get; init; }
        public bool IsRunning => !Completion.IsCompleted;
    }
}
