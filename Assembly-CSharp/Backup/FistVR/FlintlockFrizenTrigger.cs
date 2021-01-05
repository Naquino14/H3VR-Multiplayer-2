// Decompiled with JetBrains decompiler
// Type: FistVR.FlintlockFrizenTrigger
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class FlintlockFrizenTrigger : FVRInteractiveObject
  {
    public FlintlockFlashPan FlashPan;
    public Transform DistPoint;

    public override bool IsInteractable() => this.FlashPan.GetWeapon().HammerState != FlintlockWeapon.HState.Uncocked;

    public override void SimpleInteraction(FVRViveHand hand)
    {
      this.FlashPan.ToggleFrizenState();
      base.SimpleInteraction(hand);
    }

    protected override void FVRUpdate()
    {
      base.FVRUpdate();
      if (!this.FlashPan.GetWeapon().IsHeld)
        return;
      FVRViveHand otherHand = this.FlashPan.GetWeapon().m_hand.OtherHand;
      if ((double) Vector3.Distance(this.DistPoint.position, this.GetClosestValidPoint(otherHand.Input.Pos, otherHand.PalmTransform.position, this.DistPoint.position)) >= 0.0450000017881393)
        return;
      Vector3 vector3 = this.FlashPan.GetWeapon().transform.InverseTransformVector(otherHand.Input.VelLinearWorld);
      if (this.FlashPan.FrizenState == FlintlockFlashPan.FState.Up)
      {
        if ((double) vector3.z >= -0.5)
          return;
        this.FlashPan.ToggleFrizenState();
      }
      else
      {
        if ((double) vector3.z <= 0.5)
          return;
        this.FlashPan.ToggleFrizenState();
      }
    }
  }
}
