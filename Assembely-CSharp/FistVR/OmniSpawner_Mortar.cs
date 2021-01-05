using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class OmniSpawner_Mortar : OmniSpawner
	{
		private OmniSpawnDef_Mortar m_def;

		public GameObject MortarPrefab;

		private bool m_canSpawn;

		private Vector2 m_spawnCadence = new Vector2(0.25f, 0.25f);

		private float m_spawnTick = 1f;

		private Vector2 m_velocityRange = new Vector2(10f, 10f);

		private int m_targetsLeftToFire;

		private List<GameObject> m_spawnedMortars = new List<GameObject>();

		private OmniSpawnDef_Mortar.MortarAngle m_angle;

		private Vector3 m_curPos = Vector3.zero;

		private Vector3 m_tarPos = Vector3.zero;

		private Quaternion m_curRot = Quaternion.identity;

		private Quaternion m_tarRot = Quaternion.identity;

		private float m_elevation;

		private float m_dist;

		public override void Configure(OmniSpawnDef def, OmniWaveEngagementRange range)
		{
			base.Configure(def, range);
			m_def = def as OmniSpawnDef_Mortar;
			m_spawnCadence = m_def.SpawnCadence;
			m_velocityRange = m_def.VelocityRange;
			m_targetsLeftToFire = m_def.NumShots;
			m_angle = m_def.Angle;
			m_elevation = base.transform.position.y;
			m_dist = base.transform.position.z;
		}

		public override void BeginSpawning()
		{
			base.BeginSpawning();
			m_canSpawn = true;
			m_curPos = base.transform.position;
			m_tarPos = base.transform.position;
			m_curRot = base.transform.rotation;
			m_tarRot = base.transform.rotation;
			GenerateNewPose();
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
			if (m_spawnedMortars.Count > 0)
			{
				for (int num = m_spawnedMortars.Count - 1; num >= 0; num--)
				{
					if (m_spawnedMortars[num] != null)
					{
						Object.Destroy(m_spawnedMortars[num]);
					}
				}
				m_spawnedMortars.Clear();
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
			if (!m_canSpawn || m_state != SpawnerState.Activated)
			{
				return;
			}
			m_curPos = Vector3.Lerp(m_curPos, m_tarPos, Time.deltaTime * 2f);
			m_curRot = Quaternion.Slerp(m_curRot, m_tarRot, Time.deltaTime * 10f);
			base.transform.position = m_curPos;
			base.transform.rotation = m_curRot;
			if (m_spawnTick > 0f)
			{
				m_spawnTick -= Time.deltaTime;
				return;
			}
			if (m_targetsLeftToFire > 0)
			{
				SpawnMortar();
				return;
			}
			m_isDoneSpawning = true;
			if (m_spawnedMortars.Count <= 0)
			{
				m_isReadyForWaveEnd = true;
			}
		}

		private void GenerateNewPose()
		{
			Vector3 position = base.transform.position;
			Vector3 position2 = base.transform.position;
			position.x = Random.Range(15f, -15f);
			position2.x = 0f - position2.x;
			position2.x += Random.Range(5f, -5f);
			position2.y = base.transform.position.y + 45f;
			switch (m_angle)
			{
			case OmniSpawnDef_Mortar.MortarAngle.DownRange:
				position2.z = 35f;
				position2.z += Random.Range(0f, 10f);
				break;
			case OmniSpawnDef_Mortar.MortarAngle.Centered:
				position2.z = 1f;
				break;
			case OmniSpawnDef_Mortar.MortarAngle.UpRange:
				position2.z = -35f;
				position2.z -= Random.Range(0f, 10f);
				break;
			}
			m_tarPos = position;
			m_tarRot = Quaternion.LookRotation(position2, Vector3.up);
		}

		private void SpawnMortar()
		{
			m_spawnTick = Random.Range(m_spawnCadence.x, m_spawnCadence.y);
			GameObject gameObject = Object.Instantiate(MortarPrefab, base.transform.position, Quaternion.identity);
			gameObject.transform.localScale = new Vector3(m_def.MortarSize, m_def.MortarSize, m_def.MortarSize);
			OmniMortar component = gameObject.GetComponent<OmniMortar>();
			m_spawnedMortars.Add(gameObject);
			component.Configure(this, base.transform.position, base.transform.forward, Random.Range(m_velocityRange.x, m_velocityRange.y));
			GenerateNewPose();
			PlaySpawnSound();
			m_targetsLeftToFire--;
		}

		public void MortarExpired(OmniMortar mortar)
		{
			m_spawnedMortars.Remove(mortar.gameObject);
			Object.Destroy(mortar.gameObject);
		}

		public void MortarHit(OmniMortar mortar)
		{
			m_spawnedMortars.Remove(mortar.gameObject);
			Invoke("PlaySuccessSound", 0.15f);
			AddPoints(100);
			Object.Destroy(mortar.gameObject);
		}
	}
}
