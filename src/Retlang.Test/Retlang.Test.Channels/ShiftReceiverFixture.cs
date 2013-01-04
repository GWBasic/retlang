using NUnit.Framework;
using Moq;
using Retlang.Channels;
using Retlang.Fibers;

namespace Retlang.Test.Channels
{
    [TestFixture]
    public class ShiftReceiverFixture : MoqTestFixture
    {
        private Mock<IReceiver<int>> _mock_initial;
        private Mock<IReceiver<int>> _mock_receiver;

        private IReceiver<int> _receiver;

        protected override void SetUp()
        {
            var fiber = new StubFiber();

            _mock_initial = CreateMock<IReceiver<int>>();
            _mock_receiver = CreateMock<IReceiver<int>>();

            _receiver = new ShiftReceiver<int>(fiber,
                _mock_initial.Object, _mock_receiver.Object);
        }

        [Test]
        public void Receive_MultipleMessages_ShiftsReceivers()
        {
            _mock_initial.Setup(m => m.Receive(0));
            _mock_receiver.Setup(m => m.Receive(1));
            _mock_receiver.Setup(mn => mn.Receive(2));

            _receiver.Receive(0);
            _receiver.Receive(1);
            _receiver.Receive(2);
        }
    }
}

