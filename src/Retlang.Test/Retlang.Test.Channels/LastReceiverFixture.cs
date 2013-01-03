using System;
using NUnit.Framework;
using Retlang.Channels;
using Retlang.Fibers;
using System.Collections.Generic;

namespace Retlang.Test.Channels
{
    [TestFixture]
    public class LastReceiverFixture : MoqTestFixture
    {
        private IList<int> _received;
        private StubFiber _fiber;
        private LastReceiver<int> _receiver;
        
        protected override void SetUp()
        {
            _received = new List<int>();
            _fiber = new StubFiber();
            
            _receiver = new LastReceiver<int>(_fiber, x => _received.Add(x), 1);
        }
        
        [Test]
        public void Receive_OneMessage_ReceivesMessage()
        {
            _receiver.Receive(0);
            _fiber.ExecuteAllScheduled();
            
            Assert.AreEqual(1, _received.Count);
            Assert.AreEqual(0, _received[0]);
        }
        
        [Test]
        public void Receive_MultipleMessages_ReceivesLastMessagePerInterval()
        {
            for (var x = 0; x < 10; x++)
            {
                _receiver.Receive(x);
                
                if (x % 2 == 1)
                {
                    _fiber.ExecuteAllScheduled();
                }
            }
            
            Assert.AreEqual(5, _received.Count);
            Assert.AreEqual(new int[] { 1, 3, 5, 7, 9 }, _received);
        }
    }
}

