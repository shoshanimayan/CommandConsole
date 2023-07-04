using System.Collections.Generic;

namespace Console.Utils
{
	/// <summary>
	/// A generic class for managing a Dictionary that stores a collection of values in each key
	/// </summary>
	[System.Serializable]
	public class HashsetDictionary<TKey, TValueType>
	{
		private Dictionary<TKey, HashSet<TValueType>> _dictionary;
		public HashsetDictionary() 
		{
			_dictionary = new Dictionary<TKey, HashSet<TValueType>>();
		}
		public IReadOnlyCollection<TValueType> Values
		{
			get
			{
				List<TValueType> values = new List<TValueType>();
				foreach(HashSet<TValueType> hashSet in _dictionary.Values)
				{
					foreach(TValueType value in hashSet)
					{
						values.Add(value);
					}
				}
				return values.ToArray();
			}
		}
		public IReadOnlyCollection<TKey> Keys => _dictionary.Keys;
		public void EnsureKey(TKey key)
		{
			if(!_dictionary.ContainsKey(key))
			{
				_dictionary.Add(key, new HashSet<TValueType>());
			}
		}
		public void Add(TKey key, TValueType value)
		{
			if (_dictionary.TryGetValue(key, out HashSet<TValueType> values))
			{
				values.Add(value);
			}
			else
			{
				HashSet<TValueType> newValueSet = new HashSet<TValueType>();
				newValueSet.Add(value);
				_dictionary.Add(key, newValueSet);
			}
		}
		/// <summary>
		/// Adds <paramref name="value"/> to <paramref name="key"/> without checking to make sure the key exists
		/// </summary>
		public void Add_CertainOfKey(TKey key, TValueType value)
		{
			try
			{
				_dictionary[key].Add(value);
			}
			catch (KeyNotFoundException)
			{
				throw new KeyNotFoundException($"The key '{key}' is not present in the _dictionary");
			}
		}
		public void Remove(TKey key, TValueType value)
		{
			if (_dictionary.TryGetValue(key, out HashSet<TValueType> values))
			{
				//if (values.Contains(value))
					values.Remove(value);
			}
		}
		public void Remove_CertainOfKey (TKey key, TValueType value)
		{
			try
			{
				_dictionary[key].Remove(value);
			}
			catch (KeyNotFoundException)
			{
				throw new KeyNotFoundException($"Either the key '{key}' or the value '{value}' is not present in the _dictionary");
			}
			
		}
		/// <summary>
		/// Removes all instances of the value 'value' in the _dictionary
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		public void RemoveAllOfValueFromAllKeys(TValueType value)
		{
			foreach(KeyValuePair<TKey, HashSet<TValueType>> kvp in _dictionary)
			{
				kvp.Value.Remove(value);
			}
		}
		public void ClearKey(TKey key)
		{
			if (_dictionary.ContainsKey(key)) { _dictionary[key].Clear(); }
		}
		public void DestroyKey (TKey key)
        {
			_dictionary.Remove(key);
        }
		public void Clear ()
		{
			_dictionary.Clear();
		}
		public void Clear_KeepKeys ()
		{
			foreach(HashSet<TValueType> hashset in _dictionary.Values)
			{
				hashset.Clear();
			}
		}
		public IReadOnlyCollection<TValueType> Get_CertainOfKey (TKey key)
		{
			return _dictionary[key];
		}
		public IReadOnlyCollection<TValueType> Get (TKey key)
		{
			if (_dictionary.TryGetValue(key, out HashSet<TValueType> values))
			{
				return values;
			}
			else
			{
				return new TValueType[0];
			}
		}
		public bool Contains (TKey key, TValueType value)
		{
			if (_dictionary.TryGetValue(key, out HashSet<TValueType> values))
			{
				return values.Contains(value);
			}
			return false;
		}

		public bool Contains_CertainOfKey (TKey key, TValueType value)
		{
			return _dictionary[key].Contains(value);
		}

		public override string ToString()
		{
			string str = $"Hashset _dictionary<{typeof(TKey).Name}, {typeof(TValueType).Name}>:\n";
			foreach(TKey key in _dictionary.Keys)
			{
				str += $"\t{key.ToString()}:\n";
				foreach (TValueType value in _dictionary[key])
				{
					str += $"\t\t{value.ToString()}\n";
				}
			}
			return str;
		}
	}
}