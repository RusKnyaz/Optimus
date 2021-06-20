using System;
using Knyaz.Optimus.Dom;
using Knyaz.Optimus.Dom.Perf;
using Knyaz.Optimus.ResourceProviders;
using Knyaz.Optimus.Tools;
using NUnit.Framework;

namespace Knyaz.Optimus.Tests.EngineTests
{
	[TestFixture]
	public class XmlHttpRequestTests
	{
		[Test]
		public void ResponseTypeArrayBuffer()
		{
			var sync = new Object();
			var resourceProvider = Mocks.ResourceProvider("http://data", new byte[] {10, 1, 0});
			var xhr = new XmlHttpRequest(resourceProvider, () => sync, null, (s, s1) => new Request(s1, UriHelper.GetUri(s)));

			xhr.Open("GET", "http://data", false);
			xhr.ResponseType = "arraybuffer";
			xhr.Send();

			Assert.IsInstanceOf<ArrayBuffer>(xhr.Response);
			var buffer = (ArrayBuffer) xhr.Response;
			Assert.AreEqual(3, buffer.ByteLength);
			var barr = new Int8Array(buffer);
			Assert.AreEqual(10, barr[0]);
			Assert.AreEqual(1, barr[1]);
			Assert.AreEqual(0, barr[2]);
			Assert.Throws<Exception>(() =>
			{
				var x = xhr.ResponseText;
			});
		}

		[Test]
		public void ResponseTypeText()
		{
			var xhr = Send("text", "hello");

			Assert.AreEqual("hello", xhr.Response);
			Assert.AreEqual("hello", xhr.ResponseText);
		}

		[Test]
		public void ResponseTypeDocument()
		{
			var xhr = Send("document", "<html><body><div id=h>hello</div></body></html>");

			Assert.IsInstanceOf<HtmlDocument>(xhr.Response);
			Assert.AreEqual(xhr.Response, xhr.ResponseXML);
			Assert.AreEqual(xhr.ResponseXML, xhr.ResponseXML);
			Assert.AreEqual("hello", xhr.ResponseXML.GetElementById("h").InnerHTML);
		}

		[Test]
		public void ResponseTypeEmpty()
		{
			var xhr = Send("", "<html><body><div id=h>hello</div></body></html>");

			Assert.IsInstanceOf<string>(xhr.Response);
			Assert.AreEqual("<html><body><div id=h>hello</div></body></html>", xhr.Response);
			Assert.AreEqual(xhr.Response, xhr.ResponseText);
			Assert.IsNull(xhr.ResponseXML);
		}

		[Test]
		public void HashShouldNotBeSent()
		{
			var sync = new Object();
			var resourceProvider = Mocks.ResourceProvider("http://data", "hello");
			var xhr = new XmlHttpRequest(resourceProvider, () => sync, null, (s, s1) => new Request(s1, UriHelper.GetUri(s)));

			xhr.Open("GET", "http://data#some", false);
			xhr.Send();
			
			Assert.AreEqual("hello", xhr.Response);
		}

		XmlHttpRequest Send(string type, string data)
		{
			var sync = new Object();
			var resourceProvider = Mocks.ResourceProvider("http://data", data);
			var xhr = new XmlHttpRequest(resourceProvider, () => sync, null, (s, s1) => new Request(s1, UriHelper.GetUri(s)));

			xhr.Open("GET", "http://data", false);
			xhr.ResponseType = type;
			xhr.Send();
			return xhr;
		}
	}
}