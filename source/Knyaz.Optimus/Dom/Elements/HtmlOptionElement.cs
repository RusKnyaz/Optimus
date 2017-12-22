namespace Knyaz.Optimus.Dom.Elements
{
	/// <summary>
	/// Represents &lt;OPTION&gt; element.
	/// </summary>
	public sealed class  HtmlOptionElement : HtmlElement
	{
		internal HtmlOptionElement(Document ownerDocument) : base(ownerDocument, TagsNames.Option)
		{
		}

		/// <summary>
		/// Gets or sets the 'name' attribute value.
		/// </summary>
		public string Name
		{
			get { return GetAttribute("name", string.Empty); }
			set { SetAttribute("name", value); }
		}

		/// <summary>
		/// Reflects the value of the value HTML attribute, if it exists; otherwise reflects value of the Node.textContent property.
		/// </summary>
		public string Value
		{
			get { return GetAttribute("value", string.Empty); }
			set { SetAttribute("value", value); }
		}

		//todo: fix it;
		public string Text
		{
			get { return InnerHTML;}
			set { InnerHTML = value; }
		}

		/// <summary>
		/// Indicates whether the option is currently selected.
		/// </summary>
		public bool Selected
		{
			get { return HasAttribute("selected"); }
			set
			{
				if(value)
					SetAttribute("selected","selected");
				else
					RemoveAttribute("selected");
			}	
		}
	}
}