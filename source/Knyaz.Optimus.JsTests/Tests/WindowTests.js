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
    }
});