using System.Linq;
using Knyaz.Optimus.Dom;
using NUnit.Framework;
using Knyaz.Optimus.Tools;

namespace Knyaz.Optimus.Tests.Dom
{
	[TestFixture]
	public class DocumentElementExtensionTests
	{
		[Test]
		public void FlattenTest()
		{
			var doc = new Document();
			var div = doc.CreateElement("DIV");
			div.InnerHTML = "<p><span></span></p><br/>";
			var res = div.Flatten().Select(x => x.NodeName).ToArray();
			Assert.AreEqual(new[]{"DIV","P","SPAN","BR"}, res);
		}
	}
}