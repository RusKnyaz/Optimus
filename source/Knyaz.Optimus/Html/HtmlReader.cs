using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Knyaz.Optimus.Html
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

		public static HtmlChunk Text(List<char> buffer)
		{
			return new HtmlChunk { Type = HtmlChunkTypes.Text, Value = new string(buffer.ToArray()).Replace("\u000D", "\u000A") };
		}

		public static HtmlChunk Text(string buffer)
		{
			return new HtmlChunk { Type = HtmlChunkTypes.Text, Value = buffer.Replace("\u000D", "\u000A") };
		}
	}

	public enum HtmlChunkTypes
	{
		Undefined,
		TagStart,
		TagEnd,
		AttributeName,
		AttributeValue,
		Text,
		Comment,
		DocType
	}

	public class HtmlInvalidFormatException : Exception
	{
		public HtmlInvalidFormatException(string message)
			: base(message)
		{

		}
	}

	public class HtmlReader
	{
		static string[] _noContentTags = { "meta", "br", "link" };

		enum ReadScriptStates
		{
			Script,
			String,
			Comment
		}

		private static string ReadUnquotedAttributeValue(StreamReader reader)
		{
			var buffer = new StringBuilder();

			var escaped = false;

			do
			{
				var symbol = (char)reader.Peek();

				if (!escaped)
				{
					if (symbol == '/' || symbol == ' ' || symbol == '>')
						break;

					if (symbol == '&')
					{
						reader.Read();
						buffer.Append(ReadSpecial(reader));
					}

					if (symbol == '\\')
					{
						reader.Read();
						escaped = true;
						continue;
					}
				}

				reader.Read();
				escaped = false;

				buffer.Append(symbol);
			} while (!reader.EndOfStream);
			
			return buffer.ToString();
		}

		private static string ReadAttributeValue(StreamReader reader)
		{
			var buffer = new StringBuilder();
			var escaped = false;
			var qMark = reader.Read();

			do
			{
				var symbol = (char)reader.Peek();

				if (!escaped)
				{
					if (symbol == qMark)
					{
						reader.Read();
						break;
					}

					if (symbol == '&')
					{
						reader.Read();
						buffer.Append(ReadSpecial(reader));
					}

					if (symbol == '\\')
					{
						reader.Read();
						escaped = true;
						continue;
					}
				}

				reader.Read();
				escaped = false;

				buffer.Append(symbol);
			} while (!reader.EndOfStream);

			return buffer.ToString();
		}

		private static string ReadSpecial(StreamReader reader)
		{
			var buffer = new List<char>();

			var symbol = (char)reader.Peek();
			while (char.IsLetter(symbol) && !reader.EndOfStream)
			{
				buffer.Add((char)reader.Read());
				symbol = (char)reader.Peek();
			}

			var s = new string(buffer.ToArray());
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
					case "apos": return "'";
					case "Kopf": return "\uD835\uDD42";
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

			if (reader.EndOfStream)
				return new HtmlChunk {Type = HtmlChunkTypes.Text, Value = string.Empty};

			do
			{
				var symbol = (char)reader.Read();

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
							if (IsEndsWith(buffer, endWord.ToLowerInvariant()))
							{
								buffer.RemoveRange(buffer.Count - endWord.Length, endWord.Length);
								return new HtmlChunk {Type = HtmlChunkTypes.Text, Value = new string(buffer.ToArray())};
							}
						}
						else if (symbol == '/')
						{
							if (reader.EndOfStream)
								break;

							var next = reader.Read();
							var nextChar = (char) next;
							if (nextChar == '/')
							{
								buffer.Add(symbol);
								buffer.Add('/');
								buffer.AddRange(reader.ReadLineWithEndings());
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
							var code = reader.Read();
							if (reader.EndOfStream)
								break;

							symbol = (char) code;
							if (symbol == '/')
								state = ReadScriptStates.Script;

							buffer.Add('*');
						}
						break;
				}
				
				buffer.Add(symbol);
			} while (!reader.EndOfStream);

			return new HtmlChunk { Type = HtmlChunkTypes.Text, Value = new string(buffer.ToArray()) };
		}

		private static string ReadComment(StreamReader reader, bool minusable)
		{
			var buffer = new StringBuilder();
			var endsByMinus = false;
			if (minusable)
			{
				var x = (char)reader.Read();
				if (x != '-' || reader.Peek() != '-')
				{
					buffer.Append(x);
				}
				else
				{
					reader.Read();
					endsByMinus = true;
				}
			}

			if (endsByMinus)
				ReadToPhrase(reader, buffer, "-->");
			else
				ReadToCharWithChar(reader, buffer, '>');

			return buffer.ToString();
		}

		private static void ReadToCharWithChar(StreamReader reader, StringBuilder buffer, char end, bool unescape = false)
		{
			while(!reader.EndOfStream)
			{
				var symbol = (char)reader.Read();
				if (symbol == end)
					return;

				if (symbol == '&' && !unescape)
					buffer.Append(ReadSpecial(reader));
				else
					buffer.Append(symbol);
			} 
		}

		private static void ReadToChar(StreamReader reader, StringBuilder buffer, char end, bool unescape = false)
		{
			while (!reader.EndOfStream)
			{
				var sym = (char)reader.Peek();

				if (sym == '&' && !unescape)
				{
					reader.Read();
					buffer.Append(ReadSpecial(reader));
					continue;
				}
				
				if (sym == end)
					return;

				reader.Read();
				buffer.Append(sym);
			}
		}


		private static void ReadToPhrase(StreamReader reader, StringBuilder buffer, string end)
		{
			var lowerBuffer = new List<char>();

			while (!reader.EndOfStream)
			{
				var symbol = (char)reader.Read();

				buffer.Append(symbol);
				lowerBuffer.Add(char.ToLowerInvariant(symbol));

				if (!IsEndsWith(lowerBuffer, end)) continue;
				buffer.Remove(buffer.Length - end.Length, end.Length);
				return;
			}
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

			for (int i = 0, j = buffer.Count - endPhrase.Length; i < endPhrase.Length; i++, j++)
			{
				if (char.ToLowerInvariant(buffer[j]) != endPhrase[i])
					return false;
			}
			return true;
		}

		public static IEnumerable<HtmlChunk> Read(Stream stream)
		{
			return Read(new StreamReader(stream));
		}

		private static IEnumerable<HtmlChunk> Read(StreamReader reader, string endTag = null)
		{
			if(reader.EndOfStream)
				yield break;

			var text = new StringBuilder();

			while (!reader.EndOfStream)
			{
				var symbol = (char)reader.Read();
				if (symbol == '<')
				{
					var next = (char)reader.Peek();

					if (endTag != null && next == '/') //probably end of tag
					{
						reader.Read();
						var t2 = new StringBuilder();
						ReadToChar(reader, t2, '>');
						var closedTagName = t2.ToString();
						if (closedTagName.ToLowerInvariant() == endTag)
						{
							if (text.Length > 0)
								yield return HtmlChunk.Text(text.ToString());

							reader.Read();
							yield break;
						}

						text.Append('<');
						text.Append('/');
						text.Append(t2);
					}
					else if (char.IsLetter(next))
					{
						if (text.Length > 0)
						{
							yield return HtmlChunk.Text(text.ToString());
							text.Clear();
						}

						foreach (var chunk in ReadTag(reader))
							yield return chunk;
					}
					else if (next == '?')
					{
						yield return new HtmlChunk {Type = HtmlChunkTypes.Comment, Value = ReadComment(reader, false)};
					}
					else if(next == '!') //comment
					{
						reader.Read();
						var res= ReadComment(reader, true);
						yield return res.ToLowerInvariant().StartsWith("doctype") ?
								new HtmlChunk {Type = HtmlChunkTypes.DocType, Value = res.Remove(0, 7).TrimStart()}
								:new HtmlChunk { Type = HtmlChunkTypes.Comment, Value = res };
						
					}
					continue;
				}

				text.Append(symbol);
			}

			if (text.Length > 0)
				yield return HtmlChunk.Text(text.ToString());
		}

		private static IEnumerable<HtmlChunk> ReadTag(StreamReader reader)
		{
			var tagName = ReadUnquotedAttributeValue(reader);
			yield return new HtmlChunk{Value = tagName, Type = HtmlChunkTypes.TagStart};

			while (!reader.EndOfStream)
			{
				var sym = (char)reader.Peek();
				if (sym == '/')//may be self-closed tag
				{
					reader.Read();
					sym = (char)reader.Peek();
					if (sym == '>')
					{
						reader.Read();
						yield return new HtmlChunk {Value = tagName, Type = HtmlChunkTypes.TagEnd};
						yield break;
					}
				}
				else if (sym == '>') //end of tag
				{
					reader.Read();
					var invariantTagName = tagName.ToLowerInvariant();

					if (_noContentTags.Contains(invariantTagName))
					{
						yield return new HtmlChunk { Value = tagName, Type = HtmlChunkTypes.TagEnd };
						break;
					}

					if (invariantTagName == "script")
					{
						yield return ReadScript(reader, "</" + tagName); //todo: revise concatination
					}
					else if (invariantTagName == "textarea")
					{
						var text = new StringBuilder();
						ReadToPhrase(reader, text, "</textarea>");
						yield return HtmlChunk.Text(text.ToString());
					}
					else
					{
						foreach (var chunk in Read(reader, tagName.ToLowerInvariant()))
							yield return chunk;
					}

					yield return new HtmlChunk {Value = tagName, Type = HtmlChunkTypes.TagEnd};
					yield break;
				}
				

				//Read attribute
				SkipWhiteSpace(reader);

				var attrName = ReadAttributeName(reader);
				if (!string.IsNullOrEmpty(attrName))
				{
					yield return new HtmlChunk {Type = HtmlChunkTypes.AttributeName, Value = attrName};
					var s = SkipWhiteSpace(reader);
					if (s == '=')
					{
						reader.Read();
						var q = SkipWhiteSpace(reader);

						var val = (q == '\'' || q == '\"') ? ReadAttributeValue(reader) : ReadUnquotedAttributeValue(reader);
						if (!string.IsNullOrEmpty(val))
							yield return new HtmlChunk {Type = HtmlChunkTypes.AttributeValue, Value = val};
					}
				}
			}
		}

		private static char SkipWhiteSpace(StreamReader reader)
		{
			char s = '\0';
			while (!reader.EndOfStream)
			{
				s = (char)reader.Peek();
				if(s!=' ')
					break;
				reader.Read();
			}
			return s;
		}

		private static string ReadAttributeName(StreamReader reader)
		{
			var buffer = new StringBuilder();

			var escaped = false;
			do
			{
				var symbol = (char)reader.Peek();

				if (!escaped)
				{
					if (symbol == '/' || symbol == ' ' || symbol == '>' || symbol == '=')
						break;

					if (symbol == '&')
					{
						reader.Read();
						buffer.Append(ReadSpecial(reader));
					}

					if (symbol == '\\')
					{
						escaped = true;
						continue;
					}
				}

				escaped = false;

				buffer.Append((char)reader.Read());
			} while (!reader.EndOfStream);

			return buffer.ToString();
		}
	}

	public class LookingStreamReader
	{
		private readonly StreamReader _reader;
		readonly Queue<char> _queue = new Queue<char>(); 

		public LookingStreamReader(StreamReader reader)
		{
			_reader = reader;
		}

		public bool EndOfStream {get { return _reader.EndOfStream && _queue.Count == 0; }}

		public char Read()
		{
			return _queue.Count > 0 ? _queue.Dequeue() : (char)_reader.Read();
		}

		public char Peek()
		{
			return _queue.Count > 0 ? _queue.Peek() : (char)_reader.Peek();
		}

		public char Look(int offset)
		{
			if(offset == 1)
				return Peek();

			if (offset < _queue.Count)
			{
				return _queue.Skip(offset - 1).First();
			}
				
			var res = (char)_reader.Peek();
			var dif = offset - _queue.Count;
			for (var i = 0; i < dif; i++)
			{
				res = (char)_reader.Read();
				_queue.Enqueue(res);
			}
			return res;
		}
	}

	public static class LookingStreamReaderExtension
	{
		public static string ReadLineWithEndings(this StreamReader streamReader)
		{
			var buffer = new List<char>();

			while (!streamReader.EndOfStream)
			{
				var s = (char)streamReader.Read();
				buffer.Add(s);
				if (s == '\r' && (streamReader.EndOfStream || streamReader.Peek() == '\n'))
				{
					s = (char)streamReader.Read();
					buffer.Add(s);
				}

				if (s == '\n')
					break;
			}

			return new string(buffer.ToArray());
		}

	}
}

