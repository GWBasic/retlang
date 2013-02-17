using System;

namespace Retlang.Core
{
    /// <summary>
    /// A scheduled event.
    /// </summary>
    public interface IPendingEvent : IDisposable
    {
        /// <summary>
        /// Time of expiration for this event
        /// </summary>
        long Expiration { get; }

        /// <summary>
        /// Execute this event and optionally schedule another execution.
        /// </summary>
        /// <returns></returns>
        IPendingEvent Execute(long currentTime);
    }

    public static class PendingEventExtensions
    {
        public static IPendingEvent Execute(this IPendingEvent pendingEvent, TimeSpan currentTime)
        {
            return pendingEvent.Execute(Convert.ToInt64(currentTime.TotalMilliseconds));
        }
    }
}
