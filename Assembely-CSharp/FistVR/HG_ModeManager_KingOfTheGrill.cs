using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
	public class HG_ModeManager_KingOfTheGrill : HG_ModeManager
	{
		public Transform PlayerSpawnPoint;

		public List<FVRObject> StartingSpawns;

		public List<Transform> StartingSpawnPoints;

		public ZosigItemSpawnTable StartingMeleeWeapon;

		public Transform StartingMeleeSpot;

		public GameObject Table;

		public List<Text> Labels_Wave;

		public List<Text> Labels_Time;

		[Header("Enemy Spawn Types")]
		public List<SosigEnemyTemplate> SosigEnemyTemplates_Invasion;

		public List<SosigEnemyTemplate> SosigEnemyTemplates_Ressurection;

		public List<SosigEnemyTemplate> SosigEnemyTemplates_Anachronism;

		private List<SosigEnemyTemplate> m_curEnemyTemplateList;

		[Header("Enemy Zones")]
		public List<HG_SpawnPointGroup> GrillSpawnZones;

		public List<Transform> EnemyEndPoints;

		private List<Sosig> m_activeSosigs = new List<Sosig>();

		private List<GameObject> m_spawnedEquipment = new List<GameObject>();

		[Header("LootSpawning")]
		public List<Transform> LootCrateSpawns;

		public GameObject LootCrate;

		public List<ZosigItemSpawnTable> LootTables;

		public FVRObject UnCookedPowerup;

		[Header("Audio")]
		public AudioEvent AudEvent_ZoneCompleted;

		public AudioEvent AudEvent_SequenceCompleted;

		private int m_waveNumber;

		private float m_tickDownToWave = 10f;

		private float m_timer;

		private float m_sosigScore;

		private int m_numSosigsKilled;

		private int m_numCivvieKills;

		private int m_numPowerUpsCooked;

		public Vector3 StripSearch_Center;

		public Vector3 StripSearch_HalfExtents;

		public LayerMask SearchLM;

		private bool m_isWaitingForWave;

		public override void InitMode(HG_Mode mode)
		{
			CleanArena();
			m_mode = mode;
			GM.CurrentSceneSettings.SosigKillEvent += CheckIfDeadSosigWasMine;
			m_activeSosigs.Clear();
			Transform playerSpawnPoint = PlayerSpawnPoint;
			GM.CurrentMovementManager.TeleportToPoint(playerSpawnPoint.position, isAbsolute: true, playerSpawnPoint.forward);
			InitialRespawnPos = GM.CurrentSceneSettings.DeathResetPoint.position;
			InitialRespawnRot = GM.CurrentSceneSettings.DeathResetPoint.rotation;
			GM.CurrentSceneSettings.DeathResetPoint.position = playerSpawnPoint.position;
			GM.CurrentSceneSettings.DeathResetPoint.rotation = playerSpawnPoint.rotation;
			StripEquipment(mode);
			m_waveNumber = 0;
			m_timer = 0f;
			m_sosigScore = 0f;
			m_numSosigsKilled = 0;
			m_numCivvieKills = 0;
			m_numPowerUpsCooked = 0;
			m_tickDownToWave = 10f;
			SM.PlayCoreSound(FVRPooledAudioType.UIChirp, AudEvent_ZoneCompleted, base.transform.position);
			Table.SetActive(value: true);
			for (int i = 0; i < StartingSpawnPoints.Count; i++)
			{
				GameObject item = Object.Instantiate(StartingSpawns[i].GetGameObject(), StartingSpawnPoints[i].position, StartingSpawnPoints[i].rotation);
				m_spawnedEquipment.Add(item);
			}
			Object.Instantiate(StartingMeleeWeapon.Objects[Random.Range(0, StartingMeleeWeapon.Objects.Count)].GetGameObject(), StartingMeleeSpot.position, StartingMeleeSpot.rotation);
			switch (mode)
			{
			case HG_Mode.KingOfTheGrill_Invasion:
				m_curEnemyTemplateList = SosigEnemyTemplates_Invasion;
				break;
			case HG_Mode.KingOfTheGrill_Resurrection:
				m_curEnemyTemplateList = SosigEnemyTemplates_Ressurection;
				break;
			case HG_Mode.KingOfTheGrill_Anachronism:
				m_curEnemyTemplateList = SosigEnemyTemplates_Anachronism;
				break;
			}
			IsPlaying = true;
			m_tickDownToWave = 59f;
			m_isWaitingForWave = true;
			SpawnLoot();
		}

		public override void HandlePlayerDeath()
		{
			EndMode(doesInvokeTeleport: false, immediateTeleportBackAndScore: true);
		}

		public void Update()
		{
			if (IsPlaying)
			{
				for (int i = 0; i < Labels_Wave.Count; i++)
				{
					Labels_Wave[i].text = "Wave - " + (m_waveNumber + 1);
				}
				for (int j = 0; j < Labels_Time.Count; j++)
				{
					Labels_Time[j].text = FloatToTime(m_tickDownToWave, "00.00");
				}
			}
			if (IsPlaying && m_isWaitingForWave)
			{
				m_tickDownToWave -= Time.deltaTime;
				if (m_tickDownToWave <= 0f)
				{
					SpawnWave();
				}
			}
		}

		public override void EndMode(bool doesInvokeTeleport, bool immediateTeleportBackAndScore)
		{
			IsPlaying = false;
			Table.SetActive(value: false);
			if (m_activeSosigs.Count > 0)
			{
				for (int num = m_activeSosigs.Count - 1; num >= 0; num--)
				{
					if (m_activeSosigs[num] != null)
					{
						m_activeSosigs[num].ClearSosig();
					}
				}
			}
			m_activeSosigs.Clear();
			for (int num2 = m_spawnedEquipment.Count - 1; num2 >= 0; num2--)
			{
				Object.Destroy(m_spawnedEquipment[num2]);
			}
			m_spawnedEquipment.Clear();
			GM.CurrentSceneSettings.SosigKillEvent -= CheckIfDeadSosigWasMine;
			GM.CurrentSceneSettings.DeathResetPoint.position = InitialRespawnPos;
			GM.CurrentSceneSettings.DeathResetPoint.rotation = InitialRespawnRot;
			base.EndMode(doesInvokeTeleport, immediateTeleportBackAndScore);
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
				if (component != null && component.ObjectWrapper != null && component.ObjectWrapper.Category != FVRObject.ObjectCategory.MeleeWeapon)
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

		private void SpawnEnemy(SosigEnemyTemplate t, Transform point, int IFF, bool IsCivvie)
		{
			GameObject weaponPrefab = null;
			if (t.WeaponOptions.Count > 0)
			{
				weaponPrefab = t.WeaponOptions[Random.Range(0, t.WeaponOptions.Count)].GetGameObject();
			}
			GameObject weaponPrefab2 = null;
			if (t.WeaponOptions_Secondary.Count > 0)
			{
				weaponPrefab2 = t.WeaponOptions_Secondary[Random.Range(0, t.WeaponOptions_Secondary.Count)].GetGameObject();
			}
			Sosig item = SpawnEnemySosig(t.SosigPrefabs[Random.Range(0, t.SosigPrefabs.Count)].GetGameObject(), weaponPrefab, weaponPrefab2, point.position, point.rotation, t.ConfigTemplates[Random.Range(0, t.ConfigTemplates.Count)], t.OutfitConfig[Random.Range(0, t.OutfitConfig.Count)], IFF, IsCivvie);
			m_activeSosigs.Add(item);
		}

		private Sosig SpawnEnemySosig(GameObject prefab, GameObject weaponPrefab, GameObject weaponPrefab2, Vector3 pos, Quaternion rot, SosigConfigTemplate t, SosigOutfitConfig w, int IFF, bool ShouldWander)
		{
			GameObject gameObject = Object.Instantiate(prefab, pos, rot);
			Sosig componentInChildren = gameObject.GetComponentInChildren<Sosig>();
			componentInChildren.Configure(t);
			componentInChildren.E.IFFCode = IFF;
			if (weaponPrefab != null)
			{
				SosigWeapon component = Object.Instantiate(weaponPrefab, pos + Vector3.up * 0.1f, rot).GetComponent<SosigWeapon>();
				component.SetAutoDestroy(b: true);
				component.O.SpawnLockable = false;
				if (component.Type == SosigWeapon.SosigWeaponType.Gun)
				{
					component.FlightVelocityMultiplier = 0.15f;
					componentInChildren.Inventory.FillAmmoWithType(component.AmmoType);
				}
				componentInChildren.Inventory.FillAllAmmo();
				if (component != null)
				{
					componentInChildren.InitHands();
					componentInChildren.ForceEquip(component);
				}
				if (weaponPrefab2 != null)
				{
					SosigWeapon component2 = Object.Instantiate(weaponPrefab2, pos + Vector3.up * 0.1f, rot).GetComponent<SosigWeapon>();
					component2.SetAutoDestroy(b: true);
					component2.O.SpawnLockable = false;
					if (component2.Type == SosigWeapon.SosigWeaponType.Gun)
					{
						component2.FlightVelocityMultiplier = 0.15f;
						componentInChildren.Inventory.FillAmmoWithType(component2.AmmoType);
					}
					if (component2 != null)
					{
						componentInChildren.ForceEquip(component2);
					}
				}
			}
			float num = 0f;
			num = Random.Range(0f, 1f);
			if (num < w.Chance_Headwear)
			{
				SpawnAccesoryToLink(w.Headwear, componentInChildren.Links[0]);
			}
			num = Random.Range(0f, 1f);
			if (num < w.Chance_Facewear)
			{
				SpawnAccesoryToLink(w.Facewear, componentInChildren.Links[0]);
			}
			num = Random.Range(0f, 1f);
			if (num < w.Chance_Torsowear)
			{
				SpawnAccesoryToLink(w.Torsowear, componentInChildren.Links[1]);
			}
			num = Random.Range(0f, 1f);
			if (num < w.Chance_Pantswear)
			{
				SpawnAccesoryToLink(w.Pantswear, componentInChildren.Links[2]);
			}
			num = Random.Range(0f, 1f);
			if (num < w.Chance_Pantswear_Lower)
			{
				SpawnAccesoryToLink(w.Pantswear_Lower, componentInChildren.Links[3]);
			}
			num = Random.Range(0f, 1f);
			if (num < w.Chance_Backpacks)
			{
				SpawnAccesoryToLink(w.Backpacks, componentInChildren.Links[1]);
			}
			if (t.UsesLinkSpawns)
			{
				for (int i = 0; i < componentInChildren.Links.Count; i++)
				{
					float num2 = Random.Range(0f, 1f);
					if (num2 < t.LinkSpawnChance[i])
					{
						componentInChildren.Links[i].RegisterSpawnOnDestroy(t.LinkSpawns[i]);
					}
				}
			}
			if (ShouldWander)
			{
				componentInChildren.CurrentOrder = Sosig.SosigOrder.Wander;
				componentInChildren.FallbackOrder = Sosig.SosigOrder.Wander;
			}
			else
			{
				componentInChildren.CurrentOrder = Sosig.SosigOrder.Assault;
				componentInChildren.FallbackOrder = Sosig.SosigOrder.Assault;
				componentInChildren.CommandAssaultPoint(EnemyEndPoints[Random.Range(0, EnemyEndPoints.Count)].position);
			}
			componentInChildren.SetDominantGuardDirection(Random.onUnitSphere);
			return componentInChildren;
		}

		private void SpawnAccesoryToLink(List<FVRObject> gs, SosigLink l)
		{
			GameObject gameObject = Object.Instantiate(gs[Random.Range(0, gs.Count)].GetGameObject(), l.transform.position, l.transform.rotation);
			gameObject.transform.SetParent(l.transform);
			SosigWearable component = gameObject.GetComponent<SosigWearable>();
			component.RegisterWearable(l);
		}

		public void CheckIfDeadSosigWasMine(Sosig s)
		{
			if (m_activeSosigs.Contains(s))
			{
				if (IsPlaying && s.GetDiedFromIFF() == GM.CurrentPlayerBody.GetPlayerIFF())
				{
					ScoreSosid(s);
				}
				s.TickDownToClear(5f);
				m_activeSosigs.Remove(s);
			}
			if (m_activeSosigs.Count < 1 && IsPlaying && !m_isWaitingForWave)
			{
				AdvanceWave();
			}
		}

		private void SpawnWave()
		{
			m_isWaitingForWave = false;
			HG_SpawnPointGroup hG_SpawnPointGroup = GrillSpawnZones[m_waveNumber];
			SM.PlayCoreSound(FVRPooledAudioType.UIChirp, AudEvent_ZoneCompleted, base.transform.position);
			for (int i = 0; i < hG_SpawnPointGroup.Points.Count; i++)
			{
				if (m_mode == HG_Mode.KingOfTheGrill_Anachronism)
				{
					float num = Random.Range(0f, 1f);
					if (num < 0.3f)
					{
						SpawnEnemy(SosigEnemyTemplates_Ressurection[m_waveNumber], hG_SpawnPointGroup.Points[i], 1, IsCivvie: false);
					}
					else if (num < 0.5f)
					{
						SpawnEnemy(SosigEnemyTemplates_Anachronism[m_waveNumber], hG_SpawnPointGroup.Points[i], 1, IsCivvie: false);
					}
					else
					{
						SpawnEnemy(SosigEnemyTemplates_Invasion[m_waveNumber], hG_SpawnPointGroup.Points[i], 1, IsCivvie: false);
					}
					continue;
				}
				int waveNumber = m_waveNumber;
				int num2 = m_waveNumber - 2;
				if (num2 < 0)
				{
					num2 = 0;
				}
				int index = Random.Range(num2, waveNumber + 1);
				SpawnEnemy(m_curEnemyTemplateList[index], hG_SpawnPointGroup.Points[i], 1, IsCivvie: false);
			}
		}

		private void SpawnLoot()
		{
			int index = Random.Range(0, LootCrateSpawns.Count);
			GameObject gameObject = Object.Instantiate(LootCrate, LootCrateSpawns[index].position, Random.rotation);
			MM_LootCrate component = gameObject.GetComponent<MM_LootCrate>();
			int waveNumber = m_waveNumber;
			if (waveNumber >= LootTables.Count)
			{
				component.Init(null, null, UnCookedPowerup, UnCookedPowerup);
				return;
			}
			int index2 = Random.Range(0, LootTables[waveNumber].Objects.Count);
			FVRObject obj = null;
			if (LootTables[waveNumber].Objects.Count > 0)
			{
				obj = LootTables[waveNumber].Objects[index2];
			}
			component.Init(obj, null, UnCookedPowerup, UnCookedPowerup);
		}

		private void AdvanceWave()
		{
			m_waveNumber++;
			if (m_waveNumber >= 15)
			{
				SM.PlayCoreSound(FVRPooledAudioType.UIChirp, AudEvent_SequenceCompleted, base.transform.position);
				M.Case();
				EndMode(doesInvokeTeleport: true, immediateTeleportBackAndScore: false);
			}
			else
			{
				CleanUpScene();
				SM.PlayCoreSound(FVRPooledAudioType.UIChirp, AudEvent_ZoneCompleted, base.transform.position);
				m_isWaitingForWave = true;
				m_tickDownToWave = 59f;
				SpawnLoot();
			}
		}

		public void DepositPowerUp(PowerupType type)
		{
			m_numPowerUpsCooked++;
			switch (type)
			{
			case PowerupType.Blort:
				break;
			case PowerupType.Health:
				m_numPowerUpsCooked += 3;
				break;
			case PowerupType.QuadDamage:
				break;
			case PowerupType.InfiniteAmmo:
				m_numPowerUpsCooked += 2;
				break;
			case PowerupType.FarOutMeat:
				break;
			case PowerupType.MuscleMeat:
				break;
			case PowerupType.SnakeEye:
				break;
			case PowerupType.Regen:
				m_numPowerUpsCooked += 3;
				break;
			case PowerupType.Cyclops:
				break;
			case PowerupType.Invincibility:
			case PowerupType.Ghosted:
			case PowerupType.HomeTown:
				break;
			}
		}

		private void ScoreSosid(Sosig s)
		{
			m_numSosigsKilled++;
			float num = 5f;
			int diedFromIFF = s.GetDiedFromIFF();
			if (diedFromIFF == GM.CurrentPlayerBody.GetPlayerIFF())
			{
				if (s.GetDiedFromHeadShot())
				{
					num += 15f;
				}
				Damage.DamageClass diedFromClass = s.GetDiedFromClass();
				Sosig.SosigDeathType diedFromType = s.GetDiedFromType();
				switch (diedFromClass)
				{
				case Damage.DamageClass.Environment:
					num += 30f;
					break;
				case Damage.DamageClass.Melee:
					num += 25f;
					break;
				case Damage.DamageClass.Projectile:
					num += 10f;
					break;
				case Damage.DamageClass.Explosive:
					num += 5f;
					break;
				}
				switch (diedFromType)
				{
				case Sosig.SosigDeathType.BleedOut:
					num += 10f;
					break;
				case Sosig.SosigDeathType.JointBreak:
					num += 15f;
					break;
				case Sosig.SosigDeathType.JointExplosion:
					num += 5f;
					break;
				case Sosig.SosigDeathType.JointSever:
					num += 35f;
					break;
				case Sosig.SosigDeathType.JointPullApart:
					num += 15f;
					break;
				}
			}
			m_sosigScore += num;
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

		public override int GetScore()
		{
			float num = 0f;
			if (m_numSosigsKilled > 0)
			{
				num = m_sosigScore / (float)m_numSosigsKilled;
			}
			return m_numSosigsKilled * 1000 + (int)num * 500 + m_numPowerUpsCooked * 6000;
		}

		public override List<string> GetScoringReadOuts()
		{
			float num = 0f;
			if (m_numSosigsKilled > 0)
			{
				num = m_sosigScore / (float)m_numSosigsKilled;
			}
			List<string> list = new List<string>();
			list.Add("Base Score: " + m_numPowerUpsCooked * 6000);
			list.Add("Sosig Bonus: " + m_numSosigsKilled * 1000);
			list.Add("Style Bonus: " + (int)num * 500);
			list.Add("Final Score: " + GetScore());
			return list;
		}

		public string FloatToTime(float toConvert, string format)
		{
			return format switch
			{
				"00.0" => $"{Mathf.Floor(toConvert) % 60f:00}:{Mathf.Floor(toConvert * 10f % 10f):0}", 
				"#0.0" => $"{Mathf.Floor(toConvert) % 60f:#0}:{Mathf.Floor(toConvert * 10f % 10f):0}", 
				"00.00" => $"{Mathf.Floor(toConvert) % 60f:00}:{Mathf.Floor(toConvert * 100f % 100f):00}", 
				"00.000" => $"{Mathf.Floor(toConvert) % 60f:00}:{Mathf.Floor(toConvert * 1000f % 1000f):000}", 
				"#00.000" => $"{Mathf.Floor(toConvert) % 60f:#00}:{Mathf.Floor(toConvert * 1000f % 1000f):000}", 
				"#0:00" => $"{Mathf.Floor(toConvert / 60f):#0}:{Mathf.Floor(toConvert) % 60f:00}", 
				"#00:00" => $"{Mathf.Floor(toConvert / 60f):#00}:{Mathf.Floor(toConvert) % 60f:00}", 
				"0:00.0" => $"{Mathf.Floor(toConvert / 60f):0}:{Mathf.Floor(toConvert) % 60f:00}.{Mathf.Floor(toConvert * 10f % 10f):0}", 
				"#0:00.0" => $"{Mathf.Floor(toConvert / 60f):#0}:{Mathf.Floor(toConvert) % 60f:00}.{Mathf.Floor(toConvert * 10f % 10f):0}", 
				"0:00.00" => $"{Mathf.Floor(toConvert / 60f):0}:{Mathf.Floor(toConvert) % 60f:00}.{Mathf.Floor(toConvert * 100f % 100f):00}", 
				"#0:00.00" => $"{Mathf.Floor(toConvert / 60f):#0}:{Mathf.Floor(toConvert) % 60f:00}.{Mathf.Floor(toConvert * 100f % 100f):00}", 
				"0:00.000" => $"{Mathf.Floor(toConvert / 60f):0}:{Mathf.Floor(toConvert) % 60f:00}.{Mathf.Floor(toConvert * 1000f % 1000f):000}", 
				"#0:00.000" => $"{Mathf.Floor(toConvert / 60f):#0}:{Mathf.Floor(toConvert) % 60f:00}.{Mathf.Floor(toConvert * 1000f % 1000f):000}", 
				_ => "error", 
			};
		}
	}
}
