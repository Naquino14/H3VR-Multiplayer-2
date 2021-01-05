// Decompiled with JetBrains decompiler
// Type: Valve.VR.SteamVR_Input_ActionFile_LocalizationItem
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace Valve.VR
{
  public class SteamVR_Input_ActionFile_LocalizationItem
  {
    public const string languageTagKeyName = "language_tag";
    public string language;
    public Dictionary<string, string> items = new Dictionary<string, string>();

    public SteamVR_Input_ActionFile_LocalizationItem(string newLanguage) => this.language = newLanguage;

    public SteamVR_Input_ActionFile_LocalizationItem(Dictionary<string, string> dictionary)
    {
      if (dictionary == null)
        return;
      if (dictionary.ContainsKey("language_tag"))
        this.language = dictionary["language_tag"];
      else
        Debug.Log((object) "<b>[SteamVR]</b> Input: Error in actions file, no language_tag in localization array item.");
      foreach (KeyValuePair<string, string> keyValuePair in dictionary)
      {
        if (keyValuePair.Key != "language_tag")
          this.items.Add(keyValuePair.Key, keyValuePair.Value);
      }
    }
  }
}
