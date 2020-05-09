Test("StyleTests", {
    "StyleOfCustom": {
        run:function(){
            var doc = document.implementation.createHTMLDocument();
            doc.write("<span id='content1' style='width:100pt; heigth:100pt'></span>");
            var style= doc.createElement('bootstrap').style;
            Assert.IsNotNull(style);
        }
    },
    "StyleRead":{
        run:function(){
            var doc = document.implementation.createHTMLDocument()
            doc.write("<span id='content1' style='width:100pt; heigth:100pt'></span>");
            var style = doc.getElementById('content1').style
            Assert.AreEqual("100pt", style.getPropertyValue('width'));
            Assert.AreEqual("width", style[0]);
            Assert.AreEqual("100pt", style['width']);
        }
    },
    "StyleWrite":{
        run:function(){
            var doc = document.implementation.createHTMLDocument()
            doc.write("<span id='content1' style='width:100pt; heigth:100pt'></span>");
            var style = doc.getElementById('content1').style;
            style['width'] = '200pt';
            Assert.AreEqual("200pt", style['width']);
            console.log(style['width']);
        }
    }
});