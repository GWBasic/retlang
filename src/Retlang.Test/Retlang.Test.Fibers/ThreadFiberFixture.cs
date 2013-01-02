using System;
using NUnit.Framework;
using Retlang.Fibers;
using System.Threading;

namespace Retlang.Test.Fibers
{
    [TestFixture]
    public class ThreadFiberFixture : MoqTestFixture
    {
        private ThreadFiber _fiber;

        protected override void SetUp()
        {
            _fiber = new ThreadFiber();
            _fiber.Start();
        }

        [Test]
        public void Start_CalledTwice_Exception()
        {
            Assert.Throws<ThreadStateException>(_fiber.Start);
        }

        [Test]
        public void Assert_SameThread_NoException()
        {
            var handle = new AutoResetEvent(false);
            Exception exception = null;

            _fiber.Enqueue(() =>
            {
                try
                {
                    _fiber.Assert();
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
                finally
                {
                    handle.Set();
                }
            });

            handle.WaitOne();
            Assert.IsNull(exception);
        }

        [Test]
        public void Assert_DifferentThread_Exception()
        {
            Assert.Throws<ThreadStateException>(_fiber.Assert);
        }
    }
}

