using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	[Serializable]
	public class MF_Squad
	{
		private MF_Team team;

		private List<Sosig> members = new List<Sosig>();

		private Dictionary<Sosig, MF_Class> classDic = new Dictionary<Sosig, MF_Class>();

		private Sosig squadmedic;

		private bool m_hasMedic;

		private MF_ZoneMeta m_assignedZone;

		private Vector3 m_patrolToPoint = Vector3.zero;

		private float m_tickDownToRearmCheck = 1f;

		public void AssignZone(MF_ZoneMeta z, Sosig s)
		{
			m_assignedZone = z;
			m_assignedZone.AssignSquad(this);
			PickNewRandomPatrolPoint(s);
		}

		public void AddMember(Sosig s, MF_Class c, MF_Team t)
		{
			team = t;
			members.Add(s);
			classDic.Add(s, c);
			if (c == MF_Class.Medic)
			{
				m_hasMedic = true;
				squadmedic = s;
			}
		}

		public void Tick(float t)
		{
			m_tickDownToRearmCheck -= Time.deltaTime;
			if (m_tickDownToRearmCheck <= 0f)
			{
				m_tickDownToRearmCheck = UnityEngine.Random.Range(1f, 2f);
				RearmCheck();
			}
			if (members.Count <= 0)
			{
				return;
			}
			Sosig sosig = members[0];
			if (sosig != null)
			{
				float num = Vector3.Distance(sosig.transform.position, m_patrolToPoint);
				if (team.GetColor() == MF_TeamColor.Blue)
				{
					Debug.DrawLine(sosig.transform.position, m_patrolToPoint, Color.blue);
				}
				else
				{
					Debug.DrawLine(sosig.transform.position, m_patrolToPoint, Color.red);
				}
				if (num < 2f)
				{
					PickNewRandomPatrolPoint(sosig);
				}
			}
			if (members.Count <= 1)
			{
				return;
			}
			for (int i = 1; i < members.Count; i++)
			{
				if (members[i] != null)
				{
					Vector3 vector = members[i].transform.position - sosig.transform.position;
					vector.y = 0f;
					vector.Normalize();
					Vector3 vector2 = sosig.transform.position + vector;
					members[i].UpdateAssaultPoint(vector2);
					Debug.DrawLine(members[i].transform.position, vector2, Color.white);
				}
			}
		}

		private void PickNewRandomPatrolPoint(Sosig s)
		{
			m_patrolToPoint = m_assignedZone.Zone.GetTargetPointByClass(classDic[s]).position;
			s.UpdateAssaultPoint(m_patrolToPoint);
		}

		private void RearmCheck()
		{
			for (int i = 0; i < members.Count; i++)
			{
				if (members[i] != null && !members[i].DoIHaveAGun() && members[i].CurrentOrder == Sosig.SosigOrder.SearchForEquipment && members[i].BodyState == Sosig.SosigBodyState.InControl)
				{
					team.GetManager().RearmSosig(members[i], classDic[members[i]]);
				}
			}
		}

		public void Cleanup()
		{
			if (members.Count <= 0)
			{
				return;
			}
			for (int num = members.Count - 1; num >= 0; num--)
			{
				if (members[num].BodyState == Sosig.SosigBodyState.Dead)
				{
					members[num].TickDownToClear(5f);
					members.RemoveAt(num);
				}
			}
		}

		public void Flush()
		{
			if (members.Count > 0)
			{
				for (int num = members.Count - 1; num >= 0; num--)
				{
					if (members[num].BodyState == Sosig.SosigBodyState.Dead)
					{
						members[num].TickDownToClear(5f);
						members.RemoveAt(num);
					}
				}
			}
			classDic.Clear();
			squadmedic = null;
			m_hasMedic = false;
			team = null;
			m_assignedZone.DeAssignSquad(this);
			m_assignedZone = null;
		}

		public int GetNumAlive()
		{
			return members.Count;
		}
	}
}
