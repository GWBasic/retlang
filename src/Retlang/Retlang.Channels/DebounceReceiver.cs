using System;
using Retlang.Core;
using Retlang.Fibers;

namespace Retlang.Channels
{
    public static class DebounceReceiverExtensions
    {
        public static IDisposable SubscribeToDebounce<T>(this ISubscriber<T> subscriber,
            IFiber fiber, Action<T> receive, long intervalInMs)
        {
            var receiver = new DebounceReceiver<T>(fiber, receive, intervalInMs);
            return subscriber.Subscribe(receiver);
        }
    }
    
    /// <summary>
    /// Receives the last action received on the channel before a period of inactivity.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DebounceReceiver<T> : BaseReceiver<T>
    {
        private readonly Action<T> _receive;
        private readonly long _intervalInMs;

        private IDisposable _scheduled;
        private T _pending;

        public DebounceReceiver(IFiber fiber, Action<T> receive, long intervalInMs)
            : base(fiber)
        {
            _receive = receive;
            _intervalInMs = intervalInMs;
        }

        protected override void ReceiveFiltered(T message)
        {
            lock (_lock)
            {
                _pending = message;

                if (_scheduled != null)
                {
                    _scheduled.Dispose();
                }
                _scheduled = _fiber.Schedule(Flush, _intervalInMs);
            }
        }

        private void Flush()
        {
            T message;
            lock (_lock)
            {
                message = _pending;
                _pending = default(T);

                _scheduled.Dispose();
                _scheduled = null;
            }

            _receive(message);
        }
    }
}

