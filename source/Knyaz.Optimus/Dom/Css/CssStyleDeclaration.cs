using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Knyaz.Optimus.Dom.Interfaces;

namespace Knyaz.Optimus.Dom.Css
{
	/// <summary>
	/// Represents a single CSS declaration block.
	/// http://www.w3.org/2003/01/dom2-javadoc/org/w3c/dom/css/CSSStyleDeclaration.html
	/// </summary>
	/// <inheritdoc cref="ICssStyleDeclaration"/>
	public partial class CssStyleDeclaration : ICssStyleDeclaration
	{
		private string _cssText = string.Empty;
		private readonly NameValueCollection _properties;
		private readonly HashSet<string> _importants = new HashSet<string>();

		internal event Action<string> OnStyleChanged;

		internal CssStyleDeclaration(CssStyleRule parentRule = null)
		{
			_properties = new NameValueCollection();
			ParentRule = parentRule;
		}

		/// <summary>
		/// The number of properties that have been explicitly set in this declaration block.
		/// </summary>
		public int Length => _properties.Count;


		//todo: it's not better idea to use 'object' but sometimes js code tries to set the value of 'double' type.
		public object this[string name]
		{
			get
			{
				int number;
				if (int.TryParse(name, out number))
					return this[number];

				return _properties[name]; //return value
			}
			set
			{
				if (int.TryParse(name, out _))
					return;

				if (value == null)
				{
					SetProperty(name, null);
				}
				else
				{
					SetProperty(name,
						value is double num ? num.ToString(CultureInfo.InvariantCulture) : value.ToString());
				}
			}
		}

		private void UpdateCssText()
		{
			var newCss = string.Join(";",
				_properties.AllKeys.Where(x => !string.IsNullOrEmpty(_properties[x])).Select(x => x + ":" + _properties[x]));

			if (newCss != _cssText)
			{
				_cssText = newCss;
				if (OnStyleChanged != null)
					OnStyleChanged(_cssText);
			}
		}

		/// <summary>
		/// Retrieve the properties anmes that have been explicitly set in this declaration block.
		/// </summary>
		/// <param name="idx"></param>
		/// <returns></returns>
		public string this[int idx] => idx < 0 || idx >= _properties.Count ? string.Empty : _properties.AllKeys[idx];

		/// <summary>
		/// Retrieves the value of a CSS property if it has been explicitly set within this declaration block.
		/// </summary>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		public string GetPropertyValue(string propertyName)
		{
			return _properties[propertyName];
		}

		/// <summary>
		/// The parsable textual representation of the declaration block (excluding the surrounding curly braces).
		/// </summary>
		public string CssText
		{
			get { return _cssText; }
			set
			{
				var newCssText = value ?? string.Empty;
				if (_cssText != newCssText)
				{
					_cssText = newCssText;
					_properties.Clear();
					if (newCssText != string.Empty)
						StyleSheetBuilder.FillStyle(this, newCssText);

					if (OnStyleChanged != null)
						OnStyleChanged(_cssText);
				}
			}
		}

		/// <summary>
		/// The CSS rule that contains this declaration block or null if this CssStyleDeclaration is not attached to a CssRule.
		/// </summary>
		public CssStyleRule ParentRule { get; private set; }

		/// <summary>
		/// Removes a CSS property if it has been explicitly set within this declaration block.
		/// </summary>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		public string RemoveProperty(string propertyName)
		{
			var val = _properties[propertyName];
			_properties.Remove(propertyName);
			if (_importants.Contains(propertyName))
				_importants.Remove(propertyName);

			return val;
		}

