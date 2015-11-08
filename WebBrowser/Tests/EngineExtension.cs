namespace WebBrowser.Tests
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

			engine.Console.OnLog += o => System.Console.WriteLine(o ?? "<null>");
			return engine;
		}
	}
}