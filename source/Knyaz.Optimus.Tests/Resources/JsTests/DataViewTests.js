Test("DataViewJsTests", {
    "ConstructWithDefaultOffset": {
        run: function () {
            var dv = new DataView(new ArrayBuffer(10));
            Assert.AreEqual(0, dv.byteOffset);
            Assert.AreEqual(10, dv.byteLength);
        }
    },
    "ConstructWithDefaultLength":{
        run:function() {
            var dv = new DataView(new ArrayBuffer(10), 2);
            Assert.AreEqual(2, dv.byteOffset);
            Assert.AreEqual(8, dv.byteLength);
        }
    },
    "Construct": {
        run: function () {
            var dv = new DataView(new ArrayBuffer(10), 1, 2);
            Assert.AreEqual(1, dv.byteOffset);
            Assert.AreEqual(2, dv.byteLength);
        }
    },
    "SetInt32DefaultEndian":{
        run: function () {
            var buffer = new ArrayBuffer(4);
            var dataView = new DataView(buffer);
            dataView.setInt16(1, 3);
            Assert.AreEqual(3, dataView.getInt16(1));
        }
    },
    "SetInt32":{
        run: function () {
            var buffer = new ArrayBuffer(4);
            var dataView = new DataView(buffer);
            dataView.setInt16(1, 3, false);
            Assert.AreEqual(768, dataView.getInt16(1, true));
        }
    }
});