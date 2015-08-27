using System.Diagnostics;
using WebBrowser.ScriptExecuting;

namespace WebBrowser.Dom.Elements
{
	public class KeyboardEvent : UIEvent
	{
		public KeyboardEvent()
		{
			Debug.Fail("Not implemented");
		}
    // KeyLocationCode
    /*const unsigned long DOM_KEY_LOCATION_STANDARD = 0x00;
    const unsigned long DOM_KEY_LOCATION_LEFT = 0x01;
    const unsigned long DOM_KEY_LOCATION_RIGHT = 0x02;
    const unsigned long DOM_KEY_LOCATION_NUMPAD = 0x03;
    readonly    attribute DOMString     key;
    readonly    attribute DOMString     code;
    readonly    attribute unsigned long location;
    readonly    attribute boolean       ctrlKey;
    readonly    attribute boolean       shiftKey;
    readonly    attribute boolean       altKey;
    readonly    attribute boolean       metaKey;
    readonly    attribute boolean       repeat;
    readonly    attribute boolean       isComposing;
    boolean getModifierState (DOMString keyArg);*/
		//todo: complete
	}

	[DomItem]
	public interface IKeyboardEvent
	{
		
	}
}
