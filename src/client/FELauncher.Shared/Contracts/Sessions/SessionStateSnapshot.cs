namespace FELauncher.Shared.Contracts.Sessions
{
    public sealed record SessionStateSnapshot(
        string? Id,
        SessionStatus Status,
        bool IsActive,
        bool CanRequestStop);
}
