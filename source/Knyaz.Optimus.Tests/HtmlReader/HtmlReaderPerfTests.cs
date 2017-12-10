using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Knyaz.Optimus.Html;
using NUnit.Framework;
using R = Knyaz.Optimus.Tests.Resources.Resources;

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
			Read(R.Large_Html);

			Test(R.Large_Html, 100);
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
				R.Large_Html.Length);
		}

		[TestCase(1000, 0, 1, Description = "Deep")]
		[TestCase(1, 1000, 1, Description = "Attributes")]
		[TestCase(1, 0, 1000, Description = "Long")]
		[TestCase(10, 10, 10, Description = "Long")]
		public void Syntetic(int deep, int attrs, int count)
		{
			var tag =
				"<div " +
				String.Join(" ", Enumerable.Range(0, attrs).Select(x => "attr" + x + "=" + "val" + x))
				+">";

			var deepTag = string.Join("", Enumerable.Range(0, deep).Select(x => tag))
            			           + string.Join("", Enumerable.Range(0, deep).Select(x => "/<div>"));

			var html = string.Join("", Enumerable.Range(0, count).Select(x => deepTag));

			Test(html, 10);
		}
	}
}
