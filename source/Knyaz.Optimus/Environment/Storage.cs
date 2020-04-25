using System.Collections.Specialized;

namespace Knyaz.Optimus.Environment
{
	/// <summary>
	/// Storage allows to example add, modify or delete stored data items.
	/// </summary>
	public class Storage
	{
		readonly NameValueCollection _collection = new NameValueCollection();
		
		/// <summary>
		/// Returns an integer representing the number of data items stored in the Storage object.
		/// </summary>
		public int Length => _collection.Count;

		/// <summary>
		/// Return the name of the key in the storage by specified index.
		/// </summary>
		/// <param name="index">Index of key to return.</param>
		public string Key(int index) => _collection.Keys[index];

		/// <summary>
		/// When passed a key name, will return that key's value.
		/// </summary>
		public string GetItem(string key) => _collection[key];

		/// <summary>
		/// When passed a key name and value, will add that key to the storage, or update that key's value if it already exists.
		/// </summary>
		public void SetItem(string keyName, string value) => _collection[keyName] = value;
			
		/// <summary>
		/// When passed a key name, will remove that key from the storage.
		/// </summary>
		/// <param name="key"></param>
		public void RemoveItem(string key) => _collection.Remove(key);

		/// <summary>
		/// When invoked, will empty all keys out of the storage.
		/// </summary>
		public void Clear() => _collection.Clear();

		/// <summary>
		/// Gets or sets storage item.
		/// </summary>
		public string this[string key]
		{
			get => _collection[key];
			set => _collection[key] = value;
		}
	}
}