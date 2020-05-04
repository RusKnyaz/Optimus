using System.Linq;
using Knyaz.Optimus.Dom;
using Knyaz.Optimus.Dom.Elements;
using Knyaz.Optimus.TestingTools;
using NUnit.Framework;

namespace Knyaz.Optimus.Tests.Dom
{
	[TestFixture]
	public class HtmlOptionElementTests
	{
		[Test]
		public void ParentForm()
		{
			var document = new Document {Body = {InnerHTML = "<form><select><option></option></select></form>"}};
			var option = (HtmlOptionElement)document.GetElementsByTagName("option").Single();
			var form = document.GetElementsByTagName("form").Single();
			Assert.AreEqual(form, option.Form);
		}
		
		[Test]
		public void FormOfOptionsWithoutSelectIsNull()
		{
			var document = new Document {Body = {InnerHTML = "<form><option></option></form>"}};
			var option = (HtmlOptionElement)document.GetElementsByTagName("option").Single();
			Assert.IsNull(option.Form);
		}

		[Test]
		public void NeighbourForm()
		{
			var document = new Document{Body = {InnerHTML = 
				@"<form id='f'></form>
				  <select form='f'><option value=1>One</option></select>"}};
			var option = (HtmlOptionElement)document.GetElementsByTagName("option").Single();
			var form = document.GetElementsByTagName("form").Single();
			Assert.AreEqual(form, option.Form);
		}

		[Test]
		public void FormOfOptionInsideOptgroup()
		{
			var document = new Document{Body={InnerHTML = @"<form id=f></form>
				<select form=f><optgroup><option></option></optgroup></select>"}};
			var option = (HtmlOptionElement)document.GetElementsByTagName("option").Single();
			var form = document.GetElementsByTagName("form").Single();
			Assert.AreEqual(form, option.Form);
		}

		[Test]
		public void NestedOptgroupsDeprecated()
		{
			var document = new Document{Body={InnerHTML = @"<select><optgroup><optgroup><option></option></optgroup></optgroup></select>"}};
			document.GetElementsByTagName("option").Single().Assert(option=>
				((HtmlElement)option.ParentNode).TagName == TagsNames.OptGroup &&
				((HtmlElement)option.ParentNode.ParentNode).TagName == TagsNames.Select);
			
		}

		private static Document Doc(string html) => new Document {Body = {InnerHTML = html}}; 
		
		[Test]
		public void Index() => 
			((HtmlOptionElement)Doc(@"<select>
				<optgroup><option></option><option></option></optgroup>
				<option id=o></option>
				</select>")
			.GetElementById("o")).Assert(option => option.Index == 2);

		[Test]
		public void DefaultSelectedInitially() =>
			Doc(@"<select><option></option><option selected></option></select>")
			.Get<HtmlOptionElement>("option").ToArray()
			.Assert(options => 
					options[0].DefaultSelected == false &&
					options[1].DefaultSelected == true);
	}
}