		/// <summary>
		/// Sets a property value and priority within this declaration block.
		/// </summary>
		/// <param name="name">The name of the property to be set.</param>
		/// <param name="value">The value of the property.</param>
		/// <param name="important">The priority of the property.</param>
		public void SetProperty(string name, string value, string important = null)
		{
			if (string.IsNullOrEmpty(value))
			{
				RemoveProperty(name);
				return;
			}

			name = name.Replace(" ", "");
			
			if (important == Css.ImportantValue)
				_importants.Add(name);
			else if(_importants.Contains(name))
				_importants.Remove(name);

			_properties[name] = ValidateComplex(name, value);

			HandleComplexProperty(name, value);
			UpdateCssText();
		}

		private void HandleComplexProperty(string name, string value)
		{
			switch (name)
			{
				case Css.Padding:
					SetPadding(value);
					break;
				case Css.Margin:
					SetClockwise(MarginNames, value);
					break;
				case Css.Background:
					SetBackground(value);
					break;
				case Css.Border:
					SetBorder(value);
					break;
				case Css.BorderTop:
					SetBorder("top", value);
					break;
				case Css.BorderRight:
					SetBorder("right", value);
					break;
				case Css.BorderBottom:
					SetBorder("bottom", value);
					break;
				case Css.BorderLeft:
					SetBorder("left", value);
					break;
				case Css.BorderWidth:
					SetClockwise(BorderWidthNames, value);
					break;
				case Css.BorderStyle:
					SetClockwise(BorderStyleNames, value, x => Validate(Css.BorderStyle, x));
					break;
				case Css.BorderColor:
					SetClockwise(BorderColorNames, value);
					break;
				case Css.Font:
					SetFont(value);
					break;
				case Css.BorderRadius:
					SetClockwise(BorderRadiusNames, value);
					break;
				case Css.ListStyle:
					SetListStyle(value);
					break;
			}
		}

		private void SetListStyle(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				this[Css.ListStyleType] = value;
				this[Css.ListStylePosition] = value;
				this[Css.ListStyleImage] = value;
			}

			var args = value.Split(' ');
			if (args.Length > 0)
			{
				this[Css.ListStyleType] = args[0];
			}

			if (args.Length > 1)
			{
				this[Css.ListStylePosition] = args[1];
			}

			if (args.Length > 2)
			{
				this[Css.ListStyleImage] = args[2];
			}

		}

		//Reges to remove spaces around commas
		private static Regex _normalizeCommas = new Regex("\\s*\\,\\s*", RegexOptions.Compiled);
		private static string[] FontWeights = {"bold", "bolder", "lighter", "normal", "100", "200", "300","400","500","600","700","800","900"};
		
