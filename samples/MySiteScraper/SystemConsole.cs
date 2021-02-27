using System;
using System.Linq;
using System.Text;
using Knyaz.Optimus.Dom.Interfaces;

namespace MySiteScraper
{
	/// <summary>
	/// Translates the browser console output to the System.Console
	/// </summary>
	internal class SystemConsole : IConsole
	{
		private int _indent = 0;
		
		public void Assert(bool assertion, params object[] objs)
		{
			if (!assertion)
				Write(objs);
		}

		private void Write(object[] objs)
		{
			if (objs.Length == 0)
				return;

			if (objs.Length == 1)
			{
				System.Console.WriteLine(objs[0]?.ToString()?? "null");
				return;
			}
			
			System.Console.Write(new string(Enumerable.Repeat(' ', _indent*2).ToArray()));
			System.Console.Write("[");
			System.Console.Write(string.Join(", ", objs.Select(x => x?.ToString()?? "null")));
			System.Console.WriteLine("]");
		}

		private void WriteFormat(string format, object[] objs)
		{
			if(string.IsNullOrEmpty(format))
				return;
			
			var builder = new StringBuilder();
			var parts = format.Split('%');
			builder.Append(parts[0]);
			for (var idx = 0; idx < parts.Length - 1; idx++)
			{
				if (idx < objs.Length)
				{
					builder.Append(objs[idx]);
				}

				builder.Append(parts[idx + 1]);
			}
			System.Console.WriteLine(builder.ToString());			
		}

		public void Assert(bool assertion, string format, params object[] objs)
		{
			if (!assertion)
				WriteFormat(format, objs);
		}

		public void Clear()
		{
			System.Console.Clear();
		}

		public void Error(params object[] objs)
		{
			System.Console.ForegroundColor = ConsoleColor.Red;
			Write(objs);
			System.Console.ResetColor();
		}

		public void Error(string format, params object[] objs)
		{
			System.Console.ForegroundColor = ConsoleColor.Red;
			WriteFormat(format, objs);
			System.Console.ResetColor();
		}

		public void Group()
		{
			_indent++;
		}

		public void Group(string label)
		{
			_indent++;
		}

		public void GroupCollapsed()
		{
		}

		public void GroupEnd()
		{
			_indent--;
		}

		public void Info(params object[] objs)
		{
			System.Console.ForegroundColor = ConsoleColor.Blue;
			Write(objs);
			System.Console.ResetColor();
		}

		public void Info(string format, params object[] objs)
		{
			System.Console.ForegroundColor = ConsoleColor.Blue;
			WriteFormat(format, objs);
			System.Console.ResetColor();
		}

		public void Log(string format, params object[] objs)
		{
			WriteFormat(format, objs);
		}

		public void Log(params object[] objs)
		{
			Write(objs);
		}

		public void Time(string label)
		{
		}

		public void TimeEnd(string label)
		{
		}

		public void Warn(params object[] objs)
		{
			System.Console.ForegroundColor = ConsoleColor.Yellow;
			Write(objs);
			System.Console.ResetColor();
		}

		public void Warn(string format, params object[] objs)
		{
			System.Console.ForegroundColor = ConsoleColor.Yellow;
			WriteFormat(format, objs);
			System.Console.ResetColor();
		}
	}
}