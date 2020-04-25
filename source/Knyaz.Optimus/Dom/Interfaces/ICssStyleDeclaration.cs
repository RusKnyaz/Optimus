namespace Knyaz.Optimus.Dom.Interfaces
{
	/// <summary>
	/// Interface for css style declaration instances.
	/// </summary>
	public interface ICssStyleDeclaration
	{
		/// <summary>
		/// Gets the property value.
		/// </summary>
		/// <param name="name">Name of property to get.</param>
		object this[string name] { get; }
		
		/// <summary>
		/// Get tye style property name.
		/// </summary>
		/// <param name="idx">Index of property to get.</param>
		string this[int idx] { get; }
		
		/// <summary>
		/// Gets the style property value.
		/// </summary>
		/// <param name="propertyName">Property name to get.</param>
		string GetPropertyValue(string propertyName);
		
		/// <summary>
		/// Gets the style property priority.
		/// </summary>
		/// <param name="propertyName">Property name to get.</param>
		string GetPropertyPriority(string propertyName);
	}
}
