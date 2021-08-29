using Knyaz.Optimus.Dom.Css;
using System;
using Knyaz.Optimus.Dom.Elements;
using Knyaz.Optimus.Dom.Events;
using Knyaz.Optimus.Environment;
using Knyaz.Optimus.ScriptExecuting;

namespace Knyaz.Optimus.Dom.Interfaces
{
	public interface IWindow : IEventTarget
	{
		int InnerWidth { get; set; }
		int InnerHeight { get; set; }

		IScreen Screen { get; }
		Location Location { get; }
		INavigator Navigator { get; }
		IHistory History { get; }
		
		int SetTimeout([JsExpandArray]Action<object[]> handler, double? delay, params object[] data);
		void ClearTimeout(int handle);
		int SetInterval([JsExpandArray]Action<object[]> handler, double? delay, params object[] data);
		void ClearInterval(int handle);

		ICssStyleDeclaration GetComputedStyle(Element element);
		ICssStyleDeclaration GetComputedStyle(Element element, string pseudoElt);
		MediaQueryList MatchMedia(string query);
		
		HtmlDocument Document {get;}
		IConsole Console { get; }
		Storage LocalStorage { get; } 
		Storage SessionStorage { get; }
		void Open(string url = null, string windowName = null, string features = null);
		void Alert(string msg);

		/// <summary> Event fires when the user submits a form by button. It doesn't fire when the user called form.submit method. </summary>
		event Func<Event, bool?> OnClick;
	}
	
	public interface IWindowEx : IWindow
	{
		XmlHttpRequest NewXmlHttpRequest();

		HtmlImageElement NewImage(int? width = 0, int? height = 0);

		Event NewEvent(string eventType = null, EventInitOptions options = null);
	}
}
