namespace Cabby.Client.Shared.Contracts.Sessions
{
    public sealed record StopSessionPayloadDto
    {
        public required string Id { get; init; }
    }
}