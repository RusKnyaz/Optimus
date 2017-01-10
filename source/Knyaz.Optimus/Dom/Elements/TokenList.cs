using System;
using System.Collections;
using System.Collections.Generic;

namespace Knyaz.Optimus.Dom.Elements
{
	/// <summary>
	/// https://www.w3.org/TR/dom/#domtokenlist
	/// </summary>
	public interface ITokenList : ICollection<string>
	{
		bool Toggle(string token);
		bool Toggle(string token, bool force);
	}


	internal class TokenList : ITokenList
	{
		readonly List<string> _internalList;

		public event Action Changed;

		public TokenList()
		{
			_internalList = new List<string>();
		}

		public IEnumerator<string> GetEnumerator()
		{
			return _internalList.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable) _internalList).GetEnumerator();
		}

		public void Add(string item)
		{
			if (!_internalList.Contains(item))
			{
				_internalList.Add(item);
				Changed.Fire();
			}
		}

		public void Clear()
		{
			_internalList.Clear();
		}

		public bool Contains(string item)
		{
			return _internalList.Contains(item);
		}

		public void CopyTo(string[] array, int arrayIndex)
		{
			_internalList.CopyTo(array, arrayIndex);
		}

		public bool Remove(string item)
		{
			var res = _internalList.Remove(item);
			if(res)
				Changed.Fire();

			return res;
		}

		public int Count
		{
			get { return _internalList.Count; }
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public string this[int index]
		{
			get { return _internalList[index]; }
			set {  }
		}

		public bool Toggle(string token)
		{
			if (_internalList.Contains(token))
			{
				_internalList.Remove(token);
				Changed.Fire();
				return false;
			}
			 
			_internalList.Add(token);
			Changed.Fire();
			return true;
		}

		public bool Toggle(string token, bool force)
		{
			if (_internalList.Contains(token))
			{
				if (!force)
				{
					_internalList.Remove(token);
					Changed.Fire();
					return false;
				}
				return true;
			}
			else
			{
				if (force)
				{
					_internalList.Add(token);
					Changed.Fire();
					return true;
				}
				return false;
			}
		}

		internal void Init(IEnumerable<string> tokens)
		{
			_internalList.Clear();
			_internalList.AddRange(tokens);
		}
	}
}