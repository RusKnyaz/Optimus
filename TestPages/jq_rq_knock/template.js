define(["require", "text", "stringTemplateEngine"], function(require, text) {
	var loader = {
		load: function(name, req, load, config) {
			var onLoad = config.isBuild ? load : function (content) {
				var tmpDiv = $("<div>");
				tmpDiv.html(content);
				var scripts = tmpDiv.find("script[type='text/html']");
				if (scripts.length > 0) {
					scripts.each(function(_) {
						var templateId = $(this).attr("id");
						ko.templates[templateId] = $(this).html();
					});
				} else {
					var safeName = inferTemplateName(name);
					ko.templates[safeName] = content;
				}
				load(content);
			};

			text.load(name, req, onLoad, config);
		},
		write: function (pluginName, moduleName, write, config) {
			text.write("text", moduleName, write, config);

			var safeName = inferTemplateName(moduleName);

			write.asModule(pluginName + "!" + moduleName,
				"define(['text!" + moduleName + "', 'knockout', 'stringTemplateEngine'], function (content, ko) {" +
					"ko.templates['" + safeName + "'] = content;" +
				"});\n"
			);
		}
	};

	function inferTemplateName(name) {
		var templateName;

		var index = name.indexOf("!");
		if(index !== -1) {
			//use the template name that is specified
			templateName = name.substring(index + 1, name.length);
		} else {
			//use the file name sans the path as the template name
			var parts = name.split("/");
			templateName = parts[parts.length - 1];

			//remove the extension from the template name
			var extensionIndex = templateName.lastIndexOf(".");
			if(extensionIndex !== -1) {
				templateName = templateName.substring(0, extensionIndex);
			}
		}

		return templateName;
	}

	return loader;
});