var loadDoc = function(html){
    var doc = document.implementation.createHTMLDocument();
    doc.write(html);
    return doc;
};

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
    },
    "Misc":{
        run:function(){
            Assert.AreEqual(false, 'alert' in {});
            Assert.AreEqual(false, ({}===this), "({}===this)");
            Assert.AreEqual(false, (function(){var data; return data !== undefined;})(), "(function(){var data; return data !== undefined;})()");
        }
    },
    "Splice":{
        run:function(){
            var result = (function (){var x = [1,2,3]; x.splice(1,0,4);return x;})();
            Assert.AreEqual(
                JSON.stringify([1, 4, 2, 3]), 
                JSON.stringify(result));        
        }
    },
    "PushApply":{
        description:"The sample come from jquery source code",
        run:function(){
            var doc = loadDoc("<div></div>").body;
            var arr = [];
            var push = arr.push;
            var slice = arr.slice;
            push.apply((arr = slice.call(doc.childNodes)), doc.childNodes );
            Assert.AreEqual(1, arr[ doc.childNodes.length ].nodeType);
        }
    }, 
    "ArrayPush":{
        run:function () {
            var arr = [];
            arr.push('x');
            Assert.AreEqual(1, arr.length);
        }    
    },
    "SliceCall":{
        run:function(){
            var arr = ['a'];
            Assert.AreEqual(1, arr.slice().length);
            Assert.AreEqual(1, [].slice.call(arr).length);
        }
    },
    "ChildNodesSlice":{
        run:function () {
            var doc = loadDoc("<div></div>")
            Assert.AreEqual(1, doc.body.childNodes.length);
            Assert.AreEqual(1, [].slice.call(doc.body.childNodes).length);
        }
    },
    "ResizeArray":{
        run:function () {
            var arr = [];
            arr.length = 8;
            Assert.AreEqual(8, arr.length);
        }
    },
    "ShiftArray":{
        run:function () {
            var arr = [1,2];
            arr.shift();
            Assert.AreEqual(2, arr[0]);
        }
    }
});