using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Jint;
using Jint.Native;
using Jint.Native.Object;
using Jint.Runtime.Interop;
using Knyaz.Optimus.Environment;

namespace Knyaz.Optimus.ScriptExecuting
{
	internal class DomConverter : IObjectConverter
	{
		private List<Type> _bindedTypes = new List<Type>();
		private Dictionary<object, JsValue> _cache = new Dictionary<object, JsValue>();
		
		public DomConverter()
		{
			var domTypes = GetType().Assembly.GetTypes().Where(x => x.GetCustomAttributes<DomItemAttribute>().Any());
			foreach (var domType in domTypes)
			{
				_bindedTypes.Add(domType);
			}
		}
		
		/// <summary>
		/// Converts CLR objects to the JsValue.
		/// </summary>
		/// <param name="engine"></param>
		/// <param name="value"></param>
		/// <param name="result"></param>
		/// <returns></returns>
		public bool TryConvert(Jint.Engine engine, object value, out JsValue result)
		{
			if (engine == null)
			{
				result = JsValue.Undefined;
				return false;
			}

			if (value is Undefined)
			{
				result = JsValue.Undefined;
				return true;
			}

			if (value is Window)
			{
				result = engine.Global;
				return true;
			}

			if (value == null)
			{
				result = JsValue.Null;
				return true;
			}

			if (_cache.TryGetValue(value, out result))
				return true;

			if (value is IList list)
			{
				result = new ListAdapter(engine, list);
				_cache[value] = result;
				return true;
			}

			if (_bindedTypes.Any(x => x.IsInstanceOfType(value)))
			{
				result = new ClrObject(engine, value, this);
				_cache[value] = result;
				return true;
			}

			result = JsValue.Null;
			return false;
		}

		/// <summary>
        /// Converts js function to c# delegate with conversion array of arguments.
        /// </summary>
		public Action<object[]> ConvertDelegate(Jint.Engine engine, JsValue @this, ICallable callable)
		{
			Action<object[]> handler = null;
			if (callable != null)
			{
				handler = (Action<object[]>) _delegatesCache.GetValue(callable, key =>
					(Action<object[]>) (e =>
					{
						var args = e == null
							? new JsValue[0]
							: e.Select(x => JsValue.FromObject(engine, x)).ToArray();

						key.Call(@this, args);
					}));
			}

			return handler;
		}


		readonly ConditionalWeakTable<ICallable, object> _delegatesCache = new ConditionalWeakTable<ICallable, object>();

		public object ConvertFromJs(JsValue jsValue)
		{
			if (jsValue.IsUndefined())
				return Undefined.Instance;
			
			if (jsValue.IsObject())
			{
				switch (jsValue.AsObject())
				{
					case ClrObject clr:
						return clr.Target;
					case ObjectInstance objInst:
						return objInst.GetOwnProperties().ToDictionary(x => x.Key, x => 
							ConvertFromJs(objInst.Get(x.Key)));
				}

				return jsValue;
			}
			
			if (jsValue.IsBoolean())
				return jsValue.AsBoolean();

			if (jsValue.IsString())
				return jsValue.AsString();

			if (jsValue.IsNumber())
				return jsValue.AsNumber();

			return null;
		}
	}

	internal class DomItemAttribute : Attribute
	{
	}
}