using System.IO;
using System.Text;
using Knyaz.Optimus.TestingTools;
using NBench;

namespace Knyaz.Optimus.Tests.Performance
{
	public class ComputedStylePerformanceTest
	{
		private Counter _counter;
		
		[PerfSetup]
		public void SetUp(BenchmarkContext ctx) => _counter = ctx.GetCounter("Counter");

		
		private Engine Load(string html)
		{
			var engine = new Engine() { ComputedStylesEnabled = true };
			engine.Load(
				new MemoryStream(
			Encoding.UTF8.GetBytes(
				html)));
			return engine;
		}

		//[TestCase("", "<div id=test></div>")] //94
		//[TestCase(@".a span > strong {font-family:""Arial""}", "<div class a><span><strong id=test></strong><span></div>")] //112
		//[TestCase(@"strong {font-family:""Arial""}", "<div class a><span><strong id=test></strong><span></div>")] //15
//		[TestCase(@"strong {font-family:""Arial""} .a string {font-family:""Curier New""}", "<div class a><span><strong id=test></strong><span></div>")] //9
		
		[PerfBenchmark(NumberOfIterations = 3, RunTimeMilliseconds = 1000, RunMode = RunMode.Throughput)]
		[CounterMeasurement("Counter")]
		[MemoryMeasurement(MemoryMetric.TotalBytesAllocated)]
		public void GetComputedStyleTest()
		{
			var styles = @"strong {font-family:""Arial""} .a string {font-family:""Curier New""}";
			var html = "<div class a><span><strong id=test></strong><span></div>";
			
			var engine = Load("<head><style>" + styles + "</style></head><body>" + html + "</body>");
			var doc = engine.Document;
			var elt = doc.GetElementById("test");
			elt.GetComputedStyle().GetPropertyValue("font-family");
			_counter.Increment();
		}
	}
}
