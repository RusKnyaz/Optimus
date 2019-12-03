using System;
using System.Linq;
using System.Reflection;
using Jint.Native;

namespace Knyaz.Optimus.ScriptExecuting.Jint
{
    using Engine = global::Jint.Engine;
    
    internal class JintJsEngine
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
            }
        }

        public void Execute(string code) => _engine.Execute(code);
        

        public void AddGlobalType(Type type)
        {
	        var jsTypeName = type.GetCustomAttribute<JsNameAttribute>()?.Name ?? type.Name;
	        var clrctor = new ClrCtor(_engine, _typeConverter, type);
            _engine.Global.FastAddProperty(jsTypeName, clrctor, false, false, false);
        }
        
        public void AddGlobalType<T>(Type[] argumentsTypes, Func<object[], object> func)
        {
	        var jsTypeName = typeof(T).GetCustomAttribute<JsNameAttribute>()?.Name ?? typeof(T).Name;
	        
	        var jsCtor = new ClrFuncCtor<T>(_engine, args =>
	        {
		        var clrArgs = args.Take(argumentsTypes.Length).Select(
				        (arg, i) => _typeConverter.ConvertToObject(arg, argumentsTypes[i]))
			        .ToArray();
		        var obj = func(clrArgs);
		        _typeConverter.TryConvert(obj, out var res);
		        return res.AsObject();
	        }, _typeConverter.GetPrototype(typeof(T)))
	        {
		        Prototype = _engine.Function.PrototypeObject
	        };
            
	        _engine.Global.FastAddProperty(jsTypeName, jsCtor, false, false, false);
        }

        public void AddGlobalType<T>(string name, Func<object[], T> func)
        {
            var ctor = new ClrFuncCtor<T>(_engine, args =>
            {
                var clrArgs = args.Select(x => _typeConverter.ConvertFromJs(x)).ToArray();
                var obj = func(clrArgs);
                _typeConverter.TryConvert(obj, out var res);
                return res.AsObject();
            }, _typeConverter.GetPrototype(typeof(T)))
            {
	            Prototype = _engine.Function.PrototypeObject
            };
            
            _engine.Global.FastAddProperty(name, ctor, false, false, false);
        }

        public object ParseJson(string json) => _engine.Json.Parse(null, new[] {new JsValue(json)});

        public object Evaluate(string code)
        {
            var res = _engine.Execute(code).GetCompletionValue().ToObject();

            if(res is Func<JsValue, JsValue[], JsValue> func)
            {
                return (Func<object, object[], object>)((@this, args) =>
                    func(JsValue.FromObject(_engine, @this), args.Select(x => JsValue.FromObject(_engine, x)).ToArray()));
            }

            return res;
        }
    }
}