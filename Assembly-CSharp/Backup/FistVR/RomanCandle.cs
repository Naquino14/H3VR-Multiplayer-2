// Decompiled with JetBrains decompiler
// Type: FistVR.RomanCandle
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class RomanCandle : FVRPhysicalObject
  {
    public GameObject PEffects;
    public ParticleSystem[] PSystems;
    public Transform Muzzle;
    private bool m_isBurning;
    private bool m_isDone;
    public GameObject[] ChargePrefabs;
    public float MinDelay = 1f;
    public float MaxDelay = 1f;
    private float m_TimeTilFire = 1f;
    private int m_chargeIndex;
    private AudioSource m_aud;
    public GameObject Fuse;
    public GameObject BurningSound;

    protected override void Awake()
    {
      base.Awake();
      this.m_aud = this.GetComponent<AudioSource>();
      this.RootRigidbody.maxAngularVelocity = 20f;
    }

    protected override void FVRUpdate()
    {
      base.FVRUpdate();
      if (!this.m_isBurning || this.m_isDone)
        return;
      this.m_TimeTilFire -= Time.deltaTime;
      if ((double) this.m_TimeTilFire > 0.0)
        return;
      this.m_TimeTilFire = Random.Range(this.MinDelay, this.MaxDelay);
      this.FireNextCharge();
    }

    private void FireNextCharge()
    {
      if (this.m_chargeIndex < this.ChargePrefabs.Length)
      {
        this.m_aud.pitch = Random.Range(1f, 1.25f);
        this.m_aud.PlayOneShot(this.m_aud.clip, Random.Range(0.15f, 0.2f));
        Object.Instantiate<GameObject>(this.ChargePrefabs[this.m_chargeIndex], this.Muzzle.position, Quaternion.Slerp(this.Muzzle.rotation, Random.rotation, 0.1f)).GetComponent<RomanCandleCharge>().Fire();
        if (this.IsHeld)
          this.m_hand.Buzz(this.m_hand.Buzzer.Buzz_GunShot);
      }
      else
        this.Extinguish();
      ++this.m_chargeIndex;
    }

    public void Ignite()
    {
      this.BurningSound.SetActive(true);
      if (!this.m_isBurning && !this.m_isDone)
      {
        this.m_isBurning = true;
        this.PEffects.SetActive(true);
        this.m_TimeTilFire = Random.Range(this.MinDelay, this.MaxDelay);
      }
      Object.Destroy((Object) this.Fuse);
    }

    private void Extinguish()
    {
      this.BurningSound.SetActive(false);
      if (!this.m_isBurning)
        return;
      this.m_isBurning = false;
      this.m_isDone = true;
      this.DisableAllPSystemEmission();
    }

    private void DisableAllPSystemEmission()
    {
      for (int index = 0; index < this.PSystems.Length; ++index)
      {
        ParticleSystem.EmissionModule emission = this.PSystems[index].emission;
        ParticleSystem.MinMaxCurve rate = emission.rate;
        rate.mode = ParticleSystemCurveMode.Constant;
        rate.constantMax = 0.0f;
        rate.constantMin = 0.0f;
        emission.rate = rate;
      }
    }
  }
}
