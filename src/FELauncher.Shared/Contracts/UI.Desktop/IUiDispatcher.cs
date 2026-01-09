namespace FELauncher.Shared.Contracts.UI.Desktop
{
    public interface IUiDispatcher
    {
        Task InvokeAsync(Action action, CancellationToken ct = default);
        Task<T> InvokeAsync<T>(Func<T> func, CancellationToken ct = default);
    }
}
