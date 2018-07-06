namespace Knyaz.Optimus.Dom.Css
{
	/// <summary>
	/// The CssRule is the abstract base interface for any type of CSS statement. This includes both rule sets and at-rules. 
	/// An implementation is expected to preserve all rules specified in a CSS style sheet, even if the rule is not recognized by the parser.
	/// Unrecognized rules are represented using the CSSUnknownRule interface.
	/// </summary>
	public class CssRule
	{
		private string _cssText;

		internal CssRule(CssStyleSheet parentStyleSheet)
		{
			ParentStyleSheet = parentStyleSheet;
		}

		/// <summary>
		/// Gets or sets parsable textual representation of the rule. 
		/// This reflects the current state of the rule and not its initial value.
		/// </summary>
		public virtual string CssText
		{
			get => _cssText;
			set { }
		}


		/// <summary>
		/// The style sheet that contains this rule.
		/// </summary>
		public CssStyleSheet ParentStyleSheet { get; private set; }
		
		/// <summary>
		/// The type of the rule.
		/// </summary>
		public int Type { get; set; }

		/// <summary>
		/// 1
		/// </summary>
		public const int STYLE_RULE = 1;
		
		/// <summary>
		/// 3, CssImportRule
		/// </summary>
		public const int IMPORT_RULE = 3;
		
		/// <summary>
		/// 4, CssMediaRule
		/// </summary>
		public const int MEDIA_RULE = 4;
		
		/// <summary>
		/// 5, CssFontFaceRule
		/// </summary>
		public const int FONT_FACE_RULE = 5;
		
		/// <summary>
		/// 6, CssPageRule
		/// </summary>
		public const int PAGE_RULE = 6;
		
		/// <summary>
		/// 7, CssKeyframesRule
		/// </summary>
		public const int KEYFRAMES_RULE = 7;
		
		/// <summary>
		/// 8, CssKeyframeRule
		/// </summary>
		public const int KEYFRAME_RULE = 8;
		
		/// <summary>
		/// 10, CssNamespaceRule
		/// </summary>
		public const int NAMESPACE_RULE = 10;
		
		/// <summary>
		/// 11, CssCounterStyleRule
		/// </summary>
		public const int COUNTER_STYLE_RULE = 11;
		
		/// <summary>
		/// 12, CssSupportsRule
		/// </summary>
		public const int SUPPORTS_RULE = 12;
		
		/// <summary>
		/// 13, CssDocumentRule
		/// </summary>
		public const int DOCUMENT_RULE = 13;
		
		/// <summary>
		/// 14, CssFontFeatureValuesRule
		/// </summary>
		public const int FONT_FEATURE_VALUES_RULE = 14;
		
		/// <summary>
		/// 15, CssViewportRule
		/// </summary>
		public const int VIEWPORT_RULE = 15;
		
		/// <summary>
		/// 16, CssRegionStyleRule
		/// </summary>
		public const int REGION_STYLE_RULE = 16;	 	 
	}
}