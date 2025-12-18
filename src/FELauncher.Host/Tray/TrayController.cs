using FELauncher.Engine.Sessions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace FELauncher.Host.Tray
{
    internal sealed class TrayController(
        ILogger<TrayController> logger,
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
            var psi = new ProcessStartInfo(HostConstants.CheckUpdatesUrl)
            {
                UseShellExecute = true
            };

            try { Process.Start(psi); }
            catch { }
        }
    }
}
