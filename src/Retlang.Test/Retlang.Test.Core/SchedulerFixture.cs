using System;
using NUnit.Framework;
using Moq;
using Retlang.Core;

namespace Retlang.Test.Core
{
    [TestFixture]
    public class SchedulerFixture : MoqTestFixture
    {
        private Mock<IExecutionContext> _mockExecutionContext;
        private IScheduler _scheduler;

        protected override void SetUp()
        {
            _mockExecutionContext = CreateMock<IExecutionContext>();
            _scheduler = new Scheduler(_mockExecutionContext.Object);
        }

        [Test]
        public void ScheduleDebounce_NullParameter_NoOp()
        {
            _scheduler.ScheduleDebounce(null, () => { }, 1);
        }

        [Test]
        public void ScheduleDebounce_NonNullParameter_DisposesScheduled()
        {
            var mock_disposable = CreateMock<IDisposable>();
            mock_disposable.Setup(m => m.Dispose());

            _scheduler.ScheduleDebounce(mock_disposable.Object, () => { }, 1);
        }
    }
}

