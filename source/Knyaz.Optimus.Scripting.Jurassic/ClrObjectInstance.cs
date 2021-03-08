using System;
using System.Linq;
using System.Reflection;
using Jurassic.Library;
using Knyaz.Optimus.ScriptExecuting;
using Knyaz.Optimus.Scripting.Jurassic.Tools;
using Knyaz.Optimus.Tools;

namespace Knyaz.Optimus.Scripting.Jurassic
{
    /// <summary>
    /// .net - javascript object adapter.
    /// </summary>
    class ClrObjectInstance : ObjectInstance
    {
        public readonly object Target;
        
        private readonly ClrTypeConverter _ctx;
        private readonly PropertyInfo[] _indexProperties;
        private readonly Array _array;
        
        public ClrObjectInstance(ClrTypeConverter ctx, object obj, ObjectInstance prototype) : base(prototype)
        {
            var type = obj.GetType();
            this[ctx.Engine.Symbol.ToStringTag] =type.GetCustomAttribute<JsNameAttribute>()?.Name ?? type.Name;
            
            _ctx = ctx;
            Target = obj;
            
            if (Target is Array array)
            {
                _array = array;
            }
            else
            {
                _indexProperties = Target.GetType()
                    .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                    .Where(x => x.GetIndexParameters().Length == 1)
                    .ToArray();
            }
        }

        protected override object GetMissingPropertyValue(object key)
        {
            if (_array != null)
            {
                switch (key)
                {
                    case int index: return _ctx.ConvertToJs(_array.GetValue(index));
                    case string str:
                        return int.TryParse(str, out var parsedIndex) && parsedIndex >=0 && parsedIndex < _array.Length
                            ? _ctx.ConvertToJs(_array.GetValue(parsedIndex))
                            : base.GetMissingPropertyValue(key);
                    default:
                        return base.GetMissingPropertyValue(key);
                }
            }
            
            var clrKey = _ctx.ConvertToClr(key);
            
            var pi =
                clrKey == null
                    ? _indexProperties.FirstOrDefault(x => x.GetIndexParameters()[0].ParameterType.CanBeNull())
                    : _indexProperties.FirstOrDefault(x => x.GetIndexParameters()[0].ParameterType.IsInstanceOfType(clrKey));

            //converts string key to integer. 
            if (pi == null && key is string stringKey)
            {
                pi = _indexProperties.FirstOrDefault(x => x.GetIndexParameters()[0].ParameterType == typeof(int));
                if (pi != null)
                {
                    if (int.TryParse(stringKey, out var idx))
                        clrKey = idx;
                    else
                        return base.GetMissingPropertyValue(key);
                }
                else
                {
                    pi = _indexProperties.FirstOrDefault(x => x.GetIndexParameters()[0].ParameterType == typeof(ulong));
                    if (ulong.TryParse(stringKey, out var idx))
                        clrKey = idx;
                    else
                        return base.GetMissingPropertyValue(key);
                }
            }
            
            return pi != null
                ? _ctx.ConvertToJs(pi.GetValue(Target, new[] {clrKey}))
                : base.GetMissingPropertyValue(key);
        }
    }
}