Test("CommentTests", {
    "Clone": {
        run: function () {
            var com = document.createComment("Some comment")
            var clone = com.cloneNode(true);
            Assert.AreEqual("Some comment", clone.data);
        }
    },
    "TextContent":{
        run:function () {
            var com = document.createComment("Some comment");
            Assert.AreEqual("Some comment", com.textContent);
        }
    },
    "AppendData":{
        run:function(){
            var com = document.createComment("Some");
            com.appendData("111");
            Assert.AreEqual("Some111", com.data);
        }
    },
    "AppendDataNull":{  
        run:function () {
            var com = document.createComment("Some");
            com.appendData(null);
            Assert.AreEqual("Somenull", com.data);
        }
    },
    "DeleteData":{
        run:function () {
            var com = document.createComment("Some");
            com.deleteData(1, 2);
            Assert.AreEqual("Se", com.data);
        }
    },
    "InsertData":{
        run:function () {
            var com = document.createComment("Se");
            com.insertData(1, "om");
            Assert.AreEqual("Some", com.data);
        }
    },
    "ReplaceData":{
        run:function () {
            var com = document.createComment("Some");
            com.replaceData(1, 2, "111");
            Assert.AreEqual("S111e", com.data);
        }
    },
    "SubstringData":{
        run:function () {
            var com = document.createComment("Some");
            var sub = com.substringData(1,2);
            Assert.AreEqual("om", sub);
            Assert.AreEqual("Some", com.data);
        }
    },
    "LengthTest":{
        run:function () {
            var com = document.createComment("Some comment");
            Assert.AreEqual(12, com.length);
        }
    },
    "AppendChildThrows":{
        run:function(){
            var com = document.createComment("T");
            var div = document.createElement("div");
            Assert.Throws(function (){
                com.appendChild(div);
            } );
        }
    }    
    
});