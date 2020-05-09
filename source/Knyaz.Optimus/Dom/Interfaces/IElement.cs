using Knyaz.Optimus.Dom.Elements;

namespace Knyaz.Optimus.Dom.Interfaces
{
	/// <summary>
	/// Represents an element in an HTML document.
	/// </summary>
	public interface IElement : INode
	{
		/// <summary>
		/// The name of the element.
		/// </summary>
		string TagName { get; }
		string Id { get; }
		string ClassName { get; set; }
		string InnerHTML { get; set; }
		string TextContent { get; set; }

		/// <summary> Returns a collection containing all descendant elements with the specified tag name. </summary>
		/// <param name="tagNameSelector">A string that specifies the tagname to search for. The value "*" matches all tags</param>
		HtmlCollection GetElementsByTagName(string tagNameSelector);

		/// <summary>Returns a collection containing all descendant elements with the specified class name.</summary>
		/// <param name="name">The class name (or multiple names divided by spaces) of the elements to get.</param>
		HtmlCollection GetElementsByClassName(string name);

		/// <summary> Retrieves an attribute value by name. </summary>
		/// <param name="name">The name of the attribute to retrieve.</param>
		/// <returns>The <see cref="Attr"/> value as a string, or the string.Empty if that attribute does not have a specified or default value.</returns>
		Attr GetAttributeNode(string name);

		/// <summary> Retrieves an attribute node by name. </summary>
		/// <param name="name">The name (nodeName) of the attribute to retrieve.</param>
		/// <returns>The <see cref="Attr"/> node with the specified name (nodeName) or <c>null</c> if there is no such attribute.</returns>
		string GetAttribute(string name);

		/// <summary> Removes an attribute by name. </summary>
		/// <param name="name">The name of the attribute to remove.</param>
		void RemoveAttribute(string name);

		/// <summary>
		/// Adds a new attribute. If an attribute with that name is already present in the element, its value is changed to be that of the value parameter.
		/// </summary>
		/// <param name="name">The name of the attribute to create or alter.</param>
		/// <param name="value">Value to set in string form.</param>
		void SetAttribute(string name, string value);

		/// <summary>
		/// Adds a new attribute node. If an attribute with that name (nodeName) is already present in the element, it is replaced by the new one.
		/// </summary>
		/// <param name="attr">The <see cref="Attr"/> node to add to the attribute list.</param>
		/// <returns>If the newAttr attribute replaces an existing attribute, the replaced Attr node is returned, otherwise null is returned.</returns>
		Attr SetAttributeNode(Attr attr);

		/// <summary> Removes the specified attribute node. </summary>
		/// <param name="attr">The <see cref="Attr"/> node to remove from the attribute list.</param>
		void RemoveAttributeNode(Attr attr);

		/// <summary>
		/// Returns <c>true</c> if the specified attribute exists, otherwise it returns <c>false</c>.
		/// </summary>
		/// <param name="name">The name of the attribute to check.</param>
		bool HasAttribute(string name);

		/// <summary>
		/// Indicates whether this node (if it is an element) has any attributes.
		/// </summary>
		/// <returns><c>true</c> if this node has any attributes, <c>false</c> otherwise.</returns>
		bool HasAttributes();

		/// <summary>
		/// Checks whether a node is a descendant of a given Element or not.
		/// </summary>
		/// <param name="element">The node to search.</param>
		/// <returns><c>True</c> if node found, <c>False</c> otherwise.</returns>
		bool Contains(INode element);
		
		/// <summary>
		/// Returns first descendant element that matches a specified CSS selector(s).
		/// </summary>
		IElement QuerySelector(string query);

		/// <summary>
		/// Returns all descendant elements that matches a specified CSS selector(s).
		/// </summary>
		NodeList QuerySelectorAll(string query);
	}
}
