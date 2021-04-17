namespace Knyaz.Optimus.Dom.Elements
{
	internal interface IFormElement
	{
		HtmlFormElement Form { get; }
		string Name { get; }
		string Value { get; }
		bool Disabled { get; }
	}
}
