﻿using System.Collections.Specialized;

namespace WebBrowser.Dom.Elements
{
	/// <summary>
	/// http://www.w3.org/2003/01/dom2-javadoc/org/w3c/dom/css/CSSStyleDeclaration.html
	/// </summary>
	public class CssStyleDeclaration
	{
		public CssStyleDeclaration()
		{
			Properties = new NameValueCollection();
		}

		public NameValueCollection Properties { get; private set; }

		public string this[string name]
		{
			get
			{
				int number;
				if (int.TryParse(name, out number))
					return Properties.AllKeys[number];
				return Properties[name];
			}
		}

		public string this[int idx]
		{
			get { return Properties.AllKeys[idx]; }
		}

		public string GetPropertyValue(string propertyName)
		{
			return Properties[propertyName];
		}
	}
}