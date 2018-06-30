using System.IO;
using Knyaz.Optimus.Dom;
using NBench;

namespace Knyaz.Optimus.Tests.Performance
{
	public class DocumentBuilderPerfTests
	{
		private Counter _counter;
		
		private static void BuildDoc(Stream stream) => 
			DocumentBuilder.Build(DomImplementation.Instance.CreateHtmlDocument(), stream);
		
		[PerfSetup]
		public void SetUp(BenchmarkContext ctx) => _counter = ctx.GetCounter("Counter");

		[PerfBenchmark(NumberOfIterations = 3, RunTimeMilliseconds = 1000, RunMode = RunMode.Throughput)]
		[CounterMeasurement("Counter")]
		[MemoryMeasurement(MemoryMetric.TotalBytesAllocated)]
		public void BuildLargeHtml()
		{
			BuildDoc(TestData.LargeHtml);
			_counter.Increment();
		}
		
		[PerfBenchmark(NumberOfIterations = 3, RunTimeMilliseconds = 1000, RunMode = RunMode.Throughput)]
		[CounterMeasurement("Counter")]
		[MemoryMeasurement(MemoryMetric.TotalBytesAllocated)]
		public void BuildDeepHtml()
		{
			BuildDoc(TestData.DeepHtml);
			_counter.Increment();
		}
		
		[PerfBenchmark(NumberOfIterations = 3, RunTimeMilliseconds = 1000, RunMode = RunMode.Throughput)]
		[CounterMeasurement("Counter")]
		[MemoryMeasurement(MemoryMetric.TotalBytesAllocated)]
		public void BuildLongHtml()
		{
			BuildDoc(TestData.LongHtml);
			_counter.Increment();
		}
		
		[PerfBenchmark(NumberOfIterations = 3, RunTimeMilliseconds = 1000, RunMode = RunMode.Throughput)]
		[CounterMeasurement("Counter")]
		[MemoryMeasurement(MemoryMetric.TotalBytesAllocated)]
		public void BuildAttributesHtml()
		{
			BuildDoc(TestData.AttributesHtml);
			_counter.Increment();
		}
		
		[PerfBenchmark(NumberOfIterations = 3, RunTimeMilliseconds = 1000, RunMode = RunMode.Throughput)]
		[CounterMeasurement("Counter")]
		[MemoryMeasurement(MemoryMetric.TotalBytesAllocated)]
		public void BuildBalancedHtml()
		{
			BuildDoc(TestData.BalancedHtml);
			_counter.Increment();
		}
	}
}