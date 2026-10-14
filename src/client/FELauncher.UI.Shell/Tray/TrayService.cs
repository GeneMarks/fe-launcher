using Microsoft.Extensions.Hosting;

namespace FELauncher.UI.Shell.Tray
{
    internal sealed class TrayService(TrayActionHandler handler) : BackgroundService
    {
        private Thread? _thread;
        private TrayWindowHost? _tray;

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _tray = new TrayWindowHost(handler);

            _thread = new Thread(_tray.Run)
            {
                Name = "TrayWindowHostThread",
                IsBackground = true
            };

            _thread.SetApartmentState(ApartmentState.STA);
            _thread.Start();

            return Task.CompletedTask;
        }

        public override Task StopAsync(CancellationToken ct)
        {
            _tray?.Stop();
            return base.StopAsync(ct);
        }
    }
}

