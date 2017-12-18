using System;
using System.Linq;
using System.Reflection;
using Jint.Native;
using Jint.Native.Function;
using Jint.Native.Object;
using Jint.Runtime.Descriptors;
using Jint.Runtime.Descriptors.Specialized;
using Jint.Runtime.Environments;
using Jint.Runtime.Interop;

namespace Knyaz.Optimus.ScriptExecuting
{
	/// <summary>
	/// Represents c# object (DOM and other) in JS environment.
	/// </summary>
	internal class ClrObject : ObjectInstance, IObjectWrapper
	{
		public Object Target { get; set; }

		public ClrObject(Jint.Engine engine, Object obj)
			: base(engine)
		{
			Target = obj;
			//todo: use static prototypes
			Prototype = new ClrPrototype(engine, obj.GetType());
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
				var descriptor = new PropertyInfoDescriptor(Engine, property, Target);
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
			var func = new ClrMethodInfoFunc(Engine, methods);
			func.FastAddProperty("toString",
				new JsValue(new ClrFunctionInstance(Engine,
					(value, values) => new JsValue("function " + propertyName + "() { [native code] }"))),
				false, false, false);

			var descriptor = new PropertyDescriptor(func, false, true, false);
			Properties.Add(propertyName, descriptor);
			return descriptor;
		}
	}

	class ClrMethodInfoFunc : FunctionInstance
	{
		private MethodInfoFunctionInstance _internalFunc;
		private MethodInfo[] _methods;

		public ClrMethodInfoFunc(Jint.Engine engine, MethodInfo[] methods) : base(engine, (string[]) null, (LexicalEnvironment) null, false)
		{
			_internalFunc = new MethodInfoFunctionInstance(engine, methods);
			_methods = methods;
			Prototype = engine.Function.PrototypeObject;
		}

		public override JsValue Call(JsValue thisObject, JsValue[] arguments)
		{
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
}