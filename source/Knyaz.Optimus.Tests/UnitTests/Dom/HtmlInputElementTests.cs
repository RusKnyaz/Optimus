using Knyaz.Optimus.Dom;
using Knyaz.Optimus.Dom.Elements;
using NUnit.Framework;

namespace Knyaz.Optimus.Tests.Dom
{
	[TestFixture]
	public partial class HtmlInputElementTests
	{
		private Document _document;
		private HtmlInputElement _input;

		[SetUp]
		public void SetUp()
		{
			_document = new Document();
			_input = (HtmlInputElement)_document.CreateElement("input");
		}

		[Test]
		public void Defaults()
		{
			_input.Assert(input => 
				input.Type == "text" &&
				input.GetAttribute("type") == null &&
				input.Checked == false &&
				input.Disabled == false
				
				/* todo: implement default styles
				input.Style.Position == "absolute" &&
				input.Style.Display == "inline-block" &&
				input.Style.Top == "0px" &&
				input.Style.Left == "0px"*/);
		}

		[TestCase("<input id='i' type='checkbox' checked=false/>", true, Description = "Value does not matter")]
		[TestCase("<input id='i' type='checkbox' checked='checked'/>", true)]
		[TestCase("<input id='i' type='checkbox' checked='true'/>", true)]
		[TestCase("<input id='i' type='checkbox' checked/>", true)]
		[TestCase("<input id='i' type='checkbox'/>", false)]
		public void InputCheckedLoad(string html, bool expectedChecked)
		{
			var document = new Document();
			document.Write("<html><body>" + html + "</body></html>");
			var el = (HtmlInputElement)document.GetElementById("i");
			Assert.AreEqual(expectedChecked, el.Checked);
		}

		[Test]
		public void SetCheckedPropertyInitiallyUnchecked()
		{
			_input.Checked = true;
			Assert.IsTrue(_input.Checked);
			Assert.IsFalse(_input.HasAttribute("checked")); //changing of property does not affect the attribute
			Assert.IsNull(_input.GetAttribute("checked"));
			_input.Checked = false;
			Assert.IsFalse(_input.Checked);
			Assert.IsNull(_input.GetAttribute("checked"));
		}

		[Test]
		public void SetCheckedPropertyInitiallyChecked()
		{
			_input.SetAttribute("checked", "checked");
			_input.Checked = false;
			Assert.IsFalse(_input.Checked);
			Assert.IsTrue(_input.HasAttribute("checked"));
			Assert.AreEqual("checked", _input.GetAttribute("checked"));
		}

		[Test]
		public void AddRemoveCheckedAttr()
		{
			_input.SetAttribute("checked", null);
			Assert.IsTrue(_input.Checked);
			
			_input.RemoveAttribute("checked");
			Assert.IsFalse(_input.Checked);
		}

		[Test]
		public void CheckedAttrDoesNotAffectsAfterPropertInitiallySetted()
		{
			_input.SetAttribute("checked","checked");
			_input.Checked = false;
			_input.SetAttribute("checked","checked");
			Assert.IsFalse(_input.Checked);
		}

		[Test]
		public void CheckedAttrDoesNotAffectsAfterPropert()
		{
			_input.Checked = false;
			_input.SetAttribute("checked", "checked");
			Assert.IsFalse(_input.Checked);//note: it's IE's behavior. Chrome produces 'True'
		}

		[TestCase("true", true)]
		[TestCase("abc", true)]
		[TestCase(null, true)]
		public void DisabledFromAttribute(string attrValue, bool expectedValue)
		{
			_input.SetAttribute("disabled", attrValue);	
			Assert.AreEqual(expectedValue, _input.Disabled);
		}
	}
}