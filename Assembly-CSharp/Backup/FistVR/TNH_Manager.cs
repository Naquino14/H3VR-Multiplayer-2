// Decompiled with JetBrains decompiler
// Type: FistVR.TNH_Manager
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
  public class TNH_Manager : MonoBehaviour
  {
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
    private List<TNH_Manager.SosigPatrolSquad> m_patrolSquads = new List<TNH_Manager.SosigPatrolSquad>();
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

    public void AddToMiscEnemies(GameObject g) => this.m_miscEnemies.Add(g);

    public void ClearMiscEnemies()
    {
      for (int index = this.m_miscEnemies.Count - 1; index >= 0; --index)
      {
        if ((UnityEngine.Object) this.m_miscEnemies[index] != (UnityEngine.Object) null)
          UnityEngine.Object.Destroy((UnityEngine.Object) this.m_miscEnemies[index]);
      }
      this.m_miscEnemies.Clear();
    }

    private void Start()
    {
      GM.CurrentSceneSettings.SosigKillEvent += new FVRSceneSettings.SosigKill(this.OnSosigKill);
      GM.CurrentSceneSettings.ShotFiredEvent += new FVRSceneSettings.ShotFired(this.OnShotFired);
      GM.CurrentSceneSettings.BotShotFiredEvent += new FVRSceneSettings.BotShotFired(this.OnBotShotFired);
      GM.CurrentSceneSettings.ObjectPickedUpEvent += new FVRSceneSettings.FVRObjectPickedUp(this.AddFVRObjectToTrackedList);
      this.PrimeNums();
      GM.TNH_Manager = this;
      GM.CurrentAIManager.NumEntitiesToCheckPerFrame = 3;
    }

    private void OnDisable()
    {
      GM.CurrentSceneSettings.SosigKillEvent -= new FVRSceneSettings.SosigKill(this.OnSosigKill);
      GM.CurrentSceneSettings.ShotFiredEvent -= new FVRSceneSettings.ShotFired(this.OnShotFired);
      GM.CurrentSceneSettings.BotShotFiredEvent -= new FVRSceneSettings.BotShotFired(this.OnBotShotFired);
      GM.CurrentSceneSettings.ObjectPickedUpEvent -= new FVRSceneSettings.FVRObjectPickedUp(this.AddFVRObjectToTrackedList);
      GM.TNH_Manager = (TNH_Manager) null;
    }

    public void PlayerDied() => this.SetPhase(TNH_Phase.Dead);

    private void DelayedInit()
    {
      if (this.m_hasInit || !this.AIManager.HasInit)
        return;
      this.m_hasInit = true;
      this.C = this.CharDB.GetDef(GM.TNHOptions.Char);
      this.m_curProgression = this.C.Progressions[UnityEngine.Random.Range(0, this.C.Progressions.Count)];
      this.m_curProgressionEndless = this.C.Progressions_Endless[UnityEngine.Random.Range(0, this.C.Progressions_Endless.Count)];
      this.m_curPointSequence = this.PossibleSequnces[UnityEngine.Random.Range(0, this.PossibleSequnces.Count)];
      this.m_level = 0;
      this.SetLevel(this.m_level);
      this.InitLibraries();
      this.LoadFromSettings();
      this.InitPlayer();
      this.InitTables();
      this.InitSound();
      this.InitHoldPoints();
      this.InitPlayerPosition();
      this.InitBeginningEquipment();
      if (this.DebugMode)
        return;
      this.SetPhase(TNH_Phase.Take);
    }

    private void LoadFromSettings()
    {
      this.ProgressionMode = GM.TNHOptions.ProgressionTypeSetting;
      this.EquipmentMode = GM.TNHOptions.EquipmentModeSetting;
      if (this.EquipmentMode == TNHSetting_EquipmentMode.LimitedAmmo)
        GM.CurrentSceneSettings.IsSpawnLockingEnabled = false;
      this.HealthMode = GM.TNHOptions.HealthModeSetting;
      this.HealthMult = GM.TNHOptions.HealthMult;
      switch (this.HealthMode)
      {
        case TNHSetting_HealthMode.StandardHealth:
          GM.CurrentPlayerBody.SetHealthThreshold(5000f);
          break;
        case TNHSetting_HealthMode.HardcoreOneHit:
          GM.CurrentPlayerBody.SetHealthThreshold(100f);
          break;
        case TNHSetting_HealthMode.CustomHealth:
          switch (this.HealthMult)
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
      this.AI_Difficulty = GM.TNHOptions.AIDifficultyModifier;
      this.RadarMode = GM.TNHOptions.RadarModeModifier;
      if (this.RadarMode == TNHModifier_RadarMode.Off)
        this.TAHReticle.RegistersEnemies = false;
      this.ItemSpawnerMode = GM.TNHOptions.ItemSpawnerMode;
      this.BackpackMode = GM.TNHOptions.BackpackMode;
      this.BGAudioMode = GM.TNHOptions.BGAudioMode;
      this.AIVoiceMode = GM.TNHOptions.AIVoiceMode;
      this.RadarHand = GM.TNHOptions.RadarHand;
      this.m_numTokens = this.C.StartingTokens;
      this.OnTokenCountChange(this.m_numTokens);
      if (this.ProgressionMode == TNHSetting_ProgressionType.Marathon)
        this.m_maxLevels = 9999999;
      else
        this.m_maxLevels = this.m_curProgression.Levels.Count;
    }

    private void InitPlayer()
    {
    }

    public ObjectTable GetObjectTable(ObjectTableDef d) => this.m_objectTableDics[d];

    private void InitTables()
    {
      for (int index = 0; index < this.C.EquipmentPool.Entries.Count; ++index)
      {
        ObjectTable objectTable = new ObjectTable();
        objectTable.Initialize(this.C.EquipmentPool.Entries[index].TableDef);
        if (this.m_objectTableDics.ContainsKey(this.C.EquipmentPool.Entries[index].TableDef))
          Debug.Log((object) this.C.EquipmentPool.Entries[index].TableDef.name);
        else
          this.m_objectTableDics.Add(this.C.EquipmentPool.Entries[index].TableDef, objectTable);
      }
      ObjectTable objectTable1 = new ObjectTable();
      objectTable1.Initialize(this.C.RequireSightTable);
      this.m_objectTableDics.Add(this.C.RequireSightTable, objectTable1);
    }

    private void InitSound()
    {
      this.FMODController.SetMasterVolume(0.25f);
      this.EnqueueLine(TNH_VoiceLineID.AI_UplinkSuccessfulTargetSystemDetectedTakeIt);
    }

    private void InitHoldPoints()
    {
      for (int index = 0; index < this.HoldPoints.Count; ++index)
        this.HoldPoints[index].Init();
    }

    private void InitPlayerPosition()
    {
      Transform pointPlayerSpawn = this.SupplyPoints[this.m_curPointSequence.StartSupplyPointIndex].SpawnPoint_PlayerSpawn;
      double point = (double) GM.CurrentMovementManager.TeleportToPoint(pointPlayerSpawn.position, true, pointPlayerSpawn.forward);
    }

    private void InitBeginningEquipment() => this.SupplyPoints[this.m_curPointSequence.StartSupplyPointIndex].ConfigureAtBeginning(this.C);

    public int GetNumTokens() => this.m_numTokens;

    public void AddTokens(int i, bool Scorethis)
    {
      this.m_numTokens += i;
      if (Scorethis)
        this.Increment(8, i, false);
      this.OnTokenCountChange(this.m_numTokens);
    }

    public void SubtractTokens(int i)
    {
      this.m_numTokens -= i;
      this.OnTokenCountChange(this.m_numTokens);
    }

    public event TNH_Manager.TokenCountChange TokenCountChangeEvent;

    public void OnTokenCountChange(int numTokens)
    {
      if (this.TokenCountChangeEvent == null)
        return;
      this.TokenCountChangeEvent(numTokens);
    }

    private void SetLevel(int level)
    {
      if (this.ProgressionMode == TNHSetting_ProgressionType.FiveHold)
        this.m_curLevel = this.m_curProgression.Levels[level];
      else if (level < this.m_curProgression.Levels.Count)
        this.m_curLevel = this.m_curProgression.Levels[level];
      else
        this.m_curLevel = this.m_curProgressionEndless.Levels[UnityEngine.Random.Range(0, this.m_curProgressionEndless.Levels.Count)];
    }

    private void SetPhase(TNH_Phase p)
    {
      if (this.Phase == TNH_Phase.Dead || this.Phase == TNH_Phase.Completed)
        return;
      this.Phase = p;
      switch (this.Phase)
      {
        case TNH_Phase.Take:
          this.SetPhase_Take();
          break;
        case TNH_Phase.Hold:
          this.SetPhase_Hold();
          break;
        case TNH_Phase.Completed:
          this.SetPhase_Completed();
          break;
        case TNH_Phase.Dead:
          this.SetPhase_Dead();
          break;
      }
    }

    private void SetPhase_Take()
    {
      if (this.RadarMode == TNHModifier_RadarMode.Standard)
        this.TAHReticle.GetComponent<AIEntity>().LM_VisualOcclusionCheck = this.ReticleMask_Take;
      else if (this.RadarMode == TNHModifier_RadarMode.Omnipresent)
        this.TAHReticle.GetComponent<AIEntity>().LM_VisualOcclusionCheck = this.ReticleMask_Hold;
      int curHoldIndex = this.m_curHoldIndex;
      this.TAHReticle.DeRegisterTrackedType(TAH_ReticleContact.ContactType.Hold);
      this.TAHReticle.DeRegisterTrackedType(TAH_ReticleContact.ContactType.Supply);
      int holdPoint;
      if (this.m_level < this.m_curPointSequence.HoldPoints.Count)
      {
        holdPoint = this.m_curPointSequence.HoldPoints[this.m_level];
      }
      else
      {
        List<int> ts = new List<int>();
        int num = curHoldIndex;
        for (int index = 0; index < this.SafePosMatrix.Entries_HoldPoints[curHoldIndex].SafePositions_HoldPoints.Count; ++index)
        {
          if (index != num && this.SafePosMatrix.Entries_HoldPoints[curHoldIndex].SafePositions_HoldPoints[index])
            ts.Add(index);
        }
        ts.Shuffle<int>();
        holdPoint = ts[0];
      }
      this.m_curHoldIndex = holdPoint;
      this.m_curHoldPoint = this.HoldPoints[holdPoint];
      this.m_curHoldPoint.ConfigureAsSystemNode(this.m_curLevel.TakeChallenge, this.m_curLevel.HoldChallenge, this.m_curLevel.NumOverrideTokensForHold);
      this.TAHReticle.RegisterTrackedObject(this.m_curHoldPoint.SpawnPoint_SystemNode, TAH_ReticleContact.ContactType.Hold);
      List<int> ts1 = new List<int>();
      int num1 = -1;
      if (this.m_level == 0)
      {
        for (int index = 0; index < this.SafePosMatrix.Entries_SupplyPoints[this.m_curPointSequence.StartSupplyPointIndex].SafePositions_SupplyPoints.Count; ++index)
        {
          if (index != num1 && this.SafePosMatrix.Entries_SupplyPoints[this.m_curPointSequence.StartSupplyPointIndex].SafePositions_SupplyPoints[index])
            ts1.Add(index);
        }
      }
      else
      {
        for (int index = 0; index < this.SafePosMatrix.Entries_HoldPoints[curHoldIndex].SafePositions_SupplyPoints.Count; ++index)
        {
          if (index != num1 && this.SafePosMatrix.Entries_HoldPoints[curHoldIndex].SafePositions_SupplyPoints[index])
            ts1.Add(index);
        }
      }
      ts1.Shuffle<int>();
      ts1.Shuffle<int>();
      int min = UnityEngine.Random.Range(Mathf.Min(this.m_level + 1, 2), Mathf.Min(3, this.m_level + 1) + 1);
      int num2 = Mathf.Clamp(min, min, ts1.Count);
      for (int index = 0; index < num2; ++index)
      {
        TNH_SupplyPoint supplyPoint = this.SupplyPoints[ts1[index]];
        TNH_SupplyPoint.SupplyPanelType panelType = TNH_SupplyPoint.SupplyPanelType.AmmoReloader;
        if (index > 0)
          panelType = (TNH_SupplyPoint.SupplyPanelType) UnityEngine.Random.Range(1, 3);
        supplyPoint.Configure(this.m_curLevel.SupplyChallenge, true, true, true, panelType, 1, 2);
        TAH_ReticleContact c = this.TAHReticle.RegisterTrackedObject(supplyPoint.SpawnPoint_PlayerSpawn, TAH_ReticleContact.ContactType.Supply);
        supplyPoint.SetContact(c);
      }
      if (this.m_level == 0)
        this.GenerateValidPatrol(this.m_curLevel.PatrolChallenge, this.m_curPointSequence.StartSupplyPointIndex, this.m_curHoldIndex, true);
      else
        this.GenerateValidPatrol(this.m_curLevel.PatrolChallenge, curHoldIndex, this.m_curHoldIndex, false);
      if (this.BGAudioMode != TNH_BGAudioMode.Default)
        return;
      this.FMODController.SwitchTo(0, 2f, false, false);
    }

    private void SetPhase_Hold()
    {
      this.CleanupOutsideCurHoldPoint();
      this.KillAllPatrols();
      this.TAHReticle.GetComponent<AIEntity>().LM_VisualOcclusionCheck = this.ReticleMask_Hold;
      this.TAHReticle.DeRegisterTrackedType(TAH_ReticleContact.ContactType.Supply);
      if (this.BGAudioMode == TNH_BGAudioMode.Default)
        this.FMODController.SwitchTo(1, 0.0f, true, false);
      for (int index = 0; index < this.HoldPoints.Count; ++index)
        this.HoldPoints[index].DeleteAllActiveEntities();
      for (int index = 0; index < this.SupplyPoints.Count; ++index)
        this.SupplyPoints[index].DeleteAllActiveEntities();
    }

    private void SetPhase_Completed()
    {
      this.KillAllPatrols();
      this.FMODController.SwitchTo(0, 2f, false, false);
      for (int index = 0; index < this.HoldPoints.Count; ++index)
        this.HoldPoints[index].ForceClearConfiguration();
      for (int index = 0; index < this.SupplyPoints.Count; ++index)
        this.SupplyPoints[index].DeleteAllActiveEntities();
      this.EnqueueLine(TNH_VoiceLineID.AI_ReturningToInterface);
      double point = (double) GM.CurrentMovementManager.TeleportToPoint(GM.CurrentSceneSettings.DeathResetPoint.position, true);
      this.DispatchScore();
    }

    private void SetPhase_Dead()
    {
      this.KillAllPatrols();
      this.FMODController.SwitchTo(0, 2f, false, false);
      for (int index = 0; index < this.HoldPoints.Count; ++index)
        this.HoldPoints[index].ForceClearConfiguration();
      for (int index = 0; index < this.SupplyPoints.Count; ++index)
        this.SupplyPoints[index].DeleteAllActiveEntities();
      this.DispatchScore();
    }

    private int GetNextHoldPointIndex() => 0;

    private void DispatchScore()
    {
      this.ScoringLabels[0].text = this.FloatToTime(this.m_timePlaying, "#00:00");
      this.ScoringLabels[1].text = this.Stats[0].ToString();
      this.ScoringLabels[2].text = this.Stats[3].ToString();
      this.ScoringLabels[3].text = this.Stats[4].ToString();
      this.ScoringLabels[4].text = this.Stats[5].ToString();
      this.ScoringLabels[5].text = this.Stats[8].ToString();
      int num1 = this.ReturnNum();
      this.ScoringLabels[6].text = num1.ToString();
      int num2 = 1;
      if (GM.TNHOptions.AIDifficultyModifier == TNHModifier_AIDifficulty.Standard)
      {
        num2 += 5;
        this.ScoringLabels[8].text = "5x";
      }
      else
        this.ScoringLabels[8].text = "1x";
      if (GM.TNHOptions.RadarModeModifier == TNHModifier_RadarMode.Standard)
      {
        num2 += 5;
        this.ScoringLabels[9].text = "5x";
      }
      else if (GM.TNHOptions.RadarModeModifier == TNHModifier_RadarMode.Omnipresent)
      {
        this.ScoringLabels[9].text = "1x";
      }
      else
      {
        num2 += 10;
        this.ScoringLabels[9].text = "10x";
      }
      int score = num1 * num2;
      this.ScoringLabels[10].text = score.ToString();
      string tableId = this.ScoreDisplay.GetTableID(this.LevelName, this.CharDB.GetDef(GM.TNHOptions.Char).TableID, GM.TNHOptions.ProgressionTypeSetting, GM.TNHOptions.EquipmentModeSetting, GM.TNHOptions.HealthModeSetting);
      this.ScoreDisplay.ForceSetSequenceID(tableId);
      Debug.Log((object) "End Of Take & Hold Run - Global Score submitted");
      if (GM.TNHOptions.ItemSpawnerMode == TNH_ItemSpawnerMode.Off && GM.TNHOptions.HealthModeSetting != TNHSetting_HealthMode.CustomHealth)
        this.ScoreDisplay.SubmitScoreAndGoToBoard(score);
      else
        Debug.Log((object) "Score not submitted due to item spawner being on");
      this.ScoreDisplay.SwitchToModeID(tableId);
    }

    public string FloatToTime(float toConvert, string format)
    {
      if (format != null)
      {
        // ISSUE: reference to a compiler-generated field
        if (TNH_Manager.\u003C\u003Ef__switch\u0024map8 == null)
        {
          // ISSUE: reference to a compiler-generated field
          TNH_Manager.\u003C\u003Ef__switch\u0024map8 = new Dictionary<string, int>(13)
          {
            {
              "00.0",
              0
            },
            {
              "#0.0",
              1
            },
            {
              "00.00",
              2
            },
            {
              "00.000",
              3
            },
            {
              "#00.000",
              4
            },
            {
              "#0:00",
              5
            },
            {
              "#00:00",
              6
            },
            {
              "0:00.0",
              7
            },
            {
              "#0:00.0",
              8
            },
            {
              "0:00.00",
              9
            },
            {
              "#0:00.00",
              10
            },
            {
              "0:00.000",
              11
            },
            {
              "#0:00.000",
              12
            }
          };
        }
        int num;
        // ISSUE: reference to a compiler-generated field
        if (TNH_Manager.\u003C\u003Ef__switch\u0024map8.TryGetValue(format, out num))
        {
          switch (num)
          {
            case 0:
              return string.Format("{0:00}:{1:0}", (object) (float) ((double) Mathf.Floor(toConvert) % 60.0), (object) Mathf.Floor((float) ((double) toConvert * 10.0 % 10.0)));
            case 1:
              return string.Format("{0:#0}:{1:0}", (object) (float) ((double) Mathf.Floor(toConvert) % 60.0), (object) Mathf.Floor((float) ((double) toConvert * 10.0 % 10.0)));
            case 2:
              return string.Format("{0:00}:{1:00}", (object) (float) ((double) Mathf.Floor(toConvert) % 60.0), (object) Mathf.Floor((float) ((double) toConvert * 100.0 % 100.0)));
            case 3:
              return string.Format("{0:00}:{1:000}", (object) (float) ((double) Mathf.Floor(toConvert) % 60.0), (object) Mathf.Floor((float) ((double) toConvert * 1000.0 % 1000.0)));
            case 4:
              return string.Format("{0:#00}:{1:000}", (object) (float) ((double) Mathf.Floor(toConvert) % 60.0), (object) Mathf.Floor((float) ((double) toConvert * 1000.0 % 1000.0)));
            case 5:
              return string.Format("{0:#0}:{1:00}", (object) Mathf.Floor(toConvert / 60f), (object) (float) ((double) Mathf.Floor(toConvert) % 60.0));
            case 6:
              return string.Format("{0:#00}:{1:00}", (object) Mathf.Floor(toConvert / 60f), (object) (float) ((double) Mathf.Floor(toConvert) % 60.0));
            case 7:
              return string.Format("{0:0}:{1:00}.{2:0}", (object) Mathf.Floor(toConvert / 60f), (object) (float) ((double) Mathf.Floor(toConvert) % 60.0), (object) Mathf.Floor((float) ((double) toConvert * 10.0 % 10.0)));
            case 8:
              return string.Format("{0:#0}:{1:00}.{2:0}", (object) Mathf.Floor(toConvert / 60f), (object) (float) ((double) Mathf.Floor(toConvert) % 60.0), (object) Mathf.Floor((float) ((double) toConvert * 10.0 % 10.0)));
            case 9:
              return string.Format("{0:0}:{1:00}.{2:00}", (object) Mathf.Floor(toConvert / 60f), (object) (float) ((double) Mathf.Floor(toConvert) % 60.0), (object) Mathf.Floor((float) ((double) toConvert * 100.0 % 100.0)));
            case 10:
              return string.Format("{0:#0}:{1:00}.{2:00}", (object) Mathf.Floor(toConvert / 60f), (object) (float) ((double) Mathf.Floor(toConvert) % 60.0), (object) Mathf.Floor((float) ((double) toConvert * 100.0 % 100.0)));
            case 11:
              return string.Format("{0:0}:{1:00}.{2:000}", (object) Mathf.Floor(toConvert / 60f), (object) (float) ((double) Mathf.Floor(toConvert) % 60.0), (object) Mathf.Floor((float) ((double) toConvert * 1000.0 % 1000.0)));
            case 12:
              return string.Format("{0:#0}:{1:00}.{2:000}", (object) Mathf.Floor(toConvert / 60f), (object) (float) ((double) Mathf.Floor(toConvert) % 60.0), (object) Mathf.Floor((float) ((double) toConvert * 1000.0 % 1000.0)));
          }
        }
      }
      return "error";
    }

    private void CleanupOutsideCurHoldPoint()
    {
    }

    private void CleanupAll()
    {
    }

    public void Update()
    {
      this.DelayedInit();
      if (this.BGAudioMode == TNH_BGAudioMode.Default)
        this.FMODController.Tick(Time.deltaTime);
      switch (this.Phase)
      {
        case TNH_Phase.Take:
          this.Update_Take();
          break;
        case TNH_Phase.Hold:
          this.Update_Hold();
          break;
      }
      if (this.Phase == TNH_Phase.Take || this.Phase == TNH_Phase.Hold)
        this.m_timePlaying += Time.deltaTime;
      this.VoiceUpdate();
      this.FMODController.SetMasterVolume(0.25f * GM.CurrentPlayerBody.GlobalHearing);
    }

    public void SetHoldWaveIntensity(int intensity)
    {
      if (this.BGAudioMode != TNH_BGAudioMode.Default)
        return;
      this.FMODController.SetIntParameterByIndex(1, "Intensity", (float) intensity);
    }

    private void TakingBotKill()
    {
      this.m_botKillThreshold += 3f;
      this.m_botKillThreshold = Mathf.Clamp(this.m_botKillThreshold, 0.0f, 10f);
    }

    private void TakingGunShot()
    {
      this.m_fireThreshold += 3f;
      this.m_fireThreshold = Mathf.Clamp(this.m_fireThreshold, 0.0f, 20f);
    }

    private void OnSosigKill(Sosig s)
    {
      if (this.Phase == TNH_Phase.Take)
        this.TakingBotKill();
      bool statOnly = false;
      if (this.Phase == TNH_Phase.Take)
        statOnly = true;
      if (s.GetDiedFromIFF() == GM.CurrentPlayerBody.GetPlayerIFF())
      {
        this.Increment(3, statOnly);
        if (s.GetDiedFromHeadShot())
          this.Increment(4, statOnly);
        Damage.DamageClass diedFromClass = s.GetDiedFromClass();
        Sosig.SosigDeathType diedFromType = s.GetDiedFromType();
        if (diedFromClass == Damage.DamageClass.Melee)
          this.Increment(5, statOnly);
        switch (diedFromType)
        {
          case Sosig.SosigDeathType.JointBreak:
            this.Increment(6, statOnly);
            break;
          case Sosig.SosigDeathType.JointPullApart:
            this.Increment(7, statOnly);
            break;
        }
      }
      s.TickDownToClear(3f);
    }

    private void OnShotFired(FVRFireArm firearm)
    {
      if (this.Phase != TNH_Phase.Take)
        return;
      this.TakingGunShot();
    }

    private void OnBotShotFired(wwBotWurstModernGun gun)
    {
      if (this.Phase != TNH_Phase.Take)
        return;
      this.TakingGunShot();
    }

    private void Update_Take()
    {
      this.UpdatePatrols();
      for (int index = 0; index < this.SupplyPoints.Count; ++index)
        this.SupplyPoints[index].TestVisited();
      this.m_takeMusicIntensityTarget = (double) this.m_botKillThreshold <= 0.5 || (double) this.m_fireThreshold <= 10.0 ? 1f : 2f;
      this.m_takeMusicIntensity = Mathf.MoveTowards(this.m_takeMusicIntensity, this.m_takeMusicIntensityTarget, Time.deltaTime * 0.5f);
      if ((double) this.m_fireThreshold > 0.0)
        this.m_fireThreshold -= Time.deltaTime;
      else if ((double) this.m_botKillThreshold > 0.0)
        this.m_botKillThreshold -= Time.deltaTime * 1f;
      if (this.BGAudioMode == TNH_BGAudioMode.Default)
        this.FMODController.SetIntParameterByIndex(0, "Intensity", this.m_takeMusicIntensity);
      Vector3 vector3 = GM.CurrentPlayerBody.LeftHand.position + GM.CurrentPlayerBody.LeftHand.forward * -0.2f;
      if (this.RadarHand == TNH_RadarHand.Right)
        vector3 = GM.CurrentPlayerBody.RightHand.position + GM.CurrentPlayerBody.RightHand.forward * -0.2f;
      this.TAHReticle.transform.position = vector3;
    }

    private void Update_Hold()
    {
      this.ObjectCleanupInHold();
      Vector3 vector3 = GM.CurrentPlayerBody.LeftHand.position + GM.CurrentPlayerBody.LeftHand.forward * -0.2f;
      if (this.RadarHand == TNH_RadarHand.Right)
        vector3 = GM.CurrentPlayerBody.RightHand.position + GM.CurrentPlayerBody.RightHand.forward * -0.2f;
      this.TAHReticle.transform.position = vector3;
    }

    public void HoldPointStarted(TNH_HoldPoint p) => this.SetPhase(TNH_Phase.Hold);

    public void HoldPointCompleted(TNH_HoldPoint p, bool success)
    {
      ++this.m_level;
      if (this.m_level < this.m_maxLevels)
      {
        this.SetLevel(this.m_level);
        this.SetPhase(TNH_Phase.Take);
      }
      else
        this.SetPhase(TNH_Phase.Completed);
    }

    public void EnqueueLine(TNH_VoiceLineID id)
    {
      if (this.AIVoiceMode == TNH_AIVoiceMode.Disabled)
        return;
      this.QueuedLines.Enqueue(id);
    }

    public void EnqueueEncryptionLine(TNH_EncryptionType t)
    {
      switch (t)
      {
        case TNH_EncryptionType.Static:
          this.EnqueueLine(TNH_VoiceLineID.AI_EncryptionType_0);
          break;
        case TNH_EncryptionType.Hardened:
          this.EnqueueLine(TNH_VoiceLineID.AI_EncryptionType_1);
          break;
        case TNH_EncryptionType.Swarm:
          this.EnqueueLine(TNH_VoiceLineID.AI_EncryptionType_2);
          break;
        case TNH_EncryptionType.Recursive:
          this.EnqueueLine(TNH_VoiceLineID.AI_EncryptionType_3);
          break;
        case TNH_EncryptionType.Stealth:
          this.EnqueueLine(TNH_VoiceLineID.AI_EncryptionType_4);
          break;
        case TNH_EncryptionType.Agile:
          this.EnqueueLine(TNH_VoiceLineID.AI_EncryptionType_5);
          break;
        case TNH_EncryptionType.Regenerative:
          this.EnqueueLine(TNH_VoiceLineID.AI_EncryptionType_6);
          break;
        case TNH_EncryptionType.Polymorphic:
          this.EnqueueLine(TNH_VoiceLineID.AI_EncryptionType_7);
          break;
        case TNH_EncryptionType.Cascading:
          this.EnqueueLine(TNH_VoiceLineID.AI_EncryptionType_8);
          break;
        case TNH_EncryptionType.Orthagonal:
          this.EnqueueLine(TNH_VoiceLineID.AI_EncryptionType_9);
          break;
        case TNH_EncryptionType.Refractive:
          this.EnqueueLine(TNH_VoiceLineID.AI_EncryptionType_10);
          break;
      }
    }

    public void EnqueueTokenLine(int i)
    {
      switch (i)
      {
        case 1:
          this.EnqueueLine(TNH_VoiceLineID.AI_OverrideTokenFound_1);
          break;
        case 2:
          this.EnqueueLine(TNH_VoiceLineID.AI_OverrideTokenFound_2);
          break;
        case 3:
          this.EnqueueLine(TNH_VoiceLineID.AI_OverrideTokenFound_3);
          break;
        case 4:
          this.EnqueueLine(TNH_VoiceLineID.AI_OverrideTokenFound_4);
          break;
        case 5:
          this.EnqueueLine(TNH_VoiceLineID.AI_OverrideTokenFound_5);
          break;
      }
    }

    private void VoiceUpdate()
    {
      if ((double) this.timeTilLineClear >= 0.0)
      {
        this.timeTilLineClear -= Time.deltaTime;
      }
      else
      {
        if (this.QueuedLines.Count <= 0)
          return;
        TNH_VoiceLineID key = this.QueuedLines.Dequeue();
        if (!this.voiceDic_Standard.ContainsKey(key))
          return;
        int index = UnityEngine.Random.Range(0, this.voiceDic_Standard[key].Count);
        AudioClip audioClip = this.voiceDic_Standard[key][index];
        AudioEvent ClipSet = new AudioEvent();
        ClipSet.Clips.Add(audioClip);
        ClipSet.PitchRange = new Vector2(1f, 1f);
        ClipSet.VolumeRange = new Vector2(0.6f, 0.6f);
        this.timeTilLineClear = audioClip.length + 1.2f;
        SM.PlayCoreSoundDelayed(FVRPooledAudioType.UIChirp, ClipSet, this.transform.position, 0.2f);
      }
    }

    public GameObject SpawnObjectConstructor(Transform point)
    {
      GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.Prefab_ObjectConstructor, point.position, point.rotation);
      TNH_ObjectConstructor component = gameObject.GetComponent<TNH_ObjectConstructor>();
      EquipmentPoolDef.PoolEntry poolEntry1 = component.GetPoolEntry(this.m_level, this.C.EquipmentPool, EquipmentPoolDef.PoolEntry.PoolEntryType.Firearm);
      EquipmentPoolDef.PoolEntry poolEntry2 = component.GetPoolEntry(this.m_level, this.C.EquipmentPool, EquipmentPoolDef.PoolEntry.PoolEntryType.Equipment);
      EquipmentPoolDef.PoolEntry poolEntry3 = component.GetPoolEntry(this.m_level, this.C.EquipmentPool, EquipmentPoolDef.PoolEntry.PoolEntryType.Consumable);
      component.SetEntries(this, this.m_level, this.C.EquipmentPool, poolEntry1, poolEntry2, poolEntry3);
      component.SetRequiredPicatinnySightTable(this.C.RequireSightTable);
      component.SetValidErasSets(this.C.ValidAmmoEras, this.C.ValidAmmoSets);
      return gameObject;
    }

    public void RegenerateConstructor(TNH_ObjectConstructor ObjectConstructor, int which)
    {
      switch (which)
      {
        case 0:
          ObjectConstructor.ResetEntry(this.m_level, this.C.EquipmentPool, EquipmentPoolDef.PoolEntry.PoolEntryType.Firearm, which);
          break;
        case 1:
          ObjectConstructor.ResetEntry(this.m_level, this.C.EquipmentPool, EquipmentPoolDef.PoolEntry.PoolEntryType.Equipment, which);
          break;
        case 2:
          ObjectConstructor.ResetEntry(this.m_level, this.C.EquipmentPool, EquipmentPoolDef.PoolEntry.PoolEntryType.Consumable, which);
          break;
      }
    }

    public GameObject SpawnAmmoReloader(Transform point)
    {
      GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.Prefab_AmmoReloader, point.position, point.rotation);
      TNH_AmmoReloader component = gameObject.GetComponent<TNH_AmmoReloader>();
      component.SetValidErasSets(this.C.ValidAmmoEras, this.C.ValidAmmoSets);
      component.M = this;
      return gameObject;
    }

    public GameObject SpawnMagDuplicator(Transform point)
    {
      GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.Prefab_MagDuplicator, point.position, point.rotation);
      gameObject.GetComponent<TNH_MagDuplicator>().M = this;
      return gameObject;
    }

    public GameObject SpawnGunRecycler(Transform point)
    {
      GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.Prefab_GunRecycler, point.position, point.rotation);
      gameObject.GetComponent<TNH_GunRecycler>().M = this;
      return gameObject;
    }

    public GameObject SpawnWeaponCase(
      GameObject caseFab,
      Vector3 position,
      Vector3 forward,
      FVRObject weapon,
      int numMag,
      int numRound,
      int minAmmo,
      int maxAmmo,
      FVRObject ammoObjOverride = null)
    {
      GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(caseFab, position, Quaternion.LookRotation(forward, Vector3.up));
      this.m_weaponCases.Add(gameObject);
      TNH_WeaponCrate component = gameObject.GetComponent<TNH_WeaponCrate>();
      FVRObject magazineClipSpeedLoaderRound = !((UnityEngine.Object) ammoObjOverride == (UnityEngine.Object) null) ? ammoObjOverride : weapon.GetRandomAmmoObject(weapon, this.C.ValidAmmoEras, minAmmo, maxAmmo, this.C.ValidAmmoSets);
      int numClipSpeedLoaderRound = !((UnityEngine.Object) magazineClipSpeedLoaderRound != (UnityEngine.Object) null) || magazineClipSpeedLoaderRound.Category != FVRObject.ObjectCategory.Cartridge ? numMag : numRound;
      FVRObject requiredAttachment_A = (FVRObject) null;
      FVRObject requiredAttachment_B = (FVRObject) null;
      if (weapon.RequiresPicatinnySight)
      {
        requiredAttachment_A = this.GetObjectTable(this.C.RequireSightTable).GetRandomObject();
        if (requiredAttachment_A.RequiredSecondaryPieces.Count > 0)
          requiredAttachment_B = requiredAttachment_A.RequiredSecondaryPieces[0];
      }
      if (weapon.RequiredSecondaryPieces.Count > 0)
        requiredAttachment_B = weapon.RequiredSecondaryPieces[0];
      component.PlaceWeaponInContainer(weapon, requiredAttachment_A, requiredAttachment_B, magazineClipSpeedLoaderRound, numClipSpeedLoaderRound);
      return gameObject;
    }

    public Sosig SpawnEnemy(
      SosigEnemyTemplate t,
      Transform point,
      int IFF,
      bool IsAssault,
      Vector3 pointOfInterest,
      bool AllowAllWeapons)
    {
      if (this.C.ForceAllAgentWeapons)
        AllowAllWeapons = true;
      GameObject weaponPrefab = (GameObject) null;
      if (t.WeaponOptions.Count > 0)
        weaponPrefab = t.WeaponOptions[UnityEngine.Random.Range(0, t.WeaponOptions.Count)].GetGameObject();
      GameObject weaponPrefab2 = (GameObject) null;
      if (t.WeaponOptions_Secondary.Count > 0 && AllowAllWeapons && (double) UnityEngine.Random.Range(0.0f, 1f) <= (double) t.SecondaryChance)
        weaponPrefab2 = t.WeaponOptions_Secondary[UnityEngine.Random.Range(0, t.WeaponOptions_Secondary.Count)].GetGameObject();
      GameObject weaponPrefab3 = (GameObject) null;
      if (t.WeaponOptions_Tertiary.Count > 0 && AllowAllWeapons && (double) UnityEngine.Random.Range(0.0f, 1f) <= (double) t.TertiaryChance)
        weaponPrefab3 = t.WeaponOptions_Tertiary[UnityEngine.Random.Range(0, t.WeaponOptions_Tertiary.Count)].GetGameObject();
      SosigConfigTemplate configTemplate = t.ConfigTemplates[UnityEngine.Random.Range(0, t.ConfigTemplates.Count)];
      if (this.AI_Difficulty == TNHModifier_AIDifficulty.Arcade && t.ConfigTemplates_Easy.Count > 0)
        configTemplate = t.ConfigTemplates_Easy[UnityEngine.Random.Range(0, t.ConfigTemplates.Count)];
      return this.SpawnEnemySosig(t.SosigPrefabs[UnityEngine.Random.Range(0, t.SosigPrefabs.Count)].GetGameObject(), weaponPrefab, weaponPrefab2, weaponPrefab3, point.position, point.rotation, configTemplate, t.OutfitConfig[UnityEngine.Random.Range(0, t.OutfitConfig.Count)], IFF, IsAssault, pointOfInterest);
    }

    private Sosig SpawnEnemySosig(
      GameObject prefab,
      GameObject weaponPrefab,
      GameObject weaponPrefab2,
      GameObject weaponPrefab3,
      Vector3 pos,
      Quaternion rot,
      SosigConfigTemplate t,
      SosigOutfitConfig o,
      int IFF,
      bool IsAssault,
      Vector3 pointOfInterest)
    {
      Sosig componentInChildren = UnityEngine.Object.Instantiate<GameObject>(prefab, pos, rot).GetComponentInChildren<Sosig>();
      componentInChildren.Configure(t);
      componentInChildren.E.IFFCode = IFF;
      if ((UnityEngine.Object) weaponPrefab != (UnityEngine.Object) null)
      {
        SosigWeapon component1 = UnityEngine.Object.Instantiate<GameObject>(weaponPrefab, pos + Vector3.up * 0.1f, rot).GetComponent<SosigWeapon>();
        component1.SetAutoDestroy(true);
        component1.O.SpawnLockable = false;
        if (component1.Type == SosigWeapon.SosigWeaponType.Gun)
          componentInChildren.Inventory.FillAmmoWithType(component1.AmmoType);
        componentInChildren.Inventory.Init();
        componentInChildren.Inventory.FillAllAmmo();
        if ((UnityEngine.Object) component1 != (UnityEngine.Object) null)
        {
          componentInChildren.InitHands();
          componentInChildren.ForceEquip(component1);
          component1.SetAmmoClamping(true);
          if (this.AI_Difficulty == TNHModifier_AIDifficulty.Arcade)
            component1.FlightVelocityMultiplier = 0.3f;
        }
        if ((UnityEngine.Object) weaponPrefab2 != (UnityEngine.Object) null)
        {
          SosigWeapon component2 = UnityEngine.Object.Instantiate<GameObject>(weaponPrefab2, pos + Vector3.up * 0.1f, rot).GetComponent<SosigWeapon>();
          component2.SetAutoDestroy(true);
          component2.O.SpawnLockable = false;
          component2.SetAmmoClamping(true);
          if (component2.Type == SosigWeapon.SosigWeaponType.Gun)
            componentInChildren.Inventory.FillAmmoWithType(component2.AmmoType);
          if ((UnityEngine.Object) component2 != (UnityEngine.Object) null)
            componentInChildren.ForceEquip(component2);
          if (this.AI_Difficulty == TNHModifier_AIDifficulty.Arcade)
            component2.FlightVelocityMultiplier = 0.3f;
        }
        if ((UnityEngine.Object) weaponPrefab3 != (UnityEngine.Object) null)
        {
          SosigWeapon component2 = UnityEngine.Object.Instantiate<GameObject>(weaponPrefab3, pos + Vector3.up * 0.1f, rot).GetComponent<SosigWeapon>();
          component2.SetAutoDestroy(true);
          component2.O.SpawnLockable = false;
          component2.SetAmmoClamping(true);
          if (component2.Type == SosigWeapon.SosigWeaponType.Gun)
            componentInChildren.Inventory.FillAmmoWithType(component2.AmmoType);
          if ((UnityEngine.Object) component2 != (UnityEngine.Object) null)
            componentInChildren.ForceEquip(component2);
          if (this.AI_Difficulty == TNHModifier_AIDifficulty.Arcade)
            component2.FlightVelocityMultiplier = 0.3f;
        }
      }
      if ((double) UnityEngine.Random.Range(0.0f, 1f) < (double) o.Chance_Headwear)
        this.SpawnAccesoryToLink(o.Headwear, componentInChildren.Links[0]);
      if ((double) UnityEngine.Random.Range(0.0f, 1f) < (double) o.Chance_Facewear)
        this.SpawnAccesoryToLink(o.Facewear, componentInChildren.Links[0]);
      if ((double) UnityEngine.Random.Range(0.0f, 1f) < (double) o.Chance_Eyewear)
        this.SpawnAccesoryToLink(o.Eyewear, componentInChildren.Links[0]);
      if ((double) UnityEngine.Random.Range(0.0f, 1f) < (double) o.Chance_Torsowear)
        this.SpawnAccesoryToLink(o.Torsowear, componentInChildren.Links[1]);
      if ((double) UnityEngine.Random.Range(0.0f, 1f) < (double) o.Chance_Pantswear)
        this.SpawnAccesoryToLink(o.Pantswear, componentInChildren.Links[2]);
      if ((double) UnityEngine.Random.Range(0.0f, 1f) < (double) o.Chance_Pantswear_Lower)
        this.SpawnAccesoryToLink(o.Pantswear_Lower, componentInChildren.Links[3]);
      if ((double) UnityEngine.Random.Range(0.0f, 1f) < (double) o.Chance_Backpacks)
        this.SpawnAccesoryToLink(o.Backpacks, componentInChildren.Links[1]);
      if (t.UsesLinkSpawns)
      {
        for (int index = 0; index < componentInChildren.Links.Count; ++index)
        {
          float num = UnityEngine.Random.Range(0.0f, 1f);
          if (t.LinkSpawns.Count > index && (UnityEngine.Object) t.LinkSpawns[index] != (UnityEngine.Object) null && (t.LinkSpawns[index].Category != FVRObject.ObjectCategory.Loot && (double) num < (double) t.LinkSpawnChance[index]))
            componentInChildren.Links[index].RegisterSpawnOnDestroy(t.LinkSpawns[index]);
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
        float num = UnityEngine.Random.Range(0.0f, 1f);
        bool flag = false;
        if ((double) num > 0.25)
          flag = true;
        componentInChildren.CommandGuardPoint(pointOfInterest, true);
        componentInChildren.SetDominantGuardDirection(UnityEngine.Random.onUnitSphere);
      }
      componentInChildren.SetGuardInvestigateDistanceThreshold(25f);
      return componentInChildren;
    }

    private void SpawnAccesoryToLink(List<FVRObject> gs, SosigLink l)
    {
      GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(gs[UnityEngine.Random.Range(0, gs.Count)].GetGameObject(), l.transform.position, l.transform.rotation);
      gameObject.transform.SetParent(l.transform);
      gameObject.GetComponent<SosigWearable>().RegisterWearable(l);
    }

    private void GenerateValidPatrol(
      TNH_PatrolChallenge P,
      int curStandardIndex,
      int excludeHoldIndex,
      bool isStart)
    {
      if (P.Patrols.Count < 1)
        return;
      TNH_PatrolChallenge.Patrol patrol = P.Patrols[UnityEngine.Random.Range(0, P.Patrols.Count)];
      List<int> ts = new List<int>();
      float num1 = this.TAHReticle.Range * 1.2f;
      if (isStart)
      {
        for (int index = 0; index < this.SafePosMatrix.Entries_SupplyPoints[curStandardIndex].SafePositions_HoldPoints.Count; ++index)
        {
          if (index != excludeHoldIndex && this.SafePosMatrix.Entries_SupplyPoints[curStandardIndex].SafePositions_HoldPoints[index] && (double) Vector3.Distance(GM.CurrentPlayerBody.transform.position, this.HoldPoints[index].transform.position) > (double) num1)
            ts.Add(index);
        }
      }
      else
      {
        for (int index = 0; index < this.SafePosMatrix.Entries_HoldPoints[curStandardIndex].SafePositions_HoldPoints.Count; ++index)
        {
          if (index != excludeHoldIndex && this.SafePosMatrix.Entries_HoldPoints[curStandardIndex].SafePositions_HoldPoints[index] && (double) Vector3.Distance(GM.CurrentPlayerBody.transform.position, this.HoldPoints[index].transform.position) > (double) num1)
            ts.Add(index);
        }
      }
      ts.Shuffle<int>();
      ts.Shuffle<int>();
      if (ts.Count < 1)
        return;
      int PatrolSize = patrol.PatrolSize;
      if (PatrolSize != 0)
      {
        int num2 = patrol.PatrolSize - 1;
        int num3 = patrol.PatrolSize + 2;
        int num4 = UnityEngine.Random.Range(patrol.PatrolSize - 1, patrol.PatrolSize + 2);
        int max = 6;
        if (this.EquipmentMode == TNHSetting_EquipmentMode.LimitedAmmo)
          num3 = 3;
        PatrolSize = Mathf.Clamp(num4, 1, max);
      }
      this.m_patrolSquads.Add(this.GeneratePatrol(patrol.LType, patrol.EType, PatrolSize, ts[0], patrol.IFFUsed));
      if (this.EquipmentMode == TNHSetting_EquipmentMode.Spawnlocking)
      {
        this.m_timeTilPatrolCanSpawn = patrol.TimeTilRegen;
      }
      else
      {
        if (this.EquipmentMode != TNHSetting_EquipmentMode.LimitedAmmo)
          return;
        this.m_timeTilPatrolCanSpawn = patrol.TimeTilRegen_LimitedAmmo;
      }
    }

    private TNH_Manager.SosigPatrolSquad GeneratePatrol(
      SosigEnemyID Type_Leader,
      SosigEnemyID Type_Regular,
      int PatrolSize,
      int HoldPointStart,
      int iff)
    {
      TNH_Manager.SosigPatrolSquad sosigPatrolSquad = new TNH_Manager.SosigPatrolSquad();
      List<int> intList = new List<int>();
      intList.Add(HoldPointStart);
      int num1 = 0;
      int num2 = 0;
      while (num1 < 5)
      {
        int num3 = UnityEngine.Random.Range(0, this.HoldPoints.Count);
        if (!intList.Contains(num3))
        {
          intList.Add(num3);
          ++num1;
        }
        ++num2;
        if (num2 > 200)
          break;
      }
      sosigPatrolSquad.PatrolPoints = new List<Vector3>();
      for (int index = 0; index < intList.Count; ++index)
        sosigPatrolSquad.PatrolPoints.Add(this.HoldPoints[intList[index]].SpawnPoints_Sosigs_Defense[UnityEngine.Random.Range(0, this.HoldPoints[intList[index]].SpawnPoints_Sosigs_Defense.Count)].position);
      this.HoldPoints[HoldPointStart].SpawnPoints_Sosigs_Defense.Shuffle<Transform>();
      for (int index = 0; index < PatrolSize; ++index)
      {
        Transform point = this.HoldPoints[HoldPointStart].SpawnPoints_Sosigs_Defense[index];
        SosigEnemyTemplate t;
        bool AllowAllWeapons;
        if (index == 0)
        {
          t = ManagerSingleton<IM>.Instance.odicSosigObjsByID[Type_Leader];
          AllowAllWeapons = true;
        }
        else
        {
          t = ManagerSingleton<IM>.Instance.odicSosigObjsByID[Type_Regular];
          AllowAllWeapons = false;
        }
        Sosig sosig = this.SpawnEnemy(t, point, iff, true, sosigPatrolSquad.PatrolPoints[0], AllowAllWeapons);
        float num3 = UnityEngine.Random.Range(0.0f, 1f);
        if (index == 0 && (double) num3 > 0.649999976158142)
          sosig.Links[1].RegisterSpawnOnDestroy(this.Prefab_HealthPickupMinor);
        sosig.SetAssaultSpeed(Sosig.SosigMoveSpeed.Walking);
        sosigPatrolSquad.Squad.Add(sosig);
      }
      return sosigPatrolSquad;
    }

    private void UpdatePatrols()
    {
      if ((double) this.m_timeTilPatrolCanSpawn > 0.0)
        this.m_timeTilPatrolCanSpawn -= Time.deltaTime;
      TNH_PatrolChallenge patrolChallenge = this.m_curLevel.PatrolChallenge;
      int num = patrolChallenge.Patrols[0].MaxPatrols;
      if (this.EquipmentMode == TNHSetting_EquipmentMode.LimitedAmmo)
        num = patrolChallenge.Patrols[0].MaxPatrols_LimitedAmmo;
      if ((double) this.m_timeTilPatrolCanSpawn <= 0.0 && this.m_patrolSquads.Count < patrolChallenge.Patrols[0].MaxPatrols)
      {
        int curStandardIndex = -1;
        Vector3 position = GM.CurrentPlayerBody.Head.position;
        for (int index = 0; index < this.SupplyPoints.Count; ++index)
        {
          if (this.SupplyPoints[index].IsPointInBounds(position))
          {
            curStandardIndex = index;
            break;
          }
        }
        if (curStandardIndex > -1)
          this.GenerateValidPatrol(patrolChallenge, curStandardIndex, this.m_curHoldIndex, true);
        else
          this.m_timeTilPatrolCanSpawn = 6f;
      }
      for (int index1 = 0; index1 < this.m_patrolSquads.Count; ++index1)
      {
        TNH_Manager.SosigPatrolSquad patrolSquad = this.m_patrolSquads[index1];
        if (patrolSquad.Squad.Count > 0)
        {
          for (int index2 = patrolSquad.Squad.Count - 1; index2 >= 0; --index2)
          {
            if ((UnityEngine.Object) patrolSquad.Squad[index2] == (UnityEngine.Object) null)
              patrolSquad.Squad.RemoveAt(index2);
          }
          bool flag = true;
          for (int index2 = 0; index2 < patrolSquad.Squad.Count; ++index2)
          {
            if ((UnityEngine.Object) patrolSquad.Squad[index2] != (UnityEngine.Object) null && (double) Vector3.Distance(patrolSquad.Squad[index2].transform.position, patrolSquad.PatrolPoints[patrolSquad.CurPatrolPointIndex]) > 4.0)
              flag = false;
          }
          if (flag)
          {
            if (patrolSquad.CurPatrolPointIndex + 1 >= patrolSquad.PatrolPoints.Count && patrolSquad.IsPatrollingUp)
              patrolSquad.IsPatrollingUp = false;
            if (patrolSquad.CurPatrolPointIndex == 0)
              patrolSquad.IsPatrollingUp = true;
            if (patrolSquad.IsPatrollingUp)
              ++patrolSquad.CurPatrolPointIndex;
            else
              --patrolSquad.CurPatrolPointIndex;
            for (int index2 = 0; index2 < patrolSquad.Squad.Count; ++index2)
            {
              if ((UnityEngine.Object) patrolSquad.Squad[index2] != (UnityEngine.Object) null)
                patrolSquad.Squad[index2].CommandAssaultPoint(patrolSquad.PatrolPoints[patrolSquad.CurPatrolPointIndex]);
            }
          }
          for (int index2 = 0; index2 < patrolSquad.Squad.Count; ++index2)
          {
            if ((UnityEngine.Object) patrolSquad.Squad[index2] != (UnityEngine.Object) null)
            {
              if (patrolSquad.Squad[index2].CurrentOrder == Sosig.SosigOrder.Wander)
                patrolSquad.Squad[index2].CurrentOrder = Sosig.SosigOrder.Assault;
              patrolSquad.Squad[index2].FallbackOrder = Sosig.SosigOrder.Assault;
            }
          }
        }
      }
      if (this.m_patrolSquads.Count < 1)
        return;
      for (int index = this.m_patrolSquads.Count - 1; index >= 0; --index)
      {
        if (this.m_patrolSquads[index].Squad.Count < 1)
        {
          this.m_patrolSquads[index].PatrolPoints.Clear();
          this.m_patrolSquads.RemoveAt(index);
        }
      }
    }

    private void KillAllPatrols()
    {
      if (this.m_patrolSquads.Count < 1)
        return;
      for (int index1 = this.m_patrolSquads.Count - 1; index1 >= 0; --index1)
      {
        if (this.m_patrolSquads[index1].Squad.Count > 0)
        {
          for (int index2 = 0; index2 < this.m_patrolSquads[index1].Squad.Count; ++index2)
          {
            if ((UnityEngine.Object) this.m_patrolSquads[index1].Squad[index2] != (UnityEngine.Object) null)
              this.m_patrolSquads[index1].Squad[index2].ClearSosig();
          }
        }
        this.m_patrolSquads[index1].Squad.Clear();
        this.m_patrolSquads[index1].PatrolPoints.Clear();
      }
      this.m_patrolSquads.Clear();
    }

    private void DeleteAllPatrols()
    {
      for (int index1 = this.m_patrolSquads.Count - 1; index1 >= 0; --index1)
      {
        if (this.m_patrolSquads[index1].Squad.Count > 0)
        {
          for (int index2 = this.m_patrolSquads[index1].Squad.Count - 1; index2 >= 0; --index2)
          {
            if ((UnityEngine.Object) this.m_patrolSquads[index1].Squad[index2] == (UnityEngine.Object) null)
              this.m_patrolSquads[index1].Squad.RemoveAt(index2);
            else if ((UnityEngine.Object) this.m_patrolSquads[index1].Squad[index2] != (UnityEngine.Object) null)
              this.m_patrolSquads[index1].Squad[index2].DeSpawnSosig();
          }
        }
      }
      this.m_patrolSquads.Clear();
    }

    public void AddObjectToTrackedList(GameObject g)
    {
      FVRPhysicalObject component = g.GetComponent<FVRPhysicalObject>();
      if (!((UnityEngine.Object) component != (UnityEngine.Object) null))
        return;
      this.AddFVRObjectToTrackedList(component);
    }

    public void AddFVRObjectToTrackedList(FVRPhysicalObject g)
    {
      if (!this.m_knownObjsHash.Add(g))
        return;
      if (g is FVRFireArm)
        this.Increment(11, false);
      this.m_knownObjs.Add(g);
    }

    private void ObjectCleanupInHold()
    {
      if (this.m_knownObjs.Count <= 0)
        return;
      ++this.knownObjectCheckIndex;
      if (this.knownObjectCheckIndex >= this.m_knownObjs.Count)
        this.knownObjectCheckIndex = 0;
      if ((UnityEngine.Object) this.m_knownObjs[this.knownObjectCheckIndex] == (UnityEngine.Object) null)
      {
        this.m_knownObjsHash.Remove(this.m_knownObjs[this.knownObjectCheckIndex]);
        this.m_knownObjs.RemoveAt(this.knownObjectCheckIndex);
      }
      else
      {
        Vector3 position = this.m_knownObjs[this.knownObjectCheckIndex].transform.position;
        if (this.m_curHoldPoint.IsPointInBounds(position) || (double) Vector3.Distance(position, GM.CurrentPlayerBody.transform.position) <= 10.0)
          return;
        this.m_knownObjsHash.Remove(this.m_knownObjs[this.knownObjectCheckIndex]);
        UnityEngine.Object.Destroy((UnityEngine.Object) this.m_knownObjs[this.knownObjectCheckIndex].gameObject);
        this.m_knownObjs.RemoveAt(this.knownObjectCheckIndex);
      }
    }

    private void InitLibraries()
    {
      for (int index = 0; index < this.ResourceLib.EncryptionObjects.Count; ++index)
        this.dic_encryption.Add((TNH_EncryptionType) index, this.ResourceLib.EncryptionObjects[index]);
      for (int index = 0; index < this.ResourceLib.TurretObjects.Count; ++index)
        this.dic_turret.Add((TNH_TurretType) index, this.ResourceLib.TurretObjects[index]);
      for (int index = 0; index < this.ResourceLib.TrapObjects.Count; ++index)
        this.dic_trap.Add((TNH_TrapType) index, this.ResourceLib.TrapObjects[index]);
      for (int index = 0; index < this.VoiceDB.Lines.Count; ++index)
      {
        if (this.voiceDic_Standard.ContainsKey(this.VoiceDB.Lines[index].ID))
          this.voiceDic_Standard[this.VoiceDB.Lines[index].ID].Add(this.VoiceDB.Lines[index].Clip_Standard);
        else
          this.voiceDic_Standard.Add(this.VoiceDB.Lines[index].ID, new List<AudioClip>()
          {
            this.VoiceDB.Lines[index].Clip_Standard
          });
        if (this.voiceDic_Corrupted.ContainsKey(this.VoiceDB.Lines[index].ID))
          this.voiceDic_Corrupted[this.VoiceDB.Lines[index].ID].Add(this.VoiceDB.Lines[index].Clip_Corrupted);
        else
          this.voiceDic_Corrupted.Add(this.VoiceDB.Lines[index].ID, new List<AudioClip>()
          {
            this.VoiceDB.Lines[index].Clip_Corrupted
          });
      }
    }

    public FVRObject GetEncryptionPrefab(TNH_EncryptionType t) => this.dic_encryption[t];

    public FVRObject GetTurretPrefab(TNH_TurretType t) => this.dic_turret[t];

    public FVRObject GetTrapPrefab(TNH_TrapType t) => this.dic_trap[t];

    private void PrimeNums()
    {
    }

    public void Increment(int i, bool statOnly)
    {
      int num1 = this.Stats[i] + 1;
      this.Stats[i] = num1;
      if (statOnly)
        return;
      int num2 = this.Nums[i] + 1;
      this.Nums[i] = num2;
    }

    public void Increment(int i, int amount, bool statOnly)
    {
      int num1 = this.Stats[i] + amount;
      this.Stats[i] = num1;
      if (statOnly)
        return;
      int num2 = this.Nums[i] + amount;
      this.Nums[i] = num2;
    }

    public int ReturnNum() => 0 + this.Nums[0] * 200 + this.Nums[1] * -20 + this.Nums[2] + this.Nums[3] * 2 + this.Nums[4] + this.Nums[5] + this.Nums[6] + this.Nums[7] + this.Nums[8] * 50 + this.Nums[9] * 20 + this.Nums[10] * 5 + this.Nums[11];

    [Serializable]
    public class SosigPatrolSquad
    {
      public List<Sosig> Squad = new List<Sosig>();
      public List<Vector3> PatrolPoints = new List<Vector3>();
      public int CurPatrolPointIndex;
      public bool IsPatrollingUp = true;
    }

    public delegate void TokenCountChange(int numTokens);
  }
}
