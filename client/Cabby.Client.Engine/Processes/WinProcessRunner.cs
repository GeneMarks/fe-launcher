using Cabby.Client.Engine.Processes.Logging;
using Cabby.Client.Shared.Contracts.Processes;
using Microsoft.Extensions.Logging;

namespace Cabby.Client.Engine.Processes
{
    internal sealed class WinProcessRunner(
        ILogger<WinProcessRunner> logger,
        WinProcessFactory factory,
        JobObjectManager jobObjectManager) : IProcessRunner
    {
        public event EventHandler<ProcessStartedEventArgs>? ProcessStarted;

        public async Task RunAsync(
            ProcessDto processDto,
            Func<ProcessCompletionContext, Task> processCompletionCallbackAsync, 
            CancellationToken ct = default)
        {
            var createdProcess = factory.TryCreate(
                processDto: processDto,
                processCompletionCallbackAsync: processCompletionCallbackAsync,
                out var process,
                out var failureReason);

            if (!createdProcess)
            {
                await processCompletionCallbackAsync(new ProcessCompletionContext
                {
                    ProcessDto = processDto,
                    WasSuccessful = false,
                    FailureMessage = failureReason 
                });
                return;
            }

            ct.ThrowIfCancellationRequested();
            var delayMs = processDto.DelayMs;
            var delayTime = TimeSpan.FromMilliseconds(delayMs);
            if (delayTime > TimeSpan.Zero)
            {
                logger.WaitingDelay(delayMs, process!.PrettyName, process.PathWithArgs);
                await Task.Delay(delayTime, ct);
            }

            ct.ThrowIfCancellationRequested();
            logger.StartingProcess(process!.PrettyName, process.PathWithArgs);
            process.StartInJob(jobObjectManager.SafeJobHandle);

            ProcessStarted?.Invoke(null, new ProcessStartedEventArgs
            {
                ProcessDto = processDto
            });
        }
    }
}