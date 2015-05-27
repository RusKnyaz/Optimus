using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Jint.Native;
using Jint.Runtime.Interop;

namespace WebBrowser.ScriptExecuting
{
	internal class DomConverter : IObjectConverter
	{
		private Func<Jint.Engine> _getEngine;

		private List<Type> _bindedTypes = new List<Type>();
		private Dictionary<object, JsValue> _cache = new Dictionary<object, JsValue>();
		
		public DomConverter(Func<Jint.Engine> getEngine)
		{
			_getEngine = getEngine;

			var domTypes = GetType().Assembly.GetTypes().Where(x => x.GetCustomAttributes<DomItemAttribute>().Any());
			foreach (var domType in domTypes)
			{
				_bindedTypes.Add(domType);
			}
		}
		
		public bool TryConvert(object value, out JsValue result)
		{
			var engine = Engine;
			if (engine == null)
			{
				result = JsValue.Undefined;
				return false;
			}

			if (value != null && _cache.TryGetValue(value, out result))
				return true;

			var list = value as IList;
			if (list != null)
			{
				result = new ListAdapter(engine, list);
				_cache[value] = result;
				return true;
			}

			if (value != null)
			{
				if (_bindedTypes.Any(x => x.IsInstanceOfType(value)))
				{
					result = new ClrObject(engine, value);
					_cache[value] = result;
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