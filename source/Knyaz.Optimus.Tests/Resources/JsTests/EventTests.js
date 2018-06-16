Test("EventTests", {
    "EventConstructor": {
        run: function () {
            var evt = new Event("click");
            Assert.AreEqual("click", evt.type);
            Assert.AreEqual(false, evt.cancelable);
            Assert.AreEqual(false, evt.bubbles);
        }
    },
    "EventConstructorWithInit": {
        run: function () {
            var evt = new Event("click", {"bubbles": true, "cancelable": false});
            Assert.AreEqual("click", evt.type);
            Assert.AreEqual(false, evt.cancelable);
            Assert.AreEqual(true, evt.bubbles);
        }
    },
    "AddEventListenerCallOnce":{
        run: function () {
            var evt = new Event("click", {"bubbles": true, "cancelable": false});
            var doc = document.implementation.createHTMLDocument();
            var div = doc.createElement("div");
            doc.body.appendChild(div);
            var result = 0;
            div.addEventListener("click", function (ev) { result++; }, {once:true});
            div.dispatchEvent(evt);
            div.dispatchEvent(evt);
            Assert.AreEqual(1, result);
        }
    },
    "AddEventListenerPassiveTrue":{
        run: function () {
            var input = document.createElement("input");
            input.type = "checkbox";
            input.addEventListener("click", function (ev) { ev.preventDefault(); }, {passive:true});
            input.click();
            //Passive event listener can't prevent default action. Therefore checkbox becomes checked.
            Assert.AreEqual(true, input.checked);
        }
    },
    "AddEventListenerPassiveFalse":{
        run: function () {
            var input = document.createElement("input");
            input.type = "checkbox";
            input.addEventListener("click", function (ev) { ev.preventDefault(); }, {passive:false});
            input.click();
            //Event prevents default action. Therefore checkbox does not become checked.
            Assert.AreEqual(false, input.checked);
        }
    },
    "AddEventListenerCaptureOptionTrue":{
        run:function () {
            var parentDiv = document.createElement("div");
            var childDiv = document.createElement("div");
            parentDiv.appendChild(childDiv);
            var result = "";
            parentDiv.addEventListener("click", function () { result+="-parent-";}, {capture:true});
            childDiv.addEventListener("click", function () { result+="-child-";}, {capture:true});
            childDiv.click();
            Assert.AreEqual("-parent--child-", result);
        }
    },
    "AddEventListenerCaptureOptionFalse":{
        run:function () {
            var parentDiv = document.createElement("div");
            var childDiv = document.createElement("div");
            parentDiv.appendChild(childDiv);
            var result = "";
            parentDiv.addEventListener("click", function () { result+="-parent-";}, {capture:false});
            childDiv.addEventListener("click", function () { result+="-child-";}, {capture:false});
            childDiv.click();
            Assert.AreEqual("-child--parent-", result);
        }
    }
});