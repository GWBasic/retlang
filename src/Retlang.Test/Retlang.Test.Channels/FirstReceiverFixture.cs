using NUnit.Framework;
using Moq;
using Retlang.Channels;
using Retlang.Fibers;

namespace Retlang.Test.Channels
{
    [TestFixture]
    public class FirstReceiverFixture : MoqTestFixture
    {
        private Mock<IReceiver<int>> _mock_first;
        private Mock<IReceiver<int>> _mock_rest;

        private IReceiver<int> _receiver;

        protected override void SetUp()
        {
            var fiber = new StubFiber();

            _mock_first = CreateMock<IReceiver<int>>();
            _mock_rest = CreateMock<IReceiver<int>>();

            _receiver = new FirstReceiver<int>(fiber,
                _mock_first.Object, _mock_rest.Object);
        }

        [Test]
        public void Receive_MultipleMessages_ShiftsReceivers()
        {
            _mock_first.Setup(m => m.Receive(0));
            _mock_rest.Setup(m => m.Receive(1));
            _mock_rest.Setup(m => m.Receive(2));

            _receiver.Receive(0);
            _receiver.Receive(1);
            _receiver.Receive(2);
        }
    }
}

