using System.Threading.Tasks;
using Knyaz.Optimus.Dom.Elements;
using NUnit.Framework;

namespace Knyaz.Optimus.Tests
{
	[TestFixture]
	public class HtmlSelectElementTests
	{
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
				.Build();
		
			var page = await engine.OpenUrl("http://localhost", false);

			var s = (HtmlSelectElement)page.Document.GetElementById("s");
			
			return s.SelectedIndex;
		}
	}
}