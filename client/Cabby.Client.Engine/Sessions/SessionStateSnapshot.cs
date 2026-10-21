namespace Cabby.Client.Engine.Sessions
{
    public sealed record SessionStateSnapshot(
        string? Id,
        SessionStatus Status,
        bool IsActive,
        bool CanRequestStop);
}
