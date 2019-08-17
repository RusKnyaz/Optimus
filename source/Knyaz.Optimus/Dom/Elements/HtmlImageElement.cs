using System;
using System.Threading.Tasks;
using Knyaz.Optimus.Dom.Events;
using Knyaz.Optimus.ScriptExecuting;
using Knyaz.Optimus.Tools;

namespace Knyaz.Optimus.Dom.Elements
{
	/// <summary>
	/// The HTMLImageElement interface provides special properties and methods  for manipulating the layout and presentation of &lt;img&gt; elements.
	/// Specs: https://www.w3.org/TR/2011/WD-html5-author-20110705/the-img-element.html#attr-img-ismap
	/// </summary>
	public class HtmlImageElement : HtmlElement
	{
		private readonly Func<string, Task<IImage>> _loadImage;
		private readonly AttributeMappedValue<string> _src;
		private readonly AttributeMappedValue<string> _alt;
		private readonly AttributeMappedValue<string> _useMap;
		private readonly AttributeMappedValue<int> _width;
		private readonly AttributeMappedValue<int> _height;
		private readonly AttributeMappedBoolValue _isMap;

		private IImage _loadedImage = null;
		
		internal HtmlImageElement(Document ownerDocument, Func<string, Task<IImage>> loadImage) : base(ownerDocument, TagsNames.Img)
		{
			_loadImage = loadImage;
			_src = new AttributeMappedValue<string>(this, "src");
			_alt = new AttributeMappedValue<string>(this, "alt");
			_useMap = new AttributeMappedValue<string>(this, "usemap");
			_isMap = new AttributeMappedBoolValue(this, "ismap");
			_width = new AttributeMappedValue<int>(this, "width");
			_height = new AttributeMappedValue<int>(this, "height");
		}

		/// <summary>
		/// Reflects 'alt' attribute value.
		/// </summary>
		public string Alt
		{
			get => _alt.Value;
			set => _alt.Value = value;
		}

		/// <summary>
		/// Reflects 'usemap' attribute value that if present, can indicate that the image has an associated.
		/// </summary>
		public string UseMap
		{
			get => _useMap.Value;
			set => _useMap.Value = value;
		}

		/// <summary>
		/// Reflects 'ismap' attribute value.
		/// </summary>
		public bool IsMap
		{
			get => _isMap.Value;
			set => _isMap.Value = value;
		}

		/// <summary>
		/// Is a String that reflects the src HTML attribute, containing the full URL of the image including base URI.
		/// </summary>
		public string Src
		{
			get => _src.Value;
			set
			{
				_src.Value = value;

				LoadImage(value);
			}
		}

		private bool _complete = false;

		private async Task LoadImage(string value)
		{
			var wasError = false;

			try
			{
				_complete = false;
				_loadedImage = await _loadImage(value);
				if (_loadedImage == null)
				{
					var errorEvent = (ErrorEvent) OwnerDocument.CreateEvent("errorevent");
					errorEvent.InitEvent("error", true, true);
					errorEvent.ErrorEventInit("", "", 0, 0, ""); //todo: fill error
					DispatchEvent(errorEvent);
					wasError = true;
				}
			}
			catch (Exception ex)
			{
				var errorEvent = (ErrorEvent) OwnerDocument.CreateEvent("errorevent");
				errorEvent.InitEvent("error", true, true);
				errorEvent.ErrorEventInit(ex.Message, "", 0, 0, ""); //todo: fill error
				DispatchEvent(errorEvent);
				wasError = true;
			}
			
			_complete = true;
			
			if (!wasError)
			{
				var loadEvent = OwnerDocument.CreateEvent("Event");
				loadEvent.InitEvent("load", true, true);
				DispatchEvent(loadEvent);
			}
		}


		/// <summary>
		/// Reflects the 'width' HTML attribute, indicating the rendered width of the image in CSS pixels.
		/// </summary>
		public int Width
		{
			get => GetAttribute("width", 0);
			set => SetAttribute("width", value.ToString());
		}

		/// <summary>
		/// Reflects the 'height' HTML attribute, indicating the rendered height of the image in CSS pixels.
		/// </summary>
		public int Height
		{
			get => GetAttribute("height", 0);
			set => SetAttribute("height", value.ToString());
		}

		/// <summary>
		/// The intrinsic width of the image in CSS pixels, if it is available; else, it shows 0.
		/// </summary>
		public int NaturalWidth => _loadedImage != null ? _loadedImage.Width : 0;
		
		/// <summary>
		/// The intrinsic height of the image in CSS pixels, if it is available; else, it shows 0.
		/// </summary>
		public int NaturalHeight => _loadedImage != null ? _loadedImage.Height : 0;
		
		/// <summary>
		/// Returns a Boolean that is true if the browser has finished fetching the image, whether successful or not. It also shows true, if the image has no src value
		/// </summary>
		public bool Complete => string.IsNullOrEmpty(Src) || _complete;
		
		/// <summary>
		/// Fired immediately after an element has been loaded.
		/// </summary>
		public event Action<Event> OnLoad;
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

		[JsHidden]
		public IImage ImageData => _loadedImage;
	}
}