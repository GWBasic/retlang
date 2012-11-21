using System;
using Retlang.Fibers;
using Retlang.Core;

namespace Retlang.Fibers
{
	public class CocoaTouchFiber : GuiFiber
	{
		public CocoaTouchFiber (IExecutor executor)
			: base(new CocoaTouchAdapter(), executor)
		{

		}
	}
}

