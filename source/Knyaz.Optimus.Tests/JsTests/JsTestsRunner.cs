using Knyaz.Optimus.Tests.Resources;
using NUnit.Framework;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Knyaz.Optimus.ResourceProviders;
using Knyaz.Optimus.ScriptExecuting.Jint;
using Knyaz.Optimus.Scripting.Jurassic;
using Knyaz.Optimus.Tests.ScriptExecuting;

namespace Knyaz.Optimus.Tests.JsTests
{
    class JsTestsRunner
    {
	    public static void Run(JsEngines engineType, string testName, [CallerMemberName] string fixture = null)
		{
			var testJs = R.GetString("Knyaz.Optimus.Tests.Resources.JsTests."+fixture+".js");

			var resources = Mocks.ResourceProvider("http://test",
					"<html><script>" + R.JsTestsBase + "</script>" + "<script>" + testJs + "</script></html>")
				.Resource("http://test/sample.js", R.GetString("Knyaz.Optimus.Tests.Resources.JsTests.sample.js"));

			var builder = new EngineBuilder()
				.SetResourceProvider(resources);

			switch (engineType)
			{
				case JsEngines.Jurassic:builder.UseJurassic();break;
				case JsEngines.Jint:builder.UseJint();break;
			}
				
			var engine = builder.Build();

			engine.OpenUrl("http://test").Wait();

			engine.Console.OnLog += o => {
				Debug.WriteLine(o.ToString());
			};

			object res;
			lock (engine.Document)
			{
				res = engine.ScriptExecutor.Evaluate("text/javascript",
					@"Run('" + fixture + "', '" + testName + "');");
			}

			if (res != null)
				Assert.Fail(res.ToString());
		}
    }
}
