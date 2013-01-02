using System;
using NUnit.Framework;
using Retlang.Channels;
using Moq;

namespace Retlang.Test.Channels
{
    [TestFixture]
    public class ChannelFixture : MoqTestFixture
    {
        private Mock<IReceiver<int>> _mock_receiver;
        private IChannel<int> _channel;

        protected override void SetUp()
        {
            _mock_receiver = CreateMock<IReceiver<int>>();
            _channel = new Channel<int>();
        }

        [Test]
        public void Publish_NoReceivers_ReturnsFalse()
        {
            var result = _channel.Publish(0);
            Assert.IsFalse(result);
        }

        [Test]
        public void Subscribe_OneReceiver_DeliversMessage()
        {
            int received = -1;
            _mock_receiver.
                Setup(m => m.Receive(0)).
                Callback<int>(x => received = x);

            var unsubscriber = _channel.Subscribe(_mock_receiver.Object);

            var result = _channel.Publish(0);
            Assert.IsNotNull(unsubscriber);
            Assert.IsTrue(result);
            Assert.AreEqual(0, received);
        }

        [Test]
        public void Unsubscribe_NoReceivers_ReturnsFalse()
        {
            var unsubscriber = _channel.Subscribe(_mock_receiver.Object);
            unsubscriber.Dispose();

            var result = _channel.Publish(0);
            Assert.IsNotNull(unsubscriber);
            Assert.IsFalse(result);
        }
    }
}

