using System;
using System.IO;

namespace WebBrowser.Dom.Elements
{
	public interface IScript : INode
	{
		string Code { get; }
		string Type { get; }
	}

	public class EmbeddedScript : Element, IScript
	{
		public EmbeddedScript(string type, string code) : base("script")
		{
			Type = type;
			Code = code;
		}

		public string Code { get; private set; }
		public string Type { get; private set; }

		public override INode CloneNode()
		{
			return new EmbeddedScript(Type, Code);
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

		public string Code { get; private set; }
		public string Type { get; private set; }
		public void Load(IResourceProvider resourceProvider)
		{
			var resource = resourceProvider.GetResource(new Uri(_url));
			using (var reader = new StreamReader(resource.Stream))
			{
				Code = reader.ReadToEnd();
			}
		}

		public override INode CloneNode()
		{
			return new LinkScript(Type, _url);
		}
	}

	internal interface IDelayedResource
	{
		void Load(IResourceProvider resourceProvider);
	}
}