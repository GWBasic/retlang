using System;
using Retlang.Core;
using Android.OS;

namespace Retlang.Fibers
{
    public class HandlerAdapter : IExecutionContext
    {
        private readonly Handler _handler;

        public HandlerAdapter (Handler handler)
        {
            _handler = handler;
        }

        public void Enqueue (Action action)
        {
            _handler.Post(action);
        }
    }
}

