using Cabby.Client.Shared.Contracts.Processes;

namespace Cabby.Client.Engine.Processes
{
    internal sealed class ProcessDispatcher(IEnumerable<IProcessRunner> runners)
    {
        public event EventHandler<ProcessStartedEventArgs>? ProcessStarted;

        public async Task DispatchAsync(
            ProcessDto processDto,
            Func<ProcessCompletionContext, Task> processCompletionCallbackAsync, 
            CancellationToken ct)
        {
            var runner = ResolveRunner(processDto);
            runner.ProcessStarted += OnProcessStarted;
            try
            {
                await runner.RunAsync(processDto, processCompletionCallbackAsync, ct);
            }
            finally
            {
                runner.ProcessStarted -= OnProcessStarted;
            }
        }

        private IProcessRunner ResolveRunner(ProcessDto processDto) => processDto.Type switch
        {
            ProcessType.Executable => runners.OfType<WinProcessRunner>().First(),
            // ProcessType.LuaScript => runners.OfType<LuaProcessRunner>().First()
        };

        private void OnProcessStarted(object? sender, ProcessStartedEventArgs e)
        {
            ProcessStarted?.Invoke(sender, e);
        }
    }
}