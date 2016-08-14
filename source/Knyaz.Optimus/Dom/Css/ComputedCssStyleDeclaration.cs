using System;

namespace Knyaz.Optimus.Dom.Css
{
	internal class ComputedCssStyleDeclaration : ICssStyleDeclaration
	{
		private readonly ICssStyleDeclaration[] _styles;

		public ComputedCssStyleDeclaration(ICssStyleDeclaration[] styles)
		{
			_styles = styles;
		}

		public object this[string name]
		{
			get
			{
				int number;
				return int.TryParse(name, out number) ? this[number] : GetPropertyValue(name);
			}
		}

		public string this[int idx]
		{
			get { throw new NotImplementedException(); }
		}

		public string GetPropertyValue(string propertyName)
		{
			foreach (var style in _styles)
			{
				var val = style.GetPropertyValue(propertyName);
				if (val != null)
					return val;
			}
			return null;
		}
	}
}