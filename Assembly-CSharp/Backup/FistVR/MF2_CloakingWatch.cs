// Decompiled with JetBrains decompiler
// Type: FistVR.MF2_CloakingWatch
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class MF2_CloakingWatch : FVRPhysicalObject
  {
    [Header("Reticle Display")]
    public Renderer HealthRingRend;
    [Header("Cloaking VFX")]
    public Renderer WatchRend;
    public GameObject ShortingOutFX;
    public MF2_CloakingWatch.WatchState State;
    private float m_cloakingLerp;
    private float m_shortOutCooldown = 10f;
    private float m_cloakingEnergy = 10f;
    [Header("Cloaking Audio")]
    public AudioEvent AudEvent_Cloak;
    public AudioEvent AudEvent_Uncloak;

    private void ShortOut()
    {
      this.State = MF2_CloakingWatch.WatchState.ShortedOut;
      this.m_shortOutCooldown = 10f;
      this.ShortingOutFX.SetActive(true);
      this.m_cloakingEnergy = 0.0f;
      GM.CurrentPlayerBody.DeActivatePowerIfActive(PowerupType.Ghosted);
      this.m_cloakingLerp = 0.0f;
      this.UpdateCloakEffect();
    }

    private void PulseCloaking()
    {
      if (this.IsHeld)
        GM.CurrentPlayerBody.ActivatePower(PowerupType.Ghosted, PowerUpIntensity.High, PowerUpDuration.Blip, false, false, 1.05f);
      this.m_cloakingEnergy -= Time.deltaTime;
      if ((double) this.m_cloakingEnergy > 0.0)
        return;
      this.m_cloakingLerp = 1f;
      this.State = MF2_CloakingWatch.WatchState.Uncloaking;
      SM.PlayCoreSound(FVRPooledAudioType.GenericClose, this.AudEvent_Uncloak, this.transform.position);
    }

    private void UpdatePowerDial() => this.HealthRingRend.material.SetTextureOffset("_MainTex", new Vector2(0.0f, -Mathf.Clamp(this.m_cloakingEnergy / 10f, 0.0f, 1f) + 0.5f));

    private void UpdateCloakEffect() => this.WatchRend.material.SetFloat("_DissolveCutoff", this.m_cloakingLerp);

    private void ShortOutCheck()
    {
      if (this.State != MF2_CloakingWatch.WatchState.Cloaked || !this.IsHeld || !((Object) this.m_hand.OtherHand.CurrentInteractable != (Object) null))
        return;
      this.ShortOut();
    }

    protected override void FVRUpdate()
    {
      base.FVRUpdate();
      this.UpdatePowerDial();
      this.ShortOutCheck();
      switch (this.State)
      {
        case MF2_CloakingWatch.WatchState.Inactive:
          if ((double) this.m_cloakingEnergy < 10.0)
            this.m_cloakingEnergy += Time.deltaTime;
          this.ShortOutCheck();
          break;
        case MF2_CloakingWatch.WatchState.Cloaking:
          this.m_cloakingLerp += Time.deltaTime;
          if ((double) this.m_cloakingLerp >= 1.0)
          {
            this.m_cloakingLerp = 1f;
            this.State = MF2_CloakingWatch.WatchState.Cloaked;
          }
          this.UpdateCloakEffect();
          break;
        case MF2_CloakingWatch.WatchState.Cloaked:
          this.PulseCloaking();
          break;
        case MF2_CloakingWatch.WatchState.Uncloaking:
          this.m_cloakingLerp -= Time.deltaTime;
          if ((double) this.m_cloakingLerp <= 0.0)
          {
            this.m_cloakingLerp = 0.0f;
            this.State = MF2_CloakingWatch.WatchState.Inactive;
          }
          this.UpdateCloakEffect();
          break;
        case MF2_CloakingWatch.WatchState.ShortedOut:
          this.m_shortOutCooldown -= Time.deltaTime;
          if ((double) this.m_shortOutCooldown > 0.0)
            break;
          this.State = MF2_CloakingWatch.WatchState.Inactive;
          this.ShortingOutFX.SetActive(false);
          break;
      }
    }

    public override void UpdateInteraction(FVRViveHand hand)
    {
      base.UpdateInteraction(hand);
      if (!this.m_hasTriggeredUpSinceBegin || !hand.Input.TriggerDown)
        return;
      if (this.State == MF2_CloakingWatch.WatchState.Inactive)
      {
        SM.PlayCoreSound(FVRPooledAudioType.GenericClose, this.AudEvent_Cloak, this.transform.position);
        this.State = MF2_CloakingWatch.WatchState.Cloaking;
      }
      else
      {
        if (this.State != MF2_CloakingWatch.WatchState.Cloaked)
          return;
        SM.PlayCoreSound(FVRPooledAudioType.GenericClose, this.AudEvent_Uncloak, this.transform.position);
        this.State = MF2_CloakingWatch.WatchState.Uncloaking;
      }
    }

    public enum WatchState
    {
      Inactive,
      Cloaking,
      Cloaked,
      Uncloaking,
      ShortedOut,
    }
  }
}
