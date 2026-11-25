using Cabby.Client.Shared.Contracts.Processes;

namespace Cabby.Client.Engine.Processes
{
    internal interface IProcessRunner
    {
        event EventHandler<ProcessStartedEventArgs>? ProcessStarted;

        Task RunAsync(
            ProcessDto process,
            Func<ProcessCompletionContext, Task>? processCompletionCallbackAsync,
            CancellationToken ct = default);

        Task RunAndWaitAsync(
            ProcessDto process,
            Func<ProcessCompletionContext, Task>? processCompletionCallbackAsync,
            CancellationToken ct = default);
    }
}