var window = this;

(function (engine) {
	var converters = [];
	
	Object.defineProperty(window, 'document', { get: function () { return engine.Document; } });
	
	function upFirstLetter(string) { return string.charAt(0).toUpperCase() + string.slice(1); }

	function bindProps(target, owner, propsString) {
		var propsNames = propsString.split(' ');
		for (var i = 0; i < propsNames.length; i++) {
			var names = propsNames[i].split(':');
			var jsPropName = names[0];
			var netPropName = names[1] || upFirstLetter(jsPropName);

			var prop = {
				set: !owner["set_" + netPropName] ? undefined :
					function (x) { return function (v) { owner[x] = v; }; }(netPropName)
			};

			if (owner["get_" + netPropName]) {
				var getType = owner.GetType().GetMethod("get_" + netPropName).ReturnType;
				var converter = converters[getType.Name];

				prop.get = converter
					? function (x, c) { return function () { return c(owner[x]); }; }(netPropName, converter)
					: function (x) { return function () { return owner[x]; }; }(netPropName);
			}

			if (prop.get || prop.set)
				Object.defineProperty(target, jsPropName, prop);
		}
	}

	window.addEventListener = function(a, b, c) { engine.Window.AddEventListener(a, b, c); };
	window.removeEventListener = function (a, b, c) { engine.Window.RemoveEventListener(a, b, c); };
	window.dispatchEvent = function (x) { return engine.Window.DispatchEvent(x); };
	window.clearTimeout = engine.Window.clearTimeout;
	window.location = engine.Window.Location;
	window.navigator = engine.Window.Navigator;
	window.screen = engine.Window.Screen;
    
	bindProps(window, engine.Window, "innerWidth innerHeight");
	window.setTimeout = function (handler, timeout) {
		if (!handler)
			return;
		var args = Array.prototype.slice.call(arguments, 2);
		return engine.Window.SetTimeout(function () {
			handler.apply({}, args);
		}, timeout);
	};
	
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