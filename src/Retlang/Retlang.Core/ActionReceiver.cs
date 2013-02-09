using System;

using Retlang.Fibers;

namespace Retlang.Core
{
    public static class ActionReceiver
    {
        public static Action SubscribeOn(this Action action, IExecutionContext fiber)
        {
            return () => fiber.Enqueue(action);
        }

        public static Action<T> SubscribeOn<T>(this Action<T> action, IExecutionContext fiber)
        {
            return t => fiber.Enqueue(() => action(t));
        }

        public static Action<T1, T2> SubscribeOn<T1, T2>(this Action<T1, T2> action, IExecutionContext fiber)
        {
            return (t1, t2) => fiber.Enqueue(() => action(t1, t2));
        }

        public static Action<T1, T2, T3> SubscribeOn<T1, T2, T3>(this Action<T1, T2, T3> action, IExecutionContext fiber)
        {
            return (t1, t2, t3) => fiber.Enqueue(() => action(t1, t2, t3));
        }

        public static Action<T1, T2, T3, T4> SubscribeOn<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> action, IExecutionContext fiber)
        {
            return (t1, t2, t3, t4) => fiber.Enqueue(() => action(t1, t2, t3, t4));
        }

        public static Action<T1, T2, T3, T4, T5> SubscribeOn<T1, T2, T3, T4, T5>(this Action<T1, T2, T3, T4, T5> action, IExecutionContext fiber)
        {
            return (t1, t2, t3, t4, t5) => fiber.Enqueue(() => action(t1, t2, t3, t4, t5));
        }

        public static Action<T1, T2, T3, T4, T5, T6> SubscribeOn<T1, T2, T3, T4, T5, T6>(this Action<T1, T2, T3, T4, T5, T6> action, IExecutionContext fiber)
        {
            return (t1, t2, t3, t4, t5, t6) => fiber.Enqueue(() => action(t1, t2, t3, t4, t5, t6));
        }
    }
}

