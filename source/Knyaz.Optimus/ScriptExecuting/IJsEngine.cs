using System;

namespace Knyaz.Optimus.ScriptExecuting
{
    internal interface IJsEngine
    {
        void Execute(string code);
        void AddGlobalType(Type type);
        void AddGlobalGetter(string name, Func<object> getter);
        
        void AddGlobalAct(string name, Action<object[]> action);

        void AddGlobalFunc(string name, Func<object[], object> action);

        void SetGlobal(object global); 
        
        /// <summary>
        /// Registers new global type.
        /// </summary>
        /// <param name="name">The type name available in java script.</param>
        /// <param name="func">The function called to construct new object.</param>
        void AddGlobalType(string name, Type type, Func<object[], object> func);
        
        void AddGlobalType(Type type, string name, Type[] argumentsTypes, Func<object[], object> func);

        object ParseJson(string json);
        object Evaluate(string code);
    }
}