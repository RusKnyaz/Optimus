using System;
using Jint.Native;
using Jint.Native.Function;
using Jint.Native.Object;

namespace Knyaz.Optimus.ScriptExecuting
{
    using Engine = global::Jint.Engine;
    
    internal class ClrFuncCtor<ClrType> : FunctionInstance, IConstructor
    {
        private readonly Func<JsValue[], ObjectInstance> _act;

        public ClrFuncCtor(
	        Engine engine, Func<JsValue[], ObjectInstance> act, 
	        ObjectInstance prototype)
	        : base(engine, null, null, false)
        {
            _act = act;
            FastAddProperty("prototype", prototype, false, false, false);
            DomConverter.DefineStatic(this, typeof(ClrType));
        }

        public override JsValue Call(JsValue thisObject, JsValue[] arguments)
        {
            throw new NotImplementedException();
        }

        public ObjectInstance Construct(JsValue[] arguments) => _act(arguments);

        public override bool HasInstance(JsValue v)
        {
            var clrObject = v.TryCast<ClrObject>();
            if (clrObject != null && clrObject.Target != null)
            {
                return clrObject.Target is ClrType;
            }

            return base.HasInstance(v);
        }
    }
}