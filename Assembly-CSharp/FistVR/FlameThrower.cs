// Decompiled with JetBrains decompiler
// Type: FistVR.FlameThrower
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class FlameThrower : FVRFireArm
  {
    [Header("FlameThrower Params")]
    public FlameThrowerValve Valve;
    public bool UsesValve = true;
    public MF2_FlamethrowerValve MF2Valve;
    public bool UsesMF2Valve;
    [Header("Trigger Config")]
    public Transform Trigger;
    public float TriggerFiringThreshold = 0.8f;
    public float Trigger_ForwardValue;
    public float Trigger_RearwardValue;
    public FVRPhysicalObject.InterpStyle TriggerInterpStyle = FVRPhysicalObject.InterpStyle.Rotation;
    private float m_triggerFloat;
    [Header("Special Audio Config")]
    public AudioEvent AudEvent_Ignite;
    public AudioEvent AudEvent_Extinguish;
    public AudioSource AudSource_FireLoop;
    private float m_triggerHasBeenHeldFor;
    private bool m_hasFiredStartSound;
    private bool m_isFiring;
    public ParticleSystem FireParticles;
    public Vector2 FireWidthRange;
    public Vector2 SpeedRangeMin;
    public Vector2 SpeedRangeMax;
    public Vector2 SizeRangeMin;
    public Vector2 SizeRangeMax;
    public Vector2 AudioPitchRange = new Vector2(1.5f, 0.5f);
    public float ParticleVolume = 40f;
    public bool UsesPilotLightSystem;
    public bool UsesAirBlastSystem;
    [Header("PilotLight")]
    public Transform PilotLight;
    private bool m_isPilotLightOn;
    public AudioEvent AudEvent_PilotOn;
    [Header("Airblast")]
    public bool UsesAirBlast;
    public Transform AirBlastCenter;
    public GameObject AirBlastGo;
    private float m_airBurstRecovery;

    protected override void Start()
    {
      if (this.UsesPilotLightSystem)
        this.PilotLight.gameObject.SetActive(false);
      base.Start();
    }

    private float GetVLerp()
    {
      if (this.UsesValve)
        return this.Valve.ValvePos;
      return this.UsesMF2Valve ? this.MF2Valve.Lerp : 0.5f;
    }

    protected override void FVRUpdate()
    {
      base.FVRUpdate();
      this.UpdateControls();
      this.UpdateFire();
      if (!this.UsesPilotLightSystem)
        return;
      if ((Object) this.Magazine != (Object) null && (double) this.Magazine.FuelAmountLeft > 0.0)
      {
        if (!this.m_isPilotLightOn)
          this.PilotOn();
      }
      else if (this.m_isPilotLightOn)
        this.PilotOff();
      if (!this.m_isPilotLightOn)
        return;
      this.PilotLight.localScale = Vector3.one + Random.onUnitSphere * 0.05f;
    }

    private void PilotOn()
    {
      this.m_isPilotLightOn = true;
      SM.PlayCoreSound(FVRPooledAudioType.GenericClose, this.AudEvent_PilotOn, this.GetMuzzle().position);
      this.PilotLight.gameObject.SetActive(true);
    }

    private void PilotOff()
    {
      this.m_isPilotLightOn = false;
      this.PilotLight.gameObject.SetActive(false);
    }

    private void AirBlast()
    {
      GameObject gameObject = Object.Instantiate<GameObject>(this.AirBlastGo, this.AirBlastCenter.position, this.AirBlastCenter.rotation);
      gameObject.GetComponent<Explosion>().IFF = GM.CurrentPlayerBody.GetPlayerIFF();
      gameObject.GetComponent<ExplosionSound>().IFF = GM.CurrentPlayerBody.GetPlayerIFF();
    }

    private void UpdateControls()
    {
      if (this.IsHeld)
      {
        this.m_triggerFloat = !this.m_hasTriggeredUpSinceBegin || this.IsAltHeld ? 0.0f : this.m_hand.Input.TriggerFloat;
        if (this.UsesAirBlast && (double) this.m_airBurstRecovery <= 0.0 && this.HasFuel() && (this.m_hand.IsInStreamlinedMode && this.m_hand.Input.BYButtonDown || !this.m_hand.IsInStreamlinedMode && this.m_hand.Input.TouchpadDown))
        {
          this.m_airBurstRecovery = 1f;
          this.AirBlast();
          this.Magazine.DrainFuel(5f);
        }
        if ((double) this.m_airBurstRecovery > 0.0)
          this.m_airBurstRecovery -= Time.deltaTime;
        if ((double) this.m_triggerFloat > 0.200000002980232 && this.HasFuel() && (double) this.m_airBurstRecovery <= 0.0)
        {
          if ((double) this.m_triggerHasBeenHeldFor < 2.0)
            this.m_triggerHasBeenHeldFor += Time.deltaTime;
          this.m_isFiring = true;
          if (!this.m_hasFiredStartSound)
          {
            this.m_hasFiredStartSound = true;
            SM.PlayCoreSound(FVRPooledAudioType.GenericClose, this.AudEvent_Ignite, this.GetMuzzle().position);
          }
          this.AudSource_FireLoop.volume = Mathf.Clamp(this.m_triggerHasBeenHeldFor * 2f, 0.0f, 0.4f);
          this.AudSource_FireLoop.pitch = Mathf.Lerp(this.AudioPitchRange.x, this.AudioPitchRange.y, this.GetVLerp());
          if (!this.AudSource_FireLoop.isPlaying)
            this.AudSource_FireLoop.Play();
          this.Magazine.DrainFuel(Time.deltaTime);
        }
        else
        {
          this.m_triggerHasBeenHeldFor = 0.0f;
          this.StopFiring();
        }
      }
      else
        this.m_triggerFloat = 0.0f;
      if ((double) this.m_triggerFloat > 0.0)
        return;
      this.StopFiring();
    }

    public void UpdateFire()
    {
      ParticleSystem.EmissionModule emission = this.FireParticles.emission;
      ParticleSystem.MinMaxCurve rateOverTime = emission.rateOverTime;
      if (this.m_isFiring)
      {
        rateOverTime.mode = ParticleSystemCurveMode.Constant;
        rateOverTime.constantMax = this.ParticleVolume;
        rateOverTime.constantMin = this.ParticleVolume;
        float vlerp = this.GetVLerp();
        ParticleSystem.MainModule main = this.FireParticles.main;
        ParticleSystem.MinMaxCurve startSpeed = main.startSpeed;
        startSpeed.mode = ParticleSystemCurveMode.TwoConstants;
        startSpeed.constantMax = Mathf.Lerp(this.SpeedRangeMax.x, this.SpeedRangeMax.y, vlerp);
        startSpeed.constantMin = Mathf.Lerp(this.SpeedRangeMin.x, this.SpeedRangeMin.y, vlerp);
        main.startSpeed = startSpeed;
        ParticleSystem.MinMaxCurve startSize = main.startSize;
        startSize.mode = ParticleSystemCurveMode.TwoConstants;
        startSize.constantMax = Mathf.Lerp(this.SizeRangeMax.x, this.SizeRangeMax.y, vlerp);
        startSize.constantMin = Mathf.Lerp(this.SizeRangeMin.x, this.SizeRangeMin.y, vlerp);
        main.startSize = startSize;
        this.FireParticles.shape.angle = Mathf.Lerp(this.FireWidthRange.x, this.FireWidthRange.y, vlerp);
      }
      else
      {
        rateOverTime.mode = ParticleSystemCurveMode.Constant;
        rateOverTime.constantMax = 0.0f;
        rateOverTime.constantMin = 0.0f;
      }
      emission.rateOverTime = rateOverTime;
    }

    private bool HasFuel() => !((Object) this.Magazine == (Object) null) && (double) this.Magazine.FuelAmountLeft > 0.0;

    private void StopFiring()
    {
      if (this.m_isFiring)
      {
        SM.PlayCoreSound(FVRPooledAudioType.GenericClose, this.AudEvent_Extinguish, this.GetMuzzle().position);
        this.AudSource_FireLoop.Stop();
        this.AudSource_FireLoop.volume = 0.0f;
      }
      this.m_isFiring = false;
      this.m_hasFiredStartSound = false;
    }

    public override void UpdateInteraction(FVRViveHand hand)
    {
      base.UpdateInteraction(hand);
      if (!((Object) this.Trigger != (Object) null))
        return;
      if (this.TriggerInterpStyle == FVRPhysicalObject.InterpStyle.Translate)
      {
        this.Trigger.localPosition = new Vector3(0.0f, 0.0f, Mathf.Lerp(this.Trigger_ForwardValue, this.Trigger_RearwardValue, this.m_triggerFloat));
      }
      else
      {
        if (this.TriggerInterpStyle != FVRPhysicalObject.InterpStyle.Rotation)
          return;
        this.Trigger.localEulerAngles = new Vector3(Mathf.Lerp(this.Trigger_ForwardValue, this.Trigger_RearwardValue, this.m_triggerFloat), 0.0f, 0.0f);
      }
    }

    public override void EndInteraction(FVRViveHand hand)
    {
      base.EndInteraction(hand);
      this.m_triggerFloat = 0.0f;
      if (!((Object) this.Trigger != (Object) null))
        return;
      if (this.TriggerInterpStyle == FVRPhysicalObject.InterpStyle.Translate)
      {
        this.Trigger.localPosition = new Vector3(0.0f, 0.0f, Mathf.Lerp(this.Trigger_ForwardValue, this.Trigger_RearwardValue, this.m_triggerFloat));
      }
      else
      {
        if (this.TriggerInterpStyle != FVRPhysicalObject.InterpStyle.Rotation)
          return;
        this.Trigger.localEulerAngles = new Vector3(Mathf.Lerp(this.Trigger_ForwardValue, this.Trigger_RearwardValue, this.m_triggerFloat), 0.0f, 0.0f);
      }
    }
  }
}
