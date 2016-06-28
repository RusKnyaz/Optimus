using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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

		public static HtmlChunk AttributeValue(string val)
		{
			return new HtmlChunk {Type = HtmlChunkTypes.AttributeValue, Value = WebUtility.HtmlDecode(val)};

		}

		internal static HtmlChunk TagStart(List<char> buffer)
		{
			return new HtmlChunk { Type = HtmlChunkTypes.TagStart, Value = new string(buffer.ToArray()) };
		}

		internal static HtmlChunk Text(List<char> buffer)
		{
			return new HtmlChunk { Type = HtmlChunkTypes.Text, Value = WebUtility.HtmlDecode(new string(buffer.ToArray())).Replace("\u000D", "\u000A") };
		}

		internal static HtmlChunk Text(string buffer)
		{
			return new HtmlChunk { Type = HtmlChunkTypes.Text, Value = WebUtility.HtmlDecode(buffer).Replace("\u000D", "\u000A") };
		}

		internal static HtmlChunk Comment(string res)
		{
			return res.ToLowerInvariant().StartsWith("doctype")
				? new HtmlChunk { Type = HtmlChunkTypes.DocType, Value = res.Remove(0, 7).TrimStart() }
				: new HtmlChunk { Type = HtmlChunkTypes.Comment, Value = res };
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

	public class HtmlReader
	{
		static string[] _noContentTags = { "meta", "br", "link" };

		private static HtmlChunk ReadScript(StreamReader reader, string endWord)
		{
			var buffer = new List<char>();
			if (reader.EndOfStream)
				return new HtmlChunk {Type = HtmlChunkTypes.Text, Value = string.Empty};

			do
			{
				var symbol = (char)reader.Read();

				if (symbol == '"' || symbol == '\'')
				{
					buffer.Add(symbol);
					var txt = ReadScriptString(reader, x => x != symbol);
					buffer.AddRange(txt);
					buffer.Add((char)reader.Read());
				}
				else if (symbol == '>' && IsEndsWith(buffer, endWord.ToLowerInvariant()))
				{
					buffer.RemoveRange(buffer.Count - endWord.Length, endWord.Length);
					return new HtmlChunk {Type = HtmlChunkTypes.Text, Value = new string(buffer.ToArray())};
				}
				else if (symbol == '/')
				{
					if (reader.EndOfStream)
						break;

					var next = reader.Peek();
					var nextChar = (char) next;
					if (nextChar == '/') // Line commet - //
					{
						buffer.Add(symbol);
						buffer.Add((char) reader.Read());
						buffer.AddRange(reader.ReadLineWithEndings());
					}
					else if (nextChar == '*') // Block comment - /* */
					{
						buffer.Add(symbol);
						buffer.Add((char) reader.Read());
						buffer.AddRange(ReadToPhrase(reader, "*/"));
						buffer.AddRange("*/");
					}
					else if (buffer.Last() != '<') //regexp
					{
						buffer.Add('/');
						buffer.AddRange(ReadScriptString(reader, x => x != '/'));
						buffer.Add((char) reader.Read());
					}
					else
					{
						buffer.Add(symbol);
					}
				}
				else
				{
					buffer.Add(symbol);
				}
			}
			while (!reader.EndOfStream);

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

		private static void ReadToCharWithChar(StreamReader reader, StringBuilder buffer, char end)
		{
			while(!reader.EndOfStream)
			{
				var symbol = (char)reader.Read();
				if (symbol == end)
					return;

				buffer.Append(symbol);
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

		private static IEnumerable<HtmlChunk> Read(StreamReader reader)
		{
			if(reader.EndOfStream)
				yield break;

			var text = new StringBuilder();

			var tagsStack = new Stack<string>();

			while (!reader.EndOfStream)
			{
				text.Append(ReadWhile(reader, x => x != '<'));
				if (reader.EndOfStream) break;

				reader.Read();
				var next = (char) reader.Peek();

				if (tagsStack.Count > 0 && next == '/') //probably end of tag
				{
					var endTag = tagsStack.Peek();
					reader.Read();
					var closedTagName = ReadWhile(reader, x => x != '>');
					if (closedTagName.Equals(endTag, StringComparison.InvariantCultureIgnoreCase))
					{
						if (text.Length > 0)
						{
							yield return HtmlChunk.Text(text.ToString());
							text.Clear();
						}

						yield return new HtmlChunk {Type = HtmlChunkTypes.TagEnd, Value = closedTagName};

						reader.Read();
						tagsStack.Pop();
						continue;
					}

					text.Append("</");
					text.Append(closedTagName);
				}
				else if (char.IsLetter(next)) //start of tag
				{
					if (text.Length > 0)
					{
						yield return HtmlChunk.Text(text.ToString());
						text.Clear();
					}

					var tagName = ReadWhile(reader, c => c != '/' && c != ' ' && c != '>');
					yield return new HtmlChunk {Value = tagName, Type = HtmlChunkTypes.TagStart};

					while (!reader.EndOfStream)
					{
						var sym = (char) reader.Peek();
						if (sym == '/') //may be self-closed tag
						{
							reader.Read();
							if (reader.Peek() == '>')
							{
								reader.Read();
								yield return new HtmlChunk {Value = tagName, Type = HtmlChunkTypes.TagEnd};
								break;
							}
						}
						else if (sym == '>') //end of tag
						{
							reader.Read();
							var invariantTagName = tagName.ToLowerInvariant();
							if (_noContentTags.Contains(invariantTagName))
							{
								yield return new HtmlChunk {Value = tagName, Type = HtmlChunkTypes.TagEnd};
							}
							else if (invariantTagName == "script" || invariantTagName == "style")
							{
								yield return ReadScript(reader, "</" + tagName); //todo: revise concatination
								yield return new HtmlChunk { Value = tagName, Type = HtmlChunkTypes.TagEnd };
							}
							else if (invariantTagName == "textarea")
							{
								yield return HtmlChunk.Text(ReadToPhrase(reader, "</textarea>"));
								yield return new HtmlChunk { Value = tagName, Type = HtmlChunkTypes.TagEnd };
							}
							else
							{
								tagsStack.Push(tagName.ToLowerInvariant());
							}
							break;
						}

						//Read attribute
						SkipWhiteSpace(reader);

						var attrName = ReadWhile(reader, c => c != '/' && c != ' ' && c != '>' && c != '=');
						if (!string.IsNullOrEmpty(attrName))
						{
							yield return new HtmlChunk {Type = HtmlChunkTypes.AttributeName, Value = attrName};
							if (SkipWhiteSpace(reader) == '=')
							{
								reader.Read();
								SkipWhiteSpace(reader);
								var val = ReadAttributeValue(reader);
								if (!string.IsNullOrEmpty(val))
									yield return HtmlChunk.AttributeValue(val);
							}
						}
					}
				}
				else if (next == '?')
				{
					yield return new HtmlChunk {Type = HtmlChunkTypes.Comment, Value = ReadComment(reader, false)};
				}
				else if (next == '!') //comment
				{
					reader.Read();
					yield return HtmlChunk.Comment(ReadComment(reader, true));
				}
			}

			if (text.Length > 0)
				yield return HtmlChunk.Text(text.ToString());
		}

		private static string ReadAttributeValue(StreamReader reader)
		{
			var q = (char)reader.Read();
			if (q == '\'' || q == '\"')
			{
				var res = ReadWhileWithEscapes(reader, "\\\"'", c => c != q);
				reader.Read();
				return res;
			}

			return  q + ReadWhileWithEscapes(reader, "\"'", c => c != '/' && c != ' ' && c != '>');
		}

		private static string ReadToPhrase(StreamReader reader, string end)
		{
			var textAreaContent = new StringBuilder();
			ReadToPhrase(reader, textAreaContent, end);
			return textAreaContent.ToString();
		}

		private static char SkipWhiteSpace(StreamReader reader)
		{
			ReadWhile(reader, c => c == ' ');
			return (char)reader.Peek();
		}

		private static string ReadScriptString(StreamReader reader, Func<char, bool> condition)
		{
			var buffer = new StringBuilder();

			while (!reader.EndOfStream)
			{
				var symbol = (char) reader.Peek();

				if (symbol == '\\')
				{
					buffer.Append((char)reader.Read());
					buffer.Append((char) reader.Read());
					continue;
				}

				if (!condition(symbol))
					break;

				reader.Read();

				buffer.Append(symbol);
			}

			return buffer.ToString();
		}


		private static string ReadWhile(StreamReader reader, Func<char, bool> condition)
		{
			var buffer = new StringBuilder();
			while (!reader.EndOfStream)
			{
				var symbol = (char) reader.Peek();

				if (!condition(symbol))
					break;

				reader.Read();
				buffer.Append(symbol);
			}
			return buffer.ToString();
		}

		private static string ReadWhileWithEscapes(StreamReader reader, string escapes, Func<char, bool> condition)
		{
			var buffer = new StringBuilder();

			while (!reader.EndOfStream)
			{
				var symbol = (char)reader.Peek();

				if (symbol == '\\')
				{
					reader.Read();
					symbol = (char) reader.Read();
					if (escapes.IndexOf(symbol) < 0)
					{
						buffer.Append('\\');
						if (!condition(symbol))
							break;
					}

					buffer.Append(symbol);
					continue;
				}

				if (!condition(symbol))
					break;

				reader.Read();
				buffer.Append(symbol);
			}

			return buffer.ToString();
		}
	}

	
	public static class StreamReaderExtension
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

