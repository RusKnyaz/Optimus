using System.Collections.Generic;
using NUnit.Framework;

namespace WebBrowser.Tests.EngineTests
{
	[TestFixture]
	public class EngineJsTests
	{
		private List<object> _log;
		private Engine _engine;

		[SetUp]
		public void SetUp()
		{
			_log =new List<object>();
			_engine = new Engine();
			_engine.Console.OnLog += _log.Add;
		}

		private Engine CreateEngineWithScript(string js)
		{
			_engine.Load("<html><head><script>"+js+"</script></head></html>");
			return _engine;
		}

		[Test]
		public void CreateDivTest()
		{
			var engine = CreateEngineWithScript(
@"var div = document.createElement('div');
console.log(div != null);
console.log(div.ownerDocument == document);
console.log(div.tagName);
console.log(div.parentNode == null);
console.log(div.appendChild != null);");

			CollectionAssert.AreEqual(new object[]{true, true, "div", true, true}, _log);
		}

		[Test]
		public void Document()
		{
			var engine = CreateEngineWithScript(
@"console.log(document == document);
console.log(document != null);
console.log(document.hasOwnProperty('ownerDocument'));
console.log(document.ownerDocument === null);
console.log(document.parentNode === null);
console.log(document.documentElement != null);
console.log(document.appendChild != null);");

			CollectionAssert.AreEqual(new object[] { true, true, true, true, true, true, true}, _log);
		}

		[Test]
		public void DocumentElement()
		{
			var engine = CreateEngineWithScript(
@"var e = document.documentElement;
console.log(e == e);
console.log(e.ownerDocument === document);
console.log(e.parentNode === document);
console.log(e.removeChild != null);
console.log(e.appendChild != null);");

			CollectionAssert.AreEqual(new object[] { true, true, true, true, true }, _log);
		}
	}
}
