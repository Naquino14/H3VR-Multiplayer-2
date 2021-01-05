using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class HG_ModeManager_AssaultNPepper : HG_ModeManager
	{
		public List<SosigEnemyTemplate> Templates_Enemy;

		public SosigEnemyTemplate Template_Civvie;

		public GameObject SpawningCore_Prefab;

		public GameObject Indicator_Prefab;

		public List<int> ZoneSequence_Skirmish;

		public List<int> ZoneSequence_Brawl;

		public List<int> ZoneSequence_Maelstrom;

		public List<Transform> CivvieSpawnPoints;

		private List<int> m_curZoneSequence;

		private int m_curZoneInSequence;

		private int m_numZonesActiveAtOnce;

		private List<HG_Target> m_activeTargets = new List<HG_Target>();

		private List<HG_Zone> m_activeSpawningZones = new List<HG_Zone>();

		private List<Sosig> m_activeSosigs = new List<Sosig>();

		private List<Sosig> m_civvieSosigs = new List<Sosig>();

		private int m_maxSosigs = 8;

		private float m_TickToSpawn = 20f;

		private int m_difficulty;

		[Header("Audio")]
		public AudioEvent AudEvent_ZoneCompleted;

		public AudioEvent AudEvent_SequenceCompleted;

		private float m_timer;

		private float m_sosigScore;

		private int m_numSosigsKilled;

		private int m_numCivvieKills;

		public override void InitMode(HG_Mode mode)
		{
			m_mode = mode;
			GM.CurrentSceneSettings.SosigKillEvent += CheckIfDeadSosigWasMine;
			m_activeTargets.Clear();
			m_activeSpawningZones.Clear();
			switch (mode)
			{
			case HG_Mode.AssaultNPepper_Skirmish:
				m_curZoneSequence = ZoneSequence_Skirmish;
				break;
			case HG_Mode.AssaultNPepper_Brawl:
				m_curZoneSequence = ZoneSequence_Brawl;
				break;
			case HG_Mode.AssaultNPepper_Maelstrom:
				m_curZoneSequence = ZoneSequence_Maelstrom;
				break;
			case HG_Mode.MeatNMetal_Neophyte:
				m_curZoneSequence = ZoneSequence_Skirmish;
				break;
			case HG_Mode.MeatNMetal_Warrior:
				m_curZoneSequence = ZoneSequence_Brawl;
				break;
			case HG_Mode.MeatNMetal_Veteran:
				m_curZoneSequence = ZoneSequence_Maelstrom;
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
			ConfigureCurrentZone(isBeginning: true);
			if (mode == HG_Mode.AssaultNPepper_Brawl || mode == HG_Mode.AssaultNPepper_Maelstrom || mode == HG_Mode.MeatNMetal_Warrior || mode == HG_Mode.MeatNMetal_Veteran)
			{
				m_curZoneInSequence++;
				ConfigureCurrentZone(isBeginning: true);
			}
			if (mode == HG_Mode.AssaultNPepper_Maelstrom || mode == HG_Mode.MeatNMetal_Veteran)
			{
				m_curZoneInSequence++;
				ConfigureCurrentZone(isBeginning: true);
			}
			m_timer = 0f;
			m_sosigScore = 0f;
			m_numSosigsKilled = 0;
			m_numCivvieKills = 0;
			SpawnCivvies();
			SM.PlayCoreSound(FVRPooledAudioType.UIChirp, AudEvent_ZoneCompleted, base.transform.position);
			m_TickToSpawn = 10f;
			IsPlaying = true;
		}

		private void SpawnCivvies()
		{
			CivvieSpawnPoints.Shuffle();
			CivvieSpawnPoints.Shuffle();
			CivvieSpawnPoints.Shuffle();
			for (int i = 0; i < 5; i++)
			{
				SpawnEnemy(Template_Civvie, CivvieSpawnPoints[i], 1, IsCivvie: true);
			}
		}

		public void Update()
		{
			if (!IsPlaying)
			{
				return;
			}
			m_timer += Time.deltaTime;
			m_TickToSpawn -= Time.deltaTime;
			if (m_TickToSpawn < 0f)
			{
				m_TickToSpawn = Random.Range(18f, 22f);
				SpawnRoutine();
			}
			for (int i = 0; i < m_activeSpawningZones.Count; i++)
			{
				if (m_activeSpawningZones[i].Indicator != null)
				{
					m_activeSpawningZones[i].Indicator.transform.position = m_activeSpawningZones[i].transform.position + Vector3.up * 14f + Vector3.up * (Mathf.Sin(Time.time * 4f) * 1.5f);
				}
			}
		}

		private void SpawnRoutine()
		{
			int num = 2;
			if (m_mode == HG_Mode.AssaultNPepper_Brawl)
			{
				num += 2;
			}
			else if (m_mode == HG_Mode.AssaultNPepper_Maelstrom)
			{
				num += 4;
			}
			int b = Mathf.Clamp(m_maxSosigs - m_activeSosigs.Count, 0, m_maxSosigs);
			int num2 = num;
			num2 = ((m_activeSpawningZones.Count > 2) ? ((num2 <= 4) ? 1 : 2) : ((m_activeSpawningZones.Count <= 1) ? 2 : ((num2 <= 2) ? 1 : 2)));
			num2 = Mathf.Min(num2, b);
			if (num2 <= 0 || m_activeSpawningZones.Count <= 0)
			{
				return;
			}
			for (int i = 0; i < m_activeSpawningZones.Count; i++)
			{
				m_activeSpawningZones[i].SpawnPoints_Defense.Shuffle();
				if (!(Vector3.Distance(GM.CurrentPlayerBody.transform.position, m_activeSpawningZones[i].transform.position) < 10f))
				{
					for (int j = 0; j < num2; j++)
					{
						SpawnEnemyAtPoint(m_activeSpawningZones[i].SpawnPoints_Defense[j]);
					}
				}
			}
		}

		private void SpawnEnemyAtPoint(Transform point)
		{
			int index = Random.Range(0, Mathf.Min(m_difficulty, Templates_Enemy.Count));
			SpawnEnemy(Templates_Enemy[index], point, 1, IsCivvie: false);
		}

		public override void EndMode(bool doesInvokeTeleport, bool immediateTeleportBackAndScore)
		{
			if (IsPlaying)
			{
				IsPlaying = false;
				for (int num = m_activeSosigs.Count - 1; num >= 0; num--)
				{
					m_activeSosigs[num].ClearSosig();
				}
				m_activeSosigs.Clear();
				for (int num2 = m_civvieSosigs.Count - 1; num2 >= 0; num2--)
				{
					m_civvieSosigs[num2].ClearSosig();
				}
				m_civvieSosigs.Clear();
				GM.CurrentSceneSettings.SosigKillEvent -= CheckIfDeadSosigWasMine;
				GM.CurrentSceneSettings.DeathResetPoint.position = InitialRespawnPos;
				GM.CurrentSceneSettings.DeathResetPoint.rotation = InitialRespawnRot;
				M.Case();
				base.EndMode(doesInvokeTeleport: true, immediateTeleportBackAndScore: false);
			}
		}

		private void ConfigureCurrentZone(bool isBeginning)
		{
			HG_Zone hG_Zone = M.Zones[m_curZoneSequence[m_curZoneInSequence]];
			GameObject gameObject = Object.Instantiate(SpawningCore_Prefab, hG_Zone.PlayerSpawnPoint.position + Vector3.up * 2f, Quaternion.identity);
			HG_Target component = gameObject.GetComponent<HG_Target>();
			component.Init(this, hG_Zone);
			m_activeTargets.Add(component);
			m_activeSpawningZones.Add(hG_Zone);
			hG_Zone.Indicator = Object.Instantiate(Indicator_Prefab, hG_Zone.transform.position + Vector3.up * 14f, Quaternion.identity).transform;
			m_TickToSpawn = Mathf.Max(m_TickToSpawn, 5f);
			if (!isBeginning)
			{
				SM.PlayCoreSound(FVRPooledAudioType.UIChirp, AudEvent_ZoneCompleted, hG_Zone.transform.position);
			}
		}

		public void CheckIfDeadSosigWasMine(Sosig s)
		{
			if (m_activeSosigs.Contains(s))
			{
				if (IsPlaying)
				{
					ScoreSosid(s);
				}
				s.TickDownToClear(5f);
				m_activeSosigs.Remove(s);
			}
			else if (m_civvieSosigs.Contains(s))
			{
				if (IsPlaying)
				{
					m_numCivvieKills++;
				}
				s.TickDownToClear(5f);
				m_civvieSosigs.Remove(s);
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

		public override void TargetDestroyed(HG_Target t)
		{
			HG_Zone zone = t.GetZone();
			if (zone.Indicator != null)
			{
				Object.Destroy(zone.Indicator.gameObject);
			}
			m_activeSpawningZones.Remove(zone);
			m_difficulty++;
			m_curZoneInSequence++;
			if (m_curZoneInSequence < m_curZoneSequence.Count)
			{
				ConfigureCurrentZone(isBeginning: false);
			}
			if (m_activeSpawningZones.Count < 1)
			{
				SM.PlayCoreSound(FVRPooledAudioType.UIChirp, AudEvent_SequenceCompleted, base.transform.position);
				EndMode(doesInvokeTeleport: true, immediateTeleportBackAndScore: false);
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
			Sosig item = SpawnEnemySosig(t.SosigPrefabs[Random.Range(0, t.SosigPrefabs.Count)].GetGameObject(), weaponPrefab, weaponPrefab2, point.position, point.rotation, t.ConfigTemplates[Random.Range(0, t.ConfigTemplates.Count)], t.OutfitConfig[Random.Range(0, t.OutfitConfig.Count)], IFF);
			if (IsCivvie)
			{
				m_civvieSosigs.Add(item);
			}
			else
			{
				m_activeSosigs.Add(item);
			}
		}

		private Sosig SpawnEnemySosig(GameObject prefab, GameObject weaponPrefab, GameObject weaponPrefab2, Vector3 pos, Quaternion rot, SosigConfigTemplate t, SosigOutfitConfig w, int IFF)
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
			componentInChildren.CurrentOrder = Sosig.SosigOrder.Wander;
			componentInChildren.FallbackOrder = Sosig.SosigOrder.Wander;
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

		public override int GetScore()
		{
			float num = 0f;
			if (m_numSosigsKilled > 0)
			{
				num = m_sosigScore / (float)m_numSosigsKilled;
			}
			return (m_curZoneSequence.Count - 1) * 750 + m_numSosigsKilled * 10 + Mathf.Max((m_curZoneSequence.Count * 60 - (int)m_timer) * 10, 0) + (int)num * 200 - m_numCivvieKills * 200;
		}

		public override List<string> GetScoringReadOuts()
		{
			float num = 0f;
			if (m_numSosigsKilled > 0)
			{
				num = m_sosigScore / (float)m_numSosigsKilled;
			}
			List<string> list = new List<string>();
			list.Add("Base Score: " + ((m_curZoneSequence.Count - 1) * 750 + m_numSosigsKilled * 10));
			list.Add("Time Bonus: " + Mathf.Max((m_curZoneSequence.Count * 60 - (int)m_timer) * 10, 0));
			list.Add("Style Bonus: " + (int)num * 200);
			list.Add("Friendly Fire Penalty: " + m_numCivvieKills * -200);
			list.Add("Final Score: " + GetScore());
			return list;
		}
	}
}
