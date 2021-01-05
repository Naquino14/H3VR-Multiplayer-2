using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class TNH_HoldPoint : MonoBehaviour
	{
		public enum HoldState
		{
			Beginning,
			Analyzing,
			Hacking,
			Transition,
			Ending
		}

		[Serializable]
		public class AttackVector
		{
			public List<Transform> SpawnPoints_Sosigs_Attack;

			public Transform GrenadeVector;

			public float GrenadeRandAngle = 30f;

			public Vector2 GrenadeVelRange = new Vector2(3f, 8f);
		}

		public TNH_Manager M;

		public TNH_TakeChallenge T;

		public TNH_HoldChallenge H;

		public List<Transform> Bounds;

		public GameObject NavBlockers;

		public List<TNH_DestructibleBarrierPoint> BarrierPoints;

		public List<AICoverPoint> CoverPoints;

		private bool m_isInHold;

		private HoldState m_state;

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

		public List<AttackVector> AttackVectors;

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
			GM.CurrentSceneSettings.SosigKillEvent += CheckIfDeadSosigWasMine;
			for (int i = 0; i < BarrierPoints.Count; i++)
			{
				BarrierPoints[i].Init();
			}
		}

		private void OnDestroy()
		{
			GM.CurrentSceneSettings.SosigKillEvent -= CheckIfDeadSosigWasMine;
		}

		public void ConfigureAsSystemNode(TNH_TakeChallenge t, TNH_HoldChallenge h, int reward)
		{
			T = t;
			H = h;
			SpawnTakeChallengeEntities(t);
			SpawnSystemNode();
			m_phaseIndex = 0;
			m_maxPhases = h.Phases.Count;
			m_tokenReward = reward;
		}

		private void SpawnTakeChallengeEntities(TNH_TakeChallenge t)
		{
			SpawnTakeEnemyGroup();
			SpawnTurrets();
		}

		private void SpawnSystemNode()
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(M.Prefab_SystemNode, SpawnPoint_SystemNode.position, SpawnPoint_SystemNode.rotation);
			m_systemNode = gameObject.GetComponent<TNH_HoldPointSystemNode>();
			m_systemNode.HoldPoint = this;
		}

		public void BeginHoldChallenge()
		{
			DeletionBurst();
			DeleteAllActiveEntities();
			NavBlockers.SetActive(value: true);
			m_phaseIndex = 0;
			m_maxPhases = H.Phases.Count;
			M.EnqueueLine(TNH_VoiceLineID.BASE_IntrusionDetectedInitiatingLockdown);
			M.EnqueueLine(TNH_VoiceLineID.AI_InterfacingWithSystemNode);
			M.EnqueueLine(TNH_VoiceLineID.BASE_ResponseTeamEnRoute);
			m_isInHold = true;
			m_numWarnings = 0;
			M.HoldPointStarted(this);
			BeginPhase();
		}

		public void Update()
		{
			if (TestingModeActivate && Input.GetKeyDown(KeyCode.R))
			{
				ForceClearConfiguration();
				ConfigureAsSystemNode(T, H, 3);
				BeginHoldChallenge();
			}
			if (!m_isInHold)
			{
				return;
			}
			switch (m_state)
			{
			case HoldState.Beginning:
				m_tickDownIntro -= Time.deltaTime;
				if (m_tickDownIntro <= 0f)
				{
					BeginAnalyzing();
				}
				m_systemNode.SetDisplayString("SCANNING SYSTEM");
				break;
			case HoldState.Analyzing:
				m_tickDownToIdentification -= Time.deltaTime;
				if (m_tickDownToIdentification <= 0f)
				{
					IdentifyEncryption();
				}
				m_systemNode.SetDisplayString("ANALYZING");
				break;
			case HoldState.Hacking:
				m_tickDownToFailure -= Time.deltaTime;
				if (!m_hasPlayedTimeWarning1 && m_tickDownToFailure < 60f)
				{
					m_hasPlayedTimeWarning1 = true;
					M.EnqueueLine(TNH_VoiceLineID.AI_Encryption_Reminder1);
					M.Increment(1, statOnly: false);
				}
				if (!m_hasPlayedTimeWarning2 && m_tickDownToFailure < 30f)
				{
					m_hasPlayedTimeWarning2 = true;
					M.EnqueueLine(TNH_VoiceLineID.AI_Encryption_Reminder2);
					m_numWarnings++;
					M.Increment(1, statOnly: false);
				}
				m_systemNode.SetDisplayString("FAILURE IN: " + FloatToTime(m_tickDownToFailure, "0:00.00"));
				if (m_tickDownToFailure <= 0f)
				{
					FailOut();
				}
				break;
			case HoldState.Transition:
				m_tickDownTransition -= Time.deltaTime;
				if (m_tickDownTransition <= 0f)
				{
					if (m_phaseIndex < m_maxPhases)
					{
						M.EnqueueLine(TNH_VoiceLineID.AI_AdvancingToNextSystemLayer);
						BeginPhase();
					}
					else
					{
						CompleteHold();
					}
				}
				if (m_systemNode != null)
				{
					m_systemNode.SetDisplayString("SCANNING SYSTEM");
				}
				break;
			}
			if (m_state != HoldState.Transition)
			{
				SpawningRoutineUpdate();
			}
		}

		private void BeginPhase()
		{
			m_state = HoldState.Beginning;
			m_isFirstWave = true;
			m_activeTargets.Clear();
			m_tickDownIntro = 5f;
			m_tickDownTransition = 5f;
			m_curPhase = H.Phases[m_phaseIndex];
			if (m_phaseIndex >= m_maxPhases - 1 && m_maxPhases > 1)
			{
				M.SetHoldWaveIntensity(2);
			}
			else
			{
				M.SetHoldWaveIntensity(1);
			}
			m_tickDownToNextGroupSpawn = m_curPhase.WarmUp * UnityEngine.Random.Range(0.8f, 1.1f);
			m_hasPlayedTimeWarning1 = false;
			m_hasPlayedTimeWarning2 = false;
			RefreshCoverInHold();
			m_systemNode.SetNodeMode(TNH_HoldPointSystemNode.SystemNodeMode.Hacking);
		}

		private void BeginAnalyzing()
		{
			M.EnqueueLine(TNH_VoiceLineID.AI_AnalyzingSystem);
			m_state = HoldState.Analyzing;
			m_tickDownToIdentification = UnityEngine.Random.Range(m_curPhase.ScanTime * 0.8f, m_curPhase.ScanTime * 1.2f);
			SpawnPoints_Targets.Shuffle();
			SpawnWarpInMarkers();
			m_systemNode.SetNodeMode(TNH_HoldPointSystemNode.SystemNodeMode.Analyzing);
		}

		private void IdentifyEncryption()
		{
			m_state = HoldState.Hacking;
			m_tickDownToFailure = 120f;
			M.EnqueueEncryptionLine(m_curPhase.Encryption);
			SpawnTargetGroup();
			m_systemNode.SetNodeMode(TNH_HoldPointSystemNode.SystemNodeMode.Indentified);
		}

		private void CompletePhase()
		{
			DeletionBurst();
			M.ClearMiscEnemies();
			SM.PlayCoreSound(FVRPooledAudioType.GenericLongRange, AUDEvent_HoldWave, base.transform.position);
			UnityEngine.Object.Instantiate(VFX_HoldWave, m_systemNode.NodeCenter.position, m_systemNode.NodeCenter.rotation);
			M.EnqueueLine(TNH_VoiceLineID.AI_Encryption_Neutralized);
			M.Increment(2, (int)m_tickDownToFailure, statOnly: false);
			m_phaseIndex++;
			m_state = HoldState.Transition;
			m_tickDownTransition = 5f;
			LowerAllBarriers();
			m_systemNode.SetNodeMode(TNH_HoldPointSystemNode.SystemNodeMode.Hacking);
		}

		private void SpawningRoutineUpdate()
		{
			m_tickDownToNextGroupSpawn -= Time.deltaTime;
			if (m_activeSosigs.Count < 1 && m_state == HoldState.Analyzing)
			{
				m_tickDownToNextGroupSpawn -= Time.deltaTime;
			}
			if (!m_hasThrownNadesInWave && m_tickDownToNextGroupSpawn <= 5f)
			{
				m_hasThrownNadesInWave = true;
				AttackVectors.Shuffle();
				AttackVectors.Shuffle();
			}
			if (m_tickDownToNextGroupSpawn <= 0f)
			{
				SpawnHoldEnemyGroup();
				m_hasThrownNadesInWave = false;
				float num = 1f;
				if (M.EquipmentMode == TNHSetting_EquipmentMode.LimitedAmmo)
				{
					num = 1.35f;
				}
				m_tickDownToNextGroupSpawn = m_curPhase.SpawnCadence * UnityEngine.Random.Range(0.9f, 1.1f) * num;
			}
		}

		private void DeletionBurst()
		{
			KillSosigs();
			M.ClearMiscEnemies();
		}

		private void RefreshCoverInHold()
		{
			int num = Mathf.CeilToInt((float)BarrierPoints.Count * 0.6f);
			int numActiveBarriers = GetNumActiveBarriers();
			int num2 = num - numActiveBarriers;
			if (num2 > 0)
			{
				RaiseRandomBarriers(num2);
			}
		}

		private void RaiseRandomBarriers(int howMany)
		{
			int num = howMany;
			BarrierPoints.Shuffle();
			for (int i = 0; i < BarrierPoints.Count; i++)
			{
				if (BarrierPoints[i].SpawnRandomBarrier())
				{
					num--;
				}
				if (num <= 0)
				{
					break;
				}
			}
		}

		private void LowerAllBarriers()
		{
			for (int i = 0; i < BarrierPoints.Count; i++)
			{
				BarrierPoints[i].LowerBarrierThenDestroy();
			}
		}

		private int GetNumActiveBarriers()
		{
			int num = 0;
			for (int i = 0; i < BarrierPoints.Count; i++)
			{
				if (BarrierPoints[i].IsBarrierActive())
				{
					num++;
				}
			}
			return num;
		}

		private void SpawnTurrets()
		{
			TNH_TurretType turretType = T.TurretType;
			FVRObject turretPrefab = M.GetTurretPrefab(turretType);
			int value = UnityEngine.Random.Range(T.NumTurrets - 1, T.NumTurrets + 1);
			value = Mathf.Clamp(value, 0, 5);
			for (int i = 0; i < value; i++)
			{
				Vector3 position = SpawnPoints_Turrets[i].position + Vector3.up * 0.25f;
				GameObject gameObject = UnityEngine.Object.Instantiate(turretPrefab.GetGameObject(), position, SpawnPoints_Turrets[i].rotation);
				m_activeTurrets.Add(gameObject.GetComponent<AutoMeater>());
			}
		}

		private void SpawnTakeEnemyGroup()
		{
			SpawnPoints_Sosigs_Defense.Shuffle();
			SpawnPoints_Sosigs_Defense.Shuffle();
			int value = UnityEngine.Random.Range(T.NumGuards - 1, T.NumGuards + 1);
			value = Mathf.Clamp(value, 0, 9);
			for (int i = 0; i < T.NumGuards; i++)
			{
				Transform transform = SpawnPoints_Sosigs_Defense[i];
				SosigEnemyTemplate t = ManagerSingleton<IM>.Instance.odicSosigObjsByID[T.GID];
				Sosig item = M.SpawnEnemy(t, transform, T.IFFUsed, IsAssault: false, transform.position, AllowAllWeapons: true);
				m_activeSosigs.Add(item);
			}
		}

		private void SpawnHoldEnemyGroup()
		{
			int count = m_activeSosigs.Count;
			int maxEnemiesAlive = m_curPhase.MaxEnemiesAlive;
			if (M.EquipmentMode == TNHSetting_EquipmentMode.LimitedAmmo)
			{
				maxEnemiesAlive = Mathf.Clamp(maxEnemiesAlive, maxEnemiesAlive, 4);
			}
			int num = m_curPhase.MaxEnemiesAlive - count;
			if (num <= 0)
			{
				return;
			}
			int value = UnityEngine.Random.Range(m_curPhase.MinEnemies, m_curPhase.MaxEnemies + 1);
			value = Mathf.Clamp(value, 0, num);
			if (M.EquipmentMode == TNHSetting_EquipmentMode.LimitedAmmo && value > 2)
			{
				value--;
			}
			int num2 = Mathf.CeilToInt((float)value / 3f);
			int maxDirections = m_curPhase.MaxDirections;
			num2 = Mathf.Clamp(maxDirections, 0, AttackVectors.Count);
			maxDirections = Mathf.Clamp(maxDirections, 0, AttackVectors.Count);
			int num3 = UnityEngine.Random.Range(num2, maxDirections + 1);
			value = Mathf.Clamp(value, 0, num3 * 3);
			int num4 = 0;
			int num5 = 0;
			SosigEnemyTemplate t = ManagerSingleton<IM>.Instance.odicSosigObjsByID[m_curPhase.LType];
			while (num4 < value)
			{
				for (int i = 0; i < num3; i++)
				{
					Transform point = AttackVectors[i].SpawnPoints_Sosigs_Attack[num5];
					if (num4 > 0)
					{
						t = ManagerSingleton<IM>.Instance.odicSosigObjsByID[m_curPhase.EType];
					}
					bool allowAllWeapons = true;
					if (i > 0)
					{
						allowAllWeapons = false;
					}
					Sosig item = M.SpawnEnemy(t, point, m_curPhase.IFFUsed, IsAssault: true, SpawnPoints_Turrets[UnityEngine.Random.Range(0, SpawnPoints_Turrets.Count)].position, allowAllWeapons);
					m_activeSosigs.Add(item);
					num4++;
				}
				num5++;
			}
			m_isFirstWave = false;
		}

		private void SpawnWarpInMarkers()
		{
			m_numTargsToSpawn = UnityEngine.Random.Range(m_curPhase.MinTargets, m_curPhase.MaxTargets + 1);
			SpawnPoints_Targets.Shuffle();
			for (int i = 0; i < m_numTargsToSpawn; i++)
			{
				GameObject item = UnityEngine.Object.Instantiate(M.Prefab_TargetWarpingIn, SpawnPoints_Targets[i].position, SpawnPoints_Targets[i].rotation);
				m_warpInTargets.Add(item);
			}
		}

		private void SpawnTargetGroup()
		{
			DeleteAllActiveWarpIns();
			FVRObject encryptionPrefab = M.GetEncryptionPrefab(m_curPhase.Encryption);
			if (M.EquipmentMode == TNHSetting_EquipmentMode.LimitedAmmo)
			{
				if (m_curPhase.Encryption != 0)
				{
					m_numTargsToSpawn = 1;
				}
				else
				{
					m_numTargsToSpawn = Mathf.Clamp(m_numTargsToSpawn, 1, 3);
				}
			}
			if (m_curPhase.Encryption == TNH_EncryptionType.Stealth)
			{
				SpawnPoints_Targets.Shuffle();
			}
			for (int i = 0; i < m_numTargsToSpawn; i++)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(encryptionPrefab.GetGameObject(), SpawnPoints_Targets[i].position, SpawnPoints_Targets[i].rotation);
				TNH_EncryptionTarget component = gameObject.GetComponent<TNH_EncryptionTarget>();
				component.SetHoldPoint(this);
				RegisterNewTarget(component);
			}
		}

		public void RegisterNewTarget(TNH_EncryptionTarget t)
		{
			m_activeTargets.Add(t);
		}

		public void TargetDestroyed(TNH_EncryptionTarget t)
		{
			m_activeTargets.Remove(t);
			if (m_activeTargets.Count <= 0)
			{
				CompletePhase();
			}
		}

		public void ShutDownHoldPoint()
		{
			m_isInHold = false;
			m_state = HoldState.Beginning;
			NavBlockers.SetActive(value: false);
			m_phaseIndex = 0;
			m_maxPhases = 0;
			m_curPhase = null;
			DeleteSystemNode();
			DeleteAllActiveTargets();
			DeleteSosigs();
			DeleteTurrets();
			LowerAllBarriers();
		}

		public void ForceClearConfiguration()
		{
			m_isInHold = false;
			m_state = HoldState.Beginning;
			NavBlockers.SetActive(value: false);
			m_phaseIndex = 0;
			m_maxPhases = 0;
			m_curPhase = null;
			DeleteSystemNode();
			DeleteAllActiveEntities();
		}

		public void DeleteAllActiveEntities()
		{
			DeleteAllActiveTargets();
			DeleteBarriers();
			DeleteSosigs();
			DeleteTurrets();
		}

		private void DeleteSystemNode()
		{
			if (m_systemNode != null)
			{
				UnityEngine.Object.Destroy(m_systemNode.gameObject);
			}
			m_systemNode = null;
		}

		private void DeleteBarriers()
		{
			for (int i = 0; i < BarrierPoints.Count; i++)
			{
				BarrierPoints[i].DeleteBarrier();
			}
		}

		private void DeleteSosigs()
		{
			for (int num = m_activeSosigs.Count - 1; num >= 0; num--)
			{
				m_activeSosigs[num].DeSpawnSosig();
			}
			m_activeSosigs.Clear();
		}

		private void KillSosigs()
		{
			for (int num = m_activeSosigs.Count - 1; num >= 0; num--)
			{
				m_activeSosigs[num].ClearSosig();
			}
			m_activeSosigs.Clear();
		}

		private void DeleteAllActiveWarpIns()
		{
			for (int num = m_warpInTargets.Count - 1; num >= 0; num--)
			{
				if (m_warpInTargets[num] != null)
				{
					UnityEngine.Object.Destroy(m_warpInTargets[num]);
				}
			}
		}

		private void DeleteAllActiveTargets()
		{
			for (int num = m_activeTargets.Count - 1; num >= 0; num--)
			{
				if (m_activeTargets[num] != null)
				{
					UnityEngine.Object.Destroy(m_activeTargets[num].gameObject);
				}
			}
			m_activeTargets.Clear();
			for (int num2 = m_warpInTargets.Count - 1; num2 >= 0; num2--)
			{
				if (m_warpInTargets[num2] != null)
				{
					UnityEngine.Object.Destroy(m_warpInTargets[num2]);
				}
			}
		}

		private void DeleteTurrets()
		{
			for (int num = m_activeTurrets.Count - 1; num >= 0; num--)
			{
				if (m_activeTurrets[num] != null)
				{
					UnityEngine.Object.Destroy(m_activeTurrets[num].gameObject);
				}
			}
			m_activeTurrets.Clear();
		}

		public void CheckIfDeadSosigWasMine(Sosig s)
		{
			if (m_activeSosigs.Contains(s))
			{
				s.TickDownToClear(3f);
				m_activeSosigs.Remove(s);
			}
		}

		private void CompleteHold()
		{
			m_isInHold = false;
			M.Increment(0, statOnly: false);
			SM.PlayCoreSound(FVRPooledAudioType.GenericLongRange, AUDEvent_Success, base.transform.position);
			M.EnqueueLine(TNH_VoiceLineID.AI_HoldSuccessfulDataExtracted);
			int num = m_tokenReward;
			if (m_numWarnings > 6)
			{
				num--;
			}
			else if (m_numWarnings > 3)
			{
				num--;
			}
			if (num > 0)
			{
				M.EnqueueTokenLine(num);
				M.AddTokens(num, Scorethis: true);
			}
			m_tokenReward = 0;
			M.EnqueueLine(TNH_VoiceLineID.AI_AdvanceToNextSystemNodeAndTakeIt);
			M.HoldPointCompleted(this, success: true);
			ShutDownHoldPoint();
		}

		private void FailOut()
		{
			m_isInHold = false;
			M.EnqueueLine(TNH_VoiceLineID.AI_HoldFailedNodeConnectionTerminated);
			SM.PlayCoreSound(FVRPooledAudioType.GenericLongRange, AUDEvent_Failure, base.transform.position);
			M.HoldPointCompleted(this, success: false);
			ShutDownHoldPoint();
		}

		public bool IsPointInBounds(Vector3 p)
		{
			for (int i = 0; i < Bounds.Count; i++)
			{
				if (TestVolumeBool(Bounds[i], p))
				{
					return true;
				}
			}
			return false;
		}

		public bool TestVolumeBool(Transform t, Vector3 pos)
		{
			bool result = true;
			Vector3 vector = t.InverseTransformPoint(pos);
			if (Mathf.Abs(vector.x) > 0.5f || Mathf.Abs(vector.y) > 0.5f || Mathf.Abs(vector.z) > 0.5f)
			{
				result = false;
			}
			return result;
		}

		private void OnDrawGizmos()
		{
			if (ShowPoint_SystemNode)
			{
				Gizmos.color = new Color(1f, 1f, 1f);
				Gizmos.DrawSphere(SpawnPoint_SystemNode.position + Vector3.up, 0.5f);
				Gizmos.DrawWireSphere(SpawnPoint_SystemNode.position + Vector3.up, 0.5f);
			}
			if (ShowPoints_Targets)
			{
				for (int i = 0; i < SpawnPoints_Targets.Count; i++)
				{
					Gizmos.color = new Color(1f, 0.2f, 0.2f);
					Gizmos.DrawSphere(SpawnPoints_Targets[i].position, 0.5f);
					Gizmos.DrawWireSphere(SpawnPoints_Targets[i].position, 0.5f);
				}
			}
			if (ShowPoints_Sosigs_Attack)
			{
				for (int j = 0; j < AttackVectors.Count; j++)
				{
					Gizmos.color = new Color(0.8f, 0f, 0.5f);
					Gizmos.DrawSphere(AttackVectors[j].GrenadeVector.position, 0.15f);
					Gizmos.DrawLine(AttackVectors[j].GrenadeVector.position, AttackVectors[j].GrenadeVector.position + AttackVectors[j].GrenadeVector.forward);
					for (int k = 0; k < AttackVectors[j].SpawnPoints_Sosigs_Attack.Count; k++)
					{
						float num = (float)j * 0.1f;
						Gizmos.color = new Color(0.8f, 0f, num + 0.5f);
						Gizmos.DrawMesh(GizmoMesh_SosigAttack, AttackVectors[j].SpawnPoints_Sosigs_Attack[k].position, AttackVectors[j].SpawnPoints_Sosigs_Attack[k].rotation);
					}
				}
			}
			if (ShowPoints_Sosigs_Defense)
			{
				for (int l = 0; l < SpawnPoints_Sosigs_Defense.Count; l++)
				{
					Gizmos.color = new Color(0f, 0.8f, 0.8f);
					Gizmos.DrawMesh(GizmoMesh_SosigAttack, SpawnPoints_Sosigs_Defense[l].position, SpawnPoints_Sosigs_Defense[l].rotation);
				}
			}
			if (ShowPoints_Turrets)
			{
				for (int m = 0; m < SpawnPoints_Turrets.Count; m++)
				{
					Gizmos.color = new Color(0f, 0.2f, 1f);
					Gizmos.DrawMesh(GizmoMesh_SosigAttack, SpawnPoints_Turrets[m].position, SpawnPoints_Turrets[m].rotation);
				}
			}
		}

		[ContextMenu("PrimeAttackVectors")]
		public void PrimeAttackVectors()
		{
			for (int i = 0; i < AttackVectors.Count; i++)
			{
				if (AttackVectors[i].GrenadeVector == null)
				{
					GameObject gameObject = new GameObject("GrenadeThrow_" + i);
					AttackVectors[i].GrenadeVector = gameObject.transform;
				}
				AttackVectors[i].GrenadeVector.SetParent(AttackVectors[i].SpawnPoints_Sosigs_Attack[0].parent);
				AttackVectors[i].GrenadeVector.position = AttackVectors[i].SpawnPoints_Sosigs_Attack[0].position + Vector3.up;
				AttackVectors[i].GrenadeVector.rotation = AttackVectors[i].SpawnPoints_Sosigs_Attack[0].rotation;
			}
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
