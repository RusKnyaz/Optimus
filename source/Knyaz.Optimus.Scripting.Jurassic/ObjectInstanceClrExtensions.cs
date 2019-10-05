using System;
using System.Linq;
using System.Reflection;
using Jurassic.Library;
using Knyaz.Optimus.ScriptExecuting;
using PropertyAttributes = Jurassic.Library.PropertyAttributes;

namespace Knyaz.Optimus.Scripting.Jurassic
{
    static class ObjectInstanceClrExtensions
    {
        /// <summary>
        /// Adds properties to the specified js object from the specified clr object.
        /// </summary>
        public static void DefineProperties(this ObjectInstance jsObject, ClrTypeConverter ctx, Type type, object clrOwner = null)
        {
            var bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly;
            //register properties
            foreach (var property in type.GetProperties(bindingFlags).Where(p => p.GetCustomAttribute<JsHiddenAttribute>() == null))
            {
                var name = property.GetName();

                //todo: compile delegate instead of reflection, research what is faster.

                var jsGetter =
                    property.GetMethod != null && property.GetMethod.IsPublic ? new FuncInst(ctx, clrThis => property.GetValue(clrThis)) : null;
                
                var jsSetter =
                    property.SetMethod != null && property.SetMethod.IsPublic ? new ActInst(ctx, (clrThis, objects) => 
                        property.SetValue(clrThis, ctx.ConvertToClr(objects[0], property.PropertyType, ctx.ConvertToJs(clrThis)))) : null;

                var jsProperty = new PropertyDescriptor(jsGetter, jsSetter, PropertyAttributes.Configurable);

                jsObject.DefineProperty(name, jsProperty, false);
            }
            
            //register methods
            foreach (var methods in type
                .GetMethods(bindingFlags)
                .Where(p => p.GetCustomAttribute<JsHiddenAttribute>() == null)
                .GroupBy(p => p.GetName()))
            {
                var methodsArray = methods.ToArray();

                var firstMethod = methodsArray[0];
                
                if (firstMethod.IsSpecialName)
                    continue;

                if (firstMethod.DeclaringType == typeof(object))
                    continue;
                
                var name = methods.Key;
                
                if (name == "toString")
                    continue;

                var jsMethod =
                    methodsArray.Length == 1
                        ? new ClrMethodInstance(ctx, clrOwner, firstMethod, name)
                        : (FunctionInstance) new ClrOverloadMethodInstance(ctx, clrOwner, methodsArray, name);
                
                //todo: check the PropertyAttributes
                var jsProperty = new PropertyDescriptor(jsMethod, PropertyAttributes.Sealed);

                jsObject.DefineProperty(name, jsProperty, false);
            }
            
			//register events
            foreach (var evt in type.GetEvents(bindingFlags)
                .Where(p => p.GetCustomAttribute<JsHiddenAttribute>() == null))
            {
                var name = evt.GetName().ToLowerInvariant();
            
                var jsGetter = new FuncInst(ctx, clrThis =>
                {
                    var attachedEvents = ctx.GetAttachedEventsFor(clrThis);
                    return attachedEvents[evt];
                }, false);
                    
                var jsSetter = new ActInst(ctx, 
                    (clrThis, args) =>
                    {
                        var jsThis = ctx.ConvertToJs(clrThis);
                        
                        var attachedEvents = ctx.GetAttachedEventsFor(clrThis);
                        
                        if (attachedEvents.TryGetValue(evt, out var existHandler))
                        {
                            var clrHandler = ctx.ConvertToClr(existHandler, evt.EventHandlerType, jsThis);
                            evt.RemoveMethod.Invoke(clrThis, new []{clrHandler});
                            attachedEvents.Remove(evt);
                        }
                        
                        if (args.Length != 0 && args[0] is FunctionInstance functionInstance)
                        {
                            var clrHandler = ctx.ConvertToClr(args[0], evt.EventHandlerType, jsThis);
                            evt.AddMethod.Invoke(clrThis, new[]{clrHandler});
                            attachedEvents[evt] = functionInstance;
                        }
                    });

                var jsProperty = new PropertyDescriptor(jsGetter, jsSetter, PropertyAttributes.Configurable);

                jsObject.DefineProperty(name, jsProperty, false);
            }
            
            //Register static fields
            DefineStaticProperties(jsObject, ctx, type);
        }

        public static void DefineStaticProperties(this ObjectInstance jsObject, ClrTypeConverter ctx, Type type)
        {
            foreach (var staticField in type.GetFields(BindingFlags.Public | BindingFlags.Static)
                .Where(p => p.GetCustomAttribute<JsHiddenAttribute>() == null))
            {
                var clrValue = staticField.GetValue(null);
                var prop = new PropertyDescriptor(ctx.ConvertToJs(clrValue), PropertyAttributes.Sealed);
                jsObject.DefineProperty(staticField.Name, prop, false);
            }
        }
    }
}