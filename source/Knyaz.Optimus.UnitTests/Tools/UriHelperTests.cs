using System;
using System.Text;
using Knyaz.Optimus.Tools;
using NUnit.Framework;

namespace Knyaz.Optimus.Tests.UnitTests.Tools
{
	[TestFixture]
	public class UriHelperTests
	{
		[TestCase("data:text/html,<b>hi</b>", "text/html", "<b>hi</b>", TestName="PlainTextData")]
		[TestCase("data:text/JavaScript;base64,Y29uc29sZS5sb2coJ2hpJyk7", "text/JavaScript", "console.log('hi');", TestName = "Base64Data")]
		[TestCase("data:text/javascript;charset=utf8,window", "text/javascript", "window")]
		public void Test(string uri, string expectedType, string expectedData)
		{
			var data = new Uri(uri).GetUriData();
			Assert.AreEqual(expectedType, data.Type);
			Assert.AreEqual(expectedData, Encoding.UTF8.GetString(data.Data));
		}
	}
}