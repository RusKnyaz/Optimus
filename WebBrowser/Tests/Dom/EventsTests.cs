#if NUNIT
using System.Collections.Generic;
using NUnit.Framework;
using WebBrowser.Dom;
using WebBrowser.Dom.Elements;

namespace WebBrowser.Tests.Dom
{
	[TestFixture]
	class EventsTests
	{
		//capturing:the event propagation in direction from parent to child
		[TestCase(true, false, true, "body table td tr")]
		[TestCase(true, true, true, "body table")]
		[TestCase(false, false, true, "body table td")]
		[TestCase(false, true, true, "body table")]
		//Bubbling:the event propagation in direction from child to parent
		[TestCase(true, false, false, "td tr table body")]
		[TestCase(true, true, false, "td tr table")]
		[TestCase(false, false, false, "td")]
		[TestCase(false, true, false, "td")]
		public void Propagation(bool bubblable, bool stop, bool capture, string expectedEventsOrder)
		{
			var doc = new Document();
			doc.Write("<html><body><table id='table'><tr id='row'><td id='cell'></td></tr></table></body></html>");
			var table = doc.GetElementById("table");
			var row = doc.GetElementById("row");
			var cell = doc.GetElementById("cell");
			
			var tags = new List<string>();

			doc.Body.AddEventListener("click", e => tags.Add(((Element)e.CurrentTarget).TagName), capture);
			table.AddEventListener("click", e => { tags.Add(((Element) e.CurrentTarget).TagName); if(stop)e.StopPropagation();}, capture);
			row.AddEventListener("click", e => tags.Add(((Element)e.CurrentTarget).TagName), false);
			cell.AddEventListener("click", e =>tags.Add(((Element)e.CurrentTarget).TagName), false);

			cell.RaiseEvent("click", bubblable, false);

			Assert.AreEqual(expectedEventsOrder, string.Join(" ", tags));
		}



		//todo: test default event handlers
	}
}
#endif