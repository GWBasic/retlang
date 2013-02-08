using System;
using NUnit.Framework;
using Moq;
using Retlang.Core;
using Retlang.Fibers;

namespace Retlang.Test.Core
{
    [TestFixture]
    public class ExecutionContextExtensionsTests
    {
        private void TestWithFiber(IFiber fiber)
        {
            fiber.Start();

            var called = false;
            fiber.Lock(() => called = true);

            Assert.IsTrue(called, "Lock doesn't work for " + fiber.GetType().Name);
        }

        private class TestException : Exception {}

        private void TestExceptionWithFiber(IFiber fiber)
        {
            fiber.Start();

            Assert.Throws<TestException>(() =>
                fiber.Lock(() =>
                {
                    throw new TestException();
                }));
        }

        [Test]
        public void TestWithThreadFiber()
        {
            using (var threadFiber = new ThreadFiber("unit test"))
            {
                this.TestWithFiber(threadFiber);
            }
        }

        [Test]
        public void TestWithThreadFiber_Exception()
        {
            using (var threadFiber = new ThreadFiber("unit test"))
            {
                this.TestExceptionWithFiber(threadFiber);
            }
        }

        [Test]
        public void TestWithThreadPoolFiber()
        {
            using (var threadPoolFiber = new PoolFiber())
            {
                this.TestWithFiber(threadPoolFiber);
            }
        }
        
        [Test]
        public void TestWithThreadPoolFiber_Exception()
        {
            using (var threadPoolFiber = new PoolFiber())
            {
                this.TestExceptionWithFiber(threadPoolFiber);
            }
        }

        [Test]
        public void TestWithStubFiber()
        {
            using (var stubFiber = new StubFiber())
            {
                this.TestWithFiber(stubFiber);
            }
        }

        [Test]
        public void TestWithStubFiber_Exception()
        {
            using (var stubFiber = new StubFiber())
            {
                this.TestExceptionWithFiber(stubFiber);
            }
        }
    }
}

