using System;
using Retlang.Core;
using System.Threading;

namespace Retlang.Fibers
{
    public abstract class BaseFiber : IFiber
    {
        protected volatile ExecutionState _state = ExecutionState.Created;

        private readonly Subscriptions _subscriptions = new Subscriptions();
        private readonly Scheduler _scheduler;

        public BaseFiber ()
        {
            _scheduler = new Scheduler(this);
        }

        public virtual void Assert()
        {

        }

        public virtual void Start()
        {
            if (_state != ExecutionState.Created)
            {
                var message = String.Format("Start() called on Fiber with state: {0}", _state);
                throw new ThreadStateException(message);
            }

            _state = ExecutionState.Running;
        }

        public abstract void Enqueue(Action action);

        public int SubscriptionsCount
        {
            get { return _subscriptions.Count; }
        }

        public void RegisterSubscription(IDisposable toAdd)
        {
            _subscriptions.Add(toAdd);
        }
        
        public bool DeregisterSubscription(IDisposable toRemove)
        {
            return _subscriptions.Remove(toRemove);
        }

        public IDisposable Schedule(Action action, long firstInMs)
        {
            return _scheduler.Schedule(action, firstInMs);
        }

        public IDisposable Reschedule(IDisposable scheduled, Action action, long firstInMs)
        {
            return _scheduler.Reschedule(scheduled, action, firstInMs);
        }

        public IDisposable ScheduleOnInterval(Action action, long firstInMs, long regularInMs)
        {
            return _scheduler.ScheduleOnInterval(action, firstInMs, regularInMs);
        }

        public virtual void Dispose()
        {
            _state = ExecutionState.Stopped;
            _scheduler.Dispose();
            _subscriptions.Dispose();
        }
    }
}

