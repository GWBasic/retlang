using System;
using Retlang.Core;

namespace Retlang.Fibers
{
    public abstract class BaseFiber : IFiber
    {
        private readonly Subscriptions _subscriptions = new Subscriptions();
        private readonly Scheduler _scheduler;

        public BaseFiber ()
        {
            _scheduler = new Scheduler(this);
        }

        public abstract void Start();
        public abstract void Enqueue(Action action);

        public int Count
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

        public IDisposable ScheduleOnInterval(Action action, long firstInMs, long regularInMs)
        {
            return _scheduler.ScheduleOnInterval(action, firstInMs, regularInMs);
        }

        public virtual void Dispose()
        {
            _scheduler.Dispose();
            _subscriptions.Dispose();
        }
    }
}

