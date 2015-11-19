function Detector() {
	"use strict";
	var e = "detector", t = document.documentElement;
	return {
		isBorderRadiusSupported: function() {
			var e = document.documentElement.style;
			return "string" == typeof e.borderRadius || "string" == typeof e.WebkitBorderRadius || "string" == typeof e.KhtmlBorderRadius || "string" == typeof e.MozBorderRadius
		},
		getCSS3TransformProperty: function() {
			for (var e = document.documentElement.style, t = ["transform", "MozTransform", "MsTransform", "WebkitTransform", "OTransform"], n = "", i = 0, r = t.length; r > i; ++i)
				if (void 0 !== e[t[i]]) {
					n = t[i];
					break
				}
			return this.getCSS3TransformProperty = function() { return n }, n
		},
		checkDataURLSupport: function(e) {
			var t = new Image;
			t.onload = t.onerror = function() { e(1 === this.width && 1 === this.height) }, t.src = "data:image/gif;base64,R0lGODlhAQABAIAAAAAAAP///ywAAAAAAQABAAACAUwAOw=="
		},
		isActivexEnabled: function() {
			var e = !1;
			try {
				e = !!new window.ActiveXObject("htmlfile")
			} catch (t) {
				e = !1
			}
			return e
		},
		isWin64: function() { return window.navigator && "Win64" === window.navigator.platform },
		isFullScreen: function() { return window.innerWidth && window.screen && window.screen.width && window.screen.height && window.innerHeight && window.innerWidth === screen.width && window.innerHeight === screen.height },
		isIEMetroMode: function() { return this.isFullScreen() && this.isWin64() && !this.isActivexEnabled() },
		isSVGSupported: function() {
			if ("opera" in window) return !1;
			var e = document.createElement("svg");
			return e.innerHTML = "<svg/>", e.firstChild && "http://www.w3.org/2000/svg" === e.firstChild.namespaceURI
		},
		injectElementWithStyles: function(n, i, r, o) {
			var d, a, s, u, l = document.createElement("div"), c = document.body, m = c || document.createElement("body");
			if (parseInt(r, 10)) for (; r--;) s = document.createElement("div"), s.id = o ? o[r] : e + (r + 1), l.appendChild(s);
			return d = ["&#173;", '<style id="s', e, '">', n, "</style>"].join(""), l.id = e, (c ? l : m).innerHTML += d, m.appendChild(l), c || (m.style.background = "", m.style.overflow = "hidden", u = t.style.overflow, t.style.overflow = "hidden", t.appendChild(m)), a = i(l, n), c ? l.parentNode.removeChild(l) : (m.parentNode.removeChild(m), t.style.overflow = u), !!a
		},
		isMQuerySupported: function(t) {
			var n = window.matchMedia || window.msMatchMedia, i = !1;
			return n ? i = n(t).matches : this.injectElementWithStyles("@media " + t + " { #" + e + " { position: absolute; } }", function(e) { i = "absolute" === (window.getComputedStyle ? getComputedStyle(e, null) : e.currentStyle).position }), i
		},
		isInlineBlockSupported: function() {
			var e, t = $('<span style="display:none"><div style="width:100px;display:inline-block"></div><div style="width:100px;display:inline-block"></div></span>');
			return $("body").append(t), e = t.width() > 100, t.remove(), e
		},
		isTransparentPNGSupported: function() {
			var e = navigator.userAgent.match(/MSIE (\d+)/);
			return e && e[1] ? (e = parseFloat(e[1]), "Microsoft Internet Explorer" !== navigator.appName || e > 6) : !0
		},
		isAnimationSupported: function() { for (var e = document.documentElement.style, t = ["animationName", "webkitAnimationName"], n = 0, i = t.length; i > n; n++) if (void 0 !== e[t[n]]) return !0 }
	}
}

function myMap() { this.rules = {}, this.rulesData = [] }


myMap.prototype = {
	add: function(e) { return e = e || null, e && !this.rules[e] && (this.rulesData.push(e), this.rules[e] = this.rulesData.length - 1), this },
	get: function(e) {
		var t = this.rules[e];
		return t && this.rulesData[t] || null
	},
	getAll: function() { return this.rulesData.join(" ") }
};
var detector = new Detector;
!function() {
	var e, t = new myMap;
	t.add("js").add(detector.isBorderRadiusSupported() ? "m-border-radius" : "m-no-border-radius").add(detector.isIEMetroMode() && "m-ie10-metro").add(detector.isSVGSupported() ? "i-ua_inlinesvg_yes m-svg" : "i-ua_inlinesvg_no no-data-url").add(detector.isAnimationSupported() && "i-ua_animation_yes").add(this.device && "m-touch"), detector.checkDataURLSupport(function(e) { e || (document.documentElement.className += " no-data-url") }), this.document && this.document.documentElement && (e = this.document.documentElement, e.className = e.className.replace("i-ua_js_no", "i-ua_js_yes") + " " + t.getAll())
}();