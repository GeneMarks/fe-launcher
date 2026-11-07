namespace Cabby.Client.Shared.Contracts.Sessions
{
    public interface ISessionStatusProvider
    {
        SessionStatus CurrentStatus { get; }
        SessionDto? CurrentSession { get; }
        bool IsActive { get; }
        TimeSpan ElapsedTime { get; }
    }
}