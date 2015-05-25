using System;
using System.Threading;

namespace WebBrowser.TestingTools
{
	internal static class SynchronizationContextExtension
	{
		public static void Send(this SynchronizationContext context, Action action)
		{
			if(action != null)
				context.Send(x =>((Action)x)(), action);
		}
	}
}
