using Knyaz.Optimus.Dom.Interfaces;
using Knyaz.Optimus.ResourceProviders;
using Knyaz.Optimus.ScriptExecuting.Jint;

namespace Knyaz.Optimus.Tests.TestingTools
{
	/// <summary> Helper methods to build <see cref="Engine"/> for tests. </summary>
	internal static class TestingEngine
	{
		public static Engine Build(string html)
		{
			var resources = Mocks.ResourceProvider("http://localhost", html);
			var engine = TestingEngine.BuildJintCss(resources);
			return engine;
		}
		
		/// <summary> Configures engine with Jint. </summary>
		public static Engine BuildJint() =>
			EngineBuilder.New().UseJint().Build();
		
		public static Engine BuildJint(IConsole console) =>
			EngineBuilder.New().UseJint().Window(w => w.SetConsole(console)).Build();
		
		/// <summary> Configures engine with Jint and specified resource provider. </summary>
		public static Engine BuildJint(IResourceProvider resourceProvider)
			=> EngineBuilder.New().UseJint().SetResourceProvider(resourceProvider).Build();
		
		public static Engine BuildJint(IResourceProvider resourceProvider, IConsole console)
			=> EngineBuilder.New().UseJint().SetResourceProvider(resourceProvider)
				.Window(w => w.SetConsole(console))
				.Build();
		
		public static Engine BuildJintCss(IResourceProvider resourceProvider)
			=> EngineBuilder.New().UseJint().SetResourceProvider(resourceProvider).EnableCss().Build();
		
		public static Engine BuildJintCss()
			=> EngineBuilder.New().UseJint().EnableCss().Build();
	}
}