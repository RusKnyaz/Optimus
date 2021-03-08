using System;
using System.Linq.Expressions;
using System.Reflection;
using Jint.Native;
using Jint.Runtime;
using Jint.Runtime.Descriptors;
using Jint.Runtime.Interop;

namespace Knyaz.Optimus.Scripting.Jint.Internal
{
	class ClrProperty : PropertyDescriptor
	{
		public ClrProperty(global::Jint.Engine engine, DomConverter converter, PropertyInfo property)
		{
			Get =
				property.GetMethod != null && property.GetMethod.IsPublic ?  new ClrFunctionInstance(engine, (jsThis, values) =>
				{
					var clrThis = converter.ConvertFromJs(jsThis);
					return JsValue.FromObject(engine, property.GetValue(clrThis));
				}) : null;

			var setter = new Lazy<Action<object, JsValue>>(() => CreateSetter(converter, property));
			
			Set = 
				property.SetMethod != null && property.SetMethod.IsPublic ? new ClrFunctionInstance(engine, (jsThis, values) =>
				{
					try
					{
						var clrThis = converter.ConvertFromJs(jsThis);
						if (values.Length == 0)
							return JsValue.Undefined;

						setter.Value(clrThis, values[0]);

						return JsValue.Undefined;
					}
					catch (Exception e)
					{
						throw new JavaScriptException(e.Message);
					}
				}) : new ClrFunctionInstance(engine, (value, values) => JsValue.Undefined);
		}

		private static Action<object, JsValue> CreateSetter(DomConverter converter, PropertyInfo property)
		{
			var expThisArg = Expression.Parameter(typeof(object), "clrThis");
			var expValueArg = Expression.Parameter(typeof(JsValue), "jsValue");

			var setterExpression =
				Expression.Assign(
					Expression.Property(Expression.Convert(expThisArg, property.DeclaringType), property),
					converter.CreateConverterExpr(property.PropertyType, expValueArg, expThisArg));

			var setterLambda = Expression.Lambda<Action<object, JsValue>>(setterExpression, expThisArg, expValueArg)
				.Compile();
			return setterLambda;
		}
	}
}