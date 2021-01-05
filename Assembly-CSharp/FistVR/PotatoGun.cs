// Decompiled with JetBrains decompiler
// Type: FistVR.PotatoGun
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class PotatoGun : FVRFireArm
  {
    [Header("PatootGun Config")]
    public FVRFireArmChamber Chamber;
    public Transform Trigger;
    public Vector2 TriggerRange;
    private float m_chamberGas;
    private bool m_hasTriggerReset;
    private float m_triggerfloat;
    public ParticleSystem PSystem_Sparks;
    public ParticleSystem PSystem_Backblast;

    public override void UpdateInteraction(FVRViveHand hand)
    {
      base.UpdateInteraction(hand);
      if (!this.IsAltHeld && hand.Input.TriggerDown && (this.m_hasTriggeredUpSinceBegin && this.m_hasTriggerReset))
      {
        this.PlayAudioEvent(FirearmAudioEventType.HammerHit);
        this.Spark();
      }
      float num = !this.m_hasTriggeredUpSinceBegin ? 0.0f : hand.Input.TriggerFloat;
      if ((double) num != (double) this.m_triggerfloat)
      {
        this.m_triggerfloat = num;
        this.SetAnimatedComponent(this.Trigger, Mathf.Lerp(this.TriggerRange.x, this.TriggerRange.y, this.m_triggerfloat), FVRPhysicalObject.InterpStyle.Translate, FVRPhysicalObject.Axis.Z);
      }
      if ((double) num >= 0.400000005960464 || this.m_hasTriggerReset)
        return;
      this.m_hasTriggerReset = true;
      this.PlayAudioEvent(FirearmAudioEventType.TriggerReset);
    }

    public override void EndInteraction(FVRViveHand hand)
    {
      base.EndInteraction(hand);
      this.m_triggerfloat = 0.0f;
      this.SetAnimatedComponent(this.Trigger, Mathf.Lerp(this.TriggerRange.x, this.TriggerRange.y, 0.0f), FVRPhysicalObject.InterpStyle.Translate, FVRPhysicalObject.Axis.Z);
    }

    public void InsertGas(float f)
    {
      if ((Object) this.Magazine != (Object) null)
        return;
      this.m_chamberGas += f;
      this.m_chamberGas = Mathf.Clamp(this.m_chamberGas, 0.0f, 1.5f);
    }

    private void Spark()
    {
      this.PSystem_Sparks.Emit(5);
      this.m_hasTriggerReset = false;
      if ((double) this.m_chamberGas <= 0.0)
        return;
      if ((Object) this.Magazine == (Object) null)
      {
        this.PSystem_Backblast.Emit(Mathf.RoundToInt(this.m_chamberGas * 10f));
        this.m_chamberGas = 0.0f;
        this.PlayAudioGunShot(false, FVRTailSoundClass.Launcher, FVRTailSoundClass.SuppressedSmall, GM.CurrentPlayerBody.GetCurrentSoundEnvironment());
        this.FireMuzzleSmoke();
      }
      else
        this.Fire();
    }

    public void Fire()
    {
      if (this.Chamber.IsFull && !this.Chamber.IsSpent)
      {
        this.Chamber.Fire();
        this.Fire(this.Chamber, this.GetMuzzle(), true, this.m_chamberGas);
        this.FireMuzzleSmoke();
        this.Recoil(this.IsTwoHandStabilized(), (Object) this.AltGrip != (Object) null, this.IsShoulderStabilized());
        this.PlayAudioGunShot(true, this.Chamber.GetRound().TailClass, this.Chamber.GetRound().TailClassSuppressed, GM.CurrentPlayerBody.GetCurrentSoundEnvironment());
        if (GM.CurrentSceneSettings.IsAmmoInfinite || GM.CurrentPlayerBody.IsInfiniteAmmo)
        {
          this.Chamber.IsSpent = false;
          this.Chamber.UpdateProxyDisplay();
        }
        else if (this.Chamber.GetRound().IsCaseless)
          this.Chamber.SetRound((FVRFireArmRound) null);
      }
      else
      {
        this.PlayAudioGunShot(false, FVRTailSoundClass.Launcher, FVRTailSoundClass.SuppressedSmall, GM.CurrentPlayerBody.GetCurrentSoundEnvironment());
        this.FireMuzzleSmoke();
      }
      this.m_chamberGas = 0.0f;
    }
  }
}
