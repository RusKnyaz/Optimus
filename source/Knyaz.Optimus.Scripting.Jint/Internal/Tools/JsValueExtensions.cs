using System;
using Jint.Native;
using Jint.Native.Array;
using Jint.Native.Function;
using Jint.Native.Object;
using Knyaz.Optimus.Scripting.Jint.Internal;

namespace Knyaz.Optimus.ScriptExecuting
{
	static class JsValueExtensions
	{
		public static bool CanConvert(this JsValue jsValue, Type targetType)
		{
			if (jsValue.IsNull() || jsValue.IsUndefined())
				return targetType.IsClass;
				
			if (jsValue.IsString() && (targetType == typeof(string) || targetType == typeof(bool)))
				return true;

			if (jsValue.IsBoolean() && (targetType == typeof(bool) || targetType == typeof(bool?)))
				return true;
            
			if(jsValue.IsNumber() && (targetType == typeof(int) 
			                          ||targetType == typeof(ulong)
			                          || targetType == typeof(double)
			                          || targetType == typeof(short)
			                          || targetType == typeof(short?)
			                          || targetType == typeof(int?) 
			                          || targetType == typeof(double?)
			                          || targetType == typeof(string)
			                          || targetType == typeof(bool)
			                          || targetType == typeof(bool?)))
				return true;

			if (jsValue.IsObject())
			{
				var jsObj = jsValue.AsObject();
					
				if (jsObj is FunctionInstance && (typeof(Delegate)).IsAssignableFrom(targetType))
					return true;

				if (jsObj is ClrObject clrObjectInstance)
				{
					if (clrObjectInstance.Target == null)
						return targetType.IsClass;
                
					return targetType.IsAssignableFrom(clrObjectInstance.Target.GetType());
				}

				if (jsObj is ArrayInstance)
				{
					return targetType.IsArray;
				}		
					
				if (jsObj is ObjectInstance)
				{
					//if the target type is class that contains only public fields
					//it can be deserialized from js object.

					return targetType.IsClass && targetType.BaseType == typeof(object) && targetType != typeof(string);
				}
			}

			return false;
		}
	}
}