using Knyaz.Optimus.ScriptExecuting;

namespace Knyaz.Optimus.Dom.Interfaces
{
	/// <summary>
	/// Interface for css style declaration instances.
	/// </summary>
	[DomItem]
	public interface ICssStyleDeclaration
	{
		object this[string name] { get; }
		string this[int idx] { get; }
		string GetPropertyValue(string propertyName);
		string GetPropertyPriority(string propertyName);
	}
}
