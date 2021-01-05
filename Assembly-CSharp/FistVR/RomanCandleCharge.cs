// Decompiled with JetBrains decompiler
// Type: FistVR.RomanCandleCharge
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class RomanCandleCharge : MonoBehaviour
  {
    public ParticleSystem[] PSystems;
    public GameObject ImpactEffect;
    private bool hasSploded;
    public float VelocityMin;
    public float VelocityMax;
    public Vector3 velocity;
    public float MinFuseTime;
    public float MaxFuseTime;
    private float m_fuse;
    public LayerMask CollisionLayerMask;
    private RaycastHit m_hit;
    public bool isNewType;
    public GameObject[] ExplosionEffects;

    private void Explode()
    {
      if (this.hasSploded)
        return;
      this.hasSploded = true;
      for (int index = 0; index < this.PSystems.Length; ++index)
      {
        ParticleSystem.EmissionModule emission = this.PSystems[index].emission;
        ParticleSystem.MinMaxCurve rate = emission.rate;
        rate.mode = ParticleSystemCurveMode.Constant;
        rate.constantMax = 0.0f;
        rate.constantMin = 0.0f;
        emission.rate = rate;
        this.PSystems[index].transform.SetParent((Transform) null);
        this.PSystems[index].gameObject.AddComponent<KillAfter>().DieTime = 10f;
      }
      Object.Instantiate<GameObject>(this.ExplosionEffects[Random.Range(0, this.ExplosionEffects.Length)], this.transform.position, this.transform.rotation);
      Object.Destroy((Object) this.gameObject);
    }

    public void Fire()
    {
      this.velocity = this.transform.forward * Random.Range(this.VelocityMin, this.VelocityMax);
      this.m_fuse = Random.Range(this.MinFuseTime, this.MaxFuseTime);
    }

    public void Update()
    {
      this.m_fuse -= Time.deltaTime;
      if ((double) this.m_fuse <= 0.0)
        this.Explode();
      this.velocity += Physics.gravity * Time.deltaTime;
      this.transform.LookAt(this.transform.position + this.velocity * Time.deltaTime + this.transform.forward * Mathf.Epsilon);
      float magnitude = this.velocity.magnitude;
      if (Physics.Raycast(this.transform.position, this.transform.forward, out this.m_hit, magnitude * Time.deltaTime, (int) this.CollisionLayerMask, QueryTriggerInteraction.Collide))
      {
        if (!float.IsNaN(this.m_hit.point.x))
          this.transform.position = this.m_hit.point;
        this.velocity = Vector3.Reflect(this.velocity, this.m_hit.normal);
        this.m_fuse -= 0.5f;
        if (!(this.isNewType = false))
          return;
        Object.Instantiate<GameObject>(this.ImpactEffect, this.m_hit.point, Quaternion.LookRotation(this.m_hit.normal));
      }
      else
        this.transform.position += this.transform.forward * magnitude * Time.deltaTime;
    }
  }
}
