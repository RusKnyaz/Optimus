using System.Collections;
using Jint;
using Jint.Native;
using Jint.Native.Array;
using Jint.Native.Object;
using Jint.Runtime.Descriptors;
using Jint.Runtime.Descriptors.Specialized;
using Jint.Runtime.Interop;

namespace Knyaz.Optimus.ScriptExecuting
{
	/// <summary>
	/// Fork of jint's list adapter
	/// </summary>
	internal class ListAdapter : ObjectInstance, IObjectWrapper
	{
		private readonly IList _list;

		public ListAdapter(Jint.Engine engine, IList list)
			: base(engine)
		{
			_list = list;
			Prototype = engine.Array;
		}

		public override void Put(in Key propertyName, JsValue value, bool throwOnError)
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

		public override JsValue Get(in Key propertyName)
		{
			int index;
			if (int.TryParse(propertyName, out index))
			{
				return _list.Count > index ? JsValue.FromObject(_engine, _list[index]) : JsValue.Undefined;
			}

			return base.Get(propertyName);
		}

		public override PropertyDescriptor GetOwnProperty(in Key propertyName)
		{
			var existProperty = base.GetOwnProperty(propertyName);
			if (existProperty != null && existProperty != PropertyDescriptor.Undefined)
				return existProperty;
			
			if (propertyName == "length")
			{
				var p = new GetSetPropertyDescriptor(
					new GetterFunctionInstance(_engine, (@this) => _list.Count),
					new SetterFunctionInstance(_engine, (@this, value) =>
					{
						//todo: resize list
					}));
				
				base.AddProperty(propertyName, p);
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