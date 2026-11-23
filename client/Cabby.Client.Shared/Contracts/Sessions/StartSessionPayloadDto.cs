namespace Cabby.Client.Shared.Contracts.Sessions
{
    public sealed record StartSessionPayloadDto
    {
        public required SessionDto Session { get; init; }
    }
}