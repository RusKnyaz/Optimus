Test("DataViewTests", {
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
    },
    "ArrayBufferType":{run: function(){ Assert.AreEqual("function", typeof ArrayBuffer); }},
    "Int8ArrayType":{run: function(){ Assert.AreEqual("function", typeof Int8Array); }},
    "Uint8ArrayType":{run: function(){ Assert.AreEqual("function", typeof Uint8Array); }},
    "Int16ArrayType":{run: function(){ Assert.AreEqual("function", typeof Int16Array); }},
    "Uint16ArrayType":{run: function(){ Assert.AreEqual("function", typeof Uint16Array); }},
    "Int32ArrayType":{run: function(){ Assert.AreEqual("function", typeof Int32Array); }},
    "Uint32ArrayType":{run: function(){ Assert.AreEqual("function", typeof Uint32Array); }},
    "Float32ArrayType":{run: function(){ Assert.AreEqual("function", typeof Float32Array); }},
    "Float64ArrayType":{run: function(){ Assert.AreEqual("function", typeof Float64Array); }},
    "DataViewType":{run: function(){ Assert.AreEqual("function", typeof DataView); }},
    "NewArrayBuffer":{
        run:function(){
            var buffer= new ArrayBuffer(2);
            Assert.AreEqual(2, buffer.byteLength);
        }
    },
    "Int16FromArrayBuffer":{
        run:function(){
            var buffer= new ArrayBuffer(2);
            var arr = new Int16Array(buffer);
            Assert.AreEqual(1, arr.length);
            Assert.AreEqual(2, arr.byteLength);
        }
    },
    "Int16FromArray":{ 
        run:function() {
            var arr = new Int16Array([1, 2, -3]);
            Assert.AreEqual(1, arr[0]);
            Assert.AreEqual(2, arr[1]);
            Assert.AreEqual(-3, arr[2]);
            Assert.AreEqual(3, arr.length);
        }
    },
    "Int16FromArrayWithFloats": {
        run: function(){
            var arr = new Int16Array([1.5, 2.7]);
            Assert.AreEqual(1, arr[0]);
            Assert.AreEqual(2, arr[2]);
            Assert.AreEqual(2, arr.length);
        }
    },
    "Int16FromArrayWithFloats" : {
        run: function(){
            var arr = new Int16Array([1.5, 2.7]);
            Assert.AreEqual(1, arr[0]);
            Assert.AreEqual(2, arr[1]);
            Assert.AreEqual(2, arr.length);
        }
    },
    "Uint16InstatiatedFromArray" : {
        run: function () {
            var arr = new Uint16Array([1, 2, 3]);
            Assert.AreEqual(1, arr[0]);
            Assert.AreEqual(2, arr[1]);
            Assert.AreEqual(3, arr[2]);
            Assert.AreEqual(3, arr.length);
        }
    },
    "Uint16InstatiatedFromSignedArray" : {
        run: function () {
            var arr = new Uint16Array([1, 2, -3]);
            Assert.AreEqual(65533, arr[2]);
        }
    }
});