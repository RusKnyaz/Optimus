using System;
using Knyaz.Optimus.Dom.Elements;

namespace Knyaz.Optimus.Dom
{
	class AttributeMappedValue<T>
	{
		private readonly Element _owner;
		private readonly string _attributeName;

		public AttributeMappedValue(Element owner, string attributeName)
		{
			_owner = owner;
			_attributeName = attributeName;
		}

		public T Value
		{
			get
			{
				return (T)Convert.ChangeType(_owner.GetAttribute(_attributeName), typeof(T));
			}
			set
			{
				if (value == null)
					_owner.RemoveAttribute(_attributeName);
				else
					_owner.SetAttribute(_attributeName, value.ToString());
			}
		}
	}

	class AttributeMappedBoolValue
	{
		private readonly Element _owner;
		private readonly string _attributeName;

		public AttributeMappedBoolValue(Element owner, string attributeName)
		{
			_owner = owner;
			_attributeName = attributeName;
		}

		public bool Value
		{
			get
			{
				return _owner.HasAttribute(_attributeName);
			}
			set
			{
				if (value)
					_owner.SetAttributeNode(_owner.OwnerDocument.CreateAttribute(_attributeName));
				else
					_owner.RemoveAttribute(_attributeName);
			}
		}
	}

	internal class AttributeMapper
	{
		public static bool GetExistAttributeValue(HtmlElement element, string attributeName)
		{
			return element.HasAttribute(attributeName);
		}

		public static void SetExistAttributeValue(HtmlElement element, string attributeName, bool value)
		{
			if(value)
				element.SetAttribute(attributeName, string.Empty);
			else
				element.RemoveAttribute(attributeName);
		}
	}
}