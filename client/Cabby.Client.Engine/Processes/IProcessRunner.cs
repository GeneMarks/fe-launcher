using Cabby.Client.Shared.Contracts.Processes;

namespace Cabby.Client.Engine.Processes
{
    internal interface IProcessRunner
    {
        Task RunAsync(
            ProcessDto process,
            Action<ProcessCompletionContext> processCompletionCallback,
            CancellationToken ct = default);
    }
}