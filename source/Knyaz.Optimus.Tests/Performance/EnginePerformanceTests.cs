using Knyaz.Optimus.Dom.Elements;
using Knyaz.Optimus.ResourceProviders;
using Knyaz.Optimus.TestingTools;
using Knyaz.Optimus.Tests.Resources;
using Knyaz.Optimus.Tests.TestingTools;
using NBench;
using NUnit.Framework;

namespace Knyaz.Optimus.Tests.Performance
{
	public class EnginePerformanceTests
	{
		private IResourceProvider _resourceProvider;
		private Counter _counter;

		[PerfSetup]
		public void SetUp(BenchmarkContext ctx)
		{
			var vm =
				@"function VM() {
	var _this = this;	
	this.Name = ko.observable('World');
	this.Greeting = ko.computed(function(){return 'Hello, ' + _this.Name();});
}
ko.applyBindings(new VM());";
			
			_resourceProvider = Mocks.ResourceProvider("http://local", 
				"<html><head><script> " + R.KnockoutJs + " </script></head>" +
			    "<body>" +
			    "<input type='text' data-bind='value:Name' id='in'/>" +
			    "<span id = 'c1' data-bind='text:Greeting'/>" +
			    "</body><script>"+vm + "</script></html>");

			_counter = ctx.GetCounter("Counter");
		}
		
		[PerfBenchmark(NumberOfIterations = 5)]
		[CounterMeasurement("Counter")]
		[MemoryMeasurement(MemoryMetric.TotalBytesAllocated)]
		public void SimpleKnockout()
		{
			var engine = TestingEngine.BuildJint(_resourceProvider);
			var result = engine.OpenUrl("http://local");
			result.Wait();
			var doc = result.Result.Document;

			var input = (HtmlInputElement)doc.Body.ChildNodes[0];
			input.EnterText("Lord");
			var s = doc.GetElementById("c1");
			Assert.AreEqual("Hello, Lord", s.InnerHTML);
			_counter.Increment();
		}
	}
}