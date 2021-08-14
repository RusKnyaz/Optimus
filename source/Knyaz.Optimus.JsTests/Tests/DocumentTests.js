function loadDoc(html) {
    var doc = document.implementation.createHTMLDocument();
    doc.write(html);
    return doc;
}

Test("DocumentTests", {
    "DocumentElement": {
        run: function () {
            var e = document.documentElement;
            Assert.IsTrue(e == e, "e == e")
            Assert.IsTrue(e.ownerDocument === document, "e.ownerDocument === document")
            Assert.IsTrue(e.ownerDocument === document, "e.ownerDocument === document");
            Assert.IsTrue(e.parentNode === document, "e.parentNode === document");
            Assert.IsNotNull(e.removeChild, "e.removeChild != null");
            Assert.IsNotNull(e.appendChild, "e.appendChild != null");
        }
    },
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
    "GetElementsByTagName":{
        run:function() {
            var doc = document.implementation.createHTMLDocument();
            doc.write("<div id='d'></div><div></div><span></span>");
            var elements = doc.body.getElementsByTagName('div');
            Assert.AreEqual(2, elements.length);
        }
    },
    "GetElementsByTagNameByIndex":{
        run:function() {
            var doc = document.implementation.createHTMLDocument();
            doc.write("<div id='content1'></div><div id='content2'></div>");
            Assert.IsNotNull(doc.body.getElementsByTagName('div')[0]);
            Assert.AreEqual(doc.body.firstChild, doc.body.getElementsByTagName('div')[0]);
        }
    },
    "GetElementsByTagNameReturnsHTMLCollection":{
        run:function(){
            var doc = document.implementation.createHTMLDocument();
            var elts = doc.getElementsByTagName("custom");
            Assert.AreEqual("[object HTMLCollection]", elts.toString());
        }
    },
    "GetElementsByTagNameIsLive":{
        run:function() {
            var doc = document.implementation.createHTMLDocument();
            doc.write("<div id='d'></div><div></div><span></span>");
            var elements = doc.body.getElementsByTagName('div');
            Assert.AreEqual(2, elements.length);
            doc.body.lastChild.remove();
            Assert.AreEqual(2, elements.length);
            doc.body.lastChild.remove();
            Assert.AreEqual(1, elements.length);
        }
    },
    "GetElementsByTagNameNamedItem":{
        run : function () {
            var doc = loadDoc("<P id=p1></P><p id=p2></p>") 
            var p2 = doc.getElementsByTagName("P").namedItem("p2");
            Assert.AreEqual("p2", p2.id);
        }
    },
    "GetElementsByTagNameNamedItemIndexer":{
        run : function () {
            var doc = loadDoc("<P id=p1></P><p id=p2></p>")
            var p1 = doc.getElementsByTagName("P")["p1"];
            Assert.AreEqual("p1", p1.id);
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
    "GetElementsByClassNameReturnsHTMLCollection":{
        run:function () {
            var doc = document.implementation.createHTMLDocument();
            doc.write("<body><div class='c1'></div><div class='c2'></div><div class='c2'></div></body>");
            var elts = doc.getElementsByClassName('c2');
            Assert.AreEqual("[object HTMLCollection]", elts.toString());
        }
    },
    "GetElementsByClassNameIsLive":{
        run:function () {
            var doc = document.implementation.createHTMLDocument();
            doc.write("<body><div class='c1'></div><div class='c2'></div><div class='c2'></div></body>");
            var elts = doc.getElementsByClassName('c2');
            Assert.AreEqual(2, elts.length);
            elts[0].innerHTML="<p class=c2></p>";
            Assert.AreEqual(3, elts.length);
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
    "GetElementsByNameReturnsLiveCollection":{
        run:function(){
            var doc = document.implementation.createHTMLDocument();
            var customs = doc.getElementsByName("custom");
            Assert.AreEqual(0, customs.length);
            doc.body.innerHTML="<div name=custom></div>";
            Assert.AreEqual(1, customs.length);
        }
    },
    "GetElementsByNameReturnsNodeList":{
        run:function(){
            var doc = document.implementation.createHTMLDocument();
            var elts = doc.getElementsByName("custom");
            Assert.AreEqual("[object NodeList]", elts.toString());
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
            Assert.AreEqual("[object HTMLDocument]", Object.getPrototypeOf(document).toString(), "Object.getPrototypeOf(document).toString()");
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
    },
    "QuerySelectorAllReturnsNodeList":{
        run:function(){
            var doc = document.implementation.createHTMLDocument();
            Assert.AreEqual("[object NodeList]", doc.querySelectorAll("*").toString());
        }
    },
    "QuerySelectorAllReturnsStaticCollection":{
        run:function(){
            var doc = document.implementation.createHTMLDocument();
            doc.write("<div class=x></div>")
            var xClass = doc.querySelectorAll(".x");
            Assert.AreEqual(1, xClass.length);
            doc.write("<div class=x></div>");
            //ensure that node is added.
            var newXClass = doc.querySelectorAll(".x");
            Assert.AreEqual(2, newXClass.length);
            //original query result should not be changed.            
            Assert.AreEqual(1, xClass.length);
        }
    },
    "ScriptsExists":{
        run:function(){
            Assert.IsNotNull(document.scripts, "document.scripts != null");
        }
    },
    "ScriptsLength":{
        run:function(){
            var js = document.scripts;
            var l1 = js.length;
            var s = document.createElement("script");
            document.head.appendChild(s);
            var l2 = js.length;
            Assert.AreEqual(1, l2-l1);
        }
    },
    "FormsLength":{
        run:function(){
            var forms = document.forms;
            var l1 = forms.length;
            var s = document.createElement("form");
            document.head.appendChild(s);
            var l2 = forms.length;
            Assert.AreEqual(1, l2-l1);
        }
    }
});