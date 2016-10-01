using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Knyaz.Optimus.Html
{
	struct CssChunk
	{
		public CssChunkTypes Type;
		public string Data;
	}

	enum CssChunkTypes
	{
		Selector,
		Property,
		Value,
		Directive
	}

	class CssReader
	{
		public static IEnumerable<CssChunk> Read(TextReader reader)
		{
			var type = CssChunkTypes.Selector;
			var newType = CssChunkTypes.Selector;
			var data = new StringBuilder();

			while (true)
			{
				var i = reader.Read();
				if (i < 0)
					break;

				var c = (char) i;

				if (c == '/' && reader.Peek() == '*')//skip comments
				{
					while(true)
					{
						var cc = reader.Read();
						if (cc == -1)
							yield break;

						if (cc == '*' && reader.Peek() == '/')
						{
							reader.Read();
							break;
						}
					} 
					continue;
				}

				switch (type)
				{
					case CssChunkTypes.Selector:
						if (c == '{')
							newType = CssChunkTypes.Property;
						else if(c == '@')
							newType = CssChunkTypes.Directive;
						break;
					case CssChunkTypes.Property:
						if (c == ':')
							newType = CssChunkTypes.Value;
						else if (c == '}')
							newType = CssChunkTypes.Selector;
						break;
					case CssChunkTypes.Value:
						if (c == ';')
							newType = CssChunkTypes.Property;
						else if (c == '}')
							newType = CssChunkTypes.Selector;
						break;
					case CssChunkTypes.Directive:
						if(c == ';' || c == '\n')
							newType = CssChunkTypes.Selector;
						break;
				}

				if (type != newType)
				{
					var buf = data.ToString().Trim('\r', '\n', ' ', '\t');
					if (buf != string.Empty)
						yield return new CssChunk {Type = type, Data = buf};
					type = newType;
					data.Clear();
				}
				else
				{
					data.Append(c);
				}
			}
		}
	}
}
