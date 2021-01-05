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

		public void DestroySystem()
		{
			m_isDestroyed = true;
		}

		public bool HasFreshTarget()
		{
			if (m_isDestroyed)
			{
				return m_lastFreshTargetAnswer;
			}
			if (m_timeToNoFreshTargetTick >= m_timeToNoFreshTarget)
			{
				m_lastFreshTargetAnswer = false;
				return false;
			}
			m_lastFreshTargetAnswer = true;
			return true;
		}

		public float GetTimeSinceTopTargetSeen()
		{
			if (m_isDestroyed)
			{
				return 0f;
			}
			if (RecentEvents.Count > 0)
			{
				return RecentEvents[0].TimeSinceSensed;
			}
			return m_maxTrackingTime + 0.01f;
		}

		public Vector3 GetTargetPoint()
		{
			if (m_isDestroyed)
			{
				return E.transform.position + UnityEngine.Random.onUnitSphere;
			}
			return m_targetPoint;
		}

		public float GetTimeTargetSeen()
		{
			if (m_isDestroyed)
			{
				return UnityEngine.Random.Range(0f, 10f);
			}
			return m_timeTargetSeen;
		}

		public bool IsTargetEntity()
		{
			if (RecentEvents.Count <= 0)
			{
				return false;
			}
			if (RecentEvents[0].IsEntity)
			{
				return true;
			}
			return false;
		}

		public Vector3 GetTargetGroundPoint()
		{
			if (RecentEvents.Count > 0)
			{
				return RecentEvents[0].Entity.GetGroundPos();
			}
			return GetTargetPoint();
		}

		public float GetAngleToVertical(Transform myFrame)
		{
			if (m_isDestroyed)
			{
				return 0f;
			}
			Vector3 vector = m_targetPoint - myFrame.position;
			vector = Vector3.ProjectOnPlane(vector, myFrame.right);
			return Vector3.Angle(myFrame.forward, vector);
		}

		public float GetAngleToHorizontal(Transform myFrame)
		{
			if (m_isDestroyed)
			{
				return 0f;
			}
			Vector3 vector = m_targetPoint - myFrame.position;
			vector = Vector3.ProjectOnPlane(vector, myFrame.up);
			return Vector3.Angle(myFrame.forward, vector);
		}

		public float GetDistanceToTarget(Transform myFrame)
		{
			if (m_isDestroyed)
			{
				return 400f;
			}
			return (m_targetPoint - myFrame.position).magnitude;
		}

		public void Init(AIEntity e, int capacity, float maxTrackingTime, float noFreshTargetTime)
		{
			E = e;
			m_eventCapacity = capacity;
			m_maxTrackingTime = maxTrackingTime;
			m_timeToNoFreshTarget = noFreshTargetTime;
			m_timeToNoFreshTargetTick = m_timeToNoFreshTarget;
		}

		public void ProcessEvent(AIEvent e)
		{
			if (m_isDestroyed)
			{
				return;
			}
			if (e.IsEntity && KnownEntityDic.ContainsKey(e.Entity))
			{
				KnownEntityDic[e.Entity].UpdateFrom(e);
				e.Dispose();
				return;
			}
			if (e.IsEntity)
			{
				KnownEntityDic.Add(e.Entity, e);
			}
			RecentEvents.Add(e);
		}

		public void Compute()
		{
			if (m_isDestroyed)
			{
				return;
			}
			for (int num = RecentEvents.Count - 1; num >= 0; num--)
			{
				if ((RecentEvents[num].IsEntity && (RecentEvents[num].Entity == null || RecentEvents[num].Entity.IFFCode < -1 || RecentEvents[num].Entity.VisibilityMultiplier > 2f)) || RecentEvents[num].TimeSinceSensed > m_maxTrackingTime)
				{
					if (RecentEvents[num].IsEntity && RecentEvents[num].Entity != null)
					{
						KnownEntityDic.Remove(RecentEvents[num].Entity);
					}
					RecentEvents[num].Dispose();
					RecentEvents.RemoveAt(num);
				}
				else
				{
					RecentEvents[num].Tick();
				}
			}
			if (RecentEvents.Count > 0)
			{
				RecentEvents.Sort();
				if (RecentEvents.Count > m_eventCapacity)
				{
					for (int num2 = RecentEvents.Count - 1; num2 >= m_eventCapacity; num2--)
					{
						if (RecentEvents[num2].IsEntity)
						{
							KnownEntityDic.Remove(RecentEvents[num2].Entity);
						}
						RecentEvents[num2].Dispose();
						RecentEvents.RemoveAt(num2);
					}
				}
				m_timeToNoFreshTargetTick = 0f;
				if (RecentEvents[0].TimeSinceSensed < E.ManagerCheckFrequency)
				{
					m_targetPoint = RecentEvents[0].UpdatePos();
				}
				else
				{
					m_targetPoint = RecentEvents[0].Pos;
				}
				m_timeTargetSeen = RecentEvents[0].TimeSeen;
			}
			else
			{
				if (m_timeToNoFreshTargetTick < m_timeToNoFreshTarget)
				{
					m_timeToNoFreshTargetTick += Time.deltaTime;
				}
				m_timeTargetSeen = 0f;
			}
		}
	}
}
