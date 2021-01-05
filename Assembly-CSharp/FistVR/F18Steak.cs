// Decompiled with JetBrains decompiler
// Type: FistVR.F18Steak
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class F18Steak : MonoBehaviour, IFVRDamageable
  {
    private Vector3 TargetPoint;
    private Vector3 m_velocity;
    private Vector3 m_forwardDir;
    private Vector3 m_upDir;
    private float m_thrustCapability = 100f;
    private float m_brakingCapability = 40f;
    private float targetPointRange = 2000f;
    private Vector2 targetPointHeightRange = new Vector2(300f, 500f);
    public List<GameObject> SpawnOnExplode;
    private Vector3 m_lastPos;
    private Vector3 m_newPos;
    private bool m_isDestroyed;
    public Rigidbody RB;
    private float m_timeTilNewPoint = 10f;
    private float velClamp = 100f;
    private Vector3 upcur = Vector3.up;

    public void Start()
    {
      this.m_lastPos = this.transform.position;
      this.m_newPos = this.transform.position;
      this.InitiateFlight(this.GenerateNewPoint());
    }

    public void Damage(FistVR.Damage d) => this.Explode();

    private Vector3 GenerateNewPoint()
    {
      Vector3 onUnitSphere = Random.onUnitSphere;
      onUnitSphere.y = 0.0f;
      Vector3 vector3 = onUnitSphere * Random.Range(this.targetPointRange * 0.8f, this.targetPointRange);
      vector3.y = Random.Range(this.targetPointHeightRange.x, this.targetPointHeightRange.y);
      this.m_timeTilNewPoint = 10f;
      return vector3;
    }

    public void InitiateFlight(Vector3 tPoint)
    {
      this.TargetPoint = tPoint;
      this.m_forwardDir = (this.TargetPoint - this.transform.position).normalized;
      this.m_upDir = Vector3.up;
      this.m_velocity = this.m_forwardDir * 200f;
    }

    private void Update()
    {
      if ((double) this.m_timeTilNewPoint > 0.0)
        this.m_timeTilNewPoint -= Time.deltaTime;
      else
        this.TargetPoint = this.GenerateNewPoint();
      if (!this.m_isDestroyed)
      {
        Vector3 to = this.TargetPoint - this.transform.position;
        this.m_forwardDir = Vector3.RotateTowards(this.m_forwardDir, to.normalized, Time.deltaTime * 0.3f, 1f);
        if ((double) this.m_forwardDir.y < -0.400000005960464)
        {
          this.m_forwardDir.y = -0.4f;
          this.m_forwardDir.Normalize();
        }
        if ((double) this.m_forwardDir.y > 0.400000005960464)
        {
          this.m_forwardDir.y = 0.4f;
          this.m_forwardDir.Normalize();
        }
        Vector3 forwardDir = this.m_forwardDir;
        forwardDir.y = 0.0f;
        Vector3 vector3_1 = to;
        vector3_1.y = 0.0f;
        Vector3 vector3_2 = Vector3.ProjectOnPlane(this.transform.right, Vector3.up);
        vector3_2.Normalize();
        float t = Mathf.Clamp(Vector3.Angle(forwardDir, vector3_1) / 90f, 0.0f, 0.9f);
        this.upcur = (double) Vector3.Angle(vector3_1, vector3_2) > 90.0 ? Vector3.Slerp(Vector3.up, -vector3_2, t) : Vector3.Slerp(Vector3.up, vector3_2, t);
        this.m_upDir = Vector3.Slerp(this.m_upDir, this.upcur, Time.deltaTime);
        this.transform.rotation = Quaternion.LookRotation(this.m_forwardDir, this.m_upDir);
        this.velClamp = Mathf.Lerp(this.velClamp, Mathf.Lerp(120f, 30f, Vector3.Angle(this.m_forwardDir, to) / 180f), Time.deltaTime * 2f);
        this.m_velocity += this.m_forwardDir * Time.deltaTime * this.m_thrustCapability;
        this.m_velocity = Vector3.ClampMagnitude(this.m_velocity, this.velClamp);
        this.RB.AddForce(this.m_velocity, ForceMode.Acceleration);
        if ((double) to.magnitude < 100.0)
          this.TargetPoint = this.GenerateNewPoint();
      }
      Debug.DrawLine(this.transform.position, this.TargetPoint, Color.red);
    }

    private void Explode()
    {
      if (this.m_isDestroyed)
        return;
      this.m_isDestroyed = true;
      for (int index = 0; index < this.SpawnOnExplode.Count; ++index)
        Object.Instantiate<GameObject>(this.SpawnOnExplode[index], this.transform.position, this.transform.rotation);
      Object.Destroy((Object) this.gameObject);
    }
  }
}
