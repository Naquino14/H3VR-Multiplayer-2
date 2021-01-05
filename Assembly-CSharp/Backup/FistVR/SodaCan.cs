// Decompiled with JetBrains decompiler
// Type: FistVR.SodaCan
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class SodaCan : FVRPhysicalObject, IFVRDamageable
  {
    [Header("Sodacan Params")]
    public PMat Pmaterial;
    public GameObject SprayPSystemPrefab;
    private List<ParticleSystem> SpraySystems = new List<ParticleSystem>();
    public GameObject RupturePSystemPrefab;
    public GameObject ExplosionPSystemPrefab;
    private GameObject m_currentDisplayGo;
    public GameObject Can_Undamaged;
    public GameObject Can_Crumpled;
    public GameObject Can_RupturedTop;
    public GameObject Can_RupturedBottom;
    public GameObject Can_RupturedSideTop;
    public GameObject Can_RupturedSideBottom;
    public GameObject Can_RupturedSideGlance;
    public GameObject Can_RupturedSideCenter;
    private bool m_hasSploded;
    private bool m_hasRuptured;
    private bool m_isSpraying;
    private float m_sodaPressure = 1f;
    public float MaxSodaForce = 20f;
    public float MaxSprayForce = 1f;

    protected override void Awake()
    {
      base.Awake();
      this.m_currentDisplayGo = this.Can_Undamaged;
    }

    public void Damage(FistVR.Damage dam)
    {
      if (!this.m_hasSploded && !this.m_hasRuptured && (double) dam.Dam_TotalKinetic > 400.0)
      {
        this.m_hasSploded = true;
        if ((double) this.m_sodaPressure <= 0.949999988079071)
          ;
        this.Explode(dam.strikeDir);
      }
      else if (!this.m_hasSploded && !this.m_hasRuptured && ((double) dam.Dam_TotalKinetic > (double) Random.Range(200f, 400f) && (double) this.m_sodaPressure > 0.5))
      {
        this.m_hasRuptured = true;
        this.Rupture(dam.hitNormal, dam.point, dam.strikeDir);
      }
      else if (!this.m_hasSploded && !this.m_hasRuptured && ((double) dam.Dam_TotalKinetic >= 50.0 && (double) this.m_sodaPressure > 0.100000001490116))
      {
        this.SetDisplayGo(this.Can_Crumpled);
        this.CreateSpewer(-dam.strikeDir, dam.point);
      }
      this.RootRigidbody.AddForceAtPosition(dam.strikeDir * dam.Dam_TotalKinetic * 0.01f, dam.point, ForceMode.Impulse);
    }

    private void Explode(Vector3 hitNormal)
    {
      if ((double) this.m_sodaPressure > 0.0)
      {
        this.RootRigidbody.AddForceAtPosition(Vector3.up * this.m_sodaPressure * this.MaxSodaForce, this.transform.position, ForceMode.Impulse);
        Object.Instantiate<GameObject>(this.ExplosionPSystemPrefab, this.transform.position, Random.rotation);
      }
      this.m_sodaPressure = 0.0f;
      if ((double) Vector3.Dot(hitNormal, this.transform.up) > 0.5 || (double) Vector3.Dot(hitNormal, this.transform.up) >= -0.5)
        ;
      if (this.IsHeld)
        this.m_hand.EndInteractionIfHeld((FVRInteractiveObject) this);
      Object.Destroy((Object) this.gameObject);
    }

    private void Rupture(Vector3 hitNormal, Vector3 hitPoint, Vector3 force)
    {
      this.TurnSpewersOff();
      Debug.Log((object) hitNormal);
      if ((double) this.m_sodaPressure > 0.0)
      {
        this.RootRigidbody.AddForceAtPosition(force.normalized * this.m_sodaPressure * this.MaxSodaForce, hitPoint, ForceMode.Impulse);
        Object.Instantiate<GameObject>(this.RupturePSystemPrefab, hitPoint, Quaternion.LookRotation(-hitNormal, Random.onUnitSphere));
      }
      this.m_sodaPressure = 0.0f;
      if ((double) Vector3.Dot(hitNormal.normalized, this.transform.up) > 0.5)
        this.SetDisplayGo(this.Can_RupturedBottom);
      else if ((double) Vector3.Dot(hitNormal.normalized, -this.transform.up) > 0.5)
      {
        this.SetDisplayGo(this.Can_RupturedTop);
      }
      else
      {
        Vector3 normalized = (hitPoint - this.transform.position).normalized;
        if ((double) Vector3.Dot(normalized, this.transform.up) > 0.300000011920929)
          this.SetDisplayGo(this.Can_RupturedSideTop);
        else if ((double) Vector3.Dot(normalized, -this.transform.up) > 0.300000011920929)
        {
          this.SetDisplayGo(this.Can_RupturedSideBottom);
        }
        else
        {
          Debug.Log((object) Vector3.Dot(-hitNormal, force));
          if ((double) Vector3.Dot(-hitNormal, force) > 0.5)
            this.SetDisplayGo(this.Can_RupturedSideCenter);
          else
            this.SetDisplayGo(this.Can_RupturedSideGlance);
        }
      }
    }

    private void CreateSpewer(Vector3 facingDir, Vector3 pos)
    {
      GameObject gameObject = Object.Instantiate<GameObject>(this.SprayPSystemPrefab, pos, Quaternion.LookRotation(facingDir, Random.onUnitSphere));
      this.SpraySystems.Add(gameObject.GetComponent<ParticleSystem>());
      gameObject.transform.SetParent(this.transform);
      gameObject.GetComponent<AudioSource>().Play();
      this.m_isSpraying = true;
    }

    private void TurnSpewersOff()
    {
      this.m_isSpraying = false;
      for (int index = 0; index < this.SpraySystems.Count; ++index)
      {
        ParticleSystem.EmissionModule emission = this.SpraySystems[index].emission;
        ParticleSystem.MinMaxCurve rate = emission.rate;
        rate.mode = ParticleSystemCurveMode.Constant;
        rate.constantMax = 0.0f;
        rate.constantMin = 0.0f;
        emission.rate = rate;
        this.SpraySystems[index].gameObject.GetComponent<AudioSource>().Stop();
      }
    }

    private void SetDisplayGo(GameObject go)
    {
      this.m_currentDisplayGo.SetActive(false);
      this.m_currentDisplayGo = go;
      this.m_currentDisplayGo.SetActive(true);
    }

    protected override void FVRFixedUpdate()
    {
      base.FVRFixedUpdate();
      if (this.m_isSpraying)
      {
        this.RootRigidbody.mass = Mathf.Lerp(0.05f, 0.4f, this.m_sodaPressure * this.m_sodaPressure);
        this.m_sodaPressure -= Time.deltaTime * 0.1f * (float) this.SpraySystems.Count;
        if ((double) this.m_sodaPressure <= 0.0)
        {
          this.m_isSpraying = false;
          this.TurnSpewersOff();
        }
      }
      if (this.SpraySystems.Count <= 0 || !this.m_isSpraying)
        return;
      for (int index = 0; index < this.SpraySystems.Count; ++index)
      {
        ParticleSystem.EmissionModule emission = this.SpraySystems[index].emission;
        ParticleSystem.MinMaxCurve rate = emission.rate;
        rate.mode = ParticleSystemCurveMode.Constant;
        rate.constantMax = 15f * this.m_sodaPressure;
        rate.constantMin = 15f * this.m_sodaPressure;
        emission.rate = rate;
        this.SpraySystems[index].gameObject.GetComponent<AudioSource>().volume = this.m_sodaPressure;
        this.RootRigidbody.AddForceAtPosition(this.m_sodaPressure * this.MaxSprayForce * this.SpraySystems[index].transform.forward, this.SpraySystems[index].transform.position);
      }
    }
  }
}
