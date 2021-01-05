using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class OmniSpawner_IPSC : OmniSpawner
	{
		private OmniSpawnDef_IPSC m_def;

		public GameObject[] PlinthPrefabs;

		public GameObject[] IPSCPrefabs;

		private OmniIPSCPlinth m_plinth;

		private bool m_canSpawn;

		private Vector2 m_spawnCadence = new Vector2(0.25f, 0.25f);

		private float m_spawnTick = 0.25f;

		private int m_targetIndex;

		private List<OmniSpawnDef_IPSC.IPSCType> m_targetList;

		private List<OmniIPSC> m_activeIPSC = new List<OmniIPSC>();

		private List<Transform> m_availableSpawnPoints = new List<Transform>();

		private List<Transform> m_usedSpawnPoints = new List<Transform>();

		public override void Configure(OmniSpawnDef def, OmniWaveEngagementRange range)
		{
			base.Configure(def, range);
			m_def = def as OmniSpawnDef_IPSC;
			m_targetList = m_def.TargetList;
			m_spawnCadence = m_def.SpawnCadence;
			m_spawnTick = 0.01f;
			GameObject gameObject = Object.Instantiate(PlinthPrefabs[(int)range], base.transform.position, base.transform.rotation);
			gameObject.transform.SetParent(base.transform);
			m_plinth = gameObject.GetComponent<OmniIPSCPlinth>();
			for (int i = 0; i < m_plinth.SpawnPoints.Length; i++)
			{
				m_availableSpawnPoints.Add(m_plinth.SpawnPoints[i]);
			}
		}

		public override void BeginSpawning()
		{
			base.BeginSpawning();
			m_canSpawn = true;
		}

		public override void EndSpawning()
		{
			base.EndSpawning();
			m_canSpawn = false;
		}

		public override void Activate()
		{
			base.Activate();
		}

		public override int Deactivate()
		{
			if (m_activeIPSC.Count > 0)
			{
				for (int num = m_activeIPSC.Count - 1; num >= 0; num--)
				{
					if (m_activeIPSC[num] != null)
					{
						Object.Destroy(m_activeIPSC[num].gameObject);
					}
				}
				m_activeIPSC.Clear();
			}
			return base.Deactivate();
		}

		private void Update()
		{
			UpdateMe();
		}

		private void UpdateMe()
		{
			if (m_isConfigured)
			{
				switch (m_state)
				{
				case SpawnerState.Activated:
					SpawningLoop();
					break;
				case SpawnerState.Activating:
					Activating();
					break;
				case SpawnerState.Deactivating:
					Deactivating();
					break;
				}
			}
		}

		private void SpawningLoop()
		{
			if (!m_canSpawn)
			{
				return;
			}
			if (m_spawnTick > 0f)
			{
				m_spawnTick -= Time.deltaTime;
				return;
			}
			if (m_targetIndex < m_targetList.Count)
			{
				if (m_availableSpawnPoints.Count > 0)
				{
					SpawnTarget();
				}
				return;
			}
			m_isDoneSpawning = true;
			if (m_activeIPSC.Count <= 0)
			{
				m_isReadyForWaveEnd = true;
			}
		}

		private void SpawnTarget()
		{
			m_spawnTick = Random.Range(m_spawnCadence.x, m_spawnCadence.y);
			int index = Random.Range(0, m_availableSpawnPoints.Count);
			Transform transform = m_availableSpawnPoints[index];
			Vector3 position = transform.position;
			GameObject gameObject = Object.Instantiate(position: new Vector3(position.x, -3f, position.z), original: IPSCPrefabs[(int)m_targetList[m_targetIndex]], rotation: transform.rotation);
			OmniIPSC component = gameObject.GetComponent<OmniIPSC>();
			m_activeIPSC.Add(component);
			m_usedSpawnPoints.Add(transform);
			m_availableSpawnPoints.Remove(transform);
			component.Configure(this, transform, m_def.TimeActivated);
			m_targetIndex++;
		}

		public void TargetDeactivating(OmniIPSC targ)
		{
			m_usedSpawnPoints.Remove(targ.SpawnPoint);
			m_availableSpawnPoints.Add(targ.SpawnPoint);
			m_activeIPSC.Remove(targ);
			Object.Destroy(targ.gameObject);
		}
	}
}
