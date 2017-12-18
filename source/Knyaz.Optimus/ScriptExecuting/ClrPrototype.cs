using System;
using System.Collections.Generic;
using Jint.Native;
using Jint.Native.Function;
using Jint.Native.Object;
using System.Linq;

namespace Knyaz.Optimus.ScriptExecuting
{
	internal class ClrPrototype : FunctionInstance, IConstructor
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
			var exactCtor = _type.GetConstructor(argTypes);
			if(exactCtor  != null)
			{
				var obj = exactCtor.Invoke(argsValues);
				return new ClrObject(Engine, obj);
			}

			foreach (var ctor in _type.GetConstructors())
			{
				var ctorParameters = ctor.GetParameters();

				if (ctorParameters.Length == argTypes.Length)
				{
					var notMatch = false;
					for (var i = 0; i < ctorParameters.Length && !notMatch; i++)
					{
						if (!Convertible(argTypes[i], ctorParameters[i].ParameterType))
							notMatch = true;
					}

					if (!notMatch)
					{
						var args = ConvertArgs(argsValues, ctorParameters.Select(x => x.ParameterType))
							.ToArray();
						
						var obj = ctor.Invoke(args);
						return new ClrObject(Engine, obj);
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

				if (val is double)
				{
					if (type == typeof(int))
						val = Convert.ToInt32((double) val);
					
					if(type == typeof(float))
						val = (float) (double) val;
				}
				
				yield return val;
				idx++;
			}
		}

		private bool Convertible(Type valType, Type parType)
		{
			if (valType == typeof(double))
				return parType == typeof(int) ||
				       parType == typeof(float) ||
				       parType == typeof(uint);

			return valType == parType;
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