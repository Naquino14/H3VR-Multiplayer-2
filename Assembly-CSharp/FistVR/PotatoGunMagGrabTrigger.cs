﻿// Decompiled with JetBrains decompiler
// Type: FistVR.PotatoGunMagGrabTrigger
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class PotatoGunMagGrabTrigger : FVRInteractiveObject
  {
    public PotatoGun Patoot;

    public override bool IsInteractable() => !((Object) this.Patoot.Magazine == (Object) null);

    public override void BeginInteraction(FVRViveHand hand)
    {
      base.BeginInteraction(hand);
      if (!((Object) this.Patoot.Magazine != (Object) null))
        return;
      this.EndInteraction(hand);
      FVRFireArmMagazine magazine = this.Patoot.Magazine;
      this.Patoot.EjectMag();
      hand.ForceSetInteractable((FVRInteractiveObject) magazine);
      magazine.BeginInteraction(hand);
    }
  }
}