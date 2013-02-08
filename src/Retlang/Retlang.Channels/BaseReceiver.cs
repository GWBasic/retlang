using System;
using Retlang.Core;
using Retlang.Fibers;

namespace Retlang.Channels
{
    public abstract class BaseReceiver<T> : IReceiver<T>
    {
        protected readonly object _lock = new object();
        protected readonly IFiber _fiber;

        protected BaseReceiver(IFiber fiber)
        {
            _fiber = fiber;
        }

        /// <summary>
        /// <see cref="IReceiver{T}.Filter"/>
        /// </summary>
        public Predicate<T> Filter { get; set; }

        ///<summary>
        /// Allows for the registration and deregistration of subscriptions
        ///</summary>
        public ISubscriptionRegistry Subscriptions
        {
            get { return _fiber; }
        }

        /// <summary>
        /// <see cref="IProducerThreadReceiver{T}.Receive"/>
        /// </summary>
        /// <param name="message"></param>
        public void Receive(T message)
        {
            var filter = Filter; // copy reference for thread safety
            if (filter != null && !filter(message))
            {
                return;
            }
            else
            {
                ReceiveFiltered(message);
            }
        }

        /// <summary>
        /// Called after message has been filtered.
        /// </summary>
        /// <param name="message"></param>
        protected abstract void ReceiveFiltered(T message);

        /// <summary>
        /// Allow users to directly subscribe these objects to events
        /// </summary>
        /// <param name="subscriber"></param>
        /// <returns></returns>
        public static implicit operator Action<T>(BaseReceiver<T> baseReceiver)
        {
            return baseReceiver.Receive;
        }
    }
}
