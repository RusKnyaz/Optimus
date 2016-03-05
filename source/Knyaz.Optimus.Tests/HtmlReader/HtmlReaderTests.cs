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
	public class HtmlReaderTests
	{
		private IEnumerable<HtmlChunk> Read(string str)
		{
			using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(str)))
			{
				return Knyaz.Optimus.Html.HtmlReader.Read(stream).ToList();
			}
		}

		[Test]
		public void ParseLargeFile()
		{
			var sw = Stopwatch.StartNew();
			int c = 0;
			for (int i = 0; i < 100; i++)
			{
				c += Read(Resources.Large_Html).Count();	
			}
			sw.Stop();
			System.Console.WriteLine("Read {0} chunks from {2}*100 chars, elsaped {1} ms", c, sw.ElapsedMilliseconds, Resources.Large_Html.Length);

			//2.4sec
		}
	}
}
