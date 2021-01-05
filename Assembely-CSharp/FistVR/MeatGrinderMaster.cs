using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class MeatGrinderMaster : MonoBehaviour
	{
		[Serializable]
		public class EventAI
		{
			public enum EventAIMood
			{
				None,
				Nice,
				Nasty,
				Nightmarish
			}

			public enum EventType
			{
				eSpawn_Ammo = 0,
				eSpawn_Pistol1 = 1,
				eSpawn_Pistol2 = 2,
				eSpawn_Shotgun1 = 3,
				eSpawn_Shotgun2 = 4,
				eSpawn_SMG1 = 5,
				eSpawn_SMG2 = 6,
				eSpawn_Rifle1 = 7,
				eSpawn_Rifle2 = 8,
				eSpawn_Chainsaw = 9,
				eSpawn_AntiMaterialRifle = 10,
				eSpawn_Minigun = 11,
				eSpawn_Present = 12,
				eTrigger_Sound = 20,
				eTrigger_Flash = 21,
				eTrigger_SoundFlash = 22,
				eSpawnHazard_ExplodingWeiner = 30,
				eSpawnHazard_GiantSawBlade = 0x1F,
				eSpawnAgent_FlamingMeatball = 40,
				eSpawnAgent_KetchupBot = 41,
				eSpawnAgent_MustardBot = 42,
				eSpawnAgent_ShotgunBot = 43,
				eSpawnAgent_KneumaticBot = 44,
				eSpawnAgent_DrillWeiners = 45,
				eSpawnAgent_LetterM = 46,
				eSpawnAgent_ScreamingJerry = 47,
				eTrigger_Goober = 60
			}

			[Serializable]
			public class MGEvent
			{
				public EventType Type;

				public float Incidence;

				public Vector2 TickRange;

				public bool IsEventExclusive;

				[HideInInspector]
				public float FinalWeight;
			}

			private enum PointCheck
			{
				Front,
				Side,
				Rear
			}

			public MeatGrinderMaster Master;

			private HashSet<EventType> EventsTriggered = new HashSet<EventType>();

			private EventType m_lastEvent;

			public MG_EventAIConfig Config;

			private float m_EventTick = 90f;

			public Transform[] PossibleEventSpawnPoints;

			private int m_pointIndexToCheck;

			private Transform m_current_BackPoint;

			private Transform m_current_SidePoint;

			private Transform m_current_FrontPoint;

			private List<GameObject> RemoveIfDistant = new List<GameObject>();

			private List<FVRFireArmMagazine> SpawnedMags = new List<FVRFireArmMagazine>();

			private List<GameObject> SpawnedAgents = new List<GameObject>();

			[Header("Special Spawn Entries")]
			public GameObject Puff;

			public FVRObject Obj_Minigun;

			public FVRObject Obj_MinigunMag;

			public FVRObject Obj_Light50;

			public FVRObject Obj_Light50Mag;

			public FVRObject Obj_ChainSaw;

			public GameObject Present;

			public GameObject Hazard_Weiner;

			[Header("AudioEvents")]
			public AudioSource EventAudioSource;

			public AudioClip[] EventAudioClips;

			public void Init()
			{
				if (GM.Options.MeatGrinderFlags.HasNarratorDoneLongIntro)
				{
					m_EventTick = UnityEngine.Random.Range(30f, 40f);
				}
			}

			public void Tick()
			{
				PointChecking();
				m_EventTick -= Time.deltaTime;
				if (m_EventTick <= 0f)
				{
					EventTime();
				}
				RemoveCheck();
			}

			private void RemoveCheck()
			{
				for (int num = SpawnedMags.Count - 1; num >= 0; num--)
				{
					if (SpawnedMags[num] == null)
					{
						SpawnedMags.RemoveAt(num);
					}
					else if (!SpawnedMags[num].IsIntegrated && SpawnedMags[num].m_numRounds == 0 && !SpawnedMags[num].IsHeld && SpawnedMags[num].QuickbeltSlot == null && SpawnedMags[num].FireArm == null && SpawnedMags[num].RootRigidbody.IsSleeping())
					{
						UnityEngine.Object.Destroy(SpawnedMags[num].gameObject);
						SpawnedMags.RemoveAt(num);
					}
				}
				for (int num2 = SpawnedAgents.Count - 1; num2 >= 0; num2--)
				{
					if (SpawnedAgents[num2] == null)
					{
						SpawnedAgents.RemoveAt(num2);
					}
				}
				for (int num3 = RemoveIfDistant.Count - 1; num3 >= 0; num3--)
				{
					if (RemoveIfDistant[num3] != null)
					{
						if (Vector3.Distance(RemoveIfDistant[num3].transform.position, GM.CurrentPlayerBody.transform.position) > 20f)
						{
							UnityEngine.Object.Destroy(RemoveIfDistant[num3]);
						}
						RemoveIfDistant.RemoveAt(num3);
					}
					else
					{
						RemoveIfDistant.RemoveAt(num3);
					}
				}
			}

			private void PointChecking()
			{
				if (GM.CurrentPlayerBody == null)
				{
					return;
				}
				Transform transform = PossibleEventSpawnPoints[m_pointIndexToCheck];
				Vector3 vector = new Vector3(transform.position.x, 0f, transform.position.z);
				Vector3 vector2 = new Vector3(GM.CurrentPlayerBody.Head.position.x, 0f, GM.CurrentPlayerBody.Head.position.z);
				Vector3 forward = GM.CurrentPlayerBody.Head.forward;
				forward.y = 0f;
				forward.Normalize();
				Vector3 from = vector - vector2;
				float num = Vector3.Angle(from, forward);
				float num2 = Vector3.Distance(vector2, vector);
				if (num2 > 3f && num2 < 10f)
				{
					if (num < 45f)
					{
						m_current_FrontPoint = transform;
					}
					else if (num < 135f)
					{
						m_current_SidePoint = transform;
					}
					else
					{
						m_current_BackPoint = transform;
					}
				}
				m_pointIndexToCheck++;
				if (m_pointIndexToCheck >= PossibleEventSpawnPoints.Length)
				{
					m_pointIndexToCheck = 0;
				}
			}

			private bool isPointValid(Transform point, PointCheck check)
			{
				if (point == null)
				{
					return false;
				}
				Vector3 vector = new Vector3(point.position.x, 0f, point.position.z);
				Vector3 vector2 = new Vector3(GM.CurrentPlayerBody.Head.position.x, 0f, GM.CurrentPlayerBody.Head.position.z);
				Vector3 forward = GM.CurrentPlayerBody.Head.forward;
				forward.y = 0f;
				forward.Normalize();
				Vector3 from = vector - vector2;
				float num = Vector3.Angle(from, forward);
				float num2 = Vector3.Distance(vector2, vector);
				if (num2 < 3f || num2 > 10f)
				{
					return false;
				}
				switch (check)
				{
				case PointCheck.Front:
					if (num > 45f)
					{
						return false;
					}
					break;
				case PointCheck.Side:
					if (num < 45f || num > 135f)
					{
						return false;
					}
					break;
				case PointCheck.Rear:
					if (num < 135f)
					{
						return false;
					}
					break;
				}
				return true;
			}

			private bool isFacingGood(PointCheck check)
			{
				return check switch
				{
					PointCheck.Front => isPointValid(m_current_FrontPoint, PointCheck.Front), 
					PointCheck.Side => isPointValid(m_current_SidePoint, PointCheck.Side), 
					PointCheck.Rear => isPointValid(m_current_BackPoint, PointCheck.Rear), 
					_ => false, 
				};
			}

			private void EventTime()
			{
				MGEvent weightedRandomEntry = Config.GetWeightedRandomEntry();
				if ((weightedRandomEntry.IsEventExclusive && EventsTriggered.Contains(weightedRandomEntry.Type)) || weightedRandomEntry.Type == m_lastEvent || Master.Narrator.AUD.isPlaying)
				{
					return;
				}
				switch (weightedRandomEntry.Type)
				{
				case EventType.eSpawn_Ammo:
					if (!SpawnAmmo())
					{
						return;
					}
					break;
				case EventType.eSpawn_Chainsaw:
					if (!SpawnGun(Obj_ChainSaw))
					{
						return;
					}
					break;
				case EventType.eSpawn_AntiMaterialRifle:
					if (!SpawnGun(Obj_Light50, Obj_Light50Mag))
					{
						return;
					}
					break;
				case EventType.eSpawn_Minigun:
					if (!SpawnGun(Obj_Minigun, Obj_MinigunMag))
					{
						return;
					}
					break;
				case EventType.eSpawn_Present:
					if (!SpawnGun(Obj_ChainSaw))
					{
						return;
					}
					break;
				case EventType.eTrigger_Sound:
					if (!SoundFlashEventRandom(Sound: true, Flash: false))
					{
						return;
					}
					break;
				case EventType.eTrigger_Flash:
					if (!SoundFlashEventRandom(Sound: false, Flash: true))
					{
						return;
					}
					break;
				case EventType.eTrigger_SoundFlash:
					if (!SoundFlashEventRandom(Sound: true, Flash: true))
					{
						return;
					}
					break;
				case EventType.eSpawnHazard_ExplodingWeiner:
					if (!SpawnSplodingWeiners())
					{
						return;
					}
					break;
				case EventType.eSpawnAgent_FlamingMeatball:
					if (!SpawnFlamingMeatball())
					{
						return;
					}
					break;
				case EventType.eSpawnAgent_KneumaticBot:
					if (!SpawnBot(Master.HydraulicBotPrefabs[0], addToremovelist: true, NeedsNav: true))
					{
						return;
					}
					break;
				case EventType.eSpawnAgent_KetchupBot:
					if (!SpawnBot(Master.FlameShotgunBotPrefabs[1], addToremovelist: true, NeedsNav: true))
					{
						return;
					}
					break;
				case EventType.eSpawnAgent_MustardBot:
					if (!SpawnBot(Master.FlameShotgunBotPrefabs[2], addToremovelist: true, NeedsNav: true))
					{
						return;
					}
					break;
				case EventType.eSpawnAgent_ShotgunBot:
					if (!SpawnBot(Master.FlameShotgunBotPrefabs[0], addToremovelist: true, NeedsNav: true))
					{
						return;
					}
					break;
				case EventType.eSpawnAgent_DrillWeiners:
					if (!SpawnBot(Master.FlyingHotDogSwarmPrefab, addToremovelist: false, NeedsNav: false))
					{
						return;
					}
					break;
				case EventType.eSpawnHazard_GiantSawBlade:
				case EventType.eSpawnAgent_LetterM:
				case EventType.eTrigger_Goober:
					return;
				case EventType.eSpawnAgent_ScreamingJerry:
					if (SpawnBot(Master.ScreamingJerry, addToremovelist: false, NeedsNav: false))
					{
					}
					return;
				}
				m_EventTick = UnityEngine.Random.Range(weightedRandomEntry.TickRange.x, weightedRandomEntry.TickRange.y);
				EventsTriggered.Add(weightedRandomEntry.Type);
				m_lastEvent = weightedRandomEntry.Type;
			}

			private void SpawnPuff(Vector3 pos)
			{
				UnityEngine.Object.Instantiate(Puff, pos, UnityEngine.Random.rotation);
			}

			private bool SpawnBot(GameObject g, bool addToremovelist, bool NeedsNav)
			{
				if (SpawnedAgents.Count > 6)
				{
					return false;
				}
				if (!isFacingGood(PointCheck.Side))
				{
					return false;
				}
				GameObject gameObject = UnityEngine.Object.Instantiate(g, m_current_SidePoint.position, Quaternion.LookRotation(new Vector3(UnityEngine.Random.Range(1f, -1f), 0f, UnityEngine.Random.Range(1f, -1f)), Vector3.up));
				if (addToremovelist)
				{
					RemoveIfDistant.Add(gameObject);
				}
				SpawnedAgents.Add(gameObject);
				if (NeedsNav)
				{
					GM.CurrentSceneSettings.ShotEventReceivers.Add(gameObject);
					wwBotWurst component = gameObject.GetComponent<wwBotWurst>();
					if (component != null)
					{
						if (m_current_SidePoint.position.z > 0f)
						{
							if (m_current_SidePoint.position.x > 0f)
							{
								component.NavPointGroup = Master.FlameShotgunNavGroup_Boiler;
							}
							else
							{
								component.NavPointGroup = Master.FlameShotgunNavGroup_Office;
							}
						}
						else if (m_current_SidePoint.position.x > 0f)
						{
							component.NavPointGroup = Master.FlameShotgunNavGroup_Freezer;
						}
						else
						{
							component.NavPointGroup = Master.FlameShotgunNavGroup_Restaraunt;
						}
					}
				}
				return true;
			}

			private bool SpawnFlamingMeatball()
			{
				if (!isFacingGood(PointCheck.Rear))
				{
					return false;
				}
				UnityEngine.Object.Instantiate(Master.FlamingMeatball, m_current_BackPoint.position + Vector3.up, UnityEngine.Random.rotation);
				return true;
			}

			private bool SpawnSplodingWeiners()
			{
				if (!isFacingGood(PointCheck.Side))
				{
					return false;
				}
				UnityEngine.Object.Instantiate(Hazard_Weiner, m_current_SidePoint.position + Vector3.up * 1.5f, UnityEngine.Random.rotation);
				return true;
			}

			private bool SpawnAmmo()
			{
				if (SpawnedMags.Count > 6)
				{
					return false;
				}
				if (!isPointValid(m_current_FrontPoint, PointCheck.Front))
				{
					return false;
				}
				return true;
			}

			private bool SpawnGun(MGItemSpawnChartEntry entry)
			{
				if (!isPointValid(m_current_FrontPoint, PointCheck.Front))
				{
					return false;
				}
				Vector3 position = m_current_FrontPoint.position + Vector3.up * 0.7f;
				GameObject gameObject = UnityEngine.Object.Instantiate(entry.Obj.GetGameObject(), position, UnityEngine.Random.rotation);
				FVRFireArm component = gameObject.GetComponent<FVRFireArm>();
				GameObject gameObject2 = UnityEngine.Object.Instantiate(entry.Mag.GetGameObject(), component.GetMagMountPos(isBeltBox: false).position, component.GetMagMountPos(isBeltBox: false).rotation);
				FVRFireArmMagazine component2 = gameObject2.GetComponent<FVRFireArmMagazine>();
				component2.Load(component);
				RemoveIfDistant.Add(gameObject);
				return true;
			}

			private bool SpawnGun(FVRObject Gun, FVRObject Mag)
			{
				if (!isPointValid(m_current_FrontPoint, PointCheck.Front))
				{
					return false;
				}
				Vector3 position = m_current_FrontPoint.position + Vector3.up * 0.7f;
				GameObject gameObject = UnityEngine.Object.Instantiate(Gun.GetGameObject(), position, UnityEngine.Random.rotation);
				FVRFireArm component = gameObject.GetComponent<FVRFireArm>();
				GameObject gameObject2 = UnityEngine.Object.Instantiate(Mag.GetGameObject(), component.GetMagMountPos(isBeltBox: false).position, component.GetMagMountPos(isBeltBox: false).rotation);
				FVRFireArmMagazine component2 = gameObject2.GetComponent<FVRFireArmMagazine>();
				component2.Load(component);
				RemoveIfDistant.Add(gameObject);
				return true;
			}

			private bool SpawnGun(FVRObject Gun)
			{
				if (!isPointValid(m_current_FrontPoint, PointCheck.Front))
				{
					return false;
				}
				Vector3 position = m_current_FrontPoint.position + Vector3.up * 0.7f;
				GameObject gameObject = UnityEngine.Object.Instantiate(Gun.GetGameObject(), position, UnityEngine.Random.rotation);
				FVRFireArm component = gameObject.GetComponent<FVRFireArm>();
				RemoveIfDistant.Add(gameObject);
				return true;
			}

			private bool SpawnGun(GameObject Gun)
			{
				if (!isPointValid(m_current_FrontPoint, PointCheck.Front))
				{
					return false;
				}
				Vector3 position = m_current_FrontPoint.position + Vector3.up * 0.7f;
				GameObject gameObject = UnityEngine.Object.Instantiate(Gun, position, UnityEngine.Random.rotation);
				FVRFireArm component = gameObject.GetComponent<FVRFireArm>();
				RemoveIfDistant.Add(gameObject);
				return true;
			}

			private bool SoundFlashEventRandom(bool Sound, bool Flash)
			{
				float num = UnityEngine.Random.Range(0f, 1f);
				if (num > 0.9f)
				{
					return SoundFlashEvent(Sound, Flash, PointCheck.Front);
				}
				if (num > 0.5f)
				{
					return SoundFlashEvent(Sound, Flash, PointCheck.Side);
				}
				return SoundFlashEvent(Sound, Flash, PointCheck.Rear);
			}

			private bool SoundFlashEvent(bool Sound, bool Flash, PointCheck side)
			{
				if (!isFacingGood(side))
				{
					return false;
				}
				Vector3 vector = Vector3.zero;
				switch (side)
				{
				case PointCheck.Front:
					vector = m_current_FrontPoint.position;
					break;
				case PointCheck.Side:
					vector = m_current_SidePoint.position;
					break;
				case PointCheck.Rear:
					vector = m_current_BackPoint.position;
					break;
				}
				if (Flash)
				{
					FXM.InitiateMuzzleFlash(vector, UnityEngine.Random.onUnitSphere, UnityEngine.Random.Range(2f, 4f), Color.white, UnityEngine.Random.Range(2f, 5f));
				}
				if (Sound)
				{
					EventAudioSource.transform.position = vector;
					EventAudioSource.pitch = UnityEngine.Random.Range(0.7f, 1.1f);
					EventAudioSource.clip = EventAudioClips[UnityEngine.Random.Range(0, EventAudioClips.Length)];
					EventAudioSource.volume = UnityEngine.Random.Range(0.4f, 1f);
					EventAudioSource.Play();
				}
				return true;
			}
		}

		[Header("Narrator")]
		public MG_Narrator Narrator;

		public Transform PlayerRig;

		public GameObject StartingRoomPrefab;

		private MG_StartingRoom m_startingRoom;

		public FVRSceneSettings SceneSettings;

		public RedRoom.Quadrant PlayerQuadrant;

		[Header("Critical Item Prefabs")]
		public FVRObject FlashLightPrefab;

		public FVRObject FlipzoPrefab;

		public FVRObject TacticalFlashlightPrefab;

		public FVRObject GlowStickPrefab;

		public FVRObject BoxOfMatchesPrefab;

		public GameObject ItemSpawnerPrefab;

		public bool m_hasSpawnedSeconaryLight;

		[Header("Rooms")]
		public RedRoom[] SmallRooms;

		public RedRoom[] MediumRooms;

		public RedRoom[] LargeRooms;

		private int m_nextSmallRoomToConfigure;

		private int m_nextMediumRoomToConfigure;

		private int m_nextLargeRoomToConfigure;

		[Header("Trap Room Config")]
		public GameObject[] TrapRoomPrefabs;

		[Header("Meat Room Config")]
		public GameObject[] MeatRoomPrefabs;

		[Header("LottoRoom Config")]
		public GameObject[] LottoRoomPrefabs;

		[Header("Cabinet Config")]
		public GameObject[] CabinetPrefabs;

		public Transform[] PossibleCabinetSpawns;

		public float CabinetSpawnChance = 0.2f;

		[Header("Metal Shelves Config")]
		public GameObject[] MetalShelfPrefabs;

		public Transform[] PossibleShelfSpawns;

		public float ShelfSpawnChance = 0.45f;

		[Header("Industrial Shelves Config")]
		public GameObject IndustrialShelfPrefabShort;

		public GameObject IndustrialShelfPrefabLong;

		public float IndustrialShelfChance = 0.45f;

		public Transform[] PossibleIndustrialShelfShortSpawns;

		public Transform[] PossibleIndustrialShelfLongSpawns;

		[Header("SmashyStools")]
		public GameObject[] SmashyStoolPrefabs;

		public float SmashyStoolChance = 0.25f;

		public Transform[] PossibleSmashyStoolSpawns;

		[Header("MeatPiles")]
		public GameObject[] MeatPilePrefabs;

		public float MeatPileChance = 0.4f;

		public Transform[] PossibleMeatPileSpawns;

		[Header("CookedMeatPiles")]
		public GameObject[] CookedMeatPilePrefabs;

		public float CookedMeatPileChance = 0.3f;

		public Transform[] CookedPossibleMeatPileSpawns;

		[Header("LaserMines")]
		public GameObject LaserMinePrefab;

		private float LaserMineChance = 0.2f;

		public Transform[] LaserMinesSpawnPoints;

		[Header("Loot Tables")]
		public LootTable LT_Handguns = new LootTable();

		public LootTable LT_Shotguns = new LootTable();

		public LootTable LT_RareGuns = new LootTable();

		public LootTable LT_SuperRareGuns = new LootTable();

		public LootTable LT_Melee = new LootTable();

		public LootTable LT_Attachments = new LootTable();

		public LootTable LT_Junk = new LootTable();

		public LootTable LT_Powerups = new LootTable();

		public FVRObject LTEntry_Handgun1;

		public FVRObject LTEntry_Handgun2;

		public FVRObject LTEntry_Handgun3;

		public FVRObject LTEntry_Shotgun1;

		public FVRObject LTEntry_Shotgun2;

		public FVRObject LTEntry_Shotgun3;

		public FVRObject LTEntry_RareGun1;

		public FVRObject LTEntry_RareGun2;

		public FVRObject LTEntry_RareGun3;

		public FVRObject LTEntry_SuperRareGun1;

		[Header("Player stuff")]
		public FVRViveHand[] hands;

		private HashSet<FVRInteractiveObject> m_objectsThatHaveBeenHeld;

		private float m_TimeLeft = 1800f;

		public float[] TimeThresholds;

		public bool[] TimeThresholdPassed;

		private List<MG_Cabinet> ToSpawnCabinets;

		private List<MH_MetalShelf> ToSpawnShelves;

		[Header("Event AI Stuff")]
		public EventAI EventAIDude;

		public MG_EventAIConfig[] AIConfigs;

		private GameObject StartingWeiner1;

		private GameObject StartingWeiner2;

		[Header("Agent Stuff")]
		public RedRoom MeatRoom1;

		public RedRoom MeatRoom2;

		public RedRoom MeatRoom3;

		private GameObject Weiner1;

		private GameObject Weiner2;

		private GameObject Slicer1;

		private GameObject Slicer2;

		private GameObject Weiner3;

		private GameObject Weiner4;

		private GameObject Slicer3;

		private GameObject Slicer4;

		[Header("Bot Prefabs")]
		public GameObject[] HydraulicBotPrefabs;

		public GameObject SlicerPrefab;

		public GameObject[] FlameShotgunBotPrefabs;

		public GameObject FlyingHotDogSwarmPrefab;

		public GameObject FlamingMeatball;

		public GameObject LetterM;

		public GameObject ScreamingJerry;

		public Transform[] PossibleWeinerBotSpawnPositions;

		public Transform[] PossibleSlicerSpawnPositions;

		public Transform[] PossibleShotgunBotSpawnPositions_Boiler;

		public Transform[] PossibleShotgunBotSpawnPositions_Office;

		public Transform[] PossibleShotgunBotSpawnPositions_Freezer;

		public Transform[] PossibleShotgunBotSpawnPositions_Restaraunt;

		public wwBotWurstNavPointGroup FlameShotgunNavGroup_Boiler;

		public wwBotWurstNavPointGroup FlameShotgunNavGroup_Office;

		public wwBotWurstNavPointGroup FlameShotgunNavGroup_Freezer;

		public wwBotWurstNavPointGroup FlameShotgunNavGroup_Restaraunt;

		public List<wwBotWurst> SpawnedShotgunBots_Boiler = new List<wwBotWurst>();

		public List<wwBotWurst> SpawnedShotgunBots_Office = new List<wwBotWurst>();

		public List<wwBotWurst> SpawnedShotgunBots_Freezer = new List<wwBotWurst>();

		public List<wwBotWurst> SpawnedShotgunBots_Restaraunt = new List<wwBotWurst>();

		private float m_botCheckTick = 10f;

		private int badGuyThresholdAdd;

		public bool IsDead;

		private float m_PlayerHealth = 1f;

		private bool hasWarnedPlayerHealth1;

		public bool IsCountingDown = true;

		private void Awake()
		{
			GM.MGMaster = this;
			m_objectsThatHaveBeenHeld = new HashSet<FVRInteractiveObject>();
			GenerateLootTables();
			ConfigureRooms();
			ToSpawnCabinets = new List<MG_Cabinet>();
			ToSpawnShelves = new List<MH_MetalShelf>();
			SpawnItems();
			EventAIDude.Config = AIConfigs[(int)GM.Options.MeatGrinderFlags.AIMood];
			EventAIDude.Init();
			if (GM.Options.MeatGrinderFlags.MGMode == MeatGrinderFlags.MeatGrinderMode.KidsMeatyMeal)
			{
				GM.CurrentSceneSettings.DoesDamageGetRegistered = false;
			}
		}

		private void Start()
		{
			if (GM.Options.MeatGrinderFlags.MGMode == MeatGrinderFlags.MeatGrinderMode.KidsMeatyMeal)
			{
				GM.CurrentSceneSettings.DoesDamageGetRegistered = false;
			}
			if (GM.Options.MeatGrinderFlags.MGMode == MeatGrinderFlags.MeatGrinderMode.BuildYourOwnMeat || GM.Options.MeatGrinderFlags.MGMode == MeatGrinderFlags.MeatGrinderMode.KidsMeatyMeal)
			{
				GM.CurrentSceneSettings.IsSpawnLockingEnabled = true;
			}
			MeatRoom2.CloseDoors(playSound: false);
			MeatRoom3.CloseDoors(playSound: false);
		}

		private void GenerateLootTables()
		{
			List<FVRObject.OTagFirearmFiringMode> list = new List<FVRObject.OTagFirearmFiringMode>();
			list.Add(FVRObject.OTagFirearmFiringMode.Burst);
			list.Add(FVRObject.OTagFirearmFiringMode.FullAuto);
			List<FVRObject.OTagFirearmFiringMode> list2 = list;
			List<FVRObject.OTagFirearmRoundPower> list3 = new List<FVRObject.OTagFirearmRoundPower>();
			list3.Add(FVRObject.OTagFirearmRoundPower.Tiny);
			list3.Add(FVRObject.OTagFirearmRoundPower.Pistol);
			List<FVRObject.OTagFirearmRoundPower> list4 = list3;
			list3 = new List<FVRObject.OTagFirearmRoundPower>();
			list3.Add(FVRObject.OTagFirearmRoundPower.Shotgun);
			List<FVRObject.OTagFirearmRoundPower> list5 = list3;
			list3 = new List<FVRObject.OTagFirearmRoundPower>();
			list3.Add(FVRObject.OTagFirearmRoundPower.Pistol);
			list3.Add(FVRObject.OTagFirearmRoundPower.Intermediate);
			list3.Add(FVRObject.OTagFirearmRoundPower.FullPower);
			List<FVRObject.OTagFirearmRoundPower> list6 = list3;
			list3 = new List<FVRObject.OTagFirearmRoundPower>();
			list3.Add(FVRObject.OTagFirearmRoundPower.Intermediate);
			list3.Add(FVRObject.OTagFirearmRoundPower.FullPower);
			List<FVRObject.OTagFirearmRoundPower> list7 = list3;
			List<FVRObject.OTagEra> list8 = new List<FVRObject.OTagEra>();
			list8.Add(FVRObject.OTagEra.Modern);
			list8.Add(FVRObject.OTagEra.PostWar);
			list8.Add(FVRObject.OTagEra.WW2);
			list8.Add(FVRObject.OTagEra.WW1);
			list8.Add(FVRObject.OTagEra.TurnOfTheCentury);
			List<FVRObject.OTagEra> list9 = list8;
			LootTable lT_Handguns = LT_Handguns;
			LootTable.LootTableType type = LootTable.LootTableType.Firearm;
			list8 = list9;
			List<FVRObject.OTagFirearmSize> sizes = new List<FVRObject.OTagFirearmSize>
			{
				FVRObject.OTagFirearmSize.Pocket,
				FVRObject.OTagFirearmSize.Pistol,
				FVRObject.OTagFirearmSize.Compact
			};
			list3 = list4;
			list = list2;
			lT_Handguns.Initialize(type, list8, sizes, null, null, list, null, null, list3, null, null, null, null, null, -1, 30);
			LootTable lT_Shotguns = LT_Shotguns;
			type = LootTable.LootTableType.Firearm;
			list8 = list9;
			list3 = list5;
			lT_Shotguns.Initialize(type, list8, null, null, null, null, null, null, list3, null, null, null, null, null, -1, 8);
			LootTable lT_RareGuns = LT_RareGuns;
			type = LootTable.LootTableType.Firearm;
			list8 = list9;
			sizes = new List<FVRObject.OTagFirearmSize>
			{
				FVRObject.OTagFirearmSize.Compact,
				FVRObject.OTagFirearmSize.Carbine,
				FVRObject.OTagFirearmSize.FullSize
			};
			List<FVRObject.OTagFirearmAction> actions = new List<FVRObject.OTagFirearmAction>
			{
				FVRObject.OTagFirearmAction.Automatic
			};
			list3 = list6;
			lT_RareGuns.Initialize(type, list8, sizes, actions, null, null, null, null, list3, null, null, null, null, null, -1, 32);
			LootTable lT_SuperRareGuns = LT_SuperRareGuns;
			type = LootTable.LootTableType.Firearm;
			list8 = list9;
			sizes = new List<FVRObject.OTagFirearmSize>
			{
				FVRObject.OTagFirearmSize.Compact,
				FVRObject.OTagFirearmSize.Carbine,
				FVRObject.OTagFirearmSize.FullSize
			};
			actions = new List<FVRObject.OTagFirearmAction>
			{
				FVRObject.OTagFirearmAction.Automatic
			};
			list3 = list7;
			lT_SuperRareGuns.Initialize(type, list8, sizes, actions, null, null, null, null, list3, null, null, null, null, null, 20);
			LootTable lT_Melee = LT_Melee;
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
			lT_Melee.Initialize(type, list8, null, null, null, null, null, null, null, null, meleeStyles, meleeHandedness);
			LootTable lT_Attachments = LT_Attachments;
			type = LootTable.LootTableType.Attachments;
			list8 = list9;
			List<FVRObject.OTagAttachmentFeature> features = new List<FVRObject.OTagAttachmentFeature>
			{
				FVRObject.OTagAttachmentFeature.BarrelExtension,
				FVRObject.OTagAttachmentFeature.Grip,
				FVRObject.OTagAttachmentFeature.IronSight,
				FVRObject.OTagAttachmentFeature.Reflex,
				FVRObject.OTagAttachmentFeature.Laser
			};
			lT_Attachments.Initialize(type, list8, null, null, null, null, null, null, null, features);
			LootTable lT_Powerups = LT_Powerups;
			type = LootTable.LootTableType.Powerup;
			List<FVRObject.OTagPowerupType> powerupTypes = new List<FVRObject.OTagPowerupType>
			{
				FVRObject.OTagPowerupType.Health,
				FVRObject.OTagPowerupType.GhostMode,
				FVRObject.OTagPowerupType.InfiniteAmmo,
				FVRObject.OTagPowerupType.Invincibility,
				FVRObject.OTagPowerupType.QuadDamage
			};
			lT_Powerups.Initialize(type, null, null, null, null, null, null, null, null, null, null, null, powerupTypes);
			LootTable lT_Junk = LT_Junk;
			type = LootTable.LootTableType.Powerup;
			powerupTypes = new List<FVRObject.OTagPowerupType>
			{
				FVRObject.OTagPowerupType.Health,
				FVRObject.OTagPowerupType.GhostMode,
				FVRObject.OTagPowerupType.InfiniteAmmo,
				FVRObject.OTagPowerupType.Invincibility,
				FVRObject.OTagPowerupType.QuadDamage
			};
			lT_Junk.Initialize(type, null, null, null, null, null, null, null, null, null, null, null, powerupTypes);
			LTEntry_Handgun1 = LT_Handguns.GetRandomObject();
			LT_Handguns.Loot.Remove(LTEntry_Handgun1);
			LTEntry_Handgun2 = LT_Handguns.GetRandomObject();
			LT_Handguns.Loot.Remove(LTEntry_Handgun2);
			LTEntry_Handgun3 = LT_Handguns.GetRandomObject();
			LT_Handguns.Loot.Remove(LTEntry_Handgun3);
			LTEntry_Shotgun1 = LT_Shotguns.GetRandomObject();
			LT_Shotguns.Loot.Remove(LTEntry_Shotgun1);
			LTEntry_Shotgun2 = LT_Shotguns.GetRandomObject();
			LT_Shotguns.Loot.Remove(LTEntry_Shotgun2);
			LTEntry_Shotgun3 = LT_Shotguns.GetRandomObject();
			LT_Shotguns.Loot.Remove(LTEntry_Shotgun3);
			LTEntry_RareGun1 = LT_RareGuns.GetRandomObject();
			LT_RareGuns.Loot.Remove(LTEntry_RareGun1);
			LTEntry_RareGun2 = LT_RareGuns.GetRandomObject();
			LT_RareGuns.Loot.Remove(LTEntry_RareGun2);
			LTEntry_RareGun3 = LT_RareGuns.GetRandomObject();
			LT_RareGuns.Loot.Remove(LTEntry_RareGun3);
			LTEntry_SuperRareGun1 = LT_SuperRareGuns.GetRandomObject();
			LT_SuperRareGuns.Loot.Remove(LTEntry_SuperRareGun1);
		}

		public void SpawnObjectAtPlace(FVRObject obj, Transform t)
		{
			SpawnObjectAtPlace(obj, t.position, t.rotation);
		}

		public void SpawnAmmoAtPlaceForGun(FVRObject gun, Transform t)
		{
			SpawnAmmoAtPlaceForGun(gun, t.position, t.rotation);
		}

		public void SpawnObjectAtPlace(FVRObject obj, Vector3 pos, Quaternion rotation)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(obj.GetGameObject(), pos, rotation);
			FVRFireArmMagazine component = gameObject.GetComponent<FVRFireArmMagazine>();
			if (component != null && component.RoundType != FireArmRoundType.aFlameThrowerFuel)
			{
				component.ReloadMagWithTypeUpToPercentage(AM.GetRandomValidRoundClass(component.RoundType), Mathf.Clamp(UnityEngine.Random.Range(0.3f, 1f), 0.1f, 1f));
			}
		}

		public void SpawnAmmoAtPlaceForGun(FVRObject gun, Vector3 pos, Quaternion rotation)
		{
			if (gun.CompatibleMagazines.Count > 0)
			{
				SpawnObjectAtPlace(gun.CompatibleMagazines[UnityEngine.Random.Range(0, gun.CompatibleMagazines.Count - 1)], pos, rotation);
			}
			else if (gun.CompatibleClips.Count > 0)
			{
				SpawnObjectAtPlace(gun.CompatibleClips[UnityEngine.Random.Range(0, gun.CompatibleClips.Count - 1)], pos, rotation);
			}
			else if (gun.CompatibleSpeedLoaders.Count > 0)
			{
				SpawnObjectAtPlace(gun.CompatibleSpeedLoaders[UnityEngine.Random.Range(0, gun.CompatibleSpeedLoaders.Count - 1)], pos, rotation);
			}
			else if (gun.CompatibleSingleRounds.Count > 0)
			{
				int num = UnityEngine.Random.Range(2, 5);
				for (int i = 0; i < num; i++)
				{
					Vector3 pos2 = pos + Vector3.up * (0.05f * (float)i);
					SpawnObjectAtPlace(gun.CompatibleSingleRounds[UnityEngine.Random.Range(0, gun.CompatibleSingleRounds.Count - 1)], pos2, rotation);
				}
			}
		}

		public void SpawnGunAmmoPairToTransformList(FVRObject gun, Transform[] pointArray)
		{
			if (pointArray.Length > 0)
			{
				SpawnObjectAtPlace(gun, pointArray[0]);
				int num = pointArray.Length - 1;
				SpawnAmmoAtPlaceForGun(gun, pointArray[num]);
			}
		}

		private void Update()
		{
			CheckHandContents();
			CheckTime();
			CheckBotCounts();
			EventAIDude.Tick();
		}

		public void SpawnBadGuySetInitial()
		{
			badGuyThresholdAdd = 2;
		}

		public void SpawnBadGuySet1()
		{
			badGuyThresholdAdd = 4;
		}

		public void SpawnBadGuySet2()
		{
			badGuyThresholdAdd = 6;
		}

		private void CheckBotCounts()
		{
			if (m_botCheckTick > 0f)
			{
				m_botCheckTick -= Time.deltaTime;
				return;
			}
			m_botCheckTick = UnityEngine.Random.Range(15f, 45f);
			int num = UnityEngine.Random.Range(0, 3);
			GameObject gameObject = null;
			float num2 = UnityEngine.Random.Range(0f, 1f);
			gameObject = ((!(num2 > 0.5f)) ? HydraulicBotPrefabs[UnityEngine.Random.Range(0, HydraulicBotPrefabs.Length)] : FlameShotgunBotPrefabs[UnityEngine.Random.Range(0, FlameShotgunBotPrefabs.Length)]);
			switch (num)
			{
			case 0:
				CheckNSpawnBotWurst(SpawnedShotgunBots_Boiler, gameObject, FlameShotgunNavGroup_Boiler, PossibleShotgunBotSpawnPositions_Boiler);
				break;
			case 1:
				CheckNSpawnBotWurst(SpawnedShotgunBots_Office, gameObject, FlameShotgunNavGroup_Office, PossibleShotgunBotSpawnPositions_Office);
				break;
			case 2:
				CheckNSpawnBotWurst(SpawnedShotgunBots_Freezer, gameObject, FlameShotgunNavGroup_Freezer, PossibleShotgunBotSpawnPositions_Freezer);
				break;
			case 3:
				CheckNSpawnBotWurst(SpawnedShotgunBots_Restaraunt, gameObject, FlameShotgunNavGroup_Restaraunt, PossibleShotgunBotSpawnPositions_Restaraunt);
				break;
			}
		}

		private void CheckNSpawnBotWurst(List<wwBotWurst> list, GameObject prefab, wwBotWurstNavPointGroup Navgroup, Transform[] SpawnPoints)
		{
			for (int num = list.Count - 1; num >= 0; num--)
			{
				if (list[num] == null)
				{
					list.RemoveAt(num);
				}
			}
			int num2 = (int)GM.Options.MeatGrinderFlags.AIMood;
			if (GM.Options.MeatGrinderFlags.MGMode == MeatGrinderFlags.MeatGrinderMode.Classic)
			{
				num2 = -1;
			}
			num2 += badGuyThresholdAdd;
			if (num2 >= 0 && list.Count <= num2)
			{
				int num3 = UnityEngine.Random.Range(0, SpawnPoints.Length);
				if (Vector3.Distance(GM.CurrentPlayerBody.transform.position, SpawnPoints[num3].position) > 15f)
				{
					GameObject gameObject = UnityEngine.Object.Instantiate(prefab, SpawnPoints[num3].position, SpawnPoints[num3].rotation);
					wwBotWurst component = gameObject.GetComponent<wwBotWurst>();
					component.NavPointGroup = Navgroup;
					list.Add(component);
					GM.CurrentSceneSettings.ShotEventReceivers.Add(gameObject);
				}
			}
		}

		private void ConfigureRooms()
		{
			ShuffleRooms(SmallRooms);
			ShuffleRooms(SmallRooms);
			ShuffleRooms(MediumRooms);
			ShuffleRooms(MediumRooms);
			ShuffleRooms(LargeRooms);
			ShuffleRooms(LargeRooms);
			ShuffleRooms(LargeRooms);
			ShuffleRooms(LargeRooms);
			ConfigureStartingRoom(LargeRooms[m_nextLargeRoomToConfigure]);
			m_nextLargeRoomToConfigure++;
			float num = UnityEngine.Random.Range(0f, 1f);
			int num2 = 0;
			if (num > 0.5f)
			{
				num2++;
			}
			int num3 = 0;
			int num4 = 1;
			int num5 = 2;
			switch (UnityEngine.Random.Range(0, 4))
			{
			case 0:
				num3 = 0;
				num4 = 1;
				num5 = 2;
				break;
			case 1:
				num3 = 1;
				num4 = 0;
				num5 = 2;
				break;
			case 2:
				num3 = 2;
				num4 = 1;
				num5 = 0;
				break;
			default:
				num3 = 1;
				num4 = 2;
				num5 = 0;
				break;
			}
			GameObject gameObject = UnityEngine.Object.Instantiate(MeatRoomPrefabs[num3], SmallRooms[m_nextSmallRoomToConfigure].transform.position, Quaternion.identity);
			SmallRooms[m_nextSmallRoomToConfigure].SetRoomType(RedRoom.RedRoomType.Meat);
			SmallRooms[m_nextSmallRoomToConfigure].SetTriggerable(gameObject);
			gameObject.GetComponent<IMeatRoomAble>().SetMeatID(num2);
			num2++;
			GameObject gameObject2 = UnityEngine.Object.Instantiate(MeatRoomPrefabs[num4], MediumRooms[m_nextMediumRoomToConfigure].transform.position, Quaternion.identity);
			MediumRooms[m_nextMediumRoomToConfigure].SetRoomType(RedRoom.RedRoomType.Meat);
			MediumRooms[m_nextMediumRoomToConfigure].SetTriggerable(gameObject2);
			gameObject2.GetComponent<IMeatRoomAble>().SetMeatID(num2);
			num2++;
			GameObject gameObject3 = UnityEngine.Object.Instantiate(MeatRoomPrefabs[num5], LargeRooms[m_nextLargeRoomToConfigure].transform.position, Quaternion.identity);
			LargeRooms[m_nextLargeRoomToConfigure].SetRoomType(RedRoom.RedRoomType.Meat);
			LargeRooms[m_nextLargeRoomToConfigure].SetTriggerable(gameObject3);
			gameObject3.GetComponent<IMeatRoomAble>().SetMeatID(num2);
			float num6 = UnityEngine.Random.Range(0f, 1f);
			if ((double)num6 > 0.75)
			{
				MeatRoom1 = SmallRooms[m_nextSmallRoomToConfigure];
				MeatRoom2 = MediumRooms[m_nextMediumRoomToConfigure];
				MeatRoom3 = LargeRooms[m_nextLargeRoomToConfigure];
			}
			else if (num6 > 0.5f)
			{
				MeatRoom2 = SmallRooms[m_nextSmallRoomToConfigure];
				MeatRoom3 = MediumRooms[m_nextMediumRoomToConfigure];
				MeatRoom1 = LargeRooms[m_nextLargeRoomToConfigure];
			}
			else if (num6 > 0.25f)
			{
				MeatRoom3 = SmallRooms[m_nextSmallRoomToConfigure];
				MeatRoom1 = MediumRooms[m_nextMediumRoomToConfigure];
				MeatRoom2 = LargeRooms[m_nextLargeRoomToConfigure];
			}
			else
			{
				MeatRoom2 = SmallRooms[m_nextSmallRoomToConfigure];
				MeatRoom1 = MediumRooms[m_nextMediumRoomToConfigure];
				MeatRoom3 = LargeRooms[m_nextLargeRoomToConfigure];
			}
			MeatRoom2.CloseDoors(playSound: false);
			MeatRoom3.CloseDoors(playSound: false);
			m_nextSmallRoomToConfigure++;
			m_nextMediumRoomToConfigure++;
			m_nextLargeRoomToConfigure++;
			GameObject gameObject4 = UnityEngine.Object.Instantiate(TrapRoomPrefabs[num3], SmallRooms[m_nextSmallRoomToConfigure].transform.position, Quaternion.identity);
			SmallRooms[m_nextSmallRoomToConfigure].SetRoomType(RedRoom.RedRoomType.Trap);
			SmallRooms[m_nextSmallRoomToConfigure].SetTriggerable(gameObject4);
			IRoomTriggerable component = gameObject4.GetComponent<IRoomTriggerable>();
			component.SetRoom(SmallRooms[m_nextSmallRoomToConfigure]);
			GameObject gameObject5 = UnityEngine.Object.Instantiate(TrapRoomPrefabs[num4], MediumRooms[m_nextMediumRoomToConfigure].transform.position, Quaternion.identity);
			MediumRooms[m_nextMediumRoomToConfigure].SetRoomType(RedRoom.RedRoomType.Trap);
			MediumRooms[m_nextMediumRoomToConfigure].SetTriggerable(gameObject5);
			IRoomTriggerable component2 = gameObject5.GetComponent<IRoomTriggerable>();
			component2.SetRoom(MediumRooms[m_nextMediumRoomToConfigure]);
			GameObject gameObject6 = UnityEngine.Object.Instantiate(TrapRoomPrefabs[num5], LargeRooms[m_nextLargeRoomToConfigure].transform.position, Quaternion.identity);
			LargeRooms[m_nextLargeRoomToConfigure].SetRoomType(RedRoom.RedRoomType.Trap);
			LargeRooms[m_nextLargeRoomToConfigure].SetTriggerable(gameObject6);
			IRoomTriggerable component3 = gameObject6.GetComponent<IRoomTriggerable>();
			component3.SetRoom(LargeRooms[m_nextLargeRoomToConfigure]);
			m_nextSmallRoomToConfigure++;
			m_nextMediumRoomToConfigure++;
			m_nextLargeRoomToConfigure++;
			GameObject gameObject7 = UnityEngine.Object.Instantiate(LottoRoomPrefabs[UnityEngine.Random.Range(0, LottoRoomPrefabs.Length)], SmallRooms[m_nextSmallRoomToConfigure].transform.position, Quaternion.identity);
			SmallRooms[m_nextSmallRoomToConfigure].SetRoomType(RedRoom.RedRoomType.MonsterCloset);
			SmallRooms[m_nextSmallRoomToConfigure].SetTriggerable(gameObject7);
			IRoomTriggerable component4 = gameObject7.GetComponent<IRoomTriggerable>();
			component4.SetRoom(SmallRooms[m_nextSmallRoomToConfigure]);
			GameObject gameObject8 = UnityEngine.Object.Instantiate(LottoRoomPrefabs[UnityEngine.Random.Range(0, LottoRoomPrefabs.Length)], MediumRooms[m_nextMediumRoomToConfigure].transform.position, Quaternion.identity);
			MediumRooms[m_nextMediumRoomToConfigure].SetRoomType(RedRoom.RedRoomType.MonsterCloset);
			MediumRooms[m_nextMediumRoomToConfigure].SetTriggerable(gameObject8);
			IRoomTriggerable component5 = gameObject8.GetComponent<IRoomTriggerable>();
			component5.SetRoom(MediumRooms[m_nextMediumRoomToConfigure]);
			GameObject gameObject9 = UnityEngine.Object.Instantiate(LottoRoomPrefabs[UnityEngine.Random.Range(0, LottoRoomPrefabs.Length)], LargeRooms[m_nextLargeRoomToConfigure].transform.position, Quaternion.identity);
			LargeRooms[m_nextLargeRoomToConfigure].SetRoomType(RedRoom.RedRoomType.MonsterCloset);
			LargeRooms[m_nextLargeRoomToConfigure].SetTriggerable(gameObject9);
			IRoomTriggerable component6 = gameObject9.GetComponent<IRoomTriggerable>();
			component6.SetRoom(LargeRooms[m_nextLargeRoomToConfigure]);
			m_nextSmallRoomToConfigure++;
			m_nextMediumRoomToConfigure++;
			m_nextLargeRoomToConfigure++;
			GameObject gameObject10 = UnityEngine.Object.Instantiate(LottoRoomPrefabs[UnityEngine.Random.Range(0, LottoRoomPrefabs.Length)], MediumRooms[m_nextMediumRoomToConfigure].transform.position, Quaternion.identity);
			MediumRooms[m_nextMediumRoomToConfigure].SetRoomType(RedRoom.RedRoomType.MonsterCloset);
			MediumRooms[m_nextMediumRoomToConfigure].SetTriggerable(gameObject10);
			IRoomTriggerable component7 = gameObject10.GetComponent<IRoomTriggerable>();
			component7.SetRoom(MediumRooms[m_nextMediumRoomToConfigure]);
			GameObject triggerable = UnityEngine.Object.Instantiate(LottoRoomPrefabs[UnityEngine.Random.Range(0, LottoRoomPrefabs.Length)], SmallRooms[m_nextSmallRoomToConfigure].transform.position, Quaternion.identity);
			SmallRooms[m_nextSmallRoomToConfigure].SetRoomType(RedRoom.RedRoomType.MonsterCloset);
			SmallRooms[m_nextSmallRoomToConfigure].SetTriggerable(triggerable);
			IRoomTriggerable component8 = gameObject7.GetComponent<IRoomTriggerable>();
			component8.SetRoom(SmallRooms[m_nextSmallRoomToConfigure]);
			m_nextSmallRoomToConfigure++;
			m_nextMediumRoomToConfigure++;
		}

		private void ConfigureStartingRoom(RedRoom room)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(StartingRoomPrefab, room.transform.position, room.transform.rotation);
			m_startingRoom = gameObject.GetComponent<MG_StartingRoom>();
			room.SetRoomType(RedRoom.RedRoomType.Starting);
			room.SetTriggerable(gameObject);
			PlayerRig.transform.position = m_startingRoom.PlayerStartPos.position;
			SceneSettings.DeathResetPoint.position = m_startingRoom.PlayerStartPos.position;
			for (int i = 0; i < m_startingRoom.Spawn_CardboardBox.Length; i++)
			{
			}
			if (GM.Options.MeatGrinderFlags.MGMode == MeatGrinderFlags.MeatGrinderMode.BuildYourOwnMeat || GM.Options.MeatGrinderFlags.MGMode == MeatGrinderFlags.MeatGrinderMode.KidsMeatyMeal)
			{
				UnityEngine.Object.Instantiate(ItemSpawnerPrefab, m_startingRoom.Spawn_ItemSpawner.position, m_startingRoom.Spawn_ItemSpawner.rotation);
			}
			GameObject gameObject2 = UnityEngine.Object.Instantiate(LT_Melee.GetRandomObject().GetGameObject(), m_startingRoom.Spawn_Melee.position, m_startingRoom.Spawn_Melee.rotation);
			FVRPhysicalObject component = gameObject2.GetComponent<FVRPhysicalObject>();
			m_objectsThatHaveBeenHeld.Add(component);
			GameObject gameObject3 = UnityEngine.Object.Instantiate(LTEntry_Handgun1.GetGameObject(), m_startingRoom.Spawn_StartingPistol.position, m_startingRoom.Spawn_StartingPistol.rotation);
			GameObject gameObject4 = UnityEngine.Object.Instantiate(LTEntry_Shotgun1.GetGameObject(), m_startingRoom.Spawn_StartingShotgun.position, m_startingRoom.Spawn_StartingShotgun.rotation);
			SpawnAmmoAtPlaceForGun(LTEntry_Handgun1, m_startingRoom.Spawn_StartingPistolMag.position, m_startingRoom.Spawn_StartingPistolMag.rotation);
			SpawnAmmoAtPlaceForGun(LTEntry_Shotgun1, m_startingRoom.Spawn_StartingShotgunRounds.position, m_startingRoom.Spawn_StartingShotgunRounds.rotation);
			SpawnLight(m_startingRoom.Spawn_FlashLight.position, m_startingRoom.Spawn_FlashLight.rotation, isSecondary: false, GM.Options.MeatGrinderFlags.PrimaryLight);
			SpawnLight(m_startingRoom.Spawn_AmbientLight.position, m_startingRoom.Spawn_AmbientLight.rotation, isSecondary: true, GM.Options.MeatGrinderFlags.SecondaryLight);
		}

		public GameObject SpawnLight(Vector3 pos, Quaternion rot, bool isSecondary, MeatGrinderFlags.LightSourceOption sourceOption)
		{
			GameObject gameObject = null;
			MeatGrinderFlags.LightSourceOption lightSourceOption = MeatGrinderFlags.LightSourceOption.FlashLight;
			if (sourceOption == MeatGrinderFlags.LightSourceOption.Random)
			{
				float num = UnityEngine.Random.Range(0f, 1f);
				if (isSecondary)
				{
					num -= 0.15f;
				}
				if ((double)num > 0.75)
				{
					lightSourceOption = MeatGrinderFlags.LightSourceOption.FlashLight;
				}
				else if ((double)num > 0.5)
				{
					lightSourceOption = MeatGrinderFlags.LightSourceOption.GlowStick;
				}
				else if ((double)num > 0.25)
				{
					lightSourceOption = MeatGrinderFlags.LightSourceOption.Lighter;
				}
				else if ((double)num >= 0.0)
				{
					lightSourceOption = MeatGrinderFlags.LightSourceOption.BoxOfMatches;
				}
			}
			else
			{
				lightSourceOption = sourceOption;
			}
			switch (lightSourceOption)
			{
			case MeatGrinderFlags.LightSourceOption.FlashLight:
				if (!isSecondary)
				{
					gameObject = UnityEngine.Object.Instantiate(FlashLightPrefab.GetGameObject(), pos, rot);
					Flashlight component = gameObject.GetComponent<Flashlight>();
					component.ToggleOn();
				}
				else
				{
					gameObject = UnityEngine.Object.Instantiate(TacticalFlashlightPrefab.GetGameObject(), pos, rot);
				}
				break;
			case MeatGrinderFlags.LightSourceOption.GlowStick:
				gameObject = UnityEngine.Object.Instantiate(GlowStickPrefab.GetGameObject(), pos, rot);
				break;
			case MeatGrinderFlags.LightSourceOption.Lighter:
				gameObject = UnityEngine.Object.Instantiate(FlipzoPrefab.GetGameObject(), pos, rot);
				break;
			case MeatGrinderFlags.LightSourceOption.BoxOfMatches:
				gameObject = UnityEngine.Object.Instantiate(BoxOfMatchesPrefab.GetGameObject(), pos, rot);
				break;
			}
			if (gameObject != null && isSecondary)
			{
				m_hasSpawnedSeconaryLight = true;
			}
			return gameObject;
		}

		private void SpawnItems()
		{
			for (int i = 0; i < PossibleCabinetSpawns.Length; i++)
			{
				float num = UnityEngine.Random.Range(0f, 1f);
				if (num <= CabinetSpawnChance)
				{
					GameObject gameObject = UnityEngine.Object.Instantiate(CabinetPrefabs[UnityEngine.Random.Range(0, CabinetPrefabs.Length)], PossibleCabinetSpawns[i].position, PossibleCabinetSpawns[i].rotation);
					MG_Cabinet component = gameObject.GetComponent<MG_Cabinet>();
					component.Init();
					ToSpawnCabinets.Add(component);
				}
			}
			for (int j = 0; j < PossibleShelfSpawns.Length; j++)
			{
				float num2 = UnityEngine.Random.Range(0f, 1f);
				if (num2 <= ShelfSpawnChance)
				{
					GameObject gameObject2 = UnityEngine.Object.Instantiate(MetalShelfPrefabs[UnityEngine.Random.Range(0, MetalShelfPrefabs.Length)], PossibleShelfSpawns[j].position, PossibleShelfSpawns[j].rotation);
					MH_MetalShelf component2 = gameObject2.GetComponent<MH_MetalShelf>();
					ToSpawnShelves.Add(component2);
				}
			}
			for (int k = 0; k < PossibleIndustrialShelfShortSpawns.Length; k++)
			{
				float num3 = UnityEngine.Random.Range(0f, 1f);
				if (num3 <= IndustrialShelfChance)
				{
					GameObject gameObject3 = UnityEngine.Object.Instantiate(IndustrialShelfPrefabShort, PossibleIndustrialShelfShortSpawns[k].position, PossibleIndustrialShelfShortSpawns[k].rotation);
					MG_IndustrialShelf component3 = gameObject3.GetComponent<MG_IndustrialShelf>();
					component3.Init();
				}
			}
			for (int l = 0; l < PossibleIndustrialShelfLongSpawns.Length; l++)
			{
				float num4 = UnityEngine.Random.Range(0f, 1f);
				if (num4 <= IndustrialShelfChance)
				{
					GameObject gameObject4 = UnityEngine.Object.Instantiate(IndustrialShelfPrefabLong, PossibleIndustrialShelfLongSpawns[l].position, PossibleIndustrialShelfLongSpawns[l].rotation);
					MG_IndustrialShelf component4 = gameObject4.GetComponent<MG_IndustrialShelf>();
					component4.Init();
				}
			}
			for (int m = 0; m < PossibleSmashyStoolSpawns.Length; m++)
			{
				float num5 = UnityEngine.Random.Range(0f, 1f);
				if (num5 <= SmashyStoolChance)
				{
					GameObject gameObject5 = UnityEngine.Object.Instantiate(SmashyStoolPrefabs[UnityEngine.Random.Range(0, SmashyStoolPrefabs.Length)], PossibleSmashyStoolSpawns[m].position, PossibleSmashyStoolSpawns[m].rotation);
				}
			}
			for (int n = 0; n < PossibleMeatPileSpawns.Length; n++)
			{
				float num6 = UnityEngine.Random.Range(0f, 1f);
				if (num6 <= MeatPileChance)
				{
					GameObject gameObject6 = UnityEngine.Object.Instantiate(MeatPilePrefabs[UnityEngine.Random.Range(0, MeatPilePrefabs.Length)], PossibleMeatPileSpawns[n].position, PossibleMeatPileSpawns[n].rotation);
					MG_DestroyableWithSpawn component5 = gameObject6.GetComponent<MG_DestroyableWithSpawn>();
					component5.SetMGMaster(this);
				}
			}
			for (int num7 = 0; num7 < CookedPossibleMeatPileSpawns.Length; num7++)
			{
				float num8 = UnityEngine.Random.Range(0f, 1f);
				if (num8 <= CookedMeatPileChance)
				{
					GameObject gameObject7 = UnityEngine.Object.Instantiate(CookedMeatPilePrefabs[UnityEngine.Random.Range(0, CookedMeatPilePrefabs.Length)], CookedPossibleMeatPileSpawns[num7].position, CookedPossibleMeatPileSpawns[num7].rotation);
					MG_DestroyableWithSpawn component6 = gameObject7.GetComponent<MG_DestroyableWithSpawn>();
					component6.SetMGMaster(this);
				}
			}
			for (int num9 = 0; num9 < LaserMinesSpawnPoints.Length; num9++)
			{
				float num10 = UnityEngine.Random.Range(0f, 1f);
				if (num10 <= LaserMineChance)
				{
					UnityEngine.Object.Instantiate(LaserMinePrefab, LaserMinesSpawnPoints[num9].position, LaserMinesSpawnPoints[num9].rotation);
				}
			}
			for (int num11 = 0; num11 < ToSpawnCabinets.Count; num11++)
			{
				float num12 = UnityEngine.Random.Range(0f, 1f);
				if (num12 > 0.4f)
				{
					float num13 = UnityEngine.Random.Range(0f, 1f);
					if (num13 > 0.97f)
					{
						SpawnAmmoAtPlaceForGun(LTEntry_SuperRareGun1, ToSpawnCabinets[num11].GetRandomSpawnTransform());
					}
					else if (num13 > 0.9f)
					{
						SpawnAmmoAtPlaceForGun(LTEntry_RareGun1, ToSpawnCabinets[num11].GetRandomSpawnTransform());
					}
					else if (num13 > 0.8f)
					{
						SpawnAmmoAtPlaceForGun(LTEntry_RareGun2, ToSpawnCabinets[num11].GetRandomSpawnTransform());
					}
					else if (num13 > 0.6f)
					{
						SpawnAmmoAtPlaceForGun(LTEntry_Shotgun2, ToSpawnCabinets[num11].GetRandomSpawnTransform());
					}
					else if (num13 > 0.45f)
					{
						SpawnAmmoAtPlaceForGun(LTEntry_Shotgun1, ToSpawnCabinets[num11].GetRandomSpawnTransform());
					}
					else if (num13 > 0.3f)
					{
						SpawnAmmoAtPlaceForGun(LTEntry_Handgun3, ToSpawnCabinets[num11].GetRandomSpawnTransform());
					}
					else if (num13 > 0.2f)
					{
						SpawnAmmoAtPlaceForGun(LTEntry_Handgun2, ToSpawnCabinets[num11].GetRandomSpawnTransform());
					}
					else if (num13 > 0.1f)
					{
						SpawnAmmoAtPlaceForGun(LTEntry_Handgun1, ToSpawnCabinets[num11].GetRandomSpawnTransform());
					}
					else
					{
						SpawnObjectAtPlace(LT_Attachments.GetRandomObject(), ToSpawnCabinets[num11].GetRandomSpawnTransform());
					}
				}
			}
			for (int num14 = 0; num14 < ToSpawnShelves.Count; num14++)
			{
				float num15 = UnityEngine.Random.Range(0f, 1f);
				if (num15 > 0.1f)
				{
					float num16 = UnityEngine.Random.Range(0f, 1f);
					if (num16 > 0.95f)
					{
						SpawnGunAmmoPairToTransformList(LTEntry_RareGun1, ToSpawnShelves[num14].SpawnPositions);
					}
					else if (num16 > 0.9f)
					{
						SpawnGunAmmoPairToTransformList(LTEntry_RareGun2, ToSpawnShelves[num14].SpawnPositions);
					}
					else if (num16 > 0.85f)
					{
						SpawnGunAmmoPairToTransformList(LTEntry_Shotgun2, ToSpawnShelves[num14].SpawnPositions);
					}
					else if (num16 > 0.8f)
					{
						SpawnGunAmmoPairToTransformList(LTEntry_Handgun2, ToSpawnShelves[num14].SpawnPositions);
					}
					else if (num16 > 0.75f)
					{
						SpawnAmmoAtPlaceForGun(LTEntry_Handgun1, ToSpawnShelves[num14].SpawnPositions[0]);
					}
					else if (num16 > 0.7f)
					{
						SpawnAmmoAtPlaceForGun(LTEntry_Handgun2, ToSpawnShelves[num14].SpawnPositions[0]);
					}
					else if (num16 > 0.65f)
					{
						SpawnAmmoAtPlaceForGun(LTEntry_Shotgun1, ToSpawnShelves[num14].SpawnPositions[0]);
					}
					else if (num16 > 0.6f)
					{
						SpawnAmmoAtPlaceForGun(LTEntry_Shotgun2, ToSpawnShelves[num14].SpawnPositions[0]);
					}
					else if (num16 > 0.55f)
					{
						SpawnAmmoAtPlaceForGun(LTEntry_RareGun1, ToSpawnShelves[num14].SpawnPositions[0]);
					}
					else if (num16 > 0.5f)
					{
						SpawnAmmoAtPlaceForGun(LTEntry_RareGun2, ToSpawnShelves[num14].SpawnPositions[0]);
					}
					else if (num16 > 0.3f)
					{
						SpawnObjectAtPlace(LT_Melee.GetRandomObject(), ToSpawnShelves[num14].SpawnPositions[0]);
					}
					else if (num16 > 0.1f)
					{
						SpawnObjectAtPlace(LT_Attachments.GetRandomObject(), ToSpawnShelves[num14].SpawnPositions[0]);
					}
					else
					{
						SpawnObjectAtPlace(LT_Junk.GetRandomObject(), ToSpawnShelves[num14].SpawnPositions[0]);
					}
				}
			}
		}

		private void CheckHandContents()
		{
			for (int i = 0; i < hands.Length; i++)
			{
				if (hands[i].CurrentInteractable is FVRPhysicalObject && !m_objectsThatHaveBeenHeld.Contains(hands[i].CurrentInteractable))
				{
					m_objectsThatHaveBeenHeld.Add(hands[i].CurrentInteractable);
					if (hands[i].CurrentInteractable is FVRFireArm)
					{
						GM.MGMaster.Narrator.PlayFoundRareItem();
					}
					else if (hands[i].CurrentInteractable is FVRFireArm)
					{
						GM.MGMaster.Narrator.PlayFoundNormalItem();
					}
					else if (hands[i].CurrentInteractable is FVRMeleeWeapon)
					{
						GM.MGMaster.Narrator.PlayFoundNormalItem();
					}
					else if (hands[i].CurrentInteractable is FVRFireArmAttachment)
					{
						GM.MGMaster.Narrator.PlayFoundSpecialItem();
					}
					else if (hands[i].CurrentInteractable is AnimalNoiseMaker || hands[i].CurrentInteractable is SodaCan)
					{
						GM.MGMaster.Narrator.PlayFoundJunkItem();
					}
				}
			}
		}

		private void ShuffleRooms(RedRoom[] rooms)
		{
			for (int i = 0; i < rooms.Length; i++)
			{
				RedRoom redRoom = rooms[i];
				int num = UnityEngine.Random.Range(i, rooms.Length);
				rooms[i] = rooms[num];
				rooms[num] = redRoom;
			}
		}

		private void CheckTime()
		{
			if (IsCountingDown && GM.Options.MeatGrinderFlags.MGMode == MeatGrinderFlags.MeatGrinderMode.Classic)
			{
				m_TimeLeft -= Time.deltaTime * 0.95f;
			}
			for (int i = 0; i < TimeThresholds.Length; i++)
			{
				if (m_TimeLeft <= TimeThresholds[i] && !TimeThresholdPassed[i])
				{
					TimeThresholdPassed[i] = true;
					GM.MGMaster.Narrator.PlayTimeWarning(i);
				}
			}
			m_PlayerHealth = GM.GetPlayerHealth();
			if (m_PlayerHealth < 0.25f && !hasWarnedPlayerHealth1)
			{
				hasWarnedPlayerHealth1 = true;
				GM.MGMaster.Narrator.PlayAboutToDie();
			}
			if (m_TimeLeft < -3f && !IsDead)
			{
				IsDead = true;
				GM.CurrentPlayerBody.Hitboxes[0].Damage(12f);
				GM.MGMaster.Narrator.PlayDiedOutOfHealth();
			}
			if (m_PlayerHealth <= 0f && !IsDead)
			{
				GM.MGMaster.Narrator.PlayDiedOutOfHealth();
				IsDead = true;
			}
		}
	}
}
