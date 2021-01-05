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
			for (int i = 0; i < Points.Count; i++)
			{
				PointAdder();
			}
		}

		public List<wwBotWurstNavPoint> GetDynamicSet()
		{
			return m_dynamicPointSet;
		}

		public void Update()
		{
			if (UsesDynamicPointSet)
			{
				if (GM.CurrentPlayerBody != null)
				{
					testPos = GM.CurrentPlayerBody.transform.position;
				}
				PointRemover();
				PointAdder();
			}
		}

		[ContextMenu("ShufflePoints")]
		private void ChooseRandomIndicies()
		{
			for (int i = 0; i < Points.Count; i++)
			{
				wwBotWurstNavPoint value = Points[i];
				int index = Random.Range(i, Points.Count);
				Points[i] = Points[index];
				Points[index] = value;
			}
		}

		private void PointRemover()
		{
			m_curPointRemoveIndex--;
			if (m_dynamicPointSet.Count <= 0)
			{
				return;
			}
			if (m_curPointRemoveIndex < 0)
			{
				m_curPointRemoveIndex = m_dynamicPointSet.Count - 1;
			}
			if (m_curPointRemoveIndex >= m_dynamicPointSet.Count)
			{
				m_curPointRemoveIndex = m_dynamicPointSet.Count - 1;
			}
			if (m_dynamicPointSet[m_curPointRemoveIndex] != null)
			{
				float num = Vector3.Distance(m_dynamicPointSet[m_curPointRemoveIndex].transform.position, testPos);
				if (num > DynamicPointRemoveRange)
				{
					m_dynamicPointHash.Remove(m_dynamicPointSet[m_curPointRemoveIndex]);
					m_dynamicPointSet.RemoveAt(m_curPointRemoveIndex);
				}
			}
		}

		private void PointAdder()
		{
			m_curPointAddIndex++;
			if (m_curPointAddIndex >= Points.Count)
			{
				m_curPointAddIndex = 0;
			}
			if (!(Points[m_curPointAddIndex] == null) && !m_dynamicPointHash.Contains(Points[m_curPointAddIndex]))
			{
				float num = Vector3.Distance(Points[m_curPointAddIndex].transform.position, testPos);
				if (num < DynamicPointAddRange)
				{
					m_dynamicPointHash.Add(Points[m_curPointAddIndex]);
					m_dynamicPointSet.Add(Points[m_curPointAddIndex]);
				}
			}
		}

		public Vector3 GetClosestDestinationToTarget(Vector3 targ)
		{
			wwBotWurstNavPoint closestNavPointToTarget = GetClosestNavPointToTarget(targ);
			return GetClosestPointToTarget(targ, closestNavPointToTarget.Facings);
		}

		public Vector3 GetFurthestDestinationToTarget(Vector3 targ)
		{
			wwBotWurstNavPoint furthestNavPointFromTarget = GetFurthestNavPointFromTarget(targ);
			return GetFurthestPointFromTarget(targ, furthestNavPointFromTarget.Facings);
		}

		public Vector3 GetClosestCoverFromAttacker(Vector3 me, Vector3 attackerPos)
		{
			wwBotWurstNavPoint closestNavPointToTarget = GetClosestNavPointToTarget(me);
			Vector3 forward = attackerPos - closestNavPointToTarget.transform.position;
			int indexOfClosestForwardAlignment = GetIndexOfClosestForwardAlignment(forward, closestNavPointToTarget.Facings);
			return closestNavPointToTarget.Facings[indexOfClosestForwardAlignment].position;
		}

		public Vector3 GetBestPointToFleeTo(Vector3 me, Vector3 attackerPos)
		{
			wwBotWurstNavPoint bestNavPointToFleeTo = GetBestNavPointToFleeTo(me, attackerPos);
			return GetFurthestPointFromTarget(attackerPos, bestNavPointToFleeTo.Facings);
		}

		public Vector3 GetRandomPatrolPoint()
		{
			wwBotWurstNavPoint wwBotWurstNavPoint = Points[Random.Range(0, Points.Count)];
			return wwBotWurstNavPoint.Facings[Random.Range(0, wwBotWurstNavPoint.Facings.Length)].position;
		}

		private wwBotWurstNavPoint GetClosestNavPointToTarget(Vector3 targ)
		{
			float num = 1000f;
			wwBotWurstNavPoint wwBotWurstNavPoint = null;
			List<wwBotWurstNavPoint> list = Points;
			if (UsesDynamicPointSet)
			{
				list = m_dynamicPointSet;
			}
			wwBotWurstNavPoint = Points[0];
			for (int i = 0; i < list.Count; i++)
			{
				float num2 = Vector3.Distance(list[i].transform.position, targ);
				if (num2 < num)
				{
					num = num2;
					wwBotWurstNavPoint = list[i];
				}
			}
			return wwBotWurstNavPoint;
		}

		private wwBotWurstNavPoint GetFurthestNavPointFromTarget(Vector3 targ)
		{
			float num = 0f;
			wwBotWurstNavPoint wwBotWurstNavPoint = null;
			List<wwBotWurstNavPoint> list = Points;
			if (UsesDynamicPointSet)
			{
				list = m_dynamicPointSet;
			}
			wwBotWurstNavPoint = Points[0];
			for (int i = 0; i < list.Count; i++)
			{
				float num2 = Vector3.Distance(list[i].transform.position, targ);
				if (num2 > num)
				{
					num = num2;
					wwBotWurstNavPoint = list[i];
				}
			}
			return wwBotWurstNavPoint;
		}

		private wwBotWurstNavPoint GetBestNavPointToFleeTo(Vector3 me, Vector3 attackerPos)
		{
			Vector3 normalized = (attackerPos - me).normalized;
			float num = 0f;
			wwBotWurstNavPoint wwBotWurstNavPoint = null;
			float num2 = 0f;
			wwBotWurstNavPoint wwBotWurstNavPoint2 = null;
			List<wwBotWurstNavPoint> list = Points;
			if (UsesDynamicPointSet)
			{
				list = m_dynamicPointSet;
			}
			wwBotWurstNavPoint = Points[0];
			for (int i = 0; i < list.Count; i++)
			{
				float num3 = Vector3.Distance(list[i].transform.position, attackerPos);
				Vector3 normalized2 = (list[i].transform.position - me).normalized;
				float num4 = Vector3.Angle(normalized2, normalized);
				if (num3 > num)
				{
					num = num3;
					wwBotWurstNavPoint = Points[i];
				}
				if (num3 > num2 && num4 > 90f)
				{
					num2 = num3;
					wwBotWurstNavPoint2 = Points[i];
				}
			}
			if (wwBotWurstNavPoint2 != null)
			{
				return wwBotWurstNavPoint2;
			}
			return wwBotWurstNavPoint;
		}

		public Vector3 GetClosestPointToTarget(Vector3 targ, Transform[] points)
		{
			float num = 1000f;
			Vector3 result = Vector3.zero;
			for (int i = 0; i < points.Length; i++)
			{
				float num2 = Vector3.Distance(points[i].position, targ);
				if (num2 < num)
				{
					num = num2;
					result = points[i].position;
				}
			}
			return result;
		}

		public Vector3 GetFurthestPointFromTarget(Vector3 targ, Transform[] points)
		{
			float num = 0f;
			Vector3 result = Vector3.zero;
			for (int i = 0; i < points.Length; i++)
			{
				float num2 = Vector3.Distance(points[i].position, targ);
				if (num2 > num)
				{
					num = num2;
					result = points[i].position;
				}
			}
			return result;
		}

		public int GetIndexOfClosestForwardAlignment(Vector3 forward, Transform[] points)
		{
			float num = 360f;
			int result = 0;
			for (int i = 0; i < points.Length; i++)
			{
				float num2 = Vector3.Angle(forward, points[i].forward);
				if (num2 < num)
				{
					num = num2;
					result = i;
				}
			}
			return result;
		}

		public Vector3 GetClosestCoverPoint(Vector3 playerPos, Vector3 rayToPlayer)
		{
			return Vector3.zero;
		}
	}
}
