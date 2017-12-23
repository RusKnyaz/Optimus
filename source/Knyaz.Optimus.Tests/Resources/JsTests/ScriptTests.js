Test("ScriptTests", {
    "AppendScriptTwice": {
        run: function () {
            window.counter = 0;
            var script = document.createElement("script");
            script.textContent = "window.counter++;"
            document.head.appendChild(script);
            document.head.removeChild(script);
            document.head.appendChild(script);
            Assert.AreEqual(1, window.counter);
        }
    },
    "AppendScriptClone": {
        run: function () {
            window.counter = 0;
            var script = document.createElement("script");
            script.textContent = "window.counter++;"
            document.head.appendChild(script);
            document.head.appendChild(script.cloneNode(true));
            Assert.AreEqual(1, window.counter);
        }
    },
    "AppendClonesOfScript": {
        run: function () {
            window.counter = 0;
            var script = document.createElement("script");
            script.textContent = "window.counter++;"
            var clonedScript = script.cloneNode(true);
            document.head.appendChild(script);
            document.head.appendChild(clonedScript);
            Assert.AreEqual(2, window.counter);
        }
    },
    "OnloadOnEmbeddedScript": {
        run: function () {
            var counter = 0;
            var script = document.createElement("script");
            script.onload = function () { counter++; };
            document.head.appendChild(script);
            Assert.AreEqual(0, counter);
        }
    },
    "OnloadOnExternalScript": {
        run: function () {
            window.counter = "";
            var script = document.createElement("script");
            script.onload = function () { window.counter += "-on load-"; };
            script.src = "sample.js";
            document.head.appendChild(script);
            Assert.AreEqual("", window.counter);//external script executed after the current script finished.
        }
    },
    "CloneScript": {
        run: function () {
            var script = document.createElement("script");
            script.id = "yohoho";
            script.text = "console.log('hi');";
            var cloneScript = script.cloneNode(false);
            Assert.AreEqual("", cloneScript.text ||"");
            Assert.AreEqual("yohoho", cloneScript.id);
        }
    },
    "DeepCloneScript": {
        run: function () {
            var script = document.createElement("script");
            script.id = "yohoho";
            script.text = "console.log('hi');";
            var cloneScript = script.cloneNode(true);
            Assert.AreEqual("console.log('hi');", cloneScript.text);
            Assert.AreEqual("yohoho", cloneScript.id);
        }
    }
});