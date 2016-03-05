using System;
using Jint.Native;
using Jint.Native.Function;

namespace Knyaz.Optimus.ScriptExecuting
{
	public class ClrPrototype : FunctionInstance
	{
		private readonly Type _type;

		public ClrPrototype(Jint.Engine engine, Type type) : 
			base(engine, null, null, false)
		{
			_type = type;
			Prototype = engine.Object;
		}

		public override JsValue Call(JsValue thisObject, JsValue[] arguments)
		{
			throw new NotImplementedException();
		}

		public override bool HasInstance(JsValue v)
		{
			var clrObject = v.TryCast<ClrObject>();
			if (clrObject != null && clrObject.Target != null)
			{
				return _type.IsInstanceOfType(clrObject.Target);
			}

			return base.HasInstance(v);
		}
	}
}