using System;
using NUnit.Framework;
using Retlang.Channels;

namespace Retlang.Test.Channels
{
    [TestFixture]
    public class DebounceReceiverFixture : BaseReceiverFixture<int>
    {
        protected override void SetUp()
        {
            base.SetUp();

            _receiver = new DebounceReceiver<int>(_fiber, Receive, 50);
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
            for (var x = 0; x < 5; x++)
            {
                _receiver.Receive(x);
            }

            var signaled = _handle.WaitOne(1000);
            Assert.IsTrue(signaled);
            Assert.AreEqual(1, _received.Count);
            Assert.AreEqual(4, _received[0]);
        }
    }
}

