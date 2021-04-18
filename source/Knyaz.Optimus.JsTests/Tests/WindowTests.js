Test("WindowTests", {
    "FunctionsDefined": {
        run: function () {
            Assert.IsNotNull(addEventListener, "addEventListener");
            Assert.IsNotNull(removeEventListener, "removeEventListener");
            Assert.IsNotNull(dispatchEvent, "dispatchEvent");
            Assert.IsNotNull(setTimeout, "setTimeout");
            Assert.IsNotNull(clearTimeout, "clearTimeout");
            Assert.IsNotNull(setInterval, "setInterval");
            Assert.IsNotNull(clearInterval, "clearInterval");
            Assert.IsNotNull(alert, "alert");
            Assert.AreEqual(setInterval, setInterval, "setInterval == setInterval");
            Assert.AreEqual(alert, alert, "setInterval == setInterval");
        }
    },
    "WindowIsSelf":{
        run: function () {
            Assert.IsNotNull(window, "window != null");
            Assert.AreEqual(self, window, "window === self");
        }
    },
    "NewUIEvent":{
        run:function(){
            var evt = new UIEvent('resize');
            Assert.AreEqual("resize", evt.type);
        }
    },
    "NewEvent":{
        run:function(){
            var evt = new Event("look", {"bubbles":true, "cancelable":false});
            Assert.AreEqual("look", evt.type);
            Assert.AreEqual(true, evt.bubbles);
            Assert.AreEqual(false, evt.cancelable);
        }
    }
});