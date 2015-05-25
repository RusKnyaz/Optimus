using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Jint.Native;
using Jint.Native.Object;
using Jint.Runtime.Descriptors;
using Jint.Runtime.Interop;

namespace WebBrowser.ScriptExecuting
{
	internal class DomConverter : IObjectConverter
	{
		private Func<Jint.Engine> _getEngine;

		private Dictionary<Type, Action<ObjectInstance, object>> _bindedTypes = new Dictionary<Type, Action<ObjectInstance, object>>();
		
		public DomConverter(Func<Jint.Engine> getEngine)
		{
			_getEngine = getEngine;

			var domTypes = GetType().Assembly.GetTypes().Where(x => x.GetCustomAttributes<DomItemAttribute>().Any());
			foreach (var domType in domTypes)
			{
				_bindedTypes[domType] = CreateBinder(domType);
			}
		}

		private Action<ObjectInstance, object> CreateBinder(Type type)
		{
			//todo: rewrite with precompiled expression
			return (jsObj, netObj) =>
				{
					foreach (var prop in type.GetProperties())
					{
						var getMethod = prop.GetGetMethod();

						var getter = getMethod.IsPublic
							? new ClrFunctionInstance(
								Engine, (jsValue, values) =>
									{
										var netThis = ToClr(jsValue);
										return JsValue.FromObject(Engine, getMethod.Invoke(netThis, new object[0]));
									})
							: JsValue.Undefined;

						var setter = JsValue.Undefined; //todo

						var propertyName = Char.ToLower(prop.Name[0]) + prop.Name.Substring(1);

						jsObj.DefineOwnProperty(propertyName, new PropertyDescriptor(getter, setter), true);
					}

					foreach (var methodInfo in type.GetMethods())
					{
						var getter = new ClrFunctionInstance(
							Engine, (jsValue, values) => new JsValue(
								new ClrFunctionInstance(Engine, (jThis, args) =>
									JsValue.FromObject(Engine, methodInfo.Invoke(ToClr(jThis), args.Select(ToClr).ToArray())))));

						var propertyName = Char.ToLower(methodInfo.Name[0]) + methodInfo.Name.Substring(1);
						jsObj.DefineOwnProperty(propertyName, new PropertyDescriptor(getter, JsValue.Undefined), true);
					}

					//todo: setters
					//todo: events
				};
		}

		private object ToClr(JsValue jsValue)
		{
			return _map.ContainsKey(jsValue) ? _map[jsValue] : jsValue.ToObject();
		}

		
		private Dictionary<object, JsValue> _cache = new Dictionary<object, JsValue>();
		private Dictionary<JsValue, object> _map = new Dictionary<JsValue, object>();

		public bool TryConvert(object value, out JsValue result)
		{
			if (Engine == null)
			{
				result = JsValue.Undefined;
				return false;
			}

			if (value != null && _cache.TryGetValue(value, out result))
				return true;

			if (value != null)
			{
				var obj = new ObjectInstance(Engine){Extensible = true, Prototype = Engine.Object};

				var binded = false;

				var type = value.GetType();
				foreach (var bindedType in _bindedTypes.Where(bindedType => bindedType.Key.IsAssignableFrom(type)))
				{
					bindedType.Value(obj, value);
					binded = true;
				}
				
				if (binded)
				{
					result = new JsValue(obj);
					_cache[value] = result;
					_map[result] = value;
					return true;
				}
			}

		
			result = JsValue.Null;
			return false;
		}

		protected Jint.Engine Engine
		{
			get { return _getEngine(); }
		}
	}

	internal class DomItemAttribute : Attribute
	{
	}
}