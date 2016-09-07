using System;
using System.Collections.Specialized;
using System.Linq;
using System.Text.RegularExpressions;
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
			return Properties[propertyName];
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
				case "margin":SetClockwise(MarginNames, value);break;
				case "background":SetBackground(value);break;
				case "border":SetBorder(value);break;
				case "border-top": SetBorder("top", value);break;
				case "border-right":SetBorder("right", value);break;
				case "border-bottom":SetBorder("bottom", value);break;
				case "border-left":SetBorder("left", value);break;
				case "border-width":SetClockwise(BorderWidthNames, value);break;
				case "border-style":SetClockwise(BorderStyleNames, value);break;
				case "border-color":SetClockwise(BorderColorNames, value);break;
				case "font":SetFont(value);break;
				case "border-radius":SetClockwise(BorderRadiusNames, value);break;
	        }
	    }

		//Reges to remove spaces around commas
		private static Regex _normalizeCommas = new Regex("\\s*\\,\\s*", RegexOptions.Compiled);

		private void SetFont(string value)
		{
			var args = _normalizeCommas.Replace(value, ",")
				.Split(' ')
				.Where(x => !string.IsNullOrWhiteSpace(x))
				.Reverse()
				.ToArray();

			if (args.Length == 0)
				return;

			if (args.Length == 1)
			{
				//todo: implement
				//one of caption|icon|menu|message-box|small-caption|status-bar|initial|inherit
				return;
			}

			Properties["font-family"] = args[0];

			var sz = args[1].Split('/');
			Properties["font-size"] = sz[0];
			if (sz.Length > 1)
				Properties["line-height"] = sz[1];

			if (args.Length == 2)
				return;

			Properties["font-weight"] = args[2];

			if (args.Length == 3)
				return;
/*todo: try to parse enums to determine which property to set
			Properties["font-variant"] = args[3];

			if (args.Length == 4)
				return;*/

			Properties["font-style"] = args[3];
		}

		private void SetBackground(string value)
		{
			Properties["background-color"] = value;
		}

		private void SetBorder(string value)
		{
			SetBorder("top", value);
			SetBorder("right", value);
			SetBorder("bottom", value);
			SetBorder("left", value);
		}

		private void SetBorder(string side, string value)
		{
			var args = value.Split(' ').Where(x => !string.IsNullOrWhiteSpace(x));
			var prefix = "border-" + side;
			foreach (var arg in args)
			{
				if (char.IsDigit(arg[0]))
				{
					Properties[prefix + "-width"] = arg;
				}
				else if (arg == "none" ||
				         arg == "hidden" ||
				         arg == "dotted" ||
				         arg == "dashed" ||
				         arg == "solid" ||
				         arg == "double" ||
				         arg == "groove" ||
				         arg == "ridge" ||
				         arg == "inset" ||
				         arg == "outset" ||
				         arg == "inherit")
				{
					Properties[prefix + "-style"] = arg;
				}
				else
				{
					Properties[prefix + "-color"] = arg;
				}
			}
		}

		static string[] BorderStyleNames =
			{"border-top-style", "border-right-style", "border-bottom-style", "border-left-style"};

		static string[] BorderWidthNames =
			{"border-top-width", "border-right-width", "border-bottom-width","border-left-width"};

		static string[] BorderColorNames =
			{"border-top-color", "border-right-color", "border-bottom-color","border-left-color"};

		static string[] BorderRadiusNames =
			{"border-top-left-radius", "border-top-right-radius", "border-bottom-right-radius", "border-bottom-left-radius"};

		static string[] PaddingNames = {"padding-top", "padding-right", "padding-bottom", "padding-left"};
		static string[] MarginNames = {"margin-top", "margin-right", "margin-bottom", "margin-left"};

		private void SetPadding(string value)
		{
			SetClockwise(PaddingNames, value);
		}
		private void SetClockwise(string[] names, string value)
		{
			var top = names[0];
			var right = names[1];
			var bottom = names[2];
			var left = names[3];
			if (value == null)
			{
				Properties[top] = Properties[right] = Properties[bottom] = Properties[left] = null;
			}
			else
			{
				var args = value.Split(' ');
				if (args.Length == 1)
				{
					Properties[top] = Properties[right] = Properties[bottom] = Properties[left] = args[0];
				}
				else if(args.Length == 2)
				{
					Properties[top] = Properties[bottom] = args[0];
					Properties[right] = Properties[left] = args[1];
				}
				else if (args.Length == 3)
				{
					Properties[top] = args[0];
					Properties[right] = Properties[left] = args[1];
					Properties[bottom] = args[2];
				}else if (args.Length >= 4)
				{
					Properties[top] = args[0];
					Properties[right] = args[1];
					Properties[bottom] = args[2];
					Properties[left] = args[3];
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
