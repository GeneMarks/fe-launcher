using Cabby.Client.Shared.Contracts.Sessions;

namespace Cabby.Client.Engine.Sessions
{
    public sealed class SessionStatusProvider : ISessionStatusProvider
    {
        private readonly Lock _lock = new();
        private SessionStatus _currentStatus = SessionStatus.Idle;
        private SessionDto? _currentSession;
        private DateTime? _startedAt;
        private DateTime? _finishedAt;

        public SessionStatus CurrentStatus
        {
            get
            {
                lock (_lock) { return _currentStatus; }
            }
        }

        public SessionDto? CurrentSession
        {
            get
            {
                lock (_lock) { return _currentSession; }
            }
        }

        public bool IsIdle
        {
            get
            {
                lock (_lock) { return IsIdleUnsafe(_currentStatus); }
            }
        }

        public bool IsActive
        {
            get
            {
                lock (_lock) { return IsActiveUnsafe(_currentStatus); }
            }
        }

        public bool IsCancellable
        {
            get
            {
                lock (_lock) { return IsCancellableUnsafe(_currentStatus); }
            }
        }

        public TimeSpan ElapsedTime
        {
            get
            {
                lock (_lock) { return CalculateElapsedTimeUnsafe(_startedAt, _finishedAt); }
            }
        }

        public SessionStatusSnapshot GetSnapshot()
        {
            lock (_lock)
            {
                return new()
                {
                    CurrentStatus = _currentStatus,
                    CurrentSession = _currentSession,
                    IsIdle = IsIdleUnsafe(_currentStatus),
                    IsActive = IsActiveUnsafe(_currentStatus),
                    IsCancellable = IsCancellableUnsafe(_currentStatus),
                    ElapsedTime = CalculateElapsedTimeUnsafe(_startedAt, _finishedAt)
                };
            }
        }

        private static bool IsIdleUnsafe(SessionStatus status)
            => status is SessionStatus.Idle;

        private static bool IsActiveUnsafe(SessionStatus status)
            => status is not (
                SessionStatus.Idle or
                SessionStatus.Pending or
                SessionStatus.Stopping or
                SessionStatus.Completed or
                SessionStatus.Aborted or
                SessionStatus.Failed);

        private static bool IsCancellableUnsafe(SessionStatus status)
            => IsActiveUnsafe(status) && status is not SessionStatus.RunningPostProcesses;

        private static TimeSpan CalculateElapsedTimeUnsafe(DateTime? startedAt, DateTime? finishedAt)
        {
            if (!startedAt.HasValue)
            {
                return TimeSpan.Zero;
            }

            var endTime = finishedAt ?? DateTime.UtcNow;
            return endTime - startedAt.Value;
        }

        internal void ResetState()
        {
            lock (_lock)
            {
                _currentSession = null;
                _currentStatus = SessionStatus.Idle;
                _startedAt = null;
                _finishedAt = null;
            }
        }

        internal void UpdateSession(SessionDto newSession)
        {
            lock (_lock) _currentSession = newSession;
        }

        internal void UpdateStatus(SessionStatus newStatus)
        {
            lock (_lock)
            {
                _currentStatus = newStatus;

                if (newStatus is SessionStatus.Pending)
                {
                    _startedAt ??= DateTime.UtcNow;
                }
                else if (newStatus is
                    SessionStatus.Completed or
                    SessionStatus.Aborted or
                    SessionStatus.Failed)
                {
                    _finishedAt ??= DateTime.UtcNow;
                }
            }
        }
    }
}
