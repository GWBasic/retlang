using System;

namespace Retlang.Core
{
    /// <summary>
    /// Methods for scheduling actions that will be executed in the future.
    /// </summary>
    public interface IScheduler
    {
        /// <summary>
        /// Schedules an action to be executed once.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="firstInMs"></param>
        /// <returns>a handle to cancel the timer.</returns>
        IDisposable Schedule(Action action, long firstInMs);

        IDisposable Reschedule(IDisposable scheduled, Action action, long firstInMs);

        /// <summary>
        /// Schedule an action to be executed on a recurring interval.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="firstInMs"></param>
        /// <param name="regularInMs"></param>
        /// <returns>a handle to cancel the timer.</returns>
        IDisposable ScheduleOnInterval(Action action, long firstInMs, long regularInMs);
    }

    public static class SchedulerExtensions
    {
        /// <summary>
        /// Schedules an action to be executed once.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="firstInMs"></param>
        /// <returns>a handle to cancel the timer.</returns>
        public static IDisposable Schedule(this IScheduler scheduler, Action action, TimeSpan first)
        {
            return scheduler.Schedule(action, Convert.ToInt64(first.TotalMilliseconds));
        }

        public static IDisposable Reschedule(this IScheduler scheduler, IDisposable scheduled, Action action, TimeSpan first)
        {
            return scheduler.Reschedule(scheduled, action, Convert.ToInt64(first.TotalMilliseconds));
        }

        /// <summary>
        /// Schedule an action to be executed on a recurring interval.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="firstInMs"></param>
        /// <param name="regularInMs"></param>
        /// <returns>a handle to cancel the timer.</returns>
        public static IDisposable ScheduleOnInterval(this IScheduler scheduler, Action action, TimeSpan first, TimeSpan regular)
        {
            return scheduler.ScheduleOnInterval(action, Convert.ToInt64(first.TotalMilliseconds), Convert.ToInt64(regular.TotalMilliseconds));
        }
    }
}
