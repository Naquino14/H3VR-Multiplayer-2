// Decompiled with JetBrains decompiler
// Type: Valve.VR.SteamVR_Skeleton_FingerExtensionTypeLists
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Valve.VR
{
  public class SteamVR_Skeleton_FingerExtensionTypeLists
  {
    private SteamVR_Skeleton_FingerExtensionTypes[] _enumList;
    private string[] _stringList;

    public SteamVR_Skeleton_FingerExtensionTypes[] enumList
    {
      get
      {
        if (this._enumList == null)
          this._enumList = (SteamVR_Skeleton_FingerExtensionTypes[]) Enum.GetValues(typeof (SteamVR_Skeleton_FingerExtensionTypes));
        return this._enumList;
      }
    }

    public string[] stringList
    {
      get
      {
        if (this._stringList == null)
          this._stringList = ((IEnumerable<SteamVR_Skeleton_FingerExtensionTypes>) this.enumList).Select<SteamVR_Skeleton_FingerExtensionTypes, string>((Func<SteamVR_Skeleton_FingerExtensionTypes, string>) (element => element.ToString())).ToArray<string>();
        return this._stringList;
      }
    }
  }
}
