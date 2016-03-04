using WebBrowser.ScriptExecuting;

namespace WebBrowser.Dom.Elements
{
	[DomItem]
	public interface IMutationEvent
	{
		ushort MODIFICATION { get; }
		ushort ADDITION { get; }
		ushort REMOVAL { get; }
		Node RelatedNode { get; }
		string PrevValue { get; }
		string NewValue { get; }
		string AttrName { get; }
		ushort AttrChange { get; }

		void InitMutationEvent(string type, bool bubbles, bool cancelable, Node relatedNode,
			string prevValue, string newValue, string attrName, ushort attrChange);
	}

	public class MutationEvent : Event, IMutationEvent
	{
		public ushort MODIFICATION {get { return 1; }}
		public ushort ADDITION  {get { return 2; }}
		public ushort REMOVAL {get { return 3; }}

		public Node RelatedNode { get; private set; }
		public string PrevValue { get; private set; }
		public string NewValue { get; private set; }
		public string AttrName { get; private set; }
		public ushort AttrChange { get; private set; }

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
