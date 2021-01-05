using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class TAH_Manager : MonoBehaviour
	{
		public enum TAHLogicStyle
		{
			Classic,
			HL2
		}

		public enum TAHGameState
		{
			WaitingToStart,
			Taking,
			Holding
		}

		public GameObject TAHMenu;

		public TAHLogicStyle LogicStyle;

		public List<TAH_SupplyPoint> SupplyPoints;

		public List<TAH_DefensePoint> DefensePoints;

		public TAHGameState State;

		private int m_curDefensePointIndex = -1;

		private int m_lastDefensePointIndex = -1;

		private int m_curSupplyPointIndex = -1;

		private int m_lastSupplyPointIndex = -1;

		private int m_pointsTaken;

		public SV_LootTable LTList_PowerUps;

		public GameObject TAHCratePrefabLarge;

		public GameObject TAHCratePrefabSmall;

		public List<TAH_WaveDefinition> WaveDefinitions = new List<TAH_WaveDefinition>();

		private List<TAH_WaveDefinition> m_holdSequence = new List<TAH_WaveDefinition>();

		private float m_holdTimer;

		private float m_waveTimer;

		private int m_currentWave;

		private int m_maxWave;

		private List<GameObject> SpawnedBots = new List<GameObject>();

		private List<GameObject> SpawnedBots_Attacking = new List<GameObject>();

		public TAH_BotSpawnProfile[] SupplyPointBotProfiles;

		public TAH_BotSpawnProfile[] DefensePointBotProfiles;

		public List<TAH_MobSpawnGroup> MobSpawnGroups;

		private List<GameObject> m_spawnedMobs = new List<GameObject>();

		private HashSet<FVRPhysicalObject> m_knownObjsHash = new HashSet<FVRPhysicalObject>();

		private List<FVRPhysicalObject> m_knownObjs = new List<FVRPhysicalObject>();

		private int knownObjectCheckIndex;

		public bool UsesOldReticle = true;

		public Transform Reticle_Root;

		public Transform Reticle_TAH;

		public Transform Reticle_Supply;

		public Transform Reticle_Beacon;

		public TAH_Reticle TAHReticle;

		public GameObject FastProjectile;

		public GameObject ItemSpawner;

		public FVRFMODController FMODController;

		public LootTable LT_TestAllGuns = new LootTable();

		public List<LootTable> LTChart_CaseWeapon = new List<LootTable>();

		public LootTable LT_Firearms_SideArm = new LootTable();

		public LootTable LT_Firearms_SecondaryWeapon_LowCap = new LootTable();

		public LootTable LT_Firearms_SecondaryWeapon_HighCap = new LootTable();

		public LootTable LT_Firearms_SniperRifle = new LootTable();

		public LootTable LT_Firearms_PrimaryWeapon_LowCap = new LootTable();

		public LootTable LT_Firearms_PrimaryWeapon_MedCap = new LootTable();

		public LootTable LT_Firearms_PrimaryWeapon_HighCap = new LootTable();

		public LootTable LT_Firearms_Shotgun_LowCap = new LootTable();

		public LootTable LT_Firearms_Shotgun_HighCap = new LootTable();

		public LootTable LT_Firearms_SmallOrdnance = new LootTable();

		public LootTable LT_FirearmsSpecial = new LootTable();

		public LootTable LT_Grenades = new LootTable();

		public LootTable LT_Health = new LootTable();

		public LootTable LT_PowerUps = new LootTable();

		public LootTable LT_CommonAttachments = new LootTable();

		public LootTable LT_RareAttachments = new LootTable();

		public LootTable LT_MeleeWeapons = new LootTable();

		public LootTable LT_Utility = new LootTable();

		public LootTable LT_RequiredAttachments = new LootTable();

		private int m_ltStartMin;

		private int m_ltStartMax = 1;

		private int m_ltDiffModMin;

		private int m_ltDiffModMax = 1;

		private int m_ltDiffMinCap;

		private int m_maxWaveType;

		private int m_waveDifficultyMod;

		private float m_botHealthMultiplier = 1f;

		private float m_lootThrownThreshold = 0.9f;

		private float m_lootRareAttachThreshold = 0.8f;

		private float m_lootCommonAttachThreshold = 0.5f;

		private float m_timeUntilWeCanEventCheck = 60f;

		private Vector2 m_eventTriggerTickRange = new Vector2(20f, 60f);

		private HashSet<TAH_SupplyPoint> m_eventFiredSoFar_SupplyPoint = new HashSet<TAH_SupplyPoint>();

		private int m_checkIndex_SupplyPoint;

		private List<GameObject> m_eventSpawns = new List<GameObject>();

		public TAH_BotSpawnProfile[] EventProfiles_MeatZombies;

		public TAH_BotSpawnProfile[] EventProfiles_SpecOps;

		public TAH_BotSpawnProfile[] EventProfiles_FastZombies;

		public GameObject[] EventPrefabs_MeatCrabs;

		private int m_generatedStyle;

		private float m_takeMusicIntensity = 1f;

		private float m_takeMusicIntensityTarget = 1f;

		private float m_fireThreshold;

		private float m_botKillThreshold;

		private float PingTimer = 5f;

		public int GetDifficulty()
		{
			return m_pointsTaken;
		}

		private void Awake()
		{
			GM.TAHMaster = this;
			if (UsesOldReticle)
			{
				Reticle_Root.gameObject.SetActive(value: false);
			}
		}

		private void Start()
		{
			FMODController.SetMasterVolume(0.25f);
			GenerateLootTables();
			GM.CurrentSceneSettings.ObjectPickedUpEvent += AddFVRObjectToTrackedList;
			GM.CurrentPlayerBody.SetHealthThreshold(10000f);
			if (GM.TAHSettings.TAHOption_ItemSpawner == 1)
			{
				ItemSpawner.SetActive(value: true);
			}
			else
			{
				ItemSpawner.SetActive(value: false);
			}
			if (GM.TAHSettings.TAHOption_Music == 1)
			{
			}
			GM.CurrentSceneSettings.KillEvent += OnBotKill;
			GM.CurrentSceneSettings.ShotFiredEvent += OnShotFired;
			GM.CurrentSceneSettings.BotShotFiredEvent += OnBotShotFired;
		}

		private void OnDisable()
		{
			GM.CurrentSceneSettings.KillEvent -= OnBotKill;
			GM.CurrentSceneSettings.ShotFiredEvent -= OnShotFired;
			GM.CurrentSceneSettings.BotShotFiredEvent -= OnBotShotFired;
		}

		private void OnBotKill(Damage d)
		{
			if (State == TAHGameState.Taking)
			{
				TakingBotKill();
			}
		}

		private void OnShotFired(FVRFireArm firearm)
		{
			if (State == TAHGameState.Taking)
			{
				TakingGunShot();
			}
		}

		private void OnBotShotFired(wwBotWurstModernGun gun)
		{
			if (State == TAHGameState.Taking)
			{
				TakingGunShot();
			}
		}

		public void ItemSpawnerState(bool b)
		{
			ItemSpawner.SetActive(b);
		}

		public void MusicState(bool b)
		{
		}

		private void GenerateLootTables()
		{
			LTChart_CaseWeapon.Clear();
			int num = (m_generatedStyle = GM.TAHSettings.TAHOption_LootProgression);
			List<FVRObject.OTagFirearmSize> list = new List<FVRObject.OTagFirearmSize>();
			list.Add(FVRObject.OTagFirearmSize.Carbine);
			list.Add(FVRObject.OTagFirearmSize.Compact);
			list.Add(FVRObject.OTagFirearmSize.FullSize);
			list.Add(FVRObject.OTagFirearmSize.Pistol);
			list.Add(FVRObject.OTagFirearmSize.Pocket);
			List<FVRObject.OTagFirearmSize> list2 = list;
			list = new List<FVRObject.OTagFirearmSize>();
			list.Add(FVRObject.OTagFirearmSize.Oversize);
			list.Add(FVRObject.OTagFirearmSize.Bulky);
			List<FVRObject.OTagFirearmSize> sizes = list;
			List<FVRObject.OTagFirearmRoundPower> list3 = new List<FVRObject.OTagFirearmRoundPower>();
			list3.Add(FVRObject.OTagFirearmRoundPower.Tiny);
			list3.Add(FVRObject.OTagFirearmRoundPower.Pistol);
			List<FVRObject.OTagFirearmRoundPower> list4 = list3;
			list3 = new List<FVRObject.OTagFirearmRoundPower>();
			list3.Add(FVRObject.OTagFirearmRoundPower.Shotgun);
			List<FVRObject.OTagFirearmRoundPower> list5 = list3;
			list3 = new List<FVRObject.OTagFirearmRoundPower>();
			list3.Add(FVRObject.OTagFirearmRoundPower.Intermediate);
			list3.Add(FVRObject.OTagFirearmRoundPower.FullPower);
			List<FVRObject.OTagFirearmRoundPower> list6 = list3;
			list3 = new List<FVRObject.OTagFirearmRoundPower>();
			list3.Add(FVRObject.OTagFirearmRoundPower.Ordnance);
			List<FVRObject.OTagFirearmRoundPower> list7 = list3;
			LootTable.LootTableType type;
			if (m_generatedStyle == 0)
			{
				List<FVRObject.OTagEra> list8 = new List<FVRObject.OTagEra>();
				list8.Add(FVRObject.OTagEra.Modern);
				list8.Add(FVRObject.OTagEra.PostWar);
				list8.Add(FVRObject.OTagEra.TurnOfTheCentury);
				list8.Add(FVRObject.OTagEra.WildWest);
				list8.Add(FVRObject.OTagEra.WW1);
				list8.Add(FVRObject.OTagEra.WW2);
				List<FVRObject.OTagEra> list9 = list8;
				LootTable lT_Firearms_SideArm = LT_Firearms_SideArm;
				type = LootTable.LootTableType.Firearm;
				list8 = list9;
				list = new List<FVRObject.OTagFirearmSize>
				{
					FVRObject.OTagFirearmSize.Pocket,
					FVRObject.OTagFirearmSize.Pistol,
					FVRObject.OTagFirearmSize.Compact
				};
				list3 = list4;
				lT_Firearms_SideArm.Initialize(type, list8, list, null, null, null, null, null, list3, null, null, null, null, null, -1, 20);
				LootTable lT_Firearms_SecondaryWeapon_LowCap = LT_Firearms_SecondaryWeapon_LowCap;
				type = LootTable.LootTableType.Firearm;
				list8 = list9;
				list = new List<FVRObject.OTagFirearmSize>
				{
					FVRObject.OTagFirearmSize.Compact,
					FVRObject.OTagFirearmSize.Carbine,
					FVRObject.OTagFirearmSize.FullSize
				};
				list3 = list4;
				lT_Firearms_SecondaryWeapon_LowCap.Initialize(type, list8, list, null, null, null, null, null, list3, null, null, null, null, null, -1, 32);
				LootTable lT_Firearms_SecondaryWeapon_HighCap = LT_Firearms_SecondaryWeapon_HighCap;
				type = LootTable.LootTableType.Firearm;
				list8 = list9;
				list = new List<FVRObject.OTagFirearmSize>
				{
					FVRObject.OTagFirearmSize.Compact,
					FVRObject.OTagFirearmSize.Carbine,
					FVRObject.OTagFirearmSize.FullSize
				};
				list3 = list4;
				lT_Firearms_SecondaryWeapon_HighCap.Initialize(type, list8, list, null, null, null, null, null, list3, null, null, null, null, null, 33, 100);
				LootTable lT_Firearms_SniperRifle = LT_Firearms_SniperRifle;
				type = LootTable.LootTableType.Firearm;
				list8 = list9;
				list = new List<FVRObject.OTagFirearmSize>
				{
					FVRObject.OTagFirearmSize.Carbine,
					FVRObject.OTagFirearmSize.FullSize,
					FVRObject.OTagFirearmSize.Bulky
				};
				list3 = list6;
				List<FVRObject.OTagFirearmAction> actions = new List<FVRObject.OTagFirearmAction>
				{
					FVRObject.OTagFirearmAction.BoltAction
				};
				lT_Firearms_SniperRifle.Initialize(type, list8, list, actions, null, null, null, null, list3);
				LootTable lT_Firearms_PrimaryWeapon_LowCap = LT_Firearms_PrimaryWeapon_LowCap;
				type = LootTable.LootTableType.Firearm;
				list8 = list9;
				list = new List<FVRObject.OTagFirearmSize>
				{
					FVRObject.OTagFirearmSize.Compact,
					FVRObject.OTagFirearmSize.Carbine,
					FVRObject.OTagFirearmSize.FullSize
				};
				actions = new List<FVRObject.OTagFirearmAction>
				{
					FVRObject.OTagFirearmAction.Automatic
				};
				list3 = list6;
				lT_Firearms_PrimaryWeapon_LowCap.Initialize(type, list8, list, actions, null, null, null, null, list3, null, null, null, null, null, -1, 16);
				LootTable lT_Firearms_PrimaryWeapon_MedCap = LT_Firearms_PrimaryWeapon_MedCap;
				type = LootTable.LootTableType.Firearm;
				list8 = list9;
				list = new List<FVRObject.OTagFirearmSize>
				{
					FVRObject.OTagFirearmSize.Compact,
					FVRObject.OTagFirearmSize.Carbine,
					FVRObject.OTagFirearmSize.FullSize
				};
				actions = new List<FVRObject.OTagFirearmAction>
				{
					FVRObject.OTagFirearmAction.Automatic
				};
				list3 = list6;
				lT_Firearms_PrimaryWeapon_MedCap.Initialize(type, list8, list, actions, null, null, null, null, list3, null, null, null, null, null, 17, 44);
				LootTable lT_Firearms_PrimaryWeapon_HighCap = LT_Firearms_PrimaryWeapon_HighCap;
				type = LootTable.LootTableType.Firearm;
				list8 = list9;
				list = new List<FVRObject.OTagFirearmSize>
				{
					FVRObject.OTagFirearmSize.Compact,
					FVRObject.OTagFirearmSize.Carbine,
					FVRObject.OTagFirearmSize.FullSize
				};
				actions = new List<FVRObject.OTagFirearmAction>
				{
					FVRObject.OTagFirearmAction.Automatic
				};
				list3 = list6;
				lT_Firearms_PrimaryWeapon_HighCap.Initialize(type, list8, list, actions, null, null, null, null, list3, null, null, null, null, null, 45, 100);
				LootTable lT_Firearms_Shotgun_LowCap = LT_Firearms_Shotgun_LowCap;
				type = LootTable.LootTableType.Firearm;
				list8 = list9;
				list3 = list5;
				lT_Firearms_Shotgun_LowCap.Initialize(type, list8, null, null, null, null, null, null, list3, null, null, null, null, null, -1, 8);
				LootTable lT_Firearms_Shotgun_HighCap = LT_Firearms_Shotgun_HighCap;
				type = LootTable.LootTableType.Firearm;
				list8 = list9;
				list3 = list5;
				lT_Firearms_Shotgun_HighCap.Initialize(type, list8, null, null, null, null, null, null, list3, null, null, null, null, null, 9);
				LootTable lT_Firearms_SmallOrdnance = LT_Firearms_SmallOrdnance;
				type = LootTable.LootTableType.Firearm;
				list8 = list9;
				list = new List<FVRObject.OTagFirearmSize>
				{
					FVRObject.OTagFirearmSize.Compact,
					FVRObject.OTagFirearmSize.Carbine,
					FVRObject.OTagFirearmSize.FullSize
				};
				list3 = list7;
				lT_Firearms_SmallOrdnance.Initialize(type, list8, list, null, null, null, null, null, list3);
				LT_FirearmsSpecial.Initialize(LootTable.LootTableType.Firearm, list9, sizes);
				LootTable lT_Grenades = LT_Grenades;
				type = LootTable.LootTableType.Thrown;
				list8 = list9;
				List<FVRObject.OTagThrownType> thrownTypes = new List<FVRObject.OTagThrownType>
				{
					FVRObject.OTagThrownType.Pinned
				};
				lT_Grenades.Initialize(type, list8, null, null, null, null, null, null, null, null, null, null, null, thrownTypes);
				LootTable lT_CommonAttachments = LT_CommonAttachments;
				type = LootTable.LootTableType.Attachments;
				list8 = list9;
				List<FVRObject.OTagAttachmentFeature> features = new List<FVRObject.OTagAttachmentFeature>
				{
					FVRObject.OTagAttachmentFeature.BarrelExtension,
					FVRObject.OTagAttachmentFeature.Grip,
					FVRObject.OTagAttachmentFeature.IronSight,
					FVRObject.OTagAttachmentFeature.Reflex
				};
				lT_CommonAttachments.Initialize(type, list8, null, null, null, null, null, null, null, features);
				LootTable lT_RareAttachments = LT_RareAttachments;
				type = LootTable.LootTableType.Attachments;
				list8 = list9;
				features = new List<FVRObject.OTagAttachmentFeature>
				{
					FVRObject.OTagAttachmentFeature.Laser,
					FVRObject.OTagAttachmentFeature.Magnification,
					FVRObject.OTagAttachmentFeature.Suppression,
					FVRObject.OTagAttachmentFeature.Stock
				};
				lT_RareAttachments.Initialize(type, list8, null, null, null, null, null, null, null, features);
				LootTable lT_MeleeWeapons = LT_MeleeWeapons;
				type = LootTable.LootTableType.Melee;
				list8 = list9;
				List<FVRObject.OTagMeleeStyle> meleeStyles = new List<FVRObject.OTagMeleeStyle>
				{
					FVRObject.OTagMeleeStyle.Tactical
				};
				List<FVRObject.OTagMeleeHandedness> meleeHandedness = new List<FVRObject.OTagMeleeHandedness>
				{
					FVRObject.OTagMeleeHandedness.OneHanded
				};
				lT_MeleeWeapons.Initialize(type, list8, null, null, null, null, null, null, null, null, meleeStyles, meleeHandedness);
				LootTable lT_RequiredAttachments = LT_RequiredAttachments;
				type = LootTable.LootTableType.Attachments;
				list8 = list9;
				features = new List<FVRObject.OTagAttachmentFeature>
				{
					FVRObject.OTagAttachmentFeature.IronSight,
					FVRObject.OTagAttachmentFeature.Reflex
				};
				List<FVRObject.OTagFirearmMount> mounts = new List<FVRObject.OTagFirearmMount>
				{
					FVRObject.OTagFirearmMount.Picatinny
				};
				lT_RequiredAttachments.Initialize(type, list8, null, null, null, null, null, mounts, null, features);
				LTChart_CaseWeapon.Add(LT_Firearms_SideArm);
				LTChart_CaseWeapon.Add(LT_Firearms_Shotgun_LowCap);
				LTChart_CaseWeapon.Add(LT_Firearms_SecondaryWeapon_LowCap);
				LTChart_CaseWeapon.Add(LT_Firearms_SniperRifle);
				LTChart_CaseWeapon.Add(LT_Firearms_PrimaryWeapon_LowCap);
				LTChart_CaseWeapon.Add(LT_Firearms_SmallOrdnance);
				LTChart_CaseWeapon.Add(LT_Firearms_PrimaryWeapon_MedCap);
				LTChart_CaseWeapon.Add(LT_Firearms_SecondaryWeapon_HighCap);
				LTChart_CaseWeapon.Add(LT_Firearms_Shotgun_HighCap);
				LTChart_CaseWeapon.Add(LT_Firearms_PrimaryWeapon_HighCap);
				m_ltStartMin = 0;
				m_ltStartMax = 2;
				m_ltDiffModMin = -2;
				m_ltDiffModMax = 2;
				m_ltDiffMinCap = 5;
				m_maxWaveType = 18;
				m_waveDifficultyMod = 0;
				m_botHealthMultiplier = 1f;
				m_lootThrownThreshold = 0.75f;
				m_lootRareAttachThreshold = 0.65f;
				m_lootCommonAttachThreshold = 0.4f;
			}
			else if (m_generatedStyle == 1)
			{
				List<FVRObject.OTagEra> list8 = new List<FVRObject.OTagEra>();
				list8.Add(FVRObject.OTagEra.TurnOfTheCentury);
				list8.Add(FVRObject.OTagEra.WW1);
				list8.Add(FVRObject.OTagEra.WW2);
				List<FVRObject.OTagEra> list10 = list8;
				LootTable lT_Firearms_SideArm2 = LT_Firearms_SideArm;
				type = LootTable.LootTableType.Firearm;
				list8 = list10;
				list = new List<FVRObject.OTagFirearmSize>
				{
					FVRObject.OTagFirearmSize.Pocket,
					FVRObject.OTagFirearmSize.Pistol
				};
				list3 = list4;
				lT_Firearms_SideArm2.Initialize(type, list8, list, null, null, null, null, null, list3, null, null, null, null, null, -1, 20);
				LootTable lT_Firearms_SecondaryWeapon_LowCap2 = LT_Firearms_SecondaryWeapon_LowCap;
				type = LootTable.LootTableType.Firearm;
				list8 = list10;
				list = new List<FVRObject.OTagFirearmSize>
				{
					FVRObject.OTagFirearmSize.Compact,
					FVRObject.OTagFirearmSize.Carbine,
					FVRObject.OTagFirearmSize.FullSize
				};
				list3 = list4;
				lT_Firearms_SecondaryWeapon_LowCap2.Initialize(type, list8, list, null, null, null, null, null, list3, null, null, null, null, null, -1, 20);
				LootTable lT_Firearms_SecondaryWeapon_HighCap2 = LT_Firearms_SecondaryWeapon_HighCap;
				type = LootTable.LootTableType.Firearm;
				list8 = list10;
				list = new List<FVRObject.OTagFirearmSize>
				{
					FVRObject.OTagFirearmSize.Compact,
					FVRObject.OTagFirearmSize.Carbine,
					FVRObject.OTagFirearmSize.FullSize
				};
				list3 = list4;
				lT_Firearms_SecondaryWeapon_HighCap2.Initialize(type, list8, list, null, null, null, null, null, list3, null, null, null, null, null, 21, 100);
				LootTable lT_Firearms_SniperRifle2 = LT_Firearms_SniperRifle;
				type = LootTable.LootTableType.Firearm;
				list8 = list10;
				list = new List<FVRObject.OTagFirearmSize>
				{
					FVRObject.OTagFirearmSize.Carbine,
					FVRObject.OTagFirearmSize.FullSize,
					FVRObject.OTagFirearmSize.Bulky
				};
				list3 = list6;
				List<FVRObject.OTagFirearmAction> actions = new List<FVRObject.OTagFirearmAction>
				{
					FVRObject.OTagFirearmAction.BoltAction
				};
				lT_Firearms_SniperRifle2.Initialize(type, list8, list, actions, null, null, null, null, list3);
				LootTable lT_Firearms_PrimaryWeapon_LowCap2 = LT_Firearms_PrimaryWeapon_LowCap;
				type = LootTable.LootTableType.Firearm;
				list8 = list10;
				list = new List<FVRObject.OTagFirearmSize>
				{
					FVRObject.OTagFirearmSize.Compact,
					FVRObject.OTagFirearmSize.Carbine,
					FVRObject.OTagFirearmSize.FullSize
				};
				actions = new List<FVRObject.OTagFirearmAction>
				{
					FVRObject.OTagFirearmAction.Automatic
				};
				list3 = list6;
				lT_Firearms_PrimaryWeapon_LowCap2.Initialize(type, list8, list, actions, null, null, null, null, list3, null, null, null, null, null, -1, 15);
				LootTable lT_Firearms_PrimaryWeapon_MedCap2 = LT_Firearms_PrimaryWeapon_MedCap;
				type = LootTable.LootTableType.Firearm;
				list8 = list10;
				list = new List<FVRObject.OTagFirearmSize>
				{
					FVRObject.OTagFirearmSize.Compact,
					FVRObject.OTagFirearmSize.Carbine,
					FVRObject.OTagFirearmSize.FullSize
				};
				actions = new List<FVRObject.OTagFirearmAction>
				{
					FVRObject.OTagFirearmAction.Automatic
				};
				list3 = list6;
				lT_Firearms_PrimaryWeapon_MedCap2.Initialize(type, list8, list, actions, null, null, null, null, list3, null, null, null, null, null, 16, 100);
				LootTable lT_Firearms_SmallOrdnance2 = LT_Firearms_SmallOrdnance;
				type = LootTable.LootTableType.Firearm;
				list8 = list10;
				list = new List<FVRObject.OTagFirearmSize>
				{
					FVRObject.OTagFirearmSize.Compact,
					FVRObject.OTagFirearmSize.Carbine,
					FVRObject.OTagFirearmSize.FullSize
				};
				list3 = list7;
				lT_Firearms_SmallOrdnance2.Initialize(type, list8, list, null, null, null, null, null, list3);
				LootTable lT_Firearms_Shotgun_LowCap2 = LT_Firearms_Shotgun_LowCap;
				type = LootTable.LootTableType.Firearm;
				list8 = list10;
				list3 = list5;
				lT_Firearms_Shotgun_LowCap2.Initialize(type, list8, null, null, null, null, null, null, list3);
				LT_FirearmsSpecial.Initialize(LootTable.LootTableType.Firearm, list10, sizes);
				LootTable lT_Grenades2 = LT_Grenades;
				type = LootTable.LootTableType.Thrown;
				list8 = list10;
				List<FVRObject.OTagThrownType> thrownTypes = new List<FVRObject.OTagThrownType>
				{
					FVRObject.OTagThrownType.Pinned
				};
				lT_Grenades2.Initialize(type, list8, null, null, null, null, null, null, null, null, null, null, null, thrownTypes);
				LootTable lT_CommonAttachments2 = LT_CommonAttachments;
				type = LootTable.LootTableType.Attachments;
				list8 = list10;
				List<FVRObject.OTagAttachmentFeature> features = new List<FVRObject.OTagAttachmentFeature>
				{
					FVRObject.OTagAttachmentFeature.Adapter,
					FVRObject.OTagAttachmentFeature.BarrelExtension,
					FVRObject.OTagAttachmentFeature.Grip,
					FVRObject.OTagAttachmentFeature.IronSight,
					FVRObject.OTagAttachmentFeature.Reflex,
					FVRObject.OTagAttachmentFeature.Stock
				};
				lT_CommonAttachments2.Initialize(type, list8, null, null, null, null, null, null, null, features);
				LootTable lT_RareAttachments2 = LT_RareAttachments;
				type = LootTable.LootTableType.Attachments;
				list8 = list10;
				features = new List<FVRObject.OTagAttachmentFeature>
				{
					FVRObject.OTagAttachmentFeature.Laser,
					FVRObject.OTagAttachmentFeature.Magnification,
					FVRObject.OTagAttachmentFeature.Suppression
				};
				lT_RareAttachments2.Initialize(type, list8, null, null, null, null, null, null, null, features);
				LootTable lT_MeleeWeapons2 = LT_MeleeWeapons;
				type = LootTable.LootTableType.Melee;
				list8 = list10;
				List<FVRObject.OTagMeleeStyle> meleeStyles = new List<FVRObject.OTagMeleeStyle>
				{
					FVRObject.OTagMeleeStyle.Improvised,
					FVRObject.OTagMeleeStyle.Tactical,
					FVRObject.OTagMeleeStyle.Tool
				};
				List<FVRObject.OTagMeleeHandedness> meleeHandedness = new List<FVRObject.OTagMeleeHandedness>
				{
					FVRObject.OTagMeleeHandedness.OneHanded
				};
				lT_MeleeWeapons2.Initialize(type, list8, null, null, null, null, null, null, null, null, meleeStyles, meleeHandedness);
				LTChart_CaseWeapon.Add(LT_Firearms_SideArm);
				LTChart_CaseWeapon.Add(LT_Firearms_Shotgun_LowCap);
				LTChart_CaseWeapon.Add(LT_Firearms_SecondaryWeapon_LowCap);
				LTChart_CaseWeapon.Add(LT_Firearms_SniperRifle);
				LTChart_CaseWeapon.Add(LT_Firearms_PrimaryWeapon_LowCap);
				LTChart_CaseWeapon.Add(LT_Firearms_SecondaryWeapon_HighCap);
				LTChart_CaseWeapon.Add(LT_Firearms_SmallOrdnance);
				LTChart_CaseWeapon.Add(LT_Firearms_PrimaryWeapon_MedCap);
				m_ltStartMin = 0;
				m_ltStartMax = 2;
				m_ltDiffModMin = -2;
				m_ltDiffModMax = 2;
				m_ltDiffMinCap = 5;
				m_maxWaveType = 12;
				m_waveDifficultyMod = -1;
				m_botHealthMultiplier = 0.8f;
				m_lootThrownThreshold = 0.65f;
				m_lootRareAttachThreshold = 1.65f;
				m_lootCommonAttachThreshold = 1.4f;
			}
			else if (m_generatedStyle == 2)
			{
				List<FVRObject.OTagEra> list8 = new List<FVRObject.OTagEra>();
				list8.Add(FVRObject.OTagEra.Colonial);
				list8.Add(FVRObject.OTagEra.TurnOfTheCentury);
				list8.Add(FVRObject.OTagEra.WildWest);
				List<FVRObject.OTagEra> list11 = list8;
				LootTable lT_Firearms_SideArm3 = LT_Firearms_SideArm;
				type = LootTable.LootTableType.Firearm;
				list8 = list11;
				list = new List<FVRObject.OTagFirearmSize>
				{
					FVRObject.OTagFirearmSize.Pocket,
					FVRObject.OTagFirearmSize.Pistol
				};
				list3 = list4;
				lT_Firearms_SideArm3.Initialize(type, list8, list, null, null, null, null, null, list3, null, null, null, null, null, -1, 20);
				LootTable lT_Firearms_SecondaryWeapon_LowCap3 = LT_Firearms_SecondaryWeapon_LowCap;
				type = LootTable.LootTableType.Firearm;
				list8 = list11;
				list = new List<FVRObject.OTagFirearmSize>
				{
					FVRObject.OTagFirearmSize.Compact,
					FVRObject.OTagFirearmSize.Carbine,
					FVRObject.OTagFirearmSize.FullSize
				};
				list3 = list4;
				lT_Firearms_SecondaryWeapon_LowCap3.Initialize(type, list8, list, null, null, null, null, null, list3, null, null, null, null, null, -1, 30);
				LootTable lT_Firearms_PrimaryWeapon_LowCap3 = LT_Firearms_PrimaryWeapon_LowCap;
				type = LootTable.LootTableType.Firearm;
				list8 = list11;
				list = new List<FVRObject.OTagFirearmSize>
				{
					FVRObject.OTagFirearmSize.Compact,
					FVRObject.OTagFirearmSize.Carbine,
					FVRObject.OTagFirearmSize.FullSize,
					FVRObject.OTagFirearmSize.Bulky
				};
				list3 = list6;
				lT_Firearms_PrimaryWeapon_LowCap3.Initialize(type, list8, list, null, null, null, null, null, list3, null, null, null, null, null, -1, 30);
				LootTable lT_Firearms_Shotgun_LowCap3 = LT_Firearms_Shotgun_LowCap;
				type = LootTable.LootTableType.Firearm;
				list8 = list11;
				list3 = list5;
				lT_Firearms_Shotgun_LowCap3.Initialize(type, list8, null, null, null, null, null, null, list3, null, null, null, null, null, -1, 2);
				LootTable lT_Firearms_Shotgun_HighCap2 = LT_Firearms_Shotgun_HighCap;
				type = LootTable.LootTableType.Firearm;
				list8 = list11;
				list3 = list5;
				lT_Firearms_Shotgun_HighCap2.Initialize(type, list8, null, null, null, null, null, null, list3, null, null, null, null, null, 3);
				LT_FirearmsSpecial.Initialize(LootTable.LootTableType.Firearm, list11, sizes);
				LootTable lT_Grenades3 = LT_Grenades;
				type = LootTable.LootTableType.Thrown;
				List<FVRObject.OTagThrownType> thrownTypes = new List<FVRObject.OTagThrownType>
				{
					FVRObject.OTagThrownType.ManualFuse
				};
				lT_Grenades3.Initialize(type, null, null, null, null, null, null, null, null, null, null, null, null, thrownTypes);
				LootTable lT_CommonAttachments3 = LT_CommonAttachments;
				type = LootTable.LootTableType.Attachments;
				list8 = list11;
				List<FVRObject.OTagAttachmentFeature> features = new List<FVRObject.OTagAttachmentFeature>
				{
					FVRObject.OTagAttachmentFeature.Stock
				};
				lT_CommonAttachments3.Initialize(type, list8, null, null, null, null, null, null, null, features);
				LootTable lT_RareAttachments3 = LT_RareAttachments;
				type = LootTable.LootTableType.Attachments;
				list8 = list11;
				features = new List<FVRObject.OTagAttachmentFeature>
				{
					FVRObject.OTagAttachmentFeature.Stock
				};
				lT_RareAttachments3.Initialize(type, list8, null, null, null, null, null, null, null, features);
				LootTable lT_MeleeWeapons3 = LT_MeleeWeapons;
				type = LootTable.LootTableType.Melee;
				list8 = list11;
				List<FVRObject.OTagMeleeStyle> meleeStyles = new List<FVRObject.OTagMeleeStyle>
				{
					FVRObject.OTagMeleeStyle.Tool
				};
				List<FVRObject.OTagMeleeHandedness> meleeHandedness = new List<FVRObject.OTagMeleeHandedness>
				{
					FVRObject.OTagMeleeHandedness.OneHanded
				};
				lT_MeleeWeapons3.Initialize(type, list8, null, null, null, null, null, null, null, null, meleeStyles, meleeHandedness);
				LTChart_CaseWeapon.Add(LT_Firearms_Shotgun_LowCap);
				LTChart_CaseWeapon.Add(LT_Firearms_SideArm);
				LTChart_CaseWeapon.Add(LT_Firearms_SecondaryWeapon_LowCap);
				LTChart_CaseWeapon.Add(LT_Firearms_PrimaryWeapon_LowCap);
				LTChart_CaseWeapon.Add(LT_Firearms_Shotgun_HighCap);
				m_ltStartMin = 0;
				m_ltStartMax = 2;
				m_ltDiffModMin = -2;
				m_ltDiffModMax = 2;
				m_ltDiffMinCap = 1;
				m_maxWaveType = 8;
				m_waveDifficultyMod = -2;
				m_botHealthMultiplier = 0.4f;
				m_lootThrownThreshold = 0.65f;
				m_lootRareAttachThreshold = 1.65f;
				m_lootCommonAttachThreshold = 1.4f;
			}
			else if (m_generatedStyle == 3)
			{
				List<FVRObject.OTagEra> list8 = new List<FVRObject.OTagEra>();
				list8.Add(FVRObject.OTagEra.Modern);
				list8.Add(FVRObject.OTagEra.PostWar);
				List<FVRObject.OTagEra> list12 = list8;
				LootTable lT_Firearms_SideArm4 = LT_Firearms_SideArm;
				type = LootTable.LootTableType.Firearm;
				list8 = list12;
				list = new List<FVRObject.OTagFirearmSize>
				{
					FVRObject.OTagFirearmSize.Pocket,
					FVRObject.OTagFirearmSize.Pistol,
					FVRObject.OTagFirearmSize.Compact
				};
				list3 = list4;
				lT_Firearms_SideArm4.Initialize(type, list8, list, null, null, null, null, null, list3, null, null, null, null, null, -1, 20);
				LootTable lT_Firearms_SecondaryWeapon_LowCap4 = LT_Firearms_SecondaryWeapon_LowCap;
				type = LootTable.LootTableType.Firearm;
				list8 = list12;
				list = new List<FVRObject.OTagFirearmSize>
				{
					FVRObject.OTagFirearmSize.Compact,
					FVRObject.OTagFirearmSize.Carbine,
					FVRObject.OTagFirearmSize.FullSize
				};
				list3 = list4;
				lT_Firearms_SecondaryWeapon_LowCap4.Initialize(type, list8, list, null, null, null, null, null, list3, null, null, null, null, null, -1, 32);
				LootTable lT_Firearms_SecondaryWeapon_HighCap3 = LT_Firearms_SecondaryWeapon_HighCap;
				type = LootTable.LootTableType.Firearm;
				list8 = list12;
				list = new List<FVRObject.OTagFirearmSize>
				{
					FVRObject.OTagFirearmSize.Compact,
					FVRObject.OTagFirearmSize.Carbine,
					FVRObject.OTagFirearmSize.FullSize
				};
				list3 = list4;
				lT_Firearms_SecondaryWeapon_HighCap3.Initialize(type, list8, list, null, null, null, null, null, list3, null, null, null, null, null, 33, 100);
				LootTable lT_Firearms_SniperRifle3 = LT_Firearms_SniperRifle;
				type = LootTable.LootTableType.Firearm;
				list8 = list12;
				list = new List<FVRObject.OTagFirearmSize>
				{
					FVRObject.OTagFirearmSize.Carbine,
					FVRObject.OTagFirearmSize.FullSize,
					FVRObject.OTagFirearmSize.Bulky
				};
				list3 = list6;
				List<FVRObject.OTagFirearmAction> actions = new List<FVRObject.OTagFirearmAction>
				{
					FVRObject.OTagFirearmAction.BoltAction
				};
				lT_Firearms_SniperRifle3.Initialize(type, list8, list, actions, null, null, null, null, list3);
				LootTable lT_Firearms_PrimaryWeapon_LowCap4 = LT_Firearms_PrimaryWeapon_LowCap;
				type = LootTable.LootTableType.Firearm;
				list8 = list12;
				list = new List<FVRObject.OTagFirearmSize>
				{
					FVRObject.OTagFirearmSize.Compact,
					FVRObject.OTagFirearmSize.Carbine,
					FVRObject.OTagFirearmSize.FullSize
				};
				actions = new List<FVRObject.OTagFirearmAction>
				{
					FVRObject.OTagFirearmAction.Automatic
				};
				list3 = list6;
				lT_Firearms_PrimaryWeapon_LowCap4.Initialize(type, list8, list, actions, null, null, null, null, list3, null, null, null, null, null, -1, 16);
				LootTable lT_Firearms_PrimaryWeapon_MedCap3 = LT_Firearms_PrimaryWeapon_MedCap;
				type = LootTable.LootTableType.Firearm;
				list8 = list12;
				list = new List<FVRObject.OTagFirearmSize>
				{
					FVRObject.OTagFirearmSize.Compact,
					FVRObject.OTagFirearmSize.Carbine,
					FVRObject.OTagFirearmSize.FullSize
				};
				actions = new List<FVRObject.OTagFirearmAction>
				{
					FVRObject.OTagFirearmAction.Automatic
				};
				list3 = list6;
				lT_Firearms_PrimaryWeapon_MedCap3.Initialize(type, list8, list, actions, null, null, null, null, list3, null, null, null, null, null, 17, 44);
				LootTable lT_Firearms_PrimaryWeapon_HighCap2 = LT_Firearms_PrimaryWeapon_HighCap;
				type = LootTable.LootTableType.Firearm;
				list8 = list12;
				list = new List<FVRObject.OTagFirearmSize>
				{
					FVRObject.OTagFirearmSize.Compact,
					FVRObject.OTagFirearmSize.Carbine,
					FVRObject.OTagFirearmSize.FullSize
				};
				actions = new List<FVRObject.OTagFirearmAction>
				{
					FVRObject.OTagFirearmAction.Automatic
				};
				list3 = list6;
				lT_Firearms_PrimaryWeapon_HighCap2.Initialize(type, list8, list, actions, null, null, null, null, list3, null, null, null, null, null, 45, 100);
				LootTable lT_Firearms_Shotgun_LowCap4 = LT_Firearms_Shotgun_LowCap;
				type = LootTable.LootTableType.Firearm;
				list8 = list12;
				list3 = list5;
				lT_Firearms_Shotgun_LowCap4.Initialize(type, list8, null, null, null, null, null, null, list3, null, null, null, null, null, -1, 8);
				LootTable lT_Firearms_Shotgun_HighCap3 = LT_Firearms_Shotgun_HighCap;
				type = LootTable.LootTableType.Firearm;
				list8 = list12;
				list3 = list5;
				lT_Firearms_Shotgun_HighCap3.Initialize(type, list8, null, null, null, null, null, null, list3, null, null, null, null, null, 9);
				LootTable lT_Firearms_SmallOrdnance3 = LT_Firearms_SmallOrdnance;
				type = LootTable.LootTableType.Firearm;
				list8 = list12;
				list = new List<FVRObject.OTagFirearmSize>
				{
					FVRObject.OTagFirearmSize.Compact,
					FVRObject.OTagFirearmSize.Carbine,
					FVRObject.OTagFirearmSize.FullSize
				};
				list3 = list7;
				lT_Firearms_SmallOrdnance3.Initialize(type, list8, list, null, null, null, null, null, list3);
				LT_FirearmsSpecial.Initialize(LootTable.LootTableType.Firearm, list12, sizes);
				LootTable lT_Grenades4 = LT_Grenades;
				type = LootTable.LootTableType.Thrown;
				List<FVRObject.OTagThrownType> thrownTypes = new List<FVRObject.OTagThrownType>
				{
					FVRObject.OTagThrownType.Pinned
				};
				lT_Grenades4.Initialize(type, null, null, null, null, null, null, null, null, null, null, null, null, thrownTypes);
				LootTable lT_CommonAttachments4 = LT_CommonAttachments;
				type = LootTable.LootTableType.Attachments;
				list8 = list12;
				List<FVRObject.OTagAttachmentFeature> features = new List<FVRObject.OTagAttachmentFeature>
				{
					FVRObject.OTagAttachmentFeature.BarrelExtension,
					FVRObject.OTagAttachmentFeature.Grip,
					FVRObject.OTagAttachmentFeature.IronSight,
					FVRObject.OTagAttachmentFeature.Reflex
				};
				lT_CommonAttachments4.Initialize(type, list8, null, null, null, null, null, null, null, features);
				LootTable lT_RareAttachments4 = LT_RareAttachments;
				type = LootTable.LootTableType.Attachments;
				list8 = list12;
				features = new List<FVRObject.OTagAttachmentFeature>
				{
					FVRObject.OTagAttachmentFeature.Laser,
					FVRObject.OTagAttachmentFeature.Magnification,
					FVRObject.OTagAttachmentFeature.Suppression,
					FVRObject.OTagAttachmentFeature.Stock
				};
				lT_RareAttachments4.Initialize(type, list8, null, null, null, null, null, null, null, features);
				LootTable lT_MeleeWeapons4 = LT_MeleeWeapons;
				type = LootTable.LootTableType.Melee;
				list8 = list12;
				List<FVRObject.OTagMeleeStyle> meleeStyles = new List<FVRObject.OTagMeleeStyle>
				{
					FVRObject.OTagMeleeStyle.Tactical
				};
				List<FVRObject.OTagMeleeHandedness> meleeHandedness = new List<FVRObject.OTagMeleeHandedness>
				{
					FVRObject.OTagMeleeHandedness.OneHanded
				};
				lT_MeleeWeapons4.Initialize(type, list8, null, null, null, null, null, null, null, null, meleeStyles, meleeHandedness);
				LootTable lT_RequiredAttachments2 = LT_RequiredAttachments;
				type = LootTable.LootTableType.Attachments;
				list8 = list12;
				features = new List<FVRObject.OTagAttachmentFeature>
				{
					FVRObject.OTagAttachmentFeature.IronSight,
					FVRObject.OTagAttachmentFeature.Reflex
				};
				List<FVRObject.OTagFirearmMount> mounts = new List<FVRObject.OTagFirearmMount>
				{
					FVRObject.OTagFirearmMount.Picatinny
				};
				lT_RequiredAttachments2.Initialize(type, list8, null, null, null, null, null, mounts, null, features);
				LTChart_CaseWeapon.Add(LT_Firearms_SideArm);
				LTChart_CaseWeapon.Add(LT_Firearms_Shotgun_LowCap);
				LTChart_CaseWeapon.Add(LT_Firearms_SecondaryWeapon_LowCap);
				LTChart_CaseWeapon.Add(LT_Firearms_SniperRifle);
				LTChart_CaseWeapon.Add(LT_Firearms_PrimaryWeapon_LowCap);
				LTChart_CaseWeapon.Add(LT_Firearms_SmallOrdnance);
				LTChart_CaseWeapon.Add(LT_Firearms_PrimaryWeapon_MedCap);
				LTChart_CaseWeapon.Add(LT_Firearms_SecondaryWeapon_HighCap);
				LTChart_CaseWeapon.Add(LT_Firearms_Shotgun_HighCap);
				LTChart_CaseWeapon.Add(LT_Firearms_PrimaryWeapon_HighCap);
				m_ltStartMin = 0;
				m_ltStartMax = 2;
				m_ltDiffModMin = -2;
				m_ltDiffModMax = 2;
				m_ltDiffMinCap = 5;
				m_maxWaveType = 18;
				m_waveDifficultyMod = 0;
				m_botHealthMultiplier = 1f;
				m_lootThrownThreshold = 0.75f;
				m_lootRareAttachThreshold = 0.65f;
				m_lootCommonAttachThreshold = 0.4f;
			}
			else if (m_generatedStyle == 4)
			{
				List<FVRObject.OTagEra> list8 = new List<FVRObject.OTagEra>();
				list8.Add(FVRObject.OTagEra.Modern);
				list8.Add(FVRObject.OTagEra.PostWar);
				list8.Add(FVRObject.OTagEra.Colonial);
				list8.Add(FVRObject.OTagEra.TurnOfTheCentury);
				list8.Add(FVRObject.OTagEra.WildWest);
				list8.Add(FVRObject.OTagEra.WW1);
				list8.Add(FVRObject.OTagEra.WW2);
				List<FVRObject.OTagEra> list13 = list8;
				LT_Firearms_SideArm.Initialize(LootTable.LootTableType.Firearm, list13, new List<FVRObject.OTagFirearmSize>
				{
					FVRObject.OTagFirearmSize.Pocket,
					FVRObject.OTagFirearmSize.Pistol
				}, null, null, null, null, null, null, null, null, null, null, null, -1, 20);
				LootTable lT_Grenades5 = LT_Grenades;
				type = LootTable.LootTableType.Thrown;
				List<FVRObject.OTagThrownType> thrownTypes = new List<FVRObject.OTagThrownType>
				{
					FVRObject.OTagThrownType.Pinned
				};
				lT_Grenades5.Initialize(type, null, null, null, null, null, null, null, null, null, null, null, null, thrownTypes);
				LootTable lT_CommonAttachments5 = LT_CommonAttachments;
				type = LootTable.LootTableType.Attachments;
				list8 = list13;
				List<FVRObject.OTagAttachmentFeature> features = new List<FVRObject.OTagAttachmentFeature>
				{
					FVRObject.OTagAttachmentFeature.BarrelExtension,
					FVRObject.OTagAttachmentFeature.Grip,
					FVRObject.OTagAttachmentFeature.IronSight,
					FVRObject.OTagAttachmentFeature.Reflex
				};
				lT_CommonAttachments5.Initialize(type, list8, null, null, null, null, null, null, null, features);
				LootTable lT_RareAttachments5 = LT_RareAttachments;
				type = LootTable.LootTableType.Attachments;
				list8 = list13;
				features = new List<FVRObject.OTagAttachmentFeature>
				{
					FVRObject.OTagAttachmentFeature.Laser,
					FVRObject.OTagAttachmentFeature.Magnification,
					FVRObject.OTagAttachmentFeature.Suppression,
					FVRObject.OTagAttachmentFeature.Stock
				};
				lT_RareAttachments5.Initialize(type, list8, null, null, null, null, null, null, null, features);
				LootTable lT_MeleeWeapons5 = LT_MeleeWeapons;
				type = LootTable.LootTableType.Melee;
				list8 = list13;
				List<FVRObject.OTagMeleeStyle> meleeStyles = new List<FVRObject.OTagMeleeStyle>
				{
					FVRObject.OTagMeleeStyle.Tactical
				};
				List<FVRObject.OTagMeleeHandedness> meleeHandedness = new List<FVRObject.OTagMeleeHandedness>
				{
					FVRObject.OTagMeleeHandedness.OneHanded
				};
				lT_MeleeWeapons5.Initialize(type, list8, null, null, null, null, null, null, null, null, meleeStyles, meleeHandedness);
				LootTable lT_RequiredAttachments3 = LT_RequiredAttachments;
				type = LootTable.LootTableType.Attachments;
				list8 = list13;
				features = new List<FVRObject.OTagAttachmentFeature>
				{
					FVRObject.OTagAttachmentFeature.IronSight,
					FVRObject.OTagAttachmentFeature.Reflex
				};
				List<FVRObject.OTagFirearmMount> mounts = new List<FVRObject.OTagFirearmMount>
				{
					FVRObject.OTagFirearmMount.Picatinny
				};
				lT_RequiredAttachments3.Initialize(type, list8, null, null, null, null, null, mounts, null, features);
				LTChart_CaseWeapon.Add(LT_Firearms_SideArm);
				LTChart_CaseWeapon.Add(LT_Firearms_SideArm);
				LTChart_CaseWeapon.Add(LT_Firearms_SideArm);
				m_ltStartMin = 0;
				m_ltStartMax = 2;
				m_ltDiffModMin = -2;
				m_ltDiffModMax = 2;
				m_ltDiffMinCap = 1;
				m_maxWaveType = 18;
				m_waveDifficultyMod = 0;
				m_botHealthMultiplier = 1f;
				m_lootThrownThreshold = 0.75f;
				m_lootRareAttachThreshold = 0.65f;
				m_lootCommonAttachThreshold = 0.4f;
			}
			else if (m_generatedStyle == 5)
			{
				List<FVRObject.OTagEra> list8 = new List<FVRObject.OTagEra>();
				list8.Add(FVRObject.OTagEra.Modern);
				list8.Add(FVRObject.OTagEra.PostWar);
				list8.Add(FVRObject.OTagEra.TurnOfTheCentury);
				list8.Add(FVRObject.OTagEra.WildWest);
				list8.Add(FVRObject.OTagEra.WW1);
				list8.Add(FVRObject.OTagEra.WW2);
				list8.Add(FVRObject.OTagEra.Futuristic);
				List<FVRObject.OTagEra> list14 = list8;
				LT_Firearms_SideArm.Initialize(LootTable.LootTableType.Firearm, list14, new List<FVRObject.OTagFirearmSize>
				{
					FVRObject.OTagFirearmSize.Pocket,
					FVRObject.OTagFirearmSize.Pistol,
					FVRObject.OTagFirearmSize.Compact,
					FVRObject.OTagFirearmSize.Carbine,
					FVRObject.OTagFirearmSize.FullSize,
					FVRObject.OTagFirearmSize.Bulky
				});
				LT_FirearmsSpecial.Initialize(LootTable.LootTableType.Firearm, list14, sizes);
				LT_Grenades.Initialize(LootTable.LootTableType.Thrown, list14);
				LootTable lT_CommonAttachments6 = LT_CommonAttachments;
				type = LootTable.LootTableType.Attachments;
				list8 = list14;
				List<FVRObject.OTagAttachmentFeature> features = new List<FVRObject.OTagAttachmentFeature>
				{
					FVRObject.OTagAttachmentFeature.BarrelExtension,
					FVRObject.OTagAttachmentFeature.Grip,
					FVRObject.OTagAttachmentFeature.IronSight,
					FVRObject.OTagAttachmentFeature.Reflex
				};
				lT_CommonAttachments6.Initialize(type, list8, null, null, null, null, null, null, null, features);
				LootTable lT_RareAttachments6 = LT_RareAttachments;
				type = LootTable.LootTableType.Attachments;
				list8 = list14;
				features = new List<FVRObject.OTagAttachmentFeature>
				{
					FVRObject.OTagAttachmentFeature.Laser,
					FVRObject.OTagAttachmentFeature.Magnification,
					FVRObject.OTagAttachmentFeature.Suppression,
					FVRObject.OTagAttachmentFeature.Stock
				};
				lT_RareAttachments6.Initialize(type, list8, null, null, null, null, null, null, null, features);
				LT_MeleeWeapons.Initialize(LootTable.LootTableType.Melee, list14);
				LootTable lT_RequiredAttachments4 = LT_RequiredAttachments;
				type = LootTable.LootTableType.Attachments;
				list8 = list14;
				features = new List<FVRObject.OTagAttachmentFeature>
				{
					FVRObject.OTagAttachmentFeature.IronSight,
					FVRObject.OTagAttachmentFeature.Reflex
				};
				List<FVRObject.OTagFirearmMount> mounts = new List<FVRObject.OTagFirearmMount>
				{
					FVRObject.OTagFirearmMount.Picatinny
				};
				lT_RequiredAttachments4.Initialize(type, list8, null, null, null, null, null, mounts, null, features);
				LTChart_CaseWeapon.Add(LT_Firearms_SideArm);
				LTChart_CaseWeapon.Add(LT_Firearms_SideArm);
				LTChart_CaseWeapon.Add(LT_Firearms_SideArm);
				m_ltStartMin = 0;
				m_ltStartMax = 2;
				m_ltDiffModMin = -2;
				m_ltDiffModMax = 2;
				m_ltDiffMinCap = 5;
				m_maxWaveType = 18;
				m_waveDifficultyMod = 0;
				m_botHealthMultiplier = 1f;
				m_lootThrownThreshold = 0.75f;
				m_lootRareAttachThreshold = 0.65f;
				m_lootCommonAttachThreshold = 0.4f;
			}
			LootTable lT_Health = LT_Health;
			type = LootTable.LootTableType.Powerup;
			List<FVRObject.OTagPowerupType> powerupTypes = new List<FVRObject.OTagPowerupType>
			{
				FVRObject.OTagPowerupType.Health
			};
			lT_Health.Initialize(type, null, null, null, null, null, null, null, null, null, null, null, powerupTypes);
			LootTable lT_PowerUps = LT_PowerUps;
			type = LootTable.LootTableType.Powerup;
			powerupTypes = new List<FVRObject.OTagPowerupType>
			{
				FVRObject.OTagPowerupType.GhostMode,
				FVRObject.OTagPowerupType.InfiniteAmmo,
				FVRObject.OTagPowerupType.Invincibility,
				FVRObject.OTagPowerupType.QuadDamage
			};
			lT_PowerUps.Initialize(type, null, null, null, null, null, null, null, null, null, null, null, powerupTypes);
		}

		public void BeginGame()
		{
			if (m_generatedStyle != GM.TAHSettings.TAHOption_LootProgression)
			{
				GenerateLootTables();
			}
			TAHMenu.SetActive(value: false);
			ItemSpawnerState(b: false);
			m_curSupplyPointIndex = GetValidSupplyPointIndex();
			m_lastSupplyPointIndex = m_curSupplyPointIndex;
			TAH_SupplyPoint tAH_SupplyPoint = SupplyPoints[m_curSupplyPointIndex];
			SpawnEquipmentAtPointFromLootTables(tAH_SupplyPoint, isStart: true);
			GM.CurrentMovementManager.TeleportToPoint(tAH_SupplyPoint.PlayerSpawnPoint.position, isAbsolute: true, tAH_SupplyPoint.PlayerSpawnPoint.forward);
			UpdateHealth();
			InitiateTake(isStart: true);
		}

		public void TouchToEndTake()
		{
			if (State == TAHGameState.Taking)
			{
				EndTake();
			}
		}

		public void UpdateHealth()
		{
			if (GM.TAHSettings.TAHOption_PlayerHealth == 0)
			{
				GM.CurrentPlayerBody.SetHealthThreshold(2000f);
			}
			else if (GM.TAHSettings.TAHOption_PlayerHealth == 1)
			{
				GM.CurrentPlayerBody.SetHealthThreshold(10000f);
			}
			else if (GM.TAHSettings.TAHOption_PlayerHealth == 2)
			{
				GM.CurrentPlayerBody.SetHealthThreshold(10f);
			}
			else if (GM.TAHSettings.TAHOption_PlayerHealth == 3)
			{
				GM.CurrentPlayerBody.SetHealthThreshold(4000f);
			}
			else if (GM.TAHSettings.TAHOption_PlayerHealth == 4)
			{
				GM.CurrentPlayerBody.SetHealthThreshold(40000f);
			}
			else if (GM.TAHSettings.TAHOption_PlayerHealth == 5)
			{
				GM.CurrentPlayerBody.SetHealthThreshold(100000f);
			}
		}

		private void Update()
		{
			switch (State)
			{
			case TAHGameState.Taking:
				UpdateGameState_Taking();
				break;
			case TAHGameState.Holding:
				UpdateGameState_Holding();
				break;
			}
			BotCleanup();
			if (GM.TAHSettings.TAHOption_Music == 1)
			{
				FMODController.Tick(Time.deltaTime);
			}
		}

		private void BotCleanup()
		{
			if (SpawnedBots.Count > 0)
			{
				for (int num = SpawnedBots.Count - 1; num >= 0; num--)
				{
					if (SpawnedBots[num] == null)
					{
						SpawnedBots.RemoveAt(num);
					}
				}
			}
			if (SpawnedBots_Attacking.Count <= 0)
			{
				return;
			}
			for (int num2 = SpawnedBots_Attacking.Count - 1; num2 >= 0; num2--)
			{
				if (SpawnedBots_Attacking[num2] == null)
				{
					SpawnedBots_Attacking.RemoveAt(num2);
				}
			}
		}

		private void ObjectCleanup()
		{
			if (m_knownObjs.Count <= 0)
			{
				return;
			}
			knownObjectCheckIndex++;
			if (knownObjectCheckIndex >= m_knownObjs.Count)
			{
				knownObjectCheckIndex = 0;
			}
			if (m_knownObjs[knownObjectCheckIndex] == null)
			{
				m_knownObjsHash.Remove(m_knownObjs[knownObjectCheckIndex]);
				m_knownObjs.RemoveAt(knownObjectCheckIndex);
				return;
			}
			float num = Vector3.Distance(GM.CurrentPlayerBody.transform.position, m_knownObjs[knownObjectCheckIndex].transform.position);
			if (num > 30f)
			{
				m_knownObjsHash.Remove(m_knownObjs[knownObjectCheckIndex]);
				Object.Destroy(m_knownObjs[knownObjectCheckIndex].gameObject);
				m_knownObjs.RemoveAt(knownObjectCheckIndex);
			}
		}

		private void UpdateGameState_Taking()
		{
			TAH_DefensePoint tAH_DefensePoint = DefensePoints[m_curDefensePointIndex];
			TAH_SupplyPoint tAH_SupplyPoint = SupplyPoints[m_curSupplyPointIndex];
			Vector3 vector = GM.CurrentPlayerBody.LeftHand.position + GM.CurrentPlayerBody.LeftHand.forward * -0.2f;
			if (UsesOldReticle)
			{
				Reticle_Root.transform.position = vector;
				Reticle_Root.transform.localScale = Vector3.one * 0.08f;
			}
			else
			{
				TAHReticle.transform.position = vector;
			}
			Vector3 forward = tAH_DefensePoint.PointCircle.transform.position - vector;
			Vector3 forward2 = tAH_SupplyPoint.PlayerSpawnPoint.transform.position - vector;
			forward.Normalize();
			forward2.Normalize();
			Reticle_TAH.rotation = Quaternion.LookRotation(forward, Vector3.up);
			Reticle_Supply.rotation = Quaternion.LookRotation(forward2, Vector3.up);
			if (LogicStyle == TAHLogicStyle.HL2)
			{
				if (m_timeUntilWeCanEventCheck > 0f)
				{
					m_timeUntilWeCanEventCheck -= Time.deltaTime;
				}
				else
				{
					m_checkIndex_SupplyPoint++;
					if (m_checkIndex_SupplyPoint >= SupplyPoints.Count)
					{
						m_checkIndex_SupplyPoint = 0;
					}
					if (!m_eventFiredSoFar_SupplyPoint.Contains(SupplyPoints[m_checkIndex_SupplyPoint]))
					{
						float num = Vector3.Distance(SupplyPoints[m_checkIndex_SupplyPoint].transform.position, GM.CurrentPlayerBody.transform.position);
						if (num < 3f)
						{
							TriggerEvent_SupplyPoint(SupplyPoints[m_checkIndex_SupplyPoint]);
							m_timeUntilWeCanEventCheck = Random.Range(m_eventTriggerTickRange.x, m_eventTriggerTickRange.y);
							m_eventFiredSoFar_SupplyPoint.Add(SupplyPoints[m_checkIndex_SupplyPoint]);
						}
					}
				}
			}
			if (m_botKillThreshold > 0.5f && m_fireThreshold > 10f)
			{
				m_takeMusicIntensityTarget = 2f;
			}
			else
			{
				m_takeMusicIntensityTarget = 1f;
			}
			m_takeMusicIntensity = Mathf.MoveTowards(m_takeMusicIntensity, m_takeMusicIntensityTarget, Time.deltaTime * 0.5f);
			if (m_fireThreshold > 0f)
			{
				m_fireThreshold -= Time.deltaTime;
			}
			else if (m_botKillThreshold > 0f)
			{
				m_botKillThreshold -= Time.deltaTime * 1f;
			}
			if (GM.TAHSettings.TAHOption_Music == 1)
			{
				FMODController.SetIntParameterByIndex(0, "Intensity", m_takeMusicIntensity);
			}
		}

		public void TriggerEvent_SupplyPoint(TAH_SupplyPoint point)
		{
			float num = Random.Range((float)GetDifficulty() * 0.25f, (float)GetDifficulty() + 3f);
			if (num > 18f)
			{
				int num2 = Random.Range(2, point.BotAttackSpawnPoints.Length);
				for (int i = 0; i < point.BotAttackSpawnPoints.Length; i++)
				{
					int num3 = (int)Random.Range((float)GetDifficulty() - 15f, (float)GetDifficulty() * 1f);
					if (num3 < 0)
					{
						num3 = 0;
					}
					if (num3 >= EventProfiles_FastZombies.Length)
					{
						num3 = EventProfiles_FastZombies.Length - 1;
					}
					TAH_BotSpawnProfile tAH_BotSpawnProfile = EventProfiles_FastZombies[num3];
					SpawnBot(tAH_BotSpawnProfile.GetRandomPrefab(), tAH_BotSpawnProfile.GetRandomWeapon(), tAH_BotSpawnProfile.GetRandomConfig(), point.BotAttackSpawnPoints[i], isAggro: true, isStatic: false, point.NavGroup, isHoldBot: false, tAH_BotSpawnProfile.GetRandomSecondaryWeapon());
					num2--;
					if (num2 <= 0)
					{
						break;
					}
				}
			}
			else if (num > 12f)
			{
				for (int j = 0; j < point.BotAttackSpawnPoints.Length; j++)
				{
					Transform transform = point.BotAttackSpawnPoints[j];
					GameObject item = Object.Instantiate(EventPrefabs_MeatCrabs[1], transform.position + Vector3.up * 0.3f, transform.rotation);
					m_spawnedMobs.Add(item);
				}
			}
			else if (num > 8f)
			{
				int num4 = Random.Range(2, point.BotAttackSpawnPoints.Length);
				for (int k = 0; k < point.BotAttackSpawnPoints.Length; k++)
				{
					int num5 = Random.Range(0, EventProfiles_SpecOps.Length);
					TAH_BotSpawnProfile tAH_BotSpawnProfile2 = EventProfiles_SpecOps[num5];
					SpawnBot(tAH_BotSpawnProfile2.GetRandomPrefab(), tAH_BotSpawnProfile2.GetRandomWeapon(), tAH_BotSpawnProfile2.GetRandomConfig(), point.BotAttackSpawnPoints[k], isAggro: true, isStatic: false, point.NavGroup, isHoldBot: false, tAH_BotSpawnProfile2.GetRandomSecondaryWeapon());
					num4--;
					if (num4 <= 0)
					{
						break;
					}
				}
			}
			else if (num > 4f)
			{
				int num6 = Random.Range(1, point.BotAttackSpawnPoints.Length);
				for (int l = 0; l < point.BotAttackSpawnPoints.Length; l++)
				{
					int num7 = (int)Random.Range((float)GetDifficulty() - 1f, (float)GetDifficulty() * 1f);
					if (num7 < 0)
					{
						num7 = 0;
					}
					if (num7 >= EventProfiles_MeatZombies.Length)
					{
						num7 = EventProfiles_MeatZombies.Length - 1;
					}
					TAH_BotSpawnProfile tAH_BotSpawnProfile3 = EventProfiles_MeatZombies[num7];
					SpawnBot(tAH_BotSpawnProfile3.GetRandomPrefab(), tAH_BotSpawnProfile3.GetRandomWeapon(), tAH_BotSpawnProfile3.GetRandomConfig(), point.BotAttackSpawnPoints[l], isAggro: true, isStatic: false, point.NavGroup, isHoldBot: false, tAH_BotSpawnProfile3.GetRandomSecondaryWeapon());
					num6--;
					if (num6 <= 0)
					{
						break;
					}
				}
			}
			else
			{
				for (int m = 0; m < point.BotAttackSpawnPoints.Length; m++)
				{
					Transform transform2 = point.BotAttackSpawnPoints[m];
					GameObject item2 = Object.Instantiate(EventPrefabs_MeatCrabs[0], transform2.position + Vector3.up * 0.3f, transform2.rotation);
					m_spawnedMobs.Add(item2);
				}
			}
		}

		private void TakingBotKill()
		{
			m_botKillThreshold += 3f;
			m_botKillThreshold = Mathf.Clamp(m_botKillThreshold, 0f, 10f);
		}

		private void TakingGunShot()
		{
			m_fireThreshold += 3f;
			m_fireThreshold = Mathf.Clamp(m_fireThreshold, 0f, 20f);
		}

		private void UpdateGameState_Holding()
		{
			ObjectCleanup();
			m_holdTimer -= Time.deltaTime;
			if (m_holdTimer <= -20f)
			{
				EndHold();
				return;
			}
			TAH_DefensePoint tAH_DefensePoint = DefensePoints[m_curDefensePointIndex];
			PingTimer -= Time.deltaTime;
			if (PingTimer < 0f)
			{
				PingTimer = Random.Range(10f, 15f);
				GM.CurrentSceneSettings.PingReceivers(tAH_DefensePoint.PointCircle.transform.position);
			}
			m_waveTimer -= Time.deltaTime;
			TAH_DefensePoint.TAH_BotSpawner spawner = tAH_DefensePoint.Spawner;
			spawner.TimeTilSpawnBot -= Time.deltaTime;
			if (spawner.TimeTilSpawnBot <= 0f && spawner.NumLeftToSpawn > 0)
			{
				spawner.NumLeftToSpawn--;
				tAH_DefensePoint.Spawner.TimeTilSpawnBot = spawner.SpawnCooldownTime;
				Transform point = spawner.SpawnPoints[spawner.GetSpawnPointIndex()];
				SpawnBot(spawner.SpawnProfile.GetRandomPrefab(), spawner.SpawnProfile.GetRandomWeapon(), spawner.SpawnProfile.GetRandomConfig(), point, isAggro: true, isStatic: false, tAH_DefensePoint.NavGroup, isHoldBot: true, spawner.SpawnProfile.GetRandomSecondaryWeapon());
			}
			if (SpawnedBots_Attacking.Count == 0 && spawner.NumLeftToSpawn == 0 && m_waveTimer > 5f)
			{
				m_waveTimer -= Time.deltaTime * 1f;
				m_holdTimer -= Time.deltaTime * 1f;
			}
			if (m_waveTimer < 0f && m_currentWave < m_maxWave)
			{
				m_currentWave++;
				tAH_DefensePoint.BeginWave(m_holdSequence[m_currentWave]);
				m_waveTimer = m_holdSequence[m_currentWave].TimeForWave;
				if (GM.TAHSettings.TAHOption_Music == 1)
				{
					FMODController.SetIntParameterByIndex(1, "Intensity", m_holdSequence[m_currentWave].WaveIntensity);
				}
			}
			Vector3 position = GM.CurrentPlayerBody.LeftHand.position + GM.CurrentPlayerBody.LeftHand.forward * -0.2f;
			if (!UsesOldReticle)
			{
				TAHReticle.transform.position = position;
			}
			if (m_currentWave == m_maxWave && SpawnedBots_Attacking.Count == 0)
			{
				bool flag = false;
				if (spawner.NumLeftToSpawn == 0)
				{
					flag = true;
				}
				if (flag)
				{
					EndHold();
				}
			}
		}

		private void InitiateTake(bool isStart)
		{
			m_takeMusicIntensity = 1f;
			m_takeMusicIntensityTarget = 1f;
			m_fireThreshold = 0f;
			m_botKillThreshold = 0f;
			State = TAHGameState.Taking;
			if (GM.TAHSettings.TAHOption_Music == 1)
			{
				FMODController.SwitchTo(0, 2f, shouldStop: false, shouldDeadStop: false);
			}
			int validSupplyPointIndex = GetValidSupplyPointIndex();
			int validDefensePointIndex = GetValidDefensePointIndex();
			m_lastSupplyPointIndex = m_curSupplyPointIndex;
			m_lastDefensePointIndex = m_curDefensePointIndex;
			m_curSupplyPointIndex = validSupplyPointIndex;
			m_curDefensePointIndex = validDefensePointIndex;
			TAH_SupplyPoint tAH_SupplyPoint = SupplyPoints[m_curSupplyPointIndex];
			TAH_DefensePoint tAH_DefensePoint = DefensePoints[m_curDefensePointIndex];
			SpawnEquipmentAtPointFromLootTables(tAH_SupplyPoint, isStart: false);
			SpawnBotsAtSupplyPoint(tAH_SupplyPoint, tAH_SupplyPoint);
			SpawnBotsAtDefensePoint(tAH_DefensePoint, tAH_DefensePoint);
			if (LogicStyle == TAHLogicStyle.Classic)
			{
				if (!isStart)
				{
					int validSupplyPointIndex2 = GetValidSupplyPointIndex();
					int validSupplyPointIndex3 = GetValidSupplyPointIndex(validSupplyPointIndex2);
					SpawnBotsAtSupplyPoint(SupplyPoints[validSupplyPointIndex2], SupplyPoints[validSupplyPointIndex3]);
					SpawnBotsAtSupplyPoint(SupplyPoints[validSupplyPointIndex3], SupplyPoints[validSupplyPointIndex2]);
				}
				int validDefensePointIndex2 = GetValidDefensePointIndex();
				int validDefensePointIndex3 = GetValidDefensePointIndex(validDefensePointIndex2);
				SpawnBotsAtDefensePoint(DefensePoints[validDefensePointIndex2], DefensePoints[validDefensePointIndex2]);
				SpawnBotsAtDefensePoint(DefensePoints[validDefensePointIndex3], DefensePoints[validDefensePointIndex3]);
			}
			else if (LogicStyle == TAHLogicStyle.HL2)
			{
				StartCoroutine(LevelConfiguration_Take(isStart));
			}
			m_eventFiredSoFar_SupplyPoint.Clear();
			m_timeUntilWeCanEventCheck = Random.Range(m_eventTriggerTickRange.x, m_eventTriggerTickRange.y);
			m_checkIndex_SupplyPoint = 0;
			if (isStart && LogicStyle == TAHLogicStyle.HL2)
			{
				m_eventFiredSoFar_SupplyPoint.Add(SupplyPoints[m_lastSupplyPointIndex]);
			}
			tAH_DefensePoint.PointCircle.SetActive(value: true);
			tAH_DefensePoint.BeginTouch.SetActive(value: true);
			if (UsesOldReticle)
			{
				Reticle_Root.gameObject.SetActive(value: true);
			}
			else
			{
				TAHReticle.DeRegisterTrackedType(TAH_ReticleContact.ContactType.Hold);
				TAHReticle.RegisterTrackedObject(tAH_DefensePoint.transform, TAH_ReticleContact.ContactType.Hold);
				TAHReticle.DeRegisterTrackedType(TAH_ReticleContact.ContactType.Supply);
				TAHReticle.RegisterTrackedObject(tAH_SupplyPoint.transform, TAH_ReticleContact.ContactType.Supply);
			}
			m_pointsTaken++;
			if (GM.TAHSettings.TAHOption_DifficultyProgression > 0)
			{
				m_pointsTaken++;
			}
		}

		private IEnumerator LevelConfiguration_Take(bool isStart)
		{
			yield return 0;
			if (!isStart)
			{
				int iS1 = GetValidSupplyPointIndex();
				int iS2 = GetValidSupplyPointIndex(iS1);
				SpawnBotsAtSupplyPoint(SupplyPoints[iS1], SupplyPoints[iS2]);
				yield return 0;
				SpawnBotsAtSupplyPoint(SupplyPoints[iS2], SupplyPoints[iS1]);
			}
			yield return 0;
			int iD1 = GetValidDefensePointIndex();
			int iD2 = GetValidDefensePointIndex(iD1);
			SpawnBotsAtDefensePoint(DefensePoints[iD1], DefensePoints[iD1]);
			yield return 0;
			SpawnBotsAtDefensePoint(DefensePoints[iD2], DefensePoints[iD2]);
			yield return new WaitForSeconds(0.5f);
			Vector3 pp = GM.CurrentPlayerBody.transform.position;
			for (int i = 0; i < MobSpawnGroups.Count; i++)
			{
				float d = GetDifficulty();
				if (MobSpawnGroups[i].GetShouldSpawn(d, pp))
				{
					List<GameObject> gs = MobSpawnGroups[i].SpawnMobs(d);
					for (int j = 0; j < gs.Count; j++)
					{
						m_spawnedMobs.Add(gs[j]);
					}
					yield return 0;
				}
			}
			yield return null;
		}

		private void InitiateHold()
		{
			State = TAHGameState.Holding;
			if (GM.TAHSettings.TAHOption_Music == 1)
			{
				FMODController.SwitchTo(1, 0f, shouldStop: true, shouldDeadStop: false);
			}
			if (GM.TAHSettings.TAHOption_Music == 1)
			{
				FMODController.SetIntParameterByIndex(1, "Intensity", 2f);
			}
			TAH_SupplyPoint tAH_SupplyPoint = SupplyPoints[m_curSupplyPointIndex];
			TAH_DefensePoint tAH_DefensePoint = DefensePoints[m_curDefensePointIndex];
			tAH_DefensePoint.InitiateDefense();
			int num = Mathf.RoundToInt(Mathf.Lerp(3f, 10f, (float)GetDifficulty() * 0.08f));
			m_holdSequence.Clear();
			int difficulty = GetDifficulty();
			difficulty += m_waveDifficultyMod;
			difficulty = Mathf.Clamp(difficulty, 0, (int)((double)(float)m_maxWaveType * 1.5));
			m_holdTimer = 0f;
			for (int i = 0; i < num; i++)
			{
				int min = Random.Range((int)((float)difficulty * 0.15f), (int)((float)difficulty * 0.4f));
				int value = Mathf.Min(WaveDefinitions.Count, difficulty + 2);
				value = Mathf.Clamp(value, 0, m_maxWaveType);
				m_holdSequence.Add(WaveDefinitions[Random.Range(min, value)]);
			}
			for (int j = 0; j < m_holdSequence.Count; j++)
			{
				m_holdTimer += m_holdSequence[j].TimeForWave;
			}
			m_holdTimer += 5f;
			m_currentWave = 0;
			m_maxWave = m_holdSequence.Count - 1;
			tAH_DefensePoint.BeginWave(m_holdSequence[m_currentWave]);
			m_waveTimer = m_holdSequence[m_currentWave].TimeForWave;
			if (UsesOldReticle)
			{
				Reticle_Root.gameObject.SetActive(value: false);
			}
		}

		public void EndTake()
		{
			ClearActiveBots();
			InitiateHold();
		}

		public void EndHold()
		{
			TAH_SupplyPoint tAH_SupplyPoint = SupplyPoints[m_curSupplyPointIndex];
			TAH_DefensePoint tAH_DefensePoint = DefensePoints[m_curDefensePointIndex];
			tAH_DefensePoint.EndDefense();
			ClearActiveBots();
			ObjectCleanup();
			InitiateTake(isStart: false);
		}

		private void SpawnEquipmentAtPointFromLootTables(TAH_SupplyPoint point, bool isStart)
		{
			int difficulty = GetDifficulty();
			int value = difficulty + m_ltDiffModMin;
			int num = difficulty + m_ltDiffModMax;
			if (isStart)
			{
				value = m_ltStartMin;
				num = m_ltStartMax;
			}
			value = Mathf.Clamp(value, 0, m_ltDiffMinCap);
			num = Mathf.Clamp(num, num, LTChart_CaseWeapon.Count + m_ltDiffModMax);
			int value2 = Random.Range(value, num + 1);
			value2 = Mathf.Clamp(value2, 0, LTChart_CaseWeapon.Count - 1);
			LootTable lootTable = LTChart_CaseWeapon[value2];
			FVRObject randomObject = lootTable.GetRandomObject();
			FVRObject randomAmmoObject = lootTable.GetRandomAmmoObject(randomObject, lootTable.Eras);
			FVRObject fVRObject = null;
			FVRObject fVRObject2 = null;
			FVRObject fVRObject3 = null;
			if (randomObject.RequiredSecondaryPieces.Count > 0)
			{
				fVRObject = randomObject.RequiredSecondaryPieces[0];
				if (randomObject.RequiredSecondaryPieces.Count > 1)
				{
					fVRObject2 = randomObject.RequiredSecondaryPieces[1];
				}
				if (randomObject.RequiredSecondaryPieces.Count > 2)
				{
					fVRObject3 = randomObject.RequiredSecondaryPieces[2];
				}
			}
			else if (randomObject.RequiresPicatinnySight)
			{
				fVRObject = LT_RequiredAttachments.GetRandomObject();
				if (fVRObject.RequiredSecondaryPieces.Count > 0)
				{
					fVRObject2 = fVRObject.RequiredSecondaryPieces[0];
				}
			}
			else if (randomObject.BespokeAttachments.Count > 0)
			{
				float num2 = Random.Range(0f, 1f);
				if (num2 > 0.75f)
				{
					fVRObject = lootTable.GetRandomBespokeAttachment(randomObject);
					if (fVRObject.RequiredSecondaryPieces.Count > 0)
					{
						fVRObject2 = fVRObject.RequiredSecondaryPieces[0];
					}
				}
			}
			TAH_WeaponCrate tAH_WeaponCrate = null;
			if (randomObject.TagFirearmSize >= FVRObject.OTagFirearmSize.Compact)
			{
				if (point.CrateLarge != null)
				{
					tAH_WeaponCrate = point.CrateLarge;
				}
				else
				{
					GameObject gameObject = Object.Instantiate(TAHCratePrefabLarge, point.SpawnPos_CrateLarge.position, point.SpawnPos_CrateLarge.rotation);
					point.CrateLarge = gameObject.GetComponent<TAH_WeaponCrate>();
					tAH_WeaponCrate = point.CrateLarge;
				}
			}
			else if (point.CrateSmall != null)
			{
				tAH_WeaponCrate = point.CrateSmall;
			}
			else
			{
				GameObject gameObject2 = Object.Instantiate(TAHCratePrefabSmall, point.SpawnPos_CrateSmall.position, point.SpawnPos_CrateSmall.rotation);
				point.CrateSmall = gameObject2.GetComponent<TAH_WeaponCrate>();
				tAH_WeaponCrate = point.CrateSmall;
			}
			tAH_WeaponCrate.gameObject.SetActive(value: true);
			tAH_WeaponCrate.ResetCrate();
			tAH_WeaponCrate.Manager = this;
			GameObject gameObject3 = randomObject.GetGameObject();
			GameObject go_mag = null;
			if (randomAmmoObject != null)
			{
				go_mag = randomAmmoObject.GetGameObject();
			}
			GameObject go_attach = null;
			if (fVRObject != null)
			{
				go_attach = fVRObject.GetGameObject();
			}
			GameObject go_attach2 = null;
			if (fVRObject2 != null)
			{
				go_attach2 = fVRObject2.GetGameObject();
			}
			GameObject go_attach3 = null;
			if (fVRObject3 != null)
			{
				go_attach3 = fVRObject2.GetGameObject();
			}
			if (isStart)
			{
				FVRObject randomObject2 = LT_MeleeWeapons.GetRandomObject();
				GameObject g = Object.Instantiate(randomObject2.GetGameObject(), point.SpawnPoint_MeleeWeapon.position, point.SpawnPoint_MeleeWeapon.rotation);
				AddObjectToTrackedList(g);
			}
			else
			{
				float num3 = Random.Range(0f, 1f);
				if (num3 > m_lootThrownThreshold)
				{
					if (LT_Grenades.Loot.Count > 0)
					{
						FVRObject randomObject3 = LT_Grenades.GetRandomObject();
						GameObject g2 = Object.Instantiate(randomObject3.GetGameObject(), point.SpawnPoint_MeleeWeapon.position, point.SpawnPoint_MeleeWeapon.rotation);
						AddObjectToTrackedList(g2);
					}
				}
				else if (num3 > m_lootRareAttachThreshold)
				{
					if (LT_RareAttachments.Loot.Count > 0)
					{
						FVRObject randomObject4 = LT_RareAttachments.GetRandomObject();
						GameObject g3 = Object.Instantiate(randomObject4.GetGameObject(), point.SpawnPoint_MeleeWeapon.position, point.SpawnPoint_MeleeWeapon.rotation);
						AddObjectToTrackedList(g3);
					}
				}
				else if (num3 > m_lootCommonAttachThreshold && LT_CommonAttachments.Loot.Count > 0)
				{
					FVRObject randomObject5 = LT_CommonAttachments.GetRandomObject();
					GameObject g4 = Object.Instantiate(randomObject5.GetGameObject(), point.SpawnPoint_MeleeWeapon.position, point.SpawnPoint_MeleeWeapon.rotation);
					AddObjectToTrackedList(g4);
				}
			}
			if (LT_FirearmsSpecial.Loot.Count > 0)
			{
				float num4 = Random.Range(10f, 25f);
				if (num4 < (float)GetDifficulty())
				{
					float num5 = Random.Range(0f, 1f);
					if (num5 > 0.8f)
					{
						FVRObject randomObject6 = LT_FirearmsSpecial.GetRandomObject();
						GameObject g5 = Object.Instantiate(randomObject6.GetGameObject(), point.SpawnPoint_Large1.position, point.SpawnPoint_Large1.rotation);
						AddObjectToTrackedList(g5);
						FVRObject randomAmmoObject2 = LT_FirearmsSpecial.GetRandomAmmoObject(randomObject6);
						GameObject g6 = Object.Instantiate(randomAmmoObject2.GetGameObject(), point.SpawnPoint_Large2.position, point.SpawnPoint_Large2.rotation);
						AddObjectToTrackedList(g6);
					}
				}
			}
			tAH_WeaponCrate.PlaceItemsInCrate(gameObject3, go_mag, go_attach, go_attach2, go_attach3);
			float num6 = 10f - (float)GetDifficulty();
			Mathf.Clamp(num6, 0f, 10f);
			float num7 = Random.Range(-0.5f, 10f);
			if (num7 < num6)
			{
				float num8 = Random.Range(0f, 1f);
				FVRObject fVRObject4 = null;
				fVRObject4 = ((!(num8 > 0.3f)) ? LT_PowerUps.GetRandomObject() : LT_Health.GetRandomObject());
				GameObject g7 = Object.Instantiate(fVRObject4.GetGameObject(), point.SpawnPos_PowerUp.position, point.SpawnPos_PowerUp.rotation);
				AddObjectToTrackedList(g7);
			}
		}

		private void SpawnBotsAtSupplyPoint(TAH_SupplyPoint point, TAH_SupplyPoint moveToPoint)
		{
			for (int i = 0; i < point.BotSpawnPoints.Length; i++)
			{
				int num = Mathf.Clamp(Random.Range(0, (int)(float)GetDifficulty()), 0, SupplyPointBotProfiles.Length - 1);
				TAH_BotSpawnProfile tAH_BotSpawnProfile = SupplyPointBotProfiles[num];
				SpawnBot(tAH_BotSpawnProfile.GetRandomPrefab(), tAH_BotSpawnProfile.GetRandomWeapon(), tAH_BotSpawnProfile.GetRandomConfig(), point.BotSpawnPoints[i], isAggro: false, isStatic: false, moveToPoint.NavGroup, isHoldBot: false, tAH_BotSpawnProfile.GetRandomSecondaryWeapon());
			}
		}

		private void SpawnBotsAtDefensePoint(TAH_DefensePoint point, TAH_DefensePoint moveToPoint)
		{
			for (int i = 0; i < point.StaticBotSpawnPoints.Length; i++)
			{
				int num = Mathf.Clamp(Random.Range(0, (int)(float)GetDifficulty()), 0, SupplyPointBotProfiles.Length - 1);
				TAH_BotSpawnProfile tAH_BotSpawnProfile = DefensePointBotProfiles[num];
				SpawnBot(tAH_BotSpawnProfile.GetRandomPrefab(), tAH_BotSpawnProfile.GetRandomWeapon(), tAH_BotSpawnProfile.GetRandomConfig(), point.StaticBotSpawnPoints[i], isAggro: false, isStatic: false, moveToPoint.NavGroup, isHoldBot: false, tAH_BotSpawnProfile.GetRandomSecondaryWeapon());
			}
		}

		public void AddObjectToTrackedList(GameObject g)
		{
			FVRPhysicalObject component = g.GetComponent<FVRPhysicalObject>();
			if (component != null)
			{
				AddFVRObjectToTrackedList(component);
			}
		}

		private void AddFVRObjectToTrackedList(FVRPhysicalObject g)
		{
			if (m_knownObjsHash.Add(g))
			{
				m_knownObjs.Add(g);
			}
		}

		private void SpawnBot(GameObject prefab, GameObject weapon, wwBotWurstConfig config, Transform point, bool isAggro, bool isStatic, wwBotWurstNavPointGroup ngroup, bool isHoldBot, GameObject weapon2 = null)
		{
			GameObject gameObject = Object.Instantiate(prefab, point.position, point.rotation);
			wwBotWurst component = gameObject.GetComponent<wwBotWurst>();
			GameObject gameObject2 = Object.Instantiate(weapon, component.ModernGunMount.position, component.ModernGunMount.rotation);
			wwBotWurstModernGun component2 = gameObject2.GetComponent<wwBotWurstModernGun>();
			component.ModernGuns.Add(component2);
			component2.Bot = component;
			component2.transform.SetParent(component.ModernGunMount.parent);
			if (GM.TAHSettings.TAHOption_BotBullets > 0)
			{
				component2.SetUseFastProjectile(b: true);
			}
			if (weapon2 != null)
			{
				GameObject gameObject3 = Object.Instantiate(weapon2, component.ModernGunMount.position, component.ModernGunMount.rotation);
				wwBotWurstModernGun component3 = gameObject3.GetComponent<wwBotWurstModernGun>();
				component.ModernGuns.Add(component3);
				component3.Bot = component;
				component3.transform.SetParent(component.ModernGunMount.parent);
				if (GM.TAHSettings.TAHOption_BotBullets > 0)
				{
					component3.SetUseFastProjectile(b: true);
				}
			}
			component.Config = config;
			component.ReConfig(config, m_botHealthMultiplier);
			if (GM.TAHSettings.TAHOption_PlayerHealth == 0)
			{
				component.HealthOverride(500f, 1000f, 1000f);
			}
			component.NavPointGroup = ngroup;
			if (isAggro)
			{
				component.ShotEvent(GM.CurrentPlayerBody.transform.position);
			}
			if (isStatic)
			{
				component.State = wwBotWurst.BotState.StandingAround;
			}
			GM.CurrentSceneSettings.ShotEventReceivers.Add(gameObject);
			if (isHoldBot)
			{
				SpawnedBots_Attacking.Add(gameObject);
			}
			else
			{
				SpawnedBots.Add(gameObject);
			}
		}

		private void ClearActiveBots()
		{
			ClearStandardBots();
			ClearAttackingBots();
			ClearMobs();
			ClearEventMobs();
		}

		private void ClearStandardBots()
		{
			for (int num = SpawnedBots.Count - 1; num >= 0; num--)
			{
				if (SpawnedBots[num] != null)
				{
					Object.Destroy(SpawnedBots[num]);
				}
			}
			SpawnedBots.Clear();
		}

		private void ClearAttackingBots()
		{
			for (int num = SpawnedBots_Attacking.Count - 1; num >= 0; num--)
			{
				if (SpawnedBots_Attacking[num] != null)
				{
					Object.Destroy(SpawnedBots_Attacking[num]);
				}
			}
			SpawnedBots_Attacking.Clear();
		}

		private void ClearMobs()
		{
			for (int num = m_spawnedMobs.Count - 1; num >= 0; num--)
			{
				if (m_spawnedMobs[num] != null)
				{
					Object.Destroy(m_spawnedMobs[num]);
				}
			}
			m_spawnedMobs.Clear();
		}

		private void ClearEventMobs()
		{
			for (int num = m_eventSpawns.Count - 1; num >= 0; num--)
			{
				if (m_eventSpawns[num] != null)
				{
					Object.Destroy(m_eventSpawns[num]);
				}
			}
			m_eventSpawns.Clear();
		}

		private void PlayerDied()
		{
		}

		private int GetValidSupplyPointIndex()
		{
			List<int> list = new List<int>();
			for (int i = 0; i < SupplyPoints.Count; i++)
			{
				list.Add(i);
			}
			if (list.Contains(m_curSupplyPointIndex))
			{
				list.Remove(m_curSupplyPointIndex);
			}
			if (list.Contains(m_lastSupplyPointIndex))
			{
				list.Remove(m_lastSupplyPointIndex);
			}
			int num = list[Random.Range(0, list.Count)];
			if (Vector3.Distance(SupplyPoints[num].transform.position, GM.CurrentPlayerBody.transform.position) > 40f)
			{
				return num;
			}
			return GetValidSupplyPointIndex(num);
		}

		private int GetValidDefensePointIndex()
		{
			List<int> list = new List<int>();
			for (int i = 0; i < DefensePoints.Count; i++)
			{
				list.Add(i);
			}
			if (list.Contains(m_curDefensePointIndex))
			{
				list.Remove(m_curDefensePointIndex);
			}
			if (list.Contains(m_lastDefensePointIndex))
			{
				list.Remove(m_lastDefensePointIndex);
			}
			int num = list[Random.Range(0, list.Count)];
			if (Vector3.Distance(DefensePoints[num].transform.position, GM.CurrentPlayerBody.transform.position) > 40f)
			{
				return num;
			}
			return GetValidDefensePointIndex(num);
		}

		private int GetValidSupplyPointIndex(int exlucludeAlso)
		{
			List<int> list = new List<int>();
			for (int i = 0; i < SupplyPoints.Count; i++)
			{
				list.Add(i);
			}
			if (list.Contains(m_curSupplyPointIndex))
			{
				list.Remove(m_curSupplyPointIndex);
			}
			if (list.Contains(m_lastSupplyPointIndex))
			{
				list.Remove(m_lastSupplyPointIndex);
			}
			if (list.Contains(exlucludeAlso))
			{
				list.Remove(exlucludeAlso);
			}
			int num = list[Random.Range(0, list.Count)];
			if (Vector3.Distance(SupplyPoints[num].transform.position, GM.CurrentPlayerBody.transform.position) > 40f)
			{
				return num;
			}
			return GetValidSupplyPointIndex(num);
		}

		private int GetValidDefensePointIndex(int exlucludeAlso)
		{
			List<int> list = new List<int>();
			for (int i = 0; i < DefensePoints.Count; i++)
			{
				list.Add(i);
			}
			if (list.Contains(m_curDefensePointIndex))
			{
				list.Remove(m_curDefensePointIndex);
			}
			if (list.Contains(m_lastDefensePointIndex))
			{
				list.Remove(m_lastDefensePointIndex);
			}
			if (list.Contains(exlucludeAlso))
			{
				list.Remove(exlucludeAlso);
			}
			int num = list[Random.Range(0, list.Count)];
			if (Vector3.Distance(DefensePoints[num].transform.position, GM.CurrentPlayerBody.transform.position) > 40f)
			{
				return num;
			}
			return GetValidDefensePointIndex(num);
		}
	}
}
