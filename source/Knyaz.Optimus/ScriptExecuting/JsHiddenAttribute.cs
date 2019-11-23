using System;

namespace Knyaz.Optimus.ScriptExecuting
{
	public class JsHiddenAttribute : Attribute{}
	
	/// <summary>
	/// Tells that the object[] argument of callback function have to be expanded to arguments list in js handler.
	/// </summary>
	internal class JsExpandArrayAttribute : Attribute{}
}