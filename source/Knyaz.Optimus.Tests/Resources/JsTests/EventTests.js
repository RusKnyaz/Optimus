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
    }
});