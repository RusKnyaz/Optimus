using System;
using System.IO;
using WebBrowser.Dom;
using WebBrowser.ScriptExecuting;

namespace WebBrowser
{
	public class Engine
    {
		IResourceProvider _resourceProvider;
		private IScriptExecutor _scriptExecutor;

		Document _document;

		public Document Document { get { return _document; } }

		public Engine()
		{
			_resourceProvider = new ResourceProvider();
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
			_document = new Document(_resourceProvider);
			_scriptExecutor = new ScriptExecutor(this);

			
			var elements = DocumentBuilder.Build(stream);
			_document.Load(elements);

			_document.ResolveDelayedContent();

			_document.RunScripts(_scriptExecutor);
		}
    }
}
