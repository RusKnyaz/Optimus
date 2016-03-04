using System;
using Jint.Native;
using Jint.Native.Function;
using Jint.Native.Object;

namespace WebBrowser.ScriptExecuting
{
	internal class ClrFuncCtor : FunctionInstance, IConstructor
	{
		private readonly Func<JsValue[], ObjectInstance> _act;

		public ClrFuncCtor(Jint.Engine engine, Func<JsValue[], ObjectInstance> act) : base(engine, null, null, false)
		{
			_act = act;
		}

		public override JsValue Call(JsValue thisObject, JsValue[] arguments)
		{
			throw new NotImplementedException();
		}

		public ObjectInstance Construct(JsValue[] arguments)
		{
			return _act(arguments);
		}
	}
}
