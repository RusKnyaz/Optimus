namespace WebBrowser.Dom.Elements
{
	internal interface IFormElement
	{
		HtmlFormElement Form { get; }
		string Name { get; }
		string Value { get; }
	}
}
