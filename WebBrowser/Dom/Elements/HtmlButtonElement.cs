namespace WebBrowser.Dom.Elements
{
	public class HtmlButtonElement : HtmlElement, IFormElement
	{
		private static class Defaults
		{
			public const string Type = "submit";
			public const string Value = "";
			public const string Name = "";
			public const string AccessKey = "";
			public const long TabIndex = 0;
		}

		private static readonly string[] AvailableTypes = {"submit", "button", "reset"};

		public HtmlButtonElement(Document ownerDocument) : base(ownerDocument, "button") { }

		public string Type
		{
			get { return GetAttribute("type", AvailableTypes, Defaults.Type);}
			set { SetAttribute("type", value);}
		}

		public string Value
		{
			get { return GetAttribute("value", Defaults.Value);}
			set { SetAttribute("value", value);}
		}

		public bool Disabled
		{
			get { return GetAttribute("disabled") == null; }
			set { SetAttribute("disabled", value ? "" : null);}
		}

		public string Name
		{
			get { return GetAttribute("name", Defaults.Name); }
			set { SetAttribute("name", value); }
		}

		public string AccessKey
		{
			get { return GetAttribute("accessKey", Defaults.AccessKey); }
			set { SetAttribute("accessKey", value); }
		}

		public long TabIndex
		{
			get { return GetAttribute("tabIndex", Defaults.TabIndex); }
			set { SetAttribute("tabIndex", value.ToString());}
		}

		public override void Click()
		{
			var evt = OwnerDocument.CreateEvent("Event");
			evt.InitEvent("click", true, true);
			DispatchEvent(evt);
		}

		public override bool DispatchEvent(Event evt)
		{
			if (!base.DispatchEvent(evt))
				return false;
			
			//default actions;
			if (evt.Type == "click")
			{
				var form = Form;
				if (form != null)
					form.Submit();
			}
			return true;
		}

		public HtmlFormElement Form
		{
			get { return this.FindOwnerForm(); }
		}
	}
}
