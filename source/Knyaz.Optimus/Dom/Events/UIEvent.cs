using Knyaz.Optimus.Environment;
using Knyaz.Optimus.ScriptExecuting;

namespace Knyaz.Optimus.Dom.Events
{
	/// <summary>
	/// The base class for UI events.
	/// </summary>
	[DomItem]
	public class UIEvent : Event
	{
		internal UIEvent(Document owner) : base(owner){}
		
		public Window View {
			get;
			protected set;
		}
		/*
		readonly    attribute long    detail;*/
		//todo: complete
		
	}
}
