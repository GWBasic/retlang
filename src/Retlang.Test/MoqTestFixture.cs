using System;
using Moq;
using NUnit.Framework;

namespace Retlang.Test
{
    public abstract class MoqTestFixture
    {
        private MockRepository _repository;

        [SetUp]
        public void BaseSetUp()
        {
            _repository = new MockRepository(MockBehavior.Default) {
                DefaultValue = DefaultValue.Mock,
            };
            SetUp();
        }

        [TearDown]
        public void BaseTearDown()
        {
            TearDown();
            _repository.VerifyAll();
        }

        protected Mock<T> CreateMock<T>(MockBehavior behavior = MockBehavior.Default)
            where T : class
        {
            return _repository.Create<T>(behavior);
        }

        protected virtual void SetUp()
        {

        }

        protected virtual void TearDown()
        {

        }
    }
}

