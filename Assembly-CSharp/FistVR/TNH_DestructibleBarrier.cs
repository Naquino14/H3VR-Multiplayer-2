// Decompiled with JetBrains decompiler
// Type: FistVR.TNH_DestructibleBarrier
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace FistVR
{
  public class TNH_DestructibleBarrier : MonoBehaviour, IFVRDamageable
  {
    public TNH_DestructibleBarrier.State MyState;
    public float Height = 1.1f;
    private Vector3 m_lowerPos;
    private Vector3 m_upperPos;
    private float m_raiseLerp;
    public NavMeshObstacle Obstacle;
    public Rigidbody RB;
    public List<GameObject> Stages;
    public float Damage_PerStage;
    private int m_curStage;
    private float m_damageLeftInStage;
    private bool m_isDestroyed;
    public List<GameObject> SpawnOnDestroy;
    public List<Transform> SpawnOnDestroyPoints;
    private TNH_DestructibleBarrierPoint m_barrierPoint;

    private void Start()
    {
      this.m_curStage = 0;
      this.m_damageLeftInStage = this.Damage_PerStage;
    }

    public void SetBarrierPoint(TNH_DestructibleBarrierPoint b) => this.m_barrierPoint = b;

    public void Lower()
    {
      this.MyState = TNH_DestructibleBarrier.State.Lowering;
      this.Obstacle.enabled = false;
    }

    private void FixedUpdate()
    {
      switch (this.MyState)
      {
        case TNH_DestructibleBarrier.State.Lowering:
          this.m_raiseLerp -= Time.deltaTime;
          this.RB.MovePosition(Vector3.Lerp(this.m_lowerPos, this.m_upperPos, this.m_raiseLerp));
          if ((double) this.m_raiseLerp > 0.0)
            break;
          this.m_raiseLerp = 0.0f;
          this.MyState = TNH_DestructibleBarrier.State.Lowered;
          break;
        case TNH_DestructibleBarrier.State.Raising:
          this.m_raiseLerp += Time.deltaTime;
          this.RB.MovePosition(Vector3.Lerp(this.m_lowerPos, this.m_upperPos, this.m_raiseLerp));
          if ((double) this.m_raiseLerp < 1.0)
            break;
          this.m_raiseLerp = 1f;
          this.MyState = TNH_DestructibleBarrier.State.Raised;
          this.Obstacle.enabled = true;
          break;
      }
    }

    private void Update()
    {
      if (this.MyState != TNH_DestructibleBarrier.State.Lowered)
        return;
      Object.Destroy((Object) this.gameObject);
    }

    public void InitToPlace(Vector3 pos, Vector3 forward)
    {
      this.m_upperPos = pos;
      this.m_lowerPos = pos - Vector3.up * this.Height;
      this.transform.position = this.m_lowerPos;
      this.transform.rotation = Quaternion.LookRotation(forward, Vector3.up);
      this.Obstacle.enabled = false;
      this.MyState = TNH_DestructibleBarrier.State.Raising;
    }

    public void Damage(FistVR.Damage d)
    {
      if (this.MyState != TNH_DestructibleBarrier.State.Raised || this.m_isDestroyed)
        return;
      float damTotalKinetic = d.Dam_TotalKinetic;
      if (d.Class == FistVR.Damage.DamageClass.Explosive)
        damTotalKinetic *= 0.1f;
      else if (d.Class == FistVR.Damage.DamageClass.Melee)
        damTotalKinetic *= 0.01f;
      this.m_damageLeftInStage -= damTotalKinetic;
      if ((double) this.m_damageLeftInStage > 0.0)
        return;
      this.m_damageLeftInStage = this.Damage_PerStage;
      ++this.m_curStage;
      if (this.m_curStage < this.Stages.Count)
      {
        for (int index = 0; index < this.Stages.Count; ++index)
        {
          if (index == this.m_curStage)
            this.Stages[index].SetActive(true);
          else
            this.Stages[index].SetActive(false);
        }
      }
      else
        this.Destroy(d);
    }

    private void Destroy(FistVR.Damage dam)
    {
      if (this.m_isDestroyed)
        return;
      this.m_isDestroyed = true;
      for (int index = 0; index < this.SpawnOnDestroy.Count; ++index)
      {
        GameObject gameObject = Object.Instantiate<GameObject>(this.SpawnOnDestroy[index], this.SpawnOnDestroyPoints[index].position, this.SpawnOnDestroyPoints[index].rotation);
        Rigidbody component = gameObject.GetComponent<Rigidbody>();
        Vector3 force = Vector3.Lerp(gameObject.transform.position - dam.point, dam.strikeDir, 0.5f);
        force = force.normalized * Random.Range(1f, 10f);
        component.AddForceAtPosition(force, dam.point, ForceMode.Impulse);
      }
      this.m_barrierPoint.BarrierDestroyed();
      Object.Destroy((Object) this.gameObject);
    }

    public enum State
    {
      Lowered,
      Lowering,
      Raised,
      Raising,
    }
  }
}
