using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebBrowser.Dom.Elements
{
	/// <summary>
	/// http://www.w3.org/TR/html5/forms.html
	/// </summary>
	public class HtmlFormElement : HtmlElement
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

		public HtmlFormElement(Document ownerDocument) : base(ownerDocument, "form"){}

		public string Name
		{
			get { return GetAttribute("name", string.Empty);}
			set { SetAttribute("name", value);}
		}

		public string Method
		{
			get { return GetAttribute("method", Defaults.Method); }
			set { SetAttribute("method", value); }
		}

		/// <summary>
		/// Character encodings to use for form submission
		/// </summary>
		public string AcceptCharset
		{
			get { return GetAttribute("accept-charset", Defaults.AcceptCharset); }
			set { SetAttribute("accept-charset", value); }
		}

		/// <summary>
		/// URL to use for form submission
		/// </summary>
		public string Action
		{
			get { return GetAttribute("action", Defaults.Action); }
			set { SetAttribute("action", value); }
		}

		/// <summary>
		/// Default setting for autofill feature for controls in the form
		/// </summary>
		public string Autocomplete
		{
			get { return GetAttribute("autocomplete", Defaults.Autocomplete); }
			set { SetAttribute("autocomplete", value); }
		}

		/// <summary>
		/// Form data set encoding type to use for form submission
		/// </summary>
		public string Enctype
		{
			get { return GetAttribute("enctype", Defaults.Enctype); }
			set { SetAttribute("enctype", value); }
		}

		public string Encoding
		{
			get { return GetAttribute("encoding", Defaults.Encoding); }
			set { SetAttribute("encoding", value); }
		}

		/// <summary>
		/// Browsing context for form submission
		/// </summary>
		public string Target
		{
			get { return GetAttribute("target", Defaults.Target); }
			set { SetAttribute("target", value); }
		}

		/// <summary>
		/// return an HTMLFormControlsCollection rooted at the Document node while the form element is in a Document 
		/// and rooted at the form element itself when it is not, whose filter matches listed elements whose form owner 
		/// is the form element, with the exception of input elements whose type attribute is in the Image Button state, 
		/// which must, for historical reasons, be excluded from this particular collection.
		/// </summary>
		public IEnumerable<HtmlElement> Elements
		{
			get
			{
				//todo: consider to make search more optimal
				var allelements = this.IsInDocument() ? this.Flatten() : OwnerDocument.Flatten();
				return allelements.OfType<IFormElement>().Where(x => x.Form == this).OfType<HtmlElement>().Where(
					x=> !(x.NodeType is HtmlInputElement) ||((HtmlInputElement)x).Type != "image" );
			}
		} 

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

		public void Submit()
		{
			throw new NotImplementedException();
		}

		/*
           attribute boolean noValidate;

  readonly attribute HTMLFormControlsCollection elements;
  readonly attribute long length;
  getter Element (unsigned long index);
  getter (RadioNodeList or Element) (DOMString name);

  void submit();
  boolean checkValidity();
		 */
	}
}
