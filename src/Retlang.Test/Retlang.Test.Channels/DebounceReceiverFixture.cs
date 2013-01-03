using System;
using System.Threading;
using System.Collections.Generic;
using NUnit.Framework;
using Retlang.Channels;
using Retlang.Fibers;

namespace Retlang.Test.Channels
{
    [TestFixture]
    public class DebounceReceiverFixture : MoqTestFixture
    {
        private IList<int> _received;
        private AutoResetEvent _handle;
        private IFiber _fiber;
        private DebounceReceiver<int> _receiver;

        protected override void SetUp()
        {
            _received = new List<int>();
            _handle = new AutoResetEvent(false);
            _fiber = new PoolFiber();
            _fiber.Start();

            _receiver = new DebounceReceiver<int>(_fiber, x =>
            {
                _received.Add(x);
                _handle.Set();
            }, 50);
        }

        [Test]
        public void Receive_OneMessage_ReceivesMessage()
        {
            _receiver.Receive(0);

            var signaled = _handle.WaitOne(1000);
            Assert.IsTrue(signaled);
            Assert.AreEqual(1, _received.Count);
            Assert.AreEqual(0, _received[0]);
        }

        [Test]
        public void Receive_MultipleMessages_ReceivesLastMessage()
        {
            for (var x = 0; x < 10; x++)
            {
                _receiver.Receive(x);
            }

            var signaled = _handle.WaitOne(1000);
            Assert.IsTrue(signaled);
            Assert.AreEqual(1, _received.Count);
            Assert.AreEqual(9, _received[0]);
        }
    }
}

