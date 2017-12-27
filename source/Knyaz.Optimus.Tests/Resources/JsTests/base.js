var Tests = {};

function Test(fixtureName, definition) {
    Tests[fixtureName] = definition;
}


function Run(fixtureName, testName) {
    var definition = Tests[fixtureName];
    if (definition == null)
        return "Test fixture not found: " + fixtureName;
    var test = definition[testName];
    if (test == null)
        return "Test not found: " + testName;
    try {
        test.run();
    }
    catch (error){
        return error;
    }
}

var Assert = {
    AreSame: function (x1, x2, msg) {
        if (x1 == x2)
            return;

        var error = "Expected \r\n \'" + x1 + "\' but was \r\n\'" + x2 + "\'";
        if (msg)
            error = msg + "\r\n" + error;

        throw error;
    },
    AreEqual: function (x1, x2, msg) {
        if (x1 != x2) {
            var error = "Expected \r\n\'" + x1 + "\' but was \r\n\'" + x2+"\'";
            if (msg)
                error = msg + "\r\n" + error;
            
            throw error;
        }
    },
    IsNotNull : function (x) {
        if (x == null)
            throw "Expected not null";
    },
    IsNull : function(x){
        if (x != null)
            throw "Expected null but was " + x;
    }
};