using System.Threading.Tasks;
using Knyaz.Optimus.Dom;
using Knyaz.Optimus.Dom.Elements;
using Knyaz.Optimus.ResourceProviders;
using Knyaz.Optimus.ScriptExecuting.Jint;
using NUnit.Framework;

namespace Knyaz.Optimus.Tests.Dom
{
	[TestFixture]
	public class HtmlSelectElementTests
	{
		private Document _document;
		private HtmlSelectElement _select;

		[SetUp]
		public void SetUp()
		{
			_document = new Document();
			_select = (HtmlSelectElement)_document.CreateElement("select");
		}

		[Test]
		public void RemoveTest()
		{
			_select.AppendChild(_document.CreateElement("option"));
			Assert.AreEqual(1, _select.Length);
			_select.Remove(0);
			Assert.AreEqual(0, _select.Length);
		}

		[Test]
		public void AddingNonOption()
		{
			_select.AppendChild(_document.CreateElement("div"));
			_select.Assert(select => select.Length == 0 &&
			                         select.ChildNodes.Count == 1 &&
			                         select.Options.Length == 0);
		}

		[Test]
		public void AddingOptionTest()
		{
			_select.AppendChild(_document.CreateElement("option"));
			Assert.AreEqual(1, _select.Length);
			Assert.AreEqual(1, _select.ChildNodes.Count);
			Assert.AreEqual(1, _select.Options.Length);
		}

		[Test]
		public void OptionsItem()
		{
			var opt = _document.CreateElement("option");
			_select.AppendChild(opt);
			Assert.AreEqual(opt, _select.Options.Item(0));
		}

		[Test]
		public void OptionsIndexerByInt()
		{
			var opt = _document.CreateElement("option");
			_select.AppendChild(opt);
			Assert.AreEqual(opt, _select.Options[0]);
		}

		[Test]
		public void OptionsIndexerByName()
		{
			var opt = _document.CreateElement("option");
			opt.Id = "X";
			_select.AppendChild(opt);
			Assert.AreEqual(opt, _select.Options["X"]);
		}

		[Test]
		public void OptionsNamedItemById()
		{
			var opt = _document.CreateElement("option");
			opt.Id = "X";
			_select.AppendChild(opt);
			Assert.AreEqual(opt, _select.Options.NamedItem("X"));
		}

		[Test]
		public void OptionsNamedItemByName()
		{
			var opt = (HtmlOptionElement)_document.CreateElement("option");
			opt.Name = "X";
			_select.AppendChild(opt);
			Assert.AreEqual(opt, _select.Options.NamedItem("X"));
		}

		[Test]
		public void OptionsSetLengthIncrease()
		{
			_select.Options.Length = 2;
			Assert.AreEqual("<OPTION></OPTION><OPTION></OPTION>", _select.InnerHTML);
		}

		[Test]
		public void OptionsSetLengthDecrease()
		{
			var optX = (HtmlOptionElement)_document.CreateElement("option");
			optX.Name = "X";
			var optY = (HtmlOptionElement)_document.CreateElement("option");
			optY.Name = "Y";
			_select.AppendChild(optX);
			_select.AppendChild(optY);
			_select.Options.Length = 1;
			Assert.AreEqual(1, _select.Options.Length);
			Assert.AreEqual(optX, _select.Options.Item(0));
		}

		[Test]
		public void TypeTest()
		{
			_select.Multiple = true;
			Assert.AreEqual("select-multiple", _select.Type);
			_select.Multiple = false;
			Assert.AreEqual("select-one", _select.Type);
		}

		[Test]
		public void SetSelectedIndex()
		{
			var opt1 = (HtmlOptionElement)_document.CreateElement("option");
			var opt2 = (HtmlOptionElement)_document.CreateElement("option");
			_select.Add(opt1);
			_select.Add(opt2);

			_select.SelectedIndex = 1;
			_select.Assert(x => x.SelectedIndex == 1 && x.SelectedOptions[0] == opt2);
		}

		[Test]
		public void SelectedIndexOfEmptyIsMunusOne()
		{
			Assert.AreEqual(-1, _select.SelectedIndex);
		}

		[Test]
		public void DefaultSelectedIndexIsZeroForSingle()
		{
			_select.AppendChild(_select.OwnerDocument.CreateElement("option"));
			Assert.AreEqual(0, _select.SelectedIndex);
		}

