using System;
using System.Collections.Generic;
using System.Linq;
using Jint.Native;
using Jint.Native.Function;
using Jint.Native.Object;

namespace Knyaz.Optimus.Scripting.Jint.Internal
{
	using Engine = global::Jint.Engine;

	internal class ClrCtor : FunctionInstance, IConstructor
	{
		private readonly Type _type;
		private readonly DomConverter _converter;

		public ClrCtor(Engine engine,  DomConverter converter, Type type) : 
			base(engine, null, null, false)
		{
			_type = type;
			_converter = converter;
			Prototype = engine.Function.PrototypeObject;
			FastAddProperty("prototype", _converter.GetPrototype(type), false, false, false);
			
			DomConverter.DefineStatic(this, type);
		}

		public override JsValue Call(JsValue thisObject, JsValue[] arguments)
		{
			throw new NotImplementedException();
		}

		public ObjectInstance Construct(JsValue[] arguments)
		{
			var argsValues = arguments.Select(x => x.ToObject()).ToArray();
			var argTypes = argsValues.Select(x => x?.GetType()).ToArray();

			if (argTypes.All(x => x != null))
			{
				//todo: why we here do not onvert args?
				var exactCtor = _type.GetConstructor(argTypes);
				if (exactCtor != null)
				{
					var obj = exactCtor.Invoke(argsValues);
					JsValue val;
					if (_converter.TryConvert(obj, out val))
						return val.AsObject() as ClrObject;
				}
			}

			foreach (var ctor in _type.GetConstructors())
			{
				var ctorParameters = ctor.GetParameters();

				if (ctorParameters.Length == argTypes.Length)
				{
					var notMatch = false;
					for (var i = 0; i < ctorParameters.Length && !notMatch; i++)
					{
						if (!DomConverter.Convertible(argTypes[i], ctorParameters[i].ParameterType))
							notMatch = true;
					}

					if (!notMatch)
					{
						var args = ConvertArgs(argsValues, ctorParameters.Select(x => x.ParameterType))
							.ToArray();
						
						var obj = ctor.Invoke(args);
						return new ClrObject(Engine, obj, _converter.GetPrototype(obj.GetType()), _converter);
					}
				} 
			}

			throw new Exception("Unable to find proper constructor for the type: " + _type.Name);
		}

		private IEnumerable<object> ConvertArgs(object[] argsValues, IEnumerable<Type> types)
		{
			//todo: perhaps we have to use Engine.ClrTypeConverter
			var idx = 0;
			foreach (var type in types)
			{
				var val = argsValues[idx];

				if (val is double doubleVal)
				{
					if (type == typeof(int))
						val = Convert.ToInt32(doubleVal);
					
					if(type == typeof(float))
						val = (float) doubleVal;

					if (type == typeof(ulong))
						val = Convert.ToUInt64(doubleVal);
				}
				
				yield return val;
				idx++;
			}
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