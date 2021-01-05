// Decompiled with JetBrains decompiler
// Type: Valve.VR.SteamVR_Input_BindingFile_Source
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;

namespace Valve.VR
{
  [Serializable]
  public class SteamVR_Input_BindingFile_Source
  {
    public string path;
    public string mode;
    public SteamVR_Input_BindingFile_Source_Input_StringDictionary parameters = new SteamVR_Input_BindingFile_Source_Input_StringDictionary();
    public SteamVR_Input_BindingFile_Source_Input inputs = new SteamVR_Input_BindingFile_Source_Input();
    protected const string outputKeyName = "output";

    public string GetOutput()
    {
      foreach (KeyValuePair<string, SteamVR_Input_BindingFile_Source_Input_StringDictionary> input in (Dictionary<string, SteamVR_Input_BindingFile_Source_Input_StringDictionary>) this.inputs)
      {
        foreach (KeyValuePair<string, string> keyValuePair in (Dictionary<string, string>) input.Value)
        {
          if (keyValuePair.Key == "output")
            return keyValuePair.Value;
        }
      }
      return (string) null;
    }

    public override bool Equals(object obj)
    {
      if (!(obj is SteamVR_Input_BindingFile_Source))
        return base.Equals(obj);
      SteamVR_Input_BindingFile_Source bindingFileSource = (SteamVR_Input_BindingFile_Source) obj;
      if (bindingFileSource.mode == this.mode && bindingFileSource.path == this.path)
      {
        bool flag1 = false;
        if (this.parameters != null && bindingFileSource.parameters != null)
        {
          if (this.parameters.Equals((object) bindingFileSource.parameters))
            flag1 = true;
        }
        else if (this.parameters == null && bindingFileSource.parameters == null)
          flag1 = true;
        if (flag1)
        {
          bool flag2 = false;
          if (this.inputs != null && bindingFileSource.inputs != null)
          {
            if (this.inputs.Equals((object) bindingFileSource.inputs))
              flag2 = true;
          }
          else if (this.inputs == null && bindingFileSource.inputs == null)
            flag2 = true;
          return flag2;
        }
      }
      return false;
    }

    public override int GetHashCode() => base.GetHashCode();
  }
}
