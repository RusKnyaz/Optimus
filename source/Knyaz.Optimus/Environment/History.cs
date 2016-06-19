using System;
using Knyaz.Optimus.ScriptExecuting;
using Knyaz.Optimus.Tools;

namespace Knyaz.Optimus.Environment
{
	public class History : IHistory
	{
		private readonly Engine _engine;

		public History(Engine engine)
		{
			_engine = engine;
		}

		public void PushState(object a, object b, string url)
		{
			_engine.Uri = UriHelper.IsAbsolete(url) ? new Uri(url) : new Uri(new Uri(_engine.Uri.GetLeftPart(UriPartial.Authority)), url);
		}
	}

	[DomItem]
	public interface IHistory
	{
		void PushState(object a, object b, string url);
	}
}