using System;
using Retlang.Core;
using Retlang.Fibers;

namespace Retlang.Channels
{
    public static class LastReceiverExtensions
    {
        public static IDisposable SubscribeToLast<T>(this ISubscriber<T> subscriber,
            IFiber fiber, Action<T> receive, long intervalInMs)
        {
            var receiver = new LastReceiver<T>(fiber, receive, intervalInMs);
            return subscriber.Subscribe(receiver);
        }
    }

    /// <summary>
    /// Receives the last action received on the channel over a time interval. 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LastReceiver<T> : BaseReceiver<T>
    {
        private readonly object _batchLock = new object();

        private readonly Action<T> _receive;
        private readonly long _intervalInMs;

        private bool _flushPending;
        private T _pending;

        /// <summary>
        /// New instance.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="fiber"></param>
        /// <param name="intervalInMs"></param>
        public LastReceiver(IFiber fiber, Action<T> receive, long intervalInMs)
            : base(fiber)
        {
            _receive = receive;
            _intervalInMs = intervalInMs;
        }

        /// <summary>
        /// Receives message from producer thread.
        /// </summary>
        /// <param name="msg"></param>
        protected override void ReceiveFiltered(T msg)
        {
            lock (_batchLock)
            {
                if (!_flushPending)
                {
                    _fiber.Schedule(Flush, _intervalInMs);
                    _flushPending = true;
                }
                _pending = msg;
            }
        }

        private void Flush()
        {
            _receive(ClearPending());
        }

        private T ClearPending()
        {
            lock (_batchLock)
            {
                _flushPending = false;
                return _pending;
            }
        }
    }
}