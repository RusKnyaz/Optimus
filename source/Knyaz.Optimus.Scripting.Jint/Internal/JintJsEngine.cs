using System;
using System.Linq;
using System.Reflection;
using Jint.Native;
using Jint.Runtime;
using Knyaz.Optimus.ScriptExecuting;

namespace Knyaz.Optimus.Scripting.Jint.Internal
{
    using Engine = global::Jint.Engine;
    
    internal class JintJsEngine : IJsScriptExecutor
    {
        private readonly Engine _engine;
        private readonly DomConverter _typeConverter;

        public JintJsEngine(object global)
        {
            _typeConverter = new DomConverter(() => _engine);

            _engine = new Engine(o => o.AddObjectConverter(_typeConverter));

            if (global != null)
            {
	            _typeConverter.SetGlobal(global);
	            _typeConverter.DefineProperties(_engine.Global, global.GetType());

	            foreach (var method in global.GetType().GetMethods(BindingFlags.Public|BindingFlags.Instance))
	            {
		            var ctorAttribute = method.GetCustomAttribute<JsCtorAttribute>();
		            if(ctorAttribute == null)
			            continue;

		            //todo: optimize, write test.
		            var type = method.ReturnType;
		            AddGlobalType(ctorAttribute.Name ?? type.GetName(), global, method);
	            }
            }
        }

        public void Execute(string code)
        {
	        try
	        {
		        _engine.Execute(code);
	        }
	        catch (JavaScriptException e)
	        {
		        throw new ScriptExecutingException(e.Error.ToString(), e, code);
	        }
        }


        /// <summary> Registers new type in JS with the specified name. </summary>
        /// <param name="type">The type to be registered.</param>
        /// <param name="jsTypeName">The name to be visible in a script.</param>
        public void AddGlobalType(Type type, string jsTypeName = null)
        {
	        jsTypeName = 
		        jsTypeName ??
		        type.GetCustomAttribute<JsNameAttribute>()?.Name ?? type.Name;
	        
	        var clrctor = new ClrCtor(_engine, _typeConverter, type);
            _engine.Global.FastAddProperty(jsTypeName, clrctor, false, false, false);
        }
        
        public void AddGlobalType(string jsTypeName, object owner, MethodInfo ctorMethod)
        {
	        var type = ctorMethod.ReturnType;
	        var jsCtor = new ClrFuncCtor(type, _engine, args =>
	        {
		        //todo: create executor from methodInfo
		        var clrArgs = ctorMethod.GetParameters().Select((parameterInfo, i) => i < args.Length
			        && args[i].CanConvert(parameterInfo.ParameterType) 
			        ? _typeConverter.ConvertToObject(args[i], parameterInfo.ParameterType)
			        : parameterInfo.HasDefaultValue
				        ? parameterInfo.DefaultValue
				        : parameterInfo.ParameterType.IsValueType ? Activator.CreateInstance(parameterInfo.ParameterType) 
					        : null).ToArray();
			        
		        var obj = ctorMethod.Invoke(owner, clrArgs);
		        _typeConverter.TryConvert(obj, out var res);
		        return res.AsObject();
	        }, _typeConverter.GetPrototype(type));
            
	        _engine.Global.FastAddProperty(jsTypeName, jsCtor, false, false, false);
        }

        public object Evaluate(string code)
        {
	        try
	        {
		        var res = _engine.Execute(code).GetCompletionValue().ToObject();

		        if (res is Func<JsValue, JsValue[], JsValue> func)
		        {
			        return (Func<object, object[], object>) ((@this, args) =>
				        func(JsValue.FromObject(_engine, @this),
					        args.Select(x => JsValue.FromObject(_engine, x)).ToArray()));
		        }

		        return res;
	        }
	        catch (JavaScriptException e)
	        {
		        return new ScriptExecutingException(e.Error.ToString(), e, code);
	        }
        }
    }
}