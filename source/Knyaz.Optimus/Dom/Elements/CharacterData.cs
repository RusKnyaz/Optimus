namespace Knyaz.Optimus.Dom.Elements
{
	/// <summary>
	/// CharacterData is an abstract interface and does not exist as node. 
	/// It is used by Text, Comment, and ProcessingInstruction nodes.
	/// </summary>
	public abstract class CharacterData : Node
	{
		internal CharacterData(Document owner) => SetOwner(owner);

		/// <summary>
		/// The text of this node.
		/// </summary>
		public string Data { get; set; }

		/// <summary>
		/// The text of this node.
		/// </summary>
		public string NodeValue { get => Data; set => Data = value;}

		/// <summary>
		/// Removes this node from parent.
		/// </summary>
		public void Remove() => ParentNode?.RemoveChild(this);

		/*
		 *
		todo: to be added
		/// <summary>
		/// The number of code units in data.
		/// </summary>
		public abstract int Length { get; }
		
		
		DOMString substringData(unsigned long offset, unsigned long count);
		void appendData(DOMString data);
		void insertData(unsigned long offset, DOMString data);
		void deleteData(unsigned long offset, unsigned long count);
		void replaceData(unsigned long offset, unsigned long count, DOMString data);*/

		public string TextContent
		{
			get => NodeValue;
			set => NodeValue = value;
		}
	}

	/// <summary>
	/// Represtents the text element in the DOM.
	/// </summary>
	public class Text : CharacterData
	{
		internal Text(Document owner) : base(owner) => NodeType = TEXT_NODE;

		public override Node CloneNode(bool deep) => new Text(OwnerDocument) { Data = Data };

		public override string ToString() => Data;

		public override string NodeName => "#text";

		/*todo: implement
		[NewObject] Text splitText(unsigned long offset);
		readonly attribute DOMString wholeText;
		*/
	}

	/// <summary>
	/// Represtents the comment element in the DOM.
	/// </summary>
	public class Comment : CharacterData
	{
		internal Comment(Document owner) : base(owner) => NodeType = COMMENT_NODE;

		public override Node CloneNode(bool deep) => new Comment(OwnerDocument) { Data = Data };

		public override string NodeName => "#comment";
	}
}