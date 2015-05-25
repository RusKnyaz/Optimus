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

		private Engine CreateEngine(string body, string js){
			_engine.Load("<html><head><script>" + js + "</script></head><body>"+body+"</body></html>");
			return _engine;
		}

		[Test]
		public void NodeTest()
		{
			var engine = CreateEngine("<div id='d'><h1>1</h1><h2>2</h2><h3>3</h3></div>",
@"var e = document.getElementById('d');
console.log(e != null);
console.log(e == document.getElementById('d'));
console.log(e.tagName);
console.log(e.firstChild != null);
console.log(e.firstChild.tagName);
console.log(e.lastChild != null);
console.log(e.lastChild.tagName);
console.log(e.parentNode != null);
console.log(e.parentNode.tagName);
console.log(e.ownerDocument == document);
console.log(e.getAttribute('id'));");

			CollectionAssert.AreEqual(new object[] { true, true, "div", true, "h1", true, "h3", true, "body", true, "d" }, _log);
		}

		[Test]
		public void NodeEventsTest()
		{
			var engine = CreateEngine("<div id='d'><h1>1</h1><h2>2</h2><h3>3</h3></div>",
				@"var e = document.getElementById('d');
console.log(e.addEventListener != null);
console.log(e.removeEventListener != null);
console.log(e.dispatchEvent != null);
e.addEventListener('click', function(){console.log('click');}, true);
var ev = document.createEvent('Event');
ev.initEvent('click', false, false);
e.dispatchEvent(ev);");

			CollectionAssert.AreEqual(new object[] { true, true, true, "click" }, _log);
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
