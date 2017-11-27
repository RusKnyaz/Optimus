namespace Knyaz.Optimus.Dom.Elements
{
	/// <summary>
	/// CharacterData is an abstract interface and does not exist as node. 
	/// It is used by Text, Comment, and ProcessingInstruction nodes.
	/// </summary>
	public abstract class CharacterData : Node
	{
		public string Data { get; set; }

		public string NodeValue { get { return Data; } set { Data = value; } }
		
		
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

	}

	public class Text : CharacterData
	{
		public Text() => NodeType = TEXT_NODE;

		public override Node CloneNode(bool deep) => new Text() { Data = Data };

		public override string ToString() => Data;

		public override string NodeName => "#text";

		/*todo: implement
		[NewObject] Text splitText(unsigned long offset);
		readonly attribute DOMString wholeText;
		*/
	}

	public class Comment : CharacterData
	{
		public Comment() => NodeType = COMMENT_NODE;

		public override Node CloneNode(bool deep) => new Comment { Data = Data };

		public override string NodeName => "#comment";
	}
}