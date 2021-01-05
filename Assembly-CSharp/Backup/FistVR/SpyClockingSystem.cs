// Decompiled with JetBrains decompiler
// Type: FistVR.SpyClockingSystem
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class SpyClockingSystem : SosigWearable
  {
    private void Update()
    {
      if (!((Object) this.S != (Object) null) || this.S.BodyState != Sosig.SosigBodyState.InControl || (this.S.CurrentOrder == Sosig.SosigOrder.Skirmish || this.S.CurrentOrder == Sosig.SosigOrder.Disabled))
        return;
      this.S.BuffHealing_Invis(0.1f);
    }
  }
}