		[Test]
		public void DefaultSelectedIndexIsMinusOneForMultiple()
		{
			_select.Multiple = true;
			Assert.AreEqual(-1, _select.SelectedIndex);
		}

		[Test]
		public void FirstAddedElementIsSelectedByDefault()
		{
			var option = (HtmlOptionElement)_select.OwnerDocument.CreateElement("option");
			option.Value = "ABC";
			_select.AppendChild(option);

			_select.Assert(x => x.SelectedOptions[0] == option && x.Value == "ABC");
		}

		[Test]
		public void Add()
		{
			var opt1 = (HtmlOptionElement)_document.CreateElement("option");
			var opt2 = (HtmlOptionElement) _document.CreateElement("option");
			_select.Add(opt1);
			_select.Add(opt2,opt1);
			_select.Assert(x => x.Options[0] == opt2 && x.Options[1] == opt1);
		}

		[Test]
		public void RemoveSelectedOption()
		{
			var opt1 = (HtmlOptionElement)_document.CreateElement("option");
			var opt2 = (HtmlOptionElement)_document.CreateElement("option");
			_select.Add(opt1);
			_select.Add(opt2);
			_select.Remove(0);
			_select.Assert(x => 
				x.Options[0] == opt2 &&
				x.SelectedOptions.Count == 1 &&
				x.SelectedOptions[0] == opt2);
			_select.Remove(0);
			Assert.AreEqual(-1, _select.SelectedIndex);
		}

		[TestCase("v1", "v1", 0, 1)]
		[TestCase("v2", "v2", 1, 1)]
		[TestCase("v3", "", -1, 0)]
		[TestCase("", "", -1, 0)]
		public void SetValue(string setValue, string expectedValue, int expectedIndex, int expectedSelectedCount)
		{
			var opt1 = (HtmlOptionElement)_document.CreateElement("option");
			opt1.Value = "v1";
			var opt2 = (HtmlOptionElement)_document.CreateElement("option");
			opt2.Value = "v2";
			_select.Add(opt1);
			_select.Add(opt2);

			_select.Value = setValue;
			
			_select.Assert(select => 
				select.Value == expectedValue &&
				select.SelectedIndex == expectedIndex &&
				select.SelectedOptions.Count == expectedSelectedCount);
		}

		[Test]
		public void MultipleAttribute()
		{
			_select.SetAttributeNode(_document.CreateAttribute("multiple"));
			_select.Assert(select => select.Multiple == true);
		}

		[Test]
		public void SetEmptyValue()
		{
			var opt1 = (HtmlOptionElement)_document.CreateElement("option");
			opt1.Value = "v1";
			var opt2 = (HtmlOptionElement)_document.CreateElement("option");
			_select.Add(opt1);
			_select.Add(opt2);

			_select.Value = "";
			_select.Assert(select => select.Value == "" && select.SelectedIndex == 1 && select.SelectedOptions.Count == 1);
		}
		
		//Note: do not change formatting in html. tabs and spaces matter
		[TestCase(@"<select id=s multiple>
				<option>123
				<option>ABCDEFGHIKLMNOPQRST
				</select>", ExpectedResult = -1, Description = "Default selected index is -1 for multiple select list")]
		[TestCase(@"<select id=s>
				<option>123
				<option>ABCDEFGHIKLMNOPQRST
				</select>", ExpectedResult = 0, Description = "Default selected index is 0 when text nodes exist")]
		[TestCase(@"<select id=s>
				<option selected>123
				<option>ABCDEFGHIKLMNOPQRST
				</select>", ExpectedResult = 0)]
		
		[TestCase(@"<select id=s>
				<option>123
				<option selected>ABCDEFGHIKLMNOPQRST
				</select>", ExpectedResult = 1)]
		public static async Task<long> SelectedIndex(string html)
		{
			var resourceProvider = Mocks.ResourceProvider("http://localhost", "<html>" + html + "</html>");
			
			var engine = new EngineBuilder()
				.SetResourceProvider(resourceProvider)
				.UseJint()
				.Build();
		
			var page = await engine.OpenUrl("http://localhost", false);

			var s = page.Document.GetElementById("s") as HtmlSelectElement;
			
			return s.SelectedIndex;
		}
	}
}