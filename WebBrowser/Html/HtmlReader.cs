using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WebBrowser.Html
{
	public struct HtmlChunk
	{
		public HtmlChunkTypes Type;
		public string Value;
	}

	public enum HtmlChunkTypes
	{
		TagStart,
		TagEnd,
		AttributeName,
		AttributeValue,
		Text,
		Comment,
		DocType
	}

	public class HtmlReader
	{
		enum States
		{
			ReadText,
			ReadTagName,
			ReadAttributeName,
			ReadSelfClosedTagEnd,
			ReadCloseTagName,
			WaitAttributeValue,
			ReadSpecTag,
			ReadDocType,
		}

		enum ReadScriptStates
		{
			Script,
			String,
			Comment
		}

		private static string ReadQuotation(char qMark, StreamReader reader)
		{
			var code = 0;
			var buffer = new List<char>();

			while (code != -1)
			{
				code = reader.Read();

				var symbol = '\0';

				if (code != -1)
				{
					symbol = (char)code;

					if (symbol == '\\')
					{
						code = reader.Read();
						if (code == -1)
							break;

						buffer.Add((char)code);
						continue;
					}

					if (symbol == qMark)
						break;
				}
				if (code != -1)
						buffer.Add(symbol);
			}

			return new string(buffer.ToArray());
		}

		private static HtmlChunk ReadScript(StreamReader reader)
		{
			var code = 0;
			var buffer = new List<char>();
			var state = ReadScriptStates.Script;
			char qMark = '\0';

			while (code != -1)
			{
				code = reader.Read();

				var symbol = '\0';

				if (code != -1)
				{
					symbol = (char) code;

					switch (state)
					{
						case ReadScriptStates.Script:
							if (symbol == '"' || symbol == '\'')
							{
								state = ReadScriptStates.String;
								qMark = symbol;
							}
							else if (symbol == '<')
							{
								var end = "/script>";
								var tmp = new char[end.Length];
								var i = 0;
								for (; i < end.Length; i++)
								{
									code = reader.Read();

									if (code > 0)
									{
										tmp[i] = (char) code;
									}
									else
									{
										break;
									}

									if (((char) code).ToString().ToLower()[0] != end[i])
										break;
								}

								if (i == end.Length)
									code = -1;
								else
								{
									buffer.Add(symbol);
									buffer.AddRange(tmp.Take(i + 1));
								}

								continue;
							}
							else if (symbol == '/')
							{
								var next = reader.Read();
								if (next == -1)
								{
									code = -1;
									break;
								}

								var nextChar = (char) next;
								if (nextChar == '/')
								{
									buffer.Add(symbol);
									buffer.Add('/');
									buffer.AddRange(reader.ReadLine());
									buffer.Add('\r');
									buffer.Add('\n');
									continue;
								}
								if (nextChar == '*')
								{
									state = ReadScriptStates.Comment;
								}
								buffer.Add(symbol);
								symbol = nextChar;
							}
						break;
						case ReadScriptStates.String:
							if (symbol == qMark)
								state = ReadScriptStates.Script;
							break;
						case ReadScriptStates.Comment:
							if (symbol == '*')
							{
								code = reader.Read();
								if (code == -1)
									break;

								symbol = (char)code;
								if (symbol == '/')
									state = ReadScriptStates.Script;
								
								buffer.Add('*');
							}
							break;
					}
					if(code != -1)
						buffer.Add(symbol);
				}
			}

			return new HtmlChunk() {Type = HtmlChunkTypes.Text, Value = new string(buffer.ToArray())};
		}

		private static HtmlChunk ReadComment(StreamReader reader)
		{
			
			var buffer = new List<char>();
			var state = ReadScriptStates.Script;
			char qMark = '\0';

			var code = reader.Read();

			if(code != '-')
				throw new HtmlInvalidFormatException("'-' expected at the start of comment");

			while (code != -1)
			{
				code = reader.Read();

				if (code != -1)
				{
					var symbol = (char) code;

					if (symbol == '-')
					{
						var next = reader.Read();
						if (next == -1)
							throw new HtmlInvalidFormatException("Unexpected end of stream while read comment.");

						var nextNext = reader.Read();
						if (nextNext == -1)
							throw new HtmlInvalidFormatException("Unexpected end of stream while read comment.");

						if (next == '-' && nextNext == '>')
							break;
						
						buffer.Add(symbol);
						buffer.Add((char)next);
						symbol = (char)nextNext;
					}
					buffer.Add(symbol);
				}
			}

			return new HtmlChunk() {Value = new string(buffer.ToArray()), Type = HtmlChunkTypes.Comment};
		}

		public static IEnumerable<HtmlChunk> Read(Stream stream)
		{
			var buffer = new List<char>();

			using (var reader = new StreamReader(stream))
			{
				var code = 0;
				var state = States.ReadText;
				var newState = States.ReadText;
				string lastTag = string.Empty;

				while (code != -1)
				{
					code = reader.Read();

					var symbol = '\0';

					if (code != -1)
					{
						symbol = (char)code;

						switch (state)
						{
							case States.ReadText:
								if (symbol == '<')
									newState = States.ReadTagName;
								break;

							case States.ReadTagName:
								switch (symbol)
								{
									case ' ': newState = States.ReadAttributeName; break;
									case '/': newState = buffer.Count > 0 
										? States.ReadSelfClosedTagEnd 
										: States.ReadCloseTagName; 
										break;
									case '>':
									{
										newState = States.ReadText;
										
									} 
									break;
									case '!': newState = States.ReadSpecTag; break;
								}
								break;

							case States.ReadSelfClosedTagEnd:
								if (symbol == '>')
									newState = States.ReadText;
								else
									throw new HtmlInvalidFormatException("'>' Expected at the end of tag after '/'");
								break;

							case States.ReadAttributeName:
								switch (symbol)
								{
									case '/': newState = States.ReadSelfClosedTagEnd; break;
									case '>': newState = States.ReadText; break;
									case '=': newState = States.WaitAttributeValue;break;
								}
								break;

							case States.ReadCloseTagName:
								if(symbol == '>')
									newState = States.ReadText;
								break;
							case States.WaitAttributeValue:
								if (symbol == '\"' || symbol == '\'')
								{
									var attrValue = ReadQuotation(symbol, reader);
									state=newState = States.ReadAttributeName;
									yield return new HtmlChunk(){Type = HtmlChunkTypes.AttributeValue, Value = attrValue};
									buffer.Clear();
									continue;
								}
								break;
							case States.ReadSpecTag:
								if (symbol == 'D')
								{
									newState = States.ReadDocType;
								}
								else if (symbol == '-')
								{
									yield return ReadComment(reader);
									state = newState = States.ReadText;
									continue;
								}
								else
								{
									throw new HtmlInvalidFormatException("Unknown spec tag.");
								}
								break;

							case States.ReadDocType:
								if(symbol == '>')
									newState = States.ReadText;
								break;
						}
					}

					if (code != -1 && (state == newState))
					{
						buffer.Add(symbol);
						state = newState;
						continue;
					}

					var data = new string(buffer.ToArray());

					switch (state)
					{
						case States.ReadText:
							if (buffer.Count > 0 )
								yield return new HtmlChunk { Type = HtmlChunkTypes.Text, Value = data };
							break;
						case States.ReadTagName:
							if (newState != States.ReadCloseTagName && newState != States.ReadSpecTag)
							{
								lastTag = new string(buffer.ToArray());
								yield return new HtmlChunk { Type = HtmlChunkTypes.TagStart, Value = lastTag };
							}
							break;
						case States.ReadSelfClosedTagEnd:
						case States.ReadCloseTagName:
							lastTag = string.Empty;
							yield return new HtmlChunk { Type = HtmlChunkTypes.TagEnd, Value = new string(buffer.ToArray()) };
							break;
						case States.ReadAttributeName:
							if(buffer.Count > 0)
								yield return new HtmlChunk { Type = HtmlChunkTypes.AttributeName, Value = new string(buffer.ToArray()).Trim() };
							break;
						case States.ReadDocType:
							if(data.Length < 7 || !data.StartsWith("OCTYPE "))
								throw new HtmlInvalidFormatException("DOCTYPE tag exptected");

							yield return new HtmlChunk { Type = HtmlChunkTypes.DocType, Value = data.Substring(7)};
							break;
					}

					if (newState == States.ReadText && lastTag.ToLowerInvariant() == "script")
					{
						yield return ReadScript(reader);
						yield return new HtmlChunk { Type = HtmlChunkTypes.TagEnd, Value = "script" };
						buffer.Clear();
						newState = state = States.ReadText;
						lastTag = string.Empty;
						continue;
					}

					if (newState != States.ReadSelfClosedTagEnd)
						buffer.Clear();
					
					state = newState;
				}
			}
		}
	}

	public class HtmlInvalidFormatException : Exception
	{
		public HtmlInvalidFormatException(string message)
			: base(message)
		{

		}
	}


	
}

