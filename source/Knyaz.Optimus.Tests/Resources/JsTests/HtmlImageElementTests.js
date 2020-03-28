Test("HtmlImageElementTests", {
    "NoArgumentsCtor": {
        run: function () {
            var img = new Image();
            Assert.AreEqual(0, img.width, "width");
            Assert.AreEqual(0, img.height, "height");
            Assert.AreEqual(true, img.complete, "complete");
        }
    },
    "OneArgumentCtor":{
        run:function () {
            var img = new Image(100);
            Assert.AreEqual(100, img.width, "width");
            Assert.AreEqual(0, img.height, "height");
            Assert.AreEqual(true, img.complete, "complete");
        }
    },
    "TwoArgumentsCtor": {
        run: function () {
            var img = new Image(100,200);
            Assert.AreEqual(100, img.width, "width");
            Assert.AreEqual(200, img.height, "height");
            Assert.AreEqual(100, img.getAttribute("width"), "attribute 'width'");
            Assert.AreEqual(200, img.getAttribute("height"), "attribute 'height'");
            Assert.AreEqual(true, img.complete, "complete");
        }
    },
    "SetWidthAndHeight":{
        run: function () {
            var img = new Image(100,200);
            img.width = 300;
            img.height = 400;
            Assert.AreEqual(300, img.width, "width");
            Assert.AreEqual(400, img.height, "height");
        }
    },
    "TypeOfNewImage":{
        run: function(){
            var img = new Image(10,10);
            Assert.AreEqual("[object HTMLImageElement]", img.toString());
        }
    },
    "PrototypeOfNewImage":{
        run: function(){
            var img = new Image(10,10);
            Assert.AreEqual("[object HTMLImageElementPrototype]", Object.getPrototypeOf(img).toString());
        }
    },
    "NewImageInstanceOfHTMLImageElement":{
        run: function(){
            Assert.AreEqual(true, new Image(1,1) instanceof HTMLImageElement);
        }
    }
});