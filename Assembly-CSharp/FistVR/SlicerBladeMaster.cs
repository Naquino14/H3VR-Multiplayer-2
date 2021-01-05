// Decompiled with JetBrains decompiler
// Type: FistVR.SlicerBladeMaster
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class SlicerBladeMaster : MonoBehaviour
  {
    public Rigidbody BaseRB;
    public SlicerComputer Comp;
    public Transform Blade1;
    public Transform Blade2;
    public Collider[] Blade1Cols;
    public Collider[] Blade2Cols;
    private HashSet<Collider> m_blade1Cols = new HashSet<Collider>();
    private HashSet<Collider> m_blade2Cols = new HashSet<Collider>();
    public Collider[] Blade1Shards;
    public Collider[] Blade2Shards;
    public ParticleSystem PSystem_Sparks;
    public ParticleSystem PSystem_Sparks2;
    private ParticleSystem.EmitParams emitParams;
    public int MaxParticlesPerCollision = 1;
    private int m_myTurn;
    public AudioSource AudSource_SawImpact;
    private float m_sawImpactVolume;
    public AudioSource AudSource_SawCollision;
    private float m_sawCollisionSoundRefire;
    public AudioClip[] AudClip_SawCollision;
    private bool isBlade1Active = true;
    private bool isBlade2Active = true;
    private float m_Blade1Integrity = 2000f;
    private float m_Blade2Integrity = 2000f;
    private int currentBladeCastIndexA;
    private int currentBladeCastIndexB = 1;
    private RaycastHit m_hit;
    public LayerMask BladeLM;
    public LayerMask BladeLMPlayer;

    private void Awake()
    {
      this.emitParams = new ParticleSystem.EmitParams();
      for (int index = 0; index < this.Blade1Cols.Length; ++index)
        this.m_blade1Cols.Add(this.Blade1Cols[index]);
      for (int index = 0; index < this.Blade2Cols.Length; ++index)
        this.m_blade2Cols.Add(this.Blade2Cols[index]);
    }

    private void Update()
    {
      this.m_sawImpactVolume -= Time.deltaTime * 3f;
      if ((double) this.m_sawImpactVolume <= 0.0 && this.AudSource_SawImpact.isPlaying)
        this.AudSource_SawImpact.Stop();
      if ((double) this.m_sawCollisionSoundRefire > 0.0)
        this.m_sawCollisionSoundRefire -= Time.deltaTime;
      if (this.isBlade1Active)
        this.Blade1.localEulerAngles = new Vector3(0.0f, Mathf.Repeat(this.Blade1.localEulerAngles.y + Time.deltaTime * 3600f, 360f), 0.0f);
      if (!this.isBlade2Active)
        return;
      this.Blade2.localEulerAngles = new Vector3(0.0f, Mathf.Repeat(this.Blade2.localEulerAngles.y + Time.deltaTime * 3600f, 360f), 0.0f);
    }

    private void FixedUpdate()
    {
      if (this.isBlade1Active)
        this.BaseRB.AddTorque(-this.Blade1.up * Random.Range(-0.4f, 0.4f));
      if (this.isBlade2Active)
        this.BaseRB.AddTorque(-this.Blade2.up * Random.Range(-0.4f, 0.4f));
      ++this.currentBladeCastIndexA;
      if (this.currentBladeCastIndexA > 7)
        this.currentBladeCastIndexA = 0;
      ++this.currentBladeCastIndexB;
      if (this.currentBladeCastIndexB > 7)
        this.currentBladeCastIndexB = 0;
      if (this.isBlade1Active)
      {
        Vector3 vector3 = this.Blade1Cols[this.currentBladeCastIndexB].transform.position - this.Blade1Cols[this.currentBladeCastIndexA].transform.position;
        if (Physics.Raycast(this.Blade1Cols[this.currentBladeCastIndexA].transform.position, vector3.normalized, out this.m_hit, vector3.magnitude, (int) this.BladeLM, QueryTriggerInteraction.Collide))
        {
          IFVRDamageable component = this.m_hit.collider.transform.gameObject.GetComponent<IFVRDamageable>();
          if (component != null)
            component.Damage(new Damage()
            {
              hitNormal = this.m_hit.normal,
              Dam_Cutting = 600f,
              Dam_TotalKinetic = 600f,
              Class = Damage.DamageClass.Melee,
              point = this.m_hit.point,
              strikeDir = -this.m_hit.normal
            });
        }
      }
      if (!this.isBlade2Active)
        return;
      Vector3 vector3_1 = this.Blade2Cols[this.currentBladeCastIndexB].transform.position - this.Blade2Cols[this.currentBladeCastIndexA].transform.position;
      if (!Physics.Raycast(this.Blade2Cols[this.currentBladeCastIndexA].transform.position, vector3_1.normalized, out this.m_hit, vector3_1.magnitude, (int) this.BladeLM, QueryTriggerInteraction.Collide))
        return;
      IFVRDamageable component1 = this.m_hit.collider.transform.gameObject.GetComponent<IFVRDamageable>();
      if (component1 == null)
        return;
      component1.Damage(new Damage()
      {
        hitNormal = this.m_hit.normal,
        Dam_Cutting = 600f,
        Dam_TotalKinetic = 600f,
        Class = Damage.DamageClass.Melee,
        point = this.m_hit.point,
        strikeDir = -this.m_hit.normal
      });
    }

    private void OnCollisionEnter(Collision col)
    {
      if ((double) this.m_sawCollisionSoundRefire <= 0.0)
      {
        for (int index = 0; index < col.contacts.Length; ++index)
        {
          if (this.isBlade1Active && this.m_blade1Cols.Contains(col.contacts[index].thisCollider) || this.isBlade2Active && this.m_blade2Cols.Contains(col.contacts[index].thisCollider))
          {
            this.AudSource_SawCollision.pitch = Random.Range(0.65f, 1f);
            this.AudSource_SawCollision.PlayOneShot(this.AudClip_SawCollision[Random.Range(0, this.AudClip_SawCollision.Length)], Random.Range(0.6f, 0.9f));
            this.m_sawCollisionSoundRefire = Random.Range(0.25f, 0.6f);
          }
        }
      }
      for (int index = 0; index < col.contacts.Length; ++index)
      {
        float num1 = this.BaseRB.mass * col.relativeVelocity.magnitude;
        if ((Object) col.contacts[index].otherCollider.attachedRigidbody != (Object) null)
          num1 *= col.contacts[index].otherCollider.attachedRigidbody.mass / this.BaseRB.mass;
        float num2 = num1 / Time.fixedDeltaTime;
        float f = 0.0008f;
        float num3 = num2 / f;
        float num4 = num3 / 1000000f;
        float num5 = (float) ((double) Mathf.Pow(f, 0.25f) * (double) num3 / 1000000.0);
        if (this.isBlade1Active && this.m_blade1Cols.Contains(col.contacts[index].thisCollider))
          this.DamageBlade(num5 / (float) col.contacts.Length, 0, col.contacts[index].point);
        else if (this.isBlade2Active && this.m_blade2Cols.Contains(col.contacts[index].thisCollider))
          this.DamageBlade(num5 / (float) col.contacts.Length, 1, col.contacts[index].point);
      }
    }

    public void DamageBlade(float MPaRM, int BladeNum, Vector3 point)
    {
      if (BladeNum == 0)
      {
        this.m_Blade1Integrity -= MPaRM;
        if ((double) this.m_Blade1Integrity > 0.0)
          return;
        this.DestroyBlade(0, point);
      }
      else
      {
        this.m_Blade2Integrity -= MPaRM;
        if ((double) this.m_Blade2Integrity > 0.0)
          return;
        this.DestroyBlade(1, point);
      }
    }

    public void DestroyBlade(int BladeNum, Vector3 point)
    {
      if (BladeNum == 0 && this.isBlade1Active)
      {
        this.isBlade1Active = false;
        this.Blade1.gameObject.SetActive(false);
        for (int index = 0; index < this.Blade1Shards.Length; ++index)
        {
          this.Blade1Shards[index].gameObject.SetActive(true);
          this.Blade1Cols[index].gameObject.SetActive(false);
          this.Blade1Shards[index].transform.SetParent((Transform) null);
          Rigidbody rigidbody = this.Blade1Shards[index].gameObject.AddComponent<Rigidbody>();
          rigidbody.mass = 0.25f;
          rigidbody.AddForceAtPosition(Random.onUnitSphere * Random.Range(1f, 10f), point, ForceMode.Impulse);
        }
        DamageDealt damageDealt = new DamageDealt()
        {
          force = Vector3.up * 0.1f
        };
        damageDealt.hitNormal = damageDealt.force;
        damageDealt.IsInside = true;
        damageDealt.IsInitialContact = true;
        damageDealt.MPa = 50f;
        damageDealt.MPaRootMeter = 10f;
        damageDealt.point = point;
        damageDealt.PointsDamage = 600f;
        damageDealt.ShotOrigin = (Transform) null;
        damageDealt.strikeDir = -damageDealt.force;
        damageDealt.uvCoords = new Vector2(0.5f, 0.5f);
        if (!((Object) this.Comp != (Object) null))
          ;
      }
      else if (BladeNum == 1 && this.isBlade2Active)
      {
        this.isBlade2Active = false;
        this.Blade2.gameObject.SetActive(false);
        for (int index = 0; index < this.Blade2Shards.Length; ++index)
        {
          this.Blade2Shards[index].gameObject.SetActive(true);
          this.Blade2Cols[index].gameObject.SetActive(false);
          this.Blade2Shards[index].transform.SetParent((Transform) null);
          Rigidbody rigidbody = this.Blade2Shards[index].gameObject.AddComponent<Rigidbody>();
          rigidbody.mass = 0.25f;
          rigidbody.AddForceAtPosition(Random.onUnitSphere * Random.Range(1f, 10f), point, ForceMode.Impulse);
        }
        DamageDealt damageDealt = new DamageDealt()
        {
          force = Vector3.up * 0.1f
        };
        damageDealt.hitNormal = damageDealt.force;
        damageDealt.IsInside = true;
        damageDealt.MPa = 50f;
        damageDealt.IsInitialContact = true;
        damageDealt.MPaRootMeter = 10f;
        damageDealt.point = point;
        damageDealt.PointsDamage = 600f;
        damageDealt.ShotOrigin = (Transform) null;
        damageDealt.strikeDir = -damageDealt.force;
        damageDealt.uvCoords = new Vector2(0.5f, 0.5f);
        if (!((Object) this.Comp != (Object) null))
          ;
      }
      if (!this.isBlade1Active && !this.isBlade2Active)
      {
        if (!((Object) this.Comp != (Object) null))
          return;
        this.Comp.SetToughnessPercentageIfHigher(0.2f);
      }
      else
      {
        if (this.isBlade1Active && this.isBlade2Active || !((Object) this.Comp != (Object) null))
          return;
        this.Comp.SetToughnessPercentageIfHigher(0.5f);
      }
    }

    private void OnCollisionStay(Collision col)
    {
      int num = 0;
      for (int index = 0; index < col.contacts.Length; ++index)
      {
        if (num < this.MaxParticlesPerCollision)
        {
          this.m_myTurn = this.m_myTurn != 0 ? 0 : 1;
          if (this.isBlade1Active && this.m_blade1Cols.Contains(col.contacts[index].thisCollider))
          {
            ++num;
            this.emitParams.position = col.contacts[index].point;
            Vector3 vector3 = Vector3.Cross((col.contacts[index].point - this.transform.position).normalized, this.Blade1.up) * 30f + Random.onUnitSphere * 3f;
            this.emitParams.velocity = vector3;
            this.PSystem_Sparks.Emit(this.emitParams, 1);
            this.emitParams.velocity = vector3 * 0.2f + Random.onUnitSphere * 15f;
            this.PSystem_Sparks2.Emit(this.emitParams, 1);
            this.BaseRB.AddForceAtPosition(-vector3, col.contacts[index].point, ForceMode.Force);
            this.BaseRB.AddForceAtPosition(col.contacts[index].normal * 6f, col.contacts[index].point, ForceMode.Force);
            if (this.m_myTurn == 0)
              FXM.InitiateMuzzleFlash(col.contacts[index].point + col.contacts[index].normal * 0.025f, col.contacts[index].normal, Random.Range(0.25f, 2f), Color.white, Random.Range(0.5f, 2f));
            this.m_sawImpactVolume = Random.Range(0.9f, 1f);
            this.AudSource_SawImpact.pitch = Random.Range(0.97f, 1.03f);
            if (!this.AudSource_SawImpact.isPlaying)
            {
              this.AudSource_SawImpact.time = Random.Range(0.0f, 2f);
              this.AudSource_SawImpact.Play();
            }
          }
          if (this.isBlade2Active && this.m_blade2Cols.Contains(col.contacts[index].thisCollider))
          {
            ++num;
            this.emitParams.position = col.contacts[index].point;
            Vector3 vector3 = Vector3.Cross((col.contacts[index].point - this.transform.position).normalized, this.Blade2.up) * 30f + Random.onUnitSphere * 3f;
            this.emitParams.velocity = vector3;
            this.PSystem_Sparks.Emit(this.emitParams, 1);
            this.emitParams.velocity = vector3 * 0.15f + Random.onUnitSphere * 5f;
            this.PSystem_Sparks2.Emit(this.emitParams, 1);
            this.BaseRB.AddForceAtPosition(-vector3, col.contacts[index].point, ForceMode.Force);
            this.BaseRB.AddForceAtPosition(col.contacts[index].normal * 6f, col.contacts[index].point, ForceMode.Force);
            if (this.m_myTurn == 1)
              FXM.InitiateMuzzleFlash(col.contacts[index].point + col.contacts[index].normal * 0.025f, col.contacts[index].normal, Random.Range(0.25f, 2f), Color.white, Random.Range(0.5f, 2f));
            this.m_sawImpactVolume = Random.Range(0.9f, 1f);
            this.AudSource_SawImpact.pitch = Random.Range(0.97f, 1.03f);
            if (!this.AudSource_SawImpact.isPlaying)
            {
              this.AudSource_SawImpact.time = Random.Range(0.0f, 2f);
              this.AudSource_SawImpact.Play();
            }
          }
        }
      }
    }
  }
}
