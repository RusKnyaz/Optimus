Test("HtmlOptionElementTests", {
    "Form": {
        run: function () {
            var form = document.createElement("form");
            var select = document.createElement("select");
            var option = document.createElement("option");
            form.appendChild(select);
            select.appendChild(option);
            Assert.AreEqual(form, option.form);
        }
    },
    "NestedOptgroups":{
        run:function () {
            var div = document.createElement("div");
            div.innerHTML = "<select><optgroup><optgroup><option></option></optgroup></optgroup></select>";
            Assert.AreEqual("<select><optgroup></optgroup><optgroup><option></option></optgroup></select>", 
                div.innerHTML.toLowerCase());
        }
    },
    "SetLabelSetsAttribute":{
        run:function () {
            var option = document.createElement("option");
            option.label = "a";
            Assert.AreEqual("a", option.getAttribute("label"));
        }
    },
    "GetLabelGetsAttributeIfExists":{
        run:function () {
            var option = document.createElement("option");
            option.setAttribute("label", "b");
            Assert.AreEqual("b", option.label);
        }
    },
    "GetLabelGetsTextIfThereIsNoAttribute":{
        run:function () {
            var option = document.createElement("option");
            option.text = "b";
            Assert.AreEqual("b", option.label);
        }
    },
    "GetLabelWhenAttributeIsEmpty":{
        run:function () {
            var option = document.createElement("option");
            option.setAttribute("label", "");
            option.text = "txt";
            Assert.AreEqual("", option.label);
        }
    },
    "DivInOptionSkipped":{
        run:function () {
            var div = document.createElement("div");
            div.innerHTML="<select><option>1<div>2</div>3</option></select>";
            var option = div.getElementsByTagName("option")[0];
            Assert.AreEqual("123", option.text);
            Assert.AreEqual("123", option.innerHTML);
        }
    },
    "SetTextDoesNotParseHtml":{
        run:function () {
            var option = document.createElement("option");
            option.text = "<div></div><span></span>";
            Assert.AreEqual(1, option.childNodes.length);
            Assert.AreEqual("#text", option.firstChild.nodeName)
        }
    }
});