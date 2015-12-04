var window = this;
(function (engine) {
	//ajax:http://www.w3.org/TR/XMLHttpRequest/
	window.XMLHttpRequest = function () {
		var r = engine.XmlHttpRequest();
		r.UNSENT = 0;
		r.OPENED = 1;
		r.HEADERS_RECEIVED = 2;
		r.LOADING = 3;
		r.DONE = 4;
		return r;
	};
})(this["A89A3DC7FB5944849D4DE0781117A595"]);