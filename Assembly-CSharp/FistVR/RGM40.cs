// Decompiled with JetBrains decompiler
// Type: FistVR.RGM40
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class RGM40 : FVRFireArm
  {
    [Header("RGM Stuff")]
    public FVRFireArmChamber Chamber;
    public Transform Trigger;
    public Vector2 TriggerRange;
    public Transform Ejector;
    public Vector2 EjectorRange = new Vector2(0.0f, 0.005f);
    private bool m_isEjectorForward = true;
    public Transform EjectPos;

    public override void UpdateInteraction(FVRViveHand hand)
    {
      if (this.m_hasTriggeredUpSinceBegin)
      {
        this.SetAnimatedComponent(this.Trigger, Mathf.Lerp(this.TriggerRange.x, this.TriggerRange.y, hand.Input.TriggerFloat), FVRPhysicalObject.InterpStyle.Translate, FVRPhysicalObject.Axis.Z);
        if (hand.Input.TriggerDown)
          this.Fire();
      }
      base.UpdateInteraction(hand);
    }

    public void Fire()
    {
      if (!this.Chamber.Fire())
        return;
      this.Fire(this.Chamber, this.GetMuzzle(), true);
      this.FireMuzzleSmoke();
      this.Recoil(this.IsTwoHandStabilized(), (Object) this.AltGrip != (Object) null, this.IsShoulderStabilized());
      this.PlayAudioGunShot(this.Chamber.GetRound(), GM.CurrentPlayerBody.GetCurrentSoundEnvironment());
      if (!this.Chamber.GetRound().IsCaseless)
        return;
      this.Chamber.SetRound((FVRFireArmRound) null);
    }

    public void SafeEject()
    {
      this.Chamber.EjectRound(this.EjectPos.position, this.EjectPos.forward, Vector3.zero, true);
      this.PlayAudioEvent(FirearmAudioEventType.MagazineEjectRound);
    }

    protected override void FVRUpdate()
    {
      base.FVRUpdate();
      if (this.m_isEjectorForward)
      {
        if (!this.Chamber.IsFull)
          return;
        this.m_isEjectorForward = false;
        this.SetAnimatedComponent(this.Ejector, this.EjectorRange.x, FVRPhysicalObject.InterpStyle.Translate, FVRPhysicalObject.Axis.Z);
      }
      else
      {
        if (this.Chamber.IsFull)
          return;
        this.m_isEjectorForward = true;
        this.SetAnimatedComponent(this.Ejector, this.EjectorRange.y, FVRPhysicalObject.InterpStyle.Translate, FVRPhysicalObject.Axis.Z);
      }
    }
  }
}
