// Decompiled with JetBrains decompiler
// Type: FistVR.MF_ZoneModeConfig
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;

namespace FistVR
{
  [Serializable]
  public class MF_ZoneModeConfig
  {
    public MF_GameMode Mode = MF_GameMode.TeamDM;
    public List<MF_PlayAreaConfig> PlayAreaConfigs;

    public MF_PlayAreaConfig GetPlayAreaConfig(MF_PlayArea area)
    {
      for (int index = 0; index < this.PlayAreaConfigs.Count; ++index)
      {
        if (this.PlayAreaConfigs[index].PlayArea == area)
          return this.PlayAreaConfigs[index];
      }
      return (MF_PlayAreaConfig) null;
    }
  }
}
