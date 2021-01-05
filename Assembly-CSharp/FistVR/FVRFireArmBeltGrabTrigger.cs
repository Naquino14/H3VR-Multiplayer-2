// Decompiled with JetBrains decompiler
// Type: FistVR.FVRFireArmBeltGrabTrigger
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class FVRFireArmBeltGrabTrigger : FVRInteractiveObject
  {
    public FVRFireArmMagazine Mag;

    public override bool IsInteractable() => !((Object) this.Mag.FireArm == (Object) null) && !this.Mag.FireArm.HasBelt && this.Mag.HasARound() && base.IsInteractable();

    public override void BeginInteraction(FVRViveHand hand)
    {
      base.BeginInteraction(hand);
      if (!((Object) this.Mag.FireArm != (Object) null))
        return;
      this.Mag.FireArm.BeltDD.BeltGrabbed(this.Mag, this.m_hand);
    }

    protected override void FVRUpdate()
    {
      base.FVRUpdate();
      if (!this.IsHeld || !((Object) this.Mag.FireArm != (Object) null))
        return;
      this.Mag.FireArm.BeltDD.BeltGrabUpdate(this.Mag, this.m_hand);
    }

    public override void EndInteraction(FVRViveHand hand)
    {
      if ((Object) this.Mag.FireArm != (Object) null)
        this.Mag.FireArm.BeltDD.BeltReleased(this.Mag, this.m_hand);
      this.Mag.UpdateBulletDisplay();
      base.EndInteraction(hand);
    }
  }
}
