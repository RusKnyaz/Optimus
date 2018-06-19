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
            Assert.AreEqual("<svg:svg xmlns:svg=\"http://www.w3.org/2000/svg\"/>", doc.documentElement.outerHTML);
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
    }
});