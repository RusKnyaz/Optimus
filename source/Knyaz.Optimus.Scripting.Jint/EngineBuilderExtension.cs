using Knyaz.Optimus.ResourceProviders;
using Knyaz.Optimus.Scripting.Jint.Internal;

namespace Knyaz.Optimus.ScriptExecuting.Jint
{
    public static class EngineBuilderExtension
    {
        /// <summary> Setup JINT javascript engine. </summary>
        public static EngineBuilder UseJint(this EngineBuilder builder) => 
            builder.JsScriptExecutor(JintFactory.Create);
    }
}