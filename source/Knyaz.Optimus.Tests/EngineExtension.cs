using System.Collections.Generic;
using System.IO;
using System.Text;
using Knyaz.Optimus.ResourceProviders;

namespace Knyaz.Optimus.Tests
{
	internal static class EngineExtension
	{
		/// <summary>
		/// Attach System.Console to log engine events;
		/// </summary>
		/// <param name="engine"></param>
		public static Engine AttachConsole(this Engine engine)
		{
			engine.DocumentChanged += () =>
			{
				engine.Document.DomContentLoaded += document => System.Console.Write("DOMContentLoaded");

				engine.Scripting.BeforeScriptExecute += script => System.Console.WriteLine(
					"Executing:" + (script.Src ?? script.Id ?? "<script>"));

				engine.Scripting.AfterScriptExecute += script => System.Console.WriteLine(
					"Executed:" + (script.Src ?? script.Id ?? "<script>"));

				engine.Scripting.ScriptExecutionError += (script, exception) => System.Console.WriteLine(
					"Error script execution:" + (script.Src ?? script.Id ?? "<script>") + " " + exception.Message);
			};

			((DocumentResourceProvider)engine.ResourceProvider).OnRequest += s => System.Console.WriteLine("Request: " + s);
			engine.Console.OnLog += o => System.Console.WriteLine(o ?? "<null>");
			return engine;
		}

		public static Console ToConsole(this Console console)
		{
			console.OnLog += o => System.Console.WriteLine(o == null ? "<null>" : o.ToString());
			return console;
		}

		public static List<object> ToList(this Console console)
		{
			var log = new List<object>();
			console.OnLog += x => log.Add(x);
			return log;
		}

		public static void Load(this Engine engine, string html)
		{
			using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(html)))
			{
				engine.Load(stream);
			}
		}
	}
}