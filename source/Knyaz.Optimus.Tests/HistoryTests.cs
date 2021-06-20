using Knyaz.Optimus.Dom;
using Knyaz.Optimus.Dom.Interfaces;
using Knyaz.Optimus.Environment;
using Knyaz.Optimus.ResourceProviders;
using Knyaz.Optimus.Tests.TestingTools;
using Moq;
using NUnit.Framework;

namespace Knyaz.Optimus.Tests.Dom
{
	/// <summary>
	/// The tests for window.history API
	/// </summary>
	[TestFixture]
	public class HistoryTests
	{
		private IResourceProvider _resourceProvider;
		private Engine _engine;
		Window Window => _engine.Window;
		private HtmlDocument Document => _engine.Document;
		IHistory History => Window.History;

		[SetUp]
		public void SetUp()
		{
			_resourceProvider = Mock.Of<IResourceProvider>().Resource("http://localhost", "<html></html>");//default blank page
			_engine = TestingEngine.BuildJint(_resourceProvider);
		}

		[TestCase("http://localhost", "/Action", "http://localhost/Action")]
		[TestCase("http://localhost", "http://remotehost", "http://remotehost")]
		public void PushStateChangesLocationUrl(string startUrl, string pushUrl, string expectedUrl)
		{
			_engine.OpenUrl(startUrl).Wait();
			History.PushState(null, null, pushUrl);
			Assert.AreEqual(expectedUrl, Window.Location.Href);
		}

		[TestCase("http://localhost/", "/Action")]
		[TestCase("http://localhost/", "http://remotehost")]
		public void PushStateAndGoBackRestoresUrl(string startUrl, string pushUrl)
		{
			_engine.OpenUrl(startUrl).Wait();
			History.PushState(null, null, pushUrl);
			History.Back();
			Assert.AreEqual(startUrl, Window.Location.Href);
		}

		[TestCase("http://localhost", "/Action", "http://localhost/Action")]
		[TestCase("http://localhost", "http://remotehost", "http://remotehost")]
		public void PushStateAndGoBackGoNextChangesUrl(string startUrl, string pushUrl, string expectedUrl)
		{
			_engine.OpenUrl(startUrl).Wait();
			History.PushState(null, null, pushUrl);
			History.Back();
			History.Forward();
			Assert.AreEqual(expectedUrl, Window.Location.Href);
		}

		[Test]
		public void PustStateChangesTitle()
		{
			_engine.OpenUrl("http://localhost").Wait();
			History.PushState(null, "Action page", "Action");
			Assert.AreEqual("Action page", Document.Title);
		}


		[TestCase("http://localhost", "/Action", "http://localhost/Action")]
		[TestCase("http://localhost", "http://remotehost", "http://remotehost")]
		public void ReplaceStateChagnesLocationUrl(string startUrl, string pushUrl, string expectedUrl)
		{
			_engine.OpenUrl(startUrl).Wait();
			History.ReplaceState(null, null, pushUrl);
			Assert.AreEqual(expectedUrl, Window.Location.Href);
		}


		[TestCase("http://localhost/", "/Action", "Do")]
		[TestCase("http://localhost/", "http://remotehost", "http://remotehostother")]
		public void PustStateReplaceStateAndGoBackLeadsToInitialUrl(string startUrl, string pushUrl, string replaceUrl)
		{
			_engine.OpenUrl(startUrl).Wait();
			History.PushState(null, null, pushUrl);
			History.ReplaceState(null, null, replaceUrl);
			History.Back();
			Assert.AreEqual(startUrl, Window.Location.Href);
		}

		//todo:
		//GoBackAndPushState...
		//border conditions
		//Go(0), Go(-1), Go(+1), Go(x)
	}
}