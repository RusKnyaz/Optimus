using System;
using Knyaz.Optimus.Dom.Elements;

namespace Knyaz.Optimus.Dom.Events
{
	/// <summary>
	/// Represents DOM changes event.
	/// </summary>
	[Obsolete]
	public class MutationEvent : Event
	{
		public ushort MODIFICATION {get { return 1; }}
		public ushort ADDITION  {get { return 2; }}
		public ushort REMOVAL {get { return 3; }}

		public Node RelatedNode { get; private set; }
		public string PrevValue { get; private set; }
		public string NewValue { get; private set; }
		public string AttrName { get; private set; }
		public ushort AttrChange { get; private set; }
		
		internal MutationEvent(HtmlDocument owner) : base(owner){}

		public void InitMutationEvent(string type, bool bubbles, bool cancelable, Node relatedNode,
			string prevValue, string newValue, string attrName, ushort attrChange)
		{
			InitEvent(type, bubbles, cancelable);
			RelatedNode = relatedNode;
			PrevValue = prevValue;
			NewValue = newValue;
			AttrName = attrName;
			AttrChange = attrChange;
		}
	}
}
