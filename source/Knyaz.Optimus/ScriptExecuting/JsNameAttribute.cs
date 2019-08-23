using System;

namespace Knyaz.Optimus.ScriptExecuting
{
	/// <summary>
	/// Allows to specify the name for the method which will available in JavaScript code.
	/// </summary>
    class JsNameAttribute : Attribute
    {
		public readonly string Name;
		public JsNameAttribute(string name) => Name = name;
    }
}
