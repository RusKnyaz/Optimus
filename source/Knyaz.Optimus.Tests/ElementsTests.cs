using System.Threading.Tasks;
using Knyaz.Optimus.Dom.Elements;
using Knyaz.Optimus.ResourceProviders;
using Knyaz.Optimus.ScriptExecuting.Jint;
using NUnit.Framework;

namespace Knyaz.Optimus.Tests
{
	[TestFixture]
	public class ElementsTests
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
				.UseJint()
				.Build();
		
			var page = await engine.OpenUrl("http://localhost", false);

			var s = page.Document.GetElementById("s") as HtmlSelectElement;
			
			return s.SelectedIndex;
		}
	}
}