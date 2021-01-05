// Decompiled with JetBrains decompiler
// Type: FistVR.FVRFireArmBeltRemovalTrigger
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class FVRFireArmBeltRemovalTrigger : FVRInteractiveObject
  {
    public FVRFireArm FireArm;

    public override bool IsInteractable() => this.FireArm.HasBelt && (!this.FireArm.UsesTopCover || this.FireArm.IsTopCoverUp) && base.IsInteractable();

    public override void BeginInteraction(FVRViveHand hand)
    {
      if (this.FireArm.ConnectedToBox && (Object) this.FireArm.Magazine != (Object) null)
      {
        this.FireArm.HasBelt = false;
        this.FireArm.ConnectedToBox = false;
        hand.ForceSetInteractable((FVRInteractiveObject) this.FireArm.Magazine.BeltGrabTrigger);
        this.FireArm.Magazine.BeltGrabTrigger.BeginInteraction(hand);
      }
      else
      {
        FVRFireArmBeltSegment fireArmBeltSegment = this.FireArm.BeltDD.StripBeltSegment(hand.Input.Pos);
        hand.ForceSetInteractable((FVRInteractiveObject) fireArmBeltSegment);
        fireArmBeltSegment.BeginInteraction(hand);
      }
    }
  }
}
