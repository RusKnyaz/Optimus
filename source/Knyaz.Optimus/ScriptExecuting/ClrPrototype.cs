using System;
using Jint.Native;
using Jint.Native.Function;
using Jint.Native.Object;
using System.Linq;

namespace Knyaz.Optimus.ScriptExecuting
{
	public class ClrPrototype : FunctionInstance, IConstructor
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

		public ObjectInstance Construct(JsValue[] arguments)
		{
			var argsValues = arguments.Select(x => x.ToObject()).ToArray();
			var argTypes = argsValues.Select(x => x.GetType()).ToArray();
			var ctor = _type.GetConstructor(argTypes);
			if(ctor != null)
			{
				var obj = ctor.Invoke(argsValues);
				return new ClrObject(Engine, obj);
			}

			throw new Exception("Unable to find proper constructor for the type: " + _type.Name);
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