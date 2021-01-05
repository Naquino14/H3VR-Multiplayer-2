// Decompiled with JetBrains decompiler
// Type: FistVR.OpenBoltMagReleaseTrigger
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class OpenBoltMagReleaseTrigger : FVRInteractiveObject
  {
    public OpenBoltReceiver Receiver;
    public bool IsBeltBox;

    public override bool IsInteractable() => !((Object) this.Receiver.Magazine == (Object) null) && (!((Object) this.Receiver.BeltDD != (Object) null) || !this.Receiver.BeltDD.isBeltGrabbed()) && (this.Receiver.Magazine.IsBeltBox == this.IsBeltBox && (!this.Receiver.ConnectedToBox || this.Receiver.Magazine.CanBeTornOut));

    public override void BeginInteraction(FVRViveHand hand)
    {
      base.BeginInteraction(hand);
      if (!((Object) this.Receiver.Magazine != (Object) null))
        return;
      this.EndInteraction(hand);
      FVRFireArmMagazine magazine = this.Receiver.Magazine;
      if (this.Receiver.Magazine.IsBeltBox && this.Receiver.ConnectedToBox && this.Receiver.Magazine.CanBeTornOut)
      {
        this.Receiver.BeltDD.ForceRelease();
        this.Receiver.Magazine.UpdateBulletDisplay();
      }
      this.Receiver.ReleaseMag();
      hand.ForceSetInteractable((FVRInteractiveObject) magazine);
      magazine.BeginInteraction(hand);
    }
  }
}
