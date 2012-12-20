using System;
using Retlang.Core;

namespace Retlang.Channels
{
    /// <summary>
    /// Base implementation for subscription
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BaseReceiver<T> : IReceiver<T>
    {
        /// <summary>
        /// <see cref="IReceiver{T}.FilterOnProducerThread"/>
        /// </summary>
        public Predicate<T> FilterOnProducerThread { get; set; }

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

        ///<summary>
        /// Allows for the registration and deregistration of subscriptions
        ///</summary>
        public abstract ISubscriptionRegistry Subscriptions { get; }

        /// <summary>
        /// Called after message has been filtered.
        /// </summary>
        /// <param name="msg"></param>
        protected abstract void OnMessageOnProducerThread(T msg);
    }
}
