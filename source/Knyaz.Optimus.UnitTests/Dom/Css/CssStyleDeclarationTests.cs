using System.Reflection;
using Knyaz.Optimus.Dom.Css;
using NUnit.Framework;

namespace Knyaz.Optimus.Tests.Dom.Css
{
	[TestFixture]
	public class CssStyleDeclarationTests
	{
		[Test]
		public void SetOneCssTest()
		{
			var style = new CssStyleDeclaration {CssText = "background-color:green"};
			Assert.AreEqual("green", style.GetPropertyValue("background-color"));
		}

		[Test]
		public void AddRuleTest()
		{
			var style = new CssStyleSheet();
			style.InsertRule("div {width:100px}", 0);
			Assert.AreEqual(1, style.CssRules.Count);
			var rule = style.CssRules[0] as CssStyleRule;
			Assert.IsNotNull(rule);
			Assert.AreEqual("div", rule.SelectorText);
			Assert.AreEqual("100px", rule.Style.GetPropertyValue("width"));
		}

		private CssStyleDeclaration Style(string cssText)
		{
			return new CssStyleDeclaration() {CssText = cssText};
		}

		[Test]
		public void SetBackground()
		{
			Style("background:#ffffff").Assert(style => 
				style.GetPropertyValue("background-color") == "#ffffff");
		}

		[Test]
		public void SetBorderTest()
		{
			Style("border:1px solid white").Assert(style => 
				style.GetPropertyValue("border-top-width") == "1px" &&
				style.GetPropertyValue("border-top-style") == "solid" &&
				style.GetPropertyValue("border-top-color") == "white" &&
			
				style.GetPropertyValue("border-right-width") == "1px" &&
				style.GetPropertyValue("border-right-style") == "solid" &&
				style.GetPropertyValue("border-right-color") == "white" &&

				style.GetPropertyValue("border-bottom-width") == "1px" &&
				style.GetPropertyValue("border-bottom-style") == "solid" &&
				style.GetPropertyValue("border-bottom-color") == "white" &&

				style.GetPropertyValue("border-left-width") == "1px" &&
				style.GetPropertyValue("border-left-style") == "solid" &&
				style.GetPropertyValue("border-left-color") == "white");
		}

		[TestCase("1px", "1px", "1px", "1px", "1px")]
		[TestCase("1px 2px", "1px", "2px", "1px", "2px")]
		[TestCase("1px 2px 3px", "1px", "2px", "3px", "2px")]
		[TestCase("1px 2px 3px 4px", "1px", "2px", "3px", "4px")]
		public void SetPadding(string padding, string top, string right, string bottom ,string left)
		{
			Style("padding:" + padding).Assert(style =>
				style.GetPropertyValue("padding-top") == top &&
				style.GetPropertyValue("padding-right") == right &&
				style.GetPropertyValue("padding-bottom") == bottom &&
				style.GetPropertyValue("padding-left") == left);
		}

		[TestCase("1px", "1px", "1px", "1px", "1px")]
		[TestCase("1px 2px", "1px", "2px", "1px", "2px")]
		[TestCase("1px 2px 3px", "1px", "2px", "3px", "2px")]
		[TestCase("1px 2px 3px 4px", "1px", "2px", "3px", "4px")]
		public void SetMargin(string margin, string top, string right, string bottom, string left)
		{
			Style("margin:" + margin).Assert(style =>
				style.GetPropertyValue("margin-top") == top &&
				style.GetPropertyValue("margin-right") == right &&
				style.GetPropertyValue("margin-bottom") == bottom &&
				style.GetPropertyValue("margin-left") == left);
		}

