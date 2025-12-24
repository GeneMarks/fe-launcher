using FELauncher.Shared;
using FELauncher.Shared.Contracts;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace FELauncher.UI.Shell.Tray
{
    internal sealed class TrayActionHandler(
        ILogger<TrayActionHandler> logger,
        IHostApplicationLifetime lifetime,
        ISessionOrchestrator sessionOrchestrator)
    {
        public bool IsSessionActive => sessionOrchestrator.IsSessionActive;
        public bool CanEndSession => sessionOrchestrator.CanEndSession;

        public void LaunchFrontend()
            => sessionOrchestrator.StartNewSessionAsync();

        public void EndSession()
            => sessionOrchestrator.RequestEndSession();

        public void OpenSettings()
        {
            // todo
            return;
        }

        public void Exit()
            => lifetime.StopApplication();

        public static void CheckUpdates()
        {
            var psi = new ProcessStartInfo(AppConstants.CheckUpdatesUrl)
            {
                UseShellExecute = true
            };

            try { Process.Start(psi); }
            catch { }
        }
    }
}
