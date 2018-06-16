using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Jint.Native;
using Jint.Native.Array;
using Jint.Runtime.Descriptors;
using Jint.Runtime.Descriptors.Specialized;
using Jint.Runtime.Interop;
using Knyaz.Optimus.Environment;

namespace Knyaz.Optimus.ScriptExecuting
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

			if (value is Window)
			{
				result = engine.Global;
				return true;
			}

			if (value != null && _cache.TryGetValue(value, out result))
				return true;

			if (value is IList list)
			{
				result = new ListAdapterEx(engine, list);
				_cache[value] = result;
				return true;
			}

			if (value != null)
			{
				if (_bindedTypes.Any(x => x.IsInstanceOfType(value)))
				{
					result = new ClrObject(engine, value, this);
					_cache[value] = result;
					return true;
				}
			}

			switch (value)
			{
				case string stringValue:
					result = new JsValue(stringValue);
					return true;
				case double d:
					result = new JsValue(d);
					return true;
				case bool b:
					result = new JsValue(b);
					return true;
				case JsValue js: result = js;
					return true;
			}

			result = JsValue.Null;
			return false;
		}

		protected Jint.Engine Engine => _getEngine();

		public Action<T> ConvertDelegate<T>(JsValue @this, JsValue jsFunc)
		{
			var callable = jsFunc.AsObject() as ICallable;
			Action<T> handler = null;
			if (callable != null)
			{
				handler = (Action<T>)_delegatesCache.GetValue(callable, key => (Action<T>)(e =>
				{
					JsValue val;
					TryConvert(e, out val);
					key.Call(@this, new[] { val });
				}));
			}
			return handler;
		}
		
		/// <summary>
		/// Converts js function to c# delegate with convertion array to arguments
		/// </summary>
		public Action<object[]> ConvertDelegateArrayArguments(JsValue @this, JsValue jsFunc)
		{
			var callable = jsFunc.AsObject() as ICallable;
			Action<object[]> handler = null;
			if (callable != null)
			{
				handler = (Action<object[]>)_delegatesCache.GetValue(callable, key => 
					(Action<object[]>)(e =>
				{
					var args = e == null ? new JsValue[0] 
						: e.Select(x => { TryConvert(x, out var val); return val;}).ToArray();
					
					key.Call(@this, args);
				}));
			}
			return handler;
		}
		
		

		readonly ConditionalWeakTable<ICallable, object> _delegatesCache = new ConditionalWeakTable<ICallable, object>();
	}

	internal class DomItemAttribute : Attribute
	{
	}

	/// <summary>
	/// For of jint's list adapter
	/// </summary>
	internal class ListAdapterEx : ArrayInstance, IObjectWrapper
	{
		private readonly Jint.Engine _engine;
		private readonly IList _list;

		public ListAdapterEx(Jint.Engine engine, IList list)
			: base(engine)
		{
			_engine = engine;
			_list = list;
			Prototype = engine.Array;
		}

		public override void Put(string propertyName, JsValue value, bool throwOnError)
		{
			int index;
			if (int.TryParse(propertyName, out index))
			{
				//todo: resize the list if index is greater then count

				if (_list.Count > index)
					_list[index] = value.ToObject();
			}

			//base.Put(propertyName, value, throwOnError);
		}

		public override JsValue Get(string propertyName)
		{
			int index;
			if (int.TryParse(propertyName, out index))
			{
				return _list.Count > index ? JsValue.FromObject(_engine, _list[index]) : JsValue.Undefined;
			}

			return base.Get(propertyName);
		}

		public override PropertyDescriptor GetOwnProperty(string propertyName)
		{
			if (Properties.ContainsKey(propertyName))
				return Properties[propertyName];

			if (propertyName == "length")
			{
				var p = new PropertyDescriptor(
					new ClrFunctionInstance(_engine, (value, values) => _list.Count),
					new ClrFunctionInstance(_engine, (value, values) =>
					{
						//todo: resize list
						return value;
					}));

				Properties.Add(propertyName, p);
			}

			var index = 0u;
			if (uint.TryParse(propertyName, out index))
			{
				return new IndexDescriptor(Engine, propertyName, Target);
			}

			return base.GetOwnProperty(propertyName);
		}

		public object Target { get { return _list; } }
	}
}