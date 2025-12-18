using System.Diagnostics;

namespace FELauncher.Engine.Sessions
{
    public enum SessionStatus
    {
        Created,
        Starting,
        RunningPreHooks,
        RunningPreProcesses,
        RunningFrontend,
        RunningPostHooks,
        Stopping,
        Completed,
        Aborted,
        Failed
    }

    internal sealed class Session
    {
        public Guid Id { get; } = Guid.NewGuid();
        public DateTimeOffset CreatedAt { get; } = DateTimeOffset.UtcNow;
        public SessionStatus Status
        {
            get => _status;
            internal set
            {
                _status = value;
                if (_status is SessionStatus.Starting)
                {
                    _startTime = Stopwatch.GetTimestamp();
                }
                else if (!IsActive && _status is not SessionStatus.Created)
                {
                    _endTime = Stopwatch.GetTimestamp();
                }
            }
        }

        public TimeSpan Runtime
        {
            get
            {
                if (_status is SessionStatus.Created || _startTime == 0)
                {
                    return TimeSpan.Zero;
                }

                if (IsActive || _endTime == 0)
                {
                    return Stopwatch.GetElapsedTime(_startTime);
                }

                return Stopwatch.GetElapsedTime(_startTime, _endTime);
            }
        }

        private SessionStatus _status = SessionStatus.Created;
        private long _startTime;
        private long _endTime;

        public bool IsActive =>
            Status is not (SessionStatus.Created
                        or SessionStatus.Completed
                        or SessionStatus.Aborted
                        or SessionStatus.Failed);

        public bool CanRequestStop =>
            Status is not (SessionStatus.Created
                        or SessionStatus.Stopping
                        or SessionStatus.Completed
                        or SessionStatus.Aborted
                        or SessionStatus.Failed);
    }
}
