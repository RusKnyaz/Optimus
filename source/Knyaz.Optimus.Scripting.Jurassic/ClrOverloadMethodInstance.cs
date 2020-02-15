using System;
using System.Linq;
using System.Reflection;
using Jurassic;
using Jurassic.Library;
using Knyaz.Optimus.Tools;

namespace Knyaz.Optimus.Scripting.Jurassic
{
    internal class ClrOverloadMethodInstance : FunctionInstance
    {
        private readonly ClrTypeConverter _ctx;
        private readonly object _owner;
        private readonly MethodInfo[] _methods;
        private readonly Func<object, object[], object[]>[] _paramConverters;
        

        public ClrOverloadMethodInstance(ClrTypeConverter ctx, object owner, MethodInfo[] methods, string name) : base(ctx.Engine)
        {
            _ctx = ctx;
            _owner = owner;
            _methods = methods.OrderByDescending(x => x.GetParameters().Length).ToArray();
            _paramConverters = new Func<object, object[], object[]>[methods.Length];
            SetPropertyValue("name", name, false);
        }

        public override object CallLateBound(object thisObject, params object[] argumentValues)
        {
            var clrObject = thisObject is Undefined ? _owner : _ctx.ConvertToClr(thisObject);

            var methodIndex = _methods.IndexOf(x => x.GetParameters().IsAppropriate(argumentValues));

            if (methodIndex < 0)
                return null; //or throw exception;

            var method = _methods[methodIndex];

            var converter = _paramConverters[methodIndex] ?? (_paramConverters[methodIndex] =
	            ClrMethodInstance.GetParamsConverter(_ctx, method.GetParameters()));

            var clrArguments = converter(thisObject, argumentValues);

            try
            {
	            var clrResult = method.Invoke(clrObject, clrArguments);
	            return _ctx.ConvertToJs(clrResult);
            }
            catch (Exception e)
            {
	            throw new JavaScriptException(_ctx.Engine, ErrorType.Error, e.Message, e);
            }
        }
    }

    static class ParameterInfoExtensions
    {
        public static bool IsAppropriate(this ParameterInfo[] pars, object[] argumentValues)
        {
            for (var idx = 0; idx < pars.Length; idx++)
            {
                var par = pars[idx];

                if (idx >= argumentValues.Length && !par.HasDefaultValue)
                {
                    return false;
                }

                if (!CanConvert(argumentValues[idx], par.ParameterType))
                    return false;
            }

            return true;
        }

        private static bool CanConvert(this object argumentValue, Type type)
        {
            if (argumentValue == null || argumentValue is Null)
                return type.IsClass;

            if ((argumentValue is string || argumentValue is int || argumentValue is double) && type == typeof(bool))
                return true;

            if (type.IsAssignableFrom(argumentValue.GetType()))
                return true;

            if (argumentValue is FunctionInstance && (typeof(Delegate)).IsAssignableFrom(type))
                return true;

            if (argumentValue is ClrObjectInstance clrObjectInstance)
            {
                if (clrObjectInstance.Target == null)
                    return type.IsClass;
                
                return type.IsAssignableFrom(clrObjectInstance.Target.GetType());
            }

            if (argumentValue is ArrayInstance)
            {
                return type.IsArray;
            }

            if (argumentValue is ObjectInstance)
            {
                //if the target type is class that contains only public fields
                //it can be deserialized from js object.

                return type.IsClass && type.BaseType == typeof(object);
            }

            return false;
        }
    }
}