Test("HtmlLabelElementTests", {
    "Control":{
        run:function () {
            var label = document.createElement("label");
            label.htmlFor = "in";
            
            var input = document.createElement("input");
            input.id = "in";

            var div = document.createElement('div');
            div.appendChild(label);
            div.appendChild(input);
            
            Assert.AreEqual(null, label.control);
        }
    },
    "ControlInDocument":{
        run:function () {
            var label = document.createElement("label");
            label.htmlFor = "in";

            var input = document.createElement("input");
            input.id = "in";

            var div = document.createElement('div');
            div.id = "testarea";
            div.appendChild(label);
            div.appendChild(input);
            
            document.body.appendChild(div);

            Assert.AreEqual(input, label.control);
        }
    }
});