using FELauncher.Engine.Sessions;
using FELauncher.Shared.Contracts.Sessions;

namespace FELauncher.Tests.Engine
{
    public sealed class SessionTests
    {
        [Fact]
        public void New_Session_HasCreatedStatus()
        {
            var session = new Session();

            Assert.Equal(SessionStatus.Created, session.Status);
            Assert.False(session.IsActive);
            Assert.False(session.CanRequestStop);
            Assert.Equal(TimeSpan.Zero, session.Runtime);
        }

        [Fact]
        public void Session_StatusStarting_IsActiveAndStoppable()
        {
            var session = new Session();

            session.Status = SessionStatus.Starting;

            Assert.True(session.IsActive);
            Assert.True(session.CanRequestStop);
            Assert.True(session.Runtime >= TimeSpan.Zero);
        }

        [Fact]
        public void Session_StatusCompleted_IsNotActiveAndCannotRequestStop()
        {
            var session = new Session();

            session.Status = SessionStatus.Completed;

            Assert.False(session.IsActive);
            Assert.False(session.CanRequestStop);
        }

        [Fact]
        public void Session_StatusFailed_IsNotActiveAndCannotRequestStop()
        {
            var session = new Session();

            session.Status = SessionStatus.Failed;

            Assert.False(session.IsActive);
            Assert.False(session.CanRequestStop);
        }

        [Fact]
        public async Task Session_Runtime_DoesNotIncreaseAfterSessionComplete()
        {
            var session = new Session
            {
                Status = SessionStatus.Starting
            };

            await Task.Delay(50);
            session.Status = SessionStatus.Completed;
            var first = session.Runtime;

            await Task.Delay(50);
            var second = session.Runtime;

            Assert.True(first > TimeSpan.Zero);
            Assert.Equal(first, second);
        }
    }
}
