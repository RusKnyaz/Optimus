using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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

	/// <summary>
	/// Tokens list with lazy evaluation.
	/// </summary>
	internal class TokenList : ITokenList
	{
		readonly List<string> _internalList;

		public event Action Changed;

		List<string> InternalList
		{
			get
			{
				var curCn = _classNameFn();
				if (curCn != _className)
				{
					_className = curCn;
					var tokens = _className.Split(' ').Where(x => !string.IsNullOrEmpty(x));
					_internalList.Clear();
					_internalList.AddRange(tokens);
				}
				return _internalList;
			}
		}

		string _className;
		public TokenList(Func<string> classNameFn)
		{
			_internalList = new List<string>();
			_classNameFn = classNameFn;
		}

		public IEnumerator<string> GetEnumerator() => InternalList.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)InternalList).GetEnumerator();

		public void Add(string item)
		{
			if (!InternalList.Contains(item))
			{
				InternalList.Add(item);
				OnChanged();
			}
		}

		private void OnChanged()
		{
			Changed?.Invoke();
			//prevent token list recreation if it is a source of changes.
			_className = _classNameFn();
		}

		public void Clear() => InternalList.Clear();

		public bool Contains(string item) => InternalList.Contains(item);

		public void CopyTo(string[] array, int arrayIndex) => InternalList.CopyTo(array, arrayIndex);

		public bool Remove(string item)
		{
			var res = InternalList.Remove(item);
			if (res)
				OnChanged();

			return res;
		}

		public int Count => InternalList.Count;

		public bool IsReadOnly => false;

		private Func<string> _classNameFn;

		public string this[int index]
		{
			get { return InternalList[index]; }
			set {  }
		}

		public bool Toggle(string token)
		{
			var list = InternalList;
			if (list.Contains(token))
			{
				list.Remove(token);
				OnChanged();
				return false;
			}

			list.Add(token);
			OnChanged();
			return true;
		}

		public bool Toggle(string token, bool force)
		{
			var list = InternalList;
			if (list.Contains(token))
			{
				if (!force)
				{
					list.Remove(token);
					OnChanged();
					return false;
				}
				return true;
			}
			else
			{
				if (force)
				{
					list.Add(token);
					OnChanged();
					return true;
				}
				return false;
			}
		}
	}
}