using System;
using System.Collections.Generic;
using Knyaz.Optimus.Dom;
using Knyaz.Optimus.Dom.Elements;
using Knyaz.Optimus.Dom.Perf;

namespace Knyaz.Optimus.ScriptExecuting
{
	/// <summary> Settings to configure script executor. </summary>
	public class ScriptingSettings
	{
		public static ScriptingSettings Default = new ScriptingSettings(
			new Type[]
			{
				typeof(Node),
				typeof(Element),
				typeof(HtmlAnchorElement),
				typeof(HtmlBodyElement),
				typeof(HtmlButtonElement),
				typeof(HtmlBrElement),
				typeof(HtmlDivElement),
				typeof(HtmlElement),
				typeof(HtmlFormElement),
				typeof(HtmlHtmlElement),
				typeof(HtmlIFrameElement),
				typeof(HtmlImageElement),
				typeof(HtmlInputElement),
				typeof(HtmlLabelElement),
				typeof(HtmlLinkElement),
				typeof(HtmlOptGroupElement),
				typeof(HtmlOptionElement),
				typeof(HtmlSelectElement),
				typeof(HtmlStyleElement),
				typeof(HtmlTableCaptionElement),
				typeof(HtmlTableColElement),
				typeof(HtmlTableElement),
				typeof(HtmlTableSectionElement),
				typeof(HtmlTableRowElement),
				typeof(HtmlTextAreaElement),
				typeof(HtmlUnknownElement),
				typeof(Script),
				typeof(Comment),
				typeof(Document),
				typeof(Text),
				typeof(Attr),
				//Perf types
				typeof(ArrayBuffer),
				typeof(Int8Array),
				typeof(UInt8Array),
				typeof(Int16Array),
				typeof(UInt16Array),
				typeof(Int32Array),
				typeof(UInt32Array),
				typeof(Float32Array),
				typeof(Float64Array),
				typeof(DataView),
			});


		/// <summary> Types that have to be available by scripts. </summary>
		public readonly IReadOnlyCollection<Type> GlobalTypes;

		private ScriptingSettings(Type[] globalTypes) => GlobalTypes = Array.AsReadOnly(globalTypes);
	}
}