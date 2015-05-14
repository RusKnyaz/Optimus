using System;
using System.IO;
using WebBrowser.Dom;
using WebBrowser.Environment;
using WebBrowser.ResourceProviders;
using WebBrowser.ScriptExecuting;

namespace WebBrowser
{
	public class Engine
    {
		readonly IResourceProvider _resourceProvider;
		private IScriptExecutor _scriptExecutor;

		public Document Document { get; private set; }
		public Console Console { get; private set; }
		public Window Window { get; private set; }

		internal Engine(IResourceProvider resourceProvider)
		{
			_resourceProvider = resourceProvider;
			Console = new Console();
			Window = new Window();
		}

		public Engine() : this(new ResourceProvider())
		{
			
		}

		public void OpenUrl(string path)
		{
			var uri = new Uri(path);
			var resource = _resourceProvider.GetResource(uri);

			if (resource.Type == ResourceTypes.Html)
			{
				Load(resource.Stream);
			}
		}

		public void Load(Stream stream)
		{
			Document = new Document(_resourceProvider);
			_scriptExecutor = new ScriptExecutor(this);

			
			var elements = DocumentBuilder.Build(stream);
			Document.Load(elements);

			Document.ResolveDelayedContent();

			Document.RunScripts(_scriptExecutor);
		}
    }
}
