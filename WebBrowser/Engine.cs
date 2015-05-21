using System;
using System.IO;
using System.Threading;
using WebBrowser.Dom;
using WebBrowser.Environment;
using WebBrowser.Html;
using WebBrowser.ResourceProviders;
using WebBrowser.ScriptExecuting;
using WebBrowser.Tests.Dom;

namespace WebBrowser
{
	public class Engine
    {
		public IResourceProvider ResourceProvider { get; private set; }
		internal IScriptExecutor ScriptExecutor { get; private set; }

		public Document Document { get; private set; }
		public Console Console { get; private set; }
		public Window Window { get; private set; }

		public SynchronizationContext Context { get; private set; }

		internal Engine(IResourceProvider resourceProvider)
		{
			Context = new SignleThreadSynchronizationContext();

			ResourceProvider = resourceProvider;
			Console = new Console();
			Window = new Window(Context);
			ScriptExecutor = new ScriptExecutor(this);
		}

		public Engine() : this(new ResourceProvider()) { }

		public void OpenUrl(string path)
		{
			ResourceProvider.Root = (new Uri(path)).GetLeftPart(UriPartial.Path);
			var resource = ResourceProvider.GetResource(path);
			
			if (resource.Type == ResourceTypes.Html)
			{
				Load(resource.Stream);
			}
		}

		public void Load(Stream stream)
		{
			Document = new Document(ResourceProvider, Context);
			//todo: clear js runtime context
			
			var elements = DocumentBuilder.Build(Document, stream);
			Document.Load(elements);

			Document.ResolveDelayedContent();

			Document.RunScripts(ScriptExecutor);
		}
    }
}
