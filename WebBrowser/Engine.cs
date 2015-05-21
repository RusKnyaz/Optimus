using System;
using System.IO;
using WebBrowser.Dom;
using WebBrowser.Environment;
using WebBrowser.Html;
using WebBrowser.ResourceProviders;
using WebBrowser.ScriptExecuting;

namespace WebBrowser
{
	public class Engine
    {
		public IResourceProvider ResourceProvider { get; private set; }
		internal IScriptExecutor ScriptExecutor { get; private set; }

		public Document Document { get; private set; }
		public Console Console { get; private set; }
		public Window Window { get; private set; }
		

		internal Engine(IResourceProvider resourceProvider)
		{
			ResourceProvider = resourceProvider;
			Console = new Console();
			Window = new Window();
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
			Document = new Document(ResourceProvider);
			//todo: clear js runtime context
			
			var elements = DocumentBuilder.Build(Document, stream);
			Document.Load(elements);

			Document.ResolveDelayedContent();

			Document.RunScripts(ScriptExecutor);
		}
    }
}
