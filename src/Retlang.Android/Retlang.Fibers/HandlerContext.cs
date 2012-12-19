using System;
using Retlang.Core;
using Android.OS;

namespace Retlang.Fibers
{
    public class HandlerContext : IExecutionContext
    {
        private readonly Handler _handler;

        public HandlerContext (Handler handler)
        {
            _handler = handler;
        }

        public void Enqueue (Action action)
        {
            _handler.Post(action);
        }
    }
}