		[TestCase("italic bold 12px/30px Georgia, serif", "italic", "normal", "bold", "12px", "Georgia,serif")]
		[TestCase("italic   bold 12px/30px Georgia, serif", "italic", "normal", "bold", "12px", "Georgia,serif")]
		[TestCase("12px Arial, sans-Serif", "normal", "normal", "normal", "12px", "Arial,sans-Serif")]
		[TestCase("normal small-caps 12px/14px fantasy", "normal", "small-caps", "normal", "12px", "fantasy")]
		public void SetFont(string font, string fontStyle, string fontVariant, string fontWeight, string fontSize, string fontFamily)
		{
			Style("font:"+ font).Assert(style =>
				style.GetPropertyValue("font-size") == fontSize &&
				style.GetPropertyValue("font-family") == fontFamily &&
				style.GetPropertyValue("font-weight") == fontWeight &&
				style.GetPropertyValue("font-style") == fontStyle &&
				style.GetPropertyValue("font-variant") == fontVariant);
		}

		[Test]
		public void SetFontLineHeight()
		{
			Style("font:12px/30px Arial").Assert(style =>
				style.GetPropertyValue("font-size") == "12px" &&
				style.GetPropertyValue("line-height") == "30px");
		}

		[Test]
		public void ValueWithSpaces()
		{
			Style("border-top: rgb(255, 255, 255) solid 1px").Assert(style => 
				style.GetPropertyValue("border-top-color") == "rgb(255, 255, 255)" &&
				style.GetPropertyValue("border-top-width") == "1px" &&
				style.GetPropertyValue("border-top-style") == "solid");

			Style("border-color: rgb(255, 255, 255) red black rgb(25, 25, 25)").Assert(style =>
				style.GetPropertyValue("border-top-color") == "rgb(255, 255, 255)" &&
				style.GetPropertyValue("border-right-color") == "red" &&
				style.GetPropertyValue("border-bottom-color") == "black" &&
				style.GetPropertyValue("border-left-color") == "rgb(25, 25, 25)");
		}

		[TestCase("display:none", "display", ExpectedResult = "none", Description = "Valid lowercase 'display' value")]
		[TestCase("display:NOne", "display", ExpectedResult ="none", Description = "Valid uppercase 'display' value")]
		[TestCase("display:abc", "display", ExpectedResult = "", Description = "Invalid 'display' value")]
		[TestCase("font-style:ITALIC", "font-style", ExpectedResult ="italic", Description = "Valid uppercase 'font-style' value")]
		[TestCase("font-style:abc", "font-style", ExpectedResult ="", Description = "Invalid 'font-style' value")]
		[TestCase("border-right-style:dotted", "border-right-style", ExpectedResult ="dotted", Description= "Valid border-right-style")]
		[TestCase("border-style:punktiren", "border-style", ExpectedResult ="", Description="Invalid 'borderstyle' shorthand property  value.")]
		[TestCase("border-style:dotted punktiren", "border-style", ExpectedResult ="", Description="Invalid 'borderstyle' shorthand property  value.")]
		[TestCase("border-style:dotted solid", "border-style", ExpectedResult ="dotted solid", Description="Invalid 'borderstyle' shorthand property  value.")]
		[TestCase("border-style:dotted", "border-style", ExpectedResult ="dotted", Description="Valid 'borderstyle' shorthand property  value.")]
		[TestCase("border-style:dotted", "border-left-style", ExpectedResult ="dotted", Description="Set valid border-left-style via shorthand border-style.")]
		public string StyleValidation(string css, string propertyName) => Style(css).GetPropertyValue(propertyName);

		[Test]
		public void GetBorder()
		{
			Style("border-top: rgb(255, 255, 255) solid 1px;" +
			      "border-right: green dotted 10px;" +
			      "border-bottom: red;" +
			      "border-left: white solid").Assert(style => 
				style.BorderTopColor == "rgb(255, 255, 255)" &&
				style.BorderTopStyle == "solid" &&
				style.BorderTopWidth == "1px" &&
				style.BorderRightColor == "green" &&
				style.BorderRightStyle == "dotted" &&
				style.BorderRightWidth == "10px" &&
				style.BorderBottomColor == "red" &&
				style.BorderLeftColor == "white" &&
				style.BorderLeftStyle == "solid");
		}

