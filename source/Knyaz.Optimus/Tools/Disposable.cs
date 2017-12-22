using System;

namespace Knyaz.Optimus.Tools
{
	/// <summary>
	/// Wrapper for dispose delegate invocation.
	/// </summary>
	internal class Disposable : IDisposable
	{
		private readonly Action _disposeAction;

		public Disposable(Action disposeAction) => _disposeAction = disposeAction;

		public void Dispose() => _disposeAction();
	}
}
