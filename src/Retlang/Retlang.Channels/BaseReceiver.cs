using System;
using Retlang.Core;
using Retlang.Fibers;

namespace Retlang.Channels
{
    /// <summary>
    /// Base implementation for subscription
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BaseReceiver<T> : IReceiver<T>
    {
        protected readonly IFiber _fiber;

        protected BaseReceiver(IFiber fiber)
        {
            _fiber = fiber;
        }

        /// <summary>
        /// <see cref="IReceiver{T}.FilterOnProducerThread"/>
        /// </summary>
        public Predicate<T> FilterOnProducerThread { get; set; }

        ///<summary>
        /// Allows for the registration and deregistration of subscriptions
        ///</summary>
        public ISubscriptionRegistry Subscriptions
        {
            get { return _fiber; }
        }

        /// <summary>
        /// <see cref="IProducerThreadReceiver{T}.ReceiveOnProducerThread"/>
        /// </summary>
        /// <param name="msg"></param>
        public void ReceiveOnProducerThread(T msg)
        {
            var filter = FilterOnProducerThread; // copy reference for thread safety
            if (filter != null && !filter(msg))
            {
                return;
            }
            else
            {
                OnMessageOnProducerThread(msg);
            }
        }

        /// <summary>
        /// Called after message has been filtered.
        /// </summary>
        /// <param name="msg"></param>
        protected abstract void OnMessageOnProducerThread(T msg);
    }
}
