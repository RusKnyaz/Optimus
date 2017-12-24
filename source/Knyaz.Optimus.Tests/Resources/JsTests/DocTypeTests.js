Test("DocTypeTests", {
    "Clone": {
        run: function () {
            var docType = document.implementation.createDocumentType(
                'svg:svg', '-//W3C//DTD SVG 1.1//EN', 'http://www.w3.org/Graphics/SVG/1.1/DTD/svg11.dtd');

            docType.innerHTML="ABC";
            
            var clone = docType.cloneNode(true);
            
            Assert.AreEqual(undefined, clone.innerHTML);
            Assert.AreEqual('svg:svg', clone.name);
            Assert.AreEqual('-//W3C//DTD SVG 1.1//EN', clone.publicId);
            Assert.AreEqual('http://www.w3.org/Graphics/SVG/1.1/DTD/svg11.dtd', clone.systemId);
        }
    }
});