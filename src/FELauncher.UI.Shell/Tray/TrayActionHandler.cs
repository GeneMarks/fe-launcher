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
        ISessionManager sessionManager)
    {
        public bool IsSessionActive => sessionManager.IsSessionActive;
        public bool CanEndSession => sessionManager.CanEndSession;

        public void LaunchFrontend()
            => sessionManager.StartNewSessionAsync();

        public void EndSession()
            => sessionManager.RequestEndSession();

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
