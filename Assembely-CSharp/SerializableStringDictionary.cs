using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SerializableStringDictionary : ISerializationCallbackReceiver
{
	public Dictionary<string, string> dictionary;

	public string[] _keys;

	public string[] _values;

	public SerializableStringDictionary(Dictionary<string, string> dictionary)
	{
		this.dictionary = dictionary;
	}

	public static implicit operator SerializableStringDictionary(Dictionary<string, string> dictionary)
	{
		return new SerializableStringDictionary(dictionary);
	}

	void ISerializationCallbackReceiver.OnBeforeSerialize()
	{
		if (dictionary != null)
		{
			int count = dictionary.Count;
			if (_keys == null || _keys.Length != count)
			{
				_keys = new string[count];
			}
			dictionary.Keys.CopyTo(_keys, 0);
			if (_values == null || _values.Length != count)
			{
				_values = new string[count];
			}
			dictionary.Values.CopyTo(_values, 0);
		}
	}

	void ISerializationCallbackReceiver.OnAfterDeserialize()
	{
		if (_keys != null && _values != null)
		{
			if (dictionary == null)
			{
				dictionary = new Dictionary<string, string>();
			}
			dictionary.Clear();
			int num = Mathf.Min(_keys.Length, _values.Length);
			for (int i = 0; i < num; i++)
			{
				dictionary.Add(_keys[i], _values[i]);
			}
		}
	}
}
