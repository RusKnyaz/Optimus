using System;
using System.Linq;
using System.Net;
using System.Text;
using Knyaz.Optimus.Dom;
using Knyaz.Optimus.Dom.Elements;
using Knyaz.Optimus.ResourceProviders;

namespace Knyaz.Optimus
{
	partial class Engine
	{
		// todo: rewrite and complete the stuff
		private async void OnFormSubmit(HtmlFormElement form, HtmlElement submitElement)
		{
			var method = form.Method;
			var action = form.Action;
			var enctype = form.Enctype;
			var target = form.Target;
			
			if (submitElement is HtmlButtonElement button)
			{
				if (!string.IsNullOrEmpty(button.FormMethod))
					method = button.FormMethod;

				if (!string.IsNullOrEmpty(button.FormAction))
					action = button.FormAction;
				
				if (!string.IsNullOrEmpty(button.FormEnctype))
					enctype = button.FormEnctype;
				
				if (!string.IsNullOrEmpty(button.FormTarget))
					target = button.FormTarget;
			}
			
			if (string.IsNullOrEmpty(action))
				return;
			
			var dataElements = form.Elements.OfType<IFormElement>()
				.Where(x => !string.IsNullOrEmpty(x.Name) && 
				            !(x is HtmlInputElement input && input.Type == "checkbox" && !input.Checked) //skip unchecked cheboxes
				            );

			var replaceSpaces = method != "post" || enctype != "multipart/form-data";

			var data = string.Empty;

			if (method == "get")
			{
				data = string.Join("&", dataElements.Select(x =>
					x.Name + "=" + (GetValue(x) is string strValue ? (replaceSpaces ? strValue.Replace(' ', '+') : strValue) : "")
				));

				if (enctype == "application/x-www-form-urlencoded")
				{
					data = System.Uri.EscapeUriString(data);
				}
			}
			else if(method == "post")
			{
				if (enctype == "application/x-www-form-urlencoded")
				{
					data = string.Join("&", dataElements.Select(x =>
						x.Name + "=" + (GetValue(x) is string strValue ? WebUtility.UrlEncode(strValue).Replace("!", "%21") : "")
					));
				}
				else if (enctype == "text/plain")
				{
					data = string.Concat(dataElements.Select(x => (x.Name + "=" + (GetValue(x) ?? "")) + "\r\n"));
				}
				else
				{
					throw new NotImplementedException("multipart-form-data form encryption type is not supported");
				}
			}

			var isGet = method == "get";
			
			var url = isGet 
				? action.Split('?')[0] + (string.IsNullOrEmpty(data) ? "" : "?"+data) 
				: action;

			if (action != "about:blank")
			{
				var document = new Document(Window);

				HtmlIFrameElement targetFrame;
				if (!string.IsNullOrEmpty(form.Target) &&
				    (targetFrame = Document.GetElementsByName(target).FirstOrDefault() as HtmlIFrameElement) != null)
				{
					targetFrame.ContentDocument = document;
				}
				else
				{
					ScriptExecutor?.Clear();
					Document = document;
				}

				var request = CreateRequest(url);
				if (!isGet)
				{
					//todo: use right encoding and enctype
					request.Method = "POST";
					request.Data = Encoding.UTF8.GetBytes(data);
				}

				var response = await ResourceProvider.SendRequestAsync(request);

				//what should we do if the frame is not found?
				if (response.Type == null || response.Type.StartsWith(ResourceTypes.Html))
				{
					LoadFromResponse(document, response);
				}
			}
			//todo: handle 'about:blank'
		}

		static string GetValue(IFormElement elt) =>
			elt is HtmlInputElement input && input.Type == "checkbox" ? input.Checked ? "true" : "false"
				: elt.Value;
	}
}