using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class HG_ModeManager_TargetRelay : HG_ModeManager
	{
		public List<GameObject> Targets;

		public GameObject Indicator;

		private GameObject m_spawnedIndicator;

		private Vector3 m_spawnedIndicatorBasePos;

		public List<int> ZoneSequence_Sprint;

		public List<int> ZoneSequence_Jog;

		public List<int> ZoneSequence_Marathon;

		public List<Transform> CivvieSpawnPoints;

		private List<int> m_curZoneSequence;

		private int m_curZoneInSequence;

		private float m_timer;

		private int m_numShotsFired;

		private int m_numTargetsDestroyedByGunFire;

		private List<HG_Target> m_activeTargets = new List<HG_Target>();

		[Header("Audio")]
		public AudioEvent AudEvent_ZoneCompleted;

		public AudioEvent AudEvent_SequenceCompleted;

		public override void InitMode(HG_Mode mode)
		{
			m_mode = mode;
			GM.CurrentSceneSettings.ShotFiredEvent += GunShotFired;
			m_activeTargets.Clear();
			switch (mode)
			{
			case HG_Mode.TargetRelay_Sprint:
				m_curZoneSequence = ZoneSequence_Sprint;
				break;
			case HG_Mode.TargetRelay_Jog:
				m_curZoneSequence = ZoneSequence_Jog;
				break;
			case HG_Mode.TargetRelay_Marathon:
				m_curZoneSequence = ZoneSequence_Marathon;
				break;
			}
			m_curZoneInSequence = 0;
			Transform playerSpawnPoint = M.Zones[m_curZoneSequence[m_curZoneInSequence]].PlayerSpawnPoint;
			GM.CurrentMovementManager.TeleportToPoint(playerSpawnPoint.position, isAbsolute: true, playerSpawnPoint.forward);
			InitialRespawnPos = GM.CurrentSceneSettings.DeathResetPoint.position;
			InitialRespawnRot = GM.CurrentSceneSettings.DeathResetPoint.rotation;
			GM.CurrentSceneSettings.DeathResetPoint.position = playerSpawnPoint.position;
			GM.CurrentSceneSettings.DeathResetPoint.rotation = playerSpawnPoint.rotation;
			m_curZoneInSequence++;
			ConfigureCurrentZone();
			SM.PlayCoreSound(FVRPooledAudioType.UIChirp, AudEvent_ZoneCompleted, base.transform.position);
			m_timer = 0f;
			m_numShotsFired = 0;
			m_numTargetsDestroyedByGunFire = 0;
			IsPlaying = true;
		}

		public override int GetScore()
		{
			float num = 0f;
			if (m_numShotsFired > 0)
			{
				num = (float)m_numTargetsDestroyedByGunFire / (float)m_numShotsFired;
			}
			return (m_curZoneSequence.Count - 1) * 500 + Mathf.Max((m_curZoneSequence.Count * 30 - (int)m_timer) * 10, 0) + (int)((float)(m_curZoneSequence.Count - 1) * num * 500f);
		}

		public override List<string> GetScoringReadOuts()
		{
			List<string> list = new List<string>();
			list.Add("Base Score: " + (m_curZoneSequence.Count - 1) * 500);
			list.Add("Time Bonus: " + Mathf.Max((m_curZoneSequence.Count * 30 - (int)m_timer) * 10, 0));
			float num = 0f;
			if (m_numShotsFired > 0)
			{
				num = (float)m_numTargetsDestroyedByGunFire / (float)m_numShotsFired;
			}
			list.Add("Accuracy Bonus: " + (int)((float)(m_curZoneSequence.Count - 1) * num * 500f));
			list.Add("Final Score: " + GetScore());
			return list;
		}

		public void Update()
		{
			if (IsPlaying)
			{
				m_timer += Time.deltaTime;
				if (m_spawnedIndicator != null)
				{
					m_spawnedIndicator.transform.position = m_spawnedIndicatorBasePos + Vector3.up * (Mathf.Sin(Time.time * 4f) * 1.5f);
				}
			}
		}

		private bool AreTargetsActive()
		{
			if (m_activeTargets.Count > 0)
			{
				return true;
			}
			return false;
		}

		private void ConfigureCurrentZone()
		{
			HG_Zone hG_Zone = M.Zones[m_curZoneSequence[m_curZoneInSequence]];
			hG_Zone.TargetPoints.Shuffle();
			hG_Zone.TargetPoints.Shuffle();
			for (int i = 0; i < 5; i++)
			{
				GameObject gameObject = Object.Instantiate(Targets[Random.Range(0, Targets.Count)], hG_Zone.TargetPoints[i].position + Vector3.up * Random.Range(1.5f, 2.5f), Random.rotation);
				HG_Target component = gameObject.GetComponent<HG_Target>();
				component.Init(this, hG_Zone);
				m_activeTargets.Add(component);
			}
			m_spawnedIndicator = Object.Instantiate(Indicator, hG_Zone.transform.position + Vector3.up * 14f, Quaternion.identity);
			m_spawnedIndicatorBasePos = hG_Zone.transform.position + Vector3.up * 14f;
			SM.PlayCoreSound(FVRPooledAudioType.UIChirp, AudEvent_ZoneCompleted, hG_Zone.transform.position);
		}

		private void DeactivateCurrentZone()
		{
			if (m_activeTargets.Count > 0)
			{
				for (int num = m_activeTargets.Count - 1; num >= 0; num--)
				{
					Object.Destroy(m_activeTargets[num].gameObject);
				}
				m_activeTargets.Clear();
			}
			if (m_spawnedIndicator != null)
			{
				Object.Destroy(m_spawnedIndicator);
			}
		}

		public override void EndMode(bool doesInvokeTeleport, bool immediateTeleportBackAndScore)
		{
			DeactivateCurrentZone();
			GM.CurrentSceneSettings.ShotFiredEvent -= GunShotFired;
			GM.CurrentSceneSettings.DeathResetPoint.position = InitialRespawnPos;
			GM.CurrentSceneSettings.DeathResetPoint.rotation = InitialRespawnRot;
			IsPlaying = false;
			base.EndMode(doesInvokeTeleport: true, immediateTeleportBackAndScore: false);
		}

		public void GunShotFired(FVRFireArm firearm)
		{
			m_numShotsFired++;
		}

		public override void TargetDestroyed(HG_Target t)
		{
			if (m_activeTargets.Contains(t))
			{
				m_activeTargets.Remove(t);
			}
			if (t.GetClassThatKilledMe() == Damage.DamageClass.Projectile)
			{
				m_numTargetsDestroyedByGunFire++;
			}
			if (!AreTargetsActive())
			{
				DeactivateCurrentZone();
				m_curZoneInSequence++;
				if (m_curZoneInSequence < m_curZoneSequence.Count)
				{
					ConfigureCurrentZone();
					return;
				}
				M.Case();
				SM.PlayCoreSound(FVRPooledAudioType.UIChirp, AudEvent_SequenceCompleted, base.transform.position);
				EndMode(doesInvokeTeleport: true, immediateTeleportBackAndScore: false);
			}
		}
	}
}
