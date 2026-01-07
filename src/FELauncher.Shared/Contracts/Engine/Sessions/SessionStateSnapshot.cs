namespace FELauncher.Shared.Contracts.Engine.Sessions
{
    public sealed record SessionStateSnapshot(
        string? Id,
        SessionStatus Status,
        bool IsActive,
        bool CanRequestStop);
}
