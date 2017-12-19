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
    "Remove":{
        run:function () {
            var div = document.createElement("div");
            var childDiv = document.createElement("div");
            div.appendChild(childDiv);
            childDiv.remove();
            Assert.AreEqual(0, div.childNodes.length);
        }
    },
    "AttributesLength":{
        run:function () {
            var div = document.createElement("div");
            div.setAttribute("A", 1);
            div.setAttribute("B", 2);
            Assert.AreEqual(2, div.attributes.length);
        }
    },
    "AttributesGetByName":{
        run:function () {
            var div = document.createElement("div");
            div.setAttribute("A", 1);
            Assert.AreEqual(1, div.attributes["A"].value);
            Assert.AreEqual("a", div.attributes["A"].name);
        }
    }
});