		[TestCase("azimuth", nameof(CssStyleDeclaration.Azimuth), "center")]
		[TestCase("background", nameof(CssStyleDeclaration.Background), "url(images/hand.png) repeat-y #fc0")]
		[TestCase("border", nameof(CssStyleDeclaration.Border), "1px solid black")]
		[TestCase("bottom", nameof(CssStyleDeclaration.Bottom), "10pt")]
		[TestCase("clear", nameof(CssStyleDeclaration.Clear), "right")]
		[TestCase("clip", nameof(CssStyleDeclaration.Clip), " rect(40px, auto, auto, 40px)")]
		[TestCase("color", nameof(CssStyleDeclaration.Color), " RGB(249, 201, 16)")]
		[TestCase("content", nameof(CssStyleDeclaration.Content), "open-quote")]
		[TestCase("cue", nameof(CssStyleDeclaration.Cue), "url('ding.mp3) url('dong.mp3')")]
		[TestCase("cue-after", nameof(CssStyleDeclaration.CueAfter), "url('ding.mp3)")]
		[TestCase("cue-before", nameof(CssStyleDeclaration.CueBefore), "url('dong.mp3')")]
		[TestCase("cursor", nameof(CssStyleDeclaration.Cursor), "wait")]
		[TestCase("direction", nameof(CssStyleDeclaration.Direction), "rtl")]
		[TestCase("display", nameof(CssStyleDeclaration.Display), "block")]
		[TestCase("elevation", nameof(CssStyleDeclaration.Elevation), "20deg")]
		[TestCase("float", nameof(CssStyleDeclaration.Float), "right")]
		[TestCase("font", nameof(CssStyleDeclaration.Font), " 200% serif")]
		[TestCase("height", nameof(CssStyleDeclaration.Height), "10px")]
		[TestCase("left", nameof(CssStyleDeclaration.Left), "10px")]
		[TestCase("margin", nameof(CssStyleDeclaration.Margin), "10pt 10pt")]
		[TestCase("marks", nameof(CssStyleDeclaration.Marks), "cross")]
		[TestCase("orphans", nameof(CssStyleDeclaration.Orphans), "100")]
		[TestCase("outline", nameof(CssStyleDeclaration.Outline), " 1px solid #666")]
		[TestCase("overflow", nameof(CssStyleDeclaration.Overflow), "scroll")]
		[TestCase("padding", nameof(CssStyleDeclaration.Padding), "10pt 20pt")]
		[TestCase("page", nameof(CssStyleDeclaration.Page), "123")]
		[TestCase("pause", nameof(CssStyleDeclaration.Pause), "10")]
		[TestCase("pitch", nameof(CssStyleDeclaration.Pitch), "1")]
		[TestCase("position", nameof(CssStyleDeclaration.Position), "relative")]
		[TestCase("quotes", nameof(CssStyleDeclaration.Quotes), ". .")]
		[TestCase("richness", nameof(CssStyleDeclaration.Richness), "1")]
		[TestCase("right", nameof(CssStyleDeclaration.Right), "2")]
		[TestCase("size", nameof(CssStyleDeclaration.Size), "3")]
		[TestCase("speak", nameof(CssStyleDeclaration.Speak), "4")]
		[TestCase("stress", nameof(CssStyleDeclaration.Stress), "5")]
		[TestCase("top", nameof(CssStyleDeclaration.Top), "45pt")]
		[TestCase("visibility", nameof(CssStyleDeclaration.Visibility), "collapse")]
		[TestCase("volume", nameof(CssStyleDeclaration.Volume), "1")]
		[TestCase("widows", nameof(CssStyleDeclaration.Widows), "3")]
		[TestCase("width", nameof(CssStyleDeclaration.Width), "500px")]
		[TestCase("background-attachment", nameof(CssStyleDeclaration.BackgroundAttachment), "1")]
		[TestCase("background-color", nameof(CssStyleDeclaration.BackgroundColor), "red")]
		[TestCase("background-image", nameof(CssStyleDeclaration.BackgroundImage), "url(a)")]
		[TestCase("background-position", nameof(CssStyleDeclaration.BackgroundPosition), "100")]
		[TestCase("background-repeat", nameof(CssStyleDeclaration.BackgroundRepeat), "repeat-x")]
		[TestCase("border-bottom", nameof(CssStyleDeclaration.BorderBottom), "2px dotted green")]
		[TestCase("border-collapse", nameof(CssStyleDeclaration.BorderCollapse), "separate")]
		[TestCase("border-color", nameof(CssStyleDeclaration.BorderColor), "red green")]
		[TestCase("border-left", nameof(CssStyleDeclaration.BorderLeft), "2px dotted green")]
		[TestCase("border-right", nameof(CssStyleDeclaration.BorderRight), "2px dotted green")]
		[TestCase("border-spacing", nameof(CssStyleDeclaration.BorderSpacing), "1pt")]
		[TestCase("border-style", nameof(CssStyleDeclaration.BorderStyle), "solid dotted")]
		[TestCase("border-top", nameof(CssStyleDeclaration.BorderTop), "5px")]
		[TestCase("border-top-color", nameof(CssStyleDeclaration.BorderTopColor), "red")]
		[TestCase("border-width", nameof(CssStyleDeclaration.BorderWidth), "5px")]
		[TestCase("caption-side", nameof(CssStyleDeclaration.CaptionSide), "right")]
		[TestCase("counter-increment", nameof(CssStyleDeclaration.CounterIncrement), "heading")]
		[TestCase("counter-reset", nameof(CssStyleDeclaration.CounterReset), "heading")]
		[TestCase("empty-cells", nameof(CssStyleDeclaration.EmptyCells), "hide")]
		[TestCase("font-family", nameof(CssStyleDeclaration.FontFamily), "arial")]
		[TestCase("font-size", nameof(CssStyleDeclaration.FontSize), "12")]
		[TestCase("font-stretch", nameof(CssStyleDeclaration.FontStretch), "condensed")]
		[TestCase("font-style", nameof(CssStyleDeclaration.FontStyle), "italic")]
		[TestCase("font-variant", nameof(CssStyleDeclaration.FontVariant), "small-caps")]
		[TestCase("font-weight", nameof(CssStyleDeclaration.FontWeight), "bold")]
		[TestCase("letter-spacing", nameof(CssStyleDeclaration.LetterSpacing), "1pt")]
		[TestCase("line-height", nameof(CssStyleDeclaration.LineHeight), "10pt")]
		
