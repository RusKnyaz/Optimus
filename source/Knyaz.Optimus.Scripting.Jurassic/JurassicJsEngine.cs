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
    internal class JurassicJsEngine : IJsScriptExecutor
    {
        private readonly ClrTypeConverter _typeConverter;
        private object _global;

        private ScriptEngine Engine => _typeConverter.Engine;

        public JurassicJsEngine(object global)
        {
            _typeConverter = new ClrTypeConverter(new ScriptEngine(), () => _global);

            if (global != null)
            {
	            _global = global;
	            Engine.Global.DefineProperties(_typeConverter, global.GetType(), global);
	            foreach (var method in _global.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance))
	            {
		            var ctor = method.GetCustomAttribute<JsCtorAttribute>();
		            if(ctor == null)
			            continue;

		            var paramConverter = ClrTypeConverter.GetParamsConverter(_typeConverter, method.GetParameters());
		            
		            var prototype = _typeConverter.GetPrototype(method.ReturnType);
		            var jsCtor = new ClrCtor(_typeConverter, method.ReturnType, prototype, ctor.Name, args =>
		            {
			            var clrArgs = paramConverter(Engine.Global, args);
			            return method.Invoke(global, clrArgs);
		            });
            
		            _typeConverter.Engine.SetGlobalValue(ctor.Name, jsCtor);
	            }
            }
        }

        public void Execute(string code)
        {
	        if (code == null) //if error occurred on script loading.
		        return;

	        try
	        {
		        Engine.Execute(Clean(code));
	        }
	        catch (JavaScriptException e)
	        {
		        throw new ScriptExecutingException(e.Message, e, code);
	        }
        }

        public object Evaluate(string code)
        {
	        try
	        {
		        return _typeConverter.ConvertToClr(Engine.Evaluate(Clean(code)));
	        }
	        catch (JavaScriptException e)
	        {
		        return new ScriptExecutingException(e.Message, e, code);
	        }
        }

        public void AddGlobalType(Type type)
        {
            var jsTypeName = type.GetCustomAttribute<JsNameAttribute>()?.Name ?? type.Name;
            
            var prototype = _typeConverter.GetPrototype(type);
            
            var ctors = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance).OrderByDescending(x => x.GetParameters().Length).ToArray();

            if (ctors.Length > 0)
            {
                var jsCtor = new ClrCtor(_typeConverter, type, prototype,jsTypeName, args =>
                {
                    var ctor = ctors.FirstOrDefault(x => x.GetParameters().IsAppropriate(args));
                    if(ctor == null)
	                    throw new Exception("Appropriate constructor not found.");
                    
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


        private static string Clean(string code)
        {
            code = code.TrimStart('\r','\n','\t',' ');
            if (code.StartsWith("<!--"))
                return code.Remove(0, 4);
            return code;
        }
    }

    class ClrPrototype : ObjectInstance
    {
	    public ClrPrototype(ClrTypeConverter ctx, Type type) : base(ctx.Engine, ctx.GetPrototype(type.BaseType))
	    {
		    var name = type.GetCustomAttribute<JsNameAttribute>()?.Name ?? type.Name;
		    this[ctx.Engine.Symbol.ToStringTag] = name;
		    this.DefineProperties(ctx, type);
		    Name = name;
	    }

	    public string Name { get; }
    }
}