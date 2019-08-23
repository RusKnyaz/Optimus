using Knyaz.Optimus.Environment;
using Knyaz.Optimus.ScriptExecuting;

namespace Knyaz.Optimus.Dom.Events
{
	/// <summary>
	/// Describes a user interaction with the keyboard.
	/// </summary>
	[DomItem]
	public class KeyboardEvent : UIEvent
	{
		private string _modifiers;

		internal KeyboardEvent(Document owner):base(owner){}

		// KeyLocationCode
		/*const unsigned long DOM_KEY_LOCATION_STANDARD = 0x00;
    const unsigned long DOM_KEY_LOCATION_LEFT = 0x01;
    const unsigned long DOM_KEY_LOCATION_RIGHT = 0x02;
    const unsigned long DOM_KEY_LOCATION_NUMPAD = 0x03;*/
		//todo: complete

		/// <summary>
		/// Initializes the keyboard event.
		/// </summary>
		/// <param name="type">The type of keyboard event (one of keydown, keypress, or keyup).</param>
		/// <param name="canBubble">Whether or not the event can bubble.</param>
		/// <param name="cancellable">Whether or not the event can be canceled.</param>
		/// <param name="view">The Window it is associated to.</param>
		/// <param name="charArg">The value of the char attribute.</param>
		/// <param name="key">The value of the key attribute.</param>
		/// <param name="location">The value of the location attribute.</param>
		/// <param name="modifiers">A whitespace-delineated list of modifier keys (like 'Control' , 'Shift', etc) that should be considered to be active on the event's key</param>
		/// <param name="repeat">The value of the repeat attribute.</param>
		public void InitKeyboardEvent(string type, bool canBubble, bool cancellable, Window view, string charArg, string key,
			ulong location,
			string modifiers, bool repeat)
		{
			InitEvent(type, canBubble, cancellable);
			View = view;
			Code = charArg;
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

		/// <summary>
		/// Gets the key value of the key represented by the event.
		/// </summary>
		public string Key { get; private set; }
		
		/// <summary>
		/// Gets the code value of the key represented by the event.
		/// </summary>
		public string Code { get; private set; }
		
		/// <summary>
		/// Returns a number representing the location of the key on the keyboard or other input device.
		/// </summary>
		public ulong Location { get; private set; }
		
		/// <summary>
		/// Returns <c>true</c> if the Ctrl key was active when the key event was generated.
		/// </summary>
		public bool CtrlKey { get; private set; }
		
		/// <summary>
		/// Returns <c>true</c> if the Shift key was active when the key event was generated.
		/// </summary>
		public bool ShiftKey { get; private set; }
		
		/// <summary>
		/// Returns <c>true</c> if the Alt key was active when the key event was generated.
		/// </summary>
		public bool AltKey { get; private set; }
		
		/// <summary>
		/// Returns <c>true</c> if the Meta(windows/command) key was active when the key event was generated.
		/// </summary>
		public bool MetaKey { get; private set; }
		
		/// <summary>
		/// Returns <c>true</c> if the key is being held down such that it is automatically repeating.
		/// </summary>
		public bool Repeat { get; private set; }
		
		/// <summary>
		/// Returns <c>true</c> if the event is fired between after compositionstart and before compositionend.
		/// </summary>
		public bool IsComposing { get; private set; }

		/// <summary>
		/// Returns a bool indicating if the modifier key, like Alt, Shift, Ctrl, or Meta, was pressed.
		/// </summary>
		/// <param name="key">A modifier key.</param>
		public bool GetModifierState(string key) => _modifiers.Contains(key);
	}
}

