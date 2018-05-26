using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using Knyaz.Optimus.Dom.Elements;
using Knyaz.Optimus.ResourceProviders;
using Knyaz.Optimus.TestingTools;
using Moq;
using NUnit.Framework;
using Knyaz.Optimus.Tests.Resources;

namespace Knyaz.Optimus.Tests.EngineTests
{
	[TestFixture]
	public class JQueryTests
	{
		[Test]
		public void Smoke()
		{
			var engine = new Engine();
			engine.ScriptExecutor.OnException += exception => System.Console.WriteLine(exception);
			engine.Load("<html><head><script> " + R.JQueryJs + " </script></head><body></body></html>");
		}

		[TestCase(true, ExpectedResult = "zaza")]
		[TestCase(false, ExpectedResult = "")]
		public string JQueryIdSelectorInDeferScript(bool defer)
		{
			var resourceProvider = Mock.Of<IResourceProvider>()
				.Resource("http://localhost", "<html><head><script> " + R.JQueryJs + " </script><script src='test.js' "+ (defer?"defer":"") + "/></head><body><div id='uca'></div></body></html>")
				.Resource("http://localhost/test.js", "$('#uca').html('zaza');");
			var engine = new Engine(resourceProvider);
			engine.Console.OnLog +=o => System.Console.WriteLine(o.ToString());
			engine.OpenUrl("http://localhost").Wait();
			var ucaDiv = engine.Document.GetElementById("uca");
			return ucaDiv.InnerHTML;
		}

		[Test]
		public void JQueryIdSelectorIn()
		{
			var resourceProvider = Mock.Of<IResourceProvider>()
				.Resource("http://localhost", "<html><head><script> " + R.JQueryJs + " </script><script src='test.js'/></head><body><div id='uca'></div></body></html>")
				.Resource("http://localhost/test.js", "$('#uca').html('zaza');");
			var engine = new Engine(resourceProvider);
			engine.Console.OnLog += o => System.Console.WriteLine(o.ToString());
			engine.OpenUrl("http://localhost").Wait();
			var ucaDiv = engine.Document.GetElementById("uca");
			Assert.AreEqual("", ucaDiv.InnerHTML);
		}

		[Test]
		public void Post()
		{
			var resourceProvider = Mock.Of<IResourceProvider>()
				.Resource("http://localhost", "<html><head><script> " + R.JQueryJs + " </script><script src='test.js' defer/></head><body><div id='uca'></div></body></html>")
				.Resource("http://localhost/test.js", "$.post('http://localhost/data').done(function(x){console.log(x);});")
				.Resource("http://localhost/data", "OK");

			var engine = new Engine(resourceProvider);
			var log = new List<string>();
			engine.Console.OnLog += o =>
			{
				System.Console.WriteLine(o ?? "<null>");
				log.Add(o.ToString());
			};

			engine.OpenUrl("http://localhost");
			System.Threading.Thread.Sleep(1000);
			CollectionAssert.AreEqual(new[]{"OK"}, log);
		}
		

		[Test]
		public void JQueryCreate()
		{
			var script = "var a = $('<input type=\"file\">');console.log(a?'ok':'error');";
			var engine = new Engine();
			string result = null;
			engine.Console.OnLog += o => 
			{ 
				System.Console.WriteLine(o.ToString());
				result = o.ToString();
			};
			engine.Load("<html><head><script> " + R.JQueryJs + " </script><script defer>" + script + "</script></head><body><div id='uca'></div></body></html>");
			Assert.AreEqual("ok", result);
		}

		[Test]
		public void On()
		{
			var script = @"$('#b').on('click', function() {console.log('hi'); });
var e = document.createElement('div');
e.id = 'loaded';
document.body.appendChild(e);";

			var engine = new Engine();
			string result = null;
			engine.Console.OnLog += o =>
			{
				System.Console.WriteLine(o.ToString());
				result = o.ToString();
			};
			engine.Load("<html><head><script> " + R.JQueryJs + " </script></head><body><div id='b'></div></body><script>" + script + "</script></html>");
			var loaded = engine.WaitId("loaded");
			Assert.IsNotNull(loaded);
			
			var e = engine.Document.CreateEvent("Event");
			e.InitEvent("click", true, true);
			engine.Document.GetElementById("b").DispatchEvent(e);
			
			Assert.AreEqual("hi", result);
		}

		[Test]
		public void Bind()
		{
			var script = @"$('#b').bind('click', function() {console.log('hi'); });
var e = document.createElement('div');
e.id = 'loaded';
document.body.appendChild(e);";

			var engine = new Engine();
			string result = null;
			engine.Console.OnLog += o =>
			{
				System.Console.WriteLine(o.ToString());
				result = o.ToString();
			};
			engine.Load("<html><head><script> " + R.JQueryJs + " </script></head><body><div id='b'></div></body><script>" + script + "</script></html>");
			var loaded = engine.WaitId("loaded");
			Assert.IsNotNull(loaded);

			((HtmlElement)engine.Document.GetElementById("b")).Click();

			Assert.AreEqual("hi", result);
		}

		[Test]
		public void DocumentBody()
		{
			var script = @"$(function(){console.log(document.body);});";

			var engine = new Engine();
			object result = null;
			engine.Console.OnLog += o =>{result = o;};
			engine.Load("<html><head><script> " + R.JQueryJs + " </script><script>" + script + "</script></head><body><div id='b'></div></body></html>");
			Thread.Sleep(1000);

			Assert.IsNotNull(result);
		}

		[TestCase(".u", 1)]
		[TestCase("div", 1)]
		[TestCase("#a", 1)]
		public void Selector(string selector, int exptectedCount)
		{
			var script = @"console.log($('"+selector+"').length);";
			var engine = new Engine();
			object result = null;
			engine.Console.OnLog += o => {System.Console.WriteLine((o ??"null").ToString()); result = o; };
			engine.Load("<html><head><script> " + R.JQueryJs + " </script></head><body><div class = 'u' id='a'></div><script>" + script + "</script></body></html>");
			
			Assert.AreEqual(exptectedCount, result);
		}

		[Test]
		public void OnWithSelector()
		{
			var script = @"$('#a').on('click.some', '.u', function(){console.log('hi');});";
			var engine = new Engine();
			object result = null;
			engine.Console.OnLog += o => { System.Console.WriteLine((o ?? "null").ToString()); result = o; };
			engine.Load("<html><head><script> " + R.JQueryJs + " </script></head><body><div id='a'><span class='u' id='u'></span></div><script>" + script + "</script></body></html>");

			var u = engine.Document.GetElementById("u") as HtmlElement;
			u.Click();
			Assert.AreEqual("hi", result);
		}

		[Test]
		public void SetHtmlWithScript()
		{
			var script = "$('#target')['html']('<script>console.log(1);</script>');";
			var engine = new Engine();
			object result = null;
			engine.Console.OnLog += o => { System.Console.WriteLine((o ?? "null").ToString()); result = o; };

			engine.Load("<html><head><script> " + R.JQueryJs + " </script></head><body><div id='target'></div><script>" + script + "</script></body></html>");

			engine.DumpToFile("c:\\temp\\c.html");
			Assert.AreEqual(1, result);
		}
	}

	public static class StringExtension
	{
		public static Stream ToStream(this string str)
		{
			return new MemoryStream(Encoding.UTF8.GetBytes(str));
		}
	}
}