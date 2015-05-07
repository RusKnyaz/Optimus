using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebBrowser
{
	public interface IResourceProvider
	{
		IResource GetResource(Uri uri);
	}
}
