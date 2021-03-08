using System;
using System.Linq;
using System.Reflection;
using Jurassic;
using Jurassic.Library;
using Knyaz.Optimus.Scripting.Jurassic.Tools;

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
	            ClrTypeConverter.GetParamsConverter(_ctx, method.GetParameters()));

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
}