using Knyaz.Optimus.ResourceProviders;
using Knyaz.Optimus.ScriptExecuting.Jint;

namespace Knyaz.Optimus.Tests.TestingTools
{
	/// <summary> Helper methods to build <see cref="Engine"/> for tests. </summary>
	internal static class TestingEngine
	{
		/// <summary> Configures engine with Jint. </summary>
		public static Engine BuildJint() =>
			EngineBuilder.New().UseJint().Build();
		
		/// <summary> Configures engine with Jint and specified resource provider. </summary>
		public static Engine BuildJint(IResourceProvider resourceProvider)
			=> EngineBuilder.New().UseJint().SetResourceProvider(resourceProvider).Build();
		
		public static Engine BuildJintCss(IResourceProvider resourceProvider)
			=> EngineBuilder.New().UseJint().SetResourceProvider(resourceProvider).EnableCss().Build();
		
		public static Engine BuildJintCss()
			=> EngineBuilder.New().UseJint().EnableCss().Build();
	}
}