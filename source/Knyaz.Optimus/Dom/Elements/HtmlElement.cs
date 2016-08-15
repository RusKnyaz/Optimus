using System;
using Knyaz.Optimus.Dom.Css;
using Knyaz.Optimus.Dom.Events;
using Knyaz.Optimus.ScriptExecuting;

namespace Knyaz.Optimus.Dom.Elements
{
	/// <summary>
	/// http://www.w3.org/TR/2012/WD-html5-20121025/elements.html#htmlelement
	/// </summary>
	public class HtmlElement : Element, IHtmlElement
	{
		private CssStyleDeclaration _style;

		static class Defaults
		{
			public static bool Hidden = false;
		}

		public HtmlElement(Document ownerDocument, string tagName)
			: base(ownerDocument, tagName)
		{
			
		}

		public bool Hidden
		{
			get { return GetAttribute("hidden", Defaults.Hidden); }
			set { SetAttribute("hidden", value.ToString());}
		}

		public string ClassName
		{
			get { return GetAttribute("class", "");}
			set { SetAttribute("class", value);}
		}

		public virtual void Click()
		{
			var evt = OwnerDocument.CreateEvent("Event");
			evt.InitEvent("click", true, true);
			DispatchEvent(evt);
		}

		public event Action OnClick;

		/// <summary>
		/// This method allows the dispatch of events into the implementations event model. 
		/// Events dispatched in this manner will have the same capturing and bubbling behavior as events dispatched directly by the implementation. The target of the event is the EventTarget on which dispatchEvent is called.
		/// </summary>
		/// <returns> If preventDefault was called the value is false, else the value is true.</returns>
		public override bool DispatchEvent(Event evt)
		{
			if (evt.Type == "click" && OnClick != null)
				OnClick();
			
			return base.DispatchEvent(evt);
		}

		public CssStyleDeclaration Style
		{
			get
			{
				if (_style == null)
				{
					_style = new CssStyleDeclaration {CssText = GetAttribute("style")};
					_style.OnStyleChanged += css =>
					{
						if(GetAttribute("style") != css)
							SetAttribute("style", css);
					};
				}

				return _style;
			}
		}

		protected override void UpdatePropertyFromAttribute(string value, string invariantName)
		{
			base.UpdatePropertyFromAttribute(value, invariantName);

			if (invariantName == "style" && _style != null && Style.CssText != value)
				Style.CssText = value;
		}

		public void Blur()
		{
			OwnerDocument.ActiveElement = null;
		}

		public void Focus()
		{
			OwnerDocument.ActiveElement = this;
		}
	}

	[DomItem]
	public interface IHtmlElement
	{
		bool Hidden { get; set; }
		void Click();
		event Action OnClick;
		string ClassName { get; set; }
		void Blur();
		void Focus();
	}
}
