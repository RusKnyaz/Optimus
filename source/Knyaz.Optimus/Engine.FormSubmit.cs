using System.Linq;
using System.Text;
using Knyaz.Optimus.Dom;
using Knyaz.Optimus.Dom.Elements;
using Knyaz.Optimus.ResourceProviders;

namespace Knyaz.Optimus
{
	partial class Engine
	{
		// todo: rewrite and complete the stuff
		private async void OnFormSubmit(HtmlFormElement form)
		{
			if (string.IsNullOrEmpty(form.Action))
				return;

			var dataElements = form.Elements.OfType<IFormElement>().Where(x => !string.IsNullOrEmpty(x.Name));

			var replaceSpaces = form.Method != "post" || form.Enctype != "multipart/form-data";
			
			var data = string.Join("&", dataElements.Select(x => 
				x.Name + "=" + (x.Value != null ? (replaceSpaces ? x.Value.Replace(' ', '+') : x.Value) : "")
			));

			if(form.Enctype == "application/x-www-form-urlencoded")
			{
				data = System.Uri.EscapeUriString(data);
			}

			var isGet = form.Method == "get";

			var url = isGet ? form.Action.Split('?')[0]+"?"+data : form.Action;

			if (form.Action != "about:blank")
			{
				var document = new Document(Window);

				HtmlIFrameElement targetFrame;
				if (!string.IsNullOrEmpty(form.Target) &&
				    (targetFrame = Document.GetElementsByName(form.Target).FirstOrDefault() as HtmlIFrameElement) != null)
				{
					targetFrame.ContentDocument = document;
				}
				else
				{
					ScriptExecutor.Clear();
					Document = document;
				}

				var request = ResourceProvider.CreateRequest(url);
				if (!isGet && request is HttpRequest httpRequest)
				{
					//todo: use right encoding and enctype
					httpRequest.Method = "POST";
					httpRequest.Data = Encoding.UTF8.GetBytes(data);
				}

				var response = await ResourceProvider.SendRequestAsync(request);

				//what should we do if the frame is not found?
				if (response.Type.StartsWith(ResourceTypes.Html))
				{
					LoadFromResponse(document, response);
				}
			}
			//todo: handle 'about:blank'
		}
	}
}