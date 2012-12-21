using System;
using System.Collections.Generic;
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
        private event Action<T> _subscribers;

        /// <summary>
        /// Subscribes to actions on producer threads. Subscriber could be called from multiple threads.
        /// </summary>
        /// <param name="subscriber"></param>
        /// <returns></returns>
        public IDisposable Subscribe(IProducerThreadReceiver<T> receiver)
        {
            _subscribers += receiver.ReceiveOnProducerThread;
            
            var unsubscriber = new Unsubscriber<T>(receiver.ReceiveOnProducerThread, this, receiver.Subscriptions);
            receiver.Subscriptions.RegisterSubscription(unsubscriber);
            
            return unsubscriber;
        }

        internal void Unsubscribe(Action<T> toUnsubscribe)
        {
            _subscribers -= toUnsubscribe;
        }

        /// <summary>
        /// <see cref="IPublisher{T}.Publish(T)"/>
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool Publish(T msg)
        {
            var evnt = _subscribers; // copy reference for thread safety
            if (evnt != null)
            {
                evnt(msg);
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
                var evnt = _subscribers; // copy reference for thread safety
                return evnt == null ? 0 : evnt.GetInvocationList().Length;
            }
        }

        /// <summary>
        /// Remove all subscribers.
        /// </summary>
        public void ClearSubscribers()
        {
            _subscribers = null;
        }
    }
}
