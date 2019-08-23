using System.Linq;
using Knyaz.Optimus.Html;
using NBench;
using NUnit.Framework;
using R = Knyaz.Optimus.Tests.Resources.Resources;

namespace Knyaz.Optimus.Tests.Performance
{
	[TestFixture]
	public class HtmlReaderPerfTests
	{
		private Counter _counter;
		
		[PerfSetup]
		public void SetUp(BenchmarkContext ctx) => _counter = ctx.GetCounter("Counter");

		[PerfBenchmark(NumberOfIterations = 3, RunTimeMilliseconds = 1000, RunMode = RunMode.Throughput)]
		[CounterMeasurement("Counter")]
		[MemoryMeasurement(MemoryMetric.TotalBytesAllocated)]
		public void ParseLargeFile()
		{
			_counter.Increment();
			HtmlReader.Read(TestData.LargeHtml).Count();
		}

		[PerfBenchmark(NumberOfIterations = 3, RunTimeMilliseconds = 1000, RunMode = RunMode.Throughput)]
		[MemoryMeasurement(MemoryMetric.TotalBytesAllocated)]
		[CounterMeasurement("Counter")]
		public void ParseDeepFile()
		{
			_counter.Increment();
			HtmlReader.Read(TestData.DeepHtml).Count();
		}

		[PerfBenchmark(NumberOfIterations = 3, RunTimeMilliseconds = 1000, RunMode = RunMode.Throughput)]
		[MemoryMeasurement(MemoryMetric.TotalBytesAllocated)]
		[CounterMeasurement("Counter")]
		public void ParseLongFile()
		{
			_counter.Increment();
			HtmlReader.Read(TestData.LongHtml).Count();
		}

		[PerfBenchmark(NumberOfIterations = 3, RunTimeMilliseconds = 1000, RunMode = RunMode.Throughput)]
		[MemoryMeasurement(MemoryMetric.TotalBytesAllocated)]
		[CounterMeasurement("Counter")]
		public void ParseAttributesFile()
		{
			_counter.Increment();
			HtmlReader.Read(TestData.AttributesHtml).Count();
		}

		[PerfBenchmark(NumberOfIterations = 3, RunTimeMilliseconds = 1000, RunMode = RunMode.Throughput)]
		[MemoryMeasurement(MemoryMetric.TotalBytesAllocated)]
		[CounterMeasurement("Counter")]
		public void ParseBalancedFile()
		{
			_counter.Increment();
			HtmlReader.Read(TestData.BalancedHtml).Count();
		}
	}
}
