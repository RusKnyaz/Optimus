using System;
using System.Linq;
using System.Reflection;
using Jint.Native;
using Jint.Runtime.Descriptors.Specialized;
using Jint.Runtime.Interop;

namespace Knyaz.Optimus.ScriptExecuting
{
    using Engine = global::Jint.Engine;
    
    internal class JintJsEngine
    {
        private readonly Engine _engine;
        private readonly DomConverter _typeConverter;

        public JintJsEngine()
        {
            _typeConverter = new DomConverter(() => _engine);

            _engine = new Engine(o => o.AddObjectConverter(_typeConverter));
        }

        public void Execute(string code) => _engine.Execute(code);
        
        
        public void AddGlobalGetter(string name, Func<object> getter)
        {
            _engine.Global.DefineOwnProperty(name, new ClrAccessDescriptor(_engine, value =>
            {
                JsValue res;
                _typeConverter.TryConvert(getter(), out res);
                return res;
            }), true);
        }

        public void AddGlobalFunc(string name, Func<object[], object> action)
        {
            var jsFunc = new ClrFunctionInstance(_engine, (@this, args) =>
            {
                var clrArgs = args.Select(x => x.IsObject() && x.AsObject() is ICallable callable
                    ? _typeConverter.ConvertDelegate(@this, callable)
                    : _typeConverter.ConvertFromJs(x)).ToArray();
                
                var result = action(clrArgs);
                
                _typeConverter.TryConvert(result, out var jsValue);
                return jsValue;
            });
            _engine.Global.DefineOwnProperty(name, new ClrAccessDescriptor(_engine, value => jsFunc), true);
        }

        public void AddGlobalType(string jsTypeName, Type type)
        {
            _engine.Global.FastAddProperty(jsTypeName, new JsValue(new ClrPrototype(_engine, type, _typeConverter)), false, false, false);
        }

        public void AddGlobalType<T>(string name, Func<object[], T> func)
        {
            var ctor = new ClrFuncCtor<T>(_engine, args =>
            {
                var clrArgs = args.Select(x => _typeConverter.ConvertFromJs(x)).ToArray();
                var obj = func(clrArgs);
                _typeConverter.TryConvert(obj, out var res);
                return res.AsObject();
            }); 
            
            _engine.Global.FastAddProperty(name, ctor, false, false, false);
            
            //add public constants and static fields as getters
            foreach (var field in typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                ctor.FastAddProperty(field.Name, JsValue.FromObject(_engine, field.GetValue(null)), false, false, false);
            }
        }

        public void AddGlobalAct(string name, Action<JsValue, JsValue[]> action)
        {
            var jsFunc = new ClrFunctionInstance(_engine, (jsValue, values) =>
            {
                action(jsValue, values);
                return JsValue.Undefined;
            });

            _engine.Global.DefineOwnProperty(name, new ClrAccessDescriptor(_engine, value => jsFunc), true);
        }
        
        public void AddGlobalAct(string name, Action<object[]> action)
        {
            var jsFunc = new ClrFunctionInstance(_engine, (@this, args) =>
            {
                var clrArgs = args.Select(x => x.IsObject() && x.AsObject() is ICallable callable
                    ? _typeConverter.ConvertDelegate(@this, callable)
                    : _typeConverter.ConvertFromJs(x)).ToArray();   
                action(clrArgs);
                return JsValue.Undefined;
            });

            _engine.Global.DefineOwnProperty(name, new ClrAccessDescriptor(_engine, value => jsFunc), true);
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