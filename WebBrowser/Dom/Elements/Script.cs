using System;
using System.IO;

namespace WebBrowser.Dom.Elements
{
	public interface IScript : INode
	{
		string Text { get; }
		string Type { get; }
	}

	public class EmbeddedScript : Element, IScript
	{
		public EmbeddedScript(string type, string text) : base("script")
		{
			Type = type;
			Text = text;
		}

		public string Text { get; private set; }
		public string Type { get; private set; }

		public override INode CloneNode()
		{
			return new EmbeddedScript(Type, Text) { OwnerDocument = OwnerDocument };
		}
	}

	public class LinkScript : Element, IScript, IDelayedResource
	{
		private readonly string _url;

		public LinkScript(string type, string url)
			: base("script")
		{
			_url = url;
			Type = type;
		}

		public string Text { get; private set; }
		public string Type { get; private set; }
		public void Load(IResourceProvider resourceProvider)
		{
			var resource = resourceProvider.GetResource(new Uri(_url));
			using (var reader = new StreamReader(resource.Stream))
			{
				Text = reader.ReadToEnd();
			}
		}

		public override INode CloneNode()
		{
			return new LinkScript(Type, _url) {OwnerDocument = OwnerDocument};
		}
	}

	internal interface IDelayedResource
	{
		void Load(IResourceProvider resourceProvider);
	}
}