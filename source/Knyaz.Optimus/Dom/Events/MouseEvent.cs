using Knyaz.Optimus.Dom.Elements;
using Knyaz.Optimus.ScriptExecuting;

namespace Knyaz.Optimus.Dom.Events
{
	/// <summary> Represents events that occur when user interacting with a pointing device. </summary>
	public class MouseEvent : UiEvent
    {
		internal MouseEvent(HtmlDocument owner) : base(owner)
		{
		}
        
        internal MouseEvent(HtmlDocument owner, string type, MouseEventInitOptions options) :
            base(owner, type, options)
        {
            if (options != null)
            {
                AltKey = options.AltKey;
                Button = options.Button;
                Buttons = options.Buttons;
                ClientX = options.ClientX;
                ClientY = options.ClientY;
                CtrlKey = options.CtrlKey;
                MetaKey = options.MetaKey;
                ScreenX = options.ScreenX;
                ScreenY = options.ScreenY;
                Region = options.Region;
                RelatedTarget = options.RelatedTarget;
                ShiftKey = options.ShiftKey;
            }
        }

        /// <summary> Returns true if the alt key was down when the mouse event was fired. </summary>
        public bool AltKey { get; }
        /// <summary> The button number that was pressed (if applicable) when the mouse event was fired. </summary>
        public short Button { get; }
        /// <summary> The buttons being depressed (if any) when the mouse event was fired. </summary>
        public short Buttons { get; }
        /// <summary> The X coordinate of the mouse pointer in local (DOM content) coordinates. </summary>
        public long ClientX { get; }
        /// <summary> The Y coordinate of the mouse pointer in local (DOM content) coordinates. </summary>
        public long ClientY { get; }
        /// <summary> Returns true if the control key was down when the mouse event was fired. </summary>
        public bool CtrlKey { get; }
        /// <summary> Returns true if the meta key was down when the mouse event was fired. </summary>
        public bool MetaKey { get; }
        /// <summary> The X coordinate of the mouse pointer in global (screen) coordinates. </summary>
        public long ScreenX { get; }
        ///<summary>The Y coordinate of the mouse pointer relative to the position of the last mousemove event.</summary>    
        public long ScreenY { get; }
        /// <summary> Returns the id of the hit region affected by the event. If no hit region is affected, null is returned. </summary>
        public string Region { get;}
        /// <summary> The secondary target for the event, if there is one. </summary>
        public EventTarget RelatedTarget { get;}
        /// <summary> Returns true if the shift key was down when the mouse event was fired. </summary>
        public bool ShiftKey { get;}
        /*
MouseEvent.movementX Read only
    The X coordinate of the mouse pointer relative to the position of the last mousemove event.
MouseEvent.movementY Read only
MouseEvent.pageX Read only
    The X coordinate of the mouse pointer relative to the whole document.
MouseEvent.pageY Read only
    The Y coordinate of the mouse pointer relative to the whole document.
*/
    }

	public class MouseEventInitOptions : UiEventInitOptions
    {
        /// <summary> The horizontal position of the mouse event on the user's screen; setting this value doesn't move the mouse pointer </summary>
        public long ScreenX;
        /// <summary> The vertical position of the mouse event on the user's screen; setting this value doesn't move the mouse pointer </summary>
        public long ScreenY;
        /// <summary> the horizontal position of the mouse event on the client window of user's screen; setting this value doesn't move the mouse pointer. </summary>
        public long ClientX;
        /// <summary> The vertical position of the mouse event on the client window of the user's screen; setting this value doesn't move the mouse pointer. </summary>
        public long ClientY;
        /// <summary> Indicates if the ctrl key was simultaneously pressed. </summary>
        public bool CtrlKey;
        /// <summary> Indicates if the shift key was simultaneously pressed. </summary>
        public bool ShiftKey;
        /// <summary> Indicates if the alt key was simultaneously pressed. </summary>
        public bool AltKey;
        /// <summary> Indicates if the meta key was simultaneously pressed </summary>
        public bool MetaKey;
        /// <summary> Describes which button is pressed. </summary>
        public short Button;
        /// <summary> Describes which buttons are pressed when the event is launched. </summary>
        public short Buttons;

        public EventTarget RelatedTarget;

        /// <summary> The ID of the hit region affected by the event. The absence of any affected hit region is represented with the null value. </summary>
        [JsName("Region")]
        public string Region;
    }
}