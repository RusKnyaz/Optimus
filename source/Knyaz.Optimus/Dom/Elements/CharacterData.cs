namespace Knyaz.Optimus.Dom.Elements
{
	/// <summary>
	/// CharacterData is an abstract interface and does not exist as node. 
	/// It is used by Text, Comment, and ProcessingInstruction nodes.
	/// </summary>
	public abstract class CharacterData : Node
	{
		internal CharacterData(HtmlDocument owner):base(owner){}

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

		/// <summary>
		/// Returns a string containing the part of <see cref="Data"/> of the specified length and starting at the specified offset.
		/// </summary>
		public string SubstringData(int offset, int count) => Data.Substring(offset, count);

		/// <summary>
		/// Appends the given string to the <see cref="Data"/> string.
		/// </summary>
		/// <param name="text">String that specifies the text to insert</param>
		public void AppendData(string text) => Data = Data + (text ?? "null");

		/// <summary>
		/// Removes the specified part of the text content.
		/// </summary>
		/// <param name="offset">Integer that specifies the start position of the contents to remove.</param>
		/// <param name="count">Integer that specifies the number of characters to remove.</param>
		public void DeleteData(int offset, int count) => Data = Data.Remove(offset, count);


		/// <summary>
		/// Inserts text content into the current element at the specified position.
		/// </summary>
		/// <param name="offset">Integer that specifies the start position of the insertion.</param>
		/// <param name="text">String that specifies the text to insert.</param>
		public void InsertData(int offset, string text) => Data = Data.Insert(offset, text);

		/// <summary>
		/// Replaces the specified part of the text content.
		/// </summary>
		/// <param name="start">Integer that specifies start position of the contents to remove.</param>
		/// <param name="charsToRemove">Integer that specifies the number of characters to remove.</param>
		/// <param name="text">String that specifies the text to insert.</param>
		public void ReplaceData(int start, int charsToRemove, string text) =>
			Data = Data.Remove(start, charsToRemove).Insert(start, text);


		/// <summary>
		/// Returns an integer representing the size of the string contained in <see cref="Data"/>.
		/// </summary>
		public int Length => Data.Length;
		
		public string TextContent
		{
			get => NodeValue;
			set => NodeValue = value;
		}
	}

	/// <summary>
	/// Represents the text element in the DOM.
	/// </summary>
	public class Text : CharacterData
	{
		internal Text(HtmlDocument owner) : base(owner) => NodeType = TEXT_NODE;

		public override Node CloneNode(bool deep) => new Text(OwnerDocument) { Data = Data };

		public override string ToString() => Data;

		public override string NodeName => "#text";

		/*todo: implement
		[NewObject] Text splitText(unsigned long offset);
		readonly attribute DOMString wholeText;
		*/
	}

	/// <summary>
	/// Represents the comment element in the DOM.
	/// </summary>
	public class Comment : CharacterData
	{
		internal Comment(HtmlDocument owner) : base(owner) => NodeType = COMMENT_NODE;

		public override Node CloneNode(bool deep) => new Comment(OwnerDocument) { Data = Data };

		public override string NodeName => "#comment";
	}
}