using System.Collections.Generic;
using System.Linq;

namespace Knyaz.Optimus.Dom.Css
{
	/// <summary>
	/// Provides an ordered collection of media. An empty list is the same as a list that contains the medium "all".
	/// </summary>
	public class MediaList
	{
		private List<string> _mediums = new List<string>();

		public MediaList(string query)
		{
			MediaText = query;
		}

		/// <summary>
		/// Adds the medium newMedium to the end of the list.
		/// </summary>
		/// <param name="newMedium">The medium to add</param>
		public void AppendMedium(string newMedium)
		{
			_mediums.Add(newMedium);
		}

		/// <summary>
		/// Deletes the medium indicated by oldMedium from the list.
		/// </summary>
		/// <param name="oldMedium"></param>
		public void DeleteMedium(string oldMedium)
		{
			_mediums.Remove(oldMedium);
		}

		/// <summary>
		/// The number of media in the list.
		/// </summary>
		public int Length
		{
			get { return _mediums.Count; }
		}

		/// <summary>
		/// The parsable textual representation of the media list.
		/// </summary>
		public string MediaText
		{
			get { return string.Join(", ", _mediums); }
			set
			{
				_mediums.Clear();
				_mediums.AddRange(value.Split(',').Select(x => x.Trim()));
			}
		}

		/// <summary>
		/// Returns the medium from the list specified by index.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public string this[int index] => _mediums[index];
	}
}
