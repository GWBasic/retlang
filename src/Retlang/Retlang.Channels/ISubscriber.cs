using System;
using System.Collections.Generic;
using Retlang.Fibers;

namespace Retlang.Channels
{
    /// <summary>
    /// Channel subscription methods.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ISubscriber<T>
    {
        /// <summary>
        /// Subscribes to actions on producer threads. Subscriber could be called from multiple threads.
        /// </summary>
        /// <param name="subscriber"></param>
        /// <returns></returns>
        IDisposable Subscribe(IProducerThreadReceiver<T> receiver);

        /// <summary>
        /// Removes all subscribers.
        /// </summary>
        void ClearSubscribers();
    }
}
