using System.IO;
using Knyaz.Optimus.Tools;

namespace Knyaz.Optimus.ScriptExecuting
{
	public interface IRawJson
	{
		string JsonString { get; }
	}
	
	internal class JsonObject : IRawJson
	{
		public string JsonString { get; private set; }

		public static JsonObject FromStream(Stream stream) => new JsonObject {JsonString = stream.ReadToEnd()};
	}
}