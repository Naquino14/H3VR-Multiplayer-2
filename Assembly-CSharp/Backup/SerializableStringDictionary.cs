// Decompiled with JetBrains decompiler
// Type: SerializableStringDictionary
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SerializableStringDictionary : ISerializationCallbackReceiver
{
  public Dictionary<string, string> dictionary;
  public string[] _keys;
  public string[] _values;

  public SerializableStringDictionary(Dictionary<string, string> dictionary) => this.dictionary = dictionary;

  public static implicit operator SerializableStringDictionary(
    Dictionary<string, string> dictionary)
  {
    return new SerializableStringDictionary(dictionary);
  }

  void ISerializationCallbackReceiver.OnBeforeSerialize()
  {
    if (this.dictionary == null)
      return;
    int count = this.dictionary.Count;
    if (this._keys == null || this._keys.Length != count)
      this._keys = new string[count];
    this.dictionary.Keys.CopyTo(this._keys, 0);
    if (this._values == null || this._values.Length != count)
      this._values = new string[count];
    this.dictionary.Values.CopyTo(this._values, 0);
  }

  void ISerializationCallbackReceiver.OnAfterDeserialize()
  {
    if (this._keys == null || this._values == null)
      return;
    if (this.dictionary == null)
      this.dictionary = new Dictionary<string, string>();
    this.dictionary.Clear();
    int num = Mathf.Min(this._keys.Length, this._values.Length);
    for (int index = 0; index < num; ++index)
      this.dictionary.Add(this._keys[index], this._values[index]);
  }
}
