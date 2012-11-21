using System;
using Retlang.Fibers;
using Retlang.Core;

namespace Retlang.Fibers
{
	public class NSObjectFiber : GuiFiber
	{
		public NSObjectFiber (IExecutor executor)
			: base(new NSObjectAdapter(), executor)
		{

		}
	}
}

