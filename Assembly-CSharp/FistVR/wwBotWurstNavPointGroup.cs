// Decompiled with JetBrains decompiler
// Type: FistVR.wwBotWurstNavPointGroup
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class wwBotWurstNavPointGroup : MonoBehaviour
  {
    public List<wwBotWurstNavPoint> Points;
    public bool UsesDynamicPointSet;
    public float DynamicPointAddRange = 60f;
    public float DynamicPointRemoveRange = 70f;
    private int m_curPointAddIndex;
    private int m_curPointRemoveIndex;
    private Vector3 testPos = Vector3.zero;
    private List<wwBotWurstNavPoint> m_dynamicPointSet = new List<wwBotWurstNavPoint>();
    private HashSet<wwBotWurstNavPoint> m_dynamicPointHash = new HashSet<wwBotWurstNavPoint>();

    public void Awake()
    {
      for (int index = 0; index < this.Points.Count; ++index)
        this.PointAdder();
    }

    public List<wwBotWurstNavPoint> GetDynamicSet() => this.m_dynamicPointSet;

    public void Update()
    {
      if (!this.UsesDynamicPointSet)
        return;
      if ((Object) GM.CurrentPlayerBody != (Object) null)
        this.testPos = GM.CurrentPlayerBody.transform.position;
      this.PointRemover();
      this.PointAdder();
    }

    [ContextMenu("ShufflePoints")]
    private void ChooseRandomIndicies()
    {
      for (int index1 = 0; index1 < this.Points.Count; ++index1)
      {
        wwBotWurstNavPoint point = this.Points[index1];
        int index2 = Random.Range(index1, this.Points.Count);
        this.Points[index1] = this.Points[index2];
        this.Points[index2] = point;
      }
    }

    private void PointRemover()
    {
      --this.m_curPointRemoveIndex;
      if (this.m_dynamicPointSet.Count <= 0)
        return;
      if (this.m_curPointRemoveIndex < 0)
        this.m_curPointRemoveIndex = this.m_dynamicPointSet.Count - 1;
      if (this.m_curPointRemoveIndex >= this.m_dynamicPointSet.Count)
        this.m_curPointRemoveIndex = this.m_dynamicPointSet.Count - 1;
      if (!((Object) this.m_dynamicPointSet[this.m_curPointRemoveIndex] != (Object) null) || (double) Vector3.Distance(this.m_dynamicPointSet[this.m_curPointRemoveIndex].transform.position, this.testPos) <= (double) this.DynamicPointRemoveRange)
        return;
      this.m_dynamicPointHash.Remove(this.m_dynamicPointSet[this.m_curPointRemoveIndex]);
      this.m_dynamicPointSet.RemoveAt(this.m_curPointRemoveIndex);
    }

    private void PointAdder()
    {
      ++this.m_curPointAddIndex;
      if (this.m_curPointAddIndex >= this.Points.Count)
        this.m_curPointAddIndex = 0;
      if ((Object) this.Points[this.m_curPointAddIndex] == (Object) null || this.m_dynamicPointHash.Contains(this.Points[this.m_curPointAddIndex]) || (double) Vector3.Distance(this.Points[this.m_curPointAddIndex].transform.position, this.testPos) >= (double) this.DynamicPointAddRange)
        return;
      this.m_dynamicPointHash.Add(this.Points[this.m_curPointAddIndex]);
      this.m_dynamicPointSet.Add(this.Points[this.m_curPointAddIndex]);
    }

    public Vector3 GetClosestDestinationToTarget(Vector3 targ)
    {
      wwBotWurstNavPoint navPointToTarget = this.GetClosestNavPointToTarget(targ);
      return this.GetClosestPointToTarget(targ, navPointToTarget.Facings);
    }

    public Vector3 GetFurthestDestinationToTarget(Vector3 targ)
    {
      wwBotWurstNavPoint navPointFromTarget = this.GetFurthestNavPointFromTarget(targ);
      return this.GetFurthestPointFromTarget(targ, navPointFromTarget.Facings);
    }

    public Vector3 GetClosestCoverFromAttacker(Vector3 me, Vector3 attackerPos)
    {
      wwBotWurstNavPoint navPointToTarget = this.GetClosestNavPointToTarget(me);
      int forwardAlignment = this.GetIndexOfClosestForwardAlignment(attackerPos - navPointToTarget.transform.position, navPointToTarget.Facings);
      return navPointToTarget.Facings[forwardAlignment].position;
    }

    public Vector3 GetBestPointToFleeTo(Vector3 me, Vector3 attackerPos)
    {
      wwBotWurstNavPoint navPointToFleeTo = this.GetBestNavPointToFleeTo(me, attackerPos);
      return this.GetFurthestPointFromTarget(attackerPos, navPointToFleeTo.Facings);
    }

    public Vector3 GetRandomPatrolPoint()
    {
      wwBotWurstNavPoint point = this.Points[Random.Range(0, this.Points.Count)];
      return point.Facings[Random.Range(0, point.Facings.Length)].position;
    }

    private wwBotWurstNavPoint GetClosestNavPointToTarget(Vector3 targ)
    {
      float num1 = 1000f;
      List<wwBotWurstNavPoint> botWurstNavPointList = this.Points;
      if (this.UsesDynamicPointSet)
        botWurstNavPointList = this.m_dynamicPointSet;
      wwBotWurstNavPoint point = this.Points[0];
      for (int index = 0; index < botWurstNavPointList.Count; ++index)
      {
        float num2 = Vector3.Distance(botWurstNavPointList[index].transform.position, targ);
        if ((double) num2 < (double) num1)
        {
          num1 = num2;
          point = botWurstNavPointList[index];
        }
      }
      return point;
    }

    private wwBotWurstNavPoint GetFurthestNavPointFromTarget(Vector3 targ)
    {
      float num1 = 0.0f;
      List<wwBotWurstNavPoint> botWurstNavPointList = this.Points;
      if (this.UsesDynamicPointSet)
        botWurstNavPointList = this.m_dynamicPointSet;
      wwBotWurstNavPoint point = this.Points[0];
      for (int index = 0; index < botWurstNavPointList.Count; ++index)
      {
        float num2 = Vector3.Distance(botWurstNavPointList[index].transform.position, targ);
        if ((double) num2 > (double) num1)
        {
          num1 = num2;
          point = botWurstNavPointList[index];
        }
      }
      return point;
    }

    private wwBotWurstNavPoint GetBestNavPointToFleeTo(
      Vector3 me,
      Vector3 attackerPos)
    {
      Vector3 normalized = (attackerPos - me).normalized;
      float num1 = 0.0f;
      float num2 = 0.0f;
      wwBotWurstNavPoint botWurstNavPoint = (wwBotWurstNavPoint) null;
      List<wwBotWurstNavPoint> botWurstNavPointList = this.Points;
      if (this.UsesDynamicPointSet)
        botWurstNavPointList = this.m_dynamicPointSet;
      wwBotWurstNavPoint point = this.Points[0];
      for (int index = 0; index < botWurstNavPointList.Count; ++index)
      {
        float num3 = Vector3.Distance(botWurstNavPointList[index].transform.position, attackerPos);
        float num4 = Vector3.Angle((botWurstNavPointList[index].transform.position - me).normalized, normalized);
        if ((double) num3 > (double) num1)
        {
          num1 = num3;
          point = this.Points[index];
        }
        if ((double) num3 > (double) num2 && (double) num4 > 90.0)
        {
          num2 = num3;
          botWurstNavPoint = this.Points[index];
        }
      }
      return (Object) botWurstNavPoint != (Object) null ? botWurstNavPoint : point;
    }

    public Vector3 GetClosestPointToTarget(Vector3 targ, Transform[] points)
    {
      float num1 = 1000f;
      Vector3 vector3 = Vector3.zero;
      for (int index = 0; index < points.Length; ++index)
      {
        float num2 = Vector3.Distance(points[index].position, targ);
        if ((double) num2 < (double) num1)
        {
          num1 = num2;
          vector3 = points[index].position;
        }
      }
      return vector3;
    }

    public Vector3 GetFurthestPointFromTarget(Vector3 targ, Transform[] points)
    {
      float num1 = 0.0f;
      Vector3 vector3 = Vector3.zero;
      for (int index = 0; index < points.Length; ++index)
      {
        float num2 = Vector3.Distance(points[index].position, targ);
        if ((double) num2 > (double) num1)
        {
          num1 = num2;
          vector3 = points[index].position;
        }
      }
      return vector3;
    }

    public int GetIndexOfClosestForwardAlignment(Vector3 forward, Transform[] points)
    {
      float num1 = 360f;
      int num2 = 0;
      for (int index = 0; index < points.Length; ++index)
      {
        float num3 = Vector3.Angle(forward, points[index].forward);
        if ((double) num3 < (double) num1)
        {
          num1 = num3;
          num2 = index;
        }
      }
      return num2;
    }

    public Vector3 GetClosestCoverPoint(Vector3 playerPos, Vector3 rayToPlayer) => Vector3.zero;
  }
}
