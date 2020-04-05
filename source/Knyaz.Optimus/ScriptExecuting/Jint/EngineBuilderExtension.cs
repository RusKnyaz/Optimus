using Knyaz.Optimus.ResourceProviders;

namespace Knyaz.Optimus.ScriptExecuting.Jint
{
    public static class EngineBuilderExtension
    {
        /// <summary> Setup JINT javascript engine. </summary>
        public static EngineBuilder UseJint(this EngineBuilder builder) =>
            builder.JsScriptExecutor(ctx => new JintJsScriptExecutor(ctx.Window, ctx.CreateXhr));
    }
}