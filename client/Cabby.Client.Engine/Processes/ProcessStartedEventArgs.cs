using Cabby.Client.Shared.Contracts.Processes;

namespace Cabby.Client.Engine.Processes
{
    internal sealed class ProcessStartedEventArgs : EventArgs
    {
        public required ProcessDto ProcessDto { get; init; }
    }
}
