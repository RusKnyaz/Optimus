using System;
using Knyaz.Optimus.Dom.Events;
using Knyaz.Optimus.ScriptExecuting;

namespace Knyaz.Optimus.Dom.Elements
{
	/// <summary>
	/// Represents &lt;SCRIPT&gt; element.
	/// </summary>
	[JsName("HTMLScriptElement")]
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

		internal Script(Document ownerDocument) : base(ownerDocument, TagsNames.Script)
		{
			_type = new AttributeMappedValue<string>(this, "type");
			_charset = new AttributeMappedValue<string>(this, "charset");
			_src = new AttributeMappedValue<string>(this, "src");
			_async = new AttributeMappedBoolValue(this, "async");
			_defer = new AttributeMappedBoolValue(this, "defer");
		}

		public override string InnerHTML
		{
			get => Text;
			set => Text = value;
		}

		public string Text 
		{ 
			get => TextContent ?? "";
			set => TextContent = value; 
		}

		internal bool IsExternalScript => !string.IsNullOrEmpty(Src);

		/// <summary>
		/// Indicates whether the script was executed or not.
		/// </summary>
		internal bool Executed { get; set; }
		internal string Code { get; set; }

		/// <summary>
		/// Fired immediately after an element has been loaded.
		/// </summary>
		[JsName("onload")]
		public event Action<Event> OnLoad;
		[JsName("onerror")]
		public event Action<Event> OnError;
		
		protected override void CallDirectEventSubscribers(Event evt)
		{
			base.CallDirectEventSubscribers(evt);

			switch (evt.Type)
			{
				case "load":Handle("onload", OnLoad, evt);break;
				case "error":Handle("onerror", OnError, evt);break;
			}
		}

		/// <summary>
		/// Creates new copy of the script node.
		/// </summary>
		/// <param name="deep"></param>
		/// <returns></returns>
		public override Node CloneNode(bool deep)
		{
			var node = (Script)base.CloneNode(deep);
			node.Executed = Executed;
			return node;
		}
	}
}