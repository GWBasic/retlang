using System;
using NUnit.Framework;
using Retlang.Channels;

namespace Retlang.Test.Channels
{
    [TestFixture]
    public class ReceiverFixture : BaseReceiverFixture<int>
    {
        protected override void SetUp()
        {
            base.SetUp();

            _receiver = new Receiver<int>(_fiber, Receive);
        }

        [Test]
        public void Receive_MultipleMessages_ReceivesAllMessages()
        {
            for (var x = 0; x < 5; x++)
            {
                _receiver.Receive(x);

                var signaled = _handle.WaitOne(1000);
                Assert.IsTrue(signaled);
            }

            Assert.AreEqual(5, _received.Count);
            Assert.AreEqual(new int[] { 0, 1, 2, 3, 4 }, _received);
        }
    }
}

