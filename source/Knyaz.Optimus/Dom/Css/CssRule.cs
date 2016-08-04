namespace Knyaz.Optimus.Dom.Css
{
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
}