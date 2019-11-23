using System.Collections.Generic;
using System.Linq;

namespace Knyaz.Optimus.Dom.Css
{
	public partial class CssStyleDeclaration
	{
		private static HashSet<string> DisplayValidValues = new HashSet<string>
		{
			"block",
			"inline",
			"inline-block",
			"inline-table",
			"list-item",
			"none",
			"run-in",
			"table",
			"table-caption",
			"table-cell",
			"table-column-group",
			"table-column",
			"table-footer-group",
			"table-header-group",
			"table-row",
			"table-row-group"
		};

		private static HashSet<string> FontStyleValidValues = new HashSet<string>
		{
			"normal",
			"italic",
			"oblique",
			"inherit"
		};

		private static HashSet<string> FontVariantValidValues = new HashSet<string>
		{
			"normal",
			"small-caps",
			"initial",
			"inherit"
		};

		private static HashSet<string> BorderStyleValidValues = new HashSet<string>
		{
			"none",
			"hidden",
			"dotted",
			"dashed",
			"solid",
			"double",
			"groove",
			"ridge",
			"inset",
			"outset",
			"inherit"
		};

		private static HashSet<string> TextAlignValidValues = new HashSet<string>
		{
			"left",
			"right",
			"center",
			"justify",
			"initial",
			"inherit"
		};

		private static HashSet<string> FloatValidValues = new HashSet<string>
		{
			"none",
			"left",
			"right",
			"initial",
			"inherit"
		};

		private static HashSet<string> DirectionValidValues = new HashSet<string>
		{
			"ltr",
			"rtl",
			"initial",
			"inherit"
		};

		private static HashSet<string> FontStretchValidValues = new HashSet<string>
		{
			"ultra-condensed",
			"extra-condensed",
			"condensed",
			"semi-condensed",
			"normal",
			"semi-expanded",
			"expanded",
			"extra-expanded",
			"ultra-expanded",
			"initial",
			"inherit"
		};

		private static HashSet<string> PositionValidValues = new HashSet<string>
		{
			"static",
			"absolute",
			"fixed",
			"relative",
			"sticky",
			"initial",
			"inherit"
		};

		private static HashSet<string> VisibilityValidValues = new HashSet<string>
		{
			"visible",
			"hidden",
			"collapse",
			"initial",
			"inherit"
		};

		private static HashSet<string> OverflowValidValues = new HashSet<string>
		{
			"visible",
			"hidden",
			"scroll",
			"auto",
			"initial",
			"inherit"
		};

		/// <summary>
		/// Validates property value and returns normalized value.
		/// </summary>
		/// <param name="propertyName">Property name to be validated</param>
		/// <param name="value">Property value.</param>
		/// <returns>Valid property value.</returns>
		private static string Validate(string propertyName, string value) => 
			GetValidValues(propertyName) is HashSet<string> avaliableValues
			? Validate(value.ToLowerInvariant(), avaliableValues)
			: value;


		private static string ValidateComplex(string propertyName, string value)
		{
			switch (propertyName)
			{
				case Css.BorderStyle:
					var validValues = BorderStyleValidValues;
					return value.Split(' ').Any(x => !validValues.Contains(x.ToLower())) ? string.Empty : value;
			}
			
			return Validate(propertyName, value);
		} 
			

		private static string Validate(string value, HashSet<string> avaliableValues) =>
			avaliableValues.Contains(value) ? value : "";

		private static HashSet<string> GetValidValues(string propertyName)
		{
			switch (propertyName)
			{
				case Css.Display: return DisplayValidValues;
				case Css.FontStyle: return FontStyleValidValues;
				case Css.BorderStyle:
				case Css.BorderRightStyle:
				case Css.BorderTopStyle:
				case Css.BorderBottomStyle:
				case Css.BorderLeftStyle: return BorderStyleValidValues;
				case Css.TextAlign: return TextAlignValidValues;
				case Css.Float: return FloatValidValues;
				case Css.Direction: return DirectionValidValues;
				case Css.FontStretch: return FontStretchValidValues;
				case Css.FontVariant: return FontVariantValidValues;
				case Css.Position: return PositionValidValues;
				case Css.Visibility: return VisibilityValidValues;
				case Css.Overflow: return OverflowValidValues;
				default: return null;
			}
		}
	}
}