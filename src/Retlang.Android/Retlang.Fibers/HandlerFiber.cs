using Retlang.Core;
using Android.OS;

namespace Retlang.Fibers
{
    public class HandlerFiber : ContextFiber
    {
        public HandlerFiber (Handler handler, IExecutor executor)
            : base(new HandlerContext(handler), executor)
        {

        }

        public HandlerFiber(Handler handler)
            : this(handler, new DefaultExecutor())
        {

        }
    }
}

