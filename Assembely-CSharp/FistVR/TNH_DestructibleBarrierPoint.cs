using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace FistVR
{
	public class TNH_DestructibleBarrierPoint : MonoBehaviour
	{
		[Serializable]
		public class BarrierDataSet
		{
			[Serializable]
			public class SavedCoverPointData
			{
				public List<float> Data;
			}

			public GameObject BarrierPrefab;

			public List<SavedCoverPointData> Points;
		}

		public List<AICoverPoint> CoverPoints;

		public List<BarrierDataSet> BarrierDataSets;

		public NavMeshObstacle Obstacle;

		public bool ShouldDebug;

		public int TestingDataIndex;

		private TNH_DestructibleBarrier m_curBarrier;

		public void Init()
		{
			Obstacle.enabled = false;
			SetCoverPointData(-1);
		}

		public bool IsBarrierActive()
		{
			if (m_curBarrier == null)
			{
				return false;
			}
			return true;
		}

		public void BarrierDestroyed()
		{
			m_curBarrier = null;
			SetCoverPointData(-1);
		}

		public bool SpawnRandomBarrier()
		{
			if (m_curBarrier != null)
			{
				return false;
			}
			int num = UnityEngine.Random.Range(0, BarrierDataSets.Count);
			BarrierDataSet barrierDataSet = BarrierDataSets[num];
			GameObject gameObject = UnityEngine.Object.Instantiate(barrierDataSet.BarrierPrefab, base.transform.position, base.transform.rotation);
			m_curBarrier = gameObject.GetComponent<TNH_DestructibleBarrier>();
			m_curBarrier.InitToPlace(base.transform.position, base.transform.forward);
			m_curBarrier.SetBarrierPoint(this);
			SetCoverPointData(num);
			return true;
		}

		public void LowerBarrierThenDestroy()
		{
			if (m_curBarrier != null)
			{
				m_curBarrier.Lower();
			}
		}

		public void DeleteBarrier()
		{
			if (m_curBarrier != null)
			{
				UnityEngine.Object.Destroy(m_curBarrier.gameObject);
			}
			SetCoverPointData(-1);
		}

		private void SetCoverPointData(int index)
		{
			if (index < 0)
			{
				for (int i = 0; i < CoverPoints.Count; i++)
				{
					CoverPoints[i].IsActive = false;
				}
				return;
			}
			for (int j = 0; j < CoverPoints.Count; j++)
			{
				CoverPoints[j].IsActive = true;
				for (int k = 0; k < CoverPoints[j].MaxVis.Length; k++)
				{
					CoverPoints[j].MaxVis[k] = BarrierDataSets[index].Points[j].Data[k];
				}
			}
		}

		[ContextMenu("TestSwapData")]
		public void TestSwapData()
		{
			for (int i = 0; i < CoverPoints.Count; i++)
			{
				for (int j = 0; j < CoverPoints[i].MaxVis.Length; j++)
				{
					CoverPoints[i].MaxVis[j] = BarrierDataSets[TestingDataIndex].Points[i].Data[j];
				}
			}
		}

		[ContextMenu("DebugOn")]
		public void DebugOn()
		{
			for (int i = 0; i < CoverPoints.Count; i++)
			{
				CoverPoints[i].DoDebug[0] = true;
				CoverPoints[i].DoDebug[1] = true;
				CoverPoints[i].DoDebug[2] = true;
			}
		}

		[ContextMenu("DebugOff")]
		public void DebugOff()
		{
			for (int i = 0; i < CoverPoints.Count; i++)
			{
				CoverPoints[i].DoDebug[0] = false;
				CoverPoints[i].DoDebug[1] = false;
				CoverPoints[i].DoDebug[2] = false;
			}
		}

		[ContextMenu("BakePoints")]
		public void BakePoints()
		{
			for (int i = 0; i < BarrierDataSets.Count; i++)
			{
				BarrierDataSets[i].Points.Clear();
				GameObject obj = UnityEngine.Object.Instantiate(BarrierDataSets[i].BarrierPrefab, base.transform.position, base.transform.rotation);
				for (int j = 0; j < CoverPoints.Count; j++)
				{
					BarrierDataSet.SavedCoverPointData savedCoverPointData = new BarrierDataSet.SavedCoverPointData();
					savedCoverPointData.Data = new List<float>();
					CoverPoints[j].Calc();
					for (int k = 0; k < CoverPoints[j].MaxVis.Length; k++)
					{
						savedCoverPointData.Data.Add(CoverPoints[j].MaxVis[k]);
					}
					BarrierDataSets[i].Points.Add(savedCoverPointData);
				}
				UnityEngine.Object.DestroyImmediate(obj);
			}
		}

		private void OnDrawGizmos()
		{
			if (ShouldDebug)
			{
				Matrix4x4 matrix4x = Matrix4x4.TRS(base.transform.position, base.transform.rotation, base.transform.localScale);
				Matrix4x4 matrix = Gizmos.matrix;
				Gizmos.color = new Color(1f, 0.6f, 0.2f);
				Gizmos.matrix *= matrix4x;
				Gizmos.DrawCube(Obstacle.center, Obstacle.size);
				Gizmos.DrawWireCube(Obstacle.center, Obstacle.size);
				Gizmos.matrix = matrix;
			}
		}
	}
}
