using System;
using System.Collections;
using System.Linq;
using Jint.Native;
using Jint.Native.Array;
using Jint.Runtime.Descriptors;
using Jint.Runtime.Descriptors.Specialized;
using Jint.Runtime.Interop;

namespace Knyaz.Optimus.ScriptExecuting
{
	using Engine = global::Jint.Engine;
	
	/// <summary>
	/// List to JS Array adapter
	/// </summary>
	internal class ListAdapterEx : ArrayInstance, IObjectWrapper
	{
		private readonly Engine _engine;
		private readonly IList _list;

		public ListAdapterEx(Engine engine, IList list)
			: base(engine)
		{
			_engine = engine;
			_list = list is Array ? list.Cast<object>().ToList() : list;
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

			if (Target is Array)
			{
				return base.GetOwnProperty(propertyName);
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