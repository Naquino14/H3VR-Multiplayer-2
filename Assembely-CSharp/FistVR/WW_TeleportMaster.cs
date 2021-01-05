using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
	public class WW_TeleportMaster : MonoBehaviour
	{
		[Serializable]
		public class BunkerCase
		{
			public GameObject Gun;

			public GameObject Ammo;

			public GameObject Extra1;

			public GameObject Extra2;

			public FVRObject GunFO;

			public FVRObject AmmoFO;

			public FVRObject ExtraFO1;

			public FVRObject ExtraFO2;

			public List<int> MessagesByDay;
		}

		[Serializable]
		public class CloseWind : IComparable<CloseWind>
		{
			public WW_WindNode W;

			public float Dist;

			public int CompareTo(CloseWind other)
			{
				if (other == null)
				{
					return 1;
				}
				return Dist.CompareTo(other.Dist);
			}
		}

		public List<WW_Bunker> Bunkers;

		public WW_Panel Panel;

		public List<Transform> TowerPads;

		private bool m_setToRespawnInABunker;

		private int m_currentBunkerSpot;

		public GameObject TeleportInEffect;

		public int CurrentDay;

		public List<int> TiersNeeded;

		public List<BunkerCase> CasesByDay;

		public AudioEvent AudEvent_MessageReceived;

		public List<WW_Checkpoint> Checkpoints;

		public List<Transform> SF;

		public ObjectTableDef TableReqPic;

		public OptionsPanel_ButtonSet OBS_Difficulty;

		public int Difficulty;

		[Header("WhiteOutSystem")]
		public Material SkyboxMat;

		public float WhiteOutThreshold;

		public Vector2 WhiteOutFogVals;

		private Vector2 RefHeights = new Vector2(0f, 60f);

		private Vector2 HorizonHeights = new Vector2(0.45f, 0.25f);

		private Vector2 HorizonBlends = new Vector2(0.25f, 0.5f);

		private bool m_isBossFighting;

		public GameObject Boss;

		public AudioEvent AudEvent_Alarm;

		public Text BossBUttonText;

		public List<WinterEnemySpawnZone> WSpawns;

		public List<EncryptionBotSpawner> ESPawns;

		private bool m_isInABunker;

		private int m_whichBunkerAmIIn = -1;

		private bool m_isSF;

		public GameObject SFSpawn;

		private GameObject m_mySF;

		private float m_SFTick;

		private int m_curCheckPointCheck;

		public Transform PSys_Rig;

		public ParticleSystem PSys_SnowDown;

		public ParticleSystem PSys_SnowBluster;

		public Material MTL_Snowbluster;

		public Color C_Snowbluster_MinIntensity;

		public Color C_Snowbluster_MaxIntensity;

		public float L_Snowbluster;

		public int Cy_Snowbluster = 2;

		public Vector3 windDirMin = new Vector3(2f, 0f, 2f);

		public Vector3 windDirMax = new Vector3(20f, 0f, 20f);

		public Vector3 Wind = Vector3.zero;

		public List<WW_WindNode> WindPoints;

		private int m_windCheckIndex;

		private HashSet<WW_WindNode> m_closestTs = new HashSet<WW_WindNode>();

		private List<CloseWind> m_closestWind = new List<CloseWind>();

		private void Awake()
		{
			Panel.UpdateMessageDisplay();
			OBS_Difficulty.SetSelectedButton(Difficulty);
		}

		public void SetDifficulty(int i)
		{
			Difficulty = i;
			if (i == 0)
			{
				GM.CurrentSceneSettings.DoesDamageGetRegistered = true;
				GM.CurrentPlayerBody.SetHealthThreshold(5000f);
			}
			else
			{
				GM.CurrentSceneSettings.DoesDamageGetRegistered = false;
				GM.CurrentPlayerBody.SetHealthThreshold(10000f);
			}
		}

		public void BeginBossFight()
		{
			if (!m_isBossFighting)
			{
				m_isBossFighting = true;
				for (int i = 0; i < WSpawns.Count; i++)
				{
					WSpawns[i].DespawnAll();
					WSpawns[i].gameObject.SetActive(value: false);
				}
				for (int j = 0; j < ESPawns.Count; j++)
				{
					ESPawns[j].ClearAll();
					ESPawns[j].gameObject.SetActive(value: false);
				}
				SM.PlayCoreSound(FVRPooledAudioType.Generic, AudEvent_Alarm, BossBUttonText.transform.position);
				BossBUttonText.text = "WARNING!!! MASTER DRONE ACTIVE";
				Boss.SetActive(value: true);
			}
		}

		private void UpdateWhiteOut()
		{
			WhiteOutThreshold = Mathf.Clamp(WhiteOutThreshold, 0f, 1f);
			SkyboxMat.SetFloat("_WhiteoutAmount", WhiteOutThreshold);
			RenderSettings.fogDensity = Mathf.Lerp(WhiteOutFogVals.x, WhiteOutFogVals.y, WhiteOutThreshold);
			float t = Mathf.InverseLerp(RefHeights.x, RefHeights.y, GM.CurrentPlayerBody.Head.position.y);
			float value = Mathf.Lerp(0.15f, Mathf.Lerp(HorizonHeights.x, HorizonHeights.y, t), WhiteOutThreshold);
			float value2 = Mathf.Lerp(0.4f, Mathf.Lerp(HorizonBlends.x, HorizonBlends.y, t), WhiteOutThreshold);
			SkyboxMat.SetFloat("_HorizonHeight", value);
			SkyboxMat.SetFloat("_HorizonBlend", value2);
		}

		public void Start()
		{
			for (int i = 0; i < Bunkers.Count; i++)
			{
				Bunkers[i].SetMaster(this);
				Bunkers[i].ConfigInitBunker(i, CurrentDay, TiersNeeded[i]);
				if (i < CasesByDay.Count)
				{
					Bunkers[i].Crate.PlaceWeaponInContainer(CasesByDay[i].Gun, CasesByDay[i].Ammo, CasesByDay[i].Extra1, CasesByDay[i].Extra2);
					Bunkers[i].Crate.PlaceWeaponInContainer(CasesByDay[i].GunFO, CasesByDay[i].ExtraFO1, CasesByDay[i].ExtraFO2, CasesByDay[i].AmmoFO, 3);
				}
				else
				{
					Bunkers[i].Crate.gameObject.SetActive(value: false);
				}
			}
			for (int j = 0; j < 4; j++)
			{
				CloseWind item = new CloseWind();
				m_closestWind.Add(item);
				m_closestWind[j].Dist = 3000f;
				m_closestWind[j].W = WindPoints[0];
			}
			m_closestTs.Add(WindPoints[0]);
			ObjectTable objectTable = new ObjectTable();
			objectTable.Initialize(TableReqPic);
			for (int k = 0; k < Checkpoints.Count; k++)
			{
				Checkpoints[k].Init(objectTable);
			}
		}

		public void BunkerUnlockedUpdate()
		{
			for (int i = 0; i < Bunkers.Count; i++)
			{
				Bunkers[i].UpdateTPButtons();
			}
		}

		public void TeleportTo(int i)
		{
			GM.CurrentMovementManager.TeleportToPoint(Bunkers[i].TeleportPoint.position, isAbsolute: true);
			UnityEngine.Object.Instantiate(TeleportInEffect, Bunkers[i].TeleportPoint.position, Quaternion.identity);
			m_currentBunkerSpot = i;
			GM.CurrentSceneSettings.DeathResetPoint.position = Bunkers[i].TeleportPoint.position;
			GM.CurrentSceneSettings.DeathResetPoint.rotation = Quaternion.identity;
			GM.CurrentPlayerBody.HealPercent(1f);
		}

		public void TeleportToTowerPad(int i)
		{
			GM.CurrentMovementManager.TeleportToPoint(TowerPads[i].position, isAbsolute: true);
			UnityEngine.Object.Instantiate(TeleportInEffect, TowerPads[i].position, Quaternion.identity);
			GM.CurrentPlayerBody.HealPercent(1f);
		}

		public void EnteredBunker(int b)
		{
			for (int i = 0; i < CasesByDay[b].MessagesByDay.Count; i++)
			{
				if (!GM.Options.XmasFlags.MessagesAcquired[CasesByDay[b].MessagesByDay[i]])
				{
					UnlockMessage(CasesByDay[b].MessagesByDay[i]);
				}
			}
		}

		public void UnlockMessage(int i)
		{
			if (!GM.Options.XmasFlags.MessagesAcquired[i] && Panel.Messages[i].AudClip != null)
			{
				Debug.Log("MessageReceived " + i);
				GM.Options.XmasFlags.MessagesAcquired[i] = true;
				GM.Options.SaveToFile();
				Panel.UpdateMessageDisplay();
				FVRPooledAudioSource fVRPooledAudioSource = SM.PlayCoreSound(FVRPooledAudioType.Generic, AudEvent_MessageReceived, Panel.transform.position);
				fVRPooledAudioSource.FollowThisTransform(Panel.transform);
			}
		}

		private void Update()
		{
			PSys_Rig.position = GM.CurrentPlayerBody.transform.position;
			m_isInABunker = false;
			m_whichBunkerAmIIn = -1;
			for (int i = 0; i < Bunkers.Count; i++)
			{
				if (!Bunkers[i].IsLockDown && Bunkers[i].IsUnlocked && TestVolumeBool(Bunkers[i].BunkerBounds, GM.CurrentPlayerBody.Head.position))
				{
					if (i != m_whichBunkerAmIIn)
					{
						EnteredBunker(i);
					}
					m_whichBunkerAmIIn = i;
					m_isInABunker = true;
					break;
				}
			}
			m_curCheckPointCheck++;
			if (m_curCheckPointCheck >= Checkpoints.Count)
			{
				m_curCheckPointCheck = 0;
			}
			if (Checkpoints[m_curCheckPointCheck].HasMessage)
			{
				float num = Vector3.Distance(GM.CurrentPlayerBody.Head.position, Checkpoints[m_curCheckPointCheck].SatDish.position);
				if (num < Checkpoints[m_curCheckPointCheck].ActivationRange)
				{
					UnlockMessage(Checkpoints[m_curCheckPointCheck].MessageToUnlock);
				}
			}
			for (int j = 0; j < SF.Count; j++)
			{
				if (TestVolumeBool(SF[j], GM.CurrentPlayerBody.Head.position))
				{
					InitSF();
				}
			}
			FXManagement();
			if (m_isSF)
			{
				m_SFTick += Time.deltaTime;
				if (m_SFTick > 2f)
				{
					GM.CurrentMovementManager.TeleportToPoint(GM.CurrentSceneSettings.DeathResetPoint.position, isAbsolute: true, GM.CurrentSceneSettings.DeathResetPoint.forward);
					m_isSF = false;
					m_SFTick = 0f;
				}
			}
		}

		private void InitSF()
		{
			if (!m_isSF)
			{
				if (!m_isSF)
				{
					m_isSF = true;
					m_SFTick = 0f;
				}
				m_mySF = UnityEngine.Object.Instantiate(SFSpawn, GM.CurrentPlayerBody.Head.position, Quaternion.identity);
				m_mySF.transform.SetParent(GM.CurrentPlayerBody.Head);
			}
		}

		private void FXManagement()
		{
			for (int i = 0; i < m_closestWind.Count; i++)
			{
				m_closestWind[i].Dist = Vector3.Distance(m_closestWind[i].W.transform.position, GM.CurrentPlayerBody.Head.position);
			}
			for (int j = 0; j < 20; j++)
			{
				m_windCheckIndex++;
				if (m_windCheckIndex >= WindPoints.Count)
				{
					m_windCheckIndex = 0;
				}
				if (!m_closestTs.Contains(WindPoints[m_windCheckIndex]))
				{
					float num = Vector3.Distance(GM.CurrentPlayerBody.Head.position, WindPoints[m_windCheckIndex].transform.position);
					if (num < m_closestWind[m_closestWind.Count - 1].Dist)
					{
						m_closestWind[m_closestWind.Count - 1].Dist = num;
						m_closestTs.Remove(m_closestWind[m_closestWind.Count - 1].W);
						m_closestWind[m_closestWind.Count - 1].W = WindPoints[m_windCheckIndex];
						m_closestTs.Add(WindPoints[m_windCheckIndex]);
					}
				}
			}
			m_closestWind.Sort();
			float num2 = m_closestWind[0].Dist + m_closestWind[1].Dist + m_closestWind[2].Dist + m_closestWind[3].Dist;
			Vector3 zero = Vector3.zero;
			for (int k = 0; k < 2; k++)
			{
				float num3 = 1f - m_closestWind[k].Dist / num2;
				zero += m_closestWind[k].W.transform.forward * m_closestWind[k].W.transform.localScale.z * num3;
			}
			zero.y = 0f;
			float num4 = 0f;
			for (int l = 0; l < 2; l++)
			{
				float num5 = 1f - m_closestWind[l].Dist / num2;
				num4 += m_closestWind[l].W.WhiteOut * num5;
			}
			if (m_isInABunker)
			{
				WhiteOutThreshold = Mathf.MoveTowards(WhiteOutThreshold, 0f, Time.deltaTime * 2f);
			}
			else
			{
				WhiteOutThreshold = Mathf.MoveTowards(WhiteOutThreshold, num4, Time.deltaTime * 0.1f);
			}
			UpdateWhiteOut();
			Wind = Vector3.Slerp(Wind, zero, Time.deltaTime * 2f);
			windDirMax = Wind * 0.7f;
			windDirMin = Wind * 0.05f;
			L_Snowbluster = Wind.magnitude / 30f;
			if (L_Snowbluster >= 0.9f)
			{
				Cy_Snowbluster = 4;
			}
			else if (L_Snowbluster >= 0.65f)
			{
				Cy_Snowbluster = 3;
			}
			else if (L_Snowbluster >= 0.35f)
			{
				Cy_Snowbluster = 2;
			}
			else
			{
				Cy_Snowbluster = 1;
			}
			if (m_isInABunker)
			{
				ParticleSystem.EmissionModule emission = PSys_SnowBluster.emission;
				emission.enabled = false;
				emission = PSys_SnowDown.emission;
				emission.enabled = false;
				return;
			}
			ParticleSystem.EmissionModule emission2 = PSys_SnowBluster.emission;
			emission2.enabled = true;
			emission2 = PSys_SnowDown.emission;
			emission2.enabled = true;
			ParticleSystem.ForceOverLifetimeModule forceOverLifetime = PSys_SnowDown.forceOverLifetime;
			ParticleSystem.MinMaxCurve x = forceOverLifetime.x;
			x.constantMin = windDirMin.x * 0.5f - 1f;
			x.constantMax = windDirMax.x * 0.5f + 1f;
			forceOverLifetime.x = x;
			x = forceOverLifetime.z;
			x.constantMin = windDirMin.z * 0.5f - 1f;
			x.constantMax = windDirMax.z * 0.5f + 1f;
			forceOverLifetime.z = x;
			forceOverLifetime = PSys_SnowBluster.forceOverLifetime;
			x = forceOverLifetime.x;
			x.constantMin = windDirMax.x * 0.5f;
			x.constantMax = windDirMax.x;
			forceOverLifetime.x = x;
			x = forceOverLifetime.z;
			x.constantMin = windDirMax.z * 0.5f;
			x.constantMax = windDirMax.z;
			forceOverLifetime.z = x;
			ParticleSystem.TextureSheetAnimationModule textureSheetAnimation = PSys_SnowBluster.textureSheetAnimation;
			textureSheetAnimation.cycleCount = Cy_Snowbluster;
			MTL_Snowbluster.SetColor("_TintColor", Color.Lerp(C_Snowbluster_MinIntensity, C_Snowbluster_MaxIntensity, L_Snowbluster));
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
	}
}
