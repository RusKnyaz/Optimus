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
    "SetInt16DefaultEndian":{
        run: function () {
            var buffer = new ArrayBuffer(4);
            var dataView = new DataView(buffer);
            dataView.setInt16(1, 3);
            Assert.AreEqual(3, dataView.getInt16(1));
        }
    },
    "SetInt16":{
        run: function () {
            var buffer = new ArrayBuffer(4);
            var dataView = new DataView(buffer);
            dataView.setInt16(1, 3, false);
            Assert.AreEqual(768, dataView.getInt16(1, true));
        }
    },
    "SetFloat32ByIndexer":{
        run: function () {
            var dataView = new Float32Array(new ArrayBuffer(4));
            dataView[0] = 20;
            Assert.AreEqual(20, dataView[0], "dataView[0]");
        }
    },
    "SetFloat64ByIndexer":{
        run: function () {
            var dataView = new Float32Array(new ArrayBuffer(8));
            dataView[0] = 20;
            Assert.AreEqual(20, dataView[0], "dataView[0]");
        }
    },
    "SetInt16ByIndexer":{
        run: function () {
            var dataView = new Int16Array(new ArrayBuffer(4));
            dataView[0] = 20;
            Assert.AreEqual(20, dataView[0], "dataView[0]");
        }
    },
    "SetUInt32ByIndexer":{
        run: function () {
            var dataView = new Uint32Array(new ArrayBuffer(4));
            dataView[0] = 20;
            Assert.AreEqual(20, dataView[0], "dataView[0]");
        }
    },
    "SetInt32ByIndexer":{
        run: function () {
            var dataView = new Int32Array(new ArrayBuffer(4));
            dataView[0] = 20;
            Assert.AreEqual(20, dataView[0], "dataView[0]");
        }
    },
    "SetUInt16ByIndexer":{
        run: function () {
            var dataView = new Uint16Array(new ArrayBuffer(4));
            dataView[0] = 20;
            Assert.AreEqual(20, dataView[0], "dataView[0]");
        }
    },
    "SetInt16ByIndexer":{
        run: function () {
            var dataView = new Int16Array(new ArrayBuffer(4));
            dataView[0] = 20;
            Assert.AreEqual(20, dataView[0], "dataView[0]");
        }
    },
    "SetUInt8ByIndexer":{
        run: function () {
            var dataView = new Int8Array(new ArrayBuffer(4));
            dataView[0] = 20;
            Assert.AreEqual(20, dataView[0], "dataView[0]");
        }
    },
    "SetInt8ByIndexer":{
        run: function () {
            var dataView = new Uint8Array(new ArrayBuffer(4));
            dataView[0] = 20;
            Assert.AreEqual(20, dataView[0], "dataView[0]");
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
    "Int16FromSize":{
        run:function() {
            var arr = new Int16Array(4);
            Assert.AreEqual(8, arr.byteLength);
        }
    },
    "UInt16FromSize":{
        run:function() {
            var arr = new Uint16Array(4);
            Assert.AreEqual(8, arr.byteLength);
        }
    },
    "Int32FromSize":{
        run:function() {
            var arr = new Int32Array(4);
            Assert.AreEqual(16, arr.byteLength);
        }
    },
    "UInt32FromSize":{
        run:function() {
            var arr = new Uint32Array(4);
            Assert.AreEqual(16, arr.byteLength);
        }
    },
    "Int8FromSize":{
        run:function() {
            var arr = new Int8Array(4);
            Assert.AreEqual(4, arr.byteLength);
        }
    },
    "UInt8FromSize":{
        run:function() {
            var arr = new Uint8Array(4);
            Assert.AreEqual(4, arr.byteLength);
        }
    },
    "Float32FromSize":{
        run:function() {
            var arr = new Float32Array(4);
            Assert.AreEqual(16, arr.byteLength);
        }
    },
    "Float64FromSize":{
        run:function() {
            var arr = new Float64Array(4);
            Assert.AreEqual(32, arr.byteLength);
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
    "Uint16InstantiatedFromArray" : {
        run: function () {
            var arr = new Uint16Array([1, 2, 3]);
            Assert.AreEqual(1, arr[0]);
            Assert.AreEqual(2, arr[1]);
            Assert.AreEqual(3, arr[2]);
            Assert.AreEqual(3, arr.length);
        }
    },
    "Uint16InstantiatedFromSignedArray" : {
        run: function () {
            var arr = new Uint16Array([1, 2, -3]);
            Assert.AreEqual(65533, arr[2]);
        }
    },
    "Uint8ArrayFromNull":{
        run: function(){
            var arr = new Uint8Array(null);
            Assert.AreEqual(0, arr.length);
        }
    },
    "Uint16ArrayFromNull":{
        run: function(){
            var arr = new Uint16Array(null);
            Assert.AreEqual(0, arr.length);
        }
    },
    "Int8ArrayFromNull":{
        run: function(){
            var arr = new Int8Array(null);
            Assert.AreEqual(0, arr.length);
        }
    },
    "Int16ArrayFromNull":{
        run: function(){
            var arr = new Int16Array(null);
            Assert.AreEqual(0, arr.length);
            Assert.IsNotNull(arr.buffer);
        }
    },
    "BytesPerElement" :{
        run : function() {
            Assert.AreEqual(1, new Int8Array(2).BYTES_PER_ELEMENT, "new Int8Array(2).BYTES_PER_ELEMENT");
            Assert.AreEqual(1, new Uint8Array(2).BYTES_PER_ELEMENT, "new Uint8Array(2).BYTES_PER_ELEMENT");
            Assert.AreEqual(2, new Int16Array(2).BYTES_PER_ELEMENT, "new Int16Array(2).BYTES_PER_ELEMENT");
            Assert.AreEqual(2, new Uint16Array(2).BYTES_PER_ELEMENT, "new Uint16Array(2).BYTES_PER_ELEMENT");
            Assert.AreEqual(4, new Int32Array(2).BYTES_PER_ELEMENT, "new Int32Array(2).BYTES_PER_ELEMENT");
            Assert.AreEqual(4, new Uint32Array(2).BYTES_PER_ELEMENT, "new Uint32Array(2).BYTES_PER_ELEMENT");
            Assert.AreEqual(4, new Float32Array(2).BYTES_PER_ELEMENT, "new Float32Array(2).BYTES_PER_ELEMENT");
            Assert.AreEqual(8, new Float64Array(2).BYTES_PER_ELEMENT, "new Float64Array(2).BYTES_PER_ELEMENT");
            
            Assert.AreEqual(1, Int8Array.BYTES_PER_ELEMENT, "Int8Array.BYTES_PER_ELEMENT");
            Assert.AreEqual(1, Uint8Array.BYTES_PER_ELEMENT, "Uint8Array.BYTES_PER_ELEMENT");
            Assert.AreEqual(2, Int16Array.BYTES_PER_ELEMENT, "Int16Array.BYTES_PER_ELEMENT");
            Assert.AreEqual(2, Uint16Array.BYTES_PER_ELEMENT, "Uint16Array.BYTES_PER_ELEMENT");
            Assert.AreEqual(4, Int32Array.BYTES_PER_ELEMENT, "Int32Array.BYTES_PER_ELEMENT");
            Assert.AreEqual(4, Uint32Array.BYTES_PER_ELEMENT, "Uint32Array.BYTES_PER_ELEMENT");
            Assert.AreEqual(4, Float32Array.BYTES_PER_ELEMENT, "Float32Array.BYTES_PER_ELEMENT");
            Assert.AreEqual(8, Float64Array.BYTES_PER_ELEMENT, "Float64Array.BYTES_PER_ELEMENT");
        }
    },
    "GetPrototypeOf":{
        run: function(){
            Assert.AreEqual("[object ArrayBuffer]", Object.getPrototypeOf(new ArrayBuffer()).toString());
        }
    }
});