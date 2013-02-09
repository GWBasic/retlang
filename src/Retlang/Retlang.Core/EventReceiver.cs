using System;

using Retlang.Fibers;

namespace Retlang.Core
{
    public static class EventReceiver
    {
        public static EventHandler SubscribeOn(this EventHandler eventHandler, IExecutionContext fiber)
        {
            return (sender, e) => fiber.Enqueue(() => eventHandler(sender, e));
        }

        public static EventHandler<TEventArgs> SubscribeOn<TEventArgs>(this EventHandler<TEventArgs> eventHandler, IExecutionContext fiber)
            where TEventArgs : EventArgs
        {
            return (sender, e) => fiber.Enqueue(() => eventHandler(sender, e));
        }
    }
}

