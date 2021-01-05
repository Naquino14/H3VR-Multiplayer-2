// Decompiled with JetBrains decompiler
// Type: FistVR.MF2_Medigun
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class MF2_Medigun : FVRPhysicalObject
  {
    [Header("Connections")]
    public MedigunBeam Beam;
    public Transform BeamTarget;
    public MF2_Medigun_Handle Handle;
    public Transform HandleRot_Front;
    public Transform HandleRot_Back;
    public Transform HandleRot_Up;
    public Transform Muzzle;
    [Header("Functionality")]
    public LayerMask LatchingMask;
    public LayerMask BlockingMask;
    private float EngageRange = 10f;
    private float MaxRange = 20f;
    private RaycastHit m_hit;
    private bool m_isBeamEngaged;
    private bool m_hasSosigTarget;
    private Sosig m_targettedSosig;
    private bool m_isUberReady;
    private bool m_isUberActive;
    private float m_uberChargeUp;
    private float m_uberChargeOut = 8f;
    [Header("Audio")]
    public AudioSource AudSource_Loop_Heal;
    public AudioSource AudSource_Loop_Charged;
    public AudioEvent AudEvent_Engage;
    public AudioEvent AudEvent_Disengage;
    public AudioEvent AudEvent_Fail;
    public AudioEvent AudEvent_UberReady;
    public AudioEvent AudEvent_UberEngage;
    public AudioEvent AudEvent_UberDisengage;
    [Header("Materials")]
    public Material[] GunMatsByTeam;
    public Material[] MagMatsByTeam;
    public Material[] GunMatsByTeam_Uber;
    public Material[] MagMatsByTeam_Uber;
    public Renderer[] GunRends;
    public Renderer MagRend;
    public Transform Fore;
    private int storedIFF = -3;
    public Renderer Gauge;

    protected override void Awake()
    {
      base.Awake();
      this.BeamTarget.SetParent((Transform) null);
      this.storedIFF = GM.CurrentPlayerBody.GetPlayerIFF();
      this.Beam.Initialize();
      this.SetBaseMat();
    }

    private void SetBaseMat()
    {
      int i = Mathf.Clamp(this.storedIFF, 0, 2);
      for (int index = 0; index < this.GunRends.Length; ++index)
        this.GunRends[index].material = this.GunMatsByTeam[i];
      this.MagRend.material = this.MagMatsByTeam[i];
      this.Beam.SetLineColor(i);
      this.Beam.SetElectricityColor(i);
      this.Gauge.material = this.GunMatsByTeam_Uber[i];
    }

    private void SetUberMat()
    {
      int index1 = Mathf.Clamp(this.storedIFF, 0, 2);
      for (int index2 = 0; index2 < this.GunRends.Length; ++index2)
        this.GunRends[index2].material = this.GunMatsByTeam_Uber[index1];
      this.MagRend.material = this.MagMatsByTeam_Uber[index1];
      this.Gauge.material = this.GunMatsByTeam_Uber[index1];
    }

    private void TryEngageBeam()
    {
      Collider[] colliderArray = Physics.OverlapCapsule(this.Muzzle.position, this.Muzzle.position + this.Muzzle.forward * this.EngageRange, 2f, (int) this.LatchingMask);
      List<Rigidbody> rigidbodyList = new List<Rigidbody>();
      for (int index = 0; index < colliderArray.Length; ++index)
      {
        if ((Object) colliderArray[index].attachedRigidbody != (Object) null && !rigidbodyList.Contains(colliderArray[index].attachedRigidbody))
          rigidbodyList.Add(colliderArray[index].attachedRigidbody);
      }
      bool flag = false;
      SosigLink sosigLink = (SosigLink) null;
      float num1 = 180f;
      for (int index = 0; index < rigidbodyList.Count; ++index)
      {
        SosigLink component = rigidbodyList[index].GetComponent<SosigLink>();
        if (!((Object) component == (Object) null) && component.S.BodyState != Sosig.SosigBodyState.Dead && (this.storedIFF <= 0 || component.S.E.IFFCode == this.storedIFF || component.S.E.IFFCode < 0))
        {
          float num2 = Vector3.Angle(rigidbodyList[index].transform.transform.position - this.Muzzle.position, this.Muzzle.forward);
          if ((double) num2 < (double) num1)
          {
            num1 = num2;
            sosigLink = component;
            flag = true;
          }
        }
      }
      if (flag)
      {
        Sosig s = sosigLink.S;
        if (!Physics.Linecast(this.Muzzle.position, s.Links[1].transform.position, (int) this.BlockingMask, QueryTriggerInteraction.Ignore))
        {
          this.EngageBeam(s);
          return;
        }
      }
      SM.PlayGenericSound(this.AudEvent_Fail, this.Muzzle.position);
      this.m_isBeamEngaged = false;
    }

    private void EngageBeam(Sosig S)
    {
      SM.PlayGenericSound(this.AudEvent_Engage, this.Muzzle.position);
      this.m_isBeamEngaged = true;
      this.m_hasSosigTarget = true;
      this.m_targettedSosig = S;
      this.BeamTarget.transform.position = this.m_targettedSosig.Links[1].transform.position;
      this.Beam.StartBeam(this.BeamTarget);
    }

    private void DisEngageBeam()
    {
      if (!this.m_isBeamEngaged)
        return;
      SM.PlayGenericSound(this.AudEvent_Disengage, this.Muzzle.position);
      this.m_isBeamEngaged = false;
      this.Beam.StopBeam();
      this.m_hasSosigTarget = false;
      this.m_targettedSosig = (Sosig) null;
    }

    private void EngageUber()
    {
      SM.PlayGenericSound(this.AudEvent_UberEngage, this.Muzzle.position);
      this.m_uberChargeUp = 0.0f;
      this.m_uberChargeOut = 8f;
      this.m_isUberReady = false;
      this.m_isUberActive = true;
      this.SetUberMat();
    }

    private void DisEngageUber()
    {
      SM.PlayGenericSound(this.AudEvent_UberDisengage, this.Muzzle.position);
      this.m_isUberReady = false;
      this.m_isUberActive = false;
      this.m_uberChargeUp = 0.0f;
      this.m_uberChargeOut = 0.0f;
      this.SetBaseMat();
    }

    public void HandleEngage() => this.TryEngageBeam();

    public void HandleDisEngage() => this.DisEngageBeam();

    public override void UpdateInteraction(FVRViveHand hand)
    {
      base.UpdateInteraction(hand);
      if (!this.m_isUberReady || this.m_isUberActive || (!this.m_isBeamEngaged || !hand.Input.TriggerDown) || !this.m_hasTriggeredUpSinceBegin)
        return;
      this.EngageUber();
    }

    protected override void FVRUpdate()
    {
      base.FVRUpdate();
      int playerIff = GM.CurrentPlayerBody.GetPlayerIFF();
      if (this.storedIFF != playerIff)
      {
        this.storedIFF = playerIff;
        if (this.m_isUberActive)
          this.SetUberMat();
        else
          this.SetBaseMat();
      }
      if (this.m_isUberActive)
        this.Gauge.transform.localScale = new Vector3(3.5f, 1f, Mathf.Lerp(0.25f, 8.5f, this.m_uberChargeOut / 8f));
      else
        this.Gauge.transform.localScale = new Vector3(3.5f, 1f, Mathf.Lerp(0.25f, 8.5f, this.m_uberChargeUp / 100f));
      if (this.m_isBeamEngaged)
      {
        if ((Object) this.m_targettedSosig == (Object) null)
          this.DisEngageBeam();
        else if (this.m_targettedSosig.BodyState == Sosig.SosigBodyState.Dead)
        {
          this.DisEngageBeam();
        }
        else
        {
          Transform transform = this.m_targettedSosig.Links[1].transform;
          if (Physics.Linecast(this.Muzzle.position, transform.transform.position, (int) this.BlockingMask, QueryTriggerInteraction.Ignore))
          {
            this.DisEngageBeam();
          }
          else
          {
            this.Beam.target.position = transform.position;
            if ((double) Vector3.Distance(transform.position, this.Muzzle.position) > (double) this.MaxRange)
              this.DisEngageBeam();
          }
        }
      }
      if (this.m_isBeamEngaged)
      {
        if (!this.m_isUberReady && !this.m_isUberActive)
          this.m_uberChargeUp += Time.deltaTime * 5f;
        if ((double) this.m_uberChargeUp >= 100.0)
          this.m_isUberReady = true;
        if (!this.AudSource_Loop_Heal.isPlaying)
          this.AudSource_Loop_Heal.Play();
        this.m_targettedSosig.BuffHealing_Engage(0.1f, 20f);
        this.m_targettedSosig.BuffDamResist_Engage(0.1f, 0.6f);
        if (this.m_isUberActive)
        {
          this.m_targettedSosig.BuffInvuln_Engage(0.1f);
          GM.CurrentPlayerBody.ActivatePower(PowerupType.Regen, PowerUpIntensity.High, PowerUpDuration.Blip, false, false);
          this.m_uberChargeOut -= Time.deltaTime;
          if ((double) this.m_uberChargeOut <= 0.0)
            this.DisEngageUber();
        }
        this.Fore.localEulerAngles = Random.onUnitSphere * 0.4f;
      }
      else
      {
        if (this.AudSource_Loop_Heal.isPlaying)
          this.AudSource_Loop_Heal.Stop();
        this.Fore.localEulerAngles = Vector3.zero;
      }
      if (this.m_isUberReady || this.m_isUberActive)
      {
        this.Beam.Electricity.SetActive(true);
        if (this.AudSource_Loop_Charged.isPlaying)
          return;
        this.AudSource_Loop_Charged.Play();
      }
      else
      {
        this.Beam.Electricity.SetActive(false);
        if (!this.AudSource_Loop_Charged.isPlaying)
          return;
        this.AudSource_Loop_Charged.Stop();
      }
    }
  }
}
