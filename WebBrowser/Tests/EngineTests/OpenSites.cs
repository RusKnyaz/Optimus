using System.Linq;
using System.Threading;
using NUnit.Framework;

namespace WebBrowser.Tests.EngineTests
{
	[TestFixture, Ignore]
	public class OpenSites
	{
		[TestCase("http://okkamtech.com")]
		[TestCase("http://ya.ru")]
		[TestCase("http://redmine.todosoft.org")]
		[TestCase("http://google.com")]
		[TestCase("https://html5test.com")]
		public void OpenUrl(string url)
		{
			var engine = new Engine();
			engine.OpenUrl(url);
		}

		[Test]
		public void Html5Score()
		{
			var engine = new Engine();
			engine.OpenUrl("https://html5test.com");

			var score = engine.Document.GetElementById("score");
			Assert.IsNotNull(score, "score");

			Thread.Sleep(1000);//wait calculation
			var tagWithValue = score.GetElementsByTagName("strong").FirstOrDefault();
			Assert.IsNotNull(tagWithValue, "strong");
			System.Console.WriteLine(tagWithValue.InnerHTML);
		}

		[Test]
		public void BrowseOkkam()
		{
			var engine = new Engine();
			engine.ScriptExecutor.OnException += exception => System.Console.WriteLine(exception.ToString());
			engine.OpenUrl("http://okkamtech.com");
			Thread.Sleep(10000);
			var userName = engine.Document.GetElementById("UserName");
			Assert.IsNotNull(userName);
		}

		[Test]
		public void BrowseKwinto()
		{
			var engine = new Engine();
			engine.ScriptExecutor.OnException += exception => System.Console.WriteLine(exception.ToString());
			engine.OpenUrl("http://192.168.1.36:8891");
			Thread.Sleep(15000);
			var userName = engine.Document.GetElementById("UserName");

			Assert.IsNotNull(userName);
		}
	}
}
