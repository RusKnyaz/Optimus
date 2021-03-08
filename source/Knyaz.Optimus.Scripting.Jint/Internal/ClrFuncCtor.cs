using System;
using Jint.Native;
using Jint.Native.Function;
using Jint.Native.Object;
using Knyaz.Optimus.ScriptExecuting;

namespace Knyaz.Optimus.Scripting.Jint.Internal
{
    using Engine = global::Jint.Engine;
    
    internal class ClrFuncCtor : FunctionInstance, IConstructor
    {
        private readonly Func<JsValue[], ObjectInstance> _act;
        private readonly Type _clrType;

        public ClrFuncCtor(Type clrType, 
	        Engine engine, Func<JsValue[], ObjectInstance> act, 
	        ObjectInstance prototype)
	        : base(engine, null, null, false)
        {
            _act = act;
            _clrType = clrType;
            FastAddProperty("prototype", prototype, false, false, false);
            DomConverter.DefineStatic(this, clrType);
        }
        
	    public override JsValue Call(JsValue thisObject, JsValue[] arguments)
        {
            throw new NotImplementedException();
        }

        public ObjectInstance Construct(JsValue[] arguments) => _act(arguments);

        public override bool HasInstance(JsValue v)
        {
            var clrObject = v.TryCast<ClrObject>();
            if (clrObject?.Target != null && clrObject.Target != null)
            {
	            return clrObject.Target.GetType().IsInstanceOfType(_clrType);
            }

            return base.HasInstance(v);
        }
    }
}