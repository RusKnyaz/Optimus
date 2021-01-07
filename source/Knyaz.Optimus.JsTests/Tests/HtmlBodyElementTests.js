Test("HtmlBodyElementTests", {
    "Prototype": {
        run: function () {
            Assert.IsNotNull(HTMLBodyElement.prototype, "HTMLBodyElement.prototype");
            Assert.IsNotNull(HTMLBodyElement.prototype.toString, "HTMLBodyElement.prototype.toString");
            Assert.IsNotNull(HTMLBodyElement.prototype.addEventListener, "HTMLBodyElement.prototype.addEventListener");
            Assert.IsNull(HTMLBodyElement.prototype.prototype, "HTMLBodyElement.prototype.prototype");
            Assert.AreEqual("[object HTMLBodyElement]", HTMLBodyElement.prototype.toString(), "HTMLBodyElement.prototype.toString()");
            Assert.IsNotNull(Object.getPrototypeOf(document.body));
            Assert.AreEqual("[object HTMLElement]", Object.getPrototypeOf(HTMLBodyElement.prototype).toString(), "Object.getPrototypeOf(HTMLBodyElement.prototype).toString()");
            Assert.AreEqual(Object.getPrototypeOf(document.body), HTMLBodyElement.prototype, "HTMLBodyElement.prototype == Object.getPrototypeOf(document.body)");
        }
    },
    "ChildNodes":{
        run: function () {
            var doc = document.implementation.createHTMLDocument();
            doc.write("<body><div id='d'></div><div></div><span></span></body>");
            Assert.AreEqual(3, doc.body.childNodes.length);
            Assert.IsNotNull(doc.body.childNodes[0]);
        }
    }
});