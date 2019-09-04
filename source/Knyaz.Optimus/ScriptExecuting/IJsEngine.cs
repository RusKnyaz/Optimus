using System;
using Jint.Native;

namespace Knyaz.Optimus.ScriptExecuting
{
    internal interface IJsEngine
    {
        void Execute(string code);
        void AddGlobalType(string jsTypeName, Type type);
        void AddGlobalGetter(string name, Func<object> getter);
        
        void AddGlobalAct(string name, Action<object[]> action);

        void AddGlobalFunc(string name, Func<object[], object> action);
        
        /// <summary>
        /// Registers new global type.
        /// </summary>
        /// <param name="name">The type name available in java script.</param>
        /// <param name="func">The function called to construct new object.</param>
        /// <typeparam name="T">The type of the object.</typeparam>
        void AddGlobalType<T>(string name, Func<object[], T> func);

        object ParseJson(string json);
        object Evaluate(string code);
    }

    internal struct Undefined
    {
        public static Undefined Instance;
    }
}