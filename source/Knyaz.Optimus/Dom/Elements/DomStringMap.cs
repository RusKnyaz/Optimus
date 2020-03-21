using System.Collections.Generic;
using System.Text;

namespace Knyaz.Optimus.Dom.Elements
{
	public class DomStringMap
	{
		private readonly HtmlElement _elt;

		internal DomStringMap(HtmlElement elt) => _elt = elt;

		public string this[string dataName]
		{
			get
			{
				if (string.IsNullOrEmpty(dataName))
					return null;

				var attributeName = ToAttributeName(dataName);

				return _elt.GetAttribute(attributeName);
			}
			set
			{
				if (string.IsNullOrEmpty(dataName))
					return;

				var attributeName = ToAttributeName(dataName);
				
				_elt.SetAttribute(attributeName, value);
			}
		}

		private static string ToAttributeName(string dataSetName)
		{
			if (string.IsNullOrEmpty(dataSetName))
				return null;

			var builder = new StringBuilder();
			builder.Append("data-");

			foreach (var c in dataSetName)
			{
				if (char.IsUpper(c))
				{
					builder.Append('-');
					builder.Append(char.ToLower(c));
				}
				else
				{
					builder.Append(c);
				}
			}

			return builder.ToString();
		}
	}
}