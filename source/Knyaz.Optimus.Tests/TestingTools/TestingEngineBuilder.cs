using System.IO;
using System.Reflection;
using Knyaz.Optimus.Dom.Css;
using Knyaz.Optimus.Dom.Interfaces;
using Knyaz.Optimus.ResourceProviders;
using Knyaz.Optimus.ScriptExecuting.Jint;

namespace Knyaz.Optimus.Tests.TestingTools
{
	/// <summary> Helper methods to build <see cref="Engine"/> for tests. </summary>
	internal static class TestingEngine
	{
		public static Engine Build(string url, string html) => 
			Build(Mocks.ResourceProvider(url, html));

		public static Engine BuildJint(string html)
		{
			var resources = Mocks.ResourceProvider("http://localhost", html);
			var engine = TestingEngine.BuildJintCss(resources);
			return engine;
		}
		
		public static Engine Build(IResourceProvider resourceProvider)
			=> EngineBuilder.New().SetResourceProvider(resourceProvider).Build();
		
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
			=> EngineBuilder.New().UseJint().SetResourceProvider(resourceProvider)
				.EnableCssMoz()
				.Build();
		
		public static Engine BuildJintCss(IResourceProvider resourceProvider, IConsole console)
			=> EngineBuilder.New().UseJint().SetResourceProvider(resourceProvider)
				.Window(w => w.SetConsole(console))
				.EnableCssMoz()
				.Build();
		
		public static Engine BuildJintCss()
			=> EngineBuilder.New().UseJint().EnableCssMoz().Build();

		private static EngineBuilder EnableCssMoz(this EngineBuilder builder)
		{
			var stream = Assembly.GetExecutingAssembly()
				.GetManifestResourceStream("Knyaz.Optimus.Tests.Resources.moz_default.css"); 
			var defaultCss = StyleSheetBuilder.CreateStyleSheet(new StreamReader(stream));
			builder.EnableCss(config => config.UserAgentStyleSheet = defaultCss);
			return builder;
		}
	}
}