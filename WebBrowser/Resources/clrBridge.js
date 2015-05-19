var window = this;

(function (engine) {
	var converters = [];
	converters["Document"] = wrapDocument;
	converters["Element"] = wrap;
	converters["Element[]"] = wrapArray;
	converters["Node"] = wrap;
	converters["DocumentFragment"] = wrap;
	converters["Text"] = wrap;
	converters["Comment"] = wrap;
	converters["Event"] = wrapEvent;
	converters["CssStyleDeclaration"] = wrapStyle;
	
	function bindFunc(target, owner, funcName) {
		var netFuncName = upFirstLetter(funcName);
		var methodInfo = owner.GetType().GetMethod(netFuncName);
		if (methodInfo) {
			function getFunc(fn, o, acnt, conv) {
				return function () {
					var a = [];
					for (var x = 0; x < acnt; x++) {
						a[x] = x < arguments.length ? arguments[x] : null;
					}
					return conv(methodInfo.Invoke(owner, a));
				};
			}

			target[funcName] = getFunc(netFuncName, owner, methodInfo.GetParameters().length, converters[methodInfo.ReturnType.Name] || function(x) { return x; });
		}
	}

	function bindFuncs(target, owner, funcs) {
		var funcNames = funcs.split(' ');
		for (var i = 0; i < funcNames.length; i++) {
			bindFunc(target, owner, funcNames[i]);
		}
	}

	var wrappers = {};

	function wrapEvent(netEvent) {
		return {
			netEvent: netEvent,
			type: netEvent.Type,
			Target: wrap(netEvent.Target),
			initEvent: function (type, b1, b2) { netEvent.InitEvent(type, b1, b2); }
			//todo: remains properties
		};
	}

	function wrapStyle(netStyle) {
		var obj = [];

		for (var i = 0; i < netStyle.Properties.Count; i++) {
			obj[i] = netStyle[i];
			obj[netStyle[i]] = netStyle[netStyle[i]];
		}

		bindFuncs(obj, netStyle, "getPropertyValue getCssText getLength getParentRule getPropertyCSSValue getPropertyPriority removeProperty setCssText setProperty");

		return obj;
	}

	function wrapNode(node, netElem) {
		//funcs
		node.removeChild = function (x) { return wrap(netElem.RemoveChild(x.netElem)); };
		node.insertBefore = function (newNode, refNode) { return wrap(netElem.InsertBefore(newNode.netElem, refNode.netElem)); };
		node.replaceChild = function (newNode, oldNode) { return wrap(netElem.ReplaceChild(newNode.netElem, oldNode.netElem)); };
		node.cloneNode = function () { return wrap(netElem.CloneNode()); };

		bindFuncs(node, netElem, "hasAttributes");
		//props
		bindProps(node, netElem, "hasChildNodes nodeType nodeName nodeValue");
		Object.defineProperty(node, 'nextSibling', { get: function () { return wrap(netElem.NextSibling); } });
		Object.defineProperty(node, 'previousSibling', { get: function () { return wrap(netElem.PreviousSibling); } });
		Object.defineProperty(node, 'childNodes', { get: function () { return wrapArray(netElem.ChildNodes.ToArray()); } });
		Object.defineProperty(node, 'firstChild', { get: function () { return wrap(netElem.FirstChild); } });
		Object.defineProperty(node, 'lastChild', { get: function () { return wrap(netElem.LastChild); } });
		Object.defineProperty(node, 'parentNode', { get: function () { return wrap(netElem.Parent); } });

		//events
		var registeredEventsHandlers = [];

		node.addEventListener = function (type, handler, useCapture) {
			if (!registeredEventsHandlers[type])
				registeredEventsHandlers[type] = [];

			registeredEventsHandlers[type].push(handler);
			//todo: handle userCapture parameter
		};

		node.removeEventListener = function (type, handler, useCapture) {
			var typeHandlers = registeredEventsHandlers[type];
			if (!typeHandlers)
				return;

			var handlerIndex = typeHandlers.indexOf(handler);
			if (handlerIndex < 0)
				return;

			typeHandlers.splice(handlerIndex, 1);
		};

		node.dispatchEvent = function (evt) { netElem.DispatchEvent(evt.netEvent); };

		netElem.add_OnEvent(function (netEvent) {
			var event = wrapEvent(netEvent);
			var typeHandlers = registeredEventsHandlers[event.type];
			if (!typeHandlers || typeHandlers.length == 0)
				return;
			for (var i = 0; i < typeHandlers.length; i++) {
				//todo: stop propagation
				typeHandlers[i](event);
			}
		});
	}

	function wrap(netElem) {
		return !netElem ? null :
			wrappers[netElem.InternalId] || (wrappers[netElem.InternalId] = new documentElement(netElem));
	}

	function documentElement(netElem) {
		this.netElem = netElem;

		this.appendChild = function (node) { return wrap(netElem.AppendChild(node.netElem)); };
		bindFuncs(this, netElem, "getAttribute setAttribute removeAttribute hasAttribute getElementsByTagName");
		//node
		wrapNode(this, netElem);
		if (this.nodeType === 1) {
			bindProps(this, netElem, "innerHTML:InnerHtml tagName hidden style value disabled required readonly type checked");
			bindFuncs(this, netElem, "click");
		}
		//comment, script
		bindProps(this, netElem, "ownerDocument text data");
	}
	
	function wrapArray(netElems) {
		var result = [];
		for (var i = 0; i < netElems.length; i++) {
			result[i] = wrap(netElems[i]);
		}
		return result;
	}
	
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
	
	var docsWrappers = [];
	function wrapDocument(netDoc) {
		if (docsWrappers[netDoc.GetHashCode()])
			return docsWrappers[netDoc.GetHashCode()];

		var doc = {};
		bindFuncs(doc, netDoc, "createElement createTextNode getElementById createComment write createDocumentFragment createEvent getElementsByTagName");
		bindProps(doc, netDoc, "body documentElement");
		docsWrappers[netDoc.GetHashCode()] = doc;
		return doc;
	}

	window.addEventListener = function (x, y, z) {
		//todo: implement
	};

	bindProps(window.location = {}, engine.Window.Location, "hash host hostname href origin pathname port protocol search");
	bindProps(window.navigator = {}, engine.Window.Navigator, "appCodeName appName appVersion cookieEnabled geolocation onLine platform product userAgent");
	window.navigator.javaEnabled = function () { return engine.Window.Navigator.JavaEnabled(); };
	bindProps(window.screen = {}, engine.Window.Screen, "width height availWidth availHeight colorDepth pixeDepth");
	bindProps(window, engine.Window, "innerWidth innerHeight");
	window.setTimeout = function (handler, timeout) {
		var args = Array.prototype.slice.call(arguments, 2);
		return engine.Window.SetTimeout(function () {
			handler.apply({}, args);
		}, timeout);
	};

	bindFuncs(window, engine.Window, "clearTimeout");
	bindProps(window, engine, "document");

	//ajax:http://www.w3.org/TR/XMLHttpRequest/
	window.XMLHttpRequest = function () {
		var _this = this;
		var netRequest = engine.XmlHttpRequest();
		bindFuncs(this, netRequest, "open send setRequestHeader abort getResponseHeader getAllResponseHeaders overrideMimeType");
		bindProps(this, netRequest, "status responseXML readyState");
		var onreadystatechange;
		var onreadystatechangeWrapper;
		Object.defineProperty(this, 'onreadystatechange', {
			get: function() { return onreadystatechange;},
			set: function(handler) {
				if (onreadystatechange) {
					netRequest.remove_OnReadyStateChange(onreadystatechangeWrapper);
				}
				onreadystatechange = handler;
				onreadystatechangeWrapper = function() {onreadystatechange.call(_this);};
				netRequest.add_OnReadyStateChange(onreadystatechangeWrapper);
			}
		});

		this.UNSENT = 0;
		this.OPENED = 1;
		this.HEADERS_RECEIVED = 2;
		this.LOADING = 3;
		this.DONE = 4;
	};

})(this["A89A3DC7FB5944849D4DE0781117A595"]);