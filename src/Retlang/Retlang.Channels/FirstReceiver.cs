using System;
using Retlang.Fibers;

namespace Retlang.Channels
{
    public class FirstReceiver<T> : BaseReceiver<T>
    {
        private IReceiver<T> _first;
        private readonly IReceiver<T> _rest;

        public FirstReceiver(IFiber fiber, IReceiver<T> first, IReceiver<T> rest)
            : base(fiber)
        {
            _first = first;
            _rest = rest;
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
                    _rest.Receive(message);
                }
            }
        }
    }
}

