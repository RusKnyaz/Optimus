using Knyaz.Optimus.ResourceProviders;
using Knyaz.Optimus.ScriptExecuting.Jint;
using Knyaz.Optimus.Scripting.Jurassic;
using NUnit.Framework;

namespace Knyaz.Optimus.Tests
{
    public static class EngineBuilderTests
    {
        [Test] public static void CreateNoScriptEngine() =>
            Assert.IsNull(EngineBuilder.New().Build().ScriptExecutor);

        [Test] public static void CreateEngineWithJint() =>
            Assert.IsNotNull(EngineBuilder.New().UseJint().Build().ScriptExecutor);

        [Test] public static void CreateEngineWithJurassic() =>
            Assert.IsNotNull(EngineBuilder.New().UseJurassic().Build().ScriptExecutor);
    }
}