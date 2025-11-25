using Microsoft.Extensions.Hosting;

namespace FELauncher.Host.Tray
{
    public class TrayService : BackgroundService
    {
        private readonly ITrayController _controller;
        private Thread? _thread;
        private TrayWindowHost? _tray;

        public TrayService(ITrayController controller)
        {
            _controller = controller;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _tray = new TrayWindowHost(_controller);

            _thread = new Thread(_tray.Run)
            {
                IsBackground = true,
                Name = "TrayWindowHostThread"
            };

            _thread.SetApartmentState(ApartmentState.STA);
            _thread.Start();

            stoppingToken.Register(() => _tray.Stop());

            return Task.CompletedTask;
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _tray?.Stop();
            return base.StopAsync(cancellationToken);
        }
    }
}

