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
    }
});