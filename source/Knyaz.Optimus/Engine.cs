using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Knyaz.Optimus.Configure;
using Knyaz.Optimus.Dom;
using Knyaz.Optimus.Dom.Css;
using Knyaz.Optimus.Dom.Elements;
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
				_scriptExecutor.OnException += ex => Console.Log("Unhandled exception in script: " + ex.Message);
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
		/// Gets the browser's console object.
		/// </summary>
		public Console Console { get; }
		
		/// <summary>
		/// Gets the current Window object.
		/// </summary>
		public Window Window { get; }

		readonly CookieContainer _cookieContainer;

		/// <summary>
		/// Creates new Engine instance with default settings (Js enabled, css disabled).
		/// </summary>
		[Obsolete("Use EngineBuilder to initialize Engine")]
		public Engine(IResourceProvider resourceProvider = null) : this(resourceProvider, null, null)
		{
			ScriptExecutor = new ScriptExecutor(Window,
				parseJson => new XmlHttpRequest(ResourceProvider, () => Document, Document, CreateRequest, parseJson));
		}

		internal Engine(
			IResourceProvider resourceProvider, 
			Window window,
			IScriptExecutor scriptExecutor)
		{
			if (resourceProvider == null)
				resourceProvider = new ResourceProviderBuilder().UsePrediction().Http()
					.Notify(request => OnRequest?.Invoke(request), response => OnResponse?.Invoke(response))
					.Build();
			
			ResourceProvider = resourceProvider;
			_cookieContainer = new CookieContainer();
			
			Console = new Console();

			Window = window ?? CreateDefaultWindow();
			Window.Engine = this;

			if (scriptExecutor != null)
				ScriptExecutor = scriptExecutor; 
		}

		Window CreateDefaultWindow()
		{
			var navigator = new Navigator(new NavigatorPlugins(new PluginInfo[0]))
			{
				UserAgent =
					$"{System.Environment.OSVersion.VersionString} Optimus {GetType().Assembly.GetName().Version.Major}.{GetType().Assembly.GetName().Version.MajorRevision}"
			};
			var window = new Window(() => Document, (url, name, opts) => OnWindowOpen?.Invoke(url, name, opts),
				navigator);

			return window;
		}

		public event Action<Request> OnRequest;
		public event Action<ReceivedEventArguments> OnResponse;

		/// <summary>
		/// Occurs when window.open method called.
		/// </summary>
		public event Action<string, string, string> OnWindowOpen;

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
					if (_scriptExecutor != null)
					{
						Scripting = new DocumentScripting(_document, ScriptExecutor,
							s => ResourceProvider.SendRequestAsync(CreateRequest(s)));
					}

					Document.OnNodeException += OnNodeException;
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
		
		

		private void OnNodeException(Node node, Exception exception) =>
			Console.Log("Node event handler exception: " + exception.Message);


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
		
		/// <summary>
		/// Occurs before the document being loaded from response. 
		/// Can be used to handle non-html response types.
		/// </summary>
		public event EventHandler<ResponseEventArgs> PreHandleResponse; 

		private void LoadFromResponse(Document document, IResource resource)
		{
			if (PreHandleResponse != null)
			{
				var args = new ResponseEventArgs(resource);
				PreHandleResponse(this, args);
				if (args.Cancel)
					return;
			}

			if (resource.Type == null || !resource.Type.StartsWith(ResourceTypes.Html))
				throw new Exception("Invalid resource type: " + (resource.Type ?? "<null>"));

			if (resource is HttpResponse httpResponse && httpResponse.Uri != null)
			{
				Uri = new Uri(httpResponse.Uri+Uri.Fragment);
			}
				

			BuildDocument(document, resource.Stream);
		}

		/// <summary>
		/// For tests
		/// </summary>
		/// <param name="stream"></param>
		[Obsolete]
		public void Load(Stream stream)
		{
			ScriptExecutor.Clear();
			Document = new Document(Window);
			BuildDocument(Document, stream);
		}

		private void BuildDocument(Document document, Stream stream)
		{
			//todo: fix protocol
			if(Uri == null)
				Uri = new Uri("http://localhost");

			//todo: clear js runtime context

			var html = HtmlParser.Parse(stream).ToList();

			if (ResourceProvider is IPredictedResourceProvider resourceProvider)
			{
				foreach (var src in html.OfType<HtmlElement>()
					.Flat(x => x.Children.OfType<HtmlElement>())
					.Where(x => x.Name == "script" && x.Attributes.ContainsKey("src"))
					.Select(x => x.Attributes["src"])
					.Where(x => !string.IsNullOrEmpty(x))
					)
				{
					resourceProvider.Preload(CreateRequest(src));
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
						resourceProvider.Preload(CreateRequest(src));
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

	public class ResponseEventArgs : EventArgs
	{
		public bool Cancel { get; set; }
		public readonly IResource Response;

		public ResponseEventArgs(IResource resource)
		{
			Response = resource;
		}
	}
}
