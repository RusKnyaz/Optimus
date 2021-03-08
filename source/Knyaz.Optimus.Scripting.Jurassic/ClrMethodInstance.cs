using System;
using System.Linq;
using System.Reflection;
using Jurassic;
using Jurassic.Library;
using Knyaz.Optimus.ScriptExecuting;
using Knyaz.Optimus.Scripting.Jurassic.Tools;
using Knyaz.Optimus.Tools;

namespace Knyaz.Optimus.Scripting.Jurassic
{
    /// <summary>
    /// MethodInfo -> FunctionInstance adapter
    /// </summary>
	internal class ClrMethodInstance : FunctionInstance
	{
        private readonly ClrTypeConverter _ctx;
        private readonly object _owner;
        private readonly MethodInfo _method;
        private readonly Func<object, object[], object[]> _paramsConverter;

        public ClrMethodInstance(ClrTypeConverter ctx, object owner, MethodInfo method, string name) : base(ctx.Engine)
        {
            _ctx = ctx;
            _owner = owner;
            _method = method;
            _paramsConverter = ClrTypeConverter.GetParamsConverter(_ctx, _method.GetParameters());
            
            SetPropertyValue("name", name, false);
        }

        public override object CallLateBound(object thisObject, params object[] argumentValues)
        {
            var clrObject = thisObject is global::Jurassic.Undefined ?  _owner : _ctx.ConvertToClr(thisObject);

            //todo: investigate if the precompiled expression can be faster than reflection invoke.

            
            return CallMethod(_ctx, _method, thisObject, argumentValues, clrObject);
        }

        public object CallMethod(ClrTypeConverter ctx, MethodInfo method, object thisObject,
            object[] argumentValues, object clrObject)
        {
	        if (thisObject is ClrPrototype proto)
	        {
		        if (method.Name == "ToString")
		        {
			        return $"[object {proto.Name}]";
		        }
		        
		        throw new JavaScriptException(ctx.Engine, ErrorType.Error, "Illegal method invocation.");
	        }
	        
            var clrArguments = _paramsConverter(thisObject, argumentValues);

            try
            {
                var clrResult = method.Invoke(clrObject, clrArguments);
                return ctx.ConvertToJs(clrResult);
            }
            catch (Exception e)
            {
                throw new JavaScriptException(ctx.Engine, ErrorType.Error, e.Message, e);
            }
        }

      
    }

    static class ClrTypeConverterExtension
    {
	    public static object[] ConvertParametersToClr(this ClrTypeConverter ctx, ParameterInfo[] methodParameters,
		    object thisObject,
		    object[] argumentValues)
	    {
		    var clrArguments = new object[methodParameters.Length];
		    for (var idx = 0; idx < clrArguments.Length; idx++)
		    {
			    var par = methodParameters[idx];

			    if (idx < argumentValues.Length)
			    {
				    //todo: optimize
				    var expand = par.GetCustomAttribute<JsExpandArrayAttribute>() != null;
				    
				    var jsArgument = argumentValues[idx];

				    if (!expand && (par.ParameterType.IsValueType))
				    {
					    clrArguments[idx] = ctx.GetConverter(par.ParameterType, par.GetDefaultValue())(jsArgument);
				    }
				    else
				    {
					    clrArguments[idx] =
						    ctx.ConvertToClr(
							    jsArgument,
							    par.ParameterType,
							    thisObject,
							    expand);
				    }
			    }
			    else
			    {
				    if (par.GetCustomAttribute<ParamArrayAttribute>() != null)
					    clrArguments[idx] = Activator.CreateInstance(par.ParameterType, 0);
				    else if (par.HasDefaultValue)
					    clrArguments[idx] = par.DefaultValue;
			    }
		    }

		    return clrArguments;
	    }

	    public static object[] ConvertParametersToClr(this ClrTypeConverter ctx, Type[] methodParameters,
		    object thisObject,
		    object[] argumentValues)
	    {

		    var clrArguments = new object[methodParameters.Length];
		    for (var idx = 0; idx < clrArguments.Length; idx++)
		    {
			    if (idx < argumentValues.Length)
			    {
				    var jsArgument = argumentValues[idx];
				    clrArguments[idx] = ctx.ConvertToClr(jsArgument, methodParameters[idx], thisObject);
			    }
		    }

		    return clrArguments;
	    }
	}
}