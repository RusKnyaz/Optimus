var window = this;

(function (engine) {
	window.addEventListener = function(a, b, c) { engine.Window.AddEventListener(a, b, c); };
	window.removeEventListener = function (a, b, c) { engine.Window.RemoveEventListener(a, b, c); };
	window.dispatchEvent = function (x) { return engine.Window.DispatchEvent(x); };
   window.clearTimeout = function(x) { engine.Window.ClearTimeout(x); };
	
	window.setTimeout = function (handler, timeout) {
		if (!handler)
			return;
		var args = Array.prototype.slice.call(arguments, 2);
		return engine.Window.SetTimeout(function () {
			handler.apply({}, args);
		}, timeout);
	};

	window.setInterval = function(handler, timeout) {
		if (!handler)
			return;
		var args = Array.prototype.slice.call(arguments, 2);
		return engine.Window.SetInterval(function() { handler.apply({}, args); }, timeout);
	};
    window.clearInterval = function(x) { engine.Window.ClearInterval(x||-1); };
	
	//ajax:http://www.w3.org/TR/XMLHttpRequest/
	window.XMLHttpRequest = function () {
		var r = engine.XmlHttpRequest();
		r.UNSENT = 0;
		r.OPENED = 1;
		r.HEADERS_RECEIVED = 2;
		r.LOADING = 3;
		r.DONE = 4;
		return r;
	};

	
})(this["A89A3DC7FB5944849D4DE0781117A595"]);