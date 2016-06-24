using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Knyaz.Optimus.Html;
using Knyaz.Optimus.Tests.Properties;
using NUnit.Framework;

namespace Knyaz.Optimus.Tests.HtmlReader
{
	[TestFixture]
	public class HtmlReaderPerfTests
	{
		private IList<HtmlChunk> Read(string str)
		{
			using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(str)))
			{
				return Knyaz.Optimus.Html.HtmlReader.Read(stream).ToList();
			}
		}

		[Test]
		public void ParseLargeFile()
		{
			Read(Resources.Large_Html);

			Test(Resources.Large_Html, 10);
			//2.4sec
		}

		private void Test(string html, int count)
		{
			var sw = Stopwatch.StartNew();
			int c = 0;
			for (int i = 0; i < count; i++)
			{
				c += Read(html).Count;
			}
			sw.Stop();
			System.Console.WriteLine("Read {0} chunks from {2}*100 chars, elsaped {1} ms", c, sw.ElapsedMilliseconds,
				Resources.Large_Html.Length);
		}

		[Test]
		public void DeepNesting()
		{
			var tagsCount = 1000;

			var html = string.Join("", Enumerable.Range(0, tagsCount).Select(x => "<div>"))
			           + string.Join("", Enumerable.Range(0, tagsCount).Select(x => "/<div>"));

			Test(html, 10);
		}

		[Test]
		public void LongFlat()
		{
			var tagsCount = 1000;

			var html = string.Join("", Enumerable.Range(0, tagsCount).Select(x => "<div></div>"));

			Test(html, 10);
		}
	}
}
