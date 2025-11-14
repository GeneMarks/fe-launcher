using System.Diagnostics;

namespace FELauncher.Engine.Processes
{
    public class ProcessManager : IProcessManager
    {
        public bool StartProcess(string processPath)
        {
            Process.Start(processPath);
            return true;
        }
    }
}