Test("FormTests", {
    "ChildElements": {
        run: function () {
            var f = document.createElement("form");
            var i = document.createElement("input");
            f.appendChild(i);
            Assert.AreEqual(i,  f.elements[0]);
        }
    }
});