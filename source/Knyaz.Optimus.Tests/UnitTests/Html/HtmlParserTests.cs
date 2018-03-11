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
		private static IEnumerable<IHtmlNode> Parse(string str)
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
				(e.Children.First() as HtmlText).Value == "\n\n\t" &&
				(e.Children.Skip(1).First() as HtmlElement).Name == "script");
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

		[TestCase("<html>", "<html></html>")]
		[TestCase("<li><li>", "<li></li><li></li>")]
		[TestCase("<li>A<li>B", "<li>A</li><li>B</li>")]
		[TestCase("<div><li>A</div><li>B", "<div><li>A</li></div><li>B</li>")]
		[TestCase("<div><li></div>", "<div><li></li></div>")]
		[TestCase("<div><li></div><li>", "<div><li></li></div><li></li>")]
		[TestCase("<li><a>A</a><li>B", "<li><a>A</a></li><li>B</li>")]

		[TestCase("<dl>A<dl>B", "<dl>A</dl><dl>B</dl>")]
		[TestCase("<dl>A<dt>B<dl>C", "<dl>A</dl><dt>B</dt><dl>C</dl>")]

		[TestCase("<tr><tr>", "<tr></tr><tr></tr>")]
		[TestCase("<table><tr><td><tr></table>", "<table><tr><td></td></tr><tr></tr></table>")]
		[TestCase("<td><td><th><th><td>", "<td></td><td></td><th></th><th></th><td></td>")]

		[TestCase("<table><thead><tr><td><tbody><tr><td></table>", "<table><thead><tr><td></td></tr></thead><tbody><tr><td></td></tr></tbody></table>")]
		[TestCase("<table><thead><tr><td>asd<tr><tbody><tr><td>qqq</table>", "<table><thead><tr><td>asd</td></tr><tr></tr></thead><tbody><tr><td>qqq</td></tr></tbody></table>")]//checked with Chrome
		[TestCase("<table><thead><tr><td>asd<tr><tfoot><tr><td>qqq</table>", "<table><thead><tr><td>asd</td></tr><tr></tr></thead><tfoot><tr><td>qqq</td></tr></tfoot></table>")]
		[TestCase("<table><thead><tr><tbody></table>", "<table><thead><tr></tr></thead><tbody></tbody></table>")]

		[TestCase("<select><option>1<option>2</select>", "<select><option>1</option><option>2</option></select>")]
		[TestCase("<select><optgroup><option>1<option>2<optgroup><option>3</select", 
		          "<select><optgroup><option>1</option><option>2</option></optgroup><optgroup><option>3</option></optgroup></select>")]
		[TestCase("<div></div></ul><div></div>", "<div></div><div></div>", Description = "Sckip  unexpected end tag.")]
		[TestCase("<div>A</ul>B</div>", "<div>AB</div>",  Description = "Skip unexpected end tag and preserve text.")]
		public void OptionalEndTagTests(string sourceHtml, string expectedHtml)
		{
			var elems = Parse(sourceHtml);
			var result = string.Join("", elems.OfType<IHtmlElement>().Select(ElemToString));
			Assert.AreEqual(expectedHtml, result);
		}

		
		static string ElemToString(IHtmlElement arg)
		{
			var sb = new StringBuilder();
			BuildElemString(arg, sb);
			return sb.ToString();
		}

		static void BuildElemString(IHtmlNode node, StringBuilder sb)
		{
			var txtNode = node as IHtmlText;
			if (txtNode != null)
			{
				sb.Append(txtNode.Value);
				return;
			}

			var elem = node as IHtmlElement;

			if (elem != null)
			{
				sb.Append('<');
				sb.Append(elem.Name);
				sb.Append('>');

				if (elem.Children != null)
				{
					foreach (var child in elem.Children)
					{
						BuildElemString(child, sb);
					}
				}

				sb.Append("</");
				sb.Append(elem.Name);
				sb.Append('>');
			}
		}
	}
}