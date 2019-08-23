Test("NodeTests", {
    "CommentRemove": {
        run: function () {
            var doc = document.implementation.createHTMLDocument();
            var comment = doc.createComment("pumpum");
            doc.body.appendChild(comment);
            Assert.AreEqual(1, doc.body.childNodes.length);
            comment.remove();
            Assert.AreEqual(0, doc.body.childNodes.length);
        }
    },
    "TextRemove": {
        run: function () {
            var doc = document.implementation.createHTMLDocument();
            var text = doc.createTextNode("pumpum");
            doc.body.appendChild(text);
            Assert.AreEqual(1, doc.body.childNodes.length);
            text.remove();
            Assert.AreEqual(0, doc.body.childNodes.length);
        }
    }
});

            