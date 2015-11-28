using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using WebBrowser.Html;

namespace WebBrowser.Tests.HtmlReader
{
	[TestFixture]
	public class HtmlReaderTests
	{
		private IEnumerable<HtmlChunk> Read(string str)
		{
			using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(str)))
			{
				return WebBrowser.Html.HtmlReader.Read(stream).ToList();
			}
		}

		[Test]
		public void ParseLargeFile()
		{
			var sw = Stopwatch.StartNew();
			int c = 0;
			for (int i = 0; i < 100; i++)
			{
				c += Read(Properties.Resources.Large_Html).Count();	
			}
			sw.Stop();
			System.Console.WriteLine(string.Format("Read {0} chunks from {2}*100 chars, elsaped {1} ms", c, sw.ElapsedMilliseconds, Properties.Resources.Large_Html.Length));

			//2.4sec
		}
	}
}
