using System;
using NUnit.Framework;
using Retlang.Fibers;
using System.Threading;

namespace Retlang.Test.Fibers
{
    [TestFixture]
    public class StubFiberFixture : MoqTestFixture
    {
        private StubFiber _fiber;

        protected override void SetUp()
        {
            _fiber = new StubFiber();
        }

        [Test]
        public void Assert_Always_NoException()
        {
            Assert.DoesNotThrow(_fiber.Assert);
        }
    }
}

