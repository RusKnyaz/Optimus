using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Knyaz.Optimus.Dom.Css.Expression
{
	internal static class CssExpressionParser
	{
		public static CssExpression Parse(TextReader reader)
		{
			var ors = new List<CssExpression>();
			
			CssExpression current = null;

			var currentOp = CssExpression.BinaryOperator.And;
			
			int code;
			while ((code = reader.Read()) >= 0)
			{
				var c = (char) code;
				switch (c)
				{
					case '*': current = Combine(current, CssExpression.All, currentOp);; break;
					case '.': current = Combine(current, CssExpression.Class(ReadText(reader)), currentOp); break; 
					case '#': current = Combine(current, CssExpression.Id(ReadText(reader)), currentOp); break;
					case ':': current = CssExpression.State(current, ReadText(reader)); break;
					case '~':
						while (reader.Peek() == ' ') reader.Read();
						currentOp = CssExpression.BinaryOperator.Prev;
						break;
					case '+':
						while (reader.Peek() == ' ') reader.Read();
						currentOp = CssExpression.BinaryOperator.PrevSibling;
						break;
					case ',':
						while (reader.Peek() == ' ') reader.Read();
						ors.Add(current);
						current = null;
						break;
					case '>':
						while (reader.Peek() == ' ') reader.Read();
						currentOp = CssExpression.BinaryOperator.Parent;
						break;
					case ' ':
						currentOp = ReadOperator(reader);
						break;
					case '[':
					{
						var attrValue = ReadText(reader, true).Split('=');
						reader.Read();
						
						var attr = attrValue[0];

						if (attr.Length == 0)
							break;
						
						if (attrValue.Length == 1)
						{
							current = Combine(current, 
								CssExpression.Attr(CssExpression.AttributeOperator.Exists, attrValue[0], null),
								currentOp);
							break;
						}
						
						var opChar = attr.Last();
						var attrOp = CssExpression.AttributeOperator.Equals;
						switch (opChar)
						{
							case '~': attrOp = CssExpression.AttributeOperator.DelimitedContains; break;
							case '|': attrOp = CssExpression.AttributeOperator.DelimitedStartWith;break;
							case '^': attrOp = CssExpression.AttributeOperator.StartWith;break;
							case '$': attrOp = CssExpression.AttributeOperator.EndWith;break;
							case '*': attrOp = CssExpression.AttributeOperator.Contains; break;
						}

						var val = attrValue[1];
						if (val.StartsWith("\""))
							val = val.Trim('"');
						else if (val.StartsWith("'"))
							val = val.Trim('\'');
						
						current = Combine(current, CssExpression.Attr(
							attrOp,
							attrOp == CssExpression.AttributeOperator.Equals ? attr : attr.Substring(0, attr.Length-1),
							val),
							currentOp);
						break;
					}
					case '\r':
					case '\n':
					case '\t':
						break;
					default:
						current = Combine(current, CssExpression.Tag(c+ReadText(reader)), currentOp);
						break;
				}
			}

			if (ors.Count > 0)
			{
				ors.Add(current);
				return CssExpression.Or(ors);
			}

			return current ?? CssExpression.None;
		}

		private static CssExpression.BinaryOperator ReadOperator(TextReader reader)
		{
			var result = CssExpression.BinaryOperator.Ancestor;
			var code = 0;
			while (code >= 0)
			{
				code = reader.Peek();
				switch (code)
				{
					case '>': 
						reader.Read();
						result = CssExpression.BinaryOperator.Parent;
						break;
					case '+': 
						reader.Read();
						result = CssExpression.BinaryOperator.PrevSibling;
						break;
					case ' ':
						code = reader.Read();
						break;
					default:
						return result;
				}
			}

			return result;
		}
		
		private static string ReadText(TextReader reader, bool includeTilda = false)
		{
			var result = new StringBuilder();
			int code;
			var escape = false;
			while ((code = reader.Peek()) >= 0)
			{
				var c = (char) code;

				if (!escape)
				{
					if (c == '.' || c == '#' || c == ' '
					 || c == ']' || c == ' ' || c == ',' 
					 || c == '+' || c == '[' || c == ':' 
					 || c == '>'
					    || (!includeTilda && c=='~'))
						break;

					if (c == '\\')
					{
						reader.Read();
						escape = true;
						continue;
					}
				}
				else
				{
					escape = false;
				}

				result.Append(c);
				reader.Read();
			}
			
			return result.ToString();
		}

		private static CssExpression Combine(CssExpression left, CssExpression right, CssExpression.BinaryOperator op) =>
			left == null ? right : CssExpression.Binary(op, left, right);
	}
}