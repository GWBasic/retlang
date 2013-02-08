using System;
using System.Collections.Generic;
using System.Threading;
using Retlang.Core;
using Retlang.Fibers;

namespace Retlang.Channels
{
    ///<summary>
    /// Default Channel Implementation. Methods are thread safe.
    ///</summary>
    ///<typeparam name="T"></typeparam>
    public class Channel<T> : IChannel<T>
    {
        /// <summary>
        /// Allows subscribing to the channel using multicast events
        /// </summary>
        public event Action<T> Published;

        /// <summary>
        /// Subscribes to actions on producer threads. Subscriber could be called from multiple threads.
        /// </summary>
        /// <param name="subscriber"></param>
        /// <returns></returns>
        public IDisposable Subscribe(IReceiver<T> receiver)
        {
            Published += receiver.Receive;
            
            var unsubscriber = new Unsubscriber<T>(receiver.Receive, this, receiver.Subscriptions);
            receiver.Subscriptions.RegisterSubscription(unsubscriber);
            
            return unsubscriber;
        }

        internal void Unsubscribe(Action<T> toUnsubscribe)
        {
            Published -= toUnsubscribe;
        }

        /// <summary>
        /// <see cref="IPublisher{T}.Publish(T)"/>
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool Publish(T message)
        {
            var subscribers = Published; // copy reference for thread safety
            if (subscribers != null)
            {
                subscribers(message);
                return true;
            }
            return false;
        }

        ///<summary>
        /// Number of subscribers
        ///</summary>
        public int SubscribersCount
        {
            get
            {
                var subscribers = Published; // copy reference for thread safety
                return subscribers == null ? 0 : subscribers.GetInvocationList().Length;
            }
        }

        /// <summary>
        /// Remove all subscribers.
        /// </summary>
        public void ClearSubscribers()
        {
            Published = null;
        }
    }
}
