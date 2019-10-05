using System.Reflection;
using Jint.Native;
using Jint.Native.Function;
using Jint.Runtime.Descriptors;
using Jint.Runtime.Interop;

namespace Knyaz.Optimus.ScriptExecuting.Jint
{
	class ClrEvent : PropertyDescriptor
	{
		public ClrEvent(global::Jint.Engine engine, DomConverter converter, EventInfo eventInfo)
		{
			Get = new ClrFunctionInstance(engine, (clrThis,args) =>
			{
				var attachedEvents = converter.GetAttachedEventsFor(clrThis);
				return attachedEvents.TryGetValue(eventInfo, out var func) ? func : JsValue.Null;
			});
                    
			Set = new ClrFunctionInstance(engine, (jsThis, args) =>
			{
				var objectInstance = jsThis.AsObject();
				var clrThis = jsThis.ToObject();
                        
				var attachedEvents = converter.GetAttachedEventsFor(objectInstance);
                    
				if (attachedEvents.TryGetValue(eventInfo, out var existHandler))
				{
					var clrHandler = converter.ConvertToClr(existHandler, eventInfo.EventHandlerType, jsThis);
					eventInfo.RemoveMethod.Invoke(clrThis, new object[]{clrHandler});
					attachedEvents.Remove(eventInfo);
				}
                    
				if (args.Length != 0 && !args[0].IsNull() && !args[0].IsUndefined() && args[0].AsObject() is FunctionInstance functionInstance)
				{
					var clrHandler = converter.ConvertToClr(functionInstance, eventInfo.EventHandlerType, jsThis);
					eventInfo.AddMethod.Invoke(clrThis, new object[]{clrHandler});
					attachedEvents[eventInfo] = functionInstance;
				}

				return null;
			});
		}
	}
}