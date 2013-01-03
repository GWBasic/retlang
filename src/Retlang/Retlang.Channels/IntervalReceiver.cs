using System;
using System.Collections.Generic;
using Retlang.Core;
using Retlang.Fibers;

namespace Retlang.Channels
{
    public static class IntervalReceiverExtensions
    {
        public static IDisposable SubscribeToInterval<T>(this ISubscriber<T> subscriber,
            IFiber fiber, Action<T> receive, long intervalInMs)
        {
            var receiver = new IntervalReceiver<T>(fiber, receive, intervalInMs);
            return subscriber.Subscribe(receiver);
        }
    }
    
    /// <summary>
    /// Receives one message per interval. The first message delivered after a period of
    /// inactivity is delivered immediately.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class IntervalReceiver<T> : BaseReceiver<T>
    {
        private readonly object _lock = new object();

        private readonly Action<T> _receive;
        private readonly long _intervalInMs;

        private IDisposable _scheduled;
        private IList<T> _pending = new List<T>();

        public IntervalReceiver(IFiber fiber, Action<T> receive, long intervalInMs)
            : base(fiber)
        {
            _receive = receive;
            _intervalInMs = intervalInMs;
        }

        protected override void ReceiveFiltered(T msg)
        {
            lock (_lock)
            {
                if (_pending.Count < 2)
                {
                    _pending.Add(msg);
                }
                else
                {
                    _pending[1] = msg;
                }

                if (_scheduled == null)
                {
                    _scheduled = _fiber.ScheduleOnInterval(Flush, 0, _intervalInMs);
                }
            }
        }

        private void Flush()
        {
            T message;
            lock (_lock)
            {
                if (_pending.Count > 0)
                {
                    message = _pending[0];
                    _pending.RemoveAt(0);
                }
                else
                {
                    _scheduled.Dispose();
                    _scheduled = null;

                    return;
                }
            }

            _receive(message);
        }
    }
}

