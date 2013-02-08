using System;
using System.Threading;

using Retlang.Fibers;

namespace Retlang.Core
{
    public static class ExecutionContextExtensions
    {
        /// <summary>
        /// Blocks until the action is run. Warning! Incorrect use of this method can lead to deadlocks!
        /// </summary>
        public static void Lock(this IExecutionContext executionContext, Action action)
        {
            // Special case for stub fibers
            // Note: this method is hard to test. It deadlocks if the execution contect runs synchronously!
            if (executionContext is StubFiber)
            {
                action();
                return;
            }

            var sync = new object();

            Exception exception = null;

            lock (sync)
            {
                executionContext.Enqueue(() =>
                {
                    try
                    {
                        action();
                    }
                    catch (Exception e)
                    {
                        exception = e;
                    }
                    finally
                    {
                        lock (sync)
                        {
                            Monitor.Pulse(sync);
                        }
                    }
                });

                // This is how the calling thread waits for the executor / fiber to
                // run the action.
                Monitor.Wait(sync);

                if (null != exception)
                {
                    throw exception;
                }
            }
        }
    }
}

