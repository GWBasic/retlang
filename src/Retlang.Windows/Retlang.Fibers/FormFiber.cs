using System.ComponentModel;
using Retlang.Core;

namespace Retlang.Fibers
{
    ///<summary>
    /// Allows interaction with Windows Forms. Transparently moves actions onto the Form's thread.
    ///</summary>
    public class FormFiber : ContextFiber
    {
        /// <summary>
        /// Creates an instance.
        /// </summary>
        public FormFiber(ISynchronizeInvoke invoker, IExecutor executor)
            : base(new FormContext(invoker), executor)
        {
        }

        public FormContext(ISynchronizeInvoke invoker)
            : this(invoker, new DefaultExecutor())
        {

        }
    }
}
