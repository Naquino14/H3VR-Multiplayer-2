// Decompiled with JetBrains decompiler
// Type: FistVR.MinigunMagRelease
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class MinigunMagRelease : FVRInteractiveObject
  {
    public FVRFireArm Firearm;
    private bool ShouldRelease;
    private FVRViveHand m_handy;

    public override bool IsInteractable() => !((Object) this.Firearm.Magazine == (Object) null);

    public override void BeginInteraction(FVRViveHand hand)
    {
      base.BeginInteraction(hand);
      this.ShouldRelease = true;
      this.m_handy = hand;
    }

    protected override void FVRUpdate()
    {
      base.FVRUpdate();
      if (!this.ShouldRelease || !((Object) this.m_handy != (Object) null))
        return;
      if ((Object) this.Firearm.Magazine != (Object) null)
      {
        this.EndInteraction(this.m_handy);
        FVRFireArmMagazine magazine = this.Firearm.Magazine;
        this.Firearm.EjectMag();
        this.m_handy.ForceSetInteractable((FVRInteractiveObject) magazine);
        magazine.BeginInteraction(this.m_handy);
      }
      this.m_handy = (FVRViveHand) null;
      this.ShouldRelease = false;
    }

    public override void UpdateInteraction(FVRViveHand hand) => base.UpdateInteraction(hand);
  }
}
