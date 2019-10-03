using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Knyaz.Optimus.Tools
{
    internal static class TypeExtensions
    {
        public static bool CanBeNull(this Type type) =>
            !type.IsValueType || (Nullable.GetUnderlyingType(type) != null);
        
        public static IEnumerable<InterfaceMapping> GetAllInterfaceMaps(this Type aType) =>
            aType.GetTypeInfo()
                .ImplementedInterfaces
                .Select(ii => aType.GetInterfaceMap(ii));

        public static Type[] GetInterfacesForMethod(this MethodInfo mi) =>
            mi.ReflectedType
                .GetAllInterfaceMaps()
                .Where(im => im.TargetMethods.Any(tm => tm == mi))
                .Select(im => im.InterfaceType)
                .ToArray();

        public static ILookup<MethodInfo, Type> GetMethodsForInterfaces(this Type aType) =>
            aType.GetAllInterfaceMaps()
                .SelectMany(im => im.TargetMethods.Select(tm => new { im.TargetType, im.InterfaceType, tm }))
                .ToLookup(imtm => imtm.tm, imtm => imtm.InterfaceType);

        public static IEnumerable<MethodInfo> GetInterfaceDeclarationsForMethod(this MethodInfo mi) =>
            mi.ReflectedType
                .GetAllInterfaceMaps()
                .SelectMany(map => Enumerable.Range(0, map.TargetMethods.Length)
                    .Where(n => map.TargetMethods[n] == mi)
                    .Select(n => map.InterfaceMethods[n]));
    }
}