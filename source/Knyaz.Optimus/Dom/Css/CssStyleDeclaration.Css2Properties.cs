namespace Knyaz.Optimus.Dom.Css
{
	partial class CssStyleDeclaration
	{
		/// <summary>
		/// This property is most likely to be implemented by mixing the same signal into different channels at differing volumes. It might also use phase shifting, digital delay, and other such techniques to provide the illusion of a sound stage. The precise means used to achieve this effect and the number of speakers used to do so are user agent-dependent; this property merely identifies the desired end result. 
		/// </summary>
		/// <remarks>
		/// Value: $lt;angle&gt; | [[ left-side | far-left | left | center-left | center | center-right | right | far-right | right-side ] || behind ] | leftwards | rightwards | inherit 
		/// </remarks>
		public string Azimuth
		{
			get => GetPropertyValue(Css.Azimuth);
			set => SetProperty(Css.Azimuth, value);
		}

		/// <summary>
		/// Shorthand property for setting the individual background properties (i.e., 'background-color', 'background-image', 'background-repeat', 'background-attachment' and 'background-position') at the same place in the style sheet. 
		/// </summary>
		/// <remarks>
		/// The 'background' property first sets all the individual background properties to their initial values, then assigns explicit values given in the declaration. 
		/// Value: [&lt;'background-color'&gt; || &lt;'background-image'&gt; || &lt;'background-repeat'&gt; || &lt;'background-attachment'&gt; || &lt;'background-position'&gt;] | inherit 
		/// </remarks>
		public string Background
		{
			get => GetPropertyValue(Css.Background);
			set => SetProperty(Css.Background, value);
		}

		/// <summary>
		/// Gets or sets the 'background-attachment' css property. If a background image is specified, this property specifies whether it is fixed with regard to the viewport ('fixed') or scrolls along with the document ('scroll'). 
		/// </summary>
		public string BackgroundAttachment
		{
			get => GetPropertyValue(Css.BackgroundAttachment);
			set => SetProperty(Css.BackgroundAttachment, value);
		}

		/// <summary>
		/// Gets or sets the 'background-color' css property which represents the background color of an element, either a &lt;color&gt; value or the keyword 'transparent', to make the underlying colors shine through.
		/// </summary>
		/// <remarks>
		/// Value: &lt;color&gt; | transparent | inherit 
		/// </remarks>
		public string BackgroundColor
		{
			get => GetPropertyValue(Css.BackgroundColor);
			set => SetProperty(Css.BackgroundColor, value);
		}

		/// <summary>
		/// Gets or sets the 'background-image' css property which represents the background image of an element.
		/// </summary>
		/// <remarks>
		/// When setting a background image, authors should also specify a background color that will be used when the image is unavailable. When the image is available, it is rendered on top of the background color.
		/// Value: &lt;uri&gt; | none | inherit 
		/// </remarks>
		public string BackgroundImage
		{
			get => GetPropertyValue(Css.BackgroundImage);
			set => SetProperty(Css.BackgroundImage, value);
		}

		/// <summary>
		/// If a background image has been specified, this property specifies its initial position. Values have the following meanings:
		/// </summary>
		/// <remarks>
		/// [ [&lt;percentage&gt; | &lt;length&gt; ]{1,2} | [ [top | center | bottom] || [left | center | right] ] ] | inherit
		/// http://www.w3.org/TR/1998/REC-CSS2-19980512/colors.html#propdef-background-position
		/// </remarks>
		public string BackgroundPosition
		{
			get => GetPropertyValue(Css.BackgroundPosition);
			set => SetProperty(Css.BackgroundPosition, value);
		}

		/// <summary>
		/// If a background image is specified, this property specifies whether the image is repeated (tiled), 
		/// and how. All tiling covers the content and padding areas of a box. 
		/// </summary>
		/// <remarks>Values have the following meanings:
		/// repeat - The image is repeated both horizontally and vertically.
		/// repeat-x - The image is repeated horizontally only.
		/// repeat-y - The image is repeated vertically only.
		/// no-repeat - The image is not repeated: only one copy of the image is drawn.
		/// </remarks>
		public string BackgroundRepeat
		{
			get => GetPropertyValue(Css.BackgroundRepeat);
			set => SetProperty(Css.BackgroundRepeat, value);
		}

		/// <summary>
		/// Get or sets the 'border' css property. The 'border' property is a shorthand property for setting the same width, color, and style for all four borders of a box. Unlike the shorthand 'margin' and 'padding' properties, the 'border' property cannot set different values on the four borders. To do so, one or more of the other border properties must be used.
		/// </summary>
		/// <remarks>
		/// Value: [ &lt;'border-width'&gt; || &lt;'border-style'&gt; || &lt;color&gt; ] | inherit 
		/// </remarks>
		public string Border
		{
			get => GetPropertyValue(Css.Border);
			set => SetProperty(Css.Border, value);
		}

		/// <summary>
		/// Gets or sets the 'border-collapse' css property. This property selects a table's border model. The value 'separate' selects the separated borders border model. The value 'collapse' selects the collapsing borders model. The models are described below. 
		/// </summary>
		/// <remarks>
		/// Value: collapse | separate | inherit 
		/// </remarks>
		public string BorderCollapse
		{
			get => GetPropertyValue(Css.BorderCollapse);
			set => SetProperty(Css.BorderCollapse, value);
		}

		/// <summary>
		/// Gets or sets the 'border-color' css property. The 'border-color' property sets the color of the four borders.
		/// </summary>
		/// <remarks>
		/// Value: &lt;color&gt;{1,4} | transparent | inherit 
		/// </remarks>
		public string BorderColor
		{
			get => GetPropertyValue(Css.BorderColor);
			set => SetProperty(Css.BorderColor, value);
		}

		/// <summary>
		/// Gets or sets the 'border-spacing' css property. The lengths specify the distance that separates adjacent cell borders. If one length is specified, it gives both the horizontal and vertical spacing. If two are specified, the first gives the horizontal spacing and the second the vertical spacing. Lengths may not be negative. 
		/// </summary>
		/// <remarks>
		/// Value: &lt;length&gt; &lt;length&gt;? | inherit 
		/// </remarks>
		public string BorderSpacing
		{
			get => GetPropertyValue(Css.BorderSpacing);
			set => SetProperty(Css.BorderSpacing, value);
		}

		/// <summary>
		/// Gets or sets the 'border-style' css property. The 'border-style' property represents the style of the four borders. It can have from one to four values, and the values are set on the different sides as for 'border-width' above. 
		/// </summary>
		/// <remarks>
		/// Value: &lt;border-style&gt;{1,4} | inherit 
		/// </remarks>
		public string BorderStyle
		{
			get => GetPropertyValue(Css.BorderStyle);
			set => SetProperty(Css.BorderStyle, value);
		}

		/// <summary>
		/// Gets or sets the 'border-top' css property. This is a shorthand property for setting the width, style, and color of the top border of a box. 
		/// </summary>
		public string BorderTop
		{
			get => GetPropertyValue(Css.BorderTop);
			set => SetProperty(Css.BorderTop, value);
		}

		/// <summary>
		/// Gets or sets the 'border-right' css property. This is a shorthand property for setting the width, style, and color of the right border of a box. 
		/// </summary>
		public string BorderRight
		{
			get => GetPropertyValue(Css.BorderRight);
			set => SetProperty(Css.BorderRight, value);
		}

		/// <summary>
		/// Gets or sets the 'border-bottom' css property. This is a shorthand property for setting the width, style, and color of the bottom border of a box. 
		/// </summary>
		public string BorderBottom
		{
			get => GetPropertyValue(Css.BorderBottom);
			set => SetProperty(Css.BorderBottom, value);
		}

		/// <summary>
		/// Gets or sets the 'border-left' css property. This is a shorthand property for setting the width, style, and color of the left border of a box. 
		/// </summary>
		public string BorderLeft
		{
			get => GetPropertyValue(Css.BorderLeft);
			set => SetProperty(Css.BorderLeft, value);
		}

		/// <summary>
		/// Gets or sets the 'border-top-color' css property.
		/// </summary>
		public string BorderTopColor
		{
			get => GetPropertyValue(Css.BorderTopColor);
			set => SetProperty(Css.BorderTopColor, value);
		}

		/// <summary>
		/// Gets or sets the 'border-right-color' css property.
		/// </summary>
		public string BorderRightColor
		{
			get => GetPropertyValue(Css.BorderRightColor);
			set => SetProperty(Css.BorderRightColor, value);
		}

		/// <summary>
		/// Gets or sets the 'border-bottom-color' css property.
		/// </summary>
		public string BorderBottomColor
		{
			get => GetPropertyValue(Css.BorderBottomColor);
			set => SetProperty(Css.BorderBottomColor, value);
		}

		/// <summary>
		/// Gets or sets the 'border-left-color' css property.
		/// </summary>
		public string BorderLeftColor
		{
			get => GetPropertyValue(Css.BorderLeftColor);
			set => SetProperty(Css.BorderLeftColor, value);
		}

		/// <summary>
		/// Gets or sets the 'border-top-style' css property. Specifies the top line style of a box's border.
		/// </summary>
		/// <remarks>
		/// Value: dotted|dashed|double|groove|ridge|inset|outset|solid
		/// </remarks>
		public string BorderTopStyle
		{
			get => GetPropertyValue(Css.BorderTopStyle);
			set => SetProperty(Css.BorderTopStyle, value);
		}

		/// <summary>
		/// Gets or sets the 'border-right-style' css property. Specifies the right line style of a box's border.
		/// </summary>
		/// <remarks>
		/// Value: dotted|dashed|double|groove|ridge|inset|outset|solid
		/// </remarks>
		public string BorderRightStyle
		{
			get => GetPropertyValue(Css.BorderRightStyle);
			set => SetProperty(Css.BorderRightStyle, value);
		}

		/// <summary>
		/// Gets or sets the 'border-bottom-style' css property. Specifies the bottom line style of a box's border.
		/// </summary>
		/// <remarks>
		/// Value: dotted|dashed|double|groove|ridge|inset|outset|solid
		/// </remarks>
		public string BorderBottomStyle
		{
			get => GetPropertyValue(Css.BorderBottomStyle);
			set => SetProperty(Css.BorderBottomStyle, value);
		}

		/// <summary>
		/// Gets or sets the 'border-left-style' css property. Specifies the left line style of a box's border.
		/// </summary>
		/// <remarks>
		/// Value: dotted|dashed|double|groove|ridge|inset|outset|solid
		/// </remarks>
		public string BorderLeftStyle
		{
			get => GetPropertyValue(Css.BorderLeftStyle);
			set => SetProperty(Css.BorderLeftStyle, value);
		}

		/// <summary>
		/// Gets or sets the 'border-top-width' css property.
		/// </summary>
		/// <remarks>
		/// Value: thin|medium|thick|&lt;length&gt;
		/// Default value: medium
		/// </remarks>
		public string BorderTopWidth
		{
			get => GetPropertyValue(Css.BorderTopWidth);
			set => SetProperty(Css.BorderTopWidth, value);
		}

		/// <summary>
		/// Gets or sets the 'border-right-width' css property.
		/// </summary>
		/// <remarks>
		/// Value: thin|medium|thick|&lt;length&gt;
		/// Default value: medium
		/// </remarks>
		public string BorderRightWidth
		{
			get => GetPropertyValue(Css.BorderRightWidth);
			set => SetProperty(Css.BorderRightWidth, value);
		}

		/// <summary>
		/// Gets or sets the 'border-bottom-width' css property.
		/// </summary>
		/// <remarks>
		/// Value: thin|medium|thick|&lt;length&gt;
		/// Default value: medium
		/// </remarks>
		public string BorderBottomWidth
		{
			get => GetPropertyValue(Css.BorderBottomWidth);
			set => SetProperty(Css.BorderBottomWidth, value);
		}

		/// <summary>
		/// Gets or sets the 'border-left-width' css property.
		/// </summary>
		/// <remarks>
		/// Value: thin|medium|thick|&lt;length&gt;
		/// Default value: medium
		/// </remarks>
		public string BorderLeftWidth
		{
			get => GetPropertyValue(Css.BorderLeftWidth);
			set => SetProperty(Css.BorderLeftWidth, value);
		}

		/// <summary>
		/// Gets or sets the 'border-width' css property which is a shorthand property for setting 'border-top-width', 'border-right-width', 'border-bottom-width', and 'border-left-width' at the same place in the style sheet. 
		/// </summary>
		/// <remarks>
		/// If there is only one value, it applies to all sides. If there are two values, the top and bottom borders are set to the first value and the right and left are set to the second.If there are three values, the top is set to the first value, the left and right are set to the second, and the bottom is set to the third. If there are four values, they apply to the top, right, bottom, and left, respectively.
		/// Value: (thin|medium|thick|&lt;length&gt;){1,4} | inherit
		/// </remarks>
		public string BorderWidth
		{
			get => GetPropertyValue(Css.BorderWidth);
			set => SetProperty(Css.BorderWidth, value);
		}

		/// <summary>
		/// Specifies how far a box's bottom content edge is offset above the bottom of the box's containing block. 
		/// </summary>
		public string Bottom
		{
			get => GetPropertyValue(Css.Bottom);
			set => SetProperty(Css.Bottom, value);
		}

		/// <summary>
		/// Specifies the position of the caption box with respect to the table box.
		/// </summary>
		/// <remarks>
		/// Value: top | bottom | left | right | inherit 
		/// </remarks>
		public string CaptionSide
		{
			get => GetPropertyValue(Css.CaptionSide);
			set => SetProperty(Css.CaptionSide, value);
		}

		/// <summary>
		/// Indicates which sides of an element's box(es) may not be adjacent to an earlier floating box. (It may be that the element itself has floating descendants; the 'clear' property has no effect on those.) 
		/// </summary>
		/// <remarks>
		/// Value: none | left | right | both | inherit 
		/// </remarks>
		public string Clear
		{
			get => GetPropertyValue(Css.Clear);
			set => SetProperty(Css.Clear, value);
		}

		/// <summary>
		/// Defines what portion of an element's rendered content is visible.
		/// </summary>
		/// <remarks>
		/// Value: &lt;shape&gt; | auto | inherit 
		/// In CSS2, the only valid &lt;shape&gt; value is: rect (&lt;top&gt; &lt;right&gt; &lt;bottom&gt; &lt;left&gt;) where &lt;top&gt;, &lt;bottom&gt; &lt;right&gt;, and &lt;left&gt; specify offsets from the respective sides of the box. 
		///	&lt;top&gt;, &lt;right&gt;, &lt;bottom&gt;, and&lt;left&gt; may either have a&lt;length&gt; value or 'auto'.
		/// </remarks>
		public string Clip
		{
			get => GetPropertyValue(Css.Clip);
			set => SetProperty(Css.Clip, value);
		}

		/// <summary>
		/// Specifies the foreground color of an element's text content.
		/// </summary>
		public string Color
		{
			get => GetPropertyValue(Css.Color);
			set => SetProperty(Css.Color, value);
		}

		/// <summary>
		/// This property is used with the :before and :after pseudo-elements to generate content in a document.
		/// </summary>
		/// <remarks>
		/// Value: [ &lt;string&gt; | &lt;uri&gt; | &lt;counter&gt; | attr(X) | open-quote | close-quote | no-open-quote | no-close-quote ]+ | inherit 
		/// </remarks>
		public string Content
		{
			get => GetPropertyValue(Css.Content);
			set => SetProperty(Css.Content, value);
		}

		/// <summary>
		/// Gets or sets the 'counter-increment' property which accepts one or more names of counters (identifiers), each one optionally followed by an integer. The integer indicates by how much the counter is incremented for every occurrence of the element. The default increment is 1.
		/// </summary>
		public string CounterIncrement
		{
			get => GetPropertyValue(Css.CounterIncrement);
			set => SetProperty(Css.CounterIncrement, value);
		}

		/// <summary>
		/// Gets or sets the 'counter-reset' property which contains a list of one or more names of counters, each one optionally followed by an integer. The integer gives the value that the counter is set to on each occurrence of the element.
		/// </summary>
		public string CounterReset
		{
			get => GetPropertyValue(Css.CounterReset);
			set => SetProperty(Css.CounterReset, value);
		}

		/// <summary>
		/// Gets or sets the 'cue' property which is a shorthand for setting 'cue-before' and 'cue-after'. If two values are given, the first value is 'cue-before' and the second is 'cue-after'. If only one value is given, it applies to both properties.
		/// </summary>
		public string Cue
		{
			get => GetPropertyValue(Css.Cue);
			set => SetProperty(Css.Cue, value);
		}

		/// <summary>
		/// Auditory icons are another way to distinguish semantic elements. Sounds may be played before and/or after the element to delimit it
		/// </summary>
		public string CueAfter
		{
			get => GetPropertyValue(Css.CueAfter);
			set => SetProperty(Css.CueAfter, value);
		}

		/// <summary>
		/// Auditory icons are another way to distinguish semantic elements. Sounds may be played before and/or after the element to delimit it
		/// </summary>
		public string CueBefore
		{
			get => GetPropertyValue(Css.CueBefore);
			set => SetProperty(Css.CueBefore, value);
		}

		/// <summary>
		/// Specifies the type of cursor to be displayed for the pointing device. Values have the following meanings:
		/// </summary>
		/// <remarks>
		/// Value:[ [&lt;uri&gt; ,]* [ auto | crosshair | default | pointer | move | e-resize | ne-resize | nw-resize | n-resize | se-resize | sw-resize | s-resize | w-resize| text | wait | help ] ] | inherit 
		/// </remarks>
		public string Cursor
		{
			get => GetPropertyValue(Css.Cursor);
			set => SetProperty(Css.Cursor, value);
		}

		/// <summary>
		/// Specifies the base writing direction of blocks and the direction of embeddings and overrides (see 'unicode-bidi') for the Unicode bidirectional algorithm. In addition, it specifies the direction of table column layout, the direction of horizontal overflow, and the position of an incomplete last line in a block in case of 'text-align: justify'.
		/// </summary>
		/// <remarks>
		/// Value: ltr | rtl | inherit
		/// </remarks>
		public string Direction
		{
			get => GetPropertyValue(Css.Direction);
			set => SetProperty(Css.Direction, value);
		}

		/// <summary>
		/// Specifies how the item to be layed out.
		/// </summary>
		/// <remarks>
		/// Value:inline | block | list-item | run-in | compact | marker | table | inline-table | table-row-group | table-header-group | table-footer-group | table-row | table-column-group | table-column | table-cell | table-caption | none | inherit 
		/// block -	This value causes an element to generate a principal block box.
		/// inline - This value causes an element to generate one or more inline boxes.
		/// list-item -	This value causes an element(e.g., LI in HTML) to generate a principal block box and a list-item inline box.For information about lists and examples of list formatting, please consult the section on lists.
		/// marker - This value declares generated content before or after a box to be a marker. This value should only be used with :before and :after pseudo-elements attached to block-level elements. In other cases, this value is interpreted as 'inline'. Please consult the section on markers for more information. 
		/// none - This value causes an element to generate no boxes in the formatting structure (i.e., the element has no effect on layout). Descendant elements do not generate any boxes either; this behavior cannot be overridden by setting the 'display' property on the descendants.
		/// run-in and compact - These values create either block or inline boxes, depending on context.Properties apply to run-in and compact boxes based on their final status (inline-level or block-level). For example, the 'white-space' property only applies if the box becomes a block box.
		/// table, inline-table, table-row-group, table-column, table-column-group, table-header-group, table-footer-group, table-row, table-cell, and table-caption - These values cause an element to behave like a table element (subject to restrictions described in the chapter on tables). 
		/// </remarks>
		public string Display
		{
			get => GetPropertyValue(Css.Display);
			set => SetProperty(Css.Display, value);
		}

		/// <summary>
		/// Affects the spatial audio feature.
		/// </summary>
		public string Elevation
		{
			get => GetPropertyValue(Css.Elevation);
			set => SetProperty(Css.Elevation, value);
		}

		/// <summary>
		/// Gets or sets the 'empty-cells' css property which controls the rendering of borders around cells that have no visible content.
		/// </summary>
		/// <remarks>
		/// Empty cells and cells with the 'visibility' property set to 'hidden' are considered to have no visible content. 
		/// Visible content includes " " and other whitespace except ASCII CR ("\0D"), LF ("\0A"), tab ("\09"), 
		/// and space ("\20"). 
		/// When this property has the value 'show', borders are drawn around empty cells (like normal cells). 
		/// A value of 'hide' means that no borders are drawn around empty cells. Furthermore, if all the cells in a row have a value of 'hide' and have no visible content, the entire row behaves as if it had 'display: none'. 
		/// Available value: show | hide | inherit 
		/// </remarks>
		public string EmptyCells
		{
			get => GetPropertyValue(Css.EmptyCells);
			set => SetProperty(Css.EmptyCells, value);
		}

		/// <summary>
		/// Indicates which sides of an element's box(es) may not be adjacent to an earlier floating box. (It may be that the element itself has floating descendants; the 'clear' property has no effect on those.) 
		/// Value: none | left | right | both | inherit 
		/// </summary>
		public string Float
		{
			get => GetPropertyValue(Css.Float);
			set => SetProperty(Css.Float, value);
		}

		/// <summary>
		/// The shorthand property for setting 'font-style', 'font-variant', 'font-weight', 'font-size', 'line-height', and 'font-family', at the same place in the style sheet. The syntax of this property is based on a traditional typographical shorthand notation to set multiple properties related to fonts. 
		/// </summary>
		/// <remarks>
		/// Value: [ [ &lt;'font-style'&gt; || &lt;'font-variant'&gt; || &lt;'font-weight'&gt; ]? &lt;'font-size'&gt; [ / &lt;'line-height'&gt; ]? &lt;'font-family'&gt; ] | caption | icon | menu | message-box | small-caption | status-bar | inherit 
		/// </remarks>
		public string Font
		{
			get => GetPropertyValue(Css.Font);
			set => SetProperty(Css.Font, value);
		}

		/// <summary>
		/// Gets or sets the 'font-family' css property which specifies a prioritized list of font family names and/or generic family names.
		/// </summary>
		public string FontFamily
		{
			get => GetPropertyValue(Css.FontFamily);
			set => SetProperty(Css.FontFamily, value);
		}

		/// <summary>
		/// Gets or sets the 'font-size' css property which specifies the size of the font.
		/// </summary>
		/// <remarks>
		/// Value: &lt;absolute-size&gt; | &lt;relative-size&gt; | &lt;length&gt; | &lt;percentage&gt; | inherit 
		/// </remarks>
		public string FontSize
		{
			get => GetPropertyValue(Css.FontSize);
			set => SetProperty(Css.FontSize, value);
		}

		/// <summary>
		/// Gets or sets the 'font-size-adjust' css property. 
		/// </summary>
		/// <remarks>
		/// In bicameral scripts, the subjective apparent size and legibility of a font are less dependent on their 'font-size' value than on the value of their 'x-height', or, more usefully, on the ratio of these two values, called the aspect value (font size divided by x-height). The higher the aspect value, the more likely it is that a font at smaller sizes will be legible. Inversely, faces with a lower aspect value will become illegible more rapidly below a given threshold size than faces with a higher aspect value. Straightforward font substitution that relies on font size alone may lead to illegible characters. 
		/// </remarks>
		public string FontSizeAdjust
		{
			get => GetPropertyValue(Css.FontAdjustSize);
			set => SetProperty(Css.FontAdjustSize, value);
		}

		/// <summary>
		/// Gets or sets the 'font-stretch' css property which selects a normal, condensed, or extended face from a font family.
		/// </summary>
		/// <remarks>
		/// Value: normal | wider | narrower | ultra-condensed | extra-condensed | condensed | semi-condensed | semi-expanded | expanded | extra-expanded | ultra-expanded | inherit 
		/// </remarks>
		public string FontStretch
		{
			get => GetPropertyValue(Css.FontStretch);
			set => SetProperty(Css.FontStretch, value);
		}

		/// <summary>
		/// Gets or sets the 'font-style' css property. Value: normal | italic | oblique | inherit.
		/// </summary>
		public string FontStyle
		{
			get => GetPropertyValue(Css.FontStyle);
			set => SetProperty(Css.FontStyle, value);
		}

		/// <summary>
		/// Gets or sets the 'font-variant' css property. 
		/// </summary>
		/// <remarks>
		/// In a small-caps font, the glyphs for lowercase letters look similar to the uppercase ones, but in a smaller size and with slightly different proportions. The 'font-variant' property requests such a font for bicameral (having two cases, as with Latin script).
		/// Value: normal | small-caps | inherit 
		/// </remarks>
		public string FontVariant
		{
			get => GetPropertyValue(Css.FontVariant);
			set => SetProperty(Css.FontVariant, value);
		}

		/// <summary>
		/// Specifies the weight of the font. Value: normal | bold | bolder | lighter | 100 | 200 | 300 | 400 | 500 | 600 | 700 | 800 | 900 | inherit 
		/// </summary>
		public string FontWeight
		{
			get => GetPropertyValue(Css.FontWeight);
			set => SetProperty(Css.FontWeight, value);
		}

		/// <summary>
		/// Specifies the content height of boxes generated by block-level and replaced elements. This property does not apply to non-replaced inline-level elements.
		/// </summary>
		/// <remarks>
		/// Value: &lt;length&gt; | &lt;percentage&gt; | auto | inherit 
		/// </remarks>
		public string Height
		{
			get => GetPropertyValue(Css.Height);
			set => SetProperty(Css.Height, value);
		}

		/// <summary>
		/// specifies how far a box's left content edge is offset to the right of the left edge of the box's containing block. 
		/// </summary>
		public string Left
		{
			get => GetPropertyValue(Css.Left);
			set => SetProperty(Css.Left, value);
		}

		/// <summary>
		/// Gets or sets the 'letter-spacing' css property which specifies spacing behavior between text characters. Values: normal | &lt;length&gt; | inherit 
		/// </summary>
		public string LetterSpacing
		{
			get => GetPropertyValue(Css.LetterSpacing);
			set => SetProperty(Css.LetterSpacing, value);
		}

		/// <summary>
		/// Gets or sets the 'line-height' css property. If the property is set on a block-level element whose content is composed of inline-level elements, it specifies the minimal height of each generated inline box. 
		/// If the property is set on an inline-level element, it specifies the exact height of each box generated by the element. (Except for inline replaced elements, where the height of the box is given by the 'height' property.) 
		/// </summary>
		public string LineHeight
		{
			get => GetPropertyValue(Css.LineHeight);
			set => SetProperty(Css.LineHeight, value);
		}

		/// <summary>
		/// Gets or sets the 'line-style' css property which is a shorthand notation for setting the three properties 'list-style-type', 'list-style-image', and 'list-style-position'.
		/// </summary>
		public string ListStyle
		{
			get => GetPropertyValue(Css.ListStyle);
			set => SetProperty(Css.ListStyle, value);
		}

		/// <summary>
		/// Gets or sets the 'line-style-image' css property which specifies the image that will be used as the list item marker. Value:  &lt;uri&gt; | none | inherit
		/// </summary>
		/// <remarks>
		/// </remarks>
		public string ListStyleImage
		{
			get => GetPropertyValue(Css.ListStyleImage);
			set => SetProperty(Css.ListStyleImage, value);
		}

		/// <summary>
		/// Gets or sets the 'line-style-position' css property which specifies the position of the marker box in the principal block box. Value: inside | outside | inherit
		/// </summary>
		public string ListStylePosition
		{
			get => GetPropertyValue(Css.ListStylePosition);
			set => SetProperty(Css.ListStylePosition, value);
		}

		/// <summary>
		/// Gets or sets the 'line-style-type' css property which specifies appearance of the list item marker if 'list-style-image' has the value 'none' or if the image pointed to by the URI cannot be displayed.
		/// </summary>
		/// <remarks>Value: disc | circle | square | decimal | decimal-leading-zero | lower-roman | upper-roman | lower-greek | lower-alpha | lower-latin | upper-alpha | upper-latin | hebrew | armenian | georgian | cjk-ideographic | hiragana | katakana | hiragana-iroha | katakana-iroha | none | inherit </remarks>
		public string ListStyleType
		{
			get => GetPropertyValue(Css.ListStyleType);
			set => SetProperty(Css.ListStyleType, value);
		}

		/// <summary>
		/// The shorthand property for setting 'margin-top', 'margin-right', 'margin-bottom', and 'margin-left' at the same place in the style sheet.
		/// </summary>
		/// <remarks>
		/// Value: &lt;margin-width&gt;{1,4} | inherit 
		/// </remarks>
		public string Margin
		{
			get => GetPropertyValue(Css.Margin);
			set => SetProperty(Css.Margin, value);
		}

		/// <summary>
		/// Gets or sets the 'margin-top' css property.
		/// </summary>
		public string MarginTop
		{
			get => GetPropertyValue(Css.MarginTop);
			set => SetProperty(Css.MarginTop, value);
		}

		/// <summary>
		/// Gets or sets the 'margin-right' css property.
		/// </summary>
		public string MarginRight
		{
			get => GetPropertyValue(Css.MarginRight);
			set => SetProperty(Css.MarginRight, value);
		}

		/// <summary>
		/// Gets or sets the 'margin-bottom' css property.
		/// </summary>
		public string MarginBottom
		{
			get => GetPropertyValue(Css.MarginBottom);
			set => SetProperty(Css.MarginBottom, value);
		}

		/// <summary>
		/// Gets or sets the 'margin-left' css property.
		/// </summary>
		public string MarginLeft
		{
			get => GetPropertyValue(Css.MarginLeft);
			set => SetProperty(Css.MarginLeft, value);
		}

		/// <summary>
		/// Gets or sets the 'marker-offset' css property which specifies the distance between the nearest border edges of a marker box and its associated principal box.
		/// </summary>
		public string MarkerOffset
		{
			get => GetPropertyValue(Css.MarkerOffset);
			set => SetProperty(Css.MarkerOffset, value);
		}

		/// <summary>
		/// Specifies whether cross marks or crop marks or both should be rendered just outside the page box edge. 
		/// </summary>
		/// <remarks>
		/// Values: [ crop || cross ] | none | inherit 
		/// Crop marks indicate where the page should be cut.Cross marks(also known as register marks or registration marks) are used to align sheets.
		/// </remarks>
		public string Marks
		{
			get => GetPropertyValue(Css.Marks);
			set => SetProperty(Css.Marks, value);
		}

		/// <summary>
		/// Gets or sets the 'max-height' css property. Value: &lt;length&gt; | &lt;percentage&gt; | inherit 
		/// </summary>
		public string MaxHeight
		{
			get { return GetPropertyValue(Css.MaxHeight); }
			set { SetProperty(Css.MaxHeight, value); }
		}

		/// <summary>
		/// Gets or sets the 'max-width' css property. Value: &lt;length&gt; | &lt;percentage&gt; | inherit 
		/// </summary>
		public string MaxWidth
		{
			get => GetPropertyValue(Css.MaxWidth);
			set => SetProperty(Css.MaxWidth, value);
		}

		/// <summary>
		/// Gets or sets the 'min-height' css property. Value: &lt;length&gt; | &lt;percentage&gt; | inherit 
		/// </summary>
		public string MinHeight
		{
			get => GetPropertyValue(Css.MinHeight);
			set => SetProperty(Css.MinHeight, value);
		}

		/// <summary>
		/// Gets or sets the 'min-width' css property. Value: &lt;length&gt; | &lt;percentage&gt; | inherit 
		/// </summary>
		public string MinWidth
		{
			get => GetPropertyValue(Css.MinWidth);
			set => SetProperty(Css.MinWidth, value);
		}

		/// <summary>
		/// Specifies the minimum number of lines of a paragraph that must be left at the bottom of a page. Value:  &lt;integer&gt; | inherit
		/// </summary>
		public string Orphans
		{
			get => GetPropertyValue(Css.Orphans);
			set => SetProperty(Css.Orphans, value);
		}

		/// <summary>
		/// The shorthand property for setting 'outline-color', 'outline-style' and 'outline-width' properties. 
		/// Specifies outlines around visual objects such as buttons, active form fields, image maps, etc.
		/// </summary>
		/// <remarks>
		/// CSS2 outlines differ from borders in the following ways:
		/// Outlines do not take up space.
		/// Outlines may be non-rectangular.
		/// Value:  [ &lt;'outline-color'&gt; || &lt;'outline-style'&gt; || &lt;'outline-width'&gt; ] | inherit
		/// </remarks>
		public string Outline
		{
			get => GetPropertyValue(Css.Outline);
			set => SetProperty(Css.Outline, value);
		}

		/// <summary>
		/// Gets or sets 'outline-color' css property. <see cref="Outline"/> for more details.
		/// </summary>
		public string OutlineColor
		{
			get => GetPropertyValue(Css.OutlineColor);
			set => SetProperty(Css.OutlineColor, value);
		}

		/// <summary>
		/// Gets or sets 'outline-style' css property.
		/// </summary>
		public string OutlineStyle
		{
			get => GetPropertyValue(Css.OutlineStyle);
			set => SetProperty(Css.OutlineStyle, value);
		}

		/// <summary>
		/// Gets or sets 'outline-width' css property.
		/// </summary>
		public string OutlineWidth
		{
			get => GetPropertyValue(Css.OutlineWidth);
			set => SetProperty(Css.OutlineWidth, value);
		}

		/// <summary>
		/// specifies whether the content of a block-level element is clipped when it overflows the element's box (which is acting as a containing block for the content). 
		/// Value:  visible | hidden | scroll | auto | inherit
		/// </summary>
		public string Overflow
		{
			get => GetPropertyValue(Css.Overflow);
			set => SetProperty(Css.Overflow, value);
		}

		/// <summary>
		/// The shorthand property for setting 'padding-top', 'padding-right', 'padding-bottom', and 'padding-left' at the same place in the style sheet. Value:  &lt;padding-width&gt;{1,4} | inherit
		/// </summary>
		/// <remarks>
		/// If there is only one value, it applies to all sides. If there are two values, the top and bottom paddings are set to the first value and the right and left paddings are set to the second. If there are three values, the top is set to the first value, the left and right are set to the second, and the bottom is set to the third. If there are four values, they apply to the top, right, bottom, and left, respectively. 
		/// </remarks>
		public string Padding
		{
			get => GetPropertyValue(Css.Padding);
			set => SetProperty(Css.Padding, value);
		}

		/// <summary>
		/// Gets or sets the 'padding-top' css property which specifies top padding of a box.
		/// </summary>
		public string PaddingTop
		{
			get => GetPropertyValue(Css.PaddingTop);
			set => SetProperty(Css.PaddingTop, value);
		}

		/// <summary>
		/// Gets or sets the 'padding-right' css property which specifies right padding of a box.
		/// </summary>
		public string PaddingRight
		{
			get => GetPropertyValue(Css.PaddingRight);
			set => SetProperty(Css.PaddingRight, value);
		}

		/// <summary>
		/// Gets or sets the 'padding-bottom' css property which specifies bottom padding of a box.
		/// </summary>
		public string PaddingBottom
		{
			get => GetPropertyValue(Css.PaddingBottom);
			set => SetProperty(Css.PaddingBottom, value);
		}

		/// <summary>
		/// Gets or sets the 'padding-left' css property which specifies left padding of a box.
		/// </summary>
		public string PaddingLeft
		{
			get => GetPropertyValue(Css.PaddingLeft);
			set => SetProperty(Css.PaddingLeft, value);
		}

		/// <summary>
		/// Specifies a particular type of page where an element should be displayed. Value: &lt;identifier&gt; | auto 
		/// </summary>
		public string Page
		{
			get => GetPropertyValue(Css.Page);
			set => SetProperty(Css.Page, value);
		}

		/// <summary>
		/// Gets or sets the 'page-break-after' css property. Value: auto | always | avoid | left | right | inherit 
		/// </summary>
		public string PageBreakAfter
		{
			get => GetPropertyValue(Css.PageBreakAfter);
			set => SetProperty(Css.PageBreakAfter, value);
		}

		/// <summary>
		/// Gets or sets the 'page-break-before' css property. Value: auto | always | avoid | left | right | inherit 
		/// </summary>
		public string PageBreakBefore
		{
			get => GetPropertyValue(Css.PageBreakBefore);
			set => SetProperty(Css.PageBreakBefore, value);
		}

		/// <summary>
		/// Gets or sets the 'page-break-inside' css property. Value: avoid | auto | inherit 
		/// </summary>
		public string PageBreakInside
		{
			get => GetPropertyValue(Css.PageBreakInside);
			set => SetProperty(Css.PageBreakInside, value);
		}

		/// <summary>
		/// The shorthand for setting 'pause-before' and 'pause-after'. If two values are given, the first value is 'pause-before' and the second is 'pause-after'. If only one value is given, it applies to both properties. 
		/// </summary>
		public string Pause
		{
			get => GetPropertyValue(Css.Pause);
			set => SetProperty(Css.Pause, value);
		}

		/// <summary>
		/// Specifies a pause to be observed after speaking an element's content. Value:  &lt;time&gt; | &lt;percentage&gt; | inherit
		/// </summary>
		public string PauseAfter
		{
			get => GetPropertyValue(Css.PauseAfter);
			set => SetProperty(Css.PauseAfter, value);
		}

		/// <summary>
		/// Specifies a pause to be observed before speaking an element's content. Value:  &lt;time&gt; | &lt;percentage&gt; | inherit
		/// </summary>
		public string PauseBefore
		{
			get => GetPropertyValue(Css.PauseBefore);
			set => SetProperty(Css.PauseBefore, value);
		}

		/// <summary>
		/// Specifies the average pitch (a frequency) of the speaking voice. Values: &lt;frequency&gt; | x-low | low | medium | high | x-high | inherit 
		/// </summary>
		public string Pitch
		{
			get => GetPropertyValue(Css.Pitch);
			set => SetProperty(Css.Pitch, value);
		}

		/// <summary>
		/// Specifies variation in average pitch. Value: &lt;number&gt; | inherit
		/// </summary>
		public string PitchRange
		{
			get => GetPropertyValue(Css.PitchRange);
			set => SetProperty(Css.PitchRange, value);
		}

		/// <summary>
		/// Specifies a sound to be played as a background while an element's content is spoken. Value: &lt;uri&gt; mix? repeat? | auto | none | inherit
		/// </summary>
		public string PlayDuring
		{
			get => GetPropertyValue(Css.PlayDuring);
			set => SetProperty(Css.PlayDuring, value);
		}

		/// <summary>
		/// Determines which of the CSS2 positioning algorithms is used to calculate the position of a box. Value:  static | relative | absolute | fixed | inherit
		/// </summary>
		public string Position
		{
			get => GetPropertyValue(Css.Position);
			set => SetProperty(Css.Position, value);
		}

		/// <summary>
		/// Specifies quotation marks for any number of embedded quotations. Value:  [&lt;string&gt; &lt;string&gt;]+ | none | inherit
		/// </summary>
		public string Quotes
		{
			get => GetPropertyValue(Css.Quotes);
			set => SetProperty(Css.Quotes, value);
		}

		/// <summary>
		/// Specifies the richness, or brightness, of the speaking voice. Value:  &lt;number&gt; | inherit
		/// </summary>
		public string Richness
		{
			get => GetPropertyValue(Css.Richness);
			set => SetProperty(Css.Richness, value);
		}

		/// <summary>
		/// Specifies how far a box's right content edge is offset to the left of the right edge of the box's containing block. 
		/// </summary>
		public string Right
		{
			get => GetPropertyValue(Css.Right);
			set => SetProperty(Css.Right, value);
		}

		/// <summary>
		/// Specifies the size and orientation of a page box. Value:  &lt;length&gt;{1,2} | auto | portrait | landscape | inherit
		/// </summary>
		public string Size
		{
			get => GetPropertyValue(Css.Size);
			set => SetProperty(Css.Size, value);
		}

		/// <summary>
		/// Specifies whether text will be rendered aurally and if so, in what manner. Value:  normal | none | spell-out | inherit
		/// </summary>
		public string Speak
		{
			get => GetPropertyValue(Css.Speak);
			set => SetProperty(Css.Speak, value);
		}

		/// <summary>
		/// Gets or sets the 'speak-header' css property which specifies whether table headers are spoken before every cell, or only before a cell when that cell is associated with a different header than the previous cell.
		/// Value: once | always | inherit 
		/// </summary>
		public string SpeakHeader
		{
			get => GetPropertyValue(Css.SpeakHeader);
			set => SetProperty(Css.SpeakHeader, value);
		}

		/// <summary>
		/// Gets or sets the 'speak-numeral' css property which specifies how numerals are spoken. Value: digits | continuous | inherit.
		/// </summary>
		public string SpeakNumeral
		{
			get => GetPropertyValue(Css.SpeakNumeral);
			set => SetProperty(Css.SpeakNumeral, value);
		}

		/// <summary>
		/// Gets or sets the 'speak-punctuation' css property which specifies how punctuation is spoken. Value: code | none | inherit.
		/// </summary>
		/// <remarks>
		/// Values have the following meanings:
		/// code - Punctuation such as semicolons, braces, and so on are to be spoken literally.
		/// none - Punctuation is not to be spoken, but instead rendered naturally as various pauses. 
		/// </remarks>
		public string SpeakPunctuation
		{
			get => GetPropertyValue(Css.SpeakPunctuation);
			set => SetProperty(Css.SpeakPunctuation, value);
		}

		/// <summary>
		/// Gets or sets the 'speech-rate' css property which specifies the speaking rate. Value: &lt;number&gt; | x-slow | slow | medium | fast | x-fast | faster | slower | inherit 
		/// </summary>
		public string SpeechRate
		{
			get => GetPropertyValue(Css.SpeechRate);
			set => SetProperty(Css.SpeechRate, value);
		}

		/// <summary>
		/// Specifies the height of "local peaks" in the intonation contour of a voice. Value: &lt;number&gt; | inherit 
		/// </summary>
		public string Stress
		{
			get => GetPropertyValue(Css.Stress);
			set => SetProperty(Css.Stress, value);
		}

		/// <summary>
		/// Gets or sets the 'table-layout' css property which specifies the algorithm used to lay out the table cells, rows, and columns.
		/// Value: auto | fixed | inherit 
		/// </summary>
		public string TableLayout
		{
			get => GetPropertyValue(Css.TableLayout);
			set => SetProperty(Css.TableLayout, value);
		}

		/// <summary>
		/// Gets or sets the 'text-align' css property which specifies how inline content of a block is aligned. 
		/// Value: left | right | center | justify | &lt;string&gt; | inherit 
		/// </summary>
		public string TextAlign
		{
			get => GetPropertyValue(Css.TextAlign);
			set => SetProperty(Css.TextAlign, value);
		}

		/// <summary>
		/// Gets or sets the 'text-decoration' css property which describes decorations that are added to the text of an element.
		/// Value: none | [ underline || overline || line-through || blink ] | inherit 
		/// </summary>
		public string TextDecoration
		{
			get => GetPropertyValue(Css.TextDecoration);
			set => SetProperty(Css.TextDecoration, value);
		}

		/// <summary>
		/// Gets or sets the 'text-indent' css property.
		/// </summary>
		public string TextIndent
		{
			get => GetPropertyValue(Css.TextIndent);
			set => SetProperty(Css.TextIndent, value);
		}

		/// <summary>
		/// Gets or sets the 'text-shadow' css property which specifies shadow effects to be applied to the text of the element.
		/// </summary>
		/// <remarks>
		/// This property accepts a comma-separated list of shadow effects to be applied to the text of the element. The shadow effects are applied in the order specified and may thus overlay each other, but they will never overlay the text itself.
		/// </remarks>
		public string TextShadow
		{
			get => GetPropertyValue(Css.TextShadow);
			set => SetProperty(Css.TextShadow, value);
		}

		/// <summary>
		/// Gets or sets the 'text-transform' css property which specifies capitalization effects of an element's text.
		/// Value: capitalize | uppercase | lowercase | none | inherit.
		/// </summary>
		public string TextTransform
		{
			get => GetPropertyValue(Css.TextTransform);
			set => SetProperty(Css.TextTransform, value);
		}

		/// <summary>
		/// Specifies how far a box's top content edge is offset below the top edge of the box's containing block. 
		/// </summary>
		public string Top
		{
			get => GetPropertyValue(Css.Top);
			set => SetProperty(Css.Top, value);
		}

		/// <summary>
		/// Gets or sets the 'unicode-bidi' css property. 
		/// Value:  normal | embed | bidi-override | inherit.
		/// </summary>
		/// <remarks>
		/// normal - The element does not open an additional level of embedding with respect to the bidirectional algorithm.For inline-level elements, implicit reordering works across element boundaries. 
		/// embed - If the element is inline-level, this value opens an additional level of embedding with respect to the bidirectional algorithm.The direction of this embedding level is given by the 'direction' property.Inside the element, reordering is done implicitly. This corresponds to adding a LRE (U+202A; for 'direction: ltr') or RLE(U+202B; for 'direction: rtl') at the start of the element and a PDF(U+202C) at the end of the element.
		/// bidi-override - If the element is inline-level or a block-level element that contains only inline-level elements, this creates an override. This means that inside the element, reordering is strictly in sequence according to the 'direction' property; the implicit part of the bidirectional algorithm is ignored.This corresponds to adding a LRO (U+202D; for 'direction: ltr') or RLO(U+202E; for 'direction: rtl') at the start of the element and a PDF(U+202C) at the end of the element.
		/// </remarks>
		public string UnicodeBidi
		{
			get => GetPropertyValue(Css.UnicodeBidi);
			set => SetProperty(Css.UnicodeBidi, value);
		}

		/// <summary>
		/// Gets or sets the 'vertical-align' css property which affects the vertical positioning inside a line box of the boxes generated by an inline-level element.
		/// Value: baseline | sub | super | top | text-top | middle | bottom | text-bottom | &lt;percentage&gt; | &lt;length&gt; | inherit 
		/// </summary>
		public string VerticalAlign
		{
			get => GetPropertyValue(Css.VerticalAlign);
			set => SetProperty(Css.VerticalAlign, value);
		}

		/// <summary>
		/// Specifies whether the boxes generated by an element are rendered.
		/// Value: visible | hidden | collapse | inherit 
		/// </summary>
		public string Visibility
		{
			get => GetPropertyValue(Css.Visibility);
			set => SetProperty(Css.Visibility, value);
		}

		/// <summary>
		/// Gets or sets the 'voice-family' css property which is a comma-separated, prioritized list of voice family names.
		/// </summary>
		public string VoiceFamily
		{
			get => GetPropertyValue(Css.VoiceFamily);
			set => SetProperty(Css.VoiceFamily, value);
		}

		/// <summary>
		/// Specifies the median volume of the waveform.
		/// Value: &lt;number&gt; | &lt;percentage&gt; | silent | x-soft | soft | medium | loud | x-loud | inherit 
		/// </summary>
		public string Volume
		{
			get => GetPropertyValue(Css.Volume);
			set => SetProperty(Css.Volume, value);
		}

		/// <summary>
		/// Gets or sets the 'white-space' css property which specifies how whitespace inside the element is handled.
		/// Value: normal | pre | nowrap | inherit.
		/// </summary>
		public string WhiteSpace
		{
			get => GetPropertyValue(Css.WhiteSpace);
			set => SetProperty(Css.WhiteSpace, value);
		}

		/// <summary>
		/// Specifies the minimum number of lines of a paragraph that must be left at the top of a page. Value:  &lt;integer&gt; | inherit
		/// </summary>
		public string Widows
		{
			get => GetPropertyValue(Css.Widows);
			set => SetProperty(Css.Widows, value);
		}

		/// <summary>
		/// Specifies the content width of boxes generated by block-level and replaced elements
		/// </summary>
		public string Width
		{
			get => GetPropertyValue(Css.Width);
			set => SetProperty(Css.Width, value);
		}

		/// <summary>
		/// Gets or sets the 'word-spacing' css property which specifies spacing behavior between words.
		/// Value: normal | &lt;length&gt;	| inherit 
		/// </summary>
		public string WordSpacing
		{
			get => GetPropertyValue(Css.WordSpacing);
			set => SetProperty(Css.WordSpacing, value);
		}

		/// <summary>
		/// Gets or sets the 'z-index' css property. For a positioned box, the 'z-index' property specifies: 
		/// 1. The stack level of the box in the current stacking context.
		/// 2. Whether the box establishes a local stacking context.
		/// </summary>
		public string ZIndex
		{
			get => GetPropertyValue(Css.ZIndex);
			set => SetProperty(Css.ZIndex, value);
		}
	}
}
