using Jint;
using Jint.Native;
using Jint.Native.Object;
using Jint.Runtime;

namespace Knyaz.Optimus.ScriptExecuting
{
	static class JsValueExtensions
	{
		internal static ObjectInstance ToBooleanOrObject(this JsValue x, out bool boolVal)
		{
			if (x.IsObject())
			{
				boolVal = false;
				return x.AsObject();
			}

			switch (x.Type)
			{
				case Types.Boolean:
					boolVal = x.AsBoolean();
					break;
				case Types.Number:
					boolVal = x.AsNumber() != 0;
					break;
				default:
					boolVal = !x.IsNull() && !x.IsUndefined();
					break;
			}

			return null;
		}
	}
}