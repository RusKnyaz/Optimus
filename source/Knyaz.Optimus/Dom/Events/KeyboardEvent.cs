using Knyaz.Optimus.ScriptExecuting;

namespace Knyaz.Optimus.Dom.Events
{
	public class KeyboardEvent : UIEvent
	{
		private string _modifiers;

		public KeyboardEvent()
		{
		}

		// KeyLocationCode
		/*const unsigned long DOM_KEY_LOCATION_STANDARD = 0x00;
    const unsigned long DOM_KEY_LOCATION_LEFT = 0x01;
    const unsigned long DOM_KEY_LOCATION_RIGHT = 0x02;
    const unsigned long DOM_KEY_LOCATION_NUMPAD = 0x03;*/
		//todo: complete

		public void InitKeyboardEvent(string type, bool canBubble, bool cancellable, object view, string c, string key,
			ulong location,
			string modifiers, bool repeat)
		{
			InitEvent(type, canBubble, cancellable);
			Code = c;
			Key = key;
			Location = location;
			Repeat = repeat;

			_modifiers = modifiers ?? string.Empty;
			if (!string.IsNullOrEmpty(modifiers))
			{
				CtrlKey = modifiers.Contains("Ctrl");
				AltKey = modifiers.Contains("Alt");
				ShiftKey = modifiers.Contains("Shift");
			}
		}

		public string Key { get; private set; }
		public string Code { get; private set; }
		public ulong Location { get; private set; }
		public bool CtrlKey { get; private set; }
		public bool ShiftKey { get; private set; }
		public bool AltKey { get; private set; }
		public bool MetaKey { get; private set; }
		public bool Repeat { get; private set; }
		public bool IsComposing { get; private set; }

		public bool GetModifierState(string key)
		{
			return _modifiers.Contains(key);
		}
	}

	[DomItem]
	public interface IKeyboardEvent
	{
		string Key { get; }
		string Code { get; }
		ulong Location { get; }
		bool CtrlKey { get; }
		bool ShiftKey { get; }
		bool AltKey { get; }
		bool MetaKey { get; }
		bool Repeat { get;}
		bool IsComposing { get; }
	}
}
