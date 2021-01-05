// Decompiled with JetBrains decompiler
// Type: FistVR.ClosedBoltMagEjectionTrigger
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class ClosedBoltMagEjectionTrigger : FVRInteractiveObject
  {
    public ClosedBoltWeapon Receiver;

    public override bool IsInteractable() => !((Object) this.Receiver.Magazine == (Object) null);

    public override void BeginInteraction(FVRViveHand hand)
    {
      base.BeginInteraction(hand);
      if (!((Object) this.Receiver.Magazine != (Object) null))
        return;
      this.EndInteraction(hand);
      FVRFireArmMagazine magazine = this.Receiver.Magazine;
      this.Receiver.ReleaseMag();
      hand.ForceSetInteractable((FVRInteractiveObject) magazine);
      magazine.BeginInteraction(hand);
    }

    public override void UpdateInteraction(FVRViveHand hand)
    {
      base.UpdateInteraction(hand);
      if (!hand.Input.TouchpadDown || !((Object) this.Receiver.Magazine != (Object) null))
        return;
      this.EndInteraction(hand);
      FVRFireArmMagazine magazine = this.Receiver.Magazine;
      this.Receiver.ReleaseMag();
      hand.ForceSetInteractable((FVRInteractiveObject) magazine);
      magazine.BeginInteraction(hand);
    }
  }
}
