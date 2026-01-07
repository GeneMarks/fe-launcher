using FELauncher.Shared;
using FELauncher.Shared.Contracts.Engine.Sessions;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;

namespace FELauncher.UI.Shell.Tray
{
    internal sealed class TrayActionHandler(
        IHostApplicationLifetime lifetime,
        ISessionOrchestrator sessionOrchestrator)
    {
        public SessionStateSnapshot GetSessionState()
            => sessionOrchestrator.GetSessionState();

        public void LaunchFrontend()
            => _ = sessionOrchestrator.StartNewSessionAsync();

        public void EndSession()
            => sessionOrchestrator.RequestCancelSession();

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
