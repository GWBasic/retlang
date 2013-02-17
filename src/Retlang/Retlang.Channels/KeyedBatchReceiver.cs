using System;
using System.Collections.Generic;
using Retlang.Core;
using Retlang.Fibers;

namespace Retlang.Channels
{
    public static class KeyedBatchReceiverExtensions
    {
        public static IDisposable SubscribeToKeyedBatch<T, K>(this ISubscriber<T> subscriber,
            IFiber fiber, Action<IDictionary<K, T>> receive, Converter<T, K> converter, long intervalInMs)
        {
            var receiver = new KeyedBatchReceiver<T, K>(fiber, receive, converter, intervalInMs);
            return subscriber.Subscribe(receiver);
        }

        public static IDisposable SubscribeToKeyedBatch<T, K>(this ISubscriber<T> subscriber,
            IFiber fiber, Action<IDictionary<K, T>> receive, Converter<T, K> converter, TimeSpan interval)
        {
            var receiver = new KeyedBatchReceiver<T, K>(fiber, receive, converter, interval);
            return subscriber.Subscribe(receiver);
        }
    }

    /// <summary>
    /// Receiver that drops duplicates based upon a key.
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="T"></typeparam>
    public class KeyedBatchReceiver<T, K> : BaseReceiver<T>
    {
        private readonly Action<IDictionary<K, T>> _receive;
        private readonly Converter<T, K> _converter;
        private readonly long _intervalInMs;

        private Dictionary<K, T> _pending;

        /// <summary>
        /// Construct new instance.
        /// </summary>
        /// <param name="keyResolver"></param>
        /// <param name="target"></param>
        /// <param name="fiber"></param>
        /// <param name="intervalInMs"></param>
        public KeyedBatchReceiver(IFiber fiber, Action<IDictionary<K, T>> receive, Converter<T, K> converter, long intervalInMs)
            : base(fiber)
        {
            _receive = receive;
            _converter = converter;
            _intervalInMs = intervalInMs;
        }

        /// <summary>
        /// Construct new instance.
        /// </summary>
        /// <param name="keyResolver"></param>
        /// <param name="target"></param>
        /// <param name="fiber"></param>
        /// <param name="intervalInMs"></param>
        public KeyedBatchReceiver(IFiber fiber, Action<IDictionary<K, T>> receive, Converter<T, K> converter, TimeSpan interval)
            : base(fiber)
        {
            _receive = receive;
            _converter = converter;
            _intervalInMs = Convert.ToInt64(interval.TotalMilliseconds);
        }

        /// <summary>
        /// received on delivery thread
        /// </summary>
        /// <param name="message"></param>
        protected override void ReceiveFiltered(T message)
        {
            lock (_lock)
            {
                var key = _converter(message);
                if (_pending == null)
                {
                    _pending = new Dictionary<K, T>();
                    _fiber.Schedule(Flush, _intervalInMs);
                }
                _pending[key] = message;
            }
        }

        private void Flush()
        {
            var toReturn = ClearPending();
            if (toReturn != null)
            {
                _receive(toReturn);
            }
        }

        private IDictionary<K, T> ClearPending()
        {
            lock (_lock)
            {
                if (_pending == null || _pending.Count == 0)
                {
                    _pending = null;
                    return null;
                }
                var toReturn = _pending;
                _pending = null;
                return toReturn;
            }
        }
    }
}
