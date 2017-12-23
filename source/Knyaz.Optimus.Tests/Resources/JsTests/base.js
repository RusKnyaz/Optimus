var Tests = {};

function Test(fixtureName, definition) {
    Tests[fixtureName] = definition;
}


var currentRunContext = null;
function Run(fixtureName, testName) {
    var definition = Tests[fixtureName];
    var test = definition[testName];
    currentRunContext = null;
    test.run();
    return currentRunContext;
}

var Assert = {
    AreEqual: function (x1, x2, msg) {
        if (currentRunContext != null)
            return;
        if (x1 != x2) {
            currentRunContext = "Expected " + x1 + " but was " + x2;
            if (msg)
                currentRunContext = msg + "\r\n" + currentRunContext;
        }
    },
    IsNotNull : function (x) {
        if (currentRunContext != null)
            return;
        
        if (x == null)
            currentRunContext = "Expected null but was " + x;
    },
    IsNull : function(x){
        if (currentRunContext != null)
            return;

        if (x != null)
            currentRunContext = "Expected not null";
    }
};