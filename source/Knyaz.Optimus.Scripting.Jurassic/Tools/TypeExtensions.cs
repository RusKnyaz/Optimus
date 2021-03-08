using System;

namespace Knyaz.Optimus.Scripting.Jurassic.Tools
{
	internal static class TypeExtensions
	{
		public static bool CanBeNull(this Type type) =>
			!type.IsValueType || (Nullable.GetUnderlyingType(type) != null);

	}
}