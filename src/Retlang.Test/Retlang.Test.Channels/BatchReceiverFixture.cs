using System;
using System.Threading;
using System.Collections.Generic;
using NUnit.Framework;
using Retlang.Channels;

namespace Retlang.Test.Channels
{
    [TestFixture]
    public class BatchReceiverFixture : BaseReceiverFixture<IList<int>>
    {
        protected override void SetUp()
        {
            base.SetUp();

            _receiver = new BatchReceiver<int>(_fiber, Receive, 50);
        }

        [Test]
        public void Receive_MultipleMessages_ReceivesOneBatch()
        {
            for (var x = 0; x < 5; x++)
            {
                _receiver.Receive(x);
            }

            var signaled = _handle.WaitOne(1000);
            Assert.IsTrue(signaled);
            
            Assert.AreEqual(1, _received.Count);
            Assert.AreEqual(new int[] { 0, 1, 2, 3, 4 }, _received[0]);
        }

        [Test]
        public void Receive_MultipleMessagesWithDelay_ReceivesMultipleBatches()
        {
            for (var x = 0; x < 10; x++)
            {
                _receiver.Receive(x);
                Thread.Sleep(25);
            }
            
            var signaled = _handle.WaitOne(1000);
            Assert.IsTrue(signaled);
            
            Assert.AreEqual(5, _received.Count);
            Assert.AreEqual(new int[] { 0, 1 }, _received[0]);
            Assert.AreEqual(new int[] { 2, 3 }, _received[1]);
            Assert.AreEqual(new int[] { 4, 5 }, _received[2]);
            Assert.AreEqual(new int[] { 6, 7 }, _received[3]);
            Assert.AreEqual(new int[] { 8, 9 }, _received[4]);
        }
    }
}

