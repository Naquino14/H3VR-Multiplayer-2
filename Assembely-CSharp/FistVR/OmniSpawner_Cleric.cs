using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class OmniSpawner_Cleric : OmniSpawner
	{
		private OmniSpawnDef_Cleric m_def;

		public GameObject TargetPrefab;

		private bool m_canSpawn;

		private int m_setIndex;

		private float m_timeBetweenSets = 1f;

		private float m_timeTilNextSet = 1f;

		private bool m_waitingForNextSet;

		private OmniSpawnDef_Cleric.ClericSet m_curSet;

		private int m_clericIndex;

		private float m_timeTilNextCleric = 1f;

		private bool m_shouldSpawnNextOnHitCleric = true;

		private List<int> m_randIndexes;

		private List<OmniCleric> m_activeClerics = new List<OmniCleric>();

		public GameObject ClericRingPrefab;

		private OmniClericRing m_ring;

		public override void Configure(OmniSpawnDef Def, OmniWaveEngagementRange Range)
		{
			base.Configure(Def, Range);
			m_def = Def as OmniSpawnDef_Cleric;
			m_timeBetweenSets = m_def.TimeBetweenSets;
			m_timeTilNextSet = 0f;
			m_curSet = m_def.Sets[0];
			m_clericIndex = 0;
			m_timeTilNextCleric = 0f;
			m_shouldSpawnNextOnHitCleric = true;
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
			if (m_ring == null)
			{
				GameObject gameObject = Object.Instantiate(ClericRingPrefab, Vector3.zero, Quaternion.identity);
				m_ring = gameObject.GetComponent<OmniClericRing>();
				for (int i = 0; i < m_ring.Indicators.Count; i++)
				{
					m_ring.Indicators[i].enabled = false;
				}
			}
		}

		public override int Deactivate()
		{
			if (m_activeClerics.Count > 0)
			{
				for (int num = m_activeClerics.Count - 1; num >= 0; num--)
				{
					if (m_activeClerics[num] != null)
					{
						Object.Destroy(m_activeClerics[num].gameObject);
					}
				}
				m_activeClerics.Clear();
			}
			if (m_ring != null)
			{
				Object.Destroy(m_ring.gameObject);
				m_ring = null;
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
			if (m_canSpawn && !m_waitingForNextSet)
			{
				OmniSpawnDef_Cleric.TargetSpawnStyle spawnStyle = m_curSet.SpawnStyle;
				if (spawnStyle != OmniSpawnDef_Cleric.TargetSpawnStyle.AllAtOnce)
				{
					switch (spawnStyle)
					{
					case OmniSpawnDef_Cleric.TargetSpawnStyle.OnHit:
						if (m_clericIndex < m_curSet.TargetSet.Count && m_shouldSpawnNextOnHitCleric)
						{
							m_shouldSpawnNextOnHitCleric = false;
							SpawnCleric(m_clericIndex, m_curSet.TargetSet[m_clericIndex]);
						}
						break;
					case OmniSpawnDef_Cleric.TargetSpawnStyle.Sequential:
						if (m_clericIndex < m_curSet.TargetSet.Count)
						{
							if (m_timeTilNextCleric <= 0f)
							{
								SpawnCleric(m_clericIndex, m_curSet.TargetSet[m_clericIndex]);
								m_timeTilNextCleric = m_curSet.SequentialTiming;
							}
							else
							{
								m_timeTilNextCleric -= Time.deltaTime;
							}
						}
						break;
					}
				}
				else
				{
					while (m_clericIndex < m_curSet.TargetSet.Count)
					{
						SpawnCleric(m_clericIndex, m_curSet.TargetSet[m_clericIndex]);
					}
				}
			}
			if (m_waitingForNextSet)
			{
				if (m_timeTilNextSet > 0f)
				{
					m_timeTilNextSet -= Time.deltaTime;
				}
				else
				{
					m_timeTilNextSet = 0f;
					m_waitingForNextSet = false;
				}
			}
			if (m_clericIndex < m_curSet.TargetSet.Count || m_activeClerics.Count > 0)
			{
				return;
			}
			if (m_setIndex < m_def.Sets.Count - 1)
			{
				m_clericIndex = 0;
				m_setIndex++;
				m_curSet = m_def.Sets[m_setIndex];
				m_waitingForNextSet = true;
				m_timeTilNextSet = m_timeBetweenSets;
				m_timeTilNextCleric = m_curSet.SequentialTiming;
			}
			else
			{
				m_isDoneSpawning = true;
				if (m_activeClerics.Count <= 0)
				{
					m_isReadyForWaveEnd = true;
				}
			}
		}

		public void SpawnCleric(int index, OmniSpawnDef_Cleric.TargetLocation loc)
		{
			GameObject gameObject = Object.Instantiate(TargetPrefab, m_ring.SpawnPoints[(int)loc].position, m_ring.SpawnPoints[(int)loc].rotation);
			OmniCleric component = gameObject.GetComponent<OmniCleric>();
			component.Init(this, m_ring.SpawnPoints[(int)loc], m_curSet.FiresBack, m_curSet.FiringTime, loc);
			m_activeClerics.Add(component);
			m_clericIndex++;
			PlaySpawnSound();
			m_ring.Indicators[(int)loc].enabled = true;
		}

		public void ClearCleric(OmniCleric cleric, bool isHeadShot)
		{
			m_ring.Indicators[(int)cleric.m_pos].enabled = false;
			Vector2 vector = new Vector2(GM.CurrentPlayerBody.Head.position.x, GM.CurrentPlayerBody.Head.position.z);
			if (vector.magnitude > 1.3f)
			{
				PlayFailureSound();
				AddPoints(-200);
			}
			else
			{
				PlaySuccessSound();
				if (isHeadShot)
				{
					AddPoints(200);
				}
				else
				{
					AddPoints(100);
				}
			}
			m_activeClerics.Remove(cleric);
			m_shouldSpawnNextOnHitCleric = true;
			Object.Destroy(cleric.gameObject);
		}
	}
}
