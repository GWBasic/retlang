using System;
using NUnit.Framework;
using Retlang.Channels;

namespace Retlang.Test.Channels
{
    [TestFixture]
    public class ThrottleReceiverFixture : BaseReceiverFixture<int>
    {   
        protected override void SetUp()
        {
            base.SetUp();
            
            _receiver = new ThrottleReceiver<int>(_fiber, Receive, 50);
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
        public void Receive_MultipleMessages_ReceivesMessagesThrottledToOnePerInterval()
        {
            for (var x = 0; x < 10; x++)
            {
                _receiver.Receive(x);
                
                if (x % 2 == 1)
                {
                    var signaled = _handle.WaitOne(1000);
                    Assert.IsTrue(signaled);
                }
            }
            
            Assert.AreEqual(5, _received.Count);
            Assert.AreEqual(new int[] { 1, 3, 5, 7, 9 }, _received);
        }
    }
}

