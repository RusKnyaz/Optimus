Test("DocumentTests", {
    "ImplementationCreateHtmlDocument": {
        run: function () {
            var doc = document.implementation.createHTMLDocument("NewDoc");
            Assert.AreEqual("NewDoc", doc.title);
            Assert.IsNotNull(doc.doctype);
            Assert.IsNotNull(doc.body);
            Assert.IsNotNull(doc.documentElement);
            Assert.AreEqual("html", doc.doctype.name);
            Assert.AreEqual("", doc.doctype.publicId);
            Assert.AreEqual("", doc.doctype.systemId);
        }
    },
    "ImplementationCreateDocumentWithoutDocType":{
        run: function() {
            var doc = document.implementation.createDocument("http://www.w3.org/1999/xhtml", "html");
            Assert.IsNull(doc.doctype);
            Assert.AreEqual("html", doc.documentElement.tagName.toLowerCase());
        }
    },
    "ImplementationCreateDocumentType": {
        run: function() {
            var dt = document.implementation.createDocumentType("svg:svg", "-//W3C//DTD SVG 1.1//EN", "http://www.w3.org/Graphics/SVG/1.1/DTD/svg11.dtd");
            Assert.AreEqual("svg:svg", dt.name);
            Assert.AreEqual("-//W3C//DTD SVG 1.1//EN", dt.publicId);
            Assert.AreEqual("http://www.w3.org/Graphics/SVG/1.1/DTD/svg11.dtd", dt.systemId);
            
        }
    },
    "ImplementationCreateDocumentWithDocTypeSvg": {
        run: function () {
            var dt = document.implementation.createDocumentType("svg:svg", "-//W3C//DTD SVG 1.1//EN", "http://www.w3.org/Graphics/SVG/1.1/DTD/svg11.dtd");
            var doc = document.implementation.createDocument("http://www.w3.org/2000/svg", "svg:svg", dt);
            Assert.AreEqual("http://www.w3.org/2000/svg", doc.documentElement.namespaceURI);
            Assert.AreEqual(dt, doc.doctype);
        }
    },
    "RemoveDocType": {
        run: function () {
            var doc = document.implementation.createHTMLDocument("NewDoc");
            doc.doctype.remove();
            Assert.IsNull(doc.doctype);
        }
    },
    "TextContentIsNull":{
        run:function () {
            Assert.IsNull(document.textContent);
        }
    },
    "DefaultViewIsWindow":{
        run:function () {
            Assert.AreEqual(window, document.defaultView, "window == document.defaultView");
            Assert.AreEqual(document.defaultView, window, "document.defaultView == window");
        }
    },
    "DomBuildOrder":{
        run:function () {
            var res = "";
            
            var div = document.createElement("div");
            div.addEventListener("DOMNodeInserted", function(x){
                res = x.target.innerHTML;
            });

            div.innerHTML = "<div><span></span></div>";
            Assert.AreEqual("<span></span>", res.toLowerCase());
        }
    },
    "SetBody":{
        run:function () {
            var doc = document.implementation.createHTMLDocument();
            var body = doc.createElement("body");
            body.innerHTML = "abc";
            doc.body = body;
            Assert.AreEqual("<head></head><body>abc</body>",
                doc.documentElement.innerHTML.toLowerCase());
        }
    },
    "SetBodyDiv":{
        run: function () {
            var doc = document.implementation.createHTMLDocument();
            var div = doc.createElement("div");
            Assert.Throws(function () { doc.body = div; });
        }
    },
    "SetBodyNull":{
        run:function () {
            var doc = document.implementation.createHTMLDocument();
            Assert.Throws(function () { doc.body = null; });   
        }
    },
    "GetElementsByClassName":{
        run:function () {
            var doc = document.implementation.createHTMLDocument();
            doc.write("<body><div class='c1'></div><div class='c2'></div><div class='c2'></div></body>");
            var elts = doc.getElementsByClassName('c2');
            Assert.AreEqual(2, elts.length);
            Assert.IsNotNull(elts[1]);
            Assert.AreEqual(elts[1], elts["1"]);
        }
    },
    "GetElementsByClassNameAndSlice":{
        run:function () {
            var doc = document.implementation.createHTMLDocument();
            doc.write("<body><div class='c1'></div><div class='c2'></div><div class='c2'></div></body>");
            var elts = doc.getElementsByClassName('c2');
            var eltsCopy = Array.prototype.slice.call(elts);
            Assert.AreEqual(2, eltsCopy.length);
            Assert.IsNotNull(eltsCopy[1]);
            Assert.AreEqual(eltsCopy[1], eltsCopy["1"]);
        }
    },
    "InstanceOf":{
        run:function () {
            Assert.AreEqual(false, document.body instanceof String, "document.body instanceof String");
            Assert.AreEqual(true, document.body instanceof Element, "document.body instanceof Element");
            Assert.AreEqual(true, document.body instanceof HTMLElement, "document.body instanceof HTMLElement");
            Assert.AreEqual(true, document.body instanceof HTMLBodyElement, "document.body instanceof HTMLBodyElement");
        }
    },
    "Prototype":{
        run:function() {
            Assert.IsNotNull(Object.getPrototypeOf(document), "Object.getPrototypeOf(document)");
            Assert.IsNotNull(Object.getPrototypeOf(document).write, "Object.getPrototypeOf(document).write");
            Assert.AreEqual(document.write, Object.getPrototypeOf(document).write, "Object.getPrototypeOf(document).write === document.write");
            Assert.AreEqual("[object HTMLDocumentPrototype]", Object.getPrototypeOf(document).toString(), "Object.getPrototypeOf(document).toString()");
            Assert.AreEqual(true, 'body' in Object.getPrototypeOf(document), "'body' in Object.getPrototypeOf(document)");
        }
    },
    "Properties":{
        run:function(){
            Assert.AreEqual(undefined, document.prototype, "document.prototype === undefined");
            Assert.IsNotNull(document, "document");
            Assert.IsNotNull(document.write, "document.write");
            Assert.AreEqual(document, document, "document == document");
            Assert.AreEqual(true, 'ownerDocument' in document, "'ownerDocument' in document");
            Assert.AreEqual(false, document.hasOwnProperty('ownerDocument'), "document.hasOwnProperty('ownerDocument')");
            Assert.IsNull(document.ownerDocument, "document.ownerDocument");
            Assert.IsNull(document.parentNode, "document.parentNode");
            Assert.IsNotNull(document.documentElement, "document.documentElement");
            Assert.IsNotNull(document.appendChild, "document.appendChild");
            Assert.AreEqual(document.appendChild, document.appendChild, "document.appendChild == document.appendChild");
            Assert.AreEqual(document.body.appendChild, document.body.appendChild, "document.body.appendChild == document.body.appendChild");
            Assert.AreEqual("[object HTMLDocument]", document.toString(), "document.toString()");
            
        }
    }
});