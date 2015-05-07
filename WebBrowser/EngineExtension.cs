using System.IO;
using System.Text;

namespace WebBrowser
{
	static class EngineExtension
	{
		public static void Load(this Engine engine, string html)
		{
			using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(html)))
			{
				engine.Load(stream);
			}
		}
	}
}