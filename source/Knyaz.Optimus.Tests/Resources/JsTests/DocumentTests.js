Test("DocumentTests", {
    "ImplementationCreateHtmlDocument": {
        run: function () {
            var doc = document.implementation.createHTMLDocument("NewDoc");
            Assert.AreEqual("NewDoc", doc.title);
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
    "ImplementationCreateDocumentWithDocType": {
        run: function () {
            var dt = document.implementation.createDocumentType("svg:svg", "-//W3C//DTD SVG 1.1//EN", "http://www.w3.org/Graphics/SVG/1.1/DTD/svg11.dtd");
            var doc = document.implementation.createDocument("http://www.w3.org/2000/svg", "svg:svg", dt);
            Assert.AreEqual("http://www.w3.org/2000/svg", doc.documentElement.namespaceURI);
            Assert.AreEqual(dt, doc.doctype);
        }
    }
});