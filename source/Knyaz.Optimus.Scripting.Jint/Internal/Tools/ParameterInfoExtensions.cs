using System;
using System.Linq;
using System.Reflection;
using Jint.Native;
using Jint.Native.Array;
using Jint.Native.Function;
using Jint.Native.Object;
using Knyaz.Optimus.Scripting.Jint.Internal;

namespace Knyaz.Optimus.ScriptExecuting
{
	static class ParameterInfoExtensions
	{
		public static bool IsAppropriate(this ParameterInfo[] pars, object[] argumentValues, bool strict)
		{
			if (strict)
			{
				if (pars.Length < argumentValues.Length)
					return false;
			}

			if (pars.Length == 0)
				return true;
				
			
			var lastParam = pars.Last();
			var isParamArray = lastParam.GetCustomAttribute<ParamArrayAttribute>() != null;

			var checkLength = isParamArray ? pars.Length - 1 : pars.Length;
			
			for (var idx = 0; idx < checkLength; idx++)
			{
				var par = pars[idx];

				if (idx >= argumentValues.Length)
				{
					if(par.IsOptional)
						continue;

					return false;
				}

				if (strict)
				{
					if (!CanAssign(argumentValues[idx], par.ParameterType))
						return false;
				}
				else
				{
					if (!CanConvert(argumentValues[idx], par.ParameterType))
						return false;	
				}
			}

			if (isParamArray && argumentValues.Length > checkLength)
			{
				var elementType = lastParam.ParameterType.GetElementType();
				return argumentValues.Skip(checkLength).All(x => CanConvert(x, elementType));
			}

			return true;
		}

		private static bool CanAssign(this object argumentValue, Type type)
		{
			if (argumentValue == null && !type.IsClass)
				return false;
			
			if (type == typeof(object))
				return true;

			if (!(argumentValue is JsValue jsValue)) 
				return type.IsAssignableFrom(argumentValue.GetType());
			
			if (jsValue.IsNull() || jsValue.IsUndefined())
				return type.IsClass;
				
			if (jsValue.IsString() && (type == typeof(string)))
				return true;

			if (jsValue.IsBoolean() && (type == typeof(bool) || type == typeof(bool?)))
				return true;
            
			if(jsValue.IsNumber() && (type == typeof(int) 
			                          ||type == typeof(ulong)
			                          || type == typeof(double)
			                          || type == typeof(short)
			                          || type == typeof(short?)
			                          || type == typeof(int?) 
			                          || type == typeof(double?)))
				return true;

			if (jsValue.IsObject())
			{
				var jsObj = jsValue.AsObject();
					
				if (jsObj is FunctionInstance && (typeof(Delegate)).IsAssignableFrom(type))
					return true;

				if (jsObj is ClrObject clrObjectInstance)
				{
					if (clrObjectInstance.Target == null)
						return type.IsClass;
                
					return type.IsAssignableFrom(clrObjectInstance.Target.GetType());
				}

				if (jsObj is ArrayInstance)
				{
					return type.IsArray;
				}
				
				if (jsObj is ObjectInstance)
				{
					//if the target type is class that contains only public fields
					//it can be deserialized from js object.

					return type.IsClass && type.BaseType == typeof(object) && type != typeof(string);
				}
			}

			return type.IsAssignableFrom(argumentValue.GetType());
		}

		private static bool CanConvert(this object argumentValue, Type type)
		{
			if (argumentValue == null && !type.IsClass)
				return false;
			
			if (type == typeof(object))
				return true;

			if (argumentValue is JsValue jsValue)
				return jsValue.CanConvert(type);

			if (type.IsAssignableFrom(argumentValue.GetType()))
				return true;

			return false;
		}
	}
}