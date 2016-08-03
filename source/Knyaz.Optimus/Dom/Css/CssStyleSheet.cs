using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Knyaz.Optimus.Dom.Elements;

namespace Knyaz.Optimus.Dom.Css
{
	public class CssStyleSheet
	{
		public CssStyleSheet()
		{
			CssRules = new List<CssRule>();
		}

		public IList<CssRule> CssRules { get; private set; }

		public void DeleterRule(int idx)
		{
			CssRules.RemoveAt(idx);
		}

		public void InsertRule(string rule, int idx)
		{
			//todo: parse rule
			//CssRules.Insert(idx, rule);
		}
	}

	public class CssRule
	{
		public CssRule(CssStyleSheet parentStyleSheet)
		{
			ParentStyleSheet = parentStyleSheet;
		}

		public string Text { get; set; }
		public CssStyleSheet ParentStyleSheet { get; private set; }
		public int Type { get; set; }

		///	CSSStyleRule	The most common kind of rule:selector { prop1: val1; prop2: val2; }
		public const int STYLE_RULE = 1;

		public const int IMPORT_RULE = 3;
			//	CSSImportRule	An @import rule. (Until the documentation is completed, see the interface definition in the Mozilla source code: nsIDOMCSSImportRule.)

		public const int MEDIA_RULE = 4; //	CSSMediaRule	 
		public const int FONT_FACE_RULE = 5; //	CSSFontFaceRule	 
		public const int PAGE_RULE = 6; //	CSSPageRule	 
		public const int KEYFRAMES_RULE = 7; //	CSSKeyframesRule 	 
		public const int KEYFRAME_RULE = 8; //	CSSKeyframeRule 	 

		public const int NAMESPACE_RULE = 10; //	CSSNamespaceRule 	 
		public const int COUNTER_STYLE_RULE = 11; //	CSSCounterStyleRule 	 
		public const int SUPPORTS_RULE = 12; //	CSSSupportsRule	 
		public const int DOCUMENT_RULE = 13; //	CSSDocumentRule 	 
		public const int FONT_FEATURE_VALUES_RULE = 14; //	CSSFontFeatureValuesRule	 
		public const int VIEWPORT_RULE = 15; //CSSViewportRule 	 
		public const int REGION_STYLE_RULE = 16; //	CSSRegionStyleRule 	 
	}

	public class CssStyleRule : CssRule
	{
		public string SelectorText { get; set; }
		public CssStyleDeclaration Style { get; private set; }

		public CssStyleRule(CssStyleSheet parentStyleSheet) : base(parentStyleSheet)
		{
			Style = new CssStyleDeclaration();
		}
	}

	/*public class CssStyleDeclaration
	{
		private IDictionary<string, string> _properties;

		public CssStyleDeclaration(CssStyleRule parentRule)
		{
			ParentRule = parentRule;
			_properties = new Dictionary<string, string>();
		}

		public string CssText { get; set; }
		public CssStyleRule ParentRule { get; private set; }

		public string GetPropertyValue(string propertyName)
		{
			return _properties[propertyName];
		}

		public string Item(int idx)
		{
			throw new NotImplementedException();
		}
/*
		CSSStyleDeclaration.getPropertyPriority()
Returns the optional priority, "important". Example: priString= styleObj.getPropertyPriority('color')
CSSStyleDeclaration.item()
Returns a property name. Example: nameString= styleObj.item(0) Alternative: nameString= styleObj[0]
CSSStyleDeclaration.removeProperty()
Returns the value deleted. Example: valString= styleObj.removeProperty('color')
CSSStyleDeclaration.setProperty()
No return. Example: styleObj.setProperty('color', 'red', 'important')#1#
	}*/
}
