Test("JavaScriptTests", {
    "CallThisIsObj":{
        run: function () {
            var res = {};
            var fun = function () { res.$this = this; };
            var some = {a:'hi'};
            fun.call(some);
            Assert.IsNotNull(res.$this);
            Assert.AreEqual("hi", res.$this.a);
        }
    },
    "CallThisIsNull": {
        run: function () {
            var res = {};
            var fun = function () { res.$this = this; };
            fun.call(null);
            Assert.AreEqual(window, res.$this);
        }
    },
    "CallThisIsUndefined": {
        run: function () {
            var res = {};
            var fun = function () { res.$this = this; };
            fun.call(undefined);
            Assert.AreEqual(window, res.$this);
            Assert.AreEqual(true, res.$this === window);
            Assert.AreEqual(true, window === res.$this);
        }
    },
    "CallAndCompareThisIsUndefined": {
        run: function () {
            var res = {};
            var fun = function () { res.x = document.body === this; };
            fun.call(undefined);
            Assert.AreEqual(false, res.x);
        }
    },
    "CallThisIsNullStrict": {
        run: function () {
            "use strict";
            var res = {};
            var fun = function () { res.$this = this; };
            fun.call(null);
            Assert.AreEqual(null, res.$this);
        }
    },
    "CallThisIsUndefinedStrict": {
        run: function () {
            "use strict";
            var res = {};
            var fun = function () { res.$this = this; };
            fun.call(undefined);
            Assert.AreEqual(undefined, res.$this);
        }
    },
    "CompareObjectWithWindow":{
        run:function () {
            var res = document.body === window;
            Assert.AreEqual(false, res);
        }
    },
    "CompareObjectWithWindowInverted":{
        run:function () {
            var res = window === document.body;
            Assert.AreEqual(false, res);
        }
    }
});