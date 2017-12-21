using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Knyaz.Optimus.Dom;
using Knyaz.Optimus.Dom.Css;
using Knyaz.Optimus.Dom.Elements;
using Knyaz.Optimus.Environment;
using Knyaz.Optimus.Html;
using Knyaz.Optimus.ResourceProviders;
using Knyaz.Optimus.ScriptExecuting;
using Knyaz.Optimus.Tools;

namespace Knyaz.Optimus
{
	/// <summary>
	/// The web engine that allows you you to load html pages, execute JavaScript and get live DOM.
	/// </summary>
	public class Engine: IEngine, IDisposable
	{
		private Document _document;
		private Uri _uri;
		
		/// <summary>
		/// Gets the Engine's resource provider - entity through which the engine gets the html pages, js files, images and etc.
		/// </summary>
		public IResourceProvider ResourceProvider { get; private set; }
		
		/// <summary>
		/// Gets the current Script execution engine. Can be used to execute custom script or get some global values.
		/// </summary>
		public IScriptExecutor ScriptExecutor { get; private set; }
		
		/// <summary>
		/// Glues Document and ScriptExecutor.
		/// </summary>
		public DocumentScripting Scripting	{get; private set;}
		internal DocumentStyling Styling { get; private set; }

		/// <summary>
		/// Gets the browser's console object.
		/// </summary>
		public Console Console { get; private set; }
		
		/// <summary>
		/// Gets the current Window object.
		/// </summary>
   		public Window Window { get; private set; }

		/// <summary>
		/// Creates new Engine instance with default settings (Js enabled, css disabled).
		/// </summary>
		public Engine() : this(new PredictedResourceProvider(new ResourceProvider())) { }

   		public Engine(IResourceProvider resourceProvider)
   		{
   			ResourceProvider = resourceProvider;
   			Console = new Console();
   			Window = new Window(() => Document, this);
   			ScriptExecutor = new ScriptExecutor(this);
   			ScriptExecutor.OnException += ex => Console.Log("Unhandled exception in script: " + ex.Message);
   		}

		/// <summary>
		/// Gets the active <see cref="Document"/> if exists (OpenUrl must be called before).
		/// </summary>
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

				if (Styling != null)
				{
					Styling.Dispose();
					Styling = null;
				}

				_document = value;
				
				if (_document != null)
				{
					Scripting = new DocumentScripting (_document, ScriptExecutor, ResourceProvider);
					Document.OnNodeException += OnNodeException;
					Document.OnFormSubmit += OnFormSubmit;

					if (_computedStylesEnabled)
					{
						EnableDocumentStyling();
					}
				}

				DocumentChanged?.Invoke();
			}
		}

		private void OnNodeException(Node node, Exception exception) =>
			Console.Log("Node event handler exception: " + exception.Message);


		// todo: rewrite and complete the stuff
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

				var response = await ResourceProvider.SendRequestAsync(request);

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

		/// <summary>
		/// Gets the current Uri of the document.
		/// </summary>
		public Uri Uri
		{
			get => _uri;
			internal set
			{
				_uri = value;
				OnUriChanged?.Invoke();
			}
		}

		/// <summary>
		/// Called on <see cref="Engine.Uri"/> changed.
		/// </summary>
		public event Action OnUriChanged;
		
		/// <summary>
		/// Called when new Document created and assigned to <see cref="Engine.Document"/> property.
		/// </summary>
		public event Action DocumentChanged;

		/// <summary>
		/// Creates new <see cref="Document"/> and loads it from specified path (http or file).
		/// </summary>
		/// <param name="path">The string which represents Uri of the document to be loaded.</param>
		public async Task OpenUrl(string path)
		{
			//todo: stop unfinished ajax requests or drop their results
			Window.Timers.ClearAll();
			ScriptExecutor.Clear();
			Document = new Document(Window);
			var uri = UriHelper.IsAbsolete(path) ? new Uri(path) : new Uri(Uri, path);
			Window.History.PushState(null, null, uri.AbsoluteUri);
			
			
			ResourceProvider.Root = GetRoot(Uri);

			var response = await ResourceProvider.GetResourceAsync(Uri.ToString().TrimEnd('/'));
			LoadFromResponse(response);
		}

		private string GetRoot(Uri uri)
		{
			var root =Uri.GetLeftPart(UriPartial.Path);
			var ur = new Uri(root);
			if (ur.PathAndQuery != null && !ur.PathAndQuery.Contains('.') && ur.PathAndQuery.Last() != '/')
				return root + "/";

			return root;
		}

		private void LoadFromResponse(IResource resource)
		{
			if (resource.Type == null || !resource.Type.StartsWith(ResourceTypes.Html))
				throw new Exception("Invalid resource type: " + (resource.Type ?? "<null>"));

			var httpResponse = resource as HttpResponse;
			if (httpResponse != null && httpResponse.Uri != null)
				Uri = httpResponse.Uri;

			BuildDocument(resource.Stream);
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

			var html = HtmlParser.Parse(stream).ToList();

			if (ResourceProvider is PredictedResourceProvider resourceProvider)
			{
				foreach (var script in html.OfType<Html.IHtmlElement>()
					.Flat(x => x.Children.OfType<Html.IHtmlElement>())
					.Where(x => x.Name == "script" && x.Attributes.ContainsKey("src"))
					.Select(x => x.Attributes["src"])
					.Where(x => !string.IsNullOrEmpty(x))
					)
				{
					resourceProvider.Preload(script);
				}

				if (ComputedStylesEnabled)
				{
					foreach (var script in html.OfType<Html.IHtmlElement>()
					.Flat(x => x.Children.OfType<Html.IHtmlElement>())
					.Where(x => x.Name == "link" && x.Attributes.ContainsKey("href") &&
					            (!x.Attributes.ContainsKey("type") || x.Attributes["type"] == "text/css"))
					.Select(x => x.Attributes["href"])
					.Where(x => !string.IsNullOrEmpty(x)))
					{
						resourceProvider.Preload(script);
					}
				}
			}

			DocumentBuilder.Build(Document, html);
			Document.Complete();
		}

		public void Dispose() => Window.Dispose();

		private bool _computedStylesEnabled;
		
		/// <summary>
		/// Enables or disables the css loading and styles evaluation.
		/// </summary>
		public bool ComputedStylesEnabled
		{
			get => _computedStylesEnabled;
			set
			{
				if (_computedStylesEnabled == value)
					return;

				_computedStylesEnabled = value;
				if (_document == null) 
					return;

				if(_computedStylesEnabled)
					EnableDocumentStyling();
				else
				{
					Styling.Dispose();
					Styling = null;
				}
			}
		}

		private void EnableDocumentStyling()
		{
			Styling = new DocumentStyling(_document, ResourceProvider);
			Styling.LoadDefaultStyles();
		}

		/// <summary>
		/// Gets the current media settings (used in computed styles evaluation).
		/// </summary>
		public readonly MediaSettings CurrentMedia  = new MediaSettings {Device = "screen", Width = 1024};
	}

	public interface IEngine
	{
		Uri Uri {get;}
		Task OpenUrl(string url);
	}
}
