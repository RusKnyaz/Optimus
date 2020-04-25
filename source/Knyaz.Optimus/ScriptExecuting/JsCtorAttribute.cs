using System;

namespace Knyaz.Optimus.ScriptExecuting
{
	/// <summary> Tells to the scripting engine that the method should be converted to the constructor function. </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public class JsCtorAttribute : Attribute
	{
		public readonly string Name; 

		public JsCtorAttribute(string name = null)
		{
			Name = name;
		}
	}
}