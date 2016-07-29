#if NUNIT
using System.Linq;
using Knyaz.Optimus.Dom;
using Knyaz.Optimus.TestingTools;
using NUnit.Framework;

namespace Knyaz.Optimus.Tests.TestingTools
{
	[TestFixture]
	public class SelectorTests
	{
		[TestCase("<span id=span1></span>", "#span1", "<SPAN id=\"span1\"></SPAN>", Description = "Root by id")]
		[TestCase("<div><span id=span1></span></div>", "#span1", "<SPAN id=\"span1\"></SPAN>", Description = "Nested by id")]
		[TestCase("<div><span id=span1></span></div>", "span", "<SPAN id=\"span1\"></SPAN>", Description = "Nested by tag name")]
		[TestCase("<div id=div1><span>1</span></div><DIV id=div2><SPAN>2</SPAN></DIV>", "#div2 span", "<SPAN>2</SPAN>", Description = "By id, than by tag name")]
		[TestCase("<span class='A'></span>", ".A", "<SPAN class=\"A\"></SPAN>", Description = "Simple class selector")]
		[TestCase("<span class='B A'></span>", ".A", "<SPAN class=\"B A\"></SPAN>", Description = "Simple class selector from multiclass defenition")]
		[TestCase("<div class=A><span>1</span></div><DIV><SPAN>2</SPAN></DIV>", ".A span", "<SPAN>1</SPAN>", Description = "By class than by tag name")]
		[TestCase("<div id=a></div><div id=b></div>", "[id=b]","<DIV id=\"b\"></DIV>")]
		[TestCase("<div><div><label for='OrganizationId'></label></div></div>", "label[for=OrganizationId]", "<LABEL for=\"OrganizationId\"></LABEL>")]
		[TestCase("<div a=ab></div><div a=ac></div><div a=bc></div>", "[a^=a]", "<DIV a=\"ab\"></DIV>,<DIV a=\"ac\"></DIV>")]
		public void DocumentSelectorTests(string html, string selector, string expectedResult)
		{
			var doc = new Document();
			doc.Write(html);
			Assert.AreEqual(expectedResult, string.Join(",", doc.Select(selector).Select(x => x.ToString())));
		}
	}
}
#endif