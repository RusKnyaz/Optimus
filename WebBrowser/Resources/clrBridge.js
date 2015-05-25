var window = this;

(function (engine) {
	var converters = [];
	
	Object.defineProperty(window, 'document', { get: function () { return engine.Document; } });
	
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
	
	function wrapAttr(netAttr) {
	    var attr = {};
	    wrapNode(attr, netAttr);
	    bindProps(attr, "name specified value ownerElement isId schemaTypeInfo");
	    return attr;
	}

	function wrapEvent(netEvent) {
		return {
			netEvent: netEvent,
			type: netEvent.Type,
			Target: wrap(netEvent.Target),
			initEvent: function (type, b1, b2) { netEvent.InitEvent(type, b1, b2); }
			//todo: remains properties
		};
	}

	function wrapNode(node, netElem) {
	    //funcs
	    node.appendChild = function (x) { return wrap(netElem.AppendChild(x.netElem)); };
		node.removeChild = function (x) { return wrap(netElem.RemoveChild(x.netElem)); };
		node.insertBefore = function (newNode, refNode) { return wrap(netElem.InsertBefore(newNode.netElem, refNode.netElem)); };
		node.replaceChild = function (newNode, oldNode) { return wrap(netElem.ReplaceChild(newNode.netElem, oldNode.netElem)); };
		node.compareDocumentPosition = function (node) { return netElem.CompareDocumentPosition(node.netElem); }

		bindFuncs(node, netElem, "hasAttributes cloneNode");
		//props
		bindProps(node, netElem, "hasChildNodes nodeType nodeName nodeValue nextSibling firstChild lastChild parentNode:Parent previousSibling ownerDocument");
		Object.defineProperty(node, 'childNodes', { get: function () { return wrapArray(netElem.ChildNodes.ToArray()); } });

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

		node.isSameNode = function(n) { return n.netElem == node.netElem; };
	}

	function wrapElement(netElem) {
		var elem = {};
		elem.netElem = netElem;
		
		bindFuncs(elem, netElem, "getAttribute setAttribute removeAttribute hasAttribute getElementsByTagName getAttributeNode");

		elem.setAttributeNode = function (node) { return wrapAttr(netElem.SetAttributeNode(node.netAttr)); };
		elem.removeAttributeNode = function(node) { netElem.RemoveAttributeNode(node.netAttr); };
		//node
		wrapNode(elem, netElem);
		if (elem.nodeType === 1) {
			bindProps(elem, netElem, "innerHTML:InnerHtml tagName hidden style value disabled required readonly type checked");
			bindFuncs(elem, netElem, "click");
		}
		//comment, script
		bindProps(elem, netElem, "text data");
		return elem;
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
	
	window.addEventListener = function (x, y, z) {
		//todo: implement
	};

	bindProps(window.location = {}, engine.Window.Location, "hash host hostname href origin pathname port protocol search");
	bindProps(window.navigator = {}, engine.Window.Navigator, "appCodeName appName appVersion cookieEnabled geolocation onLine platform product userAgent");
	window.navigator.javaEnabled = function () { return engine.Window.Navigator.JavaEnabled(); };
	bindProps(window.screen = {}, engine.Window.Screen, "width height availWidth availHeight colorDepth pixeDepth");
	bindProps(window, engine.Window, "innerWidth innerHeight");
	window.setTimeout = function (handler, timeout) {
		if (!handler)
			return;
		var args = Array.prototype.slice.call(arguments, 2);
		return engine.Window.SetTimeout(function () {
			handler.apply({}, args);
		}, timeout);
	};

	bindFuncs(window, engine.Window, "clearTimeout");
	
    function bindEvent(target, owner, name) {
        var names = name.split(':');
        var jsName = names[0];
        var netName = names[1] || upFirstLetter(jsName);

        var jsHandler;
        var jsHandlerWrapper;
        Object.defineProperty(target, jsName, {
            get: function () { return jsHandler; },
            set: function (handler) {
                if (jsHandler) {
                    owner["remove_"+netName](jsHandlerWrapper);
                }
                jsHandler = handler;
                jsHandlerWrapper = function () { jsHandler.call(target); };
                owner["add_"+netName](jsHandlerWrapper);
            }
        });
    }

	//ajax:http://www.w3.org/TR/XMLHttpRequest/
	window.XMLHttpRequest = function () {
		var netRequest = engine.XmlHttpRequest();
		bindFuncs(this, netRequest, "open send setRequestHeader abort getResponseHeader getAllResponseHeaders overrideMimeType");
		bindProps(this, netRequest, "status responseXML responseText readyState");
		bindEvent(this, netRequest, "onreadystatechange:OnReadyStateChange");
		bindEvent(this, netRequest, "onload:OnLoad");
		bindEvent(this, netRequest, "onerror:OnError");

		this.UNSENT = 0;
		this.OPENED = 1;
		this.HEADERS_RECEIVED = 2;
		this.LOADING = 3;
		this.DONE = 4;
	};

})(this["A89A3DC7FB5944849D4DE0781117A595"]);