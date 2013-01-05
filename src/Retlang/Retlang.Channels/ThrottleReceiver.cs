using System;
using Retlang.Core;
using Retlang.Fibers;

namespace Retlang.Channels
{
    public static class ThrottleReceiverExtensions
    {
        public static IDisposable SubscribeToThrottle<T>(this ISubscriber<T> subscriber,
            IFiber fiber, Action<T> receive, long intervalInMs)
        {
            var receiver = new ThrottleReceiver<T>(fiber, receive, intervalInMs);
            return subscriber.Subscribe(receiver);
        }
    }

    /// <summary>
    /// Receives the last action received on the channel over a time interval.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ThrottleReceiver<T> : BaseReceiver<T>
    {
        private readonly Action<T> _receive;
        private readonly long _intervalInMs;

        private IDisposable _scheduled;
        private T _message;

        /// <summary>
        /// New instance.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="fiber"></param>
        /// <param name="intervalInMs"></param>
        public ThrottleReceiver(IFiber fiber, Action<T> receive, long intervalInMs)
            : base(fiber)
        {
            _receive = receive;
            _intervalInMs = intervalInMs;
        }

        /// <summary>
        /// Receives message from producer thread.
        /// </summary>
        /// <param name="message"></param>
        protected override void ReceiveFiltered(T message)
        {
            lock (_lock)
            {
                _message = message;

                if (_scheduled == null)
                {
                    _scheduled = _fiber.Schedule(Flush, _intervalInMs);
                }
            }
        }

        private void Flush()
        {
            T message;
            lock (_lock)
            {
                message = _message;
                _message = default(T);

                _scheduled.Dispose();
                _scheduled = null;
            }

            _receive(message);
        }
    }
}
