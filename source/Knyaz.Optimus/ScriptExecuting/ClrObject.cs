using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Jint.Native;
using Jint.Native.Function;
using Jint.Native.Object;
using Jint.Runtime;
using Jint.Runtime.Descriptors;
using Jint.Runtime.Descriptors.Specialized;
using Jint.Runtime.Environments;
using Jint.Runtime.Interop;
using Knyaz.Optimus.Dom.Events;
using Knyaz.Optimus.Dom.Interfaces;

namespace Knyaz.Optimus.ScriptExecuting
{
	/// <summary>
	/// Represents c# object (DOM and other) in JS environment.
	/// </summary>
	internal class ClrObject : ObjectInstance, IObjectWrapper
	{
		private readonly DomConverter _converter;
		public Object Target { get; set; }

		public ClrObject(Jint.Engine engine, Object obj, DomConverter converter)
			: base(engine)
		{
			_converter = converter;
			Target = obj;
			//todo: use static prototypes
			Prototype = new ClrPrototype(engine, obj.GetType(), _converter);
			Extensible = true;
		}

		public override PropertyDescriptor GetOwnProperty(string propertyName)
		{
			//todo: check indexers (for example in CssStyleDeclaration)
			PropertyDescriptor x;
			if (Properties.TryGetValue(propertyName, out x))
				return x;

			var pascalCasedPropertyName = char.ToUpperInvariant(propertyName[0]).ToString();
			if (propertyName.Length > 1)
			{
				pascalCasedPropertyName += propertyName.Substring(1);
			}

			var type = Target.GetType();

			// look for a property
			var property = type.GetProperty(pascalCasedPropertyName, BindingFlags.Instance | BindingFlags.Public);
			if(property == null)
			{
				property = type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
					.FirstOrDefault(p => (p.GetCustomAttribute(typeof(JsNameAttribute)) as JsNameAttribute)?.Name == propertyName);
			}

			if (property != null)
			{
				var descriptor = new ClrPropertyInfoDescriptor(Engine, property, Target);
				Properties.Add(propertyName, descriptor);
				return descriptor;
			}

			// look for a field
			var field = type.GetField(pascalCasedPropertyName, BindingFlags.Instance | BindingFlags.Public);

			if (field != null)
			{
				var descriptor = new FieldInfoDescriptor(Engine, field, Target);
				Properties.Add(propertyName, descriptor);
				return descriptor;
			}

			// if no properties were found then look for a method 
			var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public)
			                  .Where(m => m.Name == propertyName)
			                  .ToArray();

			if (methods.Any())
			{
				return MethodDescriptor(propertyName, methods);
			}
			
			//look for methods with JsNameAttribute
			var namedMethods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public)
				.Where(m => (m.GetCustomAttribute(typeof(JsNameAttribute)) as JsNameAttribute)?.Name == propertyName)
										 .ToArray();
			if(namedMethods.Length > 0)
				return MethodDescriptor(propertyName, namedMethods);


