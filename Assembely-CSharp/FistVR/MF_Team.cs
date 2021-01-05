using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	[Serializable]
	public class MF_Team
	{
		private MF_TeamColor m_teamColor;

		private MF_TeamManager manager;

		private MF_Team opposingTeam;

		private List<MF_ZoneMeta> m_zones = new List<MF_ZoneMeta>();

		private List<MF_ZoneMeta> m_spawnZones = new List<MF_ZoneMeta>();

		private List<MF_Squad> m_squads = new List<MF_Squad>();

		private int maxSize;

		private float spawnCadence;

		private float m_tickDownToSpawn;

		public MF_TeamColor GetColor()
		{
			return m_teamColor;
		}

		public MF_TeamManager GetManager()
		{
			return manager;
		}

		public void Init(MF_TeamManager m, MF_TeamColor tc, MF_Team o, int max, float spawnspeed)
		{
			manager = m;
			opposingTeam = o;
			m_teamColor = tc;
			maxSize = max;
			spawnCadence = spawnspeed;
			m_tickDownToSpawn = spawnCadence * UnityEngine.Random.Range(1f, 1.2f);
			UpdateZoneMetaScoring();
			for (int i = 0; i < m_spawnZones.Count; i++)
			{
				SpawnSquad(i);
			}
		}

		public void ResetZoneSet()
		{
			if (m_spawnZones.Count > 0)
			{
				for (int num = m_spawnZones.Count - 1; num >= 0; num--)
				{
					m_spawnZones[num].Zone = null;
				}
			}
			m_spawnZones.Clear();
			if (m_zones.Count > 0)
			{
				for (int num2 = m_zones.Count - 1; num2 >= 0; num2--)
				{
					m_zones[num2].Zone = null;
				}
			}
			m_zones.Clear();
		}

		public void LoadZoneList(List<MF_Zone> zones, MF_ZoneType type)
		{
			if (type == MF_ZoneType.Spawn)
			{
				for (int i = 0; i < zones.Count; i++)
				{
					MF_ZoneMeta mF_ZoneMeta = new MF_ZoneMeta();
					mF_ZoneMeta.Type = type;
					mF_ZoneMeta.Zone = zones[i];
					m_spawnZones.Add(mF_ZoneMeta);
				}
			}
			else
			{
				for (int j = 0; j < zones.Count; j++)
				{
					MF_ZoneMeta mF_ZoneMeta2 = new MF_ZoneMeta();
					mF_ZoneMeta2.Type = type;
					mF_ZoneMeta2.Zone = zones[j];
					m_zones.Add(mF_ZoneMeta2);
				}
			}
		}

		public void Tick(float t)
		{
			if (m_squads.Count > 0)
			{
				for (int num = m_squads.Count - 1; num >= 0; num--)
				{
					m_squads[num].Cleanup();
				}
			}
			if (m_squads.Count > 0)
			{
				for (int num2 = m_squads.Count - 1; num2 >= 0; num2--)
				{
					if (m_squads[num2].GetNumAlive() < 1)
					{
						m_squads[num2].Flush();
						m_squads.RemoveAt(num2);
					}
				}
			}
			UpdateZoneMetaScoring();
			m_tickDownToSpawn -= t;
			if (m_tickDownToSpawn <= 0f)
			{
				m_tickDownToSpawn = spawnCadence;
				SpawnSquad(UnityEngine.Random.Range(0, m_spawnZones.Count));
			}
			for (int i = 0; i < m_squads.Count; i++)
			{
				m_squads[i].Tick(t);
			}
		}

		private void UpdateZoneMetaScoring()
		{
			for (int i = 0; i < m_zones.Count; i++)
			{
				m_zones[i].Neccesity = UnityEngine.Random.Range(0.1f, 0.5f);
			}
			for (int j = 0; j < m_zones.Count; j++)
			{
				if (m_zones[j].Type == MF_ZoneType.EnemySide)
				{
					m_zones[j].Neccesity += UnityEngine.Random.Range(1f, 1.6f);
				}
				if (m_zones[j].Type == MF_ZoneType.Center)
				{
					m_zones[j].Neccesity += UnityEngine.Random.Range(2f, 10f);
				}
				if (m_zones[j].GetNumAssignedSquads() > 2)
				{
					m_zones[j].Neccesity -= UnityEngine.Random.Range(-0.1f, -1.5f);
				}
			}
			m_zones.Sort();
			m_zones.Reverse();
		}

		public void SpawnSquad(int spawnZoneIndex)
		{
			MF_ZoneMeta mF_ZoneMeta = m_zones[0];
			int num = maxSize - NumOnTeam();
			if (num < 1)
			{
				return;
			}
			MF_SquadDefinition mF_SquadDefinition = null;
			mF_SquadDefinition = manager.SquadDefs[UnityEngine.Random.Range(0, manager.SquadDefs.Count)];
			int num2 = Mathf.Min(num, mF_SquadDefinition.MemberClasses.Count);
			MF_ZoneMeta mF_ZoneMeta2 = m_spawnZones[spawnZoneIndex];
			MF_Squad mF_Squad = new MF_Squad();
			Sosig s = null;
			for (int i = 0; i < num2; i++)
			{
				MF_Class mF_Class = mF_SquadDefinition.MemberClasses[i];
				mF_ZoneMeta2.Zone.TargetPoints_Assault.Shuffle();
				Transform transform = mF_ZoneMeta2.Zone.TargetPoints_Assault[i].transform;
				Transform targetPointByClass = mF_ZoneMeta.Zone.GetTargetPointByClass(mF_Class);
				SosigEnemyTemplate t = manager.EnemyTemplatesByClassIndex_Red[(int)mF_Class];
				int iFF = manager.IFF_Red;
				if (m_teamColor == MF_TeamColor.Blue)
				{
					iFF = manager.IFF_Blue;
					t = manager.EnemyTemplatesByClassIndex_Blue[(int)mF_Class];
				}
				Sosig sosig = manager.SpawnEnemy(t, transform, iFF, IsAssault: true, targetPointByClass.position, AllowSecondary: true, m_teamColor);
				if (i == 0)
				{
					s = sosig;
				}
				mF_Squad.AddMember(sosig, mF_Class, this);
			}
			mF_Squad.AssignZone(mF_ZoneMeta, s);
			m_squads.Add(mF_Squad);
		}

		private int NumOnTeam()
		{
			int num = 0;
			for (int i = 0; i < m_squads.Count; i++)
			{
				num += m_squads[i].GetNumAlive();
			}
			return num;
		}
	}
}
