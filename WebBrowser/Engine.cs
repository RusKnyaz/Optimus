﻿using System;
using System.IO;
using System.Threading;
using WebBrowser.Dom;
using WebBrowser.Environment;
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
			Window = new Window(Context, this);
			ScriptExecutor = new ScriptExecutor(this);
		}

		public Engine() : this(new ResourceProvider()) { }

		public Uri Uri { get; private set; }

		public void OpenUrl(string path)
		{
			ScriptExecutor.Clear();
			Uri = new Uri(path);
			ResourceProvider.Root = Uri.GetLeftPart(UriPartial.Path);
			var resource = ResourceProvider.GetResource(path);
			
			if (resource.Type == ResourceTypes.Html)
			{
				Load(resource.Stream);
			}
		}

		public void Load(Stream stream)
		{
			Document = new Document(ResourceProvider, Context, ScriptExecutor);
			//todo: clear js runtime context
			
			var elements = DocumentBuilder.Build(Document, stream);
			Document.Load(elements);

		}
    }
}
