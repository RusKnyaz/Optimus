using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Jint.Native;
using Jint.Native.Function;
using Jint.Native.Object;
using Jint.Runtime;
using Jint.Runtime.Descriptors;
using Jint.Runtime.Descriptors.Specialized;
using Jint.Runtime.Interop;
using Knyaz.Optimus.Tools;

namespace Knyaz.Optimus.ScriptExecuting
{
	using Engine = global::Jint.Engine;
	
	/// <summary>
	/// Represents c# object (DOM and other) in JS environment.
	/// </summary>
	internal class ClrObject : ObjectInstance, IObjectWrapper
	{
		private readonly DomConverter _converter;
		private IDictionary<EventInfo, FunctionInstance> _attachedEvents;
		private readonly PropertyInfo _indexPropertyStr;
		private readonly PropertyInfo _indexPropertyInt;
		private readonly PropertyInfo _indexPropertyUlong;
		
		public Object Target { get; }

		public ClrObject(Engine engine, Object obj, ObjectInstance prototype, DomConverter converter)
			: base(engine)
		{
			_converter = converter;
			Target = obj;
			Prototype = prototype;
			Extensible = true;
			
			//todo: optimize reflection.
			_indexPropertyStr = 
				Target.GetType().GetRecursive(x => x.BaseType).SelectMany(x => 
					x.GetProperties().Where(p => p.GetIndexParameters().Length != 0 
					                             && p.GetCustomAttribute<JsHiddenAttribute>() == null
					                             && p.GetIndexParameters()[0].ParameterType == typeof(string)))
					.FirstOrDefault();
			
			_indexPropertyInt = 
				Target.GetType().GetRecursive(x => x.BaseType).SelectMany(x => 
						x.GetProperties().Where(p => p.GetIndexParameters().Length != 0 
						                             && p.GetCustomAttribute<JsHiddenAttribute>() == null
						                             && p.GetIndexParameters()[0].ParameterType == typeof(int)))
					.FirstOrDefault();
			
			_indexPropertyUlong = 
				Target.GetType().GetRecursive(x => x.BaseType).SelectMany(x => 
						x.GetProperties().Where(p => p.GetIndexParameters().Length != 0 
						                             && p.GetCustomAttribute<JsHiddenAttribute>() == null
						                             && p.GetIndexParameters()[0].ParameterType == typeof(ulong)))
					.FirstOrDefault();
		}

		public override string Class => Target.GetType().GetCustomAttribute<JsNameAttribute>()?.Name ?? Target.GetType().Name;

		public IDictionary<EventInfo, FunctionInstance> AttachedEvents => 
			_attachedEvents ?? (_attachedEvents = new Dictionary<EventInfo, FunctionInstance>());

		public override JsValue Get(string propertyName)
		{
			var property = GetProperty(propertyName);
			if (property == PropertyDescriptor.Undefined)
			{
				return _indexPropertyStr != null 
					? JsValue.FromObject(Engine, _indexPropertyStr.GetValue(Target, new object[] {propertyName})) 
					: _indexPropertyUlong != null && ulong.TryParse(propertyName, out var ulongIndex) ?
						JsValue.FromObject(Engine, _indexPropertyUlong.GetValue(Target, new object[] {ulongIndex}))
						: _indexPropertyInt != null && int.TryParse(propertyName, out var intIndex) ?
							JsValue.FromObject(Engine, _indexPropertyInt.GetValue(Target, new object[] {intIndex}))
						: JsValue.Undefined;
			}
				
			if (property.IsDataDescriptor())
			{
				var jsValue = property.Value;
				return jsValue == null ? Undefined.Instance : jsValue;
			}
			var jsValue1 = property.Get != (JsValue) null ? property.Get : Undefined.Instance;
			
			return jsValue1.IsUndefined() ? Undefined.Instance : jsValue1.TryCast<ICallable>().Call(this, new JsValue[0]);
		}

		public override void Put(string propertyName, JsValue value, bool throwOnError)
		{
			if (!CanPut(propertyName))
			{
				if (throwOnError)
					throw new JavaScriptException(Engine.TypeError);
			}
			else
			{
				var ownProperty = GetOwnProperty(propertyName);
				if (ownProperty.IsDataDescriptor())
				{
					ownProperty.Value = value;
				}
				else
				{
					PropertyDescriptor property = GetProperty(propertyName);
					if (property.IsAccessorDescriptor())
					{
						property.Set.TryCast<ICallable>().Call(new JsValue(this), new JsValue[1]
						{
							value
						});
					}
					else if (_indexPropertyStr != null)
					{
						_indexPropertyStr.SetValue(Target, value.ToObject(), new object[] {propertyName});
							
					}
					else if (_indexPropertyUlong != null && ulong.TryParse(propertyName, out var uLongIndex))
					{
						var val = _converter.ConvertToObject(value, _indexPropertyUlong.PropertyType);
						
						_indexPropertyUlong.SetValue(Target, val, new object[] {uLongIndex});
					}
					else if (_indexPropertyInt != null && int.TryParse(propertyName, out var intIndex))
					{
						_indexPropertyInt.SetValue(Target, DomConverter.ConvertToInt(value), new object[] {intIndex});
					}
					else
					{
						var desc = new PropertyDescriptor(value, true, true, true);
						DefineOwnProperty(propertyName, desc, throwOnError);
					}
				}
			}
		}
	}
}