using System.IO;
using NUnit.Framework;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Knyaz.Optimus.JsTests;
using Knyaz.Optimus.ResourceProviders;
using Knyaz.Optimus.ScriptExecuting.Jint;
using Knyaz.Optimus.Scripting.Jurassic;

namespace Knyaz.Optimus.Tests.JsTests
{
    class JsTestsRunner
    {
	    class TestingResourceProvider : IResourceProvider
	    {
		    class Resource : IResource
		    {
			    public string Type { get; set; }
			    public Stream Stream { get; set; }
		    }
		    
		    public Task<IResource> SendRequestAsync(Request request)
		    {
			    var data = GetResponseData(request);
			    return Task.Run(() => (IResource)new Resource {
				    Type = request.Url.ToString().EndsWith(".js") ? "text/javascript" : "text/html",
				    Stream = new MemoryStream(Encoding.UTF8.GetBytes(data))
			    });
		    }

		    private string GetResponseData(Request request)
		    {
			    var url = request.Url.ToString().TrimEnd('/');

			    if (!url.StartsWith("http://test"))
				    return null;

			    if (!url.EndsWith(".js"))
			    {
				    var testName = url.Substring("http://test/".Length);
				    return $"<html><script src='/base.js'/><script src='/{testName}.js'/></html>";
			    }

			    var resName = url.Substring("http://test/".Length);
			    return R.GetString("Knyaz.Optimus.JsTests.Tests." + resName);
		    }
	    }
	    
	    public static void Run(JsEngines engineType, string testName, [CallerMemberName] string fixture = null)
		{
			var builder = new EngineBuilder().SetResourceProvider(new TestingResourceProvider());

			switch (engineType)
			{
				case JsEngines.Jurassic:builder.UseJurassic();break;
				case JsEngines.Jint:builder.UseJint();break;
			}
				
			var engine = builder.Build();

			engine.OpenUrl($"http://test/{fixture}").Wait();

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
