using System;
using Retlang.Core;
using MonoTouch.Foundation;

namespace Retlang.Fibers
{
	public class CocoaTouchAdapter : NSObject, IExecutionContext
	{
		public void Enqueue(Action action)
		{
			BeginInvokeOnMainThread(() => action());
		}
	}
}

