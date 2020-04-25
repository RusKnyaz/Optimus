Test("XmlHttpRequestTests", {
    "XMLHttpRequestType": {
        run: function () {
            Assert.AreEqual("function", typeof XMLHttpRequest);
        }
    },
    "XMLHttpRequestPrototype":{
        run: function(){
            Assert.AreEqual(4, XMLHttpRequest.prototype.DONE, "XMLHttpRequest.prototype.DONE");
            Assert.IsNotNull(Object.getPrototypeOf(new XMLHttpRequest()))
            Assert.AreEqual(XMLHttpRequest.prototype, Object.getPrototypeOf(new XMLHttpRequest()))
            Assert.AreEqual(true, (new XMLHttpRequest()).prototype === undefined);
        }
    },
    "NewXmlHttpRequest":{
        run:function(){
            Assert.AreEqual(true, XMLHttpRequest != null, "XMLHttpRequest != null");
            Assert.IsNotNull(new XMLHttpRequest(), "new XMLHttpRequest()");            
        }
    },
    "XmlHttpRequestStaticFields": {
        run:function() {
            Assert.AreEqual(0, XMLHttpRequest.UNSENT);
            Assert.AreEqual(1, XMLHttpRequest.OPENED);
            Assert.AreEqual(1, (new XMLHttpRequest()).OPENED);        
        }
    }
});
