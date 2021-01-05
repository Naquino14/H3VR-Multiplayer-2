// Decompiled with JetBrains decompiler
// Type: FistVR.MF2_RL
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class MF2_RL : FVRFireArm
  {
    [Header("Rocket Launcher Config")]
    public FVRFireArmChamber Chamber;
    private float m_refireLimit;
    public Transform Trigger;
    public Vector2 TriggerRange;

    public override void UpdateInteraction(FVRViveHand hand)
    {
      base.UpdateInteraction(hand);
      if (!this.IsAltHeld && hand.Input.TriggerDown && (this.m_hasTriggeredUpSinceBegin && (double) this.m_refireLimit <= 0.0))
        this.Fire();
      this.SetAnimatedComponent(this.Trigger, Mathf.Lerp(this.TriggerRange.x, this.TriggerRange.y, hand.Input.TriggerFloat), FVRPhysicalObject.InterpStyle.Rotation, FVRPhysicalObject.Axis.X);
    }

    public override void EndInteraction(FVRViveHand hand)
    {
      this.SetAnimatedComponent(this.Trigger, this.TriggerRange.x, FVRPhysicalObject.InterpStyle.Rotation, FVRPhysicalObject.Axis.X);
      base.EndInteraction(hand);
    }

    protected override void FVRUpdate()
    {
      if ((double) this.m_refireLimit > 0.0)
        this.m_refireLimit -= Time.deltaTime;
      if (!this.Chamber.IsFull && this.Magazine.HasARound())
        this.Chamber.SetRound(this.Magazine.RemoveRound(false).GetComponent<FVRFireArmRound>());
      base.FVRUpdate();
    }

    public void Fire()
    {
      this.m_refireLimit = 0.8f;
      if (!this.Chamber.IsFull || this.Chamber.IsSpent)
        return;
      this.Chamber.Fire();
      this.Fire(this.Chamber, this.GetMuzzle(), true);
      if (this.Chamber.GetRound().IsHighPressure)
        this.FireMuzzleSmoke();
      else
        this.m_refireLimit = 0.25f;
      bool twoHandStabilized = this.IsTwoHandStabilized();
      bool foregripStabilized = (Object) this.AltGrip != (Object) null;
      bool shoulderStabilized = this.IsShoulderStabilized();
      if (this.Chamber.GetRound().IsHighPressure)
        this.Recoil(twoHandStabilized, foregripStabilized, shoulderStabilized);
      else
        this.Recoil(twoHandStabilized, foregripStabilized, shoulderStabilized, this.RecoilProfile, 0.2f);
      this.PlayAudioGunShot(this.Chamber.GetRound(), GM.CurrentPlayerBody.GetCurrentSoundEnvironment());
      this.Chamber.SetRound((FVRFireArmRound) null);
    }
  }
}
