using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Jint;
using Jint.Native;
using Jint.Native.Function;
using Jint.Native.Object;
using Jint.Runtime;
using Jint.Runtime.Descriptors;
using Jint.Runtime.Descriptors.Specialized;
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
			Prototype = new ClrPrototype(engine, "", obj.GetType(), _converter);
			Extensible = true;
		}

		
		public override PropertyDescriptor GetOwnProperty(in Key propertyName)
		{
			var existProp = base.GetOwnProperty(propertyName);
			if (existProp != null && existProp != PropertyDescriptor.Undefined)
				return existProp;

			var newProp = FindProperty(propertyName);
			if(newProp != null && newProp != PropertyDescriptor.Undefined)
				AddProperty(propertyName, newProp);
			
			return newProp;
		}
		
		private PropertyDescriptor FindProperty(string propertyName)
		{
			//todo: check indexers (for example in CssStyleDeclaration)
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
				var descriptor = CreatePropertyDescriptor(property); 
				return descriptor;
			}

			// look for a field
			var field = type.GetField(pascalCasedPropertyName, BindingFlags.Instance | BindingFlags.Public);

			if (field != null)
			{
				return new FieldInfoDescriptor(Engine, field, Target);
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

				var invoke = eventInfo.EventHandlerType.GetMethod("Invoke");
				var pars = invoke.GetParameters();
				var settedHandler = new JsValue[1] {JsValue.Null};

				Delegate listener;
				
				if (invoke.ReturnType != typeof(void))
				{
					if (pars.Length == 1 && pars[0].ParameterType == typeof(Event))
					{
						listener = (Func<Event, bool?>)(
							e => (bool?)settedHandler[0].Invoke(this, new []
							{JsValue.FromObject(Engine, e)}).ToObject());
					}
					else
						throw new NotImplementedException(); //todo: build and compile the Expression.
				}
				else
				{
					listener = pars.Length == 1 
						? (Delegate) (Action<object>) (p1 => settedHandler[0].Invoke(this, new [] {JsValue.FromObject(Engine, p1)})) 
						: (Action) (() => settedHandler[0].Invoke(this, new JsValue[0]));
				}
				
				var getter = new GetterFunctionInstance(Engine, value => settedHandler[0]);
				var setter = new SetterFunctionInstance(Engine, (@this, value) =>
				{
					if (settedHandler[0] != JsValue.Null)
						eventInfo.RemoveEventHandler(Target, listener);

					settedHandler[0] = value;
					if (settedHandler[0] != JsValue.Null)
						eventInfo.AddEventHandler(Target, listener);
				});
				
				var descriptor = new GetSetPropertyDescriptor(getter, setter);
				return descriptor;
			}

			//Look for static fields
			var staticField = type.GetField(propertyName);
			if (staticField != null && staticField.GetCustomAttribute<JsHiddenAttribute>() == null)
			{
				return new FieldInfoDescriptor(Engine, staticField, Target);
			}

			return PropertyDescriptor.Undefined;
		}

		/// <summary>
		/// Creates JS <see cref="PropertyDescriptor"/> for a given .net <see cref="PropertyInfo"/> attached to this object.
		/// </summary>
		/// <param name="property"></param>
		/// <returns></returns>
		/// <exception cref="JavaScriptException"></exception>
		private PropertyDescriptor CreatePropertyDescriptor(PropertyInfo property)
		{
			var get = property.GetMethod == null ? Undefined :  
				new GetterFunctionInstance(Engine, @this => FromObject(Engine, property.GetValue(Target, null)));

			var set = property.SetMethod == null 
				? Undefined
				: new SetterFunctionInstance(Engine, (@this, value) => {
					try
					{
						object obj;
						if (property.PropertyType == typeof(JsValue))
						{
							obj = value;
						}
						else
						{
							obj = value.ToObject();
							if (obj != null && obj.GetType() != property.PropertyType)
								obj = _engine.ClrTypeConverter.Convert(obj, property.PropertyType,
									CultureInfo.InvariantCulture);
						}

						property.SetValue(Target, obj, null);
					}
					catch (Exception ex)
					{
						throw new JavaScriptException(JsValue.FromObject(_engine, ex));
					}
				});

			return new GetSetPropertyDescriptor(get,set);
		}


		private PropertyDescriptor MethodDescriptor(string propertyName, MethodInfo[] methods)
		{
			var func = new ClrMethodInfoFunc(Engine, methods, this);
			func.FastAddProperty("toString",
				JsValue.FromObject(Engine, new ClrFunctionInstance(Engine, "", 
					(value, values) => JsValue.FromObject(Engine, "function " + propertyName + "() { [native code] }"))),
				false, false, false);

			return new PropertyDescriptor(func, false, true, false);
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
					var val = JsValue.FromObject(Engine, e);
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
			: base(engine, "", null, null, false)
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

				if ((thisObject.AsObject() as ClrObject)?.Target is IEventTarget et)
				{
					var capture = false;
					var optionsJs = arguments.Length > 2 ? arguments[2].ToBooleanOrObject(out capture) : null;
					
					if (optionsJs == null)
					{
						if (add)
							et.AddEventListener(eventName, handler, capture);
						else
							et.RemoveEventListener(eventName, handler, capture);
					}
					else
					{
						bool GetBool(JsValue val) => val.IsBoolean() && val.AsBoolean();

						var options = new EventListenerOptions {
							Capture = GetBool(optionsJs.Get("capture")),
							Passive = GetBool(optionsJs.Get("passive")),
							Once = GetBool(optionsJs.Get("once"))
						};
						
						if(add)
							et.AddEventListener(eventName, handler, options);
						else
							et.RemoveEventListener(eventName, handler, options);
					}
					
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

			try
			{
				var maxParams = _methods.Max(x => x.GetParameters().Length);

				if (arguments.Length > maxParams)
				{
					var tmp = new JsValue[maxParams];
					Array.Copy(arguments, 0, tmp, 0, tmp.Length);
					arguments = tmp;
				}
				
				return _internalFunc.Call(thisObject, arguments);
			}
			catch (Exception ex)
			{
				throw new JavaScriptException(JsValue.FromObject(Engine, ex.Message));
			}
		}
	}
}