using System;
using System.Threading;
using Retlang.Core;

namespace Retlang.Fibers
{
    /// <summary>
    /// Fiber implementation backed by a dedicated thread.
    /// <see cref="IFiber"/>
    /// </summary>
    public class ThreadFiber : BaseFiber
    {
        private static int THREAD_COUNT;

        private readonly Thread _thread;
        private readonly IQueue _queue;

        /// <summary>
        /// Create a thread fiber with the default queue.
        /// </summary>
        public ThreadFiber() 
            : this(new DefaultQueue())
        {}

        /// <summary>
        /// Creates a thread fiber with a specified executor.
        /// </summary>
        /// <param name="queue"></param>
        public ThreadFiber(IExecutor executor)
            : this(new DefaultQueue(executor))
        {}

        /// <summary>
        /// Creates a thread fiber with a specified queue.
        /// </summary>
        /// <param name="queue"></param>
        public ThreadFiber(IQueue queue) 
            : this(queue, "ThreadFiber-" + GetNextThreadId())
        {}

        /// <summary>
        /// Creates a thread fiber with a specified name.
        /// </summary>
        /// /// <param name="threadName"></param>
        public ThreadFiber(string threadName)
            : this(new DefaultQueue(), threadName)
        {}

        /// <summary>
        /// Creates a thread fiber.
        /// </summary>
        /// <param name="queue"></param>
        /// <param name="threadName"></param>
        /// <param name="isBackground"></param>
        /// <param name="priority"></param>
        public ThreadFiber(IQueue queue, string threadName, bool isBackground = true, ThreadPriority priority = ThreadPriority.Normal)
        {
            _queue = queue;
            _thread = new Thread(_queue.Run)
            {
                Name = threadName,
                IsBackground = isBackground,
                Priority = priority,
            };
        }

        /// <summary>
        /// <see cref="IFiber"/>
        /// </summary>
        public Thread Thread
        {
            get { return _thread; }
        }

        private static int GetNextThreadId()
        {
            return Interlocked.Increment(ref THREAD_COUNT);
        }

        public override void Assert()
        {
            if (Thread.CurrentThread.ManagedThreadId != _thread.ManagedThreadId)
            {
                throw new ThreadStateException();
            }
        }

        /// <summary>
        /// <see cref="IFiber.Start"/>
        /// </summary>
        public override void Start()
        {
            base.Start();
            _thread.Start();
        }

        /// <summary>
        /// Enqueue a single action.
        /// </summary>
        /// <param name="action"></param>
        public override void Enqueue(Action action)
        {
            if (_state == ExecutionState.Stopped)
            {
                return;
            }

            _queue.Enqueue(action);
        }

        ///<summary>
        /// Calls join on the thread.
        ///</summary>
        public void Join()
        {
            _thread.Join();
        }

        /// <summary>
        /// Stops the thread.
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
            _queue.Stop();
        }
    }
}
