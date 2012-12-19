using System;
using Retlang.Fibers;
using Retlang.Core;

namespace Retlang.Fibers
{
    public class NSObjectFiber : ContextFiber
    {
        public NSObjectFiber (IExecutor executor)
            : base(new NSObjectContext(), executor)
        {

        }
    }
}

