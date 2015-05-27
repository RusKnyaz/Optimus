using System;
using System.IO;
using WebBrowser.TestingTools;

namespace WebBrowser.Dom.Elements
{
	public class Script : Element, IDelayedResource
	{
		private bool _hasDelayedContent;
		private string _src;
		public string Charset { get; private set; }
		public string Type { get; private set; }

		public Script(Document ownerDocument) : base(ownerDocument, "script") { }

		/// <summary>
		/// Uri
		/// </summary>
		public string Src
		{
			get { return _src; }
			set
			{
				_src = value;
				_hasDelayedContent = !string.IsNullOrEmpty(_src);
			}
		}

		public override string InnerHTML { get; set; }

		public string Text { get { return InnerHTML; } set { InnerHTML = value; } }

		public bool HasDelayedContent { get { return _hasDelayedContent; } }

		public void Load(IResourceProvider resourceProvider)
		{
			if(string.IsNullOrEmpty(Src))
				throw new InvalidOperationException("Src not set.");
			var resource = resourceProvider.GetResource(Src);
			using (var reader = new StreamReader(resource.Stream))
			{
				InnerHTML = reader.ReadToEnd();
				Loaded = true;
			}
			OwnerDocument.Context.Send(OnLoad);
		}

		public bool Loaded { get; private set; }
		public bool Executed { get; set; }

		public event Action OnLoad;

		protected override void UpdatePropertyFromAttribute(string value, string invariantName)
		{
			base.UpdatePropertyFromAttribute(value, invariantName);

			switch (invariantName)
			{
				case "src": Src = value; break;
				case "type": Type = value; break;
				case "charset": Charset = value; break;
			}
		}
	}


	internal interface IDelayedResource
	{
		void Load(IResourceProvider resourceProvider);
		bool Loaded { get; }
		bool HasDelayedContent { get; }
	}
}