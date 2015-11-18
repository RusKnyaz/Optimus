using System;
using System.IO;
using System.Linq;
using System.Text;
using WebBrowser.Dom;
using WebBrowser.Dom.Elements;
using WebBrowser.Environment;
using WebBrowser.ResourceProviders;
using WebBrowser.ScriptExecuting;

namespace WebBrowser
{
	public class Engine: IDisposable
	{
		private Document _document;
		public IResourceProvider ResourceProvider { get; private set; }
		internal IScriptExecutor ScriptExecutor { get; private set; }

		public DocumentScripting Scripting	{get; private set;}

		public Document Document
		{
			get { return _document; }
			private set
			{
				if (_document != null)
				{
					Document.OnNodeException -= OnNodeException;
					Document.OnFormSubmit -= OnFormSubmit;
				}

				if (Scripting != null)
				{
					Scripting.Dispose();
					Scripting = null;
				}

				_document = value;
				
				if (_document != null)
				{
					Scripting = new DocumentScripting (_document, ScriptExecutor, ResourceProvider);
					Document.OnNodeException += OnNodeException;
					Document.OnFormSubmit += OnFormSubmit;
				}

				if (DocumentChanged != null)
					DocumentChanged();
			}
		}

		private void OnNodeException(Node node, Exception exception)
		{
			Console.Log("Node event handler exception: " + exception.Message);
		}

		public Console Console { get; private set; }
		public Window Window { get; private set; }

		internal Engine(IResourceProvider resourceProvider)
		{
			ResourceProvider = resourceProvider;
			Console = new Console();
			Window = new Window(() => Document, this);
			ScriptExecutor = new ScriptExecutor(this);
			ScriptExecutor.OnException += ex => Console.Log("Unhandled exception in script: " + ex.Message);
			
		}

		/// <summary>
		/// todo: rewrite and complete the stuff
		/// </summary>
		/// <param name="form"></param>
		private async void OnFormSubmit(HtmlFormElement form)
		{
			if (string.IsNullOrEmpty(form.Action))
				return;
			//todo: we should consider the case when button clicked and the button have 'method' or other attributes.
			
			var dataElements = form.Elements.OfType<IFormElement>().Where(x => !string.IsNullOrEmpty(x.Name));

			var replaceSpaces = form.Method != "post" || form.Enctype != "multipart/form-data";
			
			var data = string.Join("&", dataElements.Select(x => 
				x.Name + "=" + (x.Value != null ? (replaceSpaces ? x.Value.Replace(' ', '+') : x.Value) : "")
				));

			var isGet = form.Method.ToLowerInvariant() == "get";

			var url = form.Action;

			if (isGet)
				url += "?" + data;

			if (form.Action != "about:blank")
			{
				var document = new Document(Window);

				HtmlIFrameElement targetFrame = null;
				if (!string.IsNullOrEmpty(form.Target) &&
					(targetFrame = Document.GetElementsByName(form.Target).FirstOrDefault() as HtmlIFrameElement) != null)
				{
					targetFrame.ContentDocument = document;
				}
				else
				{
					Document = document;
				}

				var request = ResourceProvider.CreateRequest(url);
				if (!isGet)
				{
					var httpRequest = request as HttpRequest;
					if (httpRequest != null)
					{
						//todo: use right encoding and enctype
						httpRequest.Data = Encoding.UTF8.GetBytes(data);
					}
				}

				var response = await ResourceProvider.GetResourceAsync(request);

				//what should we do if the frame is not found?
				if (response.Type == ResourceTypes.Html)
				{
					//todo: clear js runtime context
					DocumentBuilder.Build(document, response.Stream);
					document.Complete();
				}
			}
			//todo: handle 'about:blank'
		}

		public Engine() : this(new ResourceProvider()) { }

		public Uri Uri { get; private set; }

		public async void OpenUrl(string path)
		{
			ScriptExecutor.Clear();
			Document = new Document(Window);
			Uri = new Uri(path);
			ResourceProvider.Root = Uri.GetLeftPart(UriPartial.Path).TrimEnd('/');
			var resource = await ResourceProvider.GetResourceAsync(path);
			LoadFromResponse(resource);
		}

		private void LoadFromResponse(IResource resource)
		{
			if (resource.Type == ResourceTypes.Html)
			{
				var httpResponse = resource as HttpResponse;
				if (httpResponse != null && httpResponse.Uri != null)
					Uri = httpResponse.Uri;

				BuildDocument(resource.Stream);
			}
		}

		/// <summary>
		/// For tests
		/// </summary>
		/// <param name="stream"></param>
		public void Load(Stream stream)
		{
			ScriptExecutor.Clear();
			Document = new Document(Window);
			BuildDocument(stream);
		}

		private void BuildDocument(Stream stream)
		{
			//todo: fix protocol
			if(Uri == null)
				Uri = new Uri("http://localhost");

			//todo: clear js runtime context
			DocumentBuilder.Build(Document, stream);
			Document.Complete();
		}

		public event Action DocumentChanged;
		public void Dispose()
		{
		}
    }
}
