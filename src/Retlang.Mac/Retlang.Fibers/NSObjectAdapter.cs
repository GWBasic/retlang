using System;
using Retlang.Core;
using MonoMac.Foundation;

namespace Retlang.Fibers
{
    public class NSObjectAdapter : NSObject, IExecutionContext
    {
        public void Enqueue(Action action)
        {
            BeginInvokeOnMainThread(() => action());
        }
    }
}

