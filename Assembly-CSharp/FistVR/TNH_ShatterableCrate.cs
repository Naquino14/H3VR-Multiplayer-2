// Decompiled with JetBrains decompiler
// Type: FistVR.TNH_ShatterableCrate
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class TNH_ShatterableCrate : MonoBehaviour, IFVRDamageable
  {
    public float DamTilDestroyed = 2500f;
    [Header("SubParts")]
    public List<GameObject> SpawnOnDestruction;
    public List<Transform> SpawnPoints;
    public GameObject ParticlePrefab_Full;
    public GameObject ParticlePrefab_Empty;
    [Header("Sound")]
    public bool SoundOnDamage;
    public AudioEvent AudEvent_SoundOnDamage;
    private bool m_isDestroyed;
    private float m_tickDownTilCanBeDamaged = 5f;
    private float damRefireLimited;
    private TNH_Manager m_m;
    private bool m_isHoldingHealth;
    private bool m_isHoldingToken;
    public GameObject OverrideToken;
    public GameObject HealthToken;

    public void Start()
    {
    }

    public void Update()
    {
      if ((double) this.m_tickDownTilCanBeDamaged > 0.0)
        this.m_tickDownTilCanBeDamaged -= Time.deltaTime;
      if ((double) this.damRefireLimited <= 0.0)
        return;
      this.damRefireLimited -= Time.deltaTime;
    }

    public void SetHoldingHealth(TNH_Manager m)
    {
      this.m_m = m;
      this.m_isHoldingHealth = true;
    }

    public void SetHoldingToken(TNH_Manager m)
    {
      this.m_m = m;
      this.m_isHoldingToken = true;
    }

    public void Damage(FistVR.Damage d)
    {
      if ((double) this.m_tickDownTilCanBeDamaged > 0.0 || this.m_isDestroyed)
        return;
      this.damRefireLimited = 0.05f;
      if ((double) this.damRefireLimited <= 0.0 && this.SoundOnDamage)
        SM.PlayCoreSound(FVRPooledAudioType.GenericLongRange, this.AudEvent_SoundOnDamage, this.transform.position);
      float damTotalKinetic = d.Dam_TotalKinetic;
      if (d.Class == FistVR.Damage.DamageClass.Explosive)
        damTotalKinetic *= 0.1f;
      else if (d.Class == FistVR.Damage.DamageClass.Melee)
        damTotalKinetic *= 2f;
      this.DamTilDestroyed -= damTotalKinetic;
      if ((double) this.DamTilDestroyed >= 0.0)
        return;
      this.Destroy(d);
    }

    private void Destroy(FistVR.Damage dam)
    {
      if (this.m_isDestroyed)
        return;
      this.m_isDestroyed = true;
      for (int index = 0; index < this.SpawnOnDestruction.Count; ++index)
      {
        GameObject gameObject = Object.Instantiate<GameObject>(this.SpawnOnDestruction[index], this.SpawnPoints[index].position, this.SpawnPoints[index].rotation);
        Rigidbody component = gameObject.GetComponent<Rigidbody>();
        Vector3 force = Vector3.Lerp(gameObject.transform.position - dam.point, dam.strikeDir, 0.5f);
        force = force.normalized * Random.Range(1f, 10f);
        component.AddForceAtPosition(force, dam.point, ForceMode.Impulse);
      }
      Vector3 forward = Vector3.up;
      if ((double) dam.strikeDir.magnitude > 0.0)
        forward = dam.strikeDir;
      if (this.m_isHoldingToken || this.m_isHoldingHealth)
        Object.Instantiate<GameObject>(this.ParticlePrefab_Full, this.transform.position, Quaternion.LookRotation(forward));
      else
        Object.Instantiate<GameObject>(this.ParticlePrefab_Empty, this.transform.position, Quaternion.LookRotation(forward));
      if (this.m_isHoldingToken)
        Object.Instantiate<GameObject>(this.OverrideToken, this.transform.position, Quaternion.identity).GetComponent<TNH_Token>().M = this.m_m;
      else if (this.m_isHoldingHealth)
        Object.Instantiate<GameObject>(this.HealthToken, this.transform.position, Quaternion.identity);
      Object.Destroy((Object) this.gameObject);
    }
  }
}
