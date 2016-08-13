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
		[TestCase("a {b:1}", "Selector:a Property:b Value:1")]
		[TestCase("a {b:1;c:2}", "Selector:a Property:b Value:1 Property:c Value:2")]
		[TestCase("a {b:1;}", "Selector:a Property:b Value:1")]
		[TestCase("a {b:1} \n.c{d:2}", "Selector:a Property:b Value:1 Selector:.c Property:d Value:2")]
		public void ReadCss(string css, string result)
		{
			var res = CssReader.Read(new StringReader(css)).Select(x => x.Type.ToString()+":"+x.Data).ToArray();
			Assert.AreEqual(result, string.Join(" ", res));
		}
	}
}
#endif