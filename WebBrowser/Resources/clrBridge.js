var document = new (function () {
	var _this = this;
	var engine = A89A3DC7FB5944849D4DE0781117A595;
	var netDoc = engine.Document;
	var wrappers = {};

	function wrapNode(node, netElem) {
		node.hasAttributes = function () { return netElem.HasAttributes(); };
		node.removeChild = function (x) { return wrap(netElem.RemoveChild(x.netElem)); };
		node.insertBefore = function (newNode, refNode) { return wrap(netElem.InsertBefore(newNode.netElem, refNode.netElem)); };
		node.replaceChild = function (newNode, oldNode) { return wrap(netElem.ReplaceChild(newNode.netElem, oldNode.netElem)); };
		node.cloneNode = function () { return wrap(netElem.CloneNode()); };
		Object.defineProperty(node, 'nodeType', { get: function () { return netElem.NodeType; } });
		Object.defineProperty(node, 'nextSibling', { get: function () { return wrap(netElem.NextSibling); } });
		Object.defineProperty(node, 'previousSibling', { get: function () { return wrap(netElem.PreviousSibling); } });
		Object.defineProperty(node, 'childNodes', { get: function () { return wrapArray(netElem.ChildNodes.ToArray()); } });
		Object.defineProperty(node, 'firstChild', { get: function () { return wrap(netElem.FirstChild); } });
		Object.defineProperty(node, 'lastChild', { get: function () { return wrap(netElem.LastChild); } });
		Object.defineProperty(node, 'parentNode', { get: function () { return wrap(netElem.Parent); } });
		Object.defineProperty(node, 'nodeName', { get: function () { return netElem.NodeName; } });
		Object.defineProperty(node, 'nodeValue', { get: function () { return netElem.NodeValue; }, set: function (v) { netElem.NodeValue = v; } });
		
		//events
		var registeredEventsHandlers = [];

		node.addEventListener = function (type, handler, useCapture) {
			if (!registeredEventsHandlers[type])
				registeredEventsHandlers[type] = [];

			registeredEventsHandlers[type].push(handler);
			//todo: handle userCapture parameter
		};

		node.removeEventListener = function(type, handler, useCapture) {
			var typeHandlers = registeredEventsHandlers[type];
			if (!typeHandlers)
				return;

			var handlerIndex = typeHandlers.indexOf(handler);
			if (handlerIndex < 0)
				return;

			typeHandlers.splice(handlerIndex, 1);
		};

		node.dispatchEvent = function(evt) {netElem.DispatchEvent(evt.netEvent);};

		netElem.add_OnEvent(function(netEvent) {
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
		Object.defineProperty(this, 'innerHTML', { get: function () { return netElem.InnerHtml; }, set: function (v) { netElem.InnerHtml = v; } });

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
			Object.defineProperty(this, 'tagName', { get: function () { return netElem.TagName; } });
			//htmlElement
			if (netElem.Click) this.click = function () { netElem.Click(); };
			if (netElem.get_Value) Object.defineProperty(this, 'value', { get: function () { return netElem.Value; }, set: function (v) { netElem.Value = v; } });
		}

		//comment
		Object.defineProperty(this, 'text', { get: function () { return netElem.Text; } });
		Object.defineProperty(this, 'hasChildNodes', { get: function () { return netElem.HasChildNodes; } });
		Object.defineProperty(this, 'data', { get: function () { return netElem.Data; }, set: function (v) { netElem.Data = v; } });
	}

	function wrap(netElem) {
		if (!netElem)
			return netElem;
		var wrapElem = wrappers[netElem.InternalId];
		if (wrapElem)
			return wrapElem;

		wrapElem = new documentElement(netElem);
		wrappers[netElem.InternalId] = wrapElem;
		return wrapElem;
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

	function wrapEvent(netEvent) {
		return {
			netEvent: netEvent,
			type: netEvent.Type,
			Target: wrap(netEvent.Target),
			initEvent: function (type, b1, b2) { netEvent.InitEvent(type, b1, b2); }
			//todo: remains properties
		};
	}

	Object.defineProperty(this, 'documentElement', { get: function () { return wrap(netDoc.DocumentElement); } });
	Object.defineProperty(this, 'body', { get: function () { return wrap(netDoc.Body); } });
})();

var window = this;
//todo: implement
window.addEventListener = function (x, y, z) {
};
window.location = { href: '' };