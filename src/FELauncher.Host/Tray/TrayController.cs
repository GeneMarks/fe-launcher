using FELauncher.Engine.Processes;
using FELauncher.Engine.Settings;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace FELauncher.Host.Tray
{
    public class TrayController : ITrayController
    {
        private readonly IHostApplicationLifetime _lifetime;
        private readonly IProcessManager _processManager;
        private readonly FrontendSettings _frontendSettings;

        public TrayController(IHostApplicationLifetime lifetime, IProcessManager processManager, IOptionsMonitor<FrontendSettings> frontendSettings)
        {
            _lifetime = lifetime;
            _processManager = processManager;
            _frontendSettings = frontendSettings.CurrentValue;
        }

        public void LaunchFrontend()
        {
            _processManager.StartProcess(_frontendSettings.FrontendPath);
        }

        public void OpenSettings()
        {
            return;
        }

        public void Exit()
        {
            _lifetime.StopApplication();
        }
    }
}
