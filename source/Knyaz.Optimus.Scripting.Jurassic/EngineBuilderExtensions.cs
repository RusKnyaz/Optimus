using Knyaz.Optimus.ResourceProviders;

namespace Knyaz.Optimus.Scripting.Jurassic
{
    /// <summary>
    /// Extends <see cref="EngineBuilder"/> class with the methods that configures Jurassic scripting engine. 
    /// </summary>
    public static class EngineBuilderExtensions
    {
        /// <summary> Sets the Jurassic to be JavaScript engine in the Optimus.</summary>
        public static EngineBuilder UseJurassic(this EngineBuilder builder) => 
            builder.ScriptExecutor(ctx => new ScriptExecutor(ctx.Window, ctx.CreateXhr));
    }
}