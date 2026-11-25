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
            Func<ProcessCompletionContext, Task>? processCompletionCallbackAsync = null, 
            CancellationToken ct = default)
        {
            var createdProcess = factory.TryCreate(
                processDto,
                processCompletionCallbackAsync,
                out var process,
                out var failureReason);

            if (!createdProcess)
            {
                if (processCompletionCallbackAsync is not null)
                {
                    await processCompletionCallbackAsync(new ProcessCompletionContext
                    {
                        ProcessDto = processDto,
                        WasSuccessful = false,
                        FailureMessage = failureReason 
                    });
                }
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

            ProcessStarted?.Invoke(this, new ProcessStartedEventArgs
            {
                ProcessDto = processDto
            });
        }

        public async Task RunAndWaitAsync(
            ProcessDto processDto,
            Func<ProcessCompletionContext, Task>? processCompletionCallbackAsync = null, 
            CancellationToken ct = default)
        {
            var tcs = new TaskCompletionSource<ProcessCompletionContext>(TaskCreationOptions.RunContinuationsAsynchronously);

            await RunAsync(
                processDto,
                async processCompletionContext =>
                {
                    tcs.TrySetResult(processCompletionContext);

                    if (processCompletionCallbackAsync is not null)
                    {
                        await processCompletionCallbackAsync(processCompletionContext);
                    }
                },
                ct);

            await tcs.Task.WaitAsync(ct);
        }
    }
}