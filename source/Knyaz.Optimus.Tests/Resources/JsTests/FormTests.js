Test("FormTests", {
    "ChildElements": {
        run: function () {
            var f = document.createElement("form");
            var i = document.createElement("input");
            f.appendChild(i);
            Assert.AreEqual(i,  f.elements[0]);
        }
    },
    "DefaultEnctype":{
        run:function () {
            var f = document.createElement("form");
            Assert.AreEqual("application/x-www-form-urlencoded", f.enctype);
        }
    }
});