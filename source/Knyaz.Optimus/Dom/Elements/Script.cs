using System;
using System.IO;
using System.Threading.Tasks;
using Knyaz.Optimus.ResourceProviders;
using Knyaz.Optimus.ScriptExecuting;
using Knyaz.Optimus.Tools;

namespace Knyaz.Optimus.Dom.Elements
{
	/// <summary>
	/// Represents &lt;SCRIPT&gt; element.
	/// </summary>
	[DomItem]
	public sealed class Script : HtmlElement
	{
		private readonly AttributeMappedValue<string> _type;
		private readonly AttributeMappedValue<string> _charset;
		private readonly AttributeMappedValue<string> _src;
		private readonly AttributeMappedBoolValue _async;
		private readonly AttributeMappedBoolValue _defer;

		/// <summary>
		/// Gets or sets the 'charset' attribute value reflecting the charset attribute.
		/// </summary>
		public string Charset {get => _charset.Value;set => _charset.Value = value;}
		
		/// <summary>
		/// Gets or sets 'async' attribute value.
		/// </summary>
		public bool Async { get => _async.Value; set => _async.Value = value;}
		
		/// <summary>
		/// Gets or sets 'defer' attribute value.
		/// </summary>
		public bool Defer { get => _defer.Value;set => _defer.Value = value;}
		
		/// <summary>
		/// Gets or sets 'src' attirbute representing the address of the external script resource to use. 
		/// </summary>
		public string Src { get => _src.Value; set => _src.Value = value;}
		
		/// <summary>
		/// Gets or sets 'type' attribute value representing MIME type of the script.
		/// </summary>
		public string Type { get => _type.Value;set => _type.Value = value;}

		public string CrossOrigin { get; set; }
	
		internal Script(Document ownerDocument) : base(ownerDocument, TagsNames.Script)
		{
			_type = new AttributeMappedValue<string>(this, "type");
			_charset = new AttributeMappedValue<string>(this, "charset");
			_src = new AttributeMappedValue<string>(this, "src");
			_async = new AttributeMappedBoolValue(this, "async");
			_defer = new AttributeMappedBoolValue(this, "defer");
		}

		public override string InnerHTML { get; set; }

		public string Text { get { return InnerHTML; } set { InnerHTML = value; } }

		internal bool HasDelayedContent => !string.IsNullOrEmpty(Src);

		public bool Loaded { get; private set; }
		internal bool Executed { get; set; }

		/// <summary>
		/// Fired immediately after an element has been loaded.
		/// </summary>
		public event Action OnLoad;
		public event Action OnError;

		//todo: revise it. it shouldn't be here.
		internal Task LoadAsync(IResourceProvider resourceProvider)
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
				OnLoad?.Invoke();
				this.RaiseEvent("load", false, false);
			}
		}

		private void RaiseError()
		{
			lock(OwnerDocument)
			{
				OnError?.Invoke();
				this.RaiseEvent("error", false, false);
			}
		}

		internal void Execute(IScriptExecutor scriptExecutor)
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

		public override string ToString() => "[Object HTMLScriptElement]";
	}
}