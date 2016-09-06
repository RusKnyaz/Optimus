using System;
using System.Collections.Specialized;
using System.Linq;
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
		private string _cssText = string.Empty;

		internal event Action<string> OnStyleChanged;

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

			    SetProperty(name, value == null ? null : value.ToString());
			}
		}

		private void UpdateCssText()
		{
			var newCss =  string.Join(";",
				Properties.AllKeys.Where(x => !string.IsNullOrEmpty(Properties[x])).Select(x => x + ":" + Properties[x]));

			if (newCss != _cssText)
			{
				_cssText = newCss;
				if (OnStyleChanged != null)
					OnStyleChanged(_cssText);
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
			return Properties[propertyName] ?? "";
		}

		public string CssText
		{
			get { return _cssText; }
			set
			{
				var newCssText = value ?? string.Empty;
				if (_cssText != newCssText)
				{
					_cssText = newCssText;
					Properties.Clear();
					if(newCssText != string.Empty)
						StyleSheetBuilder.FillStyle(this, newCssText);

					if (OnStyleChanged != null)
						OnStyleChanged(_cssText);
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

		public void SetProperty(string name, string value, string important = null)
		{
			//todo: important
			name = name.Replace(" ", "");
			Properties[name] = value;
		    HandleComplexProperty(name, value);
			UpdateCssText();
		}

	    private void HandleComplexProperty(string name, string value)
	    {
	        switch (name)
	        {
	        	case "padding":SetPadding(value);break;
		        case "margin":SetMargin(value);break;
				case "background":SetBackground(value);break;
	        }
	    }

		private void SetBackground(string value)
		{
			Properties["background-color"] = value;
		}

		private void SetPadding(string value)
		{
			if (value == null)
			{
				Properties["padding-top"] =
					Properties["padding-right"] =
						Properties["padding-bottom"] =
							Properties["padding-left"] = null;
			}
			else
			{
				var args = value.Split(' ');
				if (args.Length == 1)
				{
					Properties["padding-top"] =
						Properties["padding-right"] =
							Properties["padding-bottom"] =
								Properties["padding-left"] = args[0];
				}
				else if(args.Length == 2)
				{
					Properties["padding-top"] = Properties["padding-bottom"] = args[0];
					Properties["padding-right"] = Properties["padding-left"] = args[1];
				}
				else if (args.Length == 3)
				{
					Properties["padding-top"] = args[0];
					Properties["padding-right"] = Properties["padding-left"] = args[1];
					Properties["padding-bottom"] = args[2];
				}else if (args.Length >= 4)
				{
					Properties["padding-top"] = args[0];
					Properties["padding-right"] = args[1];
					Properties["padding-bottom"] = args[2];
					Properties["padding-left"] = args[3];
				}
			}
		}

		private void SetMargin(string value)
		{
			if (value == null)
			{
				Properties["margin-top"] =
					Properties["margin-right"] =
						Properties["margin-bottom"] =
							Properties["margin-left"] = null;
			}
			else
			{
				var args = value.Split(' ');
				if (args.Length == 1)
				{
					Properties["margin-top"] =
						Properties["margin-right"] =
							Properties["margin-bottom"] =
								Properties["margin-left"] = args[0];
				}
				else if (args.Length == 2)
				{
					Properties["margin-top"] = Properties["margin-bottom"] = args[0];
					Properties["margin-right"] = Properties["margin-left"] = args[1];
				}
				else if (args.Length == 3)
				{
					Properties["margin-top"] = args[0];
					Properties["margin-right"] = Properties["margin-left"] = args[1];
					Properties["margin-bottom"] = args[2];
				}
				else if (args.Length >= 4)
				{
					Properties["margin-top"] = args[0];
					Properties["margin-right"] = args[1];
					Properties["margin-bottom"] = args[2];
					Properties["margin-left"] = args[3];
				}
			}
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
