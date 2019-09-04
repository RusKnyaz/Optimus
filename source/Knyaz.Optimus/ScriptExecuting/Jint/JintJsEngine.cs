using System;
using System.Linq;
using System.Reflection;
using Jint.Native;
using Jint.Native.Function;
using Jint.Runtime.Descriptors.Specialized;
using Jint.Runtime.Interop;

namespace Knyaz.Optimus.ScriptExecuting
{
    internal class JintJsEngine : IJsEngine
    {
        private readonly Jint.Engine _engine;
        private readonly DomConverter _typeConverter;

        public JintJsEngine()
        {
            _typeConverter = new DomConverter();

            _engine = new Jint.Engine(o => o.AddObjectConverter(_typeConverter));
        }

        public void Execute(string code) => _engine.Execute(code);
        
        
        public void AddGlobalGetter(string name, Func<object> getter)
        {
            var jsGetter = new GetterFunctionInstance(_engine, @this => JsValue.FromObject(_engine, getter()));
            
            _engine.Global.DefineOwnProperty(name, new GetSetPropertyDescriptor(jsGetter, JsValue.Undefined), true);
        }

        public void AddGlobalFunc(string name, Func<object[], object> action)
        {
            var jsFunc = new ClrFunctionInstance(_engine, name, (@this, args) =>
            {
                var clrArgs = args.Select(x => x.IsObject() && x.AsObject() is ICallable callable
                    ? _typeConverter.ConvertDelegate(_engine, @this, callable)
                    : _typeConverter.ConvertFromJs(x)).ToArray();
                
                var result = action(clrArgs);
                
                return JsValue.FromObject(_engine, result);
            });
            
            var getter = new GetterFunctionInstance(_engine, @this => jsFunc);
            
            _engine.Global.DefineOwnProperty(name, new GetSetPropertyDescriptor(getter, JsValue.Undefined), true);
        }

        public void AddGlobalType(string jsTypeName, Type type)
        {
            _engine.Global.FastAddProperty(jsTypeName, new ClrPrototype(_engine, jsTypeName, type, _typeConverter), false, false, false);
        }

        public void AddGlobalType<T>(string name, Func<object[], T> func)
        {
            var ctor = new ClrFuncCtor<T>(_engine, args =>
            {
                var clrArgs = args.Select(x => _typeConverter.ConvertFromJs(x)).ToArray();
                var obj = func(clrArgs);
                var res = JsValue.FromObject(_engine, obj);
                return res.AsObject();
            }); 
            
            _engine.Global.FastAddProperty(name, ctor, false, false, false);
            
            //add public constants and static fields as getters
            foreach (var field in typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                ctor.FastAddProperty(field.Name, JsValue.FromObject(_engine, field.GetValue(null)), false, false, false);
            }
        }

     
        public void AddGlobalAct(string name, Action<object[]> action)
        {
            var jsFunc = new ClrFunctionInstance(_engine, "", (@this, args) =>
            {
                var clrArgs = args.Select(x => x.IsObject() && x.AsObject() is ICallable callable
                    ? _typeConverter.ConvertDelegate(_engine, @this, callable)
                    : _typeConverter.ConvertFromJs(x)).ToArray();   
                action(clrArgs);
                return JsValue.Undefined;
            });

            var getter = new GetterFunctionInstance(_engine, value => jsFunc);

            _engine.Global.DefineOwnProperty(name, new GetSetPropertyDescriptor(getter, JsValue.Undefined), true);
        }

        public object ParseJson(string json) => _engine.Json.Parse(null, new[] {JsValue.FromObject(_engine, json)});

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