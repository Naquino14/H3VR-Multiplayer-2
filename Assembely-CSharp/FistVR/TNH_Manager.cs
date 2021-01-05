using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
	public class TNH_Manager : MonoBehaviour
	{
		[Serializable]
		public class SosigPatrolSquad
		{
			public List<Sosig> Squad = new List<Sosig>();

			public List<Vector3> PatrolPoints = new List<Vector3>();

			public int CurPatrolPointIndex;

			public bool IsPatrollingUp = true;
		}

		public delegate void TokenCountChange(int numTokens);

		public TNH_CharacterDatabase CharDB;

		[Header("Game State")]
		public TNH_Phase Phase = TNH_Phase.StartUp;

		public string LevelName = "Classic";

		private int m_level = -1;

		private int m_numTokens = 5;

		private int m_maxLevels;

		private float m_timePlaying;

		private TNH_PointSequence m_curPointSequence;

		private TNH_Progression m_curProgression;

		private TNH_Progression m_curProgressionEndless;

		private TNH_Progression.Level m_curLevel;

		private int m_curHoldIndex;

		private TNH_HoldPoint m_curHoldPoint;

		private List<TNH_SupplyPoint> m_supplyPoints = new List<TNH_SupplyPoint>();

		private List<GameObject> m_weaponCases = new List<GameObject>();

		[Header("Data Settings")]
		public List<TNH_PointSequence> PossibleSequnces;

		public TNH_SafePositionMatrix SafePosMatrix;

		public TNH_CharacterDef C;

		public TNHSetting_ProgressionType ProgressionMode;

		public TNHSetting_EquipmentMode EquipmentMode;

		public TNHSetting_HealthMode HealthMode;

		public TNH_HealthMult HealthMult;

		public TNHModifier_AIDifficulty AI_Difficulty;

		public TNHModifier_RadarMode RadarMode;

		public TNH_ItemSpawnerMode ItemSpawnerMode;

		public TNH_BackpackMode BackpackMode;

		public TNH_BGAudioMode BGAudioMode;

		public TNH_AIVoiceMode AIVoiceMode;

		public TNH_RadarHand RadarHand;

		[Header("System Connections")]
		public AIManager AIManager;

		public TAH_Reticle TAHReticle;

		public GameObject ItemSpawner;

		public LayerMask ReticleMask_Take;

		public LayerMask ReticleMask_Hold;

		public FVRFMODController FMODController;

		public List<TNH_HoldPoint> HoldPoints;

		public List<TNH_SupplyPoint> SupplyPoints;

		[Header("Prefab Connections")]
		public GameObject Prefab_SystemNode;

		public GameObject Prefab_TargetWarpingIn;

		public GameObject Prefab_WeaponCaseSmall;

		public GameObject Prefab_WeaponCaseLarge;

		public GameObject Prefab_MetalTable;

		public List<GameObject> Prefabs_ShatterableCrates;

		public bool DebugMode;

		public FVRObject Prefab_HealthPickupMinor;

		[Header("Panel Prefabs")]
		public GameObject Prefab_ObjectConstructor;

		public GameObject Prefab_AmmoReloader;

		public GameObject Prefab_MagDuplicator;

		public GameObject Prefab_GunRecycler;

		private List<SosigPatrolSquad> m_patrolSquads = new List<SosigPatrolSquad>();

		private List<GameObject> m_miscEnemies = new List<GameObject>();

		[Header("ScoringSystem")]
		public int[] Nums = new int[12];

		public int[] Stats = new int[12];

		public List<Text> ScoringLabels;

		public TNH_ScoreDisplay ScoreDisplay;

		private bool m_hasInit;

		private Dictionary<ObjectTableDef, ObjectTable> m_objectTableDics = new Dictionary<ObjectTableDef, ObjectTable>();

		private float m_takeMusicIntensity = 1f;

		private float m_takeMusicIntensityTarget = 1f;

		private float m_fireThreshold;

		private float m_botKillThreshold;

		[Header("VoiceStuff")]
		public TNH_VoiceDatabase VoiceDB;

		private Dictionary<TNH_VoiceLineID, List<AudioClip>> voiceDic_Standard = new Dictionary<TNH_VoiceLineID, List<AudioClip>>();

		private Dictionary<TNH_VoiceLineID, List<AudioClip>> voiceDic_Corrupted = new Dictionary<TNH_VoiceLineID, List<AudioClip>>();

		public AudioSource Narrator_Normal;

		public AudioSource Narrator_Corrupted;

		public Queue<TNH_VoiceLineID> QueuedLines = new Queue<TNH_VoiceLineID>();

		private float timeTilLineClear = 1f;

		private float m_timeTilPatrolCanSpawn;

		private HashSet<FVRPhysicalObject> m_knownObjsHash = new HashSet<FVRPhysicalObject>();

		private List<FVRPhysicalObject> m_knownObjs = new List<FVRPhysicalObject>();

		private int knownObjectCheckIndex;

		[Header("Lib Stuff")]
		public TNH_ResourceLib ResourceLib;

		private Dictionary<TNH_EncryptionType, FVRObject> dic_encryption = new Dictionary<TNH_EncryptionType, FVRObject>();

		private Dictionary<TNH_TurretType, FVRObject> dic_turret = new Dictionary<TNH_TurretType, FVRObject>();

		private Dictionary<TNH_TrapType, FVRObject> dic_trap = new Dictionary<TNH_TrapType, FVRObject>();

		public event TokenCountChange TokenCountChangeEvent;

		public void AddToMiscEnemies(GameObject g)
		{
			m_miscEnemies.Add(g);
		}

		public void ClearMiscEnemies()
		{
			for (int num = m_miscEnemies.Count - 1; num >= 0; num--)
			{
				if (m_miscEnemies[num] != null)
				{
					UnityEngine.Object.Destroy(m_miscEnemies[num]);
				}
			}
			m_miscEnemies.Clear();
		}

		private void Start()
		{
			GM.CurrentSceneSettings.SosigKillEvent += OnSosigKill;
			GM.CurrentSceneSettings.ShotFiredEvent += OnShotFired;
			GM.CurrentSceneSettings.BotShotFiredEvent += OnBotShotFired;
			GM.CurrentSceneSettings.ObjectPickedUpEvent += AddFVRObjectToTrackedList;
			PrimeNums();
			GM.TNH_Manager = this;
			GM.CurrentAIManager.NumEntitiesToCheckPerFrame = 3;
		}

		private void OnDisable()
		{
			GM.CurrentSceneSettings.SosigKillEvent -= OnSosigKill;
			GM.CurrentSceneSettings.ShotFiredEvent -= OnShotFired;
			GM.CurrentSceneSettings.BotShotFiredEvent -= OnBotShotFired;
			GM.CurrentSceneSettings.ObjectPickedUpEvent -= AddFVRObjectToTrackedList;
			GM.TNH_Manager = null;
		}

		public void PlayerDied()
		{
			SetPhase(TNH_Phase.Dead);
		}

		private void DelayedInit()
		{
			if (!m_hasInit && AIManager.HasInit)
			{
				m_hasInit = true;
				C = CharDB.GetDef(GM.TNHOptions.Char);
				m_curProgression = C.Progressions[UnityEngine.Random.Range(0, C.Progressions.Count)];
				m_curProgressionEndless = C.Progressions_Endless[UnityEngine.Random.Range(0, C.Progressions_Endless.Count)];
				m_curPointSequence = PossibleSequnces[UnityEngine.Random.Range(0, PossibleSequnces.Count)];
				m_level = 0;
				SetLevel(m_level);
				InitLibraries();
				LoadFromSettings();
				InitPlayer();
				InitTables();
				InitSound();
				InitHoldPoints();
				InitPlayerPosition();
				InitBeginningEquipment();
				if (!DebugMode)
				{
					SetPhase(TNH_Phase.Take);
				}
			}
		}

		private void LoadFromSettings()
		{
			ProgressionMode = GM.TNHOptions.ProgressionTypeSetting;
			EquipmentMode = GM.TNHOptions.EquipmentModeSetting;
			if (EquipmentMode == TNHSetting_EquipmentMode.LimitedAmmo)
			{
				GM.CurrentSceneSettings.IsSpawnLockingEnabled = false;
			}
			HealthMode = GM.TNHOptions.HealthModeSetting;
			HealthMult = GM.TNHOptions.HealthMult;
			switch (HealthMode)
			{
			case TNHSetting_HealthMode.StandardHealth:
				GM.CurrentPlayerBody.SetHealthThreshold(5000f);
				break;
			case TNHSetting_HealthMode.HardcoreOneHit:
				GM.CurrentPlayerBody.SetHealthThreshold(100f);
				break;
			case TNHSetting_HealthMode.CustomHealth:
				switch (HealthMult)
				{
				case TNH_HealthMult.Human:
					GM.CurrentPlayerBody.SetHealthThreshold(2000f);
					break;
				case TNH_HealthMult.Armored:
					GM.CurrentPlayerBody.SetHealthThreshold(4000f);
					break;
				case TNH_HealthMult.Meaty:
					GM.CurrentPlayerBody.SetHealthThreshold(10000f);
					break;
				case TNH_HealthMult.ExtraBeefy:
					GM.CurrentPlayerBody.SetHealthThreshold(50000f);
					break;
				case TNH_HealthMult.NighInvulnerable:
					GM.CurrentPlayerBody.SetHealthThreshold(500000f);
					break;
				}
				break;
			}
			AI_Difficulty = GM.TNHOptions.AIDifficultyModifier;
			RadarMode = GM.TNHOptions.RadarModeModifier;
			if (RadarMode == TNHModifier_RadarMode.Off)
			{
				TAHReticle.RegistersEnemies = false;
			}
			ItemSpawnerMode = GM.TNHOptions.ItemSpawnerMode;
			BackpackMode = GM.TNHOptions.BackpackMode;
			BGAudioMode = GM.TNHOptions.BGAudioMode;
			AIVoiceMode = GM.TNHOptions.AIVoiceMode;
			RadarHand = GM.TNHOptions.RadarHand;
			m_numTokens = C.StartingTokens;
			OnTokenCountChange(m_numTokens);
			if (ProgressionMode == TNHSetting_ProgressionType.Marathon)
			{
				m_maxLevels = 9999999;
			}
			else
			{
				m_maxLevels = m_curProgression.Levels.Count;
			}
		}

		private void InitPlayer()
		{
		}

		public ObjectTable GetObjectTable(ObjectTableDef d)
		{
			return m_objectTableDics[d];
		}

		private void InitTables()
		{
			for (int i = 0; i < C.EquipmentPool.Entries.Count; i++)
			{
				ObjectTable objectTable = new ObjectTable();
				objectTable.Initialize(C.EquipmentPool.Entries[i].TableDef);
				if (m_objectTableDics.ContainsKey(C.EquipmentPool.Entries[i].TableDef))
				{
					Debug.Log(C.EquipmentPool.Entries[i].TableDef.name);
				}
				else
				{
					m_objectTableDics.Add(C.EquipmentPool.Entries[i].TableDef, objectTable);
				}
			}
			ObjectTable objectTable2 = new ObjectTable();
			objectTable2.Initialize(C.RequireSightTable);
			m_objectTableDics.Add(C.RequireSightTable, objectTable2);
		}

		private void InitSound()
		{
			FMODController.SetMasterVolume(0.25f);
			EnqueueLine(TNH_VoiceLineID.AI_UplinkSuccessfulTargetSystemDetectedTakeIt);
		}

		private void InitHoldPoints()
		{
			for (int i = 0; i < HoldPoints.Count; i++)
			{
				HoldPoints[i].Init();
			}
		}

		private void InitPlayerPosition()
		{
			Transform spawnPoint_PlayerSpawn = SupplyPoints[m_curPointSequence.StartSupplyPointIndex].SpawnPoint_PlayerSpawn;
			GM.CurrentMovementManager.TeleportToPoint(spawnPoint_PlayerSpawn.position, isAbsolute: true, spawnPoint_PlayerSpawn.forward);
		}

		private void InitBeginningEquipment()
		{
			SupplyPoints[m_curPointSequence.StartSupplyPointIndex].ConfigureAtBeginning(C);
		}

		public int GetNumTokens()
		{
			return m_numTokens;
		}

		public void AddTokens(int i, bool Scorethis)
		{
			m_numTokens += i;
			if (Scorethis)
			{
				Increment(8, i, statOnly: false);
			}
			OnTokenCountChange(m_numTokens);
		}

		public void SubtractTokens(int i)
		{
			m_numTokens -= i;
			OnTokenCountChange(m_numTokens);
		}

		public void OnTokenCountChange(int numTokens)
		{
			if (this.TokenCountChangeEvent != null)
			{
				this.TokenCountChangeEvent(numTokens);
			}
		}

		private void SetLevel(int level)
		{
			if (ProgressionMode == TNHSetting_ProgressionType.FiveHold)
			{
				m_curLevel = m_curProgression.Levels[level];
			}
			else if (level < m_curProgression.Levels.Count)
			{
				m_curLevel = m_curProgression.Levels[level];
			}
			else
			{
				m_curLevel = m_curProgressionEndless.Levels[UnityEngine.Random.Range(0, m_curProgressionEndless.Levels.Count)];
			}
		}

		private void SetPhase(TNH_Phase p)
		{
			if (Phase != TNH_Phase.Dead && Phase != TNH_Phase.Completed)
			{
				Phase = p;
				switch (Phase)
				{
				case TNH_Phase.Take:
					SetPhase_Take();
					break;
				case TNH_Phase.Hold:
					SetPhase_Hold();
					break;
				case TNH_Phase.Completed:
					SetPhase_Completed();
					break;
				case TNH_Phase.Dead:
					SetPhase_Dead();
					break;
				}
			}
		}

		private void SetPhase_Take()
		{
			if (RadarMode == TNHModifier_RadarMode.Standard)
			{
				TAHReticle.GetComponent<AIEntity>().LM_VisualOcclusionCheck = ReticleMask_Take;
			}
			else if (RadarMode == TNHModifier_RadarMode.Omnipresent)
			{
				TAHReticle.GetComponent<AIEntity>().LM_VisualOcclusionCheck = ReticleMask_Hold;
			}
			int curHoldIndex = m_curHoldIndex;
			TAHReticle.DeRegisterTrackedType(TAH_ReticleContact.ContactType.Hold);
			TAHReticle.DeRegisterTrackedType(TAH_ReticleContact.ContactType.Supply);
			int num = 0;
			if (m_level < m_curPointSequence.HoldPoints.Count)
			{
				num = m_curPointSequence.HoldPoints[m_level];
			}
			else
			{
				List<int> list = new List<int>();
				int num2 = curHoldIndex;
				for (int i = 0; i < SafePosMatrix.Entries_HoldPoints[curHoldIndex].SafePositions_HoldPoints.Count; i++)
				{
					if (i != num2 && SafePosMatrix.Entries_HoldPoints[curHoldIndex].SafePositions_HoldPoints[i])
					{
						list.Add(i);
					}
				}
				list.Shuffle();
				num = list[0];
			}
			m_curHoldIndex = num;
			m_curHoldPoint = HoldPoints[num];
			m_curHoldPoint.ConfigureAsSystemNode(m_curLevel.TakeChallenge, m_curLevel.HoldChallenge, m_curLevel.NumOverrideTokensForHold);
			TAHReticle.RegisterTrackedObject(m_curHoldPoint.SpawnPoint_SystemNode, TAH_ReticleContact.ContactType.Hold);
			List<int> list2 = new List<int>();
			int num3 = -1;
			if (m_level == 0)
			{
				for (int j = 0; j < SafePosMatrix.Entries_SupplyPoints[m_curPointSequence.StartSupplyPointIndex].SafePositions_SupplyPoints.Count; j++)
				{
					if (j != num3 && SafePosMatrix.Entries_SupplyPoints[m_curPointSequence.StartSupplyPointIndex].SafePositions_SupplyPoints[j])
					{
						list2.Add(j);
					}
				}
			}
			else
			{
				for (int k = 0; k < SafePosMatrix.Entries_HoldPoints[curHoldIndex].SafePositions_SupplyPoints.Count; k++)
				{
					if (k != num3 && SafePosMatrix.Entries_HoldPoints[curHoldIndex].SafePositions_SupplyPoints[k])
					{
						list2.Add(k);
					}
				}
			}
			list2.Shuffle();
			list2.Shuffle();
			int min = Mathf.Min(m_level + 1, 2);
			int num4 = Mathf.Min(3, m_level + 1);
			int num5 = UnityEngine.Random.Range(min, num4 + 1);
			num5 = Mathf.Clamp(num5, num5, list2.Count);
			for (int l = 0; l < num5; l++)
			{
				TNH_SupplyPoint tNH_SupplyPoint = SupplyPoints[list2[l]];
				TNH_SupplyPoint.SupplyPanelType panelType = TNH_SupplyPoint.SupplyPanelType.AmmoReloader;
				if (l > 0)
				{
					panelType = (TNH_SupplyPoint.SupplyPanelType)UnityEngine.Random.Range(1, 3);
				}
				tNH_SupplyPoint.Configure(m_curLevel.SupplyChallenge, spawnSosigs: true, spawnDefenses: true, spawnConstructor: true, panelType, 1, 2);
				TAH_ReticleContact contact = TAHReticle.RegisterTrackedObject(tNH_SupplyPoint.SpawnPoint_PlayerSpawn, TAH_ReticleContact.ContactType.Supply);
				tNH_SupplyPoint.SetContact(contact);
			}
			if (m_level == 0)
			{
				GenerateValidPatrol(m_curLevel.PatrolChallenge, m_curPointSequence.StartSupplyPointIndex, m_curHoldIndex, isStart: true);
			}
			else
			{
				GenerateValidPatrol(m_curLevel.PatrolChallenge, curHoldIndex, m_curHoldIndex, isStart: false);
			}
			if (BGAudioMode == TNH_BGAudioMode.Default)
			{
				FMODController.SwitchTo(0, 2f, shouldStop: false, shouldDeadStop: false);
			}
		}

		private void SetPhase_Hold()
		{
			CleanupOutsideCurHoldPoint();
			KillAllPatrols();
			TAHReticle.GetComponent<AIEntity>().LM_VisualOcclusionCheck = ReticleMask_Hold;
			TAHReticle.DeRegisterTrackedType(TAH_ReticleContact.ContactType.Supply);
			if (BGAudioMode == TNH_BGAudioMode.Default)
			{
				FMODController.SwitchTo(1, 0f, shouldStop: true, shouldDeadStop: false);
			}
			for (int i = 0; i < HoldPoints.Count; i++)
			{
				HoldPoints[i].DeleteAllActiveEntities();
			}
			for (int j = 0; j < SupplyPoints.Count; j++)
			{
				SupplyPoints[j].DeleteAllActiveEntities();
			}
		}

		private void SetPhase_Completed()
		{
			KillAllPatrols();
			FMODController.SwitchTo(0, 2f, shouldStop: false, shouldDeadStop: false);
			for (int i = 0; i < HoldPoints.Count; i++)
			{
				HoldPoints[i].ForceClearConfiguration();
			}
			for (int j = 0; j < SupplyPoints.Count; j++)
			{
				SupplyPoints[j].DeleteAllActiveEntities();
			}
			EnqueueLine(TNH_VoiceLineID.AI_ReturningToInterface);
			GM.CurrentMovementManager.TeleportToPoint(GM.CurrentSceneSettings.DeathResetPoint.position, isAbsolute: true);
			DispatchScore();
		}

		private void SetPhase_Dead()
		{
			KillAllPatrols();
			FMODController.SwitchTo(0, 2f, shouldStop: false, shouldDeadStop: false);
			for (int i = 0; i < HoldPoints.Count; i++)
			{
				HoldPoints[i].ForceClearConfiguration();
			}
			for (int j = 0; j < SupplyPoints.Count; j++)
			{
				SupplyPoints[j].DeleteAllActiveEntities();
			}
			DispatchScore();
		}

		private int GetNextHoldPointIndex()
		{
			return 0;
		}

		private void DispatchScore()
		{
			string text = FloatToTime(m_timePlaying, "#00:00");
			ScoringLabels[0].text = text;
			int num = Stats[0];
			ScoringLabels[1].text = num.ToString();
			int num2 = Stats[3];
			ScoringLabels[2].text = num2.ToString();
			int num3 = Stats[4];
			ScoringLabels[3].text = num3.ToString();
			int num4 = Stats[5];
			ScoringLabels[4].text = num4.ToString();
			int num5 = Stats[8];
			ScoringLabels[5].text = num5.ToString();
			int num6 = ReturnNum();
			ScoringLabels[6].text = num6.ToString();
			int num7 = 1;
			if (GM.TNHOptions.AIDifficultyModifier == TNHModifier_AIDifficulty.Standard)
			{
				num7 += 5;
				ScoringLabels[8].text = "5x";
			}
			else
			{
				ScoringLabels[8].text = "1x";
			}
			if (GM.TNHOptions.RadarModeModifier == TNHModifier_RadarMode.Standard)
			{
				num7 += 5;
				ScoringLabels[9].text = "5x";
			}
			else if (GM.TNHOptions.RadarModeModifier == TNHModifier_RadarMode.Omnipresent)
			{
				ScoringLabels[9].text = "1x";
			}
			else
			{
				num7 += 10;
				ScoringLabels[9].text = "10x";
			}
			int score = num6 * num7;
			ScoringLabels[10].text = score.ToString();
			TNH_Char @char = GM.TNHOptions.Char;
			TNH_CharacterDef def = CharDB.GetDef(@char);
			string tableID = ScoreDisplay.GetTableID(LevelName, def.TableID, GM.TNHOptions.ProgressionTypeSetting, GM.TNHOptions.EquipmentModeSetting, GM.TNHOptions.HealthModeSetting);
			ScoreDisplay.ForceSetSequenceID(tableID);
			Debug.Log("End Of Take & Hold Run - Global Score submitted");
			if (GM.TNHOptions.ItemSpawnerMode == TNH_ItemSpawnerMode.Off && GM.TNHOptions.HealthModeSetting != TNHSetting_HealthMode.CustomHealth)
			{
				ScoreDisplay.SubmitScoreAndGoToBoard(score);
			}
			else
			{
				Debug.Log("Score not submitted due to item spawner being on");
			}
			ScoreDisplay.SwitchToModeID(tableID);
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

		private void CleanupOutsideCurHoldPoint()
		{
		}

		private void CleanupAll()
		{
		}

		public void Update()
		{
			DelayedInit();
			if (BGAudioMode == TNH_BGAudioMode.Default)
			{
				FMODController.Tick(Time.deltaTime);
			}
			switch (Phase)
			{
			case TNH_Phase.Take:
				Update_Take();
				break;
			case TNH_Phase.Hold:
				Update_Hold();
				break;
			}
			if (Phase == TNH_Phase.Take || Phase == TNH_Phase.Hold)
			{
				m_timePlaying += Time.deltaTime;
			}
			VoiceUpdate();
			FMODController.SetMasterVolume(0.25f * GM.CurrentPlayerBody.GlobalHearing);
		}

		public void SetHoldWaveIntensity(int intensity)
		{
			if (BGAudioMode == TNH_BGAudioMode.Default)
			{
				FMODController.SetIntParameterByIndex(1, "Intensity", intensity);
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

		private void OnSosigKill(Sosig s)
		{
			if (Phase == TNH_Phase.Take)
			{
				TakingBotKill();
			}
			bool statOnly = false;
			if (Phase == TNH_Phase.Take)
			{
				statOnly = true;
			}
			int diedFromIFF = s.GetDiedFromIFF();
			if (diedFromIFF == GM.CurrentPlayerBody.GetPlayerIFF())
			{
				Increment(3, statOnly);
				if (s.GetDiedFromHeadShot())
				{
					Increment(4, statOnly);
				}
				Damage.DamageClass diedFromClass = s.GetDiedFromClass();
				Sosig.SosigDeathType diedFromType = s.GetDiedFromType();
				if (diedFromClass == Damage.DamageClass.Melee)
				{
					Increment(5, statOnly);
				}
				switch (diedFromType)
				{
				case Sosig.SosigDeathType.JointBreak:
					Increment(6, statOnly);
					break;
				case Sosig.SosigDeathType.JointPullApart:
					Increment(7, statOnly);
					break;
				}
			}
			s.TickDownToClear(3f);
		}

		private void OnShotFired(FVRFireArm firearm)
		{
			if (Phase == TNH_Phase.Take)
			{
				TakingGunShot();
			}
		}

		private void OnBotShotFired(wwBotWurstModernGun gun)
		{
			if (Phase == TNH_Phase.Take)
			{
				TakingGunShot();
			}
		}

		private void Update_Take()
		{
			UpdatePatrols();
			for (int i = 0; i < SupplyPoints.Count; i++)
			{
				SupplyPoints[i].TestVisited();
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
			if (BGAudioMode == TNH_BGAudioMode.Default)
			{
				FMODController.SetIntParameterByIndex(0, "Intensity", m_takeMusicIntensity);
			}
			Vector3 position = GM.CurrentPlayerBody.LeftHand.position + GM.CurrentPlayerBody.LeftHand.forward * -0.2f;
			if (RadarHand == TNH_RadarHand.Right)
			{
				position = GM.CurrentPlayerBody.RightHand.position + GM.CurrentPlayerBody.RightHand.forward * -0.2f;
			}
			TAHReticle.transform.position = position;
		}

		private void Update_Hold()
		{
			ObjectCleanupInHold();
			Vector3 position = GM.CurrentPlayerBody.LeftHand.position + GM.CurrentPlayerBody.LeftHand.forward * -0.2f;
			if (RadarHand == TNH_RadarHand.Right)
			{
				position = GM.CurrentPlayerBody.RightHand.position + GM.CurrentPlayerBody.RightHand.forward * -0.2f;
			}
			TAHReticle.transform.position = position;
		}

		public void HoldPointStarted(TNH_HoldPoint p)
		{
			SetPhase(TNH_Phase.Hold);
		}

		public void HoldPointCompleted(TNH_HoldPoint p, bool success)
		{
			m_level++;
			if (m_level < m_maxLevels)
			{
				SetLevel(m_level);
				SetPhase(TNH_Phase.Take);
			}
			else
			{
				SetPhase(TNH_Phase.Completed);
			}
		}

		public void EnqueueLine(TNH_VoiceLineID id)
		{
			if (AIVoiceMode != TNH_AIVoiceMode.Disabled)
			{
				QueuedLines.Enqueue(id);
			}
		}

		public void EnqueueEncryptionLine(TNH_EncryptionType t)
		{
			switch (t)
			{
			case TNH_EncryptionType.Static:
				EnqueueLine(TNH_VoiceLineID.AI_EncryptionType_0);
				break;
			case TNH_EncryptionType.Hardened:
				EnqueueLine(TNH_VoiceLineID.AI_EncryptionType_1);
				break;
			case TNH_EncryptionType.Swarm:
				EnqueueLine(TNH_VoiceLineID.AI_EncryptionType_2);
				break;
			case TNH_EncryptionType.Recursive:
				EnqueueLine(TNH_VoiceLineID.AI_EncryptionType_3);
				break;
			case TNH_EncryptionType.Stealth:
				EnqueueLine(TNH_VoiceLineID.AI_EncryptionType_4);
				break;
			case TNH_EncryptionType.Agile:
				EnqueueLine(TNH_VoiceLineID.AI_EncryptionType_5);
				break;
			case TNH_EncryptionType.Regenerative:
				EnqueueLine(TNH_VoiceLineID.AI_EncryptionType_6);
				break;
			case TNH_EncryptionType.Polymorphic:
				EnqueueLine(TNH_VoiceLineID.AI_EncryptionType_7);
				break;
			case TNH_EncryptionType.Cascading:
				EnqueueLine(TNH_VoiceLineID.AI_EncryptionType_8);
				break;
			case TNH_EncryptionType.Orthagonal:
				EnqueueLine(TNH_VoiceLineID.AI_EncryptionType_9);
				break;
			case TNH_EncryptionType.Refractive:
				EnqueueLine(TNH_VoiceLineID.AI_EncryptionType_10);
				break;
			}
		}

		public void EnqueueTokenLine(int i)
		{
			switch (i)
			{
			case 1:
				EnqueueLine(TNH_VoiceLineID.AI_OverrideTokenFound_1);
				break;
			case 2:
				EnqueueLine(TNH_VoiceLineID.AI_OverrideTokenFound_2);
				break;
			case 3:
				EnqueueLine(TNH_VoiceLineID.AI_OverrideTokenFound_3);
				break;
			case 4:
				EnqueueLine(TNH_VoiceLineID.AI_OverrideTokenFound_4);
				break;
			case 5:
				EnqueueLine(TNH_VoiceLineID.AI_OverrideTokenFound_5);
				break;
			}
		}

		private void VoiceUpdate()
		{
			if (timeTilLineClear >= 0f)
			{
				timeTilLineClear -= Time.deltaTime;
			}
			else if (QueuedLines.Count > 0)
			{
				TNH_VoiceLineID key = QueuedLines.Dequeue();
				if (voiceDic_Standard.ContainsKey(key))
				{
					int index = UnityEngine.Random.Range(0, voiceDic_Standard[key].Count);
					AudioClip audioClip = voiceDic_Standard[key][index];
					AudioEvent audioEvent = new AudioEvent();
					audioEvent.Clips.Add(audioClip);
					audioEvent.PitchRange = new Vector2(1f, 1f);
					audioEvent.VolumeRange = new Vector2(0.6f, 0.6f);
					timeTilLineClear = audioClip.length + 1.2f;
					SM.PlayCoreSoundDelayed(FVRPooledAudioType.UIChirp, audioEvent, base.transform.position, 0.2f);
				}
			}
		}

		public GameObject SpawnObjectConstructor(Transform point)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(Prefab_ObjectConstructor, point.position, point.rotation);
			TNH_ObjectConstructor component = gameObject.GetComponent<TNH_ObjectConstructor>();
			EquipmentPoolDef.PoolEntry poolEntry = component.GetPoolEntry(m_level, C.EquipmentPool, EquipmentPoolDef.PoolEntry.PoolEntryType.Firearm);
			EquipmentPoolDef.PoolEntry poolEntry2 = component.GetPoolEntry(m_level, C.EquipmentPool, EquipmentPoolDef.PoolEntry.PoolEntryType.Equipment);
			EquipmentPoolDef.PoolEntry poolEntry3 = component.GetPoolEntry(m_level, C.EquipmentPool, EquipmentPoolDef.PoolEntry.PoolEntryType.Consumable);
			component.SetEntries(this, m_level, C.EquipmentPool, poolEntry, poolEntry2, poolEntry3);
			component.SetRequiredPicatinnySightTable(C.RequireSightTable);
			component.SetValidErasSets(C.ValidAmmoEras, C.ValidAmmoSets);
			return gameObject;
		}

		public void RegenerateConstructor(TNH_ObjectConstructor ObjectConstructor, int which)
		{
			switch (which)
			{
			case 0:
				ObjectConstructor.ResetEntry(m_level, C.EquipmentPool, EquipmentPoolDef.PoolEntry.PoolEntryType.Firearm, which);
				break;
			case 1:
				ObjectConstructor.ResetEntry(m_level, C.EquipmentPool, EquipmentPoolDef.PoolEntry.PoolEntryType.Equipment, which);
				break;
			case 2:
				ObjectConstructor.ResetEntry(m_level, C.EquipmentPool, EquipmentPoolDef.PoolEntry.PoolEntryType.Consumable, which);
				break;
			}
		}

		public GameObject SpawnAmmoReloader(Transform point)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(Prefab_AmmoReloader, point.position, point.rotation);
			TNH_AmmoReloader component = gameObject.GetComponent<TNH_AmmoReloader>();
			component.SetValidErasSets(C.ValidAmmoEras, C.ValidAmmoSets);
			component.M = this;
			return gameObject;
		}

		public GameObject SpawnMagDuplicator(Transform point)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(Prefab_MagDuplicator, point.position, point.rotation);
			TNH_MagDuplicator component = gameObject.GetComponent<TNH_MagDuplicator>();
			component.M = this;
			return gameObject;
		}

		public GameObject SpawnGunRecycler(Transform point)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(Prefab_GunRecycler, point.position, point.rotation);
			TNH_GunRecycler component = gameObject.GetComponent<TNH_GunRecycler>();
			component.M = this;
			return gameObject;
		}

		public GameObject SpawnWeaponCase(GameObject caseFab, Vector3 position, Vector3 forward, FVRObject weapon, int numMag, int numRound, int minAmmo, int maxAmmo, FVRObject ammoObjOverride = null)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(caseFab, position, Quaternion.LookRotation(forward, Vector3.up));
			m_weaponCases.Add(gameObject);
			TNH_WeaponCrate component = gameObject.GetComponent<TNH_WeaponCrate>();
			FVRObject fVRObject = ((!(ammoObjOverride == null)) ? ammoObjOverride : weapon.GetRandomAmmoObject(weapon, C.ValidAmmoEras, minAmmo, maxAmmo, C.ValidAmmoSets));
			int num = 0;
			num = ((!(fVRObject != null) || fVRObject.Category != FVRObject.ObjectCategory.Cartridge) ? numMag : numRound);
			FVRObject fVRObject2 = null;
			FVRObject requiredAttachment_B = null;
			if (weapon.RequiresPicatinnySight)
			{
				fVRObject2 = GetObjectTable(C.RequireSightTable).GetRandomObject();
				if (fVRObject2.RequiredSecondaryPieces.Count > 0)
				{
					requiredAttachment_B = fVRObject2.RequiredSecondaryPieces[0];
				}
			}
			if (weapon.RequiredSecondaryPieces.Count > 0)
			{
				requiredAttachment_B = weapon.RequiredSecondaryPieces[0];
			}
			component.PlaceWeaponInContainer(weapon, fVRObject2, requiredAttachment_B, fVRObject, num);
			return gameObject;
		}

		public Sosig SpawnEnemy(SosigEnemyTemplate t, Transform point, int IFF, bool IsAssault, Vector3 pointOfInterest, bool AllowAllWeapons)
		{
			if (C.ForceAllAgentWeapons)
			{
				AllowAllWeapons = true;
			}
			GameObject weaponPrefab = null;
			if (t.WeaponOptions.Count > 0)
			{
				weaponPrefab = t.WeaponOptions[UnityEngine.Random.Range(0, t.WeaponOptions.Count)].GetGameObject();
			}
			GameObject weaponPrefab2 = null;
			if (t.WeaponOptions_Secondary.Count > 0 && AllowAllWeapons)
			{
				float num = UnityEngine.Random.Range(0f, 1f);
				if (num <= t.SecondaryChance)
				{
					weaponPrefab2 = t.WeaponOptions_Secondary[UnityEngine.Random.Range(0, t.WeaponOptions_Secondary.Count)].GetGameObject();
				}
			}
			GameObject weaponPrefab3 = null;
			if (t.WeaponOptions_Tertiary.Count > 0 && AllowAllWeapons)
			{
				float num2 = UnityEngine.Random.Range(0f, 1f);
				if (num2 <= t.TertiaryChance)
				{
					weaponPrefab3 = t.WeaponOptions_Tertiary[UnityEngine.Random.Range(0, t.WeaponOptions_Tertiary.Count)].GetGameObject();
				}
			}
			SosigConfigTemplate t2 = t.ConfigTemplates[UnityEngine.Random.Range(0, t.ConfigTemplates.Count)];
			if (AI_Difficulty == TNHModifier_AIDifficulty.Arcade && t.ConfigTemplates_Easy.Count > 0)
			{
				t2 = t.ConfigTemplates_Easy[UnityEngine.Random.Range(0, t.ConfigTemplates.Count)];
			}
			return SpawnEnemySosig(t.SosigPrefabs[UnityEngine.Random.Range(0, t.SosigPrefabs.Count)].GetGameObject(), weaponPrefab, weaponPrefab2, weaponPrefab3, point.position, point.rotation, t2, t.OutfitConfig[UnityEngine.Random.Range(0, t.OutfitConfig.Count)], IFF, IsAssault, pointOfInterest);
		}

		private Sosig SpawnEnemySosig(GameObject prefab, GameObject weaponPrefab, GameObject weaponPrefab2, GameObject weaponPrefab3, Vector3 pos, Quaternion rot, SosigConfigTemplate t, SosigOutfitConfig o, int IFF, bool IsAssault, Vector3 pointOfInterest)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(prefab, pos, rot);
			Sosig componentInChildren = gameObject.GetComponentInChildren<Sosig>();
			componentInChildren.Configure(t);
			componentInChildren.E.IFFCode = IFF;
			if (weaponPrefab != null)
			{
				SosigWeapon component = UnityEngine.Object.Instantiate(weaponPrefab, pos + Vector3.up * 0.1f, rot).GetComponent<SosigWeapon>();
				component.SetAutoDestroy(b: true);
				component.O.SpawnLockable = false;
				if (component.Type == SosigWeapon.SosigWeaponType.Gun)
				{
					componentInChildren.Inventory.FillAmmoWithType(component.AmmoType);
				}
				componentInChildren.Inventory.Init();
				componentInChildren.Inventory.FillAllAmmo();
				if (component != null)
				{
					componentInChildren.InitHands();
					componentInChildren.ForceEquip(component);
					component.SetAmmoClamping(b: true);
					if (AI_Difficulty == TNHModifier_AIDifficulty.Arcade)
					{
						component.FlightVelocityMultiplier = 0.3f;
					}
				}
				if (weaponPrefab2 != null)
				{
					SosigWeapon component2 = UnityEngine.Object.Instantiate(weaponPrefab2, pos + Vector3.up * 0.1f, rot).GetComponent<SosigWeapon>();
					component2.SetAutoDestroy(b: true);
					component2.O.SpawnLockable = false;
					component2.SetAmmoClamping(b: true);
					if (component2.Type == SosigWeapon.SosigWeaponType.Gun)
					{
						componentInChildren.Inventory.FillAmmoWithType(component2.AmmoType);
					}
					if (component2 != null)
					{
						componentInChildren.ForceEquip(component2);
					}
					if (AI_Difficulty == TNHModifier_AIDifficulty.Arcade)
					{
						component2.FlightVelocityMultiplier = 0.3f;
					}
				}
				if (weaponPrefab3 != null)
				{
					SosigWeapon component3 = UnityEngine.Object.Instantiate(weaponPrefab3, pos + Vector3.up * 0.1f, rot).GetComponent<SosigWeapon>();
					component3.SetAutoDestroy(b: true);
					component3.O.SpawnLockable = false;
					component3.SetAmmoClamping(b: true);
					if (component3.Type == SosigWeapon.SosigWeaponType.Gun)
					{
						componentInChildren.Inventory.FillAmmoWithType(component3.AmmoType);
					}
					if (component3 != null)
					{
						componentInChildren.ForceEquip(component3);
					}
					if (AI_Difficulty == TNHModifier_AIDifficulty.Arcade)
					{
						component3.FlightVelocityMultiplier = 0.3f;
					}
				}
			}
			float num = 0f;
			num = UnityEngine.Random.Range(0f, 1f);
			if (num < o.Chance_Headwear)
			{
				SpawnAccesoryToLink(o.Headwear, componentInChildren.Links[0]);
			}
			num = UnityEngine.Random.Range(0f, 1f);
			if (num < o.Chance_Facewear)
			{
				SpawnAccesoryToLink(o.Facewear, componentInChildren.Links[0]);
			}
			num = UnityEngine.Random.Range(0f, 1f);
			if (num < o.Chance_Eyewear)
			{
				SpawnAccesoryToLink(o.Eyewear, componentInChildren.Links[0]);
			}
			num = UnityEngine.Random.Range(0f, 1f);
			if (num < o.Chance_Torsowear)
			{
				SpawnAccesoryToLink(o.Torsowear, componentInChildren.Links[1]);
			}
			num = UnityEngine.Random.Range(0f, 1f);
			if (num < o.Chance_Pantswear)
			{
				SpawnAccesoryToLink(o.Pantswear, componentInChildren.Links[2]);
			}
			num = UnityEngine.Random.Range(0f, 1f);
			if (num < o.Chance_Pantswear_Lower)
			{
				SpawnAccesoryToLink(o.Pantswear_Lower, componentInChildren.Links[3]);
			}
			num = UnityEngine.Random.Range(0f, 1f);
			if (num < o.Chance_Backpacks)
			{
				SpawnAccesoryToLink(o.Backpacks, componentInChildren.Links[1]);
			}
			if (t.UsesLinkSpawns)
			{
				for (int i = 0; i < componentInChildren.Links.Count; i++)
				{
					float num2 = UnityEngine.Random.Range(0f, 1f);
					if (t.LinkSpawns.Count > i && t.LinkSpawns[i] != null && t.LinkSpawns[i].Category != FVRObject.ObjectCategory.Loot && num2 < t.LinkSpawnChance[i])
					{
						componentInChildren.Links[i].RegisterSpawnOnDestroy(t.LinkSpawns[i]);
					}
				}
			}
			if (IsAssault)
			{
				componentInChildren.CurrentOrder = Sosig.SosigOrder.Assault;
				componentInChildren.FallbackOrder = Sosig.SosigOrder.Assault;
				componentInChildren.CommandAssaultPoint(pointOfInterest);
			}
			else
			{
				componentInChildren.CurrentOrder = Sosig.SosigOrder.Wander;
				componentInChildren.FallbackOrder = Sosig.SosigOrder.Wander;
				float num3 = UnityEngine.Random.Range(0f, 1f);
				bool flag = false;
				if (num3 > 0.25f)
				{
					flag = true;
				}
				componentInChildren.CommandGuardPoint(pointOfInterest, hardguard: true);
				componentInChildren.SetDominantGuardDirection(UnityEngine.Random.onUnitSphere);
			}
			componentInChildren.SetGuardInvestigateDistanceThreshold(25f);
			return componentInChildren;
		}

		private void SpawnAccesoryToLink(List<FVRObject> gs, SosigLink l)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(gs[UnityEngine.Random.Range(0, gs.Count)].GetGameObject(), l.transform.position, l.transform.rotation);
			gameObject.transform.SetParent(l.transform);
			SosigWearable component = gameObject.GetComponent<SosigWearable>();
			component.RegisterWearable(l);
		}

		private void GenerateValidPatrol(TNH_PatrolChallenge P, int curStandardIndex, int excludeHoldIndex, bool isStart)
		{
			if (P.Patrols.Count < 1)
			{
				return;
			}
			TNH_PatrolChallenge.Patrol patrol = P.Patrols[UnityEngine.Random.Range(0, P.Patrols.Count)];
			List<int> list = new List<int>();
			float num = TAHReticle.Range * 1.2f;
			if (isStart)
			{
				for (int i = 0; i < SafePosMatrix.Entries_SupplyPoints[curStandardIndex].SafePositions_HoldPoints.Count; i++)
				{
					if (i != excludeHoldIndex && SafePosMatrix.Entries_SupplyPoints[curStandardIndex].SafePositions_HoldPoints[i])
					{
						float num2 = Vector3.Distance(GM.CurrentPlayerBody.transform.position, HoldPoints[i].transform.position);
						if (num2 > num)
						{
							list.Add(i);
						}
					}
				}
			}
			else
			{
				for (int j = 0; j < SafePosMatrix.Entries_HoldPoints[curStandardIndex].SafePositions_HoldPoints.Count; j++)
				{
					if (j != excludeHoldIndex && SafePosMatrix.Entries_HoldPoints[curStandardIndex].SafePositions_HoldPoints[j])
					{
						float num3 = Vector3.Distance(GM.CurrentPlayerBody.transform.position, HoldPoints[j].transform.position);
						if (num3 > num)
						{
							list.Add(j);
						}
					}
				}
			}
			list.Shuffle();
			list.Shuffle();
			if (list.Count < 1)
			{
				return;
			}
			int num4 = patrol.PatrolSize;
			if (num4 != 0)
			{
				int num5 = patrol.PatrolSize - 1;
				int num6 = patrol.PatrolSize + 2;
				num4 = UnityEngine.Random.Range(patrol.PatrolSize - 1, patrol.PatrolSize + 2);
				int max = 6;
				if (EquipmentMode == TNHSetting_EquipmentMode.LimitedAmmo)
				{
					num6 = 3;
				}
				num4 = Mathf.Clamp(num4, 1, max);
			}
			SosigPatrolSquad item = GeneratePatrol(patrol.LType, patrol.EType, num4, list[0], patrol.IFFUsed);
			m_patrolSquads.Add(item);
			if (EquipmentMode == TNHSetting_EquipmentMode.Spawnlocking)
			{
				m_timeTilPatrolCanSpawn = patrol.TimeTilRegen;
			}
			else if (EquipmentMode == TNHSetting_EquipmentMode.LimitedAmmo)
			{
				m_timeTilPatrolCanSpawn = patrol.TimeTilRegen_LimitedAmmo;
			}
		}

		private SosigPatrolSquad GeneratePatrol(SosigEnemyID Type_Leader, SosigEnemyID Type_Regular, int PatrolSize, int HoldPointStart, int iff)
		{
			SosigPatrolSquad sosigPatrolSquad = new SosigPatrolSquad();
			List<int> list = new List<int>();
			list.Add(HoldPointStart);
			int num = 0;
			int num2 = 0;
			while (num < 5)
			{
				int item = UnityEngine.Random.Range(0, HoldPoints.Count);
				if (!list.Contains(item))
				{
					list.Add(item);
					num++;
				}
				num2++;
				if (num2 > 200)
				{
					break;
				}
			}
			sosigPatrolSquad.PatrolPoints = new List<Vector3>();
			for (int i = 0; i < list.Count; i++)
			{
				sosigPatrolSquad.PatrolPoints.Add(HoldPoints[list[i]].SpawnPoints_Sosigs_Defense[UnityEngine.Random.Range(0, HoldPoints[list[i]].SpawnPoints_Sosigs_Defense.Count)].position);
			}
			HoldPoints[HoldPointStart].SpawnPoints_Sosigs_Defense.Shuffle();
			for (int j = 0; j < PatrolSize; j++)
			{
				Transform point = HoldPoints[HoldPointStart].SpawnPoints_Sosigs_Defense[j];
				SosigEnemyTemplate sosigEnemyTemplate = null;
				bool allowAllWeapons;
				if (j == 0)
				{
					sosigEnemyTemplate = ManagerSingleton<IM>.Instance.odicSosigObjsByID[Type_Leader];
					allowAllWeapons = true;
				}
				else
				{
					sosigEnemyTemplate = ManagerSingleton<IM>.Instance.odicSosigObjsByID[Type_Regular];
					allowAllWeapons = false;
				}
				Sosig sosig = SpawnEnemy(sosigEnemyTemplate, point, iff, IsAssault: true, sosigPatrolSquad.PatrolPoints[0], allowAllWeapons);
				float num3 = UnityEngine.Random.Range(0f, 1f);
				if (j == 0 && num3 > 0.65f)
				{
					sosig.Links[1].RegisterSpawnOnDestroy(Prefab_HealthPickupMinor);
				}
				sosig.SetAssaultSpeed(Sosig.SosigMoveSpeed.Walking);
				sosigPatrolSquad.Squad.Add(sosig);
			}
			return sosigPatrolSquad;
		}

		private void UpdatePatrols()
		{
			if (m_timeTilPatrolCanSpawn > 0f)
			{
				m_timeTilPatrolCanSpawn -= Time.deltaTime;
			}
			TNH_PatrolChallenge patrolChallenge = m_curLevel.PatrolChallenge;
			int maxPatrols = patrolChallenge.Patrols[0].MaxPatrols;
			if (EquipmentMode == TNHSetting_EquipmentMode.LimitedAmmo)
			{
				maxPatrols = patrolChallenge.Patrols[0].MaxPatrols_LimitedAmmo;
			}
			if (m_timeTilPatrolCanSpawn <= 0f && m_patrolSquads.Count < patrolChallenge.Patrols[0].MaxPatrols)
			{
				int num = -1;
				Vector3 position = GM.CurrentPlayerBody.Head.position;
				for (int i = 0; i < SupplyPoints.Count; i++)
				{
					if (SupplyPoints[i].IsPointInBounds(position))
					{
						num = i;
						break;
					}
				}
				if (num > -1)
				{
					GenerateValidPatrol(patrolChallenge, num, m_curHoldIndex, isStart: true);
				}
				else
				{
					m_timeTilPatrolCanSpawn = 6f;
				}
			}
			for (int j = 0; j < m_patrolSquads.Count; j++)
			{
				SosigPatrolSquad sosigPatrolSquad = m_patrolSquads[j];
				if (sosigPatrolSquad.Squad.Count <= 0)
				{
					continue;
				}
				for (int num2 = sosigPatrolSquad.Squad.Count - 1; num2 >= 0; num2--)
				{
					if (sosigPatrolSquad.Squad[num2] == null)
					{
						sosigPatrolSquad.Squad.RemoveAt(num2);
					}
				}
				bool flag = true;
				for (int k = 0; k < sosigPatrolSquad.Squad.Count; k++)
				{
					if (sosigPatrolSquad.Squad[k] != null)
					{
						float num3 = Vector3.Distance(sosigPatrolSquad.Squad[k].transform.position, sosigPatrolSquad.PatrolPoints[sosigPatrolSquad.CurPatrolPointIndex]);
						if (num3 > 4f)
						{
							flag = false;
						}
					}
				}
				if (flag)
				{
					if (sosigPatrolSquad.CurPatrolPointIndex + 1 >= sosigPatrolSquad.PatrolPoints.Count && sosigPatrolSquad.IsPatrollingUp)
					{
						sosigPatrolSquad.IsPatrollingUp = false;
					}
					if (sosigPatrolSquad.CurPatrolPointIndex == 0)
					{
						sosigPatrolSquad.IsPatrollingUp = true;
					}
					if (sosigPatrolSquad.IsPatrollingUp)
					{
						sosigPatrolSquad.CurPatrolPointIndex++;
					}
					else
					{
						sosigPatrolSquad.CurPatrolPointIndex--;
					}
					for (int l = 0; l < sosigPatrolSquad.Squad.Count; l++)
					{
						if (sosigPatrolSquad.Squad[l] != null)
						{
							sosigPatrolSquad.Squad[l].CommandAssaultPoint(sosigPatrolSquad.PatrolPoints[sosigPatrolSquad.CurPatrolPointIndex]);
						}
					}
				}
				for (int m = 0; m < sosigPatrolSquad.Squad.Count; m++)
				{
					if (sosigPatrolSquad.Squad[m] != null)
					{
						if (sosigPatrolSquad.Squad[m].CurrentOrder == Sosig.SosigOrder.Wander)
						{
							sosigPatrolSquad.Squad[m].CurrentOrder = Sosig.SosigOrder.Assault;
						}
						sosigPatrolSquad.Squad[m].FallbackOrder = Sosig.SosigOrder.Assault;
					}
				}
			}
			if (m_patrolSquads.Count < 1)
			{
				return;
			}
			for (int num4 = m_patrolSquads.Count - 1; num4 >= 0; num4--)
			{
				SosigPatrolSquad sosigPatrolSquad2 = m_patrolSquads[num4];
				if (sosigPatrolSquad2.Squad.Count < 1)
				{
					m_patrolSquads[num4].PatrolPoints.Clear();
					m_patrolSquads.RemoveAt(num4);
				}
			}
		}

		private void KillAllPatrols()
		{
			if (m_patrolSquads.Count < 1)
			{
				return;
			}
			for (int num = m_patrolSquads.Count - 1; num >= 0; num--)
			{
				if (m_patrolSquads[num].Squad.Count > 0)
				{
					for (int i = 0; i < m_patrolSquads[num].Squad.Count; i++)
					{
						if (m_patrolSquads[num].Squad[i] != null)
						{
							m_patrolSquads[num].Squad[i].ClearSosig();
						}
					}
				}
				m_patrolSquads[num].Squad.Clear();
				m_patrolSquads[num].PatrolPoints.Clear();
			}
			m_patrolSquads.Clear();
		}

		private void DeleteAllPatrols()
		{
			for (int num = m_patrolSquads.Count - 1; num >= 0; num--)
			{
				if (m_patrolSquads[num].Squad.Count > 0)
				{
					for (int num2 = m_patrolSquads[num].Squad.Count - 1; num2 >= 0; num2--)
					{
						if (m_patrolSquads[num].Squad[num2] == null)
						{
							m_patrolSquads[num].Squad.RemoveAt(num2);
						}
						else if (m_patrolSquads[num].Squad[num2] != null)
						{
							m_patrolSquads[num].Squad[num2].DeSpawnSosig();
						}
					}
				}
			}
			m_patrolSquads.Clear();
		}

		public void AddObjectToTrackedList(GameObject g)
		{
			FVRPhysicalObject component = g.GetComponent<FVRPhysicalObject>();
			if (component != null)
			{
				AddFVRObjectToTrackedList(component);
			}
		}

		public void AddFVRObjectToTrackedList(FVRPhysicalObject g)
		{
			if (m_knownObjsHash.Add(g))
			{
				if (g is FVRFireArm)
				{
					Increment(11, statOnly: false);
				}
				m_knownObjs.Add(g);
			}
		}

		private void ObjectCleanupInHold()
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
			Vector3 position = m_knownObjs[knownObjectCheckIndex].transform.position;
			if (!m_curHoldPoint.IsPointInBounds(position))
			{
				float num = Vector3.Distance(position, GM.CurrentPlayerBody.transform.position);
				if (num > 10f)
				{
					m_knownObjsHash.Remove(m_knownObjs[knownObjectCheckIndex]);
					UnityEngine.Object.Destroy(m_knownObjs[knownObjectCheckIndex].gameObject);
					m_knownObjs.RemoveAt(knownObjectCheckIndex);
				}
			}
		}

		private void InitLibraries()
		{
			for (int i = 0; i < ResourceLib.EncryptionObjects.Count; i++)
			{
				dic_encryption.Add((TNH_EncryptionType)i, ResourceLib.EncryptionObjects[i]);
			}
			for (int j = 0; j < ResourceLib.TurretObjects.Count; j++)
			{
				dic_turret.Add((TNH_TurretType)j, ResourceLib.TurretObjects[j]);
			}
			for (int k = 0; k < ResourceLib.TrapObjects.Count; k++)
			{
				dic_trap.Add((TNH_TrapType)k, ResourceLib.TrapObjects[k]);
			}
			for (int l = 0; l < VoiceDB.Lines.Count; l++)
			{
				if (voiceDic_Standard.ContainsKey(VoiceDB.Lines[l].ID))
				{
					voiceDic_Standard[VoiceDB.Lines[l].ID].Add(VoiceDB.Lines[l].Clip_Standard);
				}
				else
				{
					List<AudioClip> list = new List<AudioClip>();
					list.Add(VoiceDB.Lines[l].Clip_Standard);
					voiceDic_Standard.Add(VoiceDB.Lines[l].ID, list);
				}
				if (voiceDic_Corrupted.ContainsKey(VoiceDB.Lines[l].ID))
				{
					voiceDic_Corrupted[VoiceDB.Lines[l].ID].Add(VoiceDB.Lines[l].Clip_Corrupted);
					continue;
				}
				List<AudioClip> list2 = new List<AudioClip>();
				list2.Add(VoiceDB.Lines[l].Clip_Corrupted);
				voiceDic_Corrupted.Add(VoiceDB.Lines[l].ID, list2);
			}
		}

		public FVRObject GetEncryptionPrefab(TNH_EncryptionType t)
		{
			return dic_encryption[t];
		}

		public FVRObject GetTurretPrefab(TNH_TurretType t)
		{
			return dic_turret[t];
		}

		public FVRObject GetTrapPrefab(TNH_TrapType t)
		{
			return dic_trap[t];
		}

		private void PrimeNums()
		{
		}

		public void Increment(int i, bool statOnly)
		{
			int num = Stats[i];
			num++;
			Stats[i] = num;
			if (!statOnly)
			{
				int num2 = Nums[i];
				num2++;
				Nums[i] = num2;
			}
		}

		public void Increment(int i, int amount, bool statOnly)
		{
			int num = Stats[i];
			num += amount;
			Stats[i] = num;
			if (!statOnly)
			{
				int num2 = Nums[i];
				num2 += amount;
				Nums[i] = num2;
			}
		}

		public int ReturnNum()
		{
			int num = 0;
			num += Nums[0] * 200;
			num += Nums[1] * -20;
			num += Nums[2];
			num += Nums[3] * 2;
			num += Nums[4];
			num += Nums[5];
			num += Nums[6];
			num += Nums[7];
			num += Nums[8] * 50;
			num += Nums[9] * 20;
			num += Nums[10] * 5;
			return num + Nums[11];
		}
	}
}
