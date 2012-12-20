using System;

namespace Retlang.Channels
{
    /// <summary>
    /// Callback method and parameters for a channel subscription
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IReceiver<T> : IProducerThreadReceiver<T>
    {
        /// <summary>
        /// Filter called from producer threads. Should be thread safe as it may be called from
        /// multiple threads.
        /// </summary>
        Predicate<T> FilterOnProducerThread { get; set; }
    }
}