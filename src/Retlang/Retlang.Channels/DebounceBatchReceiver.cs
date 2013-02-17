using System;
using System.Collections.Generic;
using Retlang.Core;
using Retlang.Fibers;

namespace Retlang.Channels
{
    public static class DebounceBatchReceiverExtensions
    {
        public static IDisposable SubscribeToDebounceBatch<T>(this ISubscriber<T> subscriber,
            IFiber fiber, Action<IList<T>> receive, long intervalInMs)
        {
            var receiver = new DebounceBatchReceiver<T>(fiber, receive, intervalInMs);
            return subscriber.Subscribe(receiver);
        }

        public static IDisposable SubscribeToDebounceBatch<T>(this ISubscriber<T> subscriber,
            IFiber fiber, Action<IList<T>> receive, TimeSpan interval)
        {
            var receiver = new DebounceBatchReceiver<T>(fiber, receive, interval);
            return subscriber.Subscribe(receiver);
        }
    }

    public class DebounceBatchReceiver<T> : BaseReceiver<T>
    {
        private readonly Action<IList<T>> _receive;
        private readonly long _intervalInMs;

        private IDisposable _scheduled;
        private IList<T> _batch = new List<T>();

        public DebounceBatchReceiver(IFiber fiber, Action<IList<T>> receive, long intervalInMs)
            : base(fiber)
        {
            _receive = receive;
            _intervalInMs = intervalInMs;
        }

        public DebounceBatchReceiver(IFiber fiber, Action<IList<T>> receive, TimeSpan interval)
            : base(fiber)
        {
            _receive = receive;
            _intervalInMs = Convert.ToInt64(interval.TotalMilliseconds);
        }

        protected override void ReceiveFiltered(T message)
        {
            lock (_lock)
            {
                _batch.Add(message);

                if (_scheduled != null)
                {
                    _scheduled.Dispose();
                }
                _scheduled = _fiber.Schedule(Flush, _intervalInMs);
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

