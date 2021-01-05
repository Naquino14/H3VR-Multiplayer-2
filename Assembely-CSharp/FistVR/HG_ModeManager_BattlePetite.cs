using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class HG_ModeManager_BattlePetite : HG_ModeManager
	{
		public List<ZosigEnemyTemplate> EnemyTemplates;

		public ZosigEnemyTemplate CivvieTemplate;

		public List<SosigEnemyTemplate> STemplates_Ranged;

		public List<SosigEnemyTemplate> STemplates_Melee;

		public SosigEnemyTemplate STemplate_Civvie;

		public List<FVRObject> SpawnedEquipment_Sosigguns;

		public List<FVRObject> SpawnedEquipment_Sosigmelee;

		public List<FVRObject> StartingSpawnedEquipment_Player;

		public List<Transform> BR_SpawnZones_Player;

		public List<Transform> BR_SpawnZones_Equipment;

		public List<Transform> BR_SpawnZones_Civvies;

		private List<Sosig> m_activeSosigs = new List<Sosig>();

		private List<Sosig> m_civvieSosigs = new List<Sosig>();

		private List<GameObject> m_spawnedEquipment = new List<GameObject>();

		[Header("Audio")]
		public AudioEvent AudEvent_ZoneCompleted;

		public AudioEvent AudEvent_SequenceCompleted;

		private float m_timer;

		private float m_sosigScore;

		private int m_numSosigsKilled;

		private int m_numCivvieKills;

		public Transform RallyPoint;

		public Vector3 StripSearch_Center;

		public Vector3 StripSearch_HalfExtents;

		public LayerMask SearchLM;

		public override void InitMode(HG_Mode mode)
		{
			CleanArena();
			m_mode = mode;
			GM.CurrentSceneSettings.SosigKillEvent += CheckIfDeadSosigWasMine;
			m_activeSosigs.Clear();
			m_civvieSosigs.Clear();
			BR_SpawnZones_Player.Shuffle();
			BR_SpawnZones_Player.Shuffle();
			BR_SpawnZones_Player.Shuffle();
			Transform transform = BR_SpawnZones_Player[0];
			GM.CurrentMovementManager.TeleportToPoint(transform.position, isAbsolute: true, transform.forward);
			InitialRespawnPos = GM.CurrentSceneSettings.DeathResetPoint.position;
			InitialRespawnRot = GM.CurrentSceneSettings.DeathResetPoint.rotation;
			GM.CurrentSceneSettings.DeathResetPoint.position = transform.position;
			GM.CurrentSceneSettings.DeathResetPoint.rotation = transform.rotation;
			StripEquipment(mode);
			switch (mode)
			{
			case HG_Mode.BattlePetite_Sosiggun:
			{
				GameObject item2 = Object.Instantiate(StartingSpawnedEquipment_Player[Random.Range(0, StartingSpawnedEquipment_Player.Count)].GetGameObject(), BR_SpawnZones_Player[0].position + BR_SpawnZones_Player[0].forward + Vector3.up, Random.rotation);
				m_spawnedEquipment.Add(item2);
				GameObject item3 = Object.Instantiate(SpawnedEquipment_Sosigmelee[Random.Range(0, SpawnedEquipment_Sosigmelee.Count)].GetGameObject(), BR_SpawnZones_Player[0].position + BR_SpawnZones_Player[0].right + Vector3.up, Random.rotation);
				m_spawnedEquipment.Add(item3);
				break;
			}
			case HG_Mode.BattlePetite_Melee:
			{
				GameObject item = Object.Instantiate(SpawnedEquipment_Sosigmelee[Random.Range(0, SpawnedEquipment_Sosigmelee.Count)].GetGameObject(), BR_SpawnZones_Player[0].position + BR_SpawnZones_Player[0].forward + Vector3.up, Random.rotation);
				m_spawnedEquipment.Add(item);
				break;
			}
			}
			if (mode == HG_Mode.BattlePetite_Melee)
			{
				for (int i = 1; i < BR_SpawnZones_Player.Count; i++)
				{
					SpawnEnemy(STemplates_Melee[Random.Range(0, STemplates_Melee.Count)], BR_SpawnZones_Player[i], i, IsCivvie: false);
				}
			}
			else
			{
				for (int j = 1; j < BR_SpawnZones_Player.Count; j++)
				{
					SpawnEnemy(STemplates_Ranged[Random.Range(0, STemplates_Ranged.Count)], BR_SpawnZones_Player[j], j, IsCivvie: false);
				}
			}
			BR_SpawnZones_Equipment.Shuffle();
			BR_SpawnZones_Equipment.Shuffle();
			BR_SpawnZones_Equipment.Shuffle();
			switch (mode)
			{
			case HG_Mode.BattlePetite_Open:
			case HG_Mode.BattlePetite_Sosiggun:
			{
				for (int l = 0; l < 30; l++)
				{
					GameObject item5 = Object.Instantiate(SpawnedEquipment_Sosigguns[Random.Range(0, SpawnedEquipment_Sosigguns.Count)].GetGameObject(), BR_SpawnZones_Equipment[l].position + Vector3.up, Random.rotation);
					m_spawnedEquipment.Add(item5);
				}
				break;
			}
			case HG_Mode.BattlePetite_Melee:
			{
				for (int k = 0; k < 30; k++)
				{
					GameObject item4 = Object.Instantiate(SpawnedEquipment_Sosigmelee[Random.Range(0, SpawnedEquipment_Sosigmelee.Count)].GetGameObject(), BR_SpawnZones_Equipment[k].position + Vector3.up, Random.rotation);
					m_spawnedEquipment.Add(item4);
				}
				break;
			}
			}
			m_timer = 0f;
			m_sosigScore = 0f;
			m_numSosigsKilled = 0;
			m_numCivvieKills = 0;
			SpawnCivvies();
			SM.PlayCoreSound(FVRPooledAudioType.UIChirp, AudEvent_ZoneCompleted, base.transform.position);
			IsPlaying = true;
		}

		public override void HandlePlayerDeath()
		{
			EndMode(doesInvokeTeleport: false, immediateTeleportBackAndScore: true);
		}

		public override void EndMode(bool doesInvokeTeleport, bool immediateTeleportBackAndScore)
		{
			IsPlaying = false;
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
			if (m_civvieSosigs.Count > 0)
			{
				for (int num2 = m_civvieSosigs.Count - 1; num2 >= 0; num2--)
				{
					if (m_civvieSosigs[num2] != null)
					{
						m_civvieSosigs[num2].ClearSosig();
					}
				}
			}
			m_civvieSosigs.Clear();
			for (int num3 = m_spawnedEquipment.Count - 1; num3 >= 0; num3--)
			{
				Object.Destroy(m_spawnedEquipment[num3]);
			}
			m_spawnedEquipment.Clear();
			GM.CurrentSceneSettings.SosigKillEvent -= CheckIfDeadSosigWasMine;
			GM.CurrentSceneSettings.DeathResetPoint.position = InitialRespawnPos;
			GM.CurrentSceneSettings.DeathResetPoint.rotation = InitialRespawnRot;
			M.Case();
			base.EndMode(doesInvokeTeleport, immediateTeleportBackAndScore);
		}

		private void StripEquipment(HG_Mode mode)
		{
			Collider[] array = Physics.OverlapBox(StripSearch_Center, StripSearch_HalfExtents, Quaternion.identity, SearchLM, QueryTriggerInteraction.Collide);
			if (mode != HG_Mode.BattlePetite_Sosiggun && mode != HG_Mode.BattlePetite_Melee)
			{
				return;
			}
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

		private void SpawnCivvies()
		{
			BR_SpawnZones_Civvies.Shuffle();
			BR_SpawnZones_Civvies.Shuffle();
			BR_SpawnZones_Civvies.Shuffle();
			for (int i = 0; i < 5; i++)
			{
				SpawnEnemy(STemplate_Civvie, BR_SpawnZones_Civvies[i], -3, IsCivvie: true);
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
			if (IsCivvie)
			{
				m_civvieSosigs.Add(item);
			}
			else
			{
				m_activeSosigs.Add(item);
			}
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
				componentInChildren.CommandAssaultPoint(RallyPoint.position);
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
			else if (m_civvieSosigs.Contains(s))
			{
				if (IsPlaying && s.GetDiedFromIFF() == GM.CurrentPlayerBody.GetPlayerIFF())
				{
					m_numCivvieKills++;
				}
				s.TickDownToClear(5f);
				m_civvieSosigs.Remove(s);
			}
			if (m_activeSosigs.Count < 1 && IsPlaying)
			{
				SM.PlayCoreSound(FVRPooledAudioType.UIChirp, AudEvent_SequenceCompleted, base.transform.position);
				EndMode(doesInvokeTeleport: true, immediateTeleportBackAndScore: false);
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

		public override int GetScore()
		{
			float num = 0f;
			if (m_numSosigsKilled > 0)
			{
				num = m_sosigScore / (float)m_numSosigsKilled;
			}
			return m_numSosigsKilled * 500 + (int)num * 150 - m_numCivvieKills * 200;
		}

		public override List<string> GetScoringReadOuts()
		{
			float num = 0f;
			if (m_numSosigsKilled > 0)
			{
				num = m_sosigScore / (float)m_numSosigsKilled;
			}
			List<string> list = new List<string>();
			list.Add("Base Score: " + m_numSosigsKilled * 500);
			list.Add("Style Bonus: " + (int)num * 150);
			list.Add("Friendly Fire Penalty: " + m_numCivvieKills * -200);
			list.Add("Final Score: " + GetScore());
			return list;
		}
	}
}
