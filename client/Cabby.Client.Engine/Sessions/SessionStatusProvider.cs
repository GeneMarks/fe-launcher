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
                lock (_lock) return _currentStatus is SessionStatus.Idle;
            }
        }

        public bool IsActive
        {
            get
            {
                lock (_lock)
                {
                    return _currentStatus is not (
                        SessionStatus.Idle or
                        SessionStatus.Pending or
                        SessionStatus.Stopping or
                        SessionStatus.Completed or
                        SessionStatus.Aborted or
                        SessionStatus.Failed);
                }
            }
        }

        public TimeSpan ElapsedTime
        {
            get
            {
                lock (_lock)
                {
                    if (!_startedAt.HasValue)
                    {
                        return TimeSpan.Zero;
                    }

                    var endTime = _finishedAt ?? DateTime.UtcNow;
                    return endTime - _startedAt.Value;
                }
            }
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
