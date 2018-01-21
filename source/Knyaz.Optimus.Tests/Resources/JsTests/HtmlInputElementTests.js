Test("HtmlInputElementTests", {
    "InputChangeCheckedOnClick":{
        run:function (){
            var i = document.createElement("input");
            i.type = "checkbox";
            Assert.AreEqual(false, i.checked);
            i.click();
            Assert.AreEqual(true, i.checked);
            i.click();
            Assert.AreEqual(false, i.checked);
        }
    },
    "InputCheckChangingPrevented":{
        run:function () {
            var i = document.createElement("input");
            i.type = "checkbox";
            Assert.AreEqual(false, i.checked);
            i.onclick = function(e){ e.preventDefault(); };
            i.click();
            Assert.AreEqual(false, i.checked);
        }
    },
    "InputCheckChangingPreventedByReturnValue":{
        run:function () {
            var i = document.createElement("input");
            i.type = "checkbox";
            Assert.AreEqual(false, i.checked);
            i.onclick = function(e){ return false;};
            i.click();
            Assert.AreEqual(false, i.checked);
        }
    },
    "PreventDefaultDoesNotStopPropogation":{
        run:function () {
            var div = document.createElement("div");
            var i = document.createElement("input");
            var counter = 0;
            div.addEventListener("click", function(){counter++;}, true);
            div.addEventListener("click", function(){counter++;}, false);
            div.appendChild(i);
            i.type = "checkbox";
            i.onclick = function(e){	e.preventDefault();};
            i.click();
            Assert.AreEqual(2, counter);
        }
    },
    "PreventInCaptureEventListener":{
        run:function () {
            var i = document.createElement("input");
            i.addEventListener("click", function(e){ e.preventDefault();}, true);
            i.type = "checkbox";
            var onclicked = false;
            i.onclick = function(){ onclicked = true;};
            i.click();
            Assert.AreEqual(false, i.checked, "checked");
            Assert.AreEqual(true, onclicked, "onclick called");
        }
    },
    "PreventInBubbleEventListener":{
        run:function () {
            var i = document.createElement("input");
            i.addEventListener("click", function(e){ e.preventDefault();}, false);
            i.type = "checkbox";
            var onclicked = false;
            i.onclick = function(){ onclicked = true;};
            i.click();
            Assert.AreEqual(false, i.checked, "checked");
            Assert.AreEqual(true, onclicked, "onclick called");
        }
    },
    "DefaultActionOrder":{
        run:function () {
            var res = "";
            var input = document.createElement("input");
            input.type = "checkbox";
            input.addEventListener("click", function(e){ res+="- input C " + input.checked;}, false);
            input.addEventListener("click", function(e){ res+="- input B " + input.checked;}, true);

            var div = document.createElement("div");
            div.addEventListener("click", function(e){ res+="- div C " + input.checked;}, false);
            div.addEventListener("click", function(e){ res+="- div B " + input.checked;}, true);
            div.appendChild(input);

            res+="- before all " + input.checked;
            input.click();
            res+="- after all " + input.checked;

            Assert.AreEqual("- before all false- div B true- input C true- input B true- div C true- after all true", res);
        }
    },
    "UncheckInHandler":{
        run:function () {
            var input = document.createElement("input");
            input.type = "checkbox";
            input.onclick = function (e) { input.checked = false;  };
            Assert.AreEqual(false, input.checked);
        }
    },
    "CheckInHandlerAndPreventDefault":{
        run:function () {
            var input = document.createElement("input");
            input.type = "checkbox";
            input.onclick = function (e) {
                input.checked = false;
                e.preventDefault();
            };
            
            Assert.AreEqual(false, input.checked);
        }
    },
    "UncheckInHandlerAndPreventDefault":{
        run:function () {
            var input = document.createElement("input");
            input.type = "checkbox";
            input.onclick = function (e) {
                input.checked = true;
                e.preventDefault();
            };

            Assert.AreEqual(false, input.checked);
        }
    },
    "ClickOnLabel":{
        run:function () {
            var i = document.createElement('input');
            i.id='test-input';
            i.type='checkbox';
            var l = document.createElement('label');
            l.htmlFor = 'test-input';
            
            var div = document.createElement("div");
            div.id = "testarea";

            div.appendChild(i);
            div.appendChild(l);
            
            document.body.appendChild(div);
            
            l.click();
            Assert.AreEqual(true, i.checked);
        }
    },
    "LabelClickEventsOrder":{
        run:function () {
            var res = "";

            var i = document.createElement('input');
            i.id='test-input';
            i.type='checkbox';
            i.onclick=function(e){res+="-input.onclick"};
            i.addEventListener("click",function(){res+='-input.listener.bub'},false)
            i.addEventListener("click",function(){res+='-input.listener.cap'},true)
            var l = document.createElement('label');
            l.htmlFor = 'test-input';
            l.onclick=function(){res+='-label.onclick'};
            l.addEventListener("click",function(){res+='-label.listener.bub'},false)
            l.addEventListener("click",function(){res+='-label.listener.cap'},true)

            var div = document.createElement('div');
            div.addEventListener("click",function(){res+='-div.listener.bub'},false)
            div.addEventListener("click",function(){res+='-div.listener.cap'},true)
            div.appendChild(i);
            div.appendChild(l);

            l.click();
            Assert.AreEqual(
                "-div.listener.cap"+
                "-label.onclick"+
                "-label.listener.bub"+
                "-label.listener.cap"+
                "-div.listener.bub",res);
        }
    },
    "LabelClickEventsOrderInDocument":{
        run:function () {
            var res = "";

            var i = document.createElement('input');
            i.id='test-input';
            i.type='checkbox';
            i.onclick=function(e){res+="-input.onclick"};
            i.addEventListener("click",function(){res+='-input.listener.bub'},false)
            i.addEventListener("click",function(){res+='-input.listener.cap'},true)
            var l = document.createElement('label');
            l.htmlFor = 'test-input';
            l.onclick=function(){res+='-label.onclick'};
            l.addEventListener("click",function(){res+='-label.listener.bub'},false)
            l.addEventListener("click",function(){res+='-label.listener.cap'},true)

            var div = document.createElement('div');
            div.id = "testarea";
            div.addEventListener("click",function(){res+='-div.listener.bub'},false)
            div.addEventListener("click",function(){res+='-div.listener.cap'},true)
            div.appendChild(i);
            div.appendChild(l);
            document.body.appendChild(div);
            l.click();
            Assert.AreEqual(
                "-div.listener.cap"+
                "-label.onclick"+
                "-label.listener.bub"+
                "-label.listener.cap"+
                "-div.listener.bub"+
                "-div.listener.cap"+
                "-input.onclick"+
                "-input.listener.bub"+
                "-input.listener.cap"+
                "-div.listener.bub",res);
        }
    },
    "LabelClickEventsOrderInDocumentPreventDefault":{
        run:function () {
            var res = "";

            var i = document.createElement('input');
            i.id='test-input';
            i.type='checkbox';
            i.onclick=function(e){res+="-input.onclick"};
            i.addEventListener("click",function(){res+='-input.listener.bub'},false)
            i.addEventListener("click",function(){res+='-input.listener.cap'},true)
            var l = document.createElement('label');
            l.htmlFor = 'test-input';
            l.onclick=function(e){res+='-label.onclick';e.preventDefault()};
            l.addEventListener("click",function(){res+='-label.listener.bub'},false)
            l.addEventListener("click",function(){res+='-label.listener.cap'},true)

            var div = document.createElement('div');
            div.id = "testarea";
            div.addEventListener("click",function(){res+='-div.listener.bub'},false)
            div.addEventListener("click",function(){res+='-div.listener.cap'},true)
            div.appendChild(i);
            div.appendChild(l);
            
            document.body.appendChild(div);

            l.click();
            Assert.AreEqual(
                "-div.listener.cap"+
                "-label.onclick"+
                "-label.listener.bub"+
                "-label.listener.cap"+
                "-div.listener.bub",
                res);
            
            Assert.AreEqual(false, i.checked);
        }
    }

});