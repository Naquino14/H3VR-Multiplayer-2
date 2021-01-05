// Decompiled with JetBrains decompiler
// Type: FistVR.RotrwCharcoalGrillGrate
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class RotrwCharcoalGrillGrate : FVRPhysicalObject
  {
    public RotrwCharcoalGrill Grill;
    public bool m_isMountedOnGrill;
    public AudioEvent AudEvent_MountDemount;

    public override bool IsInteractable() => this.Grill.CanPickupGrate();

    public override bool IsDistantGrabbable() => !this.m_isMountedOnGrill;

    protected override void FVRFixedUpdate()
    {
      base.FVRFixedUpdate();
      if (!this.IsHeld || (double) Vector3.Distance(this.transform.position, this.Grill.GrillGrateSpot.position) >= 0.200000002980232 || !this.Grill.CanPickupGrate())
        return;
      FVRViveHand hand = this.m_hand;
      this.EndInteraction(hand);
      hand.ForceSetInteractable((FVRInteractiveObject) null);
      this.MountGrate();
      this.Grill.MountGrate();
    }

    private void MountGrate()
    {
      this.transform.position = this.Grill.GrillGrateSpot.position;
      this.transform.rotation = this.Grill.GrillGrateSpot.rotation;
      this.m_isMountedOnGrill = true;
      this.RootRigidbody.isKinematic = true;
      SM.PlayGenericSound(this.AudEvent_MountDemount, this.transform.position);
    }

    public override void BeginInteraction(FVRViveHand hand)
    {
      base.BeginInteraction(hand);
      this.RootRigidbody.isKinematic = false;
      if (!this.m_isMountedOnGrill)
        return;
      SM.PlayGenericSound(this.AudEvent_MountDemount, this.transform.position);
      this.m_isMountedOnGrill = false;
      this.Grill.DemountGrate();
    }
  }
}
