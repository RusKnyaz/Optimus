using System;
using System.Collections.Specialized;
using Knyaz.Optimus.ScriptExecuting;

namespace Knyaz.Optimus.Dom.Css
{
	[DomItem]
	public interface ICssStyleDeclaration
	{
		object this[string name] { get; }
		string this[int idx] { get; }
		string GetPropertyValue(string propertyName);
	}

	/// <summary>
	/// http://www.w3.org/2003/01/dom2-javadoc/org/w3c/dom/css/CSSStyleDeclaration.html
	/// </summary>
	public class CssStyleDeclaration : ICssStyleDeclaration
	{
		private string _cssText;

		public CssStyleDeclaration(CssStyleRule parentRule = null)
		{
			Properties = new NameValueCollection();
			ParentRule = parentRule;
		}

		public NameValueCollection Properties { get; private set; }

		public object this[string name]
		{
			get
			{
				int number;
				if (int.TryParse(name, out number))
					return this[number];
				
				return Properties[name]; //return value
			}
			set
			{
				int number;
				if (int.TryParse(name, out number))
					return;

				Properties[name] = value == null ? null : value.ToString();
			}
		}

		public string this[int idx]
		{
			get
			{
				return idx < 0 || idx >= Properties.Count ? string.Empty: Properties.AllKeys[idx]; 
			}
		}

		public string GetPropertyValue(string propertyName)
		{
			return Properties[propertyName];
		}

		public string CssText
		{
			get { return _cssText; }
			set
			{
				if (_cssText != value)
				{
					_cssText = value ?? string.Empty;
					Properties.Clear();
					if(!string.IsNullOrEmpty(value))
						StyleSheetBuilder.FillStyle(this, value);
				}
			}
		}

		public CssStyleRule ParentRule { get; private set; }

		public string RemoveProperty(string propertyName)
		{
			var val = Properties[propertyName];
			Properties.Remove(propertyName);
			return val;
		}

		public void SetProperty(string name, string value, string important)
		{
			//todo: important
			Properties.Add(name, value);
		}

		public string GetPropertyPriority(string propertyName)
		{
			throw new NotImplementedException();
		}

		public int Length
		{
			get { return Properties.Count; }
		}
	}
}
