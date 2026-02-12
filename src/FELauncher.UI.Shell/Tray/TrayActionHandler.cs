using FELauncher.Shared;
using FELauncher.Shared.Contracts.Sessions;
using FELauncher.Shared.Contracts.UI.Windows;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;

namespace FELauncher.UI.Shell.Tray
{
    internal sealed class TrayActionHandler(
        IHostApplicationLifetime lifetime,
        ISessionOrchestrator sessionOrchestrator,
        ISettingsWindowService settingsWindowService)
    {
        public SessionStateSnapshot GetSessionState()
            => sessionOrchestrator.GetSessionState();

        public void LaunchFrontend()
            => _ = sessionOrchestrator.StartNewSessionAsync();

        public void EndSession()
            => sessionOrchestrator.RequestCancelSession();

        public void OpenSettings()
            => _ = settingsWindowService.ShowWindowAsync();

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
