using System;
using System.Linq;
using System.Reflection;
using Jurassic;
using Jurassic.Library;
using Knyaz.Optimus.ScriptExecuting;
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
            _paramsConverter = GetParamsConverter(_ctx, _method.GetParameters());
            
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

        public static Func<object, object[], object[]> GetParamsConverter(ClrTypeConverter ctx, ParameterInfo[] methodParameters)
        {
	        if (methodParameters.Length > 0)
	        {
		        var lastParameter = methodParameters.Last();

		        //have a deal with 'param'
		        if (lastParameter.GetCustomAttribute<ParamArrayAttribute>() != null)
		        {
			        return (thisObject, argumentValues) =>
			        {
				        if (argumentValues.Length < methodParameters.Length)
					        return ctx.ConvertParametersToClr(methodParameters, thisObject, argumentValues);
				        
				        var part1Types = methodParameters.Take(methodParameters.Length - 1).ToArray();
				        var clrArgumentsPart1 = ctx.ConvertParametersToClr(part1Types, thisObject, argumentValues);

				        var part2Types = Enumerable
					        .Repeat(typeof(object), argumentValues.Length - part1Types.Length)
					        .ToArray();

				        var clrArgumentsPart2 = ctx.ConvertParametersToClr(part2Types, thisObject,
					        argumentValues.Skip(part1Types.Length).ToArray());

				        var clrArguments = new object[methodParameters.Length];

				        Array.Copy(clrArgumentsPart1, clrArguments, clrArgumentsPart1.Length);
				        clrArguments[clrArguments.Length - 1] = clrArgumentsPart2;
				        return clrArguments;

			        };
		        }

		        return (thisObject,argumentValues) => 
				        ctx.ConvertParametersToClr(methodParameters, thisObject, argumentValues);
	        }

	        return (_,__) => new object[0];
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

				    var expand =
					    par.Member is MethodInfo methodInfo
						    ? methodInfo.GetInterfaceDeclarationsForMethod().Any(x =>
							    x.GetParameters()[idx].GetCustomAttribute<JsExpandArrayAttribute>() != null)
						    //par.Member can be constructor
						    : par.GetCustomAttribute<JsExpandArrayAttribute>() != null;

				    var jsArgument = argumentValues[idx];
				    clrArguments[idx] = ctx.ConvertToClr(
					    jsArgument,
					    par.ParameterType,
					    thisObject,
					    expand);
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