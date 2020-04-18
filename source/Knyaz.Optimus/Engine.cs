using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Knyaz.Optimus.Dom;
using Knyaz.Optimus.Dom.Css;
using Knyaz.Optimus.Environment;
using Knyaz.Optimus.Html;
using Knyaz.Optimus.ResourceProviders;
using Knyaz.Optimus.ScriptExecuting;
using Knyaz.Optimus.Tools;
using HtmlElement = Knyaz.Optimus.Html.HtmlElement;

namespace Knyaz.Optimus
{
	/// <summary>
	/// The web engine that allows you you to load html pages, execute JavaScript and get live DOM.
	/// </summary>
	public partial class Engine: IDisposable
	{
		private Document _document;
		private Uri _uri;
		internal readonly LinkProvider LinkProvider = new LinkProvider();
		
		/// <summary>
		/// Gets the Engine's resource provider - entity through which the engine gets the html pages, js files, images and etc.
		/// </summary>
		public IResourceProvider ResourceProvider { get; }

		internal IPredictedResourceProvider _predictedResourceProvider;

		/// <summary>
		/// Gets the current Script execution engine. Can be used to execute custom script or get some global values.
		/// </summary>
		public IScriptExecutor ScriptExecutor
		{
			get => _scriptExecutor;
			set
			{
				if(_scriptExecutor != null)
					throw new InvalidOperationException("ScriptExecutor already has been set.");
				
				_scriptExecutor = value;
			}
		}

		/// <summary>
		/// Glues Document and ScriptExecutor.
		/// </summary>
		public DocumentScripting Scripting	{get; private set;}
		internal DocumentStyling Styling { get; private set; }

		/// <summary>
		/// Returns the cookie container that used to create requests.
		/// </summary>
		public CookieContainer CookieContainer => _cookieContainer; 

		/// <summary>
		/// Gets the current Window object.
		/// </summary>
		public Window Window { get; }

		readonly CookieContainer _cookieContainer;

		internal Engine(
			IResourceProvider resourceProvider, 
			Window window,
			IScriptExecutor scriptExecutor)
		{
			ResourceProvider = resourceProvider ?? throw new ArgumentNullException();
			
			_predictedResourceProvider = resourceProvider as IPredictedResourceProvider;
			
			_cookieContainer = new CookieContainer();

			Window = window ?? throw new ArgumentNullException(nameof(window));
			Window.Engine = this;

			if (scriptExecutor != null)
				ScriptExecutor = scriptExecutor; 
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
					if (_scriptExecutor != null)
					{
						Scripting = new DocumentScripting(_document, ScriptExecutor,
							s => ResourceProvider.SendRequestAsync(CreateRequest(s)));
					}

					Document.OnFormSubmit += OnFormSubmit;
					
					Document.CookieContainer = _cookieContainer;

					if (_computedStylesEnabled)
					{
						EnableDocumentStyling();
					}

					Document.GetImage = async url =>
					{
						var request = CreateRequest(url);
						var response = await ResourceProvider.SendRequestAsync(request);
						if(response is HttpResponse httpResponse && httpResponse.StatusCode != HttpStatusCode.OK)
							throw new Exception("Resource not found");
							
						return new Image(response.Type, response.Stream);
					};
				}

				DocumentChanged?.Invoke();
			}
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
		public async Task<Page> OpenUrl(string path) => await OpenUrl(path, true);
		
		public async Task<Page> OpenUrl(string path, bool clearScript)
		{
			//todo: stop unfinished ajax requests or drop their results
			Window.Timers.ClearAll();
			if(clearScript)
				ScriptExecutor?.Clear();
			var uri = UriHelper.IsAbsolute(path) ? new Uri(path) : new Uri(Uri, path);
			Window.History.PushState(null, null, uri.AbsoluteUri);
			
			Document = new Document(Window);
			LinkProvider.Root = Uri.GetRoot();
			var response = await ResourceProvider.SendRequestAsync(CreateRequest(path));
			
			LoadFromResponse(Document, response);

			return response is HttpResponse httpResponse
				? new HttpPage(Document, httpResponse.StatusCode)
				: new Page(Document);
		}

		internal Request CreateRequest(string uri, string method = "GET")
		{
			var req = new Request(method, LinkProvider.MakeUri(uri));
			req.Headers["User-Agent"] = Window.Navigator.UserAgent;
			req.Cookies = _cookieContainer;
			return req;
		}

		private void LoadFromResponse(Document document, IResource resource)
		{
			if (resource.Type == null || !resource.Type.StartsWith(ResourceTypes.Html))
				throw new Exception("Invalid resource type: " + (resource.Type ?? "<null>"));

			if (resource is HttpResponse httpResponse && httpResponse.Uri != null)
			{
				Uri = new Uri(httpResponse.Uri+Uri.Fragment);
			}
				

			BuildDocument(document, resource.Stream);
		}

		private void BuildDocument(Document document, Stream stream)
		{
			//todo: fix protocol
			if(Uri == null)
				Uri = new Uri("http://localhost");

			//todo: clear js runtime context

			var html = HtmlParser.Parse(stream).ToList();

			if (_predictedResourceProvider != null)
			{
				foreach (var src in html.OfType<HtmlElement>()
					.Flat(x => x.Children.OfType<HtmlElement>())
					.Where(x => x.Name == "script" && x.Attributes.ContainsKey("src"))
					.Select(x => x.Attributes["src"])
					.Where(x => !string.IsNullOrEmpty(x))
					)
				{
					_predictedResourceProvider.Preload(CreateRequest(src));
				}

				if (ComputedStylesEnabled)
				{
					foreach (var src in html.OfType<HtmlElement>()
					.Flat(x => x.Children.OfType<HtmlElement>())
					.Where(x => x.Name == "link" && x.Attributes.ContainsKey("href") &&
					            (!x.Attributes.ContainsKey("type") || x.Attributes["type"] == "text/css"))
					.Select(x => x.Attributes["href"])
					.Where(x => !string.IsNullOrEmpty(x)))
					{
						_predictedResourceProvider.Preload(CreateRequest(src));
					}
				}
			}

			DocumentBuilder.Build(document, html);
			document.Complete();
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
			Styling = new DocumentStyling(_document, s => ResourceProvider.SendRequestAsync(CreateRequest(s)));
			Styling.LoadDefaultStyles();
		}

		/// <summary>
		/// Gets the current media settings (used in computed styles evaluation).
		/// </summary>
		public readonly MediaSettings CurrentMedia  = new MediaSettings {Device = "screen", Width = 1024};

		private IScriptExecutor _scriptExecutor;
	}
}
