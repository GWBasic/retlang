using System;
using System.Threading;
using System.Collections.Generic;
using NUnit.Framework;
using Retlang.Channels;
using Retlang.Fibers;

namespace Retlang.Test.Channels
{
    public abstract class BaseReceiverFixture<T> : MoqTestFixture
    {
        protected IList<T> _received;
        protected AutoResetEvent _handle;
        protected IFiber _fiber;
        protected IReceiver<int> _receiver;

        protected override void SetUp()
        {
            _received = new List<T>();
            _handle = new AutoResetEvent(false);
            _fiber = new PoolFiber();
            _fiber.Start();
        }

        protected void Receive(T message)
        {
            _received.Add(message);
            _handle.Set();
        }

        [Test]
        public void TestSubscribeThroughMulticastEvents()
        {
            var channel = new Channel<int>();
            channel.Published += new Receiver<int>(
                _fiber,
                i => _handle.Set());

            channel.Publish(44);

            Assert.IsTrue(
                _handle.WaitOne(TimeSpan.FromMilliseconds(500)),
                "Callback never called");
        }
    }
}

