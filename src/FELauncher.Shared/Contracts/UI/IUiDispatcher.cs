namespace FELauncher.Shared.Contracts.UI
{
    public interface IUiDispatcher
    {
        Task InvokeAsync(Action action, CancellationToken ct = default);
        Task<T> InvokeAsync<T>(Func<T> func, CancellationToken ct = default);
    }
}
