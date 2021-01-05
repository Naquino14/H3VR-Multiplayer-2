// Decompiled with JetBrains decompiler
// Type: FistVR.TNH_HoldPoint
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class TNH_HoldPoint : MonoBehaviour
  {
    public TNH_Manager M;
    public TNH_TakeChallenge T;
    public TNH_HoldChallenge H;
    public List<Transform> Bounds;
    public GameObject NavBlockers;
    public List<TNH_DestructibleBarrierPoint> BarrierPoints;
    public List<AICoverPoint> CoverPoints;
    private bool m_isInHold;
    private TNH_HoldPoint.HoldState m_state;
    private int m_phaseIndex;
    private int m_maxPhases;
    private int m_tokenReward;
    private TNH_HoldChallenge.Phase m_curPhase;
    private TNH_HoldPointSystemNode m_systemNode;
    private float m_tickDownIntro;
    private float m_tickDownToNextGroupSpawn;
    private float m_tickDownToIdentification;
    private float m_tickDownToFailure;
    private float m_tickDownTransition;
    private List<GameObject> m_warpInTargets = new List<GameObject>();
    private List<TNH_EncryptionTarget> m_activeTargets = new List<TNH_EncryptionTarget>();
    private List<Sosig> m_activeSosigs = new List<Sosig>();
    private List<AutoMeater> m_activeTurrets = new List<AutoMeater>();
    [Header("References To Important Nodes")]
    public Transform SpawnPoint_SystemNode;
    public List<Transform> SpawnPoints_Targets;
    public List<Transform> SpawnPoints_Turrets;
    public List<TNH_HoldPoint.AttackVector> AttackVectors;
    public List<Transform> SpawnPoints_Sosigs_Defense;
    [Header("Debug")]
    public bool ShowPoint_SystemNode;
    public bool ShowPoints_Targets;
    public bool ShowPoints_Turrets;
    public bool ShowPoints_Sosigs_Attack;
    public bool ShowPoints_Sosigs_Defense;
    public Mesh GizmoMesh_SosigAttack;
    public AudioEvent AUDEvent_HoldWave;
    public AudioEvent AUDEvent_Success;
    public AudioEvent AUDEvent_Failure;
    public GameObject VFX_HoldWave;
    public bool TestingModeActivate;
    private bool m_hasPlayedTimeWarning1;
    private bool m_hasPlayedTimeWarning2;
    private int m_numWarnings;
    private bool m_isFirstWave = true;
    private bool m_hasThrownNadesInWave;
    private int m_numTargsToSpawn;

    public void Init()
    {
      GM.CurrentSceneSettings.SosigKillEvent += new FVRSceneSettings.SosigKill(this.CheckIfDeadSosigWasMine);
      for (int index = 0; index < this.BarrierPoints.Count; ++index)
        this.BarrierPoints[index].Init();
    }

    private void OnDestroy() => GM.CurrentSceneSettings.SosigKillEvent -= new FVRSceneSettings.SosigKill(this.CheckIfDeadSosigWasMine);

    public void ConfigureAsSystemNode(TNH_TakeChallenge t, TNH_HoldChallenge h, int reward)
    {
      this.T = t;
      this.H = h;
      this.SpawnTakeChallengeEntities(t);
      this.SpawnSystemNode();
      this.m_phaseIndex = 0;
      this.m_maxPhases = h.Phases.Count;
      this.m_tokenReward = reward;
    }

    private void SpawnTakeChallengeEntities(TNH_TakeChallenge t)
    {
      this.SpawnTakeEnemyGroup();
      this.SpawnTurrets();
    }

    private void SpawnSystemNode()
    {
      this.m_systemNode = UnityEngine.Object.Instantiate<GameObject>(this.M.Prefab_SystemNode, this.SpawnPoint_SystemNode.position, this.SpawnPoint_SystemNode.rotation).GetComponent<TNH_HoldPointSystemNode>();
      this.m_systemNode.HoldPoint = this;
    }

    public void BeginHoldChallenge()
    {
      this.DeletionBurst();
      this.DeleteAllActiveEntities();
      this.NavBlockers.SetActive(true);
      this.m_phaseIndex = 0;
      this.m_maxPhases = this.H.Phases.Count;
      this.M.EnqueueLine(TNH_VoiceLineID.BASE_IntrusionDetectedInitiatingLockdown);
      this.M.EnqueueLine(TNH_VoiceLineID.AI_InterfacingWithSystemNode);
      this.M.EnqueueLine(TNH_VoiceLineID.BASE_ResponseTeamEnRoute);
      this.m_isInHold = true;
      this.m_numWarnings = 0;
      this.M.HoldPointStarted(this);
      this.BeginPhase();
    }

    public void Update()
    {
      if (this.TestingModeActivate && Input.GetKeyDown(KeyCode.R))
      {
        this.ForceClearConfiguration();
        this.ConfigureAsSystemNode(this.T, this.H, 3);
        this.BeginHoldChallenge();
      }
      if (!this.m_isInHold)
        return;
      switch (this.m_state)
      {
        case TNH_HoldPoint.HoldState.Beginning:
          this.m_tickDownIntro -= Time.deltaTime;
          if ((double) this.m_tickDownIntro <= 0.0)
            this.BeginAnalyzing();
          this.m_systemNode.SetDisplayString("SCANNING SYSTEM");
          break;
        case TNH_HoldPoint.HoldState.Analyzing:
          this.m_tickDownToIdentification -= Time.deltaTime;
          if ((double) this.m_tickDownToIdentification <= 0.0)
            this.IdentifyEncryption();
          this.m_systemNode.SetDisplayString("ANALYZING");
          break;
        case TNH_HoldPoint.HoldState.Hacking:
          this.m_tickDownToFailure -= Time.deltaTime;
          if (!this.m_hasPlayedTimeWarning1 && (double) this.m_tickDownToFailure < 60.0)
          {
            this.m_hasPlayedTimeWarning1 = true;
            this.M.EnqueueLine(TNH_VoiceLineID.AI_Encryption_Reminder1);
            this.M.Increment(1, false);
          }
          if (!this.m_hasPlayedTimeWarning2 && (double) this.m_tickDownToFailure < 30.0)
          {
            this.m_hasPlayedTimeWarning2 = true;
            this.M.EnqueueLine(TNH_VoiceLineID.AI_Encryption_Reminder2);
            ++this.m_numWarnings;
            this.M.Increment(1, false);
          }
          this.m_systemNode.SetDisplayString("FAILURE IN: " + this.FloatToTime(this.m_tickDownToFailure, "0:00.00"));
          if ((double) this.m_tickDownToFailure <= 0.0)
          {
            this.FailOut();
            break;
          }
          break;
        case TNH_HoldPoint.HoldState.Transition:
          this.m_tickDownTransition -= Time.deltaTime;
          if ((double) this.m_tickDownTransition <= 0.0)
          {
            if (this.m_phaseIndex < this.m_maxPhases)
            {
              this.M.EnqueueLine(TNH_VoiceLineID.AI_AdvancingToNextSystemLayer);
              this.BeginPhase();
            }
            else
              this.CompleteHold();
          }
          if ((UnityEngine.Object) this.m_systemNode != (UnityEngine.Object) null)
          {
            this.m_systemNode.SetDisplayString("SCANNING SYSTEM");
            break;
          }
          break;
      }
      if (this.m_state == TNH_HoldPoint.HoldState.Transition)
        return;
      this.SpawningRoutineUpdate();
    }

    private void BeginPhase()
    {
      this.m_state = TNH_HoldPoint.HoldState.Beginning;
      this.m_isFirstWave = true;
      this.m_activeTargets.Clear();
      this.m_tickDownIntro = 5f;
      this.m_tickDownTransition = 5f;
      this.m_curPhase = this.H.Phases[this.m_phaseIndex];
      if (this.m_phaseIndex >= this.m_maxPhases - 1 && this.m_maxPhases > 1)
        this.M.SetHoldWaveIntensity(2);
      else
        this.M.SetHoldWaveIntensity(1);
      this.m_tickDownToNextGroupSpawn = this.m_curPhase.WarmUp * UnityEngine.Random.Range(0.8f, 1.1f);
      this.m_hasPlayedTimeWarning1 = false;
      this.m_hasPlayedTimeWarning2 = false;
      this.RefreshCoverInHold();
      this.m_systemNode.SetNodeMode(TNH_HoldPointSystemNode.SystemNodeMode.Hacking);
    }

    private void BeginAnalyzing()
    {
      this.M.EnqueueLine(TNH_VoiceLineID.AI_AnalyzingSystem);
      this.m_state = TNH_HoldPoint.HoldState.Analyzing;
      this.m_tickDownToIdentification = UnityEngine.Random.Range(this.m_curPhase.ScanTime * 0.8f, this.m_curPhase.ScanTime * 1.2f);
      this.SpawnPoints_Targets.Shuffle<Transform>();
      this.SpawnWarpInMarkers();
      this.m_systemNode.SetNodeMode(TNH_HoldPointSystemNode.SystemNodeMode.Analyzing);
    }

    private void IdentifyEncryption()
    {
      this.m_state = TNH_HoldPoint.HoldState.Hacking;
      this.m_tickDownToFailure = 120f;
      this.M.EnqueueEncryptionLine(this.m_curPhase.Encryption);
      this.SpawnTargetGroup();
      this.m_systemNode.SetNodeMode(TNH_HoldPointSystemNode.SystemNodeMode.Indentified);
    }

    private void CompletePhase()
    {
      this.DeletionBurst();
      this.M.ClearMiscEnemies();
      SM.PlayCoreSound(FVRPooledAudioType.GenericLongRange, this.AUDEvent_HoldWave, this.transform.position);
      UnityEngine.Object.Instantiate<GameObject>(this.VFX_HoldWave, this.m_systemNode.NodeCenter.position, this.m_systemNode.NodeCenter.rotation);
      this.M.EnqueueLine(TNH_VoiceLineID.AI_Encryption_Neutralized);
      this.M.Increment(2, (int) this.m_tickDownToFailure, false);
      ++this.m_phaseIndex;
      this.m_state = TNH_HoldPoint.HoldState.Transition;
      this.m_tickDownTransition = 5f;
      this.LowerAllBarriers();
      this.m_systemNode.SetNodeMode(TNH_HoldPointSystemNode.SystemNodeMode.Hacking);
    }

    private void SpawningRoutineUpdate()
    {
      this.m_tickDownToNextGroupSpawn -= Time.deltaTime;
      if (this.m_activeSosigs.Count < 1 && this.m_state == TNH_HoldPoint.HoldState.Analyzing)
        this.m_tickDownToNextGroupSpawn -= Time.deltaTime;
      if (!this.m_hasThrownNadesInWave && (double) this.m_tickDownToNextGroupSpawn <= 5.0)
      {
        this.m_hasThrownNadesInWave = true;
        this.AttackVectors.Shuffle<TNH_HoldPoint.AttackVector>();
        this.AttackVectors.Shuffle<TNH_HoldPoint.AttackVector>();
      }
      if ((double) this.m_tickDownToNextGroupSpawn > 0.0)
        return;
      this.SpawnHoldEnemyGroup();
      this.m_hasThrownNadesInWave = false;
      float num = 1f;
      if (this.M.EquipmentMode == TNHSetting_EquipmentMode.LimitedAmmo)
        num = 1.35f;
      this.m_tickDownToNextGroupSpawn = this.m_curPhase.SpawnCadence * UnityEngine.Random.Range(0.9f, 1.1f) * num;
    }

    private void DeletionBurst()
    {
      this.KillSosigs();
      this.M.ClearMiscEnemies();
    }

    private void RefreshCoverInHold()
    {
      int howMany = Mathf.CeilToInt((float) this.BarrierPoints.Count * 0.6f) - this.GetNumActiveBarriers();
      if (howMany <= 0)
        return;
      this.RaiseRandomBarriers(howMany);
    }

    private void RaiseRandomBarriers(int howMany)
    {
      int num = howMany;
      this.BarrierPoints.Shuffle<TNH_DestructibleBarrierPoint>();
      for (int index = 0; index < this.BarrierPoints.Count; ++index)
      {
        if (this.BarrierPoints[index].SpawnRandomBarrier())
          --num;
        if (num <= 0)
          break;
      }
    }

    private void LowerAllBarriers()
    {
      for (int index = 0; index < this.BarrierPoints.Count; ++index)
        this.BarrierPoints[index].LowerBarrierThenDestroy();
    }

    private int GetNumActiveBarriers()
    {
      int num = 0;
      for (int index = 0; index < this.BarrierPoints.Count; ++index)
      {
        if (this.BarrierPoints[index].IsBarrierActive())
          ++num;
      }
      return num;
    }

    private void SpawnTurrets()
    {
      FVRObject turretPrefab = this.M.GetTurretPrefab(this.T.TurretType);
      int num = Mathf.Clamp(UnityEngine.Random.Range(this.T.NumTurrets - 1, this.T.NumTurrets + 1), 0, 5);
      for (int index = 0; index < num; ++index)
      {
        Vector3 position = this.SpawnPoints_Turrets[index].position + Vector3.up * 0.25f;
        this.m_activeTurrets.Add(UnityEngine.Object.Instantiate<GameObject>(turretPrefab.GetGameObject(), position, this.SpawnPoints_Turrets[index].rotation).GetComponent<AutoMeater>());
      }
    }

    private void SpawnTakeEnemyGroup()
    {
      this.SpawnPoints_Sosigs_Defense.Shuffle<Transform>();
      this.SpawnPoints_Sosigs_Defense.Shuffle<Transform>();
      Mathf.Clamp(UnityEngine.Random.Range(this.T.NumGuards - 1, this.T.NumGuards + 1), 0, 9);
      for (int index = 0; index < this.T.NumGuards; ++index)
      {
        Transform point = this.SpawnPoints_Sosigs_Defense[index];
        this.m_activeSosigs.Add(this.M.SpawnEnemy(ManagerSingleton<IM>.Instance.odicSosigObjsByID[this.T.GID], point, this.T.IFFUsed, false, point.position, true));
      }
    }

    private void SpawnHoldEnemyGroup()
    {
      int count = this.m_activeSosigs.Count;
      int maxEnemiesAlive = this.m_curPhase.MaxEnemiesAlive;
      if (this.M.EquipmentMode == TNHSetting_EquipmentMode.LimitedAmmo)
        Mathf.Clamp(maxEnemiesAlive, maxEnemiesAlive, 4);
      int max = this.m_curPhase.MaxEnemiesAlive - count;
      if (max <= 0)
        return;
      int num1 = Mathf.Clamp(UnityEngine.Random.Range(this.m_curPhase.MinEnemies, this.m_curPhase.MaxEnemies + 1), 0, max);
      if (this.M.EquipmentMode == TNHSetting_EquipmentMode.LimitedAmmo && num1 > 2)
        --num1;
      Mathf.CeilToInt((float) num1 / 3f);
      int maxDirections = this.m_curPhase.MaxDirections;
      int num2 = UnityEngine.Random.Range(Mathf.Clamp(maxDirections, 0, this.AttackVectors.Count), Mathf.Clamp(maxDirections, 0, this.AttackVectors.Count) + 1);
      int num3 = Mathf.Clamp(num1, 0, num2 * 3);
      int num4 = 0;
      int index1 = 0;
      SosigEnemyTemplate t = ManagerSingleton<IM>.Instance.odicSosigObjsByID[this.m_curPhase.LType];
      while (num4 < num3)
      {
        for (int index2 = 0; index2 < num2; ++index2)
        {
          Transform point = this.AttackVectors[index2].SpawnPoints_Sosigs_Attack[index1];
          if (num4 > 0)
            t = ManagerSingleton<IM>.Instance.odicSosigObjsByID[this.m_curPhase.EType];
          bool AllowAllWeapons = true;
          if (index2 > 0)
            AllowAllWeapons = false;
          this.m_activeSosigs.Add(this.M.SpawnEnemy(t, point, this.m_curPhase.IFFUsed, true, this.SpawnPoints_Turrets[UnityEngine.Random.Range(0, this.SpawnPoints_Turrets.Count)].position, AllowAllWeapons));
          ++num4;
        }
        ++index1;
      }
      this.m_isFirstWave = false;
    }

    private void SpawnWarpInMarkers()
    {
      this.m_numTargsToSpawn = UnityEngine.Random.Range(this.m_curPhase.MinTargets, this.m_curPhase.MaxTargets + 1);
      this.SpawnPoints_Targets.Shuffle<Transform>();
      for (int index = 0; index < this.m_numTargsToSpawn; ++index)
        this.m_warpInTargets.Add(UnityEngine.Object.Instantiate<GameObject>(this.M.Prefab_TargetWarpingIn, this.SpawnPoints_Targets[index].position, this.SpawnPoints_Targets[index].rotation));
    }

    private void SpawnTargetGroup()
    {
      this.DeleteAllActiveWarpIns();
      FVRObject encryptionPrefab = this.M.GetEncryptionPrefab(this.m_curPhase.Encryption);
      if (this.M.EquipmentMode == TNHSetting_EquipmentMode.LimitedAmmo)
        this.m_numTargsToSpawn = this.m_curPhase.Encryption == TNH_EncryptionType.Static ? Mathf.Clamp(this.m_numTargsToSpawn, 1, 3) : 1;
      if (this.m_curPhase.Encryption == TNH_EncryptionType.Stealth)
        this.SpawnPoints_Targets.Shuffle<Transform>();
      for (int index = 0; index < this.m_numTargsToSpawn; ++index)
      {
        TNH_EncryptionTarget component = UnityEngine.Object.Instantiate<GameObject>(encryptionPrefab.GetGameObject(), this.SpawnPoints_Targets[index].position, this.SpawnPoints_Targets[index].rotation).GetComponent<TNH_EncryptionTarget>();
        component.SetHoldPoint(this);
        this.RegisterNewTarget(component);
      }
    }

    public void RegisterNewTarget(TNH_EncryptionTarget t) => this.m_activeTargets.Add(t);

    public void TargetDestroyed(TNH_EncryptionTarget t)
    {
      this.m_activeTargets.Remove(t);
      if (this.m_activeTargets.Count > 0)
        return;
      this.CompletePhase();
    }

    public void ShutDownHoldPoint()
    {
      this.m_isInHold = false;
      this.m_state = TNH_HoldPoint.HoldState.Beginning;
      this.NavBlockers.SetActive(false);
      this.m_phaseIndex = 0;
      this.m_maxPhases = 0;
      this.m_curPhase = (TNH_HoldChallenge.Phase) null;
      this.DeleteSystemNode();
      this.DeleteAllActiveTargets();
      this.DeleteSosigs();
      this.DeleteTurrets();
      this.LowerAllBarriers();
    }

    public void ForceClearConfiguration()
    {
      this.m_isInHold = false;
      this.m_state = TNH_HoldPoint.HoldState.Beginning;
      this.NavBlockers.SetActive(false);
      this.m_phaseIndex = 0;
      this.m_maxPhases = 0;
      this.m_curPhase = (TNH_HoldChallenge.Phase) null;
      this.DeleteSystemNode();
      this.DeleteAllActiveEntities();
    }

    public void DeleteAllActiveEntities()
    {
      this.DeleteAllActiveTargets();
      this.DeleteBarriers();
      this.DeleteSosigs();
      this.DeleteTurrets();
    }

    private void DeleteSystemNode()
    {
      if ((UnityEngine.Object) this.m_systemNode != (UnityEngine.Object) null)
        UnityEngine.Object.Destroy((UnityEngine.Object) this.m_systemNode.gameObject);
      this.m_systemNode = (TNH_HoldPointSystemNode) null;
    }

    private void DeleteBarriers()
    {
      for (int index = 0; index < this.BarrierPoints.Count; ++index)
        this.BarrierPoints[index].DeleteBarrier();
    }

    private void DeleteSosigs()
    {
      for (int index = this.m_activeSosigs.Count - 1; index >= 0; --index)
        this.m_activeSosigs[index].DeSpawnSosig();
      this.m_activeSosigs.Clear();
    }

    private void KillSosigs()
    {
      for (int index = this.m_activeSosigs.Count - 1; index >= 0; --index)
        this.m_activeSosigs[index].ClearSosig();
      this.m_activeSosigs.Clear();
    }

    private void DeleteAllActiveWarpIns()
    {
      for (int index = this.m_warpInTargets.Count - 1; index >= 0; --index)
      {
        if ((UnityEngine.Object) this.m_warpInTargets[index] != (UnityEngine.Object) null)
          UnityEngine.Object.Destroy((UnityEngine.Object) this.m_warpInTargets[index]);
      }
    }

    private void DeleteAllActiveTargets()
    {
      for (int index = this.m_activeTargets.Count - 1; index >= 0; --index)
      {
        if ((UnityEngine.Object) this.m_activeTargets[index] != (UnityEngine.Object) null)
          UnityEngine.Object.Destroy((UnityEngine.Object) this.m_activeTargets[index].gameObject);
      }
      this.m_activeTargets.Clear();
      for (int index = this.m_warpInTargets.Count - 1; index >= 0; --index)
      {
        if ((UnityEngine.Object) this.m_warpInTargets[index] != (UnityEngine.Object) null)
          UnityEngine.Object.Destroy((UnityEngine.Object) this.m_warpInTargets[index]);
      }
    }

    private void DeleteTurrets()
    {
      for (int index = this.m_activeTurrets.Count - 1; index >= 0; --index)
      {
        if ((UnityEngine.Object) this.m_activeTurrets[index] != (UnityEngine.Object) null)
          UnityEngine.Object.Destroy((UnityEngine.Object) this.m_activeTurrets[index].gameObject);
      }
      this.m_activeTurrets.Clear();
    }

    public void CheckIfDeadSosigWasMine(Sosig s)
    {
      if (!this.m_activeSosigs.Contains(s))
        return;
      s.TickDownToClear(3f);
      this.m_activeSosigs.Remove(s);
    }

    private void CompleteHold()
    {
      this.m_isInHold = false;
      this.M.Increment(0, false);
      SM.PlayCoreSound(FVRPooledAudioType.GenericLongRange, this.AUDEvent_Success, this.transform.position);
      this.M.EnqueueLine(TNH_VoiceLineID.AI_HoldSuccessfulDataExtracted);
      int tokenReward = this.m_tokenReward;
      if (this.m_numWarnings > 6)
        --tokenReward;
      else if (this.m_numWarnings > 3)
        --tokenReward;
      if (tokenReward > 0)
      {
        this.M.EnqueueTokenLine(tokenReward);
        this.M.AddTokens(tokenReward, true);
      }
      this.m_tokenReward = 0;
      this.M.EnqueueLine(TNH_VoiceLineID.AI_AdvanceToNextSystemNodeAndTakeIt);
      this.M.HoldPointCompleted(this, true);
      this.ShutDownHoldPoint();
    }

    private void FailOut()
    {
      this.m_isInHold = false;
      this.M.EnqueueLine(TNH_VoiceLineID.AI_HoldFailedNodeConnectionTerminated);
      SM.PlayCoreSound(FVRPooledAudioType.GenericLongRange, this.AUDEvent_Failure, this.transform.position);
      this.M.HoldPointCompleted(this, false);
      this.ShutDownHoldPoint();
    }

    public bool IsPointInBounds(Vector3 p)
    {
      for (int index = 0; index < this.Bounds.Count; ++index)
      {
        if (this.TestVolumeBool(this.Bounds[index], p))
          return true;
      }
      return false;
    }

    public bool TestVolumeBool(Transform t, Vector3 pos)
    {
      bool flag = true;
      Vector3 vector3 = t.InverseTransformPoint(pos);
      if ((double) Mathf.Abs(vector3.x) > 0.5 || (double) Mathf.Abs(vector3.y) > 0.5 || (double) Mathf.Abs(vector3.z) > 0.5)
        flag = false;
      return flag;
    }

    private void OnDrawGizmos()
    {
      if (this.ShowPoint_SystemNode)
      {
        Gizmos.color = new Color(1f, 1f, 1f);
        Gizmos.DrawSphere(this.SpawnPoint_SystemNode.position + Vector3.up, 0.5f);
        Gizmos.DrawWireSphere(this.SpawnPoint_SystemNode.position + Vector3.up, 0.5f);
      }
      if (this.ShowPoints_Targets)
      {
        for (int index = 0; index < this.SpawnPoints_Targets.Count; ++index)
        {
          Gizmos.color = new Color(1f, 0.2f, 0.2f);
          Gizmos.DrawSphere(this.SpawnPoints_Targets[index].position, 0.5f);
          Gizmos.DrawWireSphere(this.SpawnPoints_Targets[index].position, 0.5f);
        }
      }
      if (this.ShowPoints_Sosigs_Attack)
      {
        for (int index1 = 0; index1 < this.AttackVectors.Count; ++index1)
        {
          Gizmos.color = new Color(0.8f, 0.0f, 0.5f);
          Gizmos.DrawSphere(this.AttackVectors[index1].GrenadeVector.position, 0.15f);
          Gizmos.DrawLine(this.AttackVectors[index1].GrenadeVector.position, this.AttackVectors[index1].GrenadeVector.position + this.AttackVectors[index1].GrenadeVector.forward);
          for (int index2 = 0; index2 < this.AttackVectors[index1].SpawnPoints_Sosigs_Attack.Count; ++index2)
          {
            Gizmos.color = new Color(0.8f, 0.0f, (float) index1 * 0.1f + 0.5f);
            Gizmos.DrawMesh(this.GizmoMesh_SosigAttack, this.AttackVectors[index1].SpawnPoints_Sosigs_Attack[index2].position, this.AttackVectors[index1].SpawnPoints_Sosigs_Attack[index2].rotation);
          }
        }
      }
      if (this.ShowPoints_Sosigs_Defense)
      {
        for (int index = 0; index < this.SpawnPoints_Sosigs_Defense.Count; ++index)
        {
          Gizmos.color = new Color(0.0f, 0.8f, 0.8f);
          Gizmos.DrawMesh(this.GizmoMesh_SosigAttack, this.SpawnPoints_Sosigs_Defense[index].position, this.SpawnPoints_Sosigs_Defense[index].rotation);
        }
      }
      if (!this.ShowPoints_Turrets)
        return;
      for (int index = 0; index < this.SpawnPoints_Turrets.Count; ++index)
      {
        Gizmos.color = new Color(0.0f, 0.2f, 1f);
        Gizmos.DrawMesh(this.GizmoMesh_SosigAttack, this.SpawnPoints_Turrets[index].position, this.SpawnPoints_Turrets[index].rotation);
      }
    }

    [ContextMenu("PrimeAttackVectors")]
    public void PrimeAttackVectors()
    {
      for (int index = 0; index < this.AttackVectors.Count; ++index)
      {
        if ((UnityEngine.Object) this.AttackVectors[index].GrenadeVector == (UnityEngine.Object) null)
        {
          GameObject gameObject = new GameObject("GrenadeThrow_" + index.ToString());
          this.AttackVectors[index].GrenadeVector = gameObject.transform;
        }
        this.AttackVectors[index].GrenadeVector.SetParent(this.AttackVectors[index].SpawnPoints_Sosigs_Attack[0].parent);
        this.AttackVectors[index].GrenadeVector.position = this.AttackVectors[index].SpawnPoints_Sosigs_Attack[0].position + Vector3.up;
        this.AttackVectors[index].GrenadeVector.rotation = this.AttackVectors[index].SpawnPoints_Sosigs_Attack[0].rotation;
      }
    }

    public string FloatToTime(float toConvert, string format)
    {
      if (format != null)
      {
        // ISSUE: reference to a compiler-generated field
        if (TNH_HoldPoint.\u003C\u003Ef__switch\u0024map7 == null)
        {
          // ISSUE: reference to a compiler-generated field
          TNH_HoldPoint.\u003C\u003Ef__switch\u0024map7 = new Dictionary<string, int>(13)
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
        if (TNH_HoldPoint.\u003C\u003Ef__switch\u0024map7.TryGetValue(format, out num))
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

    public enum HoldState
    {
      Beginning,
      Analyzing,
      Hacking,
      Transition,
      Ending,
    }

    [Serializable]
    public class AttackVector
    {
      public List<Transform> SpawnPoints_Sosigs_Attack;
      public Transform GrenadeVector;
      public float GrenadeRandAngle = 30f;
      public Vector2 GrenadeVelRange = new Vector2(3f, 8f);
    }
  }
}
