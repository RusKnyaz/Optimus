using System.IO;

namespace Knyaz.Optimus.Tests.Resources
{
    class R
    {
		static string GetString(string file)
		{
			using (var reader = new StreamReader(typeof(R).Assembly.GetManifestResourceStream(file)))
			{
				return reader.ReadToEnd();
			}
		}

		public static string JQueryJs => GetString("Knyaz.Optimus.Tests.Resources.jquery-2.1.3.js");
		public static string KnockoutJs => GetString("Knyaz.Optimus.Tests.Resources.knockout.js");
		public static string RequireJs => GetString("Knyaz.Optimus.Tests.Resources.requirejs.js");
		public static string Template => GetString("Knyaz.Optimus.Tests.Resources.template.js");
		public static string Text => GetString("Knyaz.Optimus.Tests.Resources.text.js");
		public static string StringTemplateEngine => GetString("Knyaz.Optimus.Tests.Resources.stringTemplateEngine.js");
		public static string JQueryFormJs => GetString("Knyaz.Optimus.Tests.Resources.jQuery.Form.js");

		public static string JsTestsBase => GetString("Knyaz.Optimus.Tests.Resources.JsTests.base.js");
		public static string JsTestsDataViewTests => GetString("Knyaz.Optimus.Tests.Resources.JsTests.DataViewTests.js");
		public static string JsTestsElementTests => GetString("Knyaz.Optimus.Tests.Resources.JsTests.ElementTests.js");
	}
}