		[TestCase("list-style", nameof(CssStyleDeclaration.ListStyle), "square outside")]
		[TestCase("margin-bottom", nameof(CssStyleDeclaration.MarginBottom), "12pt")]
		[TestCase("margin-left", nameof(CssStyleDeclaration.MarginLeft), "12pt")]
		[TestCase("margin-right", nameof(CssStyleDeclaration.MarginRight), "12pt")]
		[TestCase("margin-top", nameof(CssStyleDeclaration.MarginTop), "12pt")]
		[TestCase("marker-offset", nameof(CssStyleDeclaration.MarkerOffset), "12pt")]
		[TestCase("max-height", nameof(CssStyleDeclaration.MaxHeight), "12pt")]
		[TestCase("max-width", nameof(CssStyleDeclaration.MaxWidth), "12pt")]
		[TestCase("min-height", nameof(CssStyleDeclaration.MinHeight), "12pt")]
		[TestCase("min-width", nameof(CssStyleDeclaration.MinWidth), "12pt")]
		[TestCase("outline-color", nameof(CssStyleDeclaration.OutlineColor), "red")]
		[TestCase("outline-style", nameof(CssStyleDeclaration.OutlineStyle), "dashed")]
		[TestCase("outline-width", nameof(CssStyleDeclaration.OutlineWidth), "5px")]
		[TestCase("padding-bottom", nameof(CssStyleDeclaration.PaddingBottom), "12pt")]
		[TestCase("padding-left", nameof(CssStyleDeclaration.PaddingLeft), "12pt")]
		[TestCase("padding-right", nameof(CssStyleDeclaration.PaddingRight), "12pt")]
		[TestCase("padding-top", nameof(CssStyleDeclaration.PaddingTop), "12pt")]
		[TestCase("pause-after", nameof(CssStyleDeclaration.PauseAfter), "100ms")]
		[TestCase("pause-before", nameof(CssStyleDeclaration.PauseBefore), "100ms")]
		[TestCase("pitch-range", nameof(CssStyleDeclaration.PitchRange), "3")]
		[TestCase("play-during", nameof(CssStyleDeclaration.PlayDuring), "3")]
		[TestCase("speak-numeral", nameof(CssStyleDeclaration.SpeakNumeral), "digits")]
		[TestCase("speak-punctuation", nameof(CssStyleDeclaration.SpeakPunctuation), "code")]
		[TestCase("speech-rate", nameof(CssStyleDeclaration.SpeechRate), "slow")]
		[TestCase("table-layout", nameof(CssStyleDeclaration.TableLayout), "fixed")]
		[TestCase("text-align", nameof(CssStyleDeclaration.TextAlign), "center")]
		[TestCase("text-decoration", nameof(CssStyleDeclaration.TextDecoration), "overline")]
		[TestCase("text-indent", nameof(CssStyleDeclaration.TextIndent), "4")]
		[TestCase("text-shadow", nameof(CssStyleDeclaration.TextShadow), "5")]
		[TestCase("text-transform", nameof(CssStyleDeclaration.TextTransform), "1")]
		[TestCase("unicode-bidi", nameof(CssStyleDeclaration.UnicodeBidi), "embed")]
		[TestCase("vertical-align", nameof(CssStyleDeclaration.VerticalAlign), "top")]
		[TestCase("voice-family", nameof(CssStyleDeclaration.VoiceFamily), "asd")]
		[TestCase("white-space", nameof(CssStyleDeclaration.WhiteSpace), "pre")]
		[TestCase("word-spacing", nameof(CssStyleDeclaration.WordSpacing), "5pt")]
		[TestCase("z-index", nameof(CssStyleDeclaration.ZIndex), "7")]
		
