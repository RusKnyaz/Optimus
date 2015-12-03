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

		public static HtmlChunk AttributeName(List<char> buffer)
		{
			return new HtmlChunk {Type = HtmlChunkTypes.AttributeName, Value = new string(buffer.ToArray())};
		}

		public static HtmlChunk TagStart(List<char> buffer)
		{
			return new HtmlChunk { Type = HtmlChunkTypes.TagStart, Value = new string(buffer.ToArray()) };
		}
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
		static string[] _noContentTags = new []{"meta", "br", "link"};

		enum States
		{
			ReadText,
			ReadTagName,
			ReadAttributeName,
			ReadSelfClosedTagEnd,
			ReadCloseTagName,
			WaitAttributeValue,
			ReadSpecTag,
			ReadDocType
		}

		enum ReadScriptStates
		{
			Script,
			String,
			Comment
		}

		private static string ReadAttributeValue(char qMark, StreamReader reader)
		{
			var buffer = new List<char>();
			var specialSymbol = false;

			var escaped = false;
			for(var code = reader.Read(); code != -1; code = reader.Read())
			{
				var symbol = (char)code;

				if (symbol == '&' && !specialSymbol && !escaped)
				{
					buffer.AddRange(ReadSpecial(reader));
				}

				if (!escaped && symbol == '\\')
				{
					escaped = true;
				}
				else if (symbol != qMark || escaped)
				{
					escaped = false;
					buffer.Add(symbol);
				}
				else
				{
					break;
				}

				if (qMark == ' ' && reader.Peek() == '>')
					break;
			}

			return new string(buffer.ToArray());
		}

		private static string ReadSpecial(StreamReader reader)
		{
			var buffer = new List<char>();
			while (char.IsLetter((char)reader.Peek()))
			{
				buffer.Add((char)reader.Read());
			}

			var s = new string(buffer.ToArray());
			var symbol = (char)reader.Peek();
			if (symbol == ';')
			{
				reader.Read();
				switch (s)
				{
					case "rang":
						return "\u27E9";
					case "lang":
						return "\u27E8";
					case "notinva":
						return "\u2209";
					case "apos":return "'";
					case "Kopf":return "\uD835\uDD42"; 
					case "ImaginaryI":
						return "\u2148";
				}
			}

			return "&" + s + ";";
		}

		private static HtmlChunk ReadScript(StreamReader reader, string endWord)
		{
			var buffer = new List<char>();
			var state = ReadScriptStates.Script;
			var qMark = '\0';
			var escape = false;

			for (var code = reader.Read(); code != -1; code = reader.Read())
			{
				var symbol = (char) code;

				switch (state)
				{
					case ReadScriptStates.Script:
						if (symbol == '"' || symbol == '\'')
						{
							state = ReadScriptStates.String;
							qMark = symbol;
						}
						else if (symbol == '>')
						{
							if (IsEndsWith(buffer, endWord))
							{
								buffer.RemoveRange(buffer.Count - endWord.Length, endWord.Length);
								return new HtmlChunk { Type = HtmlChunkTypes.Text, Value = new string(buffer.ToArray()) };
							}
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
							else //regexp
							{
								if (buffer.Last() != '<')
								{
									qMark = '/';
									state = ReadScriptStates.String;
									escape = nextChar == '\\';
									buffer.Add(symbol);
									buffer.Add(nextChar);
									continue;
								}
							}

							buffer.Add(symbol);
							symbol = nextChar;
						}
						break;
					case ReadScriptStates.String:
						if (symbol == qMark && !escape)
							state = ReadScriptStates.Script;
						escape = !escape && symbol == '\\';
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

			return new HtmlChunk {Type = HtmlChunkTypes.Text, Value = new string(buffer.ToArray())};
		}

		private static HtmlChunk ReadComment(StreamReader reader, bool minusable)
		{
			var buffer = new List<char>();
			var endsByMinus = false;
			if (minusable && reader.Peek() == '-')
			{
				reader.Read();
				if (reader.Peek() == '-')
				{
					reader.Read();
					endsByMinus = true;
				}
				else
				{
					buffer.Add('-');
				}
			}

			if (endsByMinus)
				ReadToPhrase(reader, buffer, "-->");
			else 
				ReadToChar(reader, buffer, '>');
			
			return new HtmlChunk {Value = new string(buffer.ToArray()), Type = HtmlChunkTypes.Comment};
		}

		private static int ReadToChars(StreamReader reader, List<char> buffer, params char[] end)
		{
			for (var code = reader.Read(); code != -1; code = reader.Read())
			{
				var symbol = (char)code;
				if (end.Contains(symbol))
					return symbol;

				buffer.Add(symbol);
			}
			return -1;
		}

		private static bool ReadToChar(StreamReader reader, List<char> buffer, char end, bool unescape = false)
		{
			for (var code = reader.Read(); code != -1; code = reader.Read())
			{
				var symbol = (char)code;
				if (symbol == end)
					return true;

				if (symbol == '&')
				{
					buffer.AddRange(ReadSpecial(reader));
				}
				else
				{
					buffer.Add(symbol);	
				}
			}
			return false;
		}

		private static string ReadToPhrase(StreamReader reader, List<char> buffer, string end)
		{
			var lowerBuffer = new List<char>();
			for (var code = reader.Read(); code != -1; code = reader.Read())
			{
				var symbol = (char)code;

				buffer.Add(symbol);
				lowerBuffer.Add(char.ToLowerInvariant(symbol));

				if(!IsEndsWith(lowerBuffer, end)) continue;
				buffer.RemoveRange(buffer.Count-end.Length, end.Length);
				return end;
			}

			return string.Empty;
		}

		
		/// <summary>
		/// Case insensitive comparison
		/// </summary>
		/// <param name="buffer"></param>
		/// <param name="endPhrase"></param>
		/// <returns></returns>
		private static bool IsEndsWith(List<char> buffer, string endPhrase)
		{
			if (buffer.Count < endPhrase.Length)
				return false;
			
			var end = endPhrase.ToLowerInvariant();

			for (int i = 0, j = buffer.Count - end.Length; i < end.Length; i++, j++)
			{
				if(char.ToLowerInvariant(buffer[j]) != end[i])
					return false;
			}
			return true;
		}
		
		public static IEnumerable<HtmlChunk> Read(Stream stream)
		{
			var buffer = new List<char>();

			using (var reader = new StreamReader(stream))
			{
				var code = -1;
				var lastTag = string.Empty;

				var state = States.ReadText;
				var newState = States.ReadText;

				do
				{
					if (state == States.ReadText)
					{
						//Read text node to tag start or stream end
						var ex = ReadToChar(reader, buffer, '<', true);
						if (buffer.Count > 0)
						{
							yield return
								new HtmlChunk {Type = HtmlChunkTypes.Text, Value = new string(buffer.ToArray()).Replace("\u000D", "\u000A")};
						}
						if (!ex)
							yield break;

						buffer.Clear();

						newState = state = States.ReadTagName;
					}

					code = reader.Read();

					if (code != -1)
					{
						var symbol = (char) code;
						if (symbol == '\u000D')
							symbol = '\u000A';

						if (state == States.ReadText && symbol == '&')
						{
							buffer.AddRange(ReadSpecial(reader));
							continue;
						}

						switch (state)
						{
							case States.ReadText:
								if (symbol == '<') newState = States.ReadTagName;
								break;

							case States.ReadTagName:
								switch (symbol)
								{
									case ' ':
										if (buffer.Count > 0)
										{
											var tag = HtmlChunk.TagStart(buffer);
											yield return tag;
											lastTag = tag.Value;
											buffer.Clear();
										}

										AAA:
										var sym = ReadToChars(reader, buffer, '/', '>', '=', ' ');
										if (buffer.Count > 0)
										{
											yield return HtmlChunk.AttributeName(buffer);
											buffer.Clear();
										}

										if (sym == ' ')
											sym = Skip(reader, ' ');
									
										switch (sym)
										{
											case '/':
												state = newState= States.ReadSelfClosedTagEnd;
												break;
											case '>':
												state = newState = States.ReadText;
												break;
											case '=':
												state = newState = States.WaitAttributeValue;
												break;
											default:
												buffer.Add((char)sym);
												goto AAA;
										}
										
										continue;
									case '/':
										newState = buffer.Count > 0
											? States.ReadSelfClosedTagEnd
											: States.ReadCloseTagName;
										break;
									case '>':
										newState = States.ReadText;
										break;
									case '?':
										var com = ReadComment(reader, false);
										com.Value = "?" + com.Value;
										yield return com;
										state = newState = States.ReadText;
										continue;
									case '!':
										var ss = reader.Peek();
										if (ss != 'D' && ss != 'd')
										{
											yield return ReadComment(reader, true);
											state = newState = States.ReadText;
											continue;
										}
										newState = States.ReadSpecTag;
										break;
								}
								break;

							case States.ReadSelfClosedTagEnd:
								if (symbol == '>')
									newState = States.ReadText;
								break;

							case States.ReadAttributeName:
								switch (symbol)
								{
									case '/':
										newState = States.ReadSelfClosedTagEnd;
										break;
									case '>':
										newState = States.ReadText;
										break;
									case '=':
										newState = States.WaitAttributeValue;
										break;
								}
								break;

							case States.ReadCloseTagName:
								if (symbol == '>')
									newState = States.ReadText;
								break;
							case States.WaitAttributeValue:
								if (symbol != '>' && symbol != ' ')
								{
									var quoted = symbol == '\"' || symbol == '\'';
									var attrValue = ReadAttributeValue(quoted ? symbol : ' ', reader);
									state = newState = States.ReadAttributeName;
									yield return new HtmlChunk() {Type = HtmlChunkTypes.AttributeValue, Value = quoted ? attrValue : symbol + attrValue};
									buffer.Clear();
									continue;
								}
								break;
							case States.ReadSpecTag:
								switch (symbol)
								{
									case 'D':
									case 'd':
										newState = States.ReadDocType;
										break;
									default:
										throw new HtmlInvalidFormatException("Unknown spec tag.");
								}
								break;

							case States.ReadDocType:
								if (symbol == '>') newState = States.ReadText;
								break;
						}

						if (state == newState)
						{
							buffer.Add(symbol);
							state = newState;
							continue;
						}
					}

					var data = new string(buffer.ToArray());

					switch (state)
					{
						case States.ReadText:
							if (buffer.Count > 0)
								yield return new HtmlChunk {Type = HtmlChunkTypes.Text, Value = data};
							break;
						case States.ReadTagName:
							if (newState != States.ReadCloseTagName && newState != States.ReadSpecTag)
							{
								lastTag = new string(buffer.ToArray());
								yield return new HtmlChunk {Type = HtmlChunkTypes.TagStart, Value = lastTag};
							}
							break;
						case States.ReadSelfClosedTagEnd:
							yield return new HtmlChunk {Type = HtmlChunkTypes.TagEnd, Value = lastTag};
							lastTag = null;
							break;
						case States.ReadCloseTagName:
							yield return new HtmlChunk {Type = HtmlChunkTypes.TagEnd, Value = new string(buffer.ToArray())};
							lastTag = null;
							break;
						case States.ReadAttributeName:
							if (buffer.Count > 0)
								yield return new HtmlChunk {Type = HtmlChunkTypes.AttributeName, Value = new string(buffer.ToArray()).Trim()};
							break;
						case States.ReadDocType:
							yield return new HtmlChunk {Type = HtmlChunkTypes.DocType, Value = data.Substring(7)};
							break;
					}

					if (newState == States.ReadText && lastTag != null)
					{
						var tagInvariantName = lastTag.ToLowerInvariant();

						if (tagInvariantName == "script" || tagInvariantName == "style")
						{
							yield return ReadScript(reader, "</"+tagInvariantName);
							yield return new HtmlChunk { Type = HtmlChunkTypes.TagEnd, Value = tagInvariantName };
							buffer.Clear();
							newState = state = States.ReadText;
							lastTag = string.Empty;
							continue;
						}
						else if (tagInvariantName == "textarea")
						{
							var buffer1 = new List<char>();
							ReadToPhrase(reader, buffer1, "</textarea>");
							yield return new HtmlChunk { Value = new string(buffer1.ToArray()), Type = HtmlChunkTypes.Text };
							yield return new HtmlChunk { Type = HtmlChunkTypes.TagEnd, Value = tagInvariantName };
							buffer.Clear();
							newState = state = States.ReadText;
							lastTag = string.Empty;
							continue;
						}
						if (_noContentTags.Contains(tagInvariantName))
						{
							yield return new HtmlChunk { Type = HtmlChunkTypes.TagEnd, Value = lastTag };
							lastTag = null;
						}
					}

					if (newState != States.ReadSelfClosedTagEnd)
						buffer.Clear();

					state = newState;
				} while (code != -1);
			}
		}

		private static int Skip(StreamReader reader, char c)
		{
			for (var s = reader.Read(); s != -1; s = reader.Read())
			{
				if (s != c)
					return s;
			}
			return -1;
		}
	}

	public class HtmlInvalidFormatException : Exception
	{
		public HtmlInvalidFormatException(string message)
			: base(message)
		{

		}
	}

	public static class StreamReaderExtension
	{
		public static IEnumerable<char> ToEnumerable(this StreamReader reader)
		{
			for (var s = reader.Read(); s != -1; s = reader.Read())
			{
				yield return (char)s;
			}
		}
	}
	
}

