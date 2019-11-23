using System.Reflection;

namespace Knyaz.Optimus.ScriptExecuting
{
	public static class Extensions
	{
		public static string GetName(this MemberInfo memberInfo)
		{
			if (typeof(System.Collections.ICollection).IsAssignableFrom(memberInfo.ReflectedType) &&
			    memberInfo.Name == "Count")
				return "length";
            
			var jsNameAttribute = memberInfo.GetCustomAttribute<JsNameAttribute>();
			var name = jsNameAttribute?.Name ?? memberInfo.Name;
			return ToCamel(name);
		}
		
		private static string ToCamel(string name)
		{
			var camelChar = char.ToLower(name[0]);
			if (name.Length > 1)
			{
				var chars = name.ToCharArray();
				chars[0] = camelChar;
				return new string(chars);
			}

			return camelChar.ToString();
		}
	}
}