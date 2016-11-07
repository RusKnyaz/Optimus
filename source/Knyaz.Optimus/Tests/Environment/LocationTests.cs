#if NUNIT
using System;
using Knyaz.Optimus.Environment;
using Moq;
using NUnit.Framework;

namespace Knyaz.Optimus.Tests.Environment
{
	public class LocationTests
	{
		Location CreateLocation(string href)
		{
			var uri = new Uri(href);
			var engine = new Mock<IEngine>();
			engine.SetupGet(x => x.Uri).Returns(() => uri);
			engine.Setup(x => x.OpenUrl(It.IsAny<string>())).Callback<string>(x => uri = new Uri(x));
			return new Location(engine.Object);
		}

		[TestCase("#part1", "#part1", "http://localhost#part1")]
		[TestCase("part1", "#part1", "http://localhost#part1")]
		public void Hash(string hash, string expectedHash, string expectedHref)
		{
			var l = CreateLocation("http://localhost");
			l.Hash = hash;
			l.Assert(location => location.Hash == expectedHash && location.Href == expectedHref);
		}


		[TestCase("http://localhost/index.html", "127.0.0.1", "http://127.0.0.1/index.html")]
		[TestCase("http://localhost:1001/index.html", "127.0.0.1", "http://127.0.0.1/index.html")]
		[TestCase("http://localhost/index.html", "127.0.0.1:1001", "http://127.0.0.1:1001/index.html")]
		public void SetHost(string startUrl, string host, string expectedUrl)
		{
			var l = CreateLocation(startUrl);
			l.Host = host;
			Assert.AreEqual(expectedUrl, l.Href);
		}

		[TestCase("http://localhost/index.html", "localhost")]
		[TestCase("http://localhost:1001/index.html", "localhost:1001")]
		public void GetHost(string href, string expectedHost)
		{
			var l = CreateLocation(href);
			Assert.AreEqual(expectedHost, l.Host);
		}


		[TestCase("http://localhost/index.html", "127.0.0.1", "http://127.0.0.1/index.html")]
		[TestCase("http://localhost:1001/index.html", "127.0.0.1", "http://127.0.0.1:1001/index.html")]
		public void SetHostname(string startUrl, string hostname, string expectedUrl)
		{
			var l = CreateLocation(startUrl);
			l.Hostname = hostname;
			Assert.AreEqual(expectedUrl, l.Href);
		}

		[TestCase("http://localhost/index.html", "localhost")]
		[TestCase("http://localhost:1001/index.html", "localhost")]
		public void GetHostname(string href, string expectedHostname)
		{
			Assert.AreEqual(expectedHostname, CreateLocation(href).Hostname);
		}

		[TestCase("http://localhost", "http://localhost")]
		[TestCase("http://www.w3schools.com:4097/test.htm#part2:", "http://www.w3schools.com:4097")]
		public void GetOrigin(string href, string expectedOrigin)
		{
			Assert.AreEqual(expectedOrigin, CreateLocation(href).Origin);
		}

		[TestCase("http://www.w3schools.com:4097", "https://localhost:1010", "https://localhost:1010/")]
		[TestCase("http://www.w3schools.com:4097/test.htm", "https://localhost:1010", "https://localhost:1010/test.htm")]
		[TestCase("http://www.w3schools.com:4097/test.htm#part2:", "https://localhost:1010", "https://localhost:1010/test.htm#part2:")]
		public void SetOrigin(string href, string origin, string expectedHref)
		{
			var loc = CreateLocation(href);
			loc.Origin = origin;
			Assert.AreEqual(expectedHref, loc.Href);
		}

		[TestCase("http://localhost:8080/sub/index.html?a=b#part", "/sub/index.html?a=b")]
		[TestCase("http://localhost:8080/sub/index.html#part", "/sub/index.html")]
		[TestCase("http://localhost:8080/index.html", "/index.html")]
		[TestCase("http://localhost/index.html", "/index.html")]
		public void GetPathname(string href, string pathname)
		{
			Assert.AreEqual(pathname, CreateLocation(href).Pathname);
		}

		[TestCase("http://localhost:8080/sub/index.html?a=b#part", "abc", "http://localhost:8080/abc#part")]
		[TestCase("http://localhost:8080/sub/index.html#part", "abc?a=b", "http://localhost:8080/abc?a=b#part")]
		[TestCase("http://localhost:8080/index.html", "sub/index.html", "http://localhost:8080/sub/index.html")]
		[TestCase("http://localhost/index.html", "index.html", "http://localhost/index.html")]
		public void SetPathname(string originalHref, string pathname, string expectedHref)
		{
			var loc = CreateLocation(originalHref);
			loc.Pathname = pathname;
			Assert.AreEqual(expectedHref, loc.Href);
		}

		[TestCase("http://localhost:8080/sub/index.html?a=b#part", "?a=b")]
		[TestCase("http://localhost:8080/sub/index.html#part", "")]
		[TestCase("http://localhost:8080/index.html?a=b&c=d", "?a=b&c=d")]
		[TestCase("http://localhost/index.html", "")]
		public void GetSearch(string href, string expectedSearch)
		{
			Assert.AreEqual(expectedSearch, CreateLocation(href).Search);
		}

		[TestCase("http://localhost:8080/sub/index.html?a=b#part", "", "http://localhost:8080/sub/index.html#part")]
		[TestCase("http://localhost:8080/sub/index.html#part", "a=b", "http://localhost:8080/sub/index.html?a=b#part")]
		[TestCase("http://localhost:8080/index.html", "a=b&c=d", "http://localhost:8080/index.html?a=b&c=d")]
		[TestCase("http://localhost/index.html", "", "http://localhost/index.html")]
		public void SetSearch(string originalHref, string search, string expectedHref)
		{
			var loc = CreateLocation(originalHref);
			loc.Search = search;
			Assert.AreEqual(expectedHref, loc.Href);
		}
	}
}
#endif