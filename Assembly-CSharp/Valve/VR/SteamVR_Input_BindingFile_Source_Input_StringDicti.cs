// Decompiled with JetBrains decompiler
// Type: Valve.VR.SteamVR_Input_BindingFile_Source_Input_StringDictionary
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Valve.VR
{
  [Serializable]
  public class SteamVR_Input_BindingFile_Source_Input_StringDictionary : Dictionary<string, string>
  {
    public override bool Equals(object obj)
    {
      if (!(obj is SteamVR_Input_BindingFile_Source_Input_StringDictionary))
        return base.Equals(obj);
      SteamVR_Input_BindingFile_Source_Input_StringDictionary stringDictionary = (SteamVR_Input_BindingFile_Source_Input_StringDictionary) obj;
      if (this == stringDictionary)
        return true;
      return this.Count == stringDictionary.Count && !this.Except<KeyValuePair<string, string>>((IEnumerable<KeyValuePair<string, string>>) stringDictionary).Any<KeyValuePair<string, string>>();
    }

    public override int GetHashCode() => base.GetHashCode();
  }
}
