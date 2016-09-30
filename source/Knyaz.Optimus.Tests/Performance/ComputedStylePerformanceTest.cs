using System.Diagnostics;
using System.IO;
using System.Text;
using Knyaz.Optimus.TestingTools;
using NUnit.Framework;

namespace Knyaz.Optimus.Tests.Performance
{
	[TestFixture(1000)]
	public class ComputedStylePerformanceTest
	{
		private readonly int _repeats;

		public ComputedStylePerformanceTest(int repeats)
		{
			_repeats = repeats;
			GetComputedStyleTest("", "<div id=test></div>");
		}

		private Engine Load(string html)
		{
			var engine = new Engine() { ComputedStylesEnabled = true };
			engine.Load(
				new MemoryStream(
			Encoding.UTF8.GetBytes(
				html)));
			return engine;
		}

		[TestCase("", "<div id=test></div>")] //94
		[TestCase(@".a span > strong {font-family:""Arial""}", "<div class a><span><strong id=test></strong><span></div>")] //112
		[TestCase(@"strong {font-family:""Arial""}", "<div class a><span><strong id=test></strong><span></div>")] //15
		[TestCase(@"strong {font-family:""Arial""} .a string {font-family:""Curier New""}", "<div class a><span><strong id=test></strong><span></div>")] //9
		public void GetComputedStyleTest(string styles, string html)
		{
			var engine = Load("<head><style>" + styles + "</style></head><body>" + html + "</body>");
			var doc = engine.Document;
			var elt = doc.GetElementById("test");

			var timer = Stopwatch.StartNew();

			for (int i = 0; i < _repeats; i++)
			{
				elt.GetComputedStyle().GetPropertyValue("font-family");
			}
			timer.Stop();
			System.Console.WriteLine(timer.ElapsedMilliseconds);
		}
	}
}
