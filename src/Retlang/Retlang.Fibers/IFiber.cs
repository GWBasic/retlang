using System;
using Retlang.Core;

namespace Retlang.Fibers
{
    /// <summary>
    /// Enqueues pending actions for the context of execution (thread, pool of threads, message pump, etc.)
    /// </summary>
    public interface IFiber : ISubscriptionRegistry, IExecutionContext, IScheduler, IDisposable
    {
        /// <summary>
        /// Raise an exception if called from another fiber.
        /// </summary> 
        void Assert();

        /// <summary>
        /// Start consuming actions.
        /// </summary>
        void Start();
    }
}
