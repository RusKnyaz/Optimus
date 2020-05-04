using System;
using System.Globalization;
using System.Threading;
using Knyaz.Optimus.Tests.TestingTools;
using NUnit.Framework;

namespace Knyaz.Optimus.Tests
{
	[TestFixture]
	public class DocumentTests
	{
		[Test]
		public void SetCookie()
		{
			var rp = Mocks.ResourceProvider("http://knyaz.ru", Mocks.Page(""));
			var engine = TestingEngine.BuildJint(rp);
			engine.OpenUrl("http://knyaz.ru").Wait();
			var document = engine.Document;
			
			document.Cookie = "name=oeschger";
			document.Cookie = "favorite_food=tripe";
			
			Assert.AreEqual("name=oeschger; favorite_food=tripe", document.Cookie);
		}

		[Test]
		public void ChangeCookieValue()
		{
			var rp = Mocks.ResourceProvider("http://knyaz.ru", Mocks.Page(""));
			var engine = TestingEngine.BuildJint(rp);
			engine.OpenUrl("http://knyaz.ru").Wait();
			var document = engine.Document;
			
			document.Cookie = "name=oeschger";
			document.Cookie = "favorite_food=tripe";
			document.Cookie = "name=ivan";
			
			Assert.AreEqual("name=ivan; favorite_food=tripe", document.Cookie);
		}

		[Test]
		public void RemoveCookie()
		{
			var rp = Mocks.ResourceProvider("http://knyaz.ru", Mocks.Page(""));
			var engine = TestingEngine.BuildJint(rp);
			engine.OpenUrl("http://knyaz.ru").Wait();
			var document = engine.Document;
			
			document.Cookie = "name=oeschger";
			document.Cookie = "authid=12345";
			document.Cookie = "name=; Expires=Thu, 01 Jan 1970 00:00:01 GMT;";
			
			Assert.AreEqual("authid=12345", document.Cookie);
		}

		[Test]
		public void SetCookieExpiration()
		{
			var rp = Mocks.ResourceProvider("http://knyaz.ru", Mocks.Page(""));
			var engine = TestingEngine.BuildJint(rp);
			engine.OpenUrl("http://knyaz.ru").Wait();
			var document = engine.Document;
			
			var time = DateTime.UtcNow.AddSeconds(3).ToString(
				"ddd, dd MMM yyyy HH:mm:ss", CultureInfo.InvariantCulture);
			var cookie = "name=ivan; Expires="+ time +" GMT;";
			document.Cookie = cookie;
			
			Assert.AreEqual("name=ivan", document.Cookie, "Cookie value before expiration");
			Thread.Sleep(4000);
			Assert.AreEqual("", document.Cookie, "Cookie value after expiration");
		}
	}
}