// Decompiled with JetBrains decompiler
// Type: Valve.VR.SteamVR_Input_BindingFile_Source_Input
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;

namespace Valve.VR
{
  [Serializable]
  public class SteamVR_Input_BindingFile_Source_Input : Dictionary<string, SteamVR_Input_BindingFile_Source_Input_StringDictionary>
  {
    public override bool Equals(object obj)
    {
      if (obj is SteamVR_Input_BindingFile_Source_Input)
      {
        SteamVR_Input_BindingFile_Source_Input bindingFileSourceInput = (SteamVR_Input_BindingFile_Source_Input) obj;
        if (this == bindingFileSourceInput)
          return true;
        if (this.Count == bindingFileSourceInput.Count)
        {
          foreach (KeyValuePair<string, SteamVR_Input_BindingFile_Source_Input_StringDictionary> keyValuePair in (Dictionary<string, SteamVR_Input_BindingFile_Source_Input_StringDictionary>) this)
          {
            if (!bindingFileSourceInput.ContainsKey(keyValuePair.Key) || !this[keyValuePair.Key].Equals((object) bindingFileSourceInput[keyValuePair.Key]))
              return false;
          }
          return true;
        }
      }
      return base.Equals(obj);
    }

    public override int GetHashCode() => base.GetHashCode();
  }
}
