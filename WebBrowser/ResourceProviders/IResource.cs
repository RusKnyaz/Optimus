using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace WebBrowser
{
	public interface IResource
	{
		ResourceTypes Type { get; }
		Stream Stream { get; }
	}

	public enum ResourceTypes
	{
		Html,
		Text,
		JavaScript
	}
}
