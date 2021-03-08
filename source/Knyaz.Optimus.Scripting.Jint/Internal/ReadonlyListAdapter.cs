using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Jint.Native;
using Jint.Native.Array;
using Jint.Runtime.Descriptors;
using Jint.Runtime.Descriptors.Specialized;
using Jint.Runtime.Interop;
using Knyaz.Optimus.ScriptExecuting;

namespace Knyaz.Optimus.Scripting.Jint.Internal
{
	/// <summary> Wrapper for the IReadOnlyList. Allows to apply Array's operations to the wrapped collection. </summary>
	/// <typeparam name="T">The type of the list item.</typeparam>
	internal class ReadonlyListAdapter<T> : ArrayInstance, IObjectWrapper
	{
		private readonly global::Jint.Engine _engine;
		private readonly IReadOnlyList<T> _list;
		private readonly PropertyInfo _indexPropertyStr;
		
		public ReadonlyListAdapter(global::Jint.Engine engine, IReadOnlyList<T> list, ClrPrototype prototype)
			: base(engine)
		{
			_engine = engine;
			_list = list;
			Prototype = prototype;
			_indexPropertyStr = 
				Target.GetType().GetRecursive(x => x.BaseType).SelectMany(x => 
						x.GetProperties().Where(p => p.GetIndexParameters().Length != 0 
						                             && p.GetCustomAttribute<JsHiddenAttribute>() == null
						                             && p.GetIndexParameters()[0].ParameterType == typeof(string)))
					.FirstOrDefault();
		}

		public override string Class => ((ClrPrototype)Prototype).Class;

		public override void Put(string propertyName, JsValue value, bool throwOnError)
		{
			if(throwOnError)
				throw new InvalidOperationException("Collection can not be modified.");
		}

		public override JsValue Get(string propertyName) =>
			int.TryParse(propertyName, out var index) 
				? _list.Count > index ? JsValue.FromObject(_engine, _list[index]) : JsValue.Undefined 
				: HasProperty(propertyName) ? base.Get(propertyName)  
				: _indexPropertyStr != null ? JsValue.FromObject(Engine, _indexPropertyStr.GetValue(Target, new object[] {propertyName})) 
				: JsValue.Undefined;

		public override PropertyDescriptor GetOwnProperty(string propertyName) => 
			uint.TryParse(propertyName, out _) ? new IndexDescriptor(Engine, propertyName, Target) 
				: base.GetOwnProperty(propertyName);

		public object Target => _list;
	}
}