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
    }
});