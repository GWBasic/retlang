﻿using System;
using System.Windows.Threading;
using Retlang.Core;

namespace Retlang.Fibers
{
    internal class DispatcherContext : IExecutionContext
    {
        private readonly Dispatcher _dispatcher;
        private readonly DispatcherPriority _priority;

        public DispatcherContext(Dispatcher dispatcher, DispatcherPriority priority)
        {
            _dispatcher = dispatcher;
            _priority = priority;
        }

        public void Enqueue(Action action)
        {
            _dispatcher.BeginInvoke(action, _priority);
        }
    }
}