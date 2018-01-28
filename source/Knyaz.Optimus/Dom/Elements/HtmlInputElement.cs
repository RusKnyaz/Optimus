using System;
using System.Globalization;
using Knyaz.Optimus.Dom.Events;
using Knyaz.Optimus.ScriptExecuting;

namespace Knyaz.Optimus.Dom.Elements
{
	/// <summary>
	/// http://www.w3.org/TR/html-markup/input.text.html
	/// </summary>
	[DomItem]
	public sealed class HtmlInputElement : HtmlElement, IResettableElement, IFormElement
	{
		static class Defaults
		{
			public static bool Readonly = false;
			public static bool Required = false;
			public static string Value = string.Empty;
			public static string Type = "text";
			public static string Autocomplete = "on";
		}

		internal HtmlInputElement(Document ownerDocument) : base(ownerDocument, TagsNames.Input){}

		private bool _prevChecked;

		protected override void BeforeEventDispatch(Event evt)
		{
			base.BeforeEventDispatch(evt);
			
			if (evt.Type == "click" && Type == "checkbox")
			{
				_prevChecked = Checked;
				Checked = !_prevChecked;
			}
		}

		protected override void AfterEventDispatch(Event evt)
		{
			base.AfterEventDispatch(evt);

			if (evt.Type == "click")
			{
				if (Type == "checkbox" && evt.IsDefaultPrevented())
					Checked = _prevChecked;

				if (Type == "submit" && !evt.IsDefaultPrevented())
					Form?.RaiseSubmit(evt.Target as HtmlElement);
			}
		}

		/// <summary>
		/// Is a <see cref="HtmlFormElement"/> reflecting the form that this button is associated with.
		/// </summary>
		public HtmlFormElement Form => this.FindOwnerForm();
		
		/// <summary>
		/// Gets or sets the 'name' attribute value reflecting the value of the form's name HTML attribute, containing the name of the form.
		/// </summary>
		public string Name
		{
			get => GetAttribute("name", string.Empty);
			set => SetAttribute("name", value);
		}

		/// <summary>
		/// Specifies whether or not an input field should have autocomplete enabled. Available values: "on"|"off".
		/// </summary>
		public string Autocomplete
		{
			get => GetAttribute("autocomplete", Defaults.Autocomplete);
			set => SetAttribute("autocomplete", value);
		}

		private object _value = null;

		/// <summary>
		/// Gets or sets current value of the control.
		/// </summary>
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

		/// <summary>
		/// Gets or sets the element's disabled attribute, indicating that the control is not available for interaction.
		/// </summary>
		public bool Disabled
		{
			get => AttributeMapper.GetExistAttributeValue(this, "disabled");
			set => AttributeMapper.SetExistAttributeValue(this, "disabled", value);
		}

		/// <summary>
		/// Gets or sets the element's type attribute, indicating the type of control to display.
		/// </summary>
		public string Type
		{
			get => GetAttribute("type", Defaults.Type);
			set => SetAttribute("type", value);
		}

		/// <summary>
		/// Gets or sets the element's readonly attribute, indicating that the user cannot modify the value of the control.
		/// </summary>
		public bool Readonly
		{
			get { return GetAttribute("readonly", Defaults.Readonly); }
			set { SetAttribute("readonly", value.ToString()); }
		}

		/// <summary>
		/// Gets or sets the element's required attribute, indicating that the user must fill in a value before submitting a form.
		/// </summary>
		public bool Required
		{
			get { return GetAttribute("required", Defaults.Required); }
			set { SetAttribute("required", value.ToString()); }
		}

		private bool? _checked;
		
		/// <summary>
		/// Gets or sets the current state of the element when type is checkbox or radio.
		/// </summary>
		public bool Checked
		{
			get => _checked ?? HasAttribute("checked");
			set => _checked = value;
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

		/// <summary>
		///  Gets or sets the element's max attribute, containing the maximum (numeric or date-time) value for this item, which must not be less than its minimum (min attribute) value.
		/// </summary>
		public string Max
		{
			get => GetAttribute("max", string.Empty);
			set => SetAttribute("max", value);
		}

		/// <summary>
		/// Gets or sets the element's min attribute, containing the minimum (numeric or date-time) value for this item, which must not be greater than its maximum (max attribute) value.
		/// </summary>
		public string Min
		{
			get => GetAttribute("min", string.Empty);
			set => SetAttribute("min", value);
		}

		/// <summary>
		/// Gets or sets the element's step attribute, which works with min and max to limit the increments at which a numeric or date-time value can be set. It can be the string any or a positive floating point number.
		/// </summary>
		public string Step
		{
			get => GetAttribute("step", string.Empty);
			set => SetAttribute("step", value);
		}

		/// <summary>
		/// Increments the value by <see cref="HtmlInputElement.Step"/>.
		/// </summary>
		public void StepUp()
		{
			long step;
			StepUp(long.TryParse(Step, out step) ? step : 1);
		}

		/// <summary>
		/// Increments the value by (<see cref="HtmlInputElement.Step"/> * n), where n defaults to 1 if not specified.
		/// </summary>
		public void StepUp(long n)
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

			numValue += n;

			long max;
			if (long.TryParse(Max, out max) && numValue > max)
				numValue = max;

			long step;
			if(long.TryParse(Step, out step))
				numValue = ((numValue - min) / n) * n + min;

			_value = numValue;
		}

		/// <summary>
		/// Dencrements the value by <see cref="HtmlInputElement.Step"/>.
		/// </summary>
		public void StepDown()
		{
			long step;
			StepDown(long.TryParse(Step, out step) ? step : 1);
		}

		/// <summary>
		/// Dencrements the value by (<see cref="HtmlInputElement.Step"/> * n), where n defaults to 1 if not specified.
		/// </summary>
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

		/// <summary>
		/// Returns the value of the element, interpreted as one of the following, in order:
		/// a time value
		/// a number
		/// NaN if conversion is impossible
		/// </summary>
		public decimal? ValueAsNumber => decimal.Parse(Value, CultureInfo.InvariantCulture);
	}
}
