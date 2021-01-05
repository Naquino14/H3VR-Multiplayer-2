using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class HG_ModeManager_MeatleGear : HG_ModeManager
	{
		public Transform PlayerSpawnPoint;

		public GameObject GronchMasterPrefab;

		private RonchMaster m_spawnedGronchMaster;

		public Transform GronchMasterSpawnPoint;

		[Header("Audio")]
		public AudioEvent AudEvent_ZoneCompleted;

		public AudioEvent AudEvent_SequenceCompleted;

		private bool m_isGronchAlive;

		private bool m_playerKilledGronch;

		private float m_timer;

		public Transform SpawnPoint_Knife;

		public Transform SpawnPoint_HealthPowerup;

		public Transform SpawnPoint_RegenPowerup;

		public FVRObject Prefab_Knife;

		public FVRObject Prefab_HealthPowerup;

		public FVRObject Prefab_RegenPowerup;

		[Header("LootSpawning")]
		public GameObject LootCrate;

		public ZosigItemSpawnTable SpawnTable;

		public List<Transform> LootboxSpawnPoints;

		private List<GameObject> m_spawnedCrates = new List<GameObject>();

		public Vector3 StripSearch_Center;

		public Vector3 StripSearch_HalfExtents;

		public LayerMask SearchLM;

		public override void InitMode(HG_Mode mode)
		{
			CleanArena();
			m_mode = mode;
			m_playerKilledGronch = false;
			Transform playerSpawnPoint = PlayerSpawnPoint;
			GM.CurrentMovementManager.TeleportToPoint(playerSpawnPoint.position, isAbsolute: true, playerSpawnPoint.forward);
			InitialRespawnPos = GM.CurrentSceneSettings.DeathResetPoint.position;
			InitialRespawnRot = GM.CurrentSceneSettings.DeathResetPoint.rotation;
			GM.CurrentSceneSettings.DeathResetPoint.position = playerSpawnPoint.position;
			GM.CurrentSceneSettings.DeathResetPoint.rotation = playerSpawnPoint.rotation;
			StripEquipment(mode);
			m_isGronchAlive = true;
			m_timer = 0f;
			Object.Instantiate(Prefab_Knife.GetGameObject(), SpawnPoint_Knife.position, SpawnPoint_Knife.rotation);
			Object.Instantiate(Prefab_HealthPowerup.GetGameObject(), SpawnPoint_HealthPowerup.position, SpawnPoint_HealthPowerup.rotation);
			Object.Instantiate(Prefab_RegenPowerup.GetGameObject(), SpawnPoint_RegenPowerup.position, SpawnPoint_RegenPowerup.rotation);
			SM.PlayCoreSound(FVRPooledAudioType.UIChirp, AudEvent_ZoneCompleted, base.transform.position);
			if (mode == HG_Mode.MeatleGear_ScavengingSnake)
			{
				SpawnLoot();
			}
			GameObject gameObject = Object.Instantiate(GronchMasterPrefab, GronchMasterSpawnPoint.position, GronchMasterSpawnPoint.rotation);
			m_spawnedGronchMaster = gameObject.GetComponent<RonchMaster>();
			m_spawnedGronchMaster.SetModeManager(this);
			IsPlaying = true;
		}

		public override void EndMode(bool doesInvokeTeleport, bool immediateTeleportBackAndScore)
		{
			IsPlaying = false;
			GM.CurrentSceneSettings.DeathResetPoint.position = InitialRespawnPos;
			GM.CurrentSceneSettings.DeathResetPoint.rotation = InitialRespawnRot;
			if (m_spawnedCrates.Count > 0)
			{
				for (int num = m_spawnedCrates.Count - 1; num >= 0; num--)
				{
					if (m_spawnedCrates[num] != null)
					{
						Object.Destroy(m_spawnedCrates[num]);
					}
				}
			}
			m_spawnedCrates.Clear();
			base.EndMode(doesInvokeTeleport, immediateTeleportBackAndScore);
		}

		public void GronchDied()
		{
			m_isGronchAlive = false;
			SM.PlayCoreSound(FVRPooledAudioType.UIChirp, AudEvent_SequenceCompleted, base.transform.position);
			m_playerKilledGronch = true;
			M.Case();
			EndMode(doesInvokeTeleport: true, immediateTeleportBackAndScore: false);
		}

		public override void HandlePlayerDeath()
		{
			EndMode(doesInvokeTeleport: false, immediateTeleportBackAndScore: true);
		}

		private void Update()
		{
			if (IsPlaying)
			{
				m_timer += Time.deltaTime;
				if (!m_isGronchAlive)
				{
				}
			}
		}

		private void SpawnLoot()
		{
			LootboxSpawnPoints.Shuffle();
			LootboxSpawnPoints.Shuffle();
			for (int i = 0; i < 12; i++)
			{
				GameObject gameObject = Object.Instantiate(LootCrate, LootboxSpawnPoints[i].position, Random.rotation);
				MM_LootCrate component = gameObject.GetComponent<MM_LootCrate>();
				int index = Random.Range(0, SpawnTable.Objects.Count);
				FVRObject obj = null;
				if (SpawnTable.Objects.Count > 0)
				{
					obj = SpawnTable.Objects[index];
				}
				component.Init(obj, null, null, null);
				m_spawnedCrates.Add(gameObject);
			}
		}

		public override int GetScore()
		{
			int num = 0;
			if (m_playerKilledGronch)
			{
				num = 1;
			}
			return 50000 * num + Mathf.Max((9000 - (int)m_timer) * 30, 0) * num + (int)(GM.CurrentPlayerBody.GetPlayerHealth() * 50000f) * num;
		}

		public override List<string> GetScoringReadOuts()
		{
			if (m_spawnedGronchMaster != null)
			{
				m_spawnedGronchMaster.Dispose();
			}
			int num = 0;
			if (m_playerKilledGronch)
			{
				num = 1;
			}
			List<string> list = new List<string>();
			list.Add("Base Score: " + 50000 * num);
			list.Add("Time Bonus: " + Mathf.Max((9000 - (int)m_timer) * 30, 0) * num);
			list.Add("Health Bonus: " + (int)(GM.CurrentPlayerBody.GetPlayerHealth() * 50000f) * num);
			list.Add("Final Score: " + GetScore());
			return list;
		}

		private void CleanArena()
		{
			Collider[] array = Physics.OverlapBox(StripSearch_Center, StripSearch_HalfExtents, Quaternion.identity, SearchLM, QueryTriggerInteraction.Collide);
			foreach (Collider collider in array)
			{
				if (!(collider.attachedRigidbody != null))
				{
					continue;
				}
				FVRPhysicalObject component = collider.attachedRigidbody.gameObject.GetComponent<FVRPhysicalObject>();
				if (component != null)
				{
					if (component.QuickbeltSlot != null)
					{
						component.ClearQuickbeltState();
					}
					if (component.IsHeld)
					{
						component.ForceBreakInteraction();
					}
					Object.Destroy(component.gameObject);
				}
			}
		}

		private void StripEquipment(HG_Mode mode)
		{
			Collider[] array = Physics.OverlapBox(StripSearch_Center, StripSearch_HalfExtents, Quaternion.identity, SearchLM, QueryTriggerInteraction.Collide);
			foreach (Collider collider in array)
			{
				if (!(collider.attachedRigidbody != null))
				{
					continue;
				}
				FVRPhysicalObject component = collider.attachedRigidbody.gameObject.GetComponent<FVRPhysicalObject>();
				if (!(component != null) || !(component.ObjectWrapper != null))
				{
					continue;
				}
				if (component.ObjectWrapper.Category == FVRObject.ObjectCategory.Powerup)
				{
					if (component.QuickbeltSlot != null)
					{
						component.ClearQuickbeltState();
					}
					if (component.IsHeld)
					{
						component.ForceBreakInteraction();
					}
					Object.Destroy(component.gameObject);
				}
				else if (mode != HG_Mode.MeatleGear_Open && component.ObjectWrapper.Category != FVRObject.ObjectCategory.MeleeWeapon)
				{
					if (component.QuickbeltSlot != null)
					{
						component.ClearQuickbeltState();
					}
					if (component.IsHeld)
					{
						component.ForceBreakInteraction();
					}
					Object.Destroy(component.gameObject);
				}
			}
		}

		public void CleanUpScene()
		{
			FVRFireArmMagazine[] array = Object.FindObjectsOfType<FVRFireArmMagazine>();
			for (int num = array.Length - 1; num >= 0; num--)
			{
				if (!array[num].IsHeld && array[num].QuickbeltSlot == null && array[num].FireArm == null && array[num].m_numRounds == 0)
				{
					Object.Destroy(array[num].gameObject);
				}
			}
			FVRFireArmClip[] array2 = Object.FindObjectsOfType<FVRFireArmClip>();
			for (int num2 = array2.Length - 1; num2 >= 0; num2--)
			{
				if (!array2[num2].IsHeld && array2[num2].QuickbeltSlot == null && array2[num2].FireArm == null && array2[num2].m_numRounds == 0)
				{
					Object.Destroy(array2[num2].gameObject);
				}
			}
			Speedloader[] array3 = Object.FindObjectsOfType<Speedloader>();
			for (int num3 = array3.Length - 1; num3 >= 0; num3--)
			{
				if (!array3[num3].IsHeld && array3[num3].QuickbeltSlot == null)
				{
					Object.Destroy(array3[num3].gameObject);
				}
			}
		}
	}
}
