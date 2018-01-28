using System;
using System.Collections.Generic;
using System.Linq;
using Knyaz.Optimus.Dom.Events;
using Knyaz.Optimus.Tools;

namespace Knyaz.Optimus.Dom.Elements
{
	/// <summary>
	/// Represents &lt;FORM&gt; element.
	/// http://www.w3.org/TR/html5/forms.html"
	/// https://developer.mozilla.org/en-US/docs/Web/API/HTMLFormElement"
	/// </summary>
	public sealed class HtmlFormElement : HtmlElement
	{
		static class Defaults
		{
			public static string Method = "get";
			public static string AcceptCharset = "";
			public static string Action = "";
			public static string Autocomplete = "on";
			public static string Enctype = "application/x-www-form-urlencoded";
			public static string Target = string.Empty;
			public static string Encoding = "application/x-www-form-urlencoded";
		}

		static string[] AllowedEnctypes = new []{"application/x-www-form-urlencoded", "multipart/form-data", "text/plain" };
		private static string[] AllowedMethods = new[] {"get", "post"};

		internal HtmlFormElement(Document ownerDocument) : base(ownerDocument, TagsNames.Form){}

		protected override void CallDirectEventSubscribers(Event evt)
		{
			base.CallDirectEventSubscribers(evt);

			if (evt.Type == "submit")
			{
				Handle("onsubmit", OnSubmit, evt);
				if (!evt.IsDefaultPrevented())
				{
					OwnerDocument.HandleFormSubmit(this, evt.OriginalTarget as HtmlElement);
				}
			}
		}

		/// <summary>
		/// Gets or sets the 'name' attribute value reflecting the value of the form's name HTML attribute, containing the name of the form.
		/// </summary>
		public string Name
		{
			get => GetAttribute("name", string.Empty);
			set => SetAttribute("name", value);
		}

		/// <summary>
		/// Gets or sets the 'method' attribute value reflecting the value of the form's method HTML attribute, indicating the HTTP method used to submit the form. 
		/// </summary>
		public string Method
		{
			get => GetAttribute("method", AllowedMethods, Defaults.Method).ToLowerInvariant();
			set => SetAttribute("method", value);
		}

		/// <summary>
		/// Character encodings to use for form submission
		/// </summary>
		public string AcceptCharset
		{
			get => GetAttribute("accept-charset", Defaults.AcceptCharset);
			set => SetAttribute("accept-charset", value);
		}

		/// <summary>
		/// URL to use for form submission
		/// </summary>
		public string Action
		{
			get => GetAttribute("action", Defaults.Action);
			set => SetAttribute("action", value);
		}

		/// <summary>
		/// Default setting for autofill feature for controls in the form
		/// </summary>
		public string Autocomplete
		{
			get => GetAttribute("autocomplete", Defaults.Autocomplete);
			set => SetAttribute("autocomplete", value);
		}

		/// <summary>
		/// Form data set encoding type to use for form submission. One of three below: 
		/// <c>application/x-www-form-urlencoded</c>Default. All characters are encoded before sent (spaces are converted to "+" symbols, and special characters are converted to ASCII HEX values)
		///	<c>multipart/form-data</c>	No characters are encoded. This value is required when you are using forms that have a file upload control
		/// <c>text/plain</c>	Spaces are converted to "+" symbols, but no special characters are encoded
		/// Note: The enctype attribute can be used only if method=<c>post</c>.
		/// </summary>
		public string Enctype
		{
			get => GetAttribute("enctype", AllowedEnctypes, Defaults.Enctype).ToLowerInvariant();
			set => SetAttribute("enctype", value);
		}

		/// <summary>
		/// Gets or sets the 'encoding' attribute value reflecting the value of the form's enctype HTML 
		/// attribute, indicating the type of content that is used to transmit the form to the server.
		/// </summary>
		public string Encoding
		{
			get => GetAttribute("encoding", Defaults.Encoding);
			set => SetAttribute("encoding", value);
		}

		/// <summary>
		/// Browsing context for form submission
		/// </summary>
		public string Target
		{
			get => GetAttribute("target", Defaults.Target);
			set => SetAttribute("target", value);
		}

		/// <summary>
		/// return an HTMLFormControlsCollection rooted at the Document node while the form element is in a Document 
		/// and rooted at the form element itself when it is not, whose filter matches listed elements whose form owner 
		/// is the form element, with the exception of input elements whose type attribute is in the Image Button state, 
		/// which must, for historical reasons, be excluded from this particular collection.
		/// </summary>
		public IReadOnlyCollection<HtmlElement> Elements
		{
			get
			{
				//todo: consider to make search more optimal
				var allelements = this.IsInDocument() ? OwnerDocument.Flatten() : this.Flatten();
				return allelements.OfType<IFormElement>()
                    .Where(x => x.Form == this)
                    .OfType<HtmlElement>()
                    .Where(x=> !(x is HtmlInputElement) ||((HtmlInputElement)x).Type != "image" )
					.ToList();
			}
		} 

		/// <summary>
		/// Resets the form to its initial state.
		/// </summary>
		public void Reset()
		{
			var resetEvent = OwnerDocument.CreateEvent("Event");
			resetEvent.InitEvent("reset", true, true);
			if (DispatchEvent(resetEvent))
			{
				foreach (var resettable in Elements.OfType<IResettableElement>())
				{
					resettable.Reset();
				}
			}
		}

		/// <summary>
		/// Submits the form to the server.
		/// </summary>
		public void Submit() => OwnerDocument.HandleFormSubmit(this, null);

		
		internal void RaiseSubmit(HtmlElement submitElement)
		{
			var evt = OwnerDocument.CreateEvent("Event");
			evt.OriginalTarget = submitElement;
			evt.InitEvent("submit", true, true);
			DispatchEvent(evt);
		}
		
		

		/// <summary>
		/// Called on form submit.
		/// </summary>
		public event Action<Event> OnSubmit;

		/*
           attribute boolean noValidate;

  readonly attribute HTMLFormControlsCollection elements;
  readonly attribute long length;
  getter Element (unsigned long index);
  getter (RadioNodeList or Element) (DOMString name);

  boolean checkValidity();
		 */
	}
}
