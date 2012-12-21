using System;
using Retlang.Core;

namespace Retlang.Channels
{
    public interface IReceiver<T>
    {
        /// <summary>
        /// Filter called from producer threads. Should be thread safe as it may be called from
        /// multiple threads.
        /// </summary>
        Predicate<T> Filter { get; set; }

        /// <summary>
        /// Method called from producer threads
        /// </summary>
        /// <param name="msg"></param>
        void Receive(T msg);
        
        ///<summary>
        /// Allows for the registration and deregistration of subscriptions
        ///</summary>
        ISubscriptionRegistry Subscriptions { get; }
    }
}