			// look for methods using pascal cased name.
			var pascalCasedMethods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public)
			                             .Where(m => m.Name == pascalCasedPropertyName)
			                             .ToArray();

			if (pascalCasedMethods.Any())
			{
				return MethodDescriptor(propertyName, pascalCasedMethods);
			}

			// if no methods are found check if target implemented indexing
			if (type.GetProperties().Where(p => p.GetIndexParameters().Length != 0).FirstOrDefault() != null)
			{
				return new IndexDescriptor(Engine, propertyName, Target);
			}

			var interfaces = type.GetInterfaces();

			// try to find a single explicit property implementation
			var explicitProperties = (from iface in interfaces
				from iprop in iface.GetProperties()
				where propertyName.Equals(iprop.Name)
				select iprop).ToArray();

			if (explicitProperties.Length == 1)
			{
				var descriptor = new PropertyInfoDescriptor(Engine, explicitProperties[0], Target);
				Properties.Add(propertyName, descriptor);
				return descriptor;
			}

			// try to find explicit method implementations
			var explicitMethods = (from iface in interfaces
				from imethod in iface.GetMethods()
				where propertyName.Equals(imethod.Name)
				select imethod).ToArray();

			if (explicitMethods.Length > 0)
			{
				var descriptor = new PropertyDescriptor(new MethodInfoFunctionInstance(Engine, explicitMethods), false, true, false);
				Properties.Add(propertyName, descriptor);
				return descriptor;
			}

			// try to find explicit method implementations using the pascal cased property name
			var explicitPascalCasedMethods = (from iface in interfaces
				from imethod in iface.GetMethods()
				where pascalCasedPropertyName.Equals(imethod.Name)
				select imethod).ToArray();

			if (explicitPascalCasedMethods.Length > 0)
			{
				var descriptor = new PropertyDescriptor(new MethodInfoFunctionInstance(Engine, explicitPascalCasedMethods), false, true, false);
				Properties.Add(propertyName, descriptor);
				return descriptor;
			}

			// try to find explicit indexer implementations
			var explicitIndexers =
				(from iface in interfaces
					from iprop in iface.GetProperties()
					where iprop.GetIndexParameters().Length != 0
					select iprop).ToArray();

			if (explicitIndexers.Length == 1)
			{
				return new IndexDescriptor(Engine, explicitIndexers[0].DeclaringType, propertyName, Target);
			}

			//look for event
			foreach (var eventInfo in type.GetEvents())
			{
				if (eventInfo.Name.ToLower() != propertyName)
					continue;

				var pars = eventInfo.EventHandlerType.GetMethod("Invoke").GetParameters();

				//todo: optimize with expressions
				var settedHandler = new JsValue[1] { JsValue.Null };
				var listener0 = (Action) (() => settedHandler[0].Invoke(this, new JsValue[0]));
				var listener1 = (Action<object>)(p1 => settedHandler[0].Invoke(this, new JsValue[] {JsValue.FromObject(Engine, p1)}));
				var listener = pars.Length == 1 ? (Delegate)listener1 : listener0;

				var getter = new ClrFunctionInstance(Engine, (value, values) => settedHandler[0]);
				var info = eventInfo;
				var setter = new ClrFunctionInstance(Engine, (value, values) =>
				{
					if (settedHandler[0] != JsValue.Null)
						info.RemoveEventHandler(Target, listener);

					settedHandler[0] = values[0];
					if (settedHandler[0] != JsValue.Null)
						info.AddEventHandler(Target, listener);
					return values[0];
				});

				var descriptor =  new PropertyDescriptor(getter, setter);
				Properties.Add(propertyName, descriptor);
				return descriptor;
			}

			//Look for static fields
			var staticField = type.GetField(propertyName);
			if (staticField != null)
			{
				var descriptor = new FieldInfoDescriptor(Engine, staticField, Target);
				Properties.Add(propertyName, descriptor);
				return descriptor;
			}


			return PropertyDescriptor.Undefined;
		}

		private PropertyDescriptor MethodDescriptor(string propertyName, MethodInfo[] methods)
		{
			var func = new ClrMethodInfoFunc(Engine, methods, this);
			func.FastAddProperty("toString",
				new JsValue(new ClrFunctionInstance(Engine,
					(value, values) => new JsValue("function " + propertyName + "() { [native code] }"))),
				false, false, false);

			var descriptor = new PropertyDescriptor(func, false, true, false);
			Properties.Add(propertyName, descriptor);
			return descriptor;
		}
		
		
		readonly ConditionalWeakTable<ICallable, object> _delegatesCache = new ConditionalWeakTable<ICallable, object>();
		internal Action<T> ConvertDelegate<T>(JsValue @this, JsValue jsValue)
		{
			if (jsValue.IsNull())
				return null;
			
			var callable = jsValue.AsObject() as ICallable;
			Action<T> handler = null;
			if (callable != null)
			{
				handler = (Action<T>)_delegatesCache.GetValue(callable, key => (Action<T>)(e =>
				{
					_converter.TryConvert(e, out var val);
					key.Call(@this, new[] { val });
				}));
			}
			return handler;
		}
	}

	class ClrMethodInfoFunc : FunctionInstance
	{
		private readonly MethodInfoFunctionInstance _internalFunc;
		private readonly MethodInfo[] _methods;
		private readonly ClrObject _clrObject;

		public ClrMethodInfoFunc(Jint.Engine engine, MethodInfo[] methods, ClrObject clrObject)
			: base(engine, null, null, false)
		{
			_internalFunc = new MethodInfoFunctionInstance(engine, methods);
			_methods = methods;
			_clrObject = clrObject;
			Prototype = engine.Function.PrototypeObject;
		}

		public override JsValue Call(JsValue thisObject, JsValue[] arguments)
		{
			//hack to pass 'this' to event handler function.
			bool add;
			if (((add = _methods[0].Name == "AddEventListener") ||
			          _methods[0].Name == "RemoveEventListener")&& 
			    arguments.Length > 1)
			{
				var eventName = arguments[0].AsString();
				var handler = _clrObject.ConvertDelegate<Event>(thisObject, arguments[1]);

				var capture = arguments.Length > 2 && arguments[2].AsBoolean();

				if ((thisObject.AsObject() as ClrObject)?.Target is IEventTarget et)
				{
					if (add)
						et.AddEventListener(eventName, handler, capture);
					else
						et.RemoveEventListener(eventName, handler, capture);

					return JsValue.Undefined;
				}
			}

			//todo: it should be done for any count of such methods.
			
			//handle default parameters
			if (_methods.Length == 1)
			{
				var methodParameters = _methods[0].GetParameters();
				if (methodParameters.Length > arguments.Length &&
				    methodParameters.Any(x => x.HasDefaultValue))
				{
					var oldArguments = arguments;
					arguments = new JsValue[methodParameters.Length];
					Array.Copy(oldArguments, arguments, oldArguments.Length);

					for (var i = oldArguments.Length; i < arguments.Length; i++)
					{
						var par = methodParameters[i];
						if(par.HasDefaultValue)
							arguments[i] = JsValue.FromObject(Engine, par.DefaultValue);
					}
				}
			}

			return _internalFunc.Call(thisObject, arguments);
		}
	}
	
	public sealed class ClrPropertyInfoDescriptor : PropertyDescriptor
	{
		private readonly Jint.Engine _engine;
		private readonly PropertyInfo _propertyInfo;
		private readonly object _item;

		public ClrPropertyInfoDescriptor(Jint.Engine engine, PropertyInfo propertyInfo, object item)
		{
			this._engine = engine;
			this._propertyInfo = propertyInfo;
			this._item = item;
			this.Writable = new bool?(propertyInfo.CanWrite);
		}

		public override JsValue Value
		{
			get
			{
				return JsValue.FromObject(_engine, _propertyInfo.GetValue(_item, null));
			}
			set
			{
				try
				{
					JsValue jsValue = value;
					object obj;
					if (_propertyInfo.PropertyType == typeof(JsValue))
					{
						obj = jsValue;
					}
					else
					{
						obj = jsValue.ToObject();
						if (obj != null && obj.GetType() != this._propertyInfo.PropertyType)
							obj = _engine.ClrTypeConverter.Convert(obj, this._propertyInfo.PropertyType,
								(IFormatProvider) CultureInfo.InvariantCulture);
					}
					_propertyInfo.SetValue(_item, obj, null);
				}
				catch (Exception ex)
				{
					throw new JavaScriptException(JsValue.FromObject(_engine, ex));
				}
			}
		}
	}
}