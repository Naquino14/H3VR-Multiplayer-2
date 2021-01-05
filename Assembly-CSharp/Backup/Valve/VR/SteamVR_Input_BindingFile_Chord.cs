// Decompiled with JetBrains decompiler
// Type: Valve.VR.SteamVR_Input_BindingFile_Chord
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;

namespace Valve.VR
{
  [Serializable]
  public class SteamVR_Input_BindingFile_Chord
  {
    public string output;
    public List<List<string>> inputs = new List<List<string>>();

    public override bool Equals(object obj)
    {
      if (!(obj is SteamVR_Input_BindingFile_Chord))
        return base.Equals(obj);
      SteamVR_Input_BindingFile_Chord bindingFileChord = (SteamVR_Input_BindingFile_Chord) obj;
      if (this.output == bindingFileChord.output && this.inputs != null && (bindingFileChord.inputs != null && this.inputs.Count == bindingFileChord.inputs.Count))
      {
        for (int index1 = 0; index1 < this.inputs.Count; ++index1)
        {
          if (this.inputs[index1] != null && bindingFileChord.inputs[index1] != null && this.inputs[index1].Count == bindingFileChord.inputs[index1].Count)
          {
            for (int index2 = 0; index2 < this.inputs[index1].Count; ++index2)
            {
              if (this.inputs[index1][index2] != bindingFileChord.inputs[index1][index2])
                return false;
            }
            return true;
          }
        }
      }
      return false;
    }

    public override int GetHashCode() => base.GetHashCode();
  }
}
