// Decompiled with JetBrains decompiler
// Type: FistVR.GP25
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class GP25 : AttachableFirearm
  {
    [Header("GP25")]
    public FVRFireArmChamber Chamber;
    public Transform Trigger;
    public Vector2 TriggerRange;
    public Transform Safety;
    public Vector2 SafetyRange;
    public Transform Ejector;
    public Vector2 EjectorRange = new Vector2(0.0f, 0.005f);
    private bool m_isEjectorForward = true;
    public bool m_safetyEngaged = true;
    public Transform EjectPos;
    public bool UsesChargeUp;
    private float m_ChargeupAmount;
    private FVRPooledAudioSource m_chargeAudio;
    private int DestabilizedShots;

    public override void ProcessInput(FVRViveHand hand, bool fromInterface, FVRInteractiveObject o)
    {
      if ((Object) this.Attachment != (Object) null && o.m_hasTriggeredUpSinceBegin && !this.m_safetyEngaged)
        this.Attachment.SetAnimatedComponent(this.Trigger, Mathf.Lerp(this.TriggerRange.x, this.TriggerRange.y, hand.Input.TriggerFloat), FVRPhysicalObject.InterpStyle.Translate, FVRPhysicalObject.Axis.Z);
      if (this.UsesChargeUp && o.m_hasTriggeredUpSinceBegin)
      {
        if (hand.Input.TriggerPressed && this.Chamber.IsFull)
        {
          if ((Object) this.m_chargeAudio == (Object) null)
            this.m_chargeAudio = this.OverrideFA.PlayAudioAsHandling(this.AudioClipSet.Prefire, this.MuzzlePos.position);
          this.m_ChargeupAmount += Time.deltaTime;
          if ((double) this.m_ChargeupAmount < 1.0)
            return;
          this.m_chargeAudio.Source.Stop();
          this.m_chargeAudio = (FVRPooledAudioSource) null;
          this.Fire(fromInterface);
        }
        else
        {
          if ((double) this.m_ChargeupAmount > 0.0 && (Object) this.m_chargeAudio != (Object) null)
          {
            this.m_chargeAudio.Source.Stop();
            this.m_chargeAudio = (FVRPooledAudioSource) null;
          }
          this.m_ChargeupAmount = 0.0f;
        }
      }
      else
      {
        if (!hand.Input.TriggerDown || !o.m_hasTriggeredUpSinceBegin || this.m_safetyEngaged)
          return;
        this.Fire(fromInterface);
      }
    }

    public void Fire(bool firedFromInterface)
    {
      if (!this.Chamber.Fire())
        return;
      this.FireMuzzleSmoke();
      if (firedFromInterface)
      {
        FVRFireArm fa = !((Object) this.Attachment != (Object) null) ? this.OverrideFA : this.Attachment.curMount.MyObject as FVRFireArm;
        if ((Object) fa != (Object) null)
        {
          fa.Recoil(fa.IsTwoHandStabilized(), fa.IsForegripStabilized(), fa.IsShoulderStabilized(), this.RecoilProfile);
          this.Fire(this.Chamber, this.MuzzlePos, true, fa);
        }
        else
          this.Fire(this.Chamber, this.MuzzlePos, true, (FVRFireArm) null);
      }
      else
      {
        this.Recoil(false, false, false);
        this.Fire(this.Chamber, this.MuzzlePos, true, (FVRFireArm) null);
      }
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

    public void ToggleSafety()
    {
      this.m_safetyEngaged = !this.m_safetyEngaged;
      this.PlayAudioEvent(FirearmAudioEventType.Safety);
      if (!((Object) this.Attachment != (Object) null))
        return;
      if (this.m_safetyEngaged)
        this.Attachment.SetAnimatedComponent(this.Safety, this.SafetyRange.y, FVRPhysicalObject.InterpStyle.Rotation, FVRPhysicalObject.Axis.X);
      else
        this.Attachment.SetAnimatedComponent(this.Safety, this.SafetyRange.x, FVRPhysicalObject.InterpStyle.Rotation, FVRPhysicalObject.Axis.X);
    }

    private void FixedUpdate()
    {
      if ((double) this.m_ChargeupAmount <= 0.0)
        return;
      this.OverrideFA.RootRigidbody.velocity += Random.onUnitSphere * 0.2f * this.m_ChargeupAmount * this.m_ChargeupAmount;
      this.OverrideFA.RootRigidbody.angularVelocity += Random.onUnitSphere * 0.7f * this.m_ChargeupAmount;
    }

    private void Update()
    {
      if (this.m_isEjectorForward)
      {
        if (!this.Chamber.IsFull)
          return;
        FVRFireArm fvrFireArm = !((Object) this.Attachment != (Object) null) ? this.OverrideFA : this.Attachment.curMount.MyObject as FVRFireArm;
        this.m_isEjectorForward = false;
        fvrFireArm.SetAnimatedComponent(this.Ejector, this.EjectorRange.x, FVRPhysicalObject.InterpStyle.Translate, FVRPhysicalObject.Axis.Z);
      }
      else
      {
        if (this.Chamber.IsFull)
          return;
        FVRFireArm fvrFireArm = !((Object) this.Attachment != (Object) null) ? this.OverrideFA : this.Attachment.curMount.MyObject as FVRFireArm;
        this.m_isEjectorForward = true;
        fvrFireArm.SetAnimatedComponent(this.Ejector, this.EjectorRange.y, FVRPhysicalObject.InterpStyle.Translate, FVRPhysicalObject.Axis.Z);
      }
    }
  }
}
