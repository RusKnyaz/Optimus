using System;
using Jint.Native.Object;

namespace Knyaz.Optimus.Scripting.Jint.Internal
{
	internal class ClrPrototype : ObjectInstance
	{
		public ClrPrototype(global::Jint.Engine engine, DomConverter converter, Type type, ObjectInstance prototype) :
			base(engine)
		{
			Prototype = prototype;
			Class = type.GetJsName();
			Extensible = true;
			converter.DefineProperties(this, type);
		}
		

		public override string Class { get; }
	}
}