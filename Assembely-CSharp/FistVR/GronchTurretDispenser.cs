using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class GronchTurretDispenser : MonoBehaviour
	{
		private GronchJobManager m_M;

		public List<FVRObject> TurretsToSpawn = new List<FVRObject>();

		private bool m_isSpawning;

		private float m_timeTilSpawn = 4f;

		private Vector2 TickRange = new Vector2(2f, 5f);

		private AutoMeater[] m_turrets = new AutoMeater[13];

		public List<Transform> Positions;

		private int m_curPos;

		public AudioEvent AudEvent_Dispense;

		public void BeginJob(GronchJobManager m)
		{
			m_M = m;
			m_isSpawning = true;
		}

		public void EndJob(GronchJobManager m)
		{
			m_M = null;
			m_isSpawning = false;
			KillTurrets();
		}

		public void PlayerDied(GronchJobManager m)
		{
			m_M = m;
			m_M.Promotion();
		}

		public void HitButton(int i)
		{
			if (m_turrets[i] != null)
			{
				m_turrets[i].KillMe();
				m_turrets[i].TickDownToClear(1f);
				m_turrets[i] = null;
				m_M.DidJobStuff();
			}
		}

		private void KillTurrets()
		{
			for (int num = m_turrets.Length - 1; num >= 0; num--)
			{
				if (m_turrets[num] != null)
				{
					m_turrets[num].KillMe();
					m_turrets[num].TickDownToClear(1f);
					m_turrets[num] = null;
				}
			}
		}

		private int GetNextPos(int curPos)
		{
			List<int> list = new List<int>();
			for (int i = 0; i < 13; i++)
			{
				if (i != curPos)
				{
					list.Add(i);
				}
			}
			list.Shuffle();
			list.Shuffle();
			list.Shuffle();
			list.Shuffle();
			return list[0];
		}

		private void Update()
		{
			if (!m_isSpawning)
			{
				return;
			}
			m_timeTilSpawn -= Time.deltaTime;
			if (!(m_timeTilSpawn <= 0f))
			{
				return;
			}
			m_timeTilSpawn = Random.Range(TickRange.x, TickRange.y);
			if (m_turrets[m_curPos] == null)
			{
				Transform point = Positions[m_curPos];
				m_turrets[m_curPos] = SpawnEnemy(TurretsToSpawn[Random.Range(0, TurretsToSpawn.Count)], point, 3);
				m_curPos = GetNextPos(m_curPos);
				if (m_curPos >= Positions.Count)
				{
					m_curPos = 0;
				}
			}
		}

		private AutoMeater SpawnEnemy(FVRObject o, Transform point, int IFF)
		{
			Vector3 onUnitSphere = Random.onUnitSphere;
			onUnitSphere.y = 0f;
			onUnitSphere.Normalize();
			SM.PlayCoreSound(FVRPooledAudioType.Generic, AudEvent_Dispense, point.position);
			GameObject gameObject = Object.Instantiate(o.GetGameObject(), point.position, Quaternion.LookRotation(onUnitSphere, Vector3.up));
			AutoMeater component = gameObject.GetComponent<AutoMeater>();
			component.E.IFFCode = IFF;
			return component;
		}
	}
}
