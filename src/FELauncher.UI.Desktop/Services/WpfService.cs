using Microsoft.Extensions.Hosting;
using System.Windows;
using System.Windows.Threading;

namespace FELauncher.UI.Desktop.Services
{
    internal sealed class WpfService : BackgroundService
    {
        private Thread? _thread;
        private Dispatcher? _dispatcher;
        private readonly TaskCompletionSource _ready = new(TaskCreationOptions.RunContinuationsAsynchronously);

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _thread = new Thread(StartWpfMessageLoop)
            {
                Name = "WpfThread",
                IsBackground = true
            };

            _thread.SetApartmentState(ApartmentState.STA);
            _thread.Start();

            return Task.CompletedTask;
        }

        public async Task InvokeAsync(Action action)
        {
            await _ready.Task.ConfigureAwait(false);
            await _dispatcher!.InvokeAsync(action).Task.ConfigureAwait(false);
        }

        public override async Task StopAsync(CancellationToken ct)
        {
            if (_dispatcher is null) return;

            await _dispatcher.InvokeAsync(() => _dispatcher.InvokeShutdown())
                .Task.ConfigureAwait(false);

            await base.StopAsync(ct);
        }

        private void StartWpfMessageLoop()
        {
            _dispatcher = Dispatcher.CurrentDispatcher;

            _ = new Application()
            {
                ShutdownMode = ShutdownMode.OnExplicitShutdown
            };

            _ready.SetResult();

            Dispatcher.Run();
        }
    }
}
