using Cabby.Client.Shared.Contracts.Processes;

namespace Cabby.Client.Engine.Processes
{
    internal sealed record ProcessCompletionContext
    {
        public required ProcessDto ProcessDto { get; init; }
        public required bool WasSuccessful { get; init; }
        public string? FailureMessage { get; init; } = null;
    }
}