		///<param name="value">[font-style", "", "font-variant", "", "font-weight] font-size [/line-height] font-family", "inherit</param>
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
				//one of caption", "icon", "menu", "message-box", "small-caption", "status-bar", "initial", "inherit
				return;
			}

			_properties[Css.FontFamily] = args[0];

			var sz = args[1].Split('/');
			_properties[Css.FontSize] = sz[0];
			if (sz.Length > 1)
				_properties["line-height"] = sz[1];

			var i = 2;

			if (args.Length == i)
			{
				_properties[Css.FontWeight] = _properties[Css.FontStyle] = _properties[Css.FontVariant] = "normal";
				return;
			}

			var val = args[i];
			if (FontWeights.Contains(val))
			{
				_properties[Css.FontWeight] = val;
				i++;
				if (args.Length == i)
					return;
				val = args[i];
			}
			else
			{
				_properties[Css.FontWeight] = "normal";
			}

			if (FontVariantValidValues.Contains(val))
			{
				_properties[Css.FontVariant] = val;
				i++;
				if (args.Length == i)
					return;
				val = args[i];
			}
			else
			{
				_properties[Css.FontVariant] = "normal";
			}

			_properties[Css.FontStyle] = FontStyleValidValues.Contains(val) ? val : "normal";
		}


		private void SetBackground(string value)
		{
			_properties[Css.BackgroundColor] = value;
		}

		private void SetBorder(string value)
		{
			SetBorder("top", value);
			SetBorder("right", value);
			SetBorder("bottom", value);
			SetBorder("left", value);
		}

		private IEnumerable<string> SplitValues(string values)
		{
			var splitIndex = 0;
			var inBrackets = false;
			int i = 0;
			for(; i< values.Length; i++)
			{
				var c = values[i];
				if (!inBrackets)
				{
					if (c == '(')
						inBrackets = true;
					else if(c == ' ')
					{
						var res = values.Substring(splitIndex, i - splitIndex);
						splitIndex = i + 1;
						if (!string.IsNullOrWhiteSpace(res))
							yield return res;
					}
				}
				else if(c == ')')
				{
					inBrackets = false;
				}
			}

			if(splitIndex != values.Length)
				yield return values.Substring(splitIndex);
		}

		private void SetBorder(string side, string value)
		{
			var args = SplitValues(value);
			var prefix = "border-" + side;
			foreach (var arg in args)
			{
				if (char.IsDigit(arg[0]))
				{
					_properties[prefix + "-width"] = arg;
				}
				else if (BorderStyleValidValues.Contains(arg))
				{
					_properties[prefix + "-style"] = arg;
				}
				else
				{
					_properties[prefix + "-color"] = arg;
				}
			}
		}

		static string[] BorderStyleNames =
			{Css.BorderTopStyle, Css.BorderRightStyle, Css.BorderBottomStyle, Css.BorderLeftStyle};

		static string[] BorderWidthNames =
		{Css.BorderTopWidth, Css.BorderRightWidth, Css.BorderBottomWidth,Css.BorderLeftWidth};

		static string[] BorderColorNames =
			{Css.BorderTopColor, Css.BorderRightColor, Css.BorderBottomColor, Css.BorderLeftColor};

		static string[] BorderRadiusNames =
			{"border-top-left-radius", "border-top-right-radius", "border-bottom-right-radius", "border-bottom-left-radius"};
		
		static string[] PaddingNames = {Css.PaddingTop, Css.PaddingRight, Css.PaddingBottom, Css.PaddingLeft};
		static string[] MarginNames = {Css.MarginTop, Css.MarginRight, Css.MarginBottom, Css.MarginLeft};

		private void SetPadding(string value)
		{
			SetClockwise(PaddingNames, value);
		}
		private void SetClockwise(string[] names, string value, Func<string, string> validate = null)
		{
			validate = validate ?? (s => s); 
			
			var top = names[0];
			var right = names[1];
			var bottom = names[2];
			var left = names[3];
			if (value == null)
			{
				_properties[top] = _properties[right] = _properties[bottom] = _properties[left] = null;
			}
			else
			{
				var args = SplitValues(value).ToArray();
				if (args.Length == 1)
				{
					_properties[top] = _properties[right] = _properties[bottom] = _properties[left] = validate(args[0]);
				}
				else if(args.Length == 2)
				{
					_properties[top] = _properties[bottom] = validate(args[0]);
					_properties[right] = _properties[left] = validate(args[1]);
				}
				else if (args.Length == 3)
				{
					_properties[top] = validate(args[0]);
					_properties[right] = _properties[left] = validate(args[1]);
					_properties[bottom] = args[2];
				}else if (args.Length >= 4)
				{
					_properties[top] = validate(args[0]);
					_properties[right] = validate(args[1]);
					_properties[bottom] = validate(args[2]);
					_properties[left] = validate(args[3]);
				}
			}
		}

		/// <summary>
		/// Used to retrieve the priority of a CSS property.
		/// </summary>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		public string GetPropertyPriority(string propertyName)
		{
			return _importants.Contains(propertyName) ? Css.ImportantValue : string.Empty;
		}

		public override string ToString()
		{
			var props = string.Join(";", _properties.Keys.OfType<string>().Select(x => x + ":"+_properties[x] + (_importants.Contains(x) ? " !important" : "")));
			return "{" + props + "}";
		}
	}
}
