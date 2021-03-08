using System;
using System.Linq;
using System.Reflection;

namespace Knyaz.Optimus.Scripting.Jurassic
{
	static class ParameterInfoExtensions
	{
		public static bool IsAppropriate(this ParameterInfo[] pars, object[] argumentValues)
		{
			var isParamArray = pars.Length > 0 && pars.Last().GetCustomAttribute<ParamArrayAttribute>() != null;
	        
			var checkLength = isParamArray ? pars.Length - 1 : pars.Length;
	        
			for (var idx = 0; idx < checkLength; idx++)
			{
				var par = pars[idx];

				if (idx >= argumentValues.Length && !par.HasDefaultValue)
				{
					return false;
				}

				if (!ClrTypeConverter.CanConvert(argumentValues[idx], par.ParameterType))
					return false;
			}
            
			if (isParamArray && argumentValues.Length > checkLength)
			{
				var elementType = pars.Last().ParameterType.GetElementType();
				return argumentValues.Skip(checkLength).All(x => ClrTypeConverter.CanConvert(x, elementType));
			}

			return true;
		}

		public static object GetDefaultValue(this ParameterInfo par)
		{
			if (par.HasDefaultValue)
				return par.DefaultValue;

			if (par.ParameterType.IsValueType)
				return Activator.CreateInstance(par.ParameterType);

			return null;
		}
	}
}