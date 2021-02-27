using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MySite.Areas.Download
{
	public class Files : Controller
	{
		[Authorize]
		public IActionResult Secret() => 
			File(Assembly.GetExecutingAssembly().GetManifestResourceStream("MySite.Resources.secret.zip"), "application/zip");
	}
}