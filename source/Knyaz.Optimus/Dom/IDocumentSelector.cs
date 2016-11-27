using System.Collections.Generic;
using Knyaz.Optimus.Dom.Elements;

namespace Knyaz.Optimus.Dom
{
	interface IElementSelector
	{
		IElement QuerySelector(string query);
		IReadOnlyList<IElement> QuerySelectorAll(string query);
	}
}
