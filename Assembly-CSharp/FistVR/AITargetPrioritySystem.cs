// Decompiled with JetBrains decompiler
// Type: FistVR.AITargetPrioritySystem
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  [Serializable]
  public class AITargetPrioritySystem
  {
    private AIEntity E;
    private Vector3 m_targetPoint = Vector3.zero;
    public List<AIEvent> RecentEvents = new List<AIEvent>();
    private Dictionary<AIEntity, AIEvent> KnownEntityDic = new Dictionary<AIEntity, AIEvent>();
    private int m_eventCapacity;
    private float m_maxTrackingTime;
    private float m_timeToNoFreshTarget;
    private float m_timeToNoFreshTargetTick;
    private bool m_isDestroyed;
    private bool m_lastFreshTargetAnswer;
    private float m_timeTargetSeen;

    public void DestroySystem() => this.m_isDestroyed = true;

    public bool HasFreshTarget()
    {
      if (this.m_isDestroyed)
        return this.m_lastFreshTargetAnswer;
      if ((double) this.m_timeToNoFreshTargetTick >= (double) this.m_timeToNoFreshTarget)
      {
        this.m_lastFreshTargetAnswer = false;
        return false;
      }
      this.m_lastFreshTargetAnswer = true;
      return true;
    }

    public float GetTimeSinceTopTargetSeen()
    {
      if (this.m_isDestroyed)
        return 0.0f;
      return this.RecentEvents.Count > 0 ? this.RecentEvents[0].TimeSinceSensed : this.m_maxTrackingTime + 0.01f;
    }

    public Vector3 GetTargetPoint() => this.m_isDestroyed ? this.E.transform.position + UnityEngine.Random.onUnitSphere : this.m_targetPoint;

    public float GetTimeTargetSeen() => this.m_isDestroyed ? UnityEngine.Random.Range(0.0f, 10f) : this.m_timeTargetSeen;

    public bool IsTargetEntity() => this.RecentEvents.Count > 0 && this.RecentEvents[0].IsEntity;

    public Vector3 GetTargetGroundPoint() => this.RecentEvents.Count > 0 ? this.RecentEvents[0].Entity.GetGroundPos() : this.GetTargetPoint();

    public float GetAngleToVertical(Transform myFrame)
    {
      if (this.m_isDestroyed)
        return 0.0f;
      Vector3 to = Vector3.ProjectOnPlane(this.m_targetPoint - myFrame.position, myFrame.right);
      return Vector3.Angle(myFrame.forward, to);
    }

    public float GetAngleToHorizontal(Transform myFrame)
    {
      if (this.m_isDestroyed)
        return 0.0f;
      Vector3 to = Vector3.ProjectOnPlane(this.m_targetPoint - myFrame.position, myFrame.up);
      return Vector3.Angle(myFrame.forward, to);
    }

    public float GetDistanceToTarget(Transform myFrame) => this.m_isDestroyed ? 400f : (this.m_targetPoint - myFrame.position).magnitude;

    public void Init(AIEntity e, int capacity, float maxTrackingTime, float noFreshTargetTime)
    {
      this.E = e;
      this.m_eventCapacity = capacity;
      this.m_maxTrackingTime = maxTrackingTime;
      this.m_timeToNoFreshTarget = noFreshTargetTime;
      this.m_timeToNoFreshTargetTick = this.m_timeToNoFreshTarget;
    }

    public void ProcessEvent(AIEvent e)
    {
      if (this.m_isDestroyed)
        return;
      if (e.IsEntity && this.KnownEntityDic.ContainsKey(e.Entity))
      {
        this.KnownEntityDic[e.Entity].UpdateFrom(e);
        e.Dispose();
      }
      else
      {
        if (e.IsEntity)
          this.KnownEntityDic.Add(e.Entity, e);
        this.RecentEvents.Add(e);
      }
    }

    public void Compute()
    {
      if (this.m_isDestroyed)
        return;
      for (int index = this.RecentEvents.Count - 1; index >= 0; --index)
      {
        if (this.RecentEvents[index].IsEntity && ((UnityEngine.Object) this.RecentEvents[index].Entity == (UnityEngine.Object) null || this.RecentEvents[index].Entity.IFFCode < -1 || (double) this.RecentEvents[index].Entity.VisibilityMultiplier > 2.0) || (double) this.RecentEvents[index].TimeSinceSensed > (double) this.m_maxTrackingTime)
        {
          if (this.RecentEvents[index].IsEntity && (UnityEngine.Object) this.RecentEvents[index].Entity != (UnityEngine.Object) null)
            this.KnownEntityDic.Remove(this.RecentEvents[index].Entity);
          this.RecentEvents[index].Dispose();
          this.RecentEvents.RemoveAt(index);
        }
        else
          this.RecentEvents[index].Tick();
      }
      if (this.RecentEvents.Count > 0)
      {
        this.RecentEvents.Sort();
        if (this.RecentEvents.Count > this.m_eventCapacity)
        {
          for (int index = this.RecentEvents.Count - 1; index >= this.m_eventCapacity; --index)
          {
            if (this.RecentEvents[index].IsEntity)
              this.KnownEntityDic.Remove(this.RecentEvents[index].Entity);
            this.RecentEvents[index].Dispose();
            this.RecentEvents.RemoveAt(index);
          }
        }
        this.m_timeToNoFreshTargetTick = 0.0f;
        this.m_targetPoint = (double) this.RecentEvents[0].TimeSinceSensed >= (double) this.E.ManagerCheckFrequency ? this.RecentEvents[0].Pos : this.RecentEvents[0].UpdatePos();
        this.m_timeTargetSeen = this.RecentEvents[0].TimeSeen;
      }
      else
      {
        if ((double) this.m_timeToNoFreshTargetTick < (double) this.m_timeToNoFreshTarget)
          this.m_timeToNoFreshTargetTick += Time.deltaTime;
        this.m_timeTargetSeen = 0.0f;
      }
    }
  }
}
