using System;
using System.Reflection;
using Jint.Native.Object;

namespace Knyaz.Optimus.ScriptExecuting.Jint
{
	internal class ClrPrototype : ObjectInstance
	{
		private readonly string _name;

		public ClrPrototype(global::Jint.Engine engine, DomConverter converter, Type type, ObjectInstance prototype) :
			base(engine)
		{
			Prototype = prototype;
			_name = (type.GetCustomAttribute<JsNameAttribute>()?.Name ?? type.Name) + "Prototype";
			Extensible = true;
			
			converter.DefineProperties(this, type);
		}

		public override string Class => _name;		
	}
}