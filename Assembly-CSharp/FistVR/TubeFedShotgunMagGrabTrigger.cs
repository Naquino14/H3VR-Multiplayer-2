// Decompiled with JetBrains decompiler
// Type: FistVR.TubeFedShotgunMagGrabTrigger
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class TubeFedShotgunMagGrabTrigger : FVRInteractiveObject
  {
    public TubeFedShotgun Shotgun;

    public override bool IsInteractable() => !((Object) this.Shotgun.Magazine == (Object) null) && !this.Shotgun.HasExtractedRound();

    public override void BeginInteraction(FVRViveHand hand)
    {
      base.BeginInteraction(hand);
      if (!((Object) this.Shotgun.Magazine != (Object) null))
        return;
      this.EndInteraction(hand);
      FVRFireArmMagazine magazine = this.Shotgun.Magazine;
      this.Shotgun.EjectMag();
      hand.ForceSetInteractable((FVRInteractiveObject) magazine);
      magazine.BeginInteraction(hand);
    }
  }
}
