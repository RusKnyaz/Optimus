var window = this;

(function (engine) {
	
	function upFirstLetter(string) { return string.charAt(0).toUpperCase() + string.slice(1); }

	function bindProps(target, owner, propsString) {
		var propsNames = propsString.split(' ');
		for (var i = 0; i < propsNames.length; i++) {
			var names = propsNames[i].split(':');
			var jsPropName = names[0];
			var netPropName = names[1] || upFirstLetter(jsPropName);

			var prop = {
				get: !owner["get_" + netPropName] ? undefined :
					function (x) { return function () { return owner[x]; }; }(netPropName),
				set: !owner["set_" + netPropName] ? undefined :
					function (x) { return function (v) { owner[x] = v; }; }(netPropName)
			};

			if (prop.get || prop.set)
				Object.defineProperty(target, jsPropName, prop);
		}
	}
	
	window.addEventListener = function (x, y, z) {
	};

	bindProps(window.location = {}, engine.Window.Location, "hash host hostname href origin pathname port protocol search");
	bindProps(window.navigator = {}, engine.Window.Navigator, "appCodeName appName appVersion cookieEnabled geolocation onLine platform product userAgent");
	window.navigator.javaEnabled = function () { return engine.Window.Navigator.JavaEnabled(); };
	bindProps(window.screen = {}, engine.Window.Screen, "width height availWidth availHeight colorDepth pixeDepth");
	bindProps(window, engine.Window, "innerWidth innerHeight");

	window.document = new (function () {
		var _this = this;
		var netDoc = engine.Document;
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
			/*var obj = {
				getCssText: function() {
					return "";//todo: implement
				},
				getLength: function() {
					return netStyle.Properties.Count;
				},
				getParentRule: function() {
					return null;//todo
				},
				getPropertyCSSValue: function(javapropertyName) {
					return netStyle.Properties[javapropertyName];//todo:
				},
				getPropertyPriority: function(javapropertyName) {
					return 0;//todo
				},
				getPropertyValue: function(propertyName) {
					return netStyle.GetPropertyValue(propertyName);
				},
				removeProperty: function(propertyName) {
					netStyle.Properties.Remove(propertyName);
				},
				setCssText : function(cssText){
					throw "not implemented";//todo
				},
				setProperty: function(propertyName, javavalue, javapriority) {
					netStyle.Properties[propertyName] = javavalue;
					//todo: priority
				}
			};*/

			var obj = [];

			for (var i = 0; i < netStyle.Properties.Count; i++) {
				obj[i] = netStyle[i];
				obj[netStyle[i]] = netStyle[netStyle[i]];
			}

			obj.getPropertyValue = function(propertyName) {
				return netStyle.GetPropertyValue(propertyName);
			};
			

			return obj;
		}
		
		function wrapNode(node, netElem) {
			//funcs
			node.hasAttributes = function () { return netElem.HasAttributes(); };
			node.removeChild = function (x) { return wrap(netElem.RemoveChild(x.netElem)); };
			node.insertBefore = function (newNode, refNode) { return wrap(netElem.InsertBefore(newNode.netElem, refNode.netElem)); };
			node.replaceChild = function (newNode, oldNode) { return wrap(netElem.ReplaceChild(newNode.netElem, oldNode.netElem)); };
			node.cloneNode = function () { return wrap(netElem.CloneNode()); };
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

		function documentElement(netElem) {
			this.netElem = netElem;

			this.ownerDocument = _this;
			this.appendChild = function (node) { return wrap(netElem.AppendChild(node.netElem)); };
			this.getElementsByTagName = function (tagName) { return wrapArray(netElem.GetElementsByTagName(tagName)); };
			this.getAttribute = function (attrName) { return netElem.GetAttribute(attrName); };
			this.setAttribute = function (attrName, value) { netElem.SetAttribute(attrName, value); };
			this.removeAttribute = function (attrName) { netElem.RemoveAttribute(attrName); };
			this.hasAttribute = function (attrName) { return netElem.HasAttribute(attrName); };
			//node
			wrapNode(this, netElem);
			if (this.nodeType === 1) {
				//Element
				bindProps(this, netElem, "innerHTML:InnerHtml tagName");

				//htmlElement
				if (netElem.Click) this.click = function () { netElem.Click(); };
				bindProps(this, netElem, "hidden");
				if (netElem.get_Style)
					Object.defineProperty(this, 'style', { get: function () { return wrapStyle(netElem.Style); } });

				//HtmlInputElement
				bindProps(this, netElem, "value disabled required readonly type checked");
			}
			//comment, script
			bindProps(this, netElem, "text data");
		}

		function wrap(netElem) {
			return !netElem ? null :
				wrappers[netElem.InternalId] || (wrappers[netElem.InternalId] = new documentElement(netElem));
		}

		function wrapArray(netElems) {
			var result = [];
			for (var i = 0; i < netElems.length; i++) {
				result[i] = wrap(netElems[i]);
			}
			return result;
		}

		wrapNode(this, netDoc);

		this.createElement = function (tagName) { return wrap(netDoc.CreateElement(tagName)); };
		this.createTextNode = function (value) { return wrap(netDoc.CreateTextNode(value)); };
		this.getElementById = function (id) { return wrap(netDoc.GetElementById(id)); };
		this.getElementsByTagName = function (tagName) { return wrapArray(netDoc.GetElementsByTagName(tagName)); };
		this.createComment = function () { return wrap(netDoc.CreateComment()); };
		this.write = function (x) { netDoc.Write(x); };
		this.createDocumentFragment = function () { return wrap(netDoc.CreateDocumentFragment()); };
		this.createEvent = function (type) { return wrapEvent(netDoc.CreateEvent(type)); };

		Object.defineProperty(this, 'documentElement', { get: function () { return wrap(netDoc.DocumentElement); } });
		Object.defineProperty(this, 'body', { get: function () { return wrap(netDoc.Body); } });
	})();

})(this["A89A3DC7FB5944849D4DE0781117A595"]);