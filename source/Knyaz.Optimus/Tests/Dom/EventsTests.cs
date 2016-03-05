#if NUNIT
using System.Collections.Generic;
using Knyaz.Optimus.Dom;
using Knyaz.Optimus.Dom.Elements;
using NUnit.Framework;

namespace Knyaz.Optimus.Tests.Dom
{
	[TestFixture]
	class EventsTests
	{
		//capturing:the event propagation in direction from parent to child
		[TestCase(true, false, true, "BODY TABLE TD TR")]
		[TestCase(true, true, true, "BODY TABLE")]
		[TestCase(false, false, true, "BODY TABLE TD")]
		[TestCase(false, true, true, "BODY TABLE")]
		//Bubbling:the event propagation in direction from child to parent
		[TestCase(true, false, false, "TD TR TABLE BODY")]
		[TestCase(true, true, false, "TD TR TABLE")]
		[TestCase(false, false, false, "TD")]
		[TestCase(false, true, false, "TD")]
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