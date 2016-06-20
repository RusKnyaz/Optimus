using System;
using System.Linq;
using System.Threading;
using Knyaz.Optimus;
using Knyaz.Optimus.Tools;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.PhantomJS;
using OpenQA.Selenium.Support.UI;

namespace Optimus.Comparison
{
	[TestFixture]
    public class ComparisonTests
    {
		[TestCase("http://chi.todosoft.org", "logon")]
		public void OpenChicoryPhantom(string url, string itemId)
		{
			var options = new PhantomJSOptions();
			options.AddAdditionalCapability("ignoreProtectedModeSettings", true);
			using(var browser = new PhantomJSDriver(
					@"c:\projects\Optimus.Comparison\Optimus.Comparison\packages\PhantomJS.2.1.1\tools\phantomjs", options))
			{
				browser.Navigate().GoToUrl(url);
				Assert.IsNotNull(PhFind(browser, By.Id(itemId)));
			}
		}

		[TestCase("http://chi.todosoft.org", "logon")]
		public void OpenChicoryOptimus(string url, string itemId)
		{
			var engine = new Engine();
			//engine.AttachConsole();
			//engine.ResourceProvider.OnRequest += s => Console.WriteLine(s);
			engine.OpenUrl(url);
			engine.WaitId(itemId);
			Assert.IsNotNull(engine.Document.GetElementById(itemId));
		}

		[Test]
		public void Html5TestOptimus()
		{
			var engine = new Engine();
			engine.OpenUrl("https://html5test.com");
			engine.WaitDocumentLoad();
			Thread.Sleep(3000);//wait calculation

			var score = engine.Document.GetElementById("score");
			Assert.IsNotNull(score, "score");
			var tagWithValue = score.GetElementsByTagName("strong").FirstOrDefault();
			Assert.IsNotNull(tagWithValue, "strong");
			System.Console.WriteLine("Score: " + tagWithValue.InnerHTML);
		}

		[Test]
		public void Html5TestPhantom()
		{
			var options = new PhantomJSOptions();
			options.AddAdditionalCapability("ignoreProtectedModeSettings", true);
			using(var browser = new PhantomJSDriver(
					@"c:\projects\Optimus.Comparison\Optimus.Comparison\packages\PhantomJS.2.1.1\tools\phantomjs", options))
			{
				browser.Navigate().GoToUrl("https://html5test.com");
				var scoreElement = PhFind(browser, By.Id("score"));
				Assert.IsNotNull(scoreElement);
				var strong = PhFind(browser, scoreElement, By.ClassName("pointsPanel"))
					.FindElement(By.TagName("h2"))
					.FindElement(By.TagName("strong"));
				Assert.IsNotNull(strong, "strong");
				System.Console.WriteLine("Score: " + strong.Text);
			}
		}

		#region .    Helpers    .

		public IWebElement PhFind(PhantomJSDriver browser, By by)
		{
			var _wait = new WebDriverWait(browser, TimeSpan.FromSeconds(120));
			try
			{
				var result = _wait.Until(x => x.FindElements(by).FirstOrDefault() != null);
			}
			catch
			{

			}

			return browser.FindElement(by);
		}

		public IWebElement PhFind(PhantomJSDriver browser, IWebElement parent, By by)
		{
			var _wait = new WebDriverWait(browser, TimeSpan.FromSeconds(120));
			try
			{
				var result = _wait.Until(x => parent.FindElements(by).FirstOrDefault() != null);
			}
			catch
			{

			}

			return parent.FindElement(by);
		}
#endregion
    }


}
