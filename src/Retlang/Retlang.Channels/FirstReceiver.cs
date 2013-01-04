using System;
using Retlang.Fibers;

namespace Retlang.Channels
{
    public static class FirstReceiverExtensions
    {
        // XXX: SubscribeToFirstThenThrottle
        public static IDisposable SubscribeToFirstThenLast<T>(this ISubscriber<T> subscriber,
            IFiber fiber, Action<T> receive, long intervalInMs)
        {
            var first = new Receiver<T>(fiber, receive);
            var rest = new LastReceiver<T>(fiber, receive, intervalInMs);

            var receiver = new FirstReceiver<T>(fiber, first, rest);
            return subscriber.Subscribe(receiver);
        }

        public static IDisposable SubscribeToFirstThenDebounce<T>(this ISubscriber<T> subscriber,
            IFiber fiber, Action<T> receive, long intervalInMs)
        {
            var first = new Receiver<T>(fiber, receive);
            var rest = new DebounceReceiver<T>(fiber, receive, intervalInMs);

            var receiver = new FirstReceiver<T>(fiber, first, rest);
            return subscriber.Subscribe(receiver);
        }
    }

    public class FirstReceiver<T> : BaseReceiver<T>
    {
        private IReceiver<T> _first;
        private readonly IReceiver<T> _rest;

        public FirstReceiver(IFiber fiber, IReceiver<T> first, IReceiver<T> rest)
            : base(fiber)
        {
            if (first.Subscriptions != rest.Subscriptions)
            {
                // XXX: maybe this is bad?
            }

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

