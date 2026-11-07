using Cabby.Client.Shared.Contracts.Processes;

namespace Cabby.Client.Engine.Processes
{
    internal sealed class ProcessDispatcher(IEnumerable<IProcessRunner> runners)
    {
        public async Task DispatchAsync(
            ProcessDto processDto,
            Action<ProcessCompletionContext> processCompletionCallback, 
            CancellationToken ct)
        {
            var runner = ResolveRunner(processDto);
            await runner.RunAsync(processDto, processCompletionCallback, ct);
        }

        private IProcessRunner ResolveRunner(ProcessDto processDto) => processDto.Type switch
        {
            ProcessType.Executable => runners.OfType<WinProcessRunner>().First(),
            // ProcessType.LuaScript => runners.OfType<LuaProcessRunner>().First()
        };
    }
}