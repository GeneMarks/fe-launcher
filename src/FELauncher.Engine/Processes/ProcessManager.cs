using FELauncher.Engine.IO;
using System.Diagnostics;

namespace FELauncher.Engine.Processes
{
    public class ProcessManager : IProcessManager
    {
        private readonly IPathResolver _pathResolver;

        public ProcessManager(IPathResolver pathResolver)
        {
            _pathResolver = pathResolver;
        }

        public void StartProcess(string processPath)
        {
            var normalizedPath = _pathResolver.ResolvePath(processPath);

            var startInfo = new ProcessStartInfo(normalizedPath);
            startInfo.WorkingDirectory = Path.GetDirectoryName(normalizedPath);

            Process.Start(startInfo);
        }
    }
}