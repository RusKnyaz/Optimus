using System;
using System.Linq;
using System.Reflection;
using Jurassic;
using Jurassic.Library;
using Knyaz.Optimus.ScriptExecuting;
using BindingFlags = System.Reflection.BindingFlags;
using FunctionInstance = Jurassic.Library.FunctionInstance;
using PropertyAttributes = Jurassic.Library.PropertyAttributes;

namespace Knyaz.Optimus.Scripting.Jurassic
{
    internal class JurassicJsEngine : IJsEngine
    {
        private readonly ClrTypeConverter _typeConverter;
        private object _global;

        private ScriptEngine Engine => _typeConverter.Engine;

        public JurassicJsEngine()
        {
            _typeConverter = new ClrTypeConverter(new ScriptEngine(), () => _global);
        }

        //Adds properties for a given object to the Global object
        public void SetGlobal(object global)
        {
            _global = global;
            Engine.Global.DefineProperties(_typeConverter, global.GetType(), global);
        }

        public void Execute(string code) => Engine.Execute(Clean(code));

        public void AddGlobalType(Type type)
        {
            var jsTypeName = type.GetCustomAttribute<JsNameAttribute>()?.Name ?? type.Name;
            
            var prototype = _typeConverter.GetPrototype(type);
            
            var ctors = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance).OrderByDescending(x => x.GetParameters().Length).ToArray();

            if (ctors.Length > 1)
            {
                var jsCtor = new ClrCtor(_typeConverter, type, prototype,jsTypeName, args =>
                {
                    var ctor = ctors.First(x => x.GetParameters().IsAppropriate(args));
                    var ctorParameters = ctor.GetParameters();
                    var clrArgs = _typeConverter.ConvertParametersToClr(ctorParameters, null, args);
                    return ctor.Invoke(clrArgs);
                });
                
                _typeConverter.Engine.SetGlobalValue(jsTypeName, jsCtor);
            }
            else
            {
                //todo: fix error
                var jsCtor = new ClrCtor(_typeConverter, type, prototype,jsTypeName, _ => throw new Exception("Error"));
                _typeConverter.Engine.SetGlobalValue(jsTypeName, jsCtor);
            }
        }
        
        
        public void AddGlobalType(string name, Type type, Func<object[], object> func)
        {
            var prototype = _typeConverter.GetPrototype(type);
            
            var ctor = new ClrCtor(_typeConverter, type, prototype, name, func);

            _typeConverter.Engine.SetGlobalValue(name, ctor);
        }

        public void AddGlobalType(Type type, string jsTypeName, Type[] argumentsTypes, Func<object[], object> func)
        {
            var prototype = _typeConverter.GetPrototype(type);
            
            var jsCtor = new ClrCtor(_typeConverter, type, prototype, jsTypeName, args =>
            {
                var clrArgs = _typeConverter.ConvertParametersToClr(argumentsTypes, null, args);
                return func(clrArgs);
            });
            
            _typeConverter.Engine.SetGlobalValue(jsTypeName, jsCtor);
        }


        class ClrCtor : ClrFunction
        {
            private readonly ClrTypeConverter _ctx;
            private readonly Func<object[], object> _creator;

            public ClrCtor(ClrTypeConverter ctx, Type type, ObjectInstance prototype, string name, Func<object[], object> creator)
                : base(ctx.Engine.Function.InstancePrototype, name, prototype)
            {
                _ctx = ctx;
                _creator = creator;
                this.DefineStaticProperties(ctx, type);
            }

            public override ObjectInstance ConstructLateBound(params object[] argumentValues)
            {
                var inst = (ObjectInstance) _ctx.ConvertToJs(_creator(argumentValues));
                return inst;
            }
        }
        
        
        class FuncInst2 : FunctionInstance
        {
            private readonly Func<object, object[], object> _getter;

            public FuncInst2(ScriptEngine engine, Func<object, object[], object> getter) : base(engine)
            {
                _getter = getter;
            }

            
            public override object CallLateBound(object thisObject, params object[] argumentValues)
            {
                return _getter(thisObject, argumentValues);
            }
        }
        
        public void AddGlobalGetter(string name, Func<object> getter)
        {
            var jsGetter = (FunctionInstance)_typeConverter.ConvertToJs(getter); 
            var property = new PropertyDescriptor(jsGetter, null, PropertyAttributes.Configurable);
            Engine.Global.DefineProperty(name, property, true);
        }

        public void AddGlobalAct(string name, Action<object[]> action)
        {
            var jsGetter = _typeConverter.ConvertToJs(action); 
            
            var property = new PropertyDescriptor(jsGetter, PropertyAttributes.FullAccess);
            Engine.Global.DefineProperty(name, property, true);
        }

        public void AddGlobalFunc(string name, Func<object[], object> action)
        {
            var jsFunc = new FuncInst2(Engine, (x,y) => action(y));
            var property = new PropertyDescriptor(jsFunc, PropertyAttributes.Sealed);
            Engine.Global.DefineProperty(name, property, true);
        }

        

        public object ParseJson(string json)
        {
            var x = (JSONObject)Engine.Global["JSON"];
            var parseMethod = (FunctionInstance)x["parse"];
            return parseMethod.Call(x, json);
        }

        private static string Clean(string code)
        {
            code = code.TrimStart('\r','\n','\t',' ');
            if (code.StartsWith("<!--"))
                return code.Remove(0, 4);
            return code;
        }

        public object Evaluate(string code) => _typeConverter.ConvertToClr(Engine.Evaluate(Clean(code)));
    }
    
    class ClrPrototype : ObjectInstance
    {
        public ClrPrototype(ClrTypeConverter ctx, Type type) : base(ctx.Engine, ctx.GetPrototype(type.BaseType))
        {
            this[ctx.Engine.Symbol.ToStringTag] =(type.GetCustomAttribute<JsNameAttribute>()?.Name ?? type.Name)+"Prototype";
            this.DefineProperties(ctx, type);
        }
    }
}