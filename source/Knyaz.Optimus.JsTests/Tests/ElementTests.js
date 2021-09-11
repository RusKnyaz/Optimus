Test("ElementTests", {
    "SetParent": {
        run: function () {
            var div = document.createElement("div");
            var div2 = document.createElement("div");
            Assert.AreEqual(null, div.parentNode);
            div.parentNode = div2;
            Assert.AreEqual(null, div.parentNode);
            div2.appendChild(div);
            Assert.AreEqual(div2, div.parentNode);
        }
    },
    "SetOwnerDocument": {
        run: function () {
            var div = document.createElement("div");
            Assert.AreEqual(document, div.ownerDocument);
            div.ownerDocument = null;
            Assert.AreEqual(document, div.ownerDocument);
        }
    },
    "Remove": {
        run: function () {
            var div = document.createElement("div");
            var childDiv = document.createElement("div");
            div.appendChild(childDiv);
            childDiv.remove();
            Assert.AreEqual(0, div.childNodes.length);
        }
    },
    "AttributesLength": {
        run: function () {
            var div = document.createElement("div");
            div.setAttribute("A", 1);
            div.setAttribute("B", 2);
            Assert.AreEqual(2, div.attributes.length);
        }
    },
    "AttributesGetByName": {
        run: function () {
            var div = document.createElement("div");
            div.setAttribute("A", 1);
            Assert.AreEqual(1, div.attributes["A"].value);
            Assert.AreEqual("a", div.attributes["A"].name);
        }
    },
    "SetOnClickAttribute": {
        run: function () {
            var d = document.createElement('div');
            d.setAttribute('onclick', 'event.currentTarget.innerHTML="HI"');
            d.click();
            Assert.AreEqual("HI", d.innerHTML);
        }
    },
    "SetOnClickAttributePropogation": {
        run: function () {
            var d = document.createElement('div');
            var subDiv = document.createElement('div');
            d.setAttribute('onclick', 'event.currentTarget.innerHTML="HI"');
            d.appendChild(subDiv);
            subDiv.click();
            Assert.AreEqual("HI", d.innerHTML);
        }
    },
    "EventHandlingOrder": {
        run: function () {
            var sequence = [];
            var d1 = document.createElement("div");
            d1.id = "A";
            var d2 = document.createElement("div");
            d2.id = "B";
            d1.appendChild(d2);
            d1.onclick = function (e) { sequence.push('d1 attr - ' + e.eventPhase + e.currentTarget.id); };
            d1.addEventListener("click", function (e) { sequence.push('d1 bubbling - ' + e.eventPhase + e.currentTarget.id) }, false);
            d1.addEventListener("click", function (e) { sequence.push('d1 capture - ' + e.eventPhase + e.currentTarget.id) }, true);
            d2.onclick = function (e) { sequence.push('d2 attr - ' + e.eventPhase + e.currentTarget.id); };
            d2.addEventListener("click", function (e) { sequence.push('d2 bubbling - ' + e.eventPhase + e.currentTarget.id) }, false);
            d2.addEventListener("click", function (e) { sequence.push('d2 capture - ' + e.eventPhase + e.currentTarget.id) }, true);
            d2.click();
            Assert.AreEqual("d1 capture - 1A,d2 attr - 2B,d2 bubbling - 2B,d2 capture - 2B,d1 attr - 3A,d1 bubbling - 3A", sequence.toString());
        }
    },
    "EventHandlingOrderCapturingString": {
        run: function () {
            var sequence = [];
            var d1 = document.createElement("div");
            d1.id = "A";
            var d2 = document.createElement("div");
            d2.id = "B";
            d1.appendChild(d2);
            d1.onclick = function (e) { sequence.push('d1 attr - ' + e.eventPhase + e.currentTarget.id); };
            d1.addEventListener("click", function (e) { sequence.push('d1 bubbling - ' + e.eventPhase + e.currentTarget.id) }, false);
            d1.addEventListener("click", function (e) { sequence.push('d1 capture - ' + e.eventPhase + e.currentTarget.id) }, "false");
            d2.onclick = function (e) { sequence.push('d2 attr - ' + e.eventPhase + e.currentTarget.id); };
            d2.addEventListener("click", function (e) { sequence.push('d2 bubbling - ' + e.eventPhase + e.currentTarget.id) }, false);
            d2.addEventListener("click", function (e) { sequence.push('d2 capture - ' + e.eventPhase + e.currentTarget.id) }, true);
            d2.click();
            Assert.AreEqual("d1 capture - 1A,d2 attr - 2B,d2 bubbling - 2B,d2 capture - 2B,d1 attr - 3A,d1 bubbling - 3A", sequence.toString());
        }
    },
    "EventHandlingOrderCapturingUndefined": {
        run: function () {
            var sequence = [];
            var d1 = document.createElement("div");
            d1.id = "A";
            var d2 = document.createElement("div");
            d2.id = "B";
            d1.appendChild(d2);
            d1.onclick = function (e) { sequence.push('d1 attr - ' + e.eventPhase + e.currentTarget.id); };
            d1.addEventListener("click", function (e) { sequence.push('d1 bubbling - ' + e.eventPhase + e.currentTarget.id) }, undefined);
            d1.addEventListener("click", function (e) { sequence.push('d1 capture - ' + e.eventPhase + e.currentTarget.id) }, true);
            d2.onclick = function (e) { sequence.push('d2 attr - ' + e.eventPhase + e.currentTarget.id); };
            d2.addEventListener("click", function (e) { sequence.push('d2 bubbling - ' + e.eventPhase + e.currentTarget.id) }, false);
            d2.addEventListener("click", function (e) { sequence.push('d2 capture - ' + e.eventPhase + e.currentTarget.id) }, true);
            d2.click();
            Assert.AreEqual("d1 capture - 1A,d2 attr - 2B,d2 bubbling - 2B,d2 capture - 2B,d1 attr - 3A,d1 bubbling - 3A", sequence.toString());
        }
    },
    "EventListenerParams": {
        run: function () {
            var res = {};
            var elt = document.createElement("div");
            elt.addEventListener("click",
                function (e) {
                    res.e = { target: e.target, currentTarget: e.currentTarget };
                    res.$this = this;
                });
            elt.click();
            Assert.AreEqual(elt, res.$this, "this");
            Assert.AreEqual(elt, res.e.target, "event.target");
            Assert.AreEqual(elt, res.e.currentTarget, "event.currentTarget");
        }
    },
    "EventHandlerParams": {
        run: function () {
            var res = {};
            var elt = document.createElement("div");
            elt.onclick = function (e) {
                res.e = { target: e.target, currentTarget: e.currentTarget };
                res.$this = this;
            };
            elt.click();
            Assert.AreEqual(elt, res.$this, "this");
            Assert.AreEqual(elt, res.e.target, "event.target");
            Assert.AreEqual(elt, res.e.currentTarget, "event.currentTarget");
        }
    },
    "AttrEventHandlerParams": {
        run: function () {
            window.res = {};
            var elt = document.createElement("div");
            elt.setAttribute("onclick",
                "window.res.e = { target: event.target, currentTarget: event.currentTarget };"
                + "window.res.$this = this;");
            elt.click();
            var res = window.res;
            Assert.AreEqual(elt, res.$this, "this");
            Assert.AreEqual(elt, res.e.target, "event.target");
            Assert.AreEqual(elt, res.e.currentTarget, "event.currentTarget");
        }
    },
    "AddRemoveEventListener" :{
        run:function () {
            var called = false;
            var handler = function (e) {  called = true;  };
            var elt = document.createElement("div");
            elt.addEventListener("click", handler);
            elt.removeEventListener("click", handler);
            elt.click();
            Assert.AreEqual(false, called);
        }
    },
    "AddTwoEventListeners":{
        run:function () {
            var resultThis = [];
            var root = document.createElement("div");
            var child = document.createElement("span");
            root.appendChild(child);
            var handler = function (e) { resultThis.push(this); };
            root.addEventListener("click", handler);
            child.addEventListener("click", handler);
            child.click();
            Assert.AreEqual(2, resultThis.length);
            Assert.AreEqual(child,  resultThis[0], "child's this");
            Assert.AreEqual(root,  resultThis[1], "root's this");
        }
    },
    "RemoveEventListenerInsideHandler":{
        run:function () {
            var counter = 0;
            var handler = function (e) {  counter++; elt.removeEventListener("click", handler); };
            var elt = document.createElement("div");
            elt.addEventListener("click", handler);
            elt.click();
            elt.click();
            Assert.AreEqual(1, counter);
        }
    },
    "RemoveOtherEventListenerInsideHandler":{
        run:function () {
            var counter = 0;
            var handler2= function (e) {  counter++; };
            var handler = function (e) {  counter++; elt.removeEventListener("click", handler2); };
            var elt = document.createElement("div");
            elt.addEventListener("click", handler);
            elt.addEventListener("click", handler2);
            elt.click();
            Assert.AreEqual(1, counter);
        }
    },
    "SetTextContent":{
        run:function(){
            var div = document.createElement("div");
            div.textContent = "<p>Text</p><p>Text 2</p>";
            Assert.AreEqual("<p>Text</p><p>Text 2</p>", div.textContent);
            Assert.AreEqual(1, div.childNodes.length);
            Assert.AreEqual("#text", div.firstChild.nodeName);
        }
    },
    "SetTextContentRemovesChildren":{
        run:function () {
            var div = document.createElement("div");
            var span = document.createElement("span");
            div.appendChild(span);
            div.textContent = "Hello";
            Assert.AreEqual("Hello", div.innerHTML);
            Assert.AreEqual(1, div.childNodes.length);
            Assert.AreEqual("#text", div.firstChild.nodeName);
        }
    },
    "SetTextContentEmpty":{
        run:function () {
            var div = document.createElement("div");
            div.textContent = "";
            Assert.AreEqual(0, div.childNodes.length);
            Assert.AreEqual("", div.textContent);
        }
    },
    "GetTextContent":{
        run: function () {
            var div = document.createElement("div");
            div.innerHTML = "<!-- Some comment --><p>Paragraph 1<span>text</span></p><p>Paragraph 2</p><script>var x = 5;</script>";
            Assert.AreEqual("Paragraph 1textParagraph 2var x = 5;", div.textContent);
        }
    },
    "TableDoesNotAcceptDivs":{
        run:function () {
            var d = document.createElement("div");
            d.innerHTML="<table><div></div><span></span><tbody></tbody><tr></tr></table>";
            Assert.AreEqual("<div></div><span></span><table><tbody></tbody><tbody><tr></tr></tbody></table>",
                d.innerHTML.toLowerCase())
        }
    },
    "TableWrongTBody":{
        run:function () {
            var d= document.createElement("div");
            d.innerHTML = "<table><div></div><span></span><tbody></tbody><tr></tr></table>";
            Assert.AreEqual("<div></div><span></span><table><tbody></tbody><tbody><tr></tr></tbody></table>",
                d.innerHTML.toLowerCase());
        }
    },
    "TableCreatedBodies":{
        run:function () {
            var d = document.createElement("div");
            d.innerHTML="<table><tbody></tbody><tr id=x></tr><tbody></tbody><tr></tr></table>";
            Assert.AreEqual("<table><tbody></tbody><tbody><tr id=\"x\"></tr></tbody><tbody></tbody><tbody><tr></tr></tbody></table>",
                d.innerHTML.toLowerCase())
        }
    },
    "SetInnerHtml":{
        run:function () {
            var div = document.createElement("div");
            div.innerHTML = "<h1>1</h1><h2>2</h2><h3>3</h3>";
            Assert.AreEqual(3, div.childNodes.length);
        }
    },
    "SetInnerHtmlText":{
        run:function () {
            var div = document.createElement("div");
            div.innerHTML = "abc";
            Assert.AreEqual("abc", div.innerHTML);
        }
    },
    "AppendAttributeThrows":{
        run:function () {
            var at = document.createAttribute("dd");
            var div = document.createElement('div');
            Assert.Throws(function ()  {
                div.appendChild(at);
            })
        }
    },
    "TextElementDispatchesEvent":{
        run:function () {
            var result = "";
            var div = document.createElement("div");
            var txt = document.createTextNode("T");
            div.appendChild(txt);
            var evt = document.createEvent("Event");
            evt.initEvent("click", true, true);
            div.onclick = function () {
                result = evt.target.data;
            }
            txt.dispatchEvent(evt);
            Assert.AreEqual("T", result);
        }
    },
    "HiddenExist":{ 
        run:function(){
            var elt = document.createElement("div");
            Assert.IsNotNull(elt.hidden);
        }
    },
    "DataSetExists":{
        run:function () {
            var elt = document.createElement("div");
            Assert.IsNotNull(elt.dataset);
        }
    },
    "DataSetFromAttribute":{
        run: function(){
            var elt = document.createElement("div");
            elt.setAttribute("data-my-data", "hello");
            Assert.AreEqual("hello", elt.dataset.myData);
        }
    },
    "DataSetToAttribute":{
        run:function(){
            var elt = document.createElement("div");
            elt.dataset.myData = "hello";
            Assert.AreEqual("hello", elt.getAttribute("data-my-data"));
            Assert.AreEqual(1, elt.attributes.length);
        }
    },
    "DataSetToExistingAttribute":{
        run:function() {
            var elt = document.createElement("div");
            elt.setAttribute("data-my-data", "hello");
            elt.dataset.myData = "hi";
            Assert.AreEqual("hi", elt.getAttribute("data-my-data"));
            Assert.AreEqual(1, elt.attributes.length);
        }
    },
    "DataSetFromAbsentAttribute": {
        run: function(){
            var elt = document.createElement("div");
            Assert.IsNull(elt.dataset.myData);
        }
    },
    "DataSetByIndexer":{
        run: function() {
            var elt = document.createElement("div");
            elt.setAttribute("data-my-data", "hello");
            Assert.AreEqual("hello", elt.dataset["myData"]);
        }
    },
    "DataSetCapitalizeFirstLetter":{
        run:function(){
            var elt = document.createElement("div");
            elt.setAttribute("data--my-data", "hello");
            Assert.AreEqual("hello", elt.dataset.MyData);
        }
    },
    "DataSetWrongAttributeName":{
        run:function(){
            var elt = document.createElement("div");
            elt.setAttribute("data-my--data", "hello");
            Assert.IsNull(elt.dataset.myData);
        }
    },
    "Prototype":{
        run: function(){
            Assert.IsNotNull(HTMLElement.prototype, "HTMLElement.prototype");
            Assert.IsNotNull(Element.prototype, "Element.prototype");
            Assert.AreEqual(true, HTMLElement.prototype.hasOwnProperty('dataset'), 
                "HTMLElement.prototype.hasOwnProperty('dataset')");
        }
    },
    "Node":{
        run: function() {
            var doc = document.implementation.createHTMLDocument();
            doc.write("<div id='d'><h1>1</h1><h2>2</h2><h3>3</h3></div>");
            var e = doc.getElementById('d');
            Assert.IsNotNull(e, "e!=null");
            Assert.AreEqual(e, doc.getElementById('d'));
            Assert.AreEqual("DIV", e.tagName);
            Assert.IsNotNull(e.firstChild, "e.firstChild");
            Assert.AreEqual("H1", e.firstChild.tagName);
            Assert.IsNotNull(e.lastChild, "e.lastChild");
            Assert.AreEqual("H3", e.lastChild.tagName);
            Assert.IsNotNull(e.parentNode, "e.parentNode");
            Assert.AreEqual("BODY", e.parentNode.tagName);
            Assert.AreEqual(doc, e.ownerDocument);
            Assert.AreEqual("d", e.getAttribute('id'));
        }
    },
    "SetChildNode":{
        run: function(){
            var doc = document.implementation.createHTMLDocument();
            doc.write("<div id='a'><span></span></div>");
            var d = doc.getElementById('a');
            d.childNodes[0] = doc.createElement('p');
            Assert.AreEqual("SPAN", d.childNodes[0].tagName, "child must not be changed");    
        }
    },
    "ChildNodesIsNodeList":{
        run: function(){
            var doc = document.implementation.createHTMLDocument();
            Assert.InstanceOf(doc.body.childNodes, NodeList)
        }    
    },
    "ChildNodesIsLive":{
        run:function(){
            var doc = document.implementation.createHTMLDocument();
            doc.write("<body><div id='d'></div><div></div><span></span></body>");
            var nodes = doc.body.childNodes;
            Assert.AreEqual(3, nodes.length, "original count");
            doc.body.innerHTML = "";//clear body
            Assert.AreEqual(0, nodes.length, "updated count");
        }
    },
    "GetAttributeNoValue":{
        run:function(){
            var doc = document.implementation.createHTMLDocument();
            doc.write("<body><div id='d' lay-submit></div></body>");
            var d = doc.getElementById('d');
            var attr = d.getAttributeNode('lay-submit');
            Assert.IsNotNull(attr);
            Assert.AreEqual('', attr.value);
            Assert.AreEqual('', d.getAttribute('lay-submit'));
        }
    }
});