		[TestCase("border-bottom-color", nameof(CssStyleDeclaration.BorderBottomColor), "red")]
		[TestCase("border-bottom-style", nameof(CssStyleDeclaration.BorderBottomStyle), "solid")]
		[TestCase("border-bottom-width", nameof(CssStyleDeclaration.BorderBottomWidth), "5pt")]
		[TestCase("border-left-color", nameof(CssStyleDeclaration.BorderLeftColor), "red")]
		[TestCase("border-left-style", nameof(CssStyleDeclaration.BorderLeftStyle), "solid")]
		[TestCase("border-left-width", nameof(CssStyleDeclaration.BorderLeftWidth), "5pt")]
		[TestCase("border-right-color", nameof(CssStyleDeclaration.BorderRightColor), "red")]
		[TestCase("border-right-style", nameof(CssStyleDeclaration.BorderRightStyle), "solid")]
		[TestCase("border-right-width", nameof(CssStyleDeclaration.BorderRightWidth), "5pt")]
		[TestCase("border-top-color", nameof(CssStyleDeclaration.BorderTopColor), "red")]
		[TestCase("border-top-style", nameof(CssStyleDeclaration.BorderTopStyle), "solid")]
		[TestCase("border-top-width", nameof(CssStyleDeclaration.BorderTopWidth), "5pt")]
		public void MapProperty(string cssProperty, string styleProperty, string value)
		{
			var propertyInfo =
				typeof(CssStyleDeclaration).GetProperty(styleProperty, BindingFlags.Public | BindingFlags.Instance);
			
			var style1 = new CssStyleDeclaration();
			style1[cssProperty] = value;
			Assert.AreEqual(value, propertyInfo.GetValue(style1), "Get object property setted by css");
			Assert.AreEqual(value, style1[cssProperty], "Get css property setted by css");

			var style2 = new CssStyleDeclaration();
			propertyInfo.SetValue(style2, value);
			
			Assert.AreEqual(value, propertyInfo.GetValue(style1), "Get object property setted by object property");
			Assert.AreEqual(value, style1[cssProperty], "Get css property setted by object property");
		}
	}
}
