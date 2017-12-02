using System;
using System.IO;
using System.Threading.Tasks;
using Knyaz.Optimus.ResourceProviders;
using Knyaz.Optimus.ScriptExecuting;
using Knyaz.Optimus.Tools;

namespace Knyaz.Optimus.Dom.Elements
{
	/// <summary>
	/// http://www.w3.org/TR/html5/scripting-1.html#the-script-element
	/// </summary>
	public sealed class Script : HtmlElement, IDelayedResource, IHtmlScriptElement
	{
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

		public bool Loaded { get; private set; }
		public bool Executed { get; set; }

		public event Action OnLoad;
		public event Action OnError;

		public Task LoadAsync(IResourceProvider resourceProvider)
		{
			if (string.IsNullOrEmpty(Src))
				throw new InvalidOperationException("Src not set.");

			return resourceProvider.GetResourceAsync(Src).ContinueWith(
				resource =>
				{
					try
					{
						using (var reader = new StreamReader(resource.Result.Stream))
						{
							InnerHTML = reader.ReadToEnd();
							Loaded = true;
						}
					}
					catch
					{
						RaiseError();
					}
				});
		}

		private void RaiseOnLoad()
		{
			if (HasDelayedContent)
			{
				if (OnLoad != null)
					OnLoad();
				this.RaiseEvent("load", false, false);
			}
		}

		private void RaiseError()
		{
			lock(OwnerDocument)
			{
				if (OnError != null)
					OnError();
				this.RaiseEvent("error", false, false);
			}
		}

		public void Execute(IScriptExecutor scriptExecutor)
		{
			scriptExecutor.Execute(Type ?? "text/javascript", Text);
			Executed = true;
			RaiseOnLoad();
		}

		public override Node CloneNode(bool deep)
		{
			var node = (Script)base.CloneNode(deep);
			node.Text = Text;
			return node;
		}
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
		bool Loaded { get; }
		bool HasDelayedContent { get; }
		Task LoadAsync(IResourceProvider resourceProvider);
	}
}