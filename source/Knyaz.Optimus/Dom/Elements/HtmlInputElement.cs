using System;
using System.Globalization;
using Knyaz.Optimus.ScriptExecuting;

namespace Knyaz.Optimus.Dom.Elements
{
	/// <summary>
	/// http://www.w3.org/TR/html-markup/input.text.html
	/// </summary>
	public class HtmlInputElement : HtmlElement, IHtmlInputElement, IResettableElement
	{
		static class Defaults
		{
			public static bool Readonly = false;
			public static bool Required = false;
			public static string Value = string.Empty;
			public static string Type = "text";
			public static string Autocomplete = "on";
		}

		public HtmlInputElement(Document ownerDocument) : base(ownerDocument, TagsNames.Input)
		{
			
		}

		/// <summary>
		/// Specifies whether or not an input field should have autocomplete enabled. Available values: "on"|"off".
		/// </summary>
		public string Autocomplete
		{
			get { return GetAttribute("autocomplete", Defaults.Autocomplete); }
			set { SetAttribute("autocomplete", value); }
		}

		private object _value = null;

		public string Value
		{
			get
			{
				if (Type == "number")
				{
					long num;
					if (_value == null && long.TryParse(GetAttribute("value"), out num))
					{
						return num.ToString();
					}

					if (_value is long)
						return _value.ToString();

					return string.Empty;
				}

				return GetAttribute("value", Defaults.Value);
			}
			set
			{
				if (Type == "number" )
				{
					long num;
					_value = long.TryParse(value, out num) ? (object) num : string.Empty;
				}
				else
				{
					SetAttribute("value", value);
				}
			}
		}

		public bool Disabled
		{
			get { return AttributeMapper.GetExistAttributeValue(this, "disabled"); }
			set { AttributeMapper.SetExistAttributeValue(this, "disabled", value); }
		}

		public string Type
		{
			get { return GetAttribute("type", Defaults.Type); }
			set { SetAttribute("type", value); }
		}

		public bool Readonly
		{
			get { return GetAttribute("readonly", Defaults.Readonly); }
			set { SetAttribute("readonly", value.ToString()); }
		}

		public bool Required
		{
			get { return GetAttribute("required", Defaults.Required); }
			set { SetAttribute("required", value.ToString()); }
		}

		private bool? _checked;
		public bool Checked
		{
			get { return _checked ?? HasAttribute("checked"); }
			set { _checked = value; } 
		}

		void IResettableElement.Reset()
		{
			//todo: implement
			//The reset algorithm for input elements is to set the dirty value flag and dirty checkedness flag back to false, 
			//set the value of the element to the value of the value content attribute, if there is one, or the empty string otherwise, 
			//set the checkedness of the element to true if the element has a checked content attribute and false if it does not, 
			//empty the list of selected files, and then invoke the value sanitization algorithm, if the type attribute's current 
			//state defines one.

			throw new System.NotImplementedException();
		}

		#region . type = number

		public string Max
		{
			get { return GetAttribute("max", string.Empty); }
			set { SetAttribute("max", value); }
		}

		public string Min
		{
			get { return GetAttribute("min", string.Empty); }
			set { SetAttribute("min", value); }
		}

		public string Step
		{
			get { return GetAttribute("step", string.Empty); }
			set { SetAttribute("step", value);}
		}

		public void StepUp()
		{
			long step;
			StepUp(long.TryParse(Step, out step) ? step : 1);
		}

		public void StepUp(long delta)
		{
			long min;
			if (!long.TryParse(Min, out min))
				min = 0;

			long numValue;
			if (_value == null)
			{
				if (!long.TryParse(GetAttribute("value"), out numValue))
					numValue = min;
			}
			else if(_value is long)
			{
				numValue = (long) _value;
			}
			else
			{
				numValue = 0;
			}

			numValue += delta;

			long max;
			if (long.TryParse(Max, out max) && numValue > max)
				numValue = max;

			long step;
			if(long.TryParse(Step, out step))
				numValue = ((numValue - min) / delta) * delta + min;

			_value = numValue;
		}

		public void StepDown()
		{
			long step;
			StepDown(long.TryParse(Step, out step) ? step : 1);
		}

		public void StepDown(long delta)
		{
			long min;
			if (!long.TryParse(Min, out min))
				min = 0;

			long numValue;
			if (_value == null)
			{
				if (!long.TryParse(GetAttribute("value"), out numValue))
					numValue = min;
			}
			else if (_value is long)
			{
				numValue = (long)_value;
			}
			else
			{
				numValue = 0;
			}

			numValue -= delta;

			if (numValue < min)
				numValue = min;

			long step;
			if(long.TryParse(Step, out step))
				numValue = (long)Math.Ceiling(((decimal)(numValue - min)) / step) * step + min;

			_value = numValue;
		}

		#endregion

		public decimal ValueAsNumber
		{
			get
			{
				return decimal.Parse(Value, CultureInfo.InvariantCulture);
			}
		}
	}

	[DomItem]
	public interface IHtmlInputElement
	{
		string Value { get; set; }
		bool Disabled { get; set; }
	}
}
