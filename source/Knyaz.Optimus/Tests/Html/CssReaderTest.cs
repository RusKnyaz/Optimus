#if NUNIT
using System.IO;
using System.Linq;
using Knyaz.Optimus.Html;
using NUnit.Framework;

namespace Knyaz.Optimus.Tests.Html
{
	[TestFixture]
	public class CssReaderTest
	{
		[TestCase("@import url(\"/styles/a.css\") screen;", "Directive:import url(\"/styles/a.css\") screen")]
		[TestCase("a {b:1}", "Selector:a Property:b Value:1")]
		[TestCase("a {b:1;c:2}", "Selector:a Property:b Value:1 Property:c Value:2")]
		[TestCase("a {b:1;}", "Selector:a Property:b Value:1")]
		[TestCase("a {b:1} \n.c{d:2}", "Selector:a Property:b Value:1 Selector:.c Property:d Value:2")]
		[TestCase("a {b:1 d}", "Selector:a Property:b Value:1 d")]
		[TestCase("a {b:\"1\"}", "Selector:a Property:b Value:\"1\"")]
		[TestCase("/*comment*/ a {b:1}", "Selector:a Property:b Value:1")]
		[TestCase("div{display:inline-block}.a{width:100px;height:100px}", "Selector:div Property:display Value:inline-block Selector:.a Property:width Value:100px Property:height Value:100px")]
		public void ReadCss(string css, string result)
		{
			var res = CssReader.Read(new StringReader(css)).Select(x => x.Type.ToString()+":"+x.Data).ToArray();
			Assert.AreEqual(result, string.Join(" ", res));
		}
	}
}
#endif