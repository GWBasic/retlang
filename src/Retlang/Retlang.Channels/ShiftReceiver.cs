using System;
using Retlang.Fibers;

namespace Retlang.Channels
{
    public class ShiftReceiver<T> : BaseReceiver<T>
    {
        private IReceiver<T> _first;
        private readonly IReceiver<T> _receiver;

        public ShiftReceiver(IFiber fiber, IReceiver<T> first, IReceiver<T> receiver)
            : base(fiber)
        {
            _first = first;
            _receiver = receiver;
        }

        protected override void ReceiveFiltered(T message)
        {
            lock (_lock)
            {
                if (_first != null)
                {
                    _first.Receive(message);
                    _first = null;
                }
                else
                {
                    _receiver.Receive(message);
                }
            }
        }
    }
}

