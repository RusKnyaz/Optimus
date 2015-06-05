using System;
using System.IO;
using WebBrowser.ScriptExecuting;
using WebBrowser.TestingTools;

namespace WebBrowser.Dom.Elements
{
	/// <summary>
	/// http://www.w3.org/TR/html5/scripting-1.html#the-script-element
	/// </summary>
	public class Script : HtmlElement, IDelayedResource, IHtmlScriptElement
	{
		private bool _hasDelayedContent;

		private readonly AttributeMappedValue<string> _type;
		private readonly AttributeMappedValue<string> _charset;
		private readonly AttributeMappedValue<string> _src;
		private readonly AttributeMappedBoolValue _async;
		private readonly AttributeMappedBoolValue _defer;

		public string Charset { get { return _charset.Value; } set { _charset.Value = value; } }
		public bool Async { get { return _async.Value; } set { _async.Value = value; } }
		public bool Defer { get { return _defer.Value; } set { _defer.Value = value; } }
		public string Src { get { return _src.Value; } set { _src.Value = value; } }
		public string Type { get { return _type.Value; } set { _type.Value = value; }}

		public string CrossOrigin { get; set; }

		public Script(Document ownerDocument) : base(ownerDocument, TagsNames.Script)
		{
			_type = new AttributeMappedValue<string>(this, "type");
			_charset = new AttributeMappedValue<string>(this, "charset");
			_src = new AttributeMappedValue<string>(this, "src");
			_async = new AttributeMappedBoolValue(this, "async");
			_defer = new AttributeMappedBoolValue(this, "defer");
		}

		public override string InnerHTML { get; set; }

		public string Text { get { return InnerHTML; } set { InnerHTML = value; } }

		public bool HasDelayedContent { get { return !string.IsNullOrEmpty(Src); } }

		public void Load(IResourceProvider resourceProvider)
		{
			if(string.IsNullOrEmpty(Src))
				throw new InvalidOperationException("Src not set.");
			try
			{
				var resource = resourceProvider.GetResource(Src);
				using (var reader = new StreamReader(resource.Stream))
				{
					InnerHTML = reader.ReadToEnd();
					Loaded = true;
				}
			}
			catch (Exception ex)
			{
				OwnerDocument.Context.Send(OnError);
				return;
			}
			OwnerDocument.Context.Send(OnLoad);
		}

		public bool Loaded { get; private set; }
		public bool Executed { get; set; }

		public event Action OnLoad;
		public event Action OnError;
	}

	[DomItem]
	public interface IHtmlScriptElement
	{
		string Src { get; set; }
        string Type { get;set; }
		string Charset { get; set; }
		bool Async { get; set; }
		bool Defer { get; set; }
		string CrossOrigin { get;set; }
		string Text { get; }
	}


	internal interface IDelayedResource
	{
		void Load(IResourceProvider resourceProvider);
		bool Loaded { get; }
		bool HasDelayedContent { get; }
	}
}