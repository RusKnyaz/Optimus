namespace Knyaz.Optimus.Tests
{
	internal static class EngineExtension
	{
		/// <summary> Writes scripting events to the console. </summary>
		/// <param name="engine"></param>
		public static Engine LogEvents(this Engine engine)
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
	}
}