Test("HtmlAnchorElementTests", {
    "DefaultValues": {
        run: function () {
            var a = document.createElement("a");
            Assert.AreEqual("", a.download);
            Assert.AreEqual("", a.href);
            Assert.AreEqual("", a.hreflang);
            Assert.AreEqual("", a.ping);
            Assert.AreEqual("", a.rel);
            Assert.AreEqual("", a.text);
            Assert.AreEqual("", a.target);
        }
    },
    "GetText": {
        run: function () {
            var a = document.createElement("a");
            a.innerHTML = "<div>A</div><span>B</span>";
            Assert.AreEqual("AB", a.text);
        }
    },
    "SetText":{
        run:function () {
            var a = document.createElement("a");
            a.innerHTML = "<div>A</div><span>B</span>";
            a.text = "AB";
            Assert.AreEqual("AB", a.innerHTML);
        }
    },
    "RelListContains":{
        run:function () {
            var a = document.createElement("a");
            a.rel = "a b c";
            Assert.AreEqual(true, a.relList.contains("a"), "RelList should contains 'a'");
            Assert.AreEqual(true, a.relList.contains("b"),"RelList should contains 'b'");
            Assert.AreEqual(true, a.relList.contains("c"),"RelList should contains 'c'");
            Assert.AreEqual(false, a.relList.contains("d"),"RelList should not contains 'd'");
        }
    },
    "ChangeRelGetRelList":{
        run:function () {
            var a = document.createElement("a");
            var lst = a.relList;
            a.rel = "a b c";
            Assert.AreEqual(true, lst.contains("a"), "RelList should contains 'a'");
            Assert.AreEqual(true, lst.contains("b"),"RelList should contains 'b'");
            Assert.AreEqual(true, lst.contains("c"),"RelList should contains 'c'");
            Assert.AreEqual(false, lst.contains("d"),"RelList should not contains 'd'");
        }
    },
    "AddToRelList":{
        run:function () {
            var a = document.createElement("a");
            var lst = a.relList;
            a.rel = "a b";
            lst.add("c");
            Assert.AreEqual("a b c", a.rel);
        }
    },
    "RelListToString":{
        run:function () {
            var a = document.createElement("a");
            a.rel = "a b";
            Assert.AreEqual("a b", a.relList.toString());
        }
    },
    "ClickOnClick":{
        run:function(){
            var data = null;
            window._test = function(msg){ data = msg;};
            var d = document.createElement('div');
            d.innerHTML="<a onclick='JavaScript:_test(1)'></a>";
            var a = d.firstChild;
            a.click();
            Assert.AreEqual(1, data);
        }
    }
});