using System;

namespace Knyaz.Optimus.Environment
{
	public class History
	{
		private readonly Engine _engine;

		public History(Engine engine)
		{
			_engine = engine;
		}

		public void PushState(object a, object b, string url)
		{
			_engine.Uri = Uri.IsWellFormedUriString(url, UriKind.Absolute) ? new Uri(url) : new Uri(new Uri(_engine.Uri.AbsoluteUri), url);
		}
	}
}