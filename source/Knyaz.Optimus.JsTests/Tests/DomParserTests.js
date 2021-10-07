Test("DomParserTests", {
	"DomParserType":{
		run: function(){
			Assert.AreEqual("function", typeof DOMParser);
		}
	},
	"DomParserConstructor": {
		run: function () {
			var parser = new DOMParser();
			Assert.IsNotNull(parser.parseFromString);
		}
	},
	"DomParserWindowConstructor": {
		run: function () {
			var parser = new window.DOMParser();
			Assert.IsNotNull(parser.parseFromString);
		}
	},
	"ParseHtml":{
		run: function () {
			var parser = new DOMParser();
			var doc = parser.parseFromString("<html><title>asd</title></html>", "text/html");
			Assert.IsNotNull(doc);
			Assert.AreEqual("asd", doc.title);
		}
	}
});