using Knyaz.Optimus.TestingTools;
using Knyaz.Optimus.Tests.TestingTools;
using NUnit.Framework;
using System.Threading.Tasks;

namespace Knyaz.Optimus.Tests.EngineTests
{
    partial class EngineUnitTests
    {
		[Test]
		public void WriteScriptFromScript()
		{
			var resourceProvider = Mocks
				.ResourceProvider("http://localhost/", "<html><head><script src='script.js'></script></head><body><div id=d1></div></body></html>")
				.Resource("http://localhost/script.js", "document.write('<script src=\"script2.js\"></script>');")
				.Resource("http://localhost/script2.js","document.write('<div id=d2>ABC</div>');");

			var task = Task.Run(() => TestingEngine.BuildJint(resourceProvider).OpenUrl("http://localhost"));
			Assert.IsTrue(task.Wait(1000));
			var page = task.Result;
			System.Diagnostics.Debug.Write(page.Document.InnerHTML);
			Assert.IsNotNull(page.Document.WaitId("d1", 1000), "Original content");
			Assert.IsNotNull(page.Document.WaitId("d2", 1000), "Generated added content");
		}
    }
}
