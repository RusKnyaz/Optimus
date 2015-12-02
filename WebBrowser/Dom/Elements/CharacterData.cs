using WebBrowser.Dom.Elements;

namespace WebBrowser.Dom
{
	public abstract class CharacterData : Node
	{
		public string Data { get; set; }

		public string NodeValue { get { return Data; } set { Data = value; } }

	}

	public class Text : CharacterData
	{
		public Text() 
		{
			NodeType = TEXT_NODE;
		}

		public override Node CloneNode(bool deep)
		{
			return new Text() { Data = Data };
		}

		public override string ToString()
		{
			return Data;
		}

		public override string NodeName
		{
			get { return "#text"; }
		}
	}

	public class Comment : CharacterData
	{
		public Comment()
		{
			NodeType = COMMENT_NODE;
		}

		public string Text { get { return Data; } }

		public override Node CloneNode(bool deep)
		{
			return new Comment { Data = Data };
		}

		public override string NodeName
		{
			get { return "#comment"; }
		}
	}
}