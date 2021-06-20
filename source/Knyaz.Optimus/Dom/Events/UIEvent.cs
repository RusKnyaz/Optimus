using System;
using Knyaz.Optimus.Environment;
using Knyaz.Optimus.ScriptExecuting;

namespace Knyaz.Optimus.Dom.Events
{
	/// <summary>
	/// The base class for UI events.
	/// </summary>
	[JsName("UIEvent")]
	public class UiEvent : Event
	{
		internal UiEvent(HtmlDocument owner) : base(owner){}

		internal UiEvent(HtmlDocument owner, string type, UiEventInitOptions options) :
			base(owner, type, options)
		{
			View = options?.View;
			Detail = options?.Detail ?? 0;
		}

		[Obsolete("The method is deprecated.")]
		[JsName("InitUIEvent")]
		public void InitUiEvent(string type, bool canBubble, bool cancelable,Window view, long detail)
		{
			base.InitEvent(type, canBubble, cancelable);
			View = view;
			Detail = detail;
		}
		
		public Window View { get; private set; }
		
		public long Detail { get; private set; }
	}
	
	public class UiEventInitOptions : EventInitOptions
	{
		public long Detail;

		public Window View;
	}
}
