using System;
using Knyaz.Optimus.Dom.Css;
using Knyaz.Optimus.Dom.Events;
using Knyaz.Optimus.ScriptExecuting;

namespace Knyaz.Optimus.Dom.Elements
{
	/// <summary>
	/// The base class for the classes representing html elemenets.
	/// </summary>
	[DomItem]
	public class HtmlElement : Element
	{
		private CssStyleDeclaration _style;

		static class Defaults
		{
			public static bool Hidden = false;
		}

		internal HtmlElement(Document ownerDocument, string tagName)
			: base(ownerDocument, tagName) {}

		/// <summary>
		/// Gets or sets the 'hidden' attribute value, indicating if the element is hidden or not.
		/// </summary>
		public bool Hidden
		{
			get => GetAttribute("hidden", Defaults.Hidden);
			set => SetAttribute("hidden", value.ToString());
		}

		/// <summary>
		/// Sends a mouse click event to the element.
		/// </summary>
		public virtual void Click()
		{
			var evt = OwnerDocument.CreateEvent("Event");
			evt.InitEvent("click", true, true);
			DispatchEvent(evt);
		}

		/// <summary>
		/// Called before the mouse 'click' dispatched.
		/// </summary>
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

		/// <summary>
		/// Gets a CssStyleDeclaration whose value represents the declarations specified in the attribute, if present. 
		/// </summary>
		/// <remarks>
		/// Mutating the CssStyleDeclaration object must create a style attribute on the element (if there isn't one 
		/// already) and then change its value to be a value representing the serialized form of the CSSStyleDeclaration
		///  object. The same object must be returned each time.
		/// </remarks>
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

		/// <summary>
		/// Removes keyboard focus from the current element.
		/// </summary>
		public void Blur() => OwnerDocument.ActiveElement = null;

		/// <summary>
		/// Sets keyboard focus on the specified element, if it can be focused.
		/// </summary>
		public void Focus() => OwnerDocument.ActiveElement = this;
	}
}
