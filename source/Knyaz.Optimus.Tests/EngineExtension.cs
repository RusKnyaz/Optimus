using System.Collections.Generic;
using System.IO;
using System.Text;

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
				engine.Document.DomContentLoaded += document => System.Console.WriteLine("DOMContentLoaded");

				engine.Scripting.BeforeScriptExecute += script => System.Console.WriteLine(
					"Executing:" + (
						!string.IsNullOrEmpty(script.Src) ? script.Src : 
						!string.IsNullOrEmpty(script.Id) ? script.Src : "<script>"));

				engine.Scripting.AfterScriptExecute += script => System.Console.WriteLine(
					"Executed:" + (!string.IsNullOrEmpty(script.Src) ? script.Src : 
						!string.IsNullOrEmpty(script.Id) ? script.Src : "<script>"));

				engine.Scripting.ScriptExecutionError += (script, exception) => System.Console.WriteLine(
					"Error script execution:" + (script.Src ?? script.Id ?? "<script>") + " " + exception.Message);
			};

			return engine;
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