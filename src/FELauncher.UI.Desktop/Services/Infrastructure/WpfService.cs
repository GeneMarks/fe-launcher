using Microsoft.Extensions.Hosting;
using System.Windows;
using System.Windows.Threading;

namespace FELauncher.UI.Desktop.Services.Infrastructure
{
    internal sealed class WpfService : BackgroundService
    {
        private Thread? _thread;
        public readonly TaskCompletionSource _ready = new(TaskCreationOptions.RunContinuationsAsynchronously);

        public Dispatcher? ThreadDispatcher { get; private set; }
        public Task Ready => _ready.Task;

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

        public override async Task StopAsync(CancellationToken ct)
        {
            if (ThreadDispatcher is null) return;

            await ThreadDispatcher.InvokeAsync(() => ThreadDispatcher.InvokeShutdown())
                .Task.ConfigureAwait(false);

            _thread?.Join();

            await base.StopAsync(ct);
        }

        private void StartWpfMessageLoop()
        {
            ThreadDispatcher = Dispatcher.CurrentDispatcher;

            _ = new Application()
            {
                ShutdownMode = ShutdownMode.OnExplicitShutdown
            };

            _ready.SetResult();

            Dispatcher.Run();
        }
    }
}
