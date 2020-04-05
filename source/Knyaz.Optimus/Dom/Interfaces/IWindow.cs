using Knyaz.Optimus.Dom.Css;
using System;
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

		ICssStyleDeclaration GetComputedStyle(IElement element);
		ICssStyleDeclaration GetComputedStyle(IElement element, string pseudoElt);
		MediaQueryList MatchMedia(string query);
	}
	
	
	//todo: join with IWindow in next major version
	public interface IWindowEx : IWindow
	{
		Document Document {get;}
		IConsole Console { get; }
		Storage LocalStorage { get; } 
		Storage SessionStorage { get; }
		void Open(string url = null, string windowName = null, string features = null);
		void Alert(string msg);
	}
}
