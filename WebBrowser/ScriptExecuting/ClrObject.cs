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

namespace WebBrowser.ScriptExecuting
{
	public class ClrPrototype : FunctionInstance
	{
		private readonly Type _type;

		public ClrPrototype(Jint.Engine engine, Type type) : 
			base(engine, null, null, false)
		{
			_type = type;
		}

		public override JsValue Call(JsValue thisObject, JsValue[] arguments)
		{
			throw new NotImplementedException();
		}

		public override bool HasInstance(JsValue v)
		{
			var clrObject = v.TryCast<ClrObject>();
			if (clrObject != null && clrObject.Target != null)
			{
				return _type.IsInstanceOfType(clrObject.Target);
			}

			return base.HasInstance(v);
		}
	}

	public class ClrObject : ObjectInstance, IObjectWrapper
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
				var descriptor = new PropertyDescriptor(new MethodInfoFunctionInstance(Engine, methods), false, true, false);
				Properties.Add(propertyName, descriptor);
				return descriptor;
			}

			// look for methods using pascal cased name.
			var pascalCasedMethods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public)
			                             .Where(m => m.Name == pascalCasedPropertyName)
			                             .ToArray();

			if (pascalCasedMethods.Any())
			{
				var descriptor = new PropertyDescriptor(new MethodInfoFunctionInstance(Engine, pascalCasedMethods), false, true, false);
				Properties.Add(propertyName, descriptor);
				return descriptor;
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

				var settedHandler = new JsValue[1] { JsValue.Null };
				var listener = (Action)(() => settedHandler[0].Invoke(this, new JsValue[0]));

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

			return PropertyDescriptor.Undefined;
		}
	}
}