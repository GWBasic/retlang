using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Retlang.Core;

namespace Retlang.Fibers
{
    ///<summary>
    /// Allows interaction with Windows Forms.  Transparently moves actions onto the Form's thread.
    ///</summary>
    public class GuiFiber : BaseFiber
    {
        private readonly object _lock = new object();
        private readonly IExecutionContext _executionContext;
        private readonly IExecutor _executor;
        private readonly List<Action> _queue = new List<Action>();

        /// <summary>
        /// Creates an instance.
        /// </summary>
        public GuiFiber(IExecutionContext executionContext, IExecutor executor)
        {
            _executionContext = executionContext;
            _executor = executor;
        }

        /// <summary>
        /// <see cref="IFiber.Start()"/>
        /// </summary>
        public override void Start()
        {
            base.Start();

            lock (_lock)
            {
                var actions = _queue.ToList();
                _queue.Clear();
                if (actions.Count > 0)
                {
                    _executionContext.Enqueue(() => _executor.Execute(actions));
                }
            }
        }

        /// <summary>
        /// Enqueue a single action.
        /// </summary>
        /// <param name="action"></param>
        public override void Enqueue(Action action)
        {
            switch (_state)
            {
                case ExecutionState.Stopped:
                    return;
                case ExecutionState.Created:
                    lock (_lock)
                    {
                        if (_state == ExecutionState.Created)
                        {
                            _queue.Add(action);
                        }
                    }
                    break;
                default:
                    _executionContext.Enqueue(() => _executor.Execute(action));
                    break;
            }
        }
    }
}
