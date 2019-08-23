Test("DocumentFragmentTests", {
    "Clone": {
        run: function () {
            var doc = document.createDocumentFragment();
            var e = document.createElement("div");
            doc.appendChild(e);
            var deepClone = doc.cloneNode(true);
            Assert.AreEqual("<div></div>", deepClone.firstChild.outerHTML.toLowerCase());
            
            var clone = doc.cloneNode();
            Assert.AreEqual(0, clone.childNodes.length);
        }
    }
});