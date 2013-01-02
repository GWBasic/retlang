using System;
using Retlang.Core;
using Retlang.Fibers;

namespace Retlang.Channels
{
    public abstract class BaseReceiver<T> : IReceiver<T>
    {
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
        /// <param name="msg"></param>
        public void Receive(T msg)
        {
            var filter = Filter; // copy reference for thread safety
            if (filter != null && !filter(msg))
            {
                return;
            }
            else
            {
                ReceiveFiltered(msg);
            }
        }

        /// <summary>
        /// Called after message has been filtered.
        /// </summary>
        /// <param name="msg"></param>
        protected abstract void ReceiveFiltered(T msg);
    }
}
