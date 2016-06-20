#if NUNIT
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Knyaz.Optimus.Html;
using NUnit.Framework;

namespace Knyaz.Optimus.Tests.Html
{
	[TestFixture]
	public class HtmlParserTests
	{
		private IEnumerable<IHtmlNode> Parse(string str)
		{
			using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(str)))
			{
				return HtmlParser.Parse(stream).ToArray();
			}
		}

		[Test]
		public void SimpleElement()
		{
			var elem = Parse("<p id='8'>Text</p>").Cast<IHtmlElement>().Single();

			Assert.AreEqual("p", elem.Name);
			Assert.AreEqual("Text", ((IHtmlText) elem.Children.Single()).Value);
			Assert.AreEqual(1, elem.Attributes.Count);
		}

		[TestCase("<script>alert('1');</script>", "alert('1');")]
		[TestCase("<script>var html = '<div></div>';</script>", "var html = '<div></div>';")]
		[TestCase("<script>var html = '<div>';</script>", "var html = '<div>';")]
		[TestCase("<script>var html = '<div />';</script>", "var html = '<div />';")]
		[TestCase("<script>var html = '<script>console.log(1);</script>';</script>",
			"var html = '<script>console.log(1);</script>';")]
		//todo: escaped chars
		public void EmbeddedScript(string html, string scriptText)
		{
			var elem = Parse(html).Cast<IHtmlElement>().Single();

			Assert.AreEqual("script", elem.Name);
			Assert.AreEqual(scriptText, ((IHtmlText) elem.Children.Single()).Value);
			Assert.AreEqual(0, elem.Attributes.Count);
		}

		[Test]
		public void TextIsNotParent()
		{
			var elem = Parse("<head>\r\n\t<script>somecode</script>\r\n</head>").Cast<IHtmlElement>().Single();

			elem.Assert(e =>
				e.Name == "head" && 
				(Enumerable.First<IHtmlNode>(e.Children) as HtmlText).Value == "\n\n\t" &&
				(Enumerable.Skip<IHtmlNode>(e.Children, 1).First() as HtmlElement).Name == "script");
		}

		[Test]
		public void Text()
		{
			var elems = Parse("Hello").ToArray();

			Assert.AreEqual(1, elems.Length);
			var elem = elems[0];
			Assert.AreEqual("Hello", ((IHtmlText) elem).Value);
		}

		[Test]
		public void Comment()
		{
			var elems = Parse("<!-- Hello -->").ToArray();
			Assert.AreEqual(1, elems.Length);
			Assert.IsInstanceOf<HtmlComment>(elems[0]);
		}

		[Test]
		public void CommentInDiv()
		{
			var elems = Parse("<div><!-- Hello --></div>").ToArray();
			var elem = elems[0] as HtmlElement;
			Assert.IsNotNull(elem);
			Assert.AreEqual(1, elem.Children.Count);
			Assert.IsInstanceOf<HtmlComment>(elem.Children[0]);
		}

		[Test]
		public void CommentFollowedByDiv()
		{
			var elems = Parse("<!-- Hello --><div></div>").ToArray();
			Assert.AreEqual(2, elems.Length);
			Assert.IsInstanceOf<HtmlComment>(elems[0]);
			Assert.IsInstanceOf<HtmlElement>(elems[1]);
		}

		[Test]
		public void AttributeWithoutValue()
		{
			var elems = Parse("<script defer></script>").ToArray();
			Assert.AreEqual(1, elems.Length);
			var e = (HtmlElement)elems.FirstOrDefault();
			Assert.IsNotNull(e);
			Assert.AreEqual(1, e.Attributes.Count);
		}

		[Test]
		public void ScriptWithHtmlStrings()
		{
			var elems = Parse("<script defer>var a = $('<input type=\"file\">');console.log(a?'ok':'error');</script>").ToArray();
			Assert.AreEqual(1, elems.Length);
			var e = (HtmlElement)elems.FirstOrDefault();
			Assert.IsNotNull(e);
			Assert.AreEqual(1, e.Attributes.Count);
			Assert.AreEqual("var a = $('<input type=\"file\">');console.log(a?'ok':'error');", e.InnerText);
		}
	}
}
#endif