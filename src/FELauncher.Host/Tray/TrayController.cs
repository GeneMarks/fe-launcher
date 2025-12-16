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

        public void LaunchFrontend()
        {
            var ct = sessionManager.CancellationTokenSource.Token;
            sessionManager.StartNewSessionAsync(ct);
        }

        public void EndSession()
        {
            var cts = sessionManager.CancellationTokenSource;
            cts.Cancel();
        }

        public void OpenSettings()
        {
            // todo
            return;
        }

        public void Exit()
        {
            lifetime.StopApplication();
        }

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
