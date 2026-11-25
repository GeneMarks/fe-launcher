using Cabby.Client.Shared.Contracts.Processes;

namespace Cabby.Client.Engine.Processes
{
    internal sealed class ProcessDispatcher(IEnumerable<IProcessRunner> runners)
    {
        public event EventHandler<ProcessStartedEventArgs>? ProcessStarted;

        public Task DispatchAsync(
            ProcessDto processDto,
            Func<ProcessCompletionContext, Task>? processCompletionCallbackAsync = null, 
            CancellationToken ct = default)
            => ExecuteWithRunnerAsync(
                processDto,
                runner => runner.RunAsync(processDto, processCompletionCallbackAsync, ct)
            );

        public Task DispatchAndWaitAsync(
            ProcessDto processDto,
            Func<ProcessCompletionContext, Task>? processCompletionCallbackAsync = null, 
            CancellationToken ct = default)
            => ExecuteWithRunnerAsync(
                processDto,
                runner => runner.RunAndWaitAsync(processDto, processCompletionCallbackAsync, ct)
            );

        private async Task ExecuteWithRunnerAsync(
            ProcessDto processDto,
            Func<IProcessRunner, Task> actionAsync
        )
        {
            var runner = ResolveRunner(processDto);
            runner.ProcessStarted += OnProcessStarted;
            try
            {
                await actionAsync(runner);
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