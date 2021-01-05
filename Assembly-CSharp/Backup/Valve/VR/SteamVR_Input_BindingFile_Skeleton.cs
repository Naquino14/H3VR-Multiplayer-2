// Decompiled with JetBrains decompiler
// Type: Valve.VR.SteamVR_Input_BindingFile_Skeleton
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;

namespace Valve.VR
{
  [Serializable]
  public class SteamVR_Input_BindingFile_Skeleton
  {
    public string output;
    public string path;

    public override bool Equals(object obj)
    {
      if (!(obj is SteamVR_Input_BindingFile_Skeleton))
        return base.Equals(obj);
      SteamVR_Input_BindingFile_Skeleton bindingFileSkeleton = (SteamVR_Input_BindingFile_Skeleton) obj;
      return bindingFileSkeleton.output == this.output && bindingFileSkeleton.path == this.path;
    }

    public override int GetHashCode() => base.GetHashCode();
  }
}
