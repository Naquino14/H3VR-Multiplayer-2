// Decompiled with JetBrains decompiler
// Type: FistVR.TNH_DestructibleBarrierPoint
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace FistVR
{
  public class TNH_DestructibleBarrierPoint : MonoBehaviour
  {
    public List<AICoverPoint> CoverPoints;
    public List<TNH_DestructibleBarrierPoint.BarrierDataSet> BarrierDataSets;
    public NavMeshObstacle Obstacle;
    public bool ShouldDebug;
    public int TestingDataIndex;
    private TNH_DestructibleBarrier m_curBarrier;

    public void Init()
    {
      this.Obstacle.enabled = false;
      this.SetCoverPointData(-1);
    }

    public bool IsBarrierActive() => !((UnityEngine.Object) this.m_curBarrier == (UnityEngine.Object) null);

    public void BarrierDestroyed()
    {
      this.m_curBarrier = (TNH_DestructibleBarrier) null;
      this.SetCoverPointData(-1);
    }

    public bool SpawnRandomBarrier()
    {
      if ((UnityEngine.Object) this.m_curBarrier != (UnityEngine.Object) null)
        return false;
      int index = UnityEngine.Random.Range(0, this.BarrierDataSets.Count);
      this.m_curBarrier = UnityEngine.Object.Instantiate<GameObject>(this.BarrierDataSets[index].BarrierPrefab, this.transform.position, this.transform.rotation).GetComponent<TNH_DestructibleBarrier>();
      this.m_curBarrier.InitToPlace(this.transform.position, this.transform.forward);
      this.m_curBarrier.SetBarrierPoint(this);
      this.SetCoverPointData(index);
      return true;
    }

    public void LowerBarrierThenDestroy()
    {
      if (!((UnityEngine.Object) this.m_curBarrier != (UnityEngine.Object) null))
        return;
      this.m_curBarrier.Lower();
    }

    public void DeleteBarrier()
    {
      if ((UnityEngine.Object) this.m_curBarrier != (UnityEngine.Object) null)
        UnityEngine.Object.Destroy((UnityEngine.Object) this.m_curBarrier.gameObject);
      this.SetCoverPointData(-1);
    }

    private void SetCoverPointData(int index)
    {
      if (index < 0)
      {
        for (int index1 = 0; index1 < this.CoverPoints.Count; ++index1)
          this.CoverPoints[index1].IsActive = false;
      }
      else
      {
        for (int index1 = 0; index1 < this.CoverPoints.Count; ++index1)
        {
          this.CoverPoints[index1].IsActive = true;
          for (int index2 = 0; index2 < this.CoverPoints[index1].MaxVis.Length; ++index2)
            this.CoverPoints[index1].MaxVis[index2] = this.BarrierDataSets[index].Points[index1].Data[index2];
        }
      }
    }

    [ContextMenu("TestSwapData")]
    public void TestSwapData()
    {
      for (int index1 = 0; index1 < this.CoverPoints.Count; ++index1)
      {
        for (int index2 = 0; index2 < this.CoverPoints[index1].MaxVis.Length; ++index2)
          this.CoverPoints[index1].MaxVis[index2] = this.BarrierDataSets[this.TestingDataIndex].Points[index1].Data[index2];
      }
    }

    [ContextMenu("DebugOn")]
    public void DebugOn()
    {
      for (int index = 0; index < this.CoverPoints.Count; ++index)
      {
        this.CoverPoints[index].DoDebug[0] = true;
        this.CoverPoints[index].DoDebug[1] = true;
        this.CoverPoints[index].DoDebug[2] = true;
      }
    }

    [ContextMenu("DebugOff")]
    public void DebugOff()
    {
      for (int index = 0; index < this.CoverPoints.Count; ++index)
      {
        this.CoverPoints[index].DoDebug[0] = false;
        this.CoverPoints[index].DoDebug[1] = false;
        this.CoverPoints[index].DoDebug[2] = false;
      }
    }

    [ContextMenu("BakePoints")]
    public void BakePoints()
    {
      for (int index1 = 0; index1 < this.BarrierDataSets.Count; ++index1)
      {
        this.BarrierDataSets[index1].Points.Clear();
        GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.BarrierDataSets[index1].BarrierPrefab, this.transform.position, this.transform.rotation);
        for (int index2 = 0; index2 < this.CoverPoints.Count; ++index2)
        {
          TNH_DestructibleBarrierPoint.BarrierDataSet.SavedCoverPointData savedCoverPointData = new TNH_DestructibleBarrierPoint.BarrierDataSet.SavedCoverPointData();
          savedCoverPointData.Data = new List<float>();
          this.CoverPoints[index2].Calc();
          for (int index3 = 0; index3 < this.CoverPoints[index2].MaxVis.Length; ++index3)
            savedCoverPointData.Data.Add(this.CoverPoints[index2].MaxVis[index3]);
          this.BarrierDataSets[index1].Points.Add(savedCoverPointData);
        }
        UnityEngine.Object.DestroyImmediate((UnityEngine.Object) gameObject);
      }
    }

    private void OnDrawGizmos()
    {
      if (!this.ShouldDebug)
        return;
      Matrix4x4 matrix4x4 = Matrix4x4.TRS(this.transform.position, this.transform.rotation, this.transform.localScale);
      Matrix4x4 matrix = Gizmos.matrix;
      Gizmos.color = new Color(1f, 0.6f, 0.2f);
      Gizmos.matrix *= matrix4x4;
      Gizmos.DrawCube(this.Obstacle.center, this.Obstacle.size);
      Gizmos.DrawWireCube(this.Obstacle.center, this.Obstacle.size);
      Gizmos.matrix = matrix;
    }

    [Serializable]
    public class BarrierDataSet
    {
      public GameObject BarrierPrefab;
      public List<TNH_DestructibleBarrierPoint.BarrierDataSet.SavedCoverPointData> Points;

      [Serializable]
      public class SavedCoverPointData
      {
        public List<float> Data;
      }
    }
  }
}
