// Decompiled with JetBrains decompiler
// Type: FistVR.MedicRadiusHeal
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class MedicRadiusHeal : SosigWearable
  {
    [Header("Healing Functionality")]
    public LayerMask HealDetect;
    public float HealRange = 10f;
    private float m_HealTick = 1f;
    [Header("FX")]
    public ParticleSystem PSystem_Health;
    [Header("Audio Functionality")]
    public AudioEvent AudEvent_HealingPulse;
    public AudioEvent AudEvent_Uber;
    private float m_uberCharge;
    private bool m_isUbering;
    private bool m_healedAggroLastTick;

    public override void Start()
    {
      base.Start();
      this.m_HealTick = Random.Range(0.5f, 1f);
    }

    private void Update()
    {
      if ((Object) this.S == (Object) null)
        return;
      this.m_HealTick -= Time.deltaTime;
      if ((double) this.m_HealTick <= 0.0)
      {
        this.m_HealTick = 1f;
        if (this.S.BodyState == Sosig.SosigBodyState.InControl && this.S.CurrentOrder != Sosig.SosigOrder.Disabled)
          this.HealPulse();
      }
      if (this.m_isUbering || (double) this.m_uberCharge < 40.0 || this.S.CurrentOrder != Sosig.SosigOrder.Skirmish && !this.m_healedAggroLastTick)
        return;
      SM.PlayCoreSound(FVRPooledAudioType.Generic, this.AudEvent_Uber, this.transform.position);
      this.m_isUbering = true;
    }

    private void HealPulse()
    {
      bool flag = false;
      this.m_healedAggroLastTick = false;
      Collider[] colliderArray = Physics.OverlapSphere(this.transform.position, this.HealRange, (int) this.HealDetect, QueryTriggerInteraction.Collide);
      for (int index = 0; index < colliderArray.Length; ++index)
      {
        if (!((Object) colliderArray[index].attachedRigidbody == (Object) null))
        {
          SosigLink component = colliderArray[index].attachedRigidbody.gameObject.GetComponent<SosigLink>();
          if ((Object) component != (Object) null && component.S.E.IFFCode == this.S.E.IFFCode)
          {
            if (this.m_isUbering)
            {
              if ((Object) this.S != (Object) component.S)
                component.S.BuffInvuln_Engage(1.01f);
              else
                component.S.BuffInvuln_Engage(1.01f);
            }
            else if ((Object) this.S != (Object) component.S)
            {
              component.S.BuffHealing_Engage(1.01f, 20f);
              flag = true;
              if (this.S.CurrentOrder == Sosig.SosigOrder.Skirmish)
                this.m_healedAggroLastTick = true;
            }
            else
            {
              component.S.BuffHealing_Engage(1.01f, 3f);
              flag = true;
            }
          }
        }
      }
      if (this.S.E.IFFCode == GM.CurrentPlayerBody.GetPlayerIFF() && (double) Vector3.Distance(this.transform.position, GM.CurrentPlayerBody.transform.position) <= (double) this.HealRange)
      {
        if (this.m_isUbering)
        {
          GM.CurrentPlayerBody.ActivatePower(PowerupType.Invincibility, PowerUpIntensity.High, PowerUpDuration.Blip, false, false);
        }
        else
        {
          flag = true;
          GM.CurrentPlayerBody.ActivatePower(PowerupType.Regen, PowerUpIntensity.Medium, PowerUpDuration.Blip, false, false);
        }
      }
      if (flag && (double) Vector3.Distance(this.transform.position, GM.CurrentPlayerBody.transform.position) < 20.0)
      {
        SM.PlayGenericSound(this.AudEvent_HealingPulse, this.transform.position);
        this.PSystem_Health.Emit(5);
      }
      if (flag && !this.m_isUbering)
        ++this.m_uberCharge;
      if (!this.m_isUbering)
        return;
      this.m_uberCharge -= 5f;
      if ((double) this.m_uberCharge > 0.0)
        return;
      this.m_isUbering = false;
    }
  }
}
