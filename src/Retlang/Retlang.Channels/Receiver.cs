using System;
using Retlang.Core;
using Retlang.Fibers;

namespace Retlang.Channels
{
    public static class ReceiverExtensions
    {
        public static IDisposable Subscribe<T>(this ISubscriber<T> subscriber,
            IFiber fiber, Action<T> receive)
        {
            var receiver = new Receiver<T>(fiber, receive);
            return subscriber.Subscribe(receiver);
        }
    }

    /// <summary>
    /// Simple receiver that enqueues messages to a fiber.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Receiver<T> : BaseReceiver<T>
    {
        private readonly Action<T> _receive;

        /// <summary>
        /// Construct the subscription
        /// </summary>
        /// <param name="fiber"></param>
        /// <param name="receiver"></param>
        public Receiver(IFiber fiber, Action<T> receive)
            : base(fiber)
        {
            _receive = receive;
        }

        /// <summary>
        /// Receives the action and queues the execution on the target fiber.
        /// </summary>
        /// <param name="msg"></param>
        protected override void OnMessageOnProducerThread(T msg)
        {
            _fiber.Enqueue(() => _receive(msg));
        }
    }
}