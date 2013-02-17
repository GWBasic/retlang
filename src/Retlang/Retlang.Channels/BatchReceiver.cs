using System;
using System.Collections.Generic;
using Retlang.Core;
using Retlang.Fibers;

namespace Retlang.Channels
{
    public static class BatchReceiverExtensions
    {
        public static IDisposable SubscribeToBatch<T>(this ISubscriber<T> subscriber,
            IFiber fiber, Action<IList<T>> receive, long intervalInMs)
        {
            var receiver = new BatchReceiver<T>(fiber, receive, intervalInMs);
            return subscriber.Subscribe(receiver);
        }

        public static IDisposable SubscribeToBatch<T>(this ISubscriber<T> subscriber,
            IFiber fiber, Action<IList<T>> receive, TimeSpan interval)
        {
            var receiver = new BatchReceiver<T>(fiber, receive, interval);
            return subscriber.Subscribe(receiver);
        }
    }

    /// <summary>
    /// Receives one batch of actions per interval for the consuming thread.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BatchReceiver<T> : BaseReceiver<T>
    {
        private readonly Action<IList<T>> _receive;
        private readonly long _intervalInMs;

        private IDisposable _scheduled;
        private IList<T> _batch = new List<T>();

        /// <summary>
        /// Construct new instance.
        /// </summary>
        /// <param name="fiber"></param>
        /// <param name="receive"></param>
        /// <param name="intervalInMs"></param>
        public BatchReceiver(IFiber fiber, Action<IList<T>> receive, long intervalInMs)
            : base(fiber)
        {
            _receive = receive;
            _intervalInMs = intervalInMs;
        }

        /// <summary>
        /// Construct new instance.
        /// </summary>
        /// <param name="fiber"></param>
        /// <param name="receive"></param>
        /// <param name="intervalInMs"></param>
        public BatchReceiver(IFiber fiber, Action<IList<T>> receive, TimeSpan interval)
            : base(fiber)
        {
            _receive = receive;
            _intervalInMs = Convert.ToInt64(interval.TotalMilliseconds);
        }

        /// <summary>
        /// Receives message and batches as needed.
        /// </summary>
        /// <param name="message"></param>
        protected override void ReceiveFiltered(T message)
        {
            lock (_lock)
            {
                _batch.Add(message);

                if (_scheduled == null)
                {
                    _scheduled = _fiber.Schedule(Flush, _intervalInMs);
                }
            }
        }

        private void Flush()
        {
            IList<T> batch = null;
            lock (_lock)
            {
                batch = _batch;
                _batch = new List<T>();

                _scheduled.Dispose();
                _scheduled = null;
            }

            _receive(batch);
        }
    }
}