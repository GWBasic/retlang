using System;
using System.ComponentModel;
using Retlang.Core;

namespace Retlang.Fibers
{
    internal class FormContext : IExecutionContext
    {
        private readonly ISynchronizeInvoke _invoker;

        public FormContext(ISynchronizeInvoke invoker)
        {
            _invoker = invoker;
        }

        public void Enqueue(Action action)
        {
            _invoker.BeginInvoke(action, null);
        }
    }
}