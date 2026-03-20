using FELauncher.Shared.Contracts.UI;

namespace FELauncher.UI.Desktop.Services.Infrastructure
{
    internal sealed class WpfDispatcher(WpfService wpf) : IUiDispatcher
    {
        public async Task InvokeAsync(Action action, CancellationToken ct)
        {
            await wpf.Ready.ConfigureAwait(false);
            ct.ThrowIfCancellationRequested();
            await wpf.ThreadDispatcher!.InvokeAsync(action).Task.ConfigureAwait(false);
        }

        public async Task<T> InvokeAsync<T>(Func<T> func, CancellationToken ct)
        {
            await wpf.Ready.ConfigureAwait(false);
            ct.ThrowIfCancellationRequested();
            return await wpf.ThreadDispatcher!.InvokeAsync(func).Task.ConfigureAwait(false);
        }
    }
}
