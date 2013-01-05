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
    }

    /// <summary>
    /// Receives one batch of actions per interval for the consuming thread.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BatchReceiver<T> : BaseReceiver<T>
    {
        private readonly Action<IList<T>> _receive;
        private readonly long _intervalInMs;

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
        /// Receives message and batches as needed.
        /// </summary>
        /// <param name="message"></param>
        protected override void ReceiveFiltered(T message)
        {
            lock (_lock)
            {
                if (_batch.Count == 0)
                {
                    _fiber.Schedule(Flush, _intervalInMs);
                }

                _batch.Add(message);
            }
        }

        private void Flush()
        {
            IList<T> batch = null;
            lock (_lock)
            {
                batch = _batch;
                _batch = new List<T>();
            }

            _receive(batch);
        }
    }
}