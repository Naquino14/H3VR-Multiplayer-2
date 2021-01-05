using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace FistVR
{
	public class FVRPlayerBody : MonoBehaviour
	{
		public Transform Head;

		public Transform LeftHand;

		public Transform RightHand;

		public Transform Neck;

		public Transform Torso;

		public ParticleSystem PSystemDamage;

		public ParticleSystem PSystemBlind;

		public GameObject NavMeshOb;

		public FVRPlayerHitbox[] Hitboxes;

		public List<FVRQuickBeltSlot> QuickbeltSlots = new List<FVRQuickBeltSlot>();

		public Transform NeckJointTransform;

		public Transform NeckTransform;

		public Transform TorsoTransform;

		public Transform NeckJointTargetsThis;

		public Transform NeckTargetsThis;

		public Transform TorsoTargetsThis;

		public Camera EyeCam;

		public PostProcessLayer PostLayer;

		public GameObject WristMenuPrefab;

		private float m_startingHealth = 5000f;

		public float Health = 5000f;

		public GameObject HealthBarPrefab;

		public GameObject HealthBar;

		public GameObject VFX_Farout;

		public GameObject VFX_Cyclops;

		public GameObject VFX_Biclops;

		private int m_playerIFF;

		private int m_backupIFF;

		public List<AIEntity> PlayerEntities;

		public FVRObject PlayerSosigBodyPrefab;

		private PlayerSosigBody m_sosigPlayerBody;

		public float VisibilityMult = 1f;

		public string DebugString;

		public float GlobalHearingTarget = 1f;

		public float GlobalHearing = 1f;

		private float m_blindAmount;

		private float m_highVisibilityEventValue;

		public LayerMask LM_OcclusionTesting;

		private RaycastHit hit;

		private float m_averageOcclusionDistance = 2.5f;

		private bool m_isHealing;

		private bool m_isDamResist;

		private bool m_isDamPowerUp;

		private bool m_isInfiniteAmmo;

		private bool m_isGhosted;

		private bool m_isMuscleMeat;

		private bool m_isCyclops;

		private bool m_isSnakeEyes;

		private bool m_isBlort;

		private bool m_isFarOutMan;

		private float m_buffTime_Heal;

		private float m_buffTime_DamResist;

		private float m_buffTime_DamPowerUp;

		private float m_buffTime_InfiniteAmmo;

		private float m_buffTime_Ghosted;

		private float m_buffTime_MuscleMeat;

		private float m_buffTime_Cyclops;

		private float m_buffTime_SnakeEyes;

		private float m_buffTime_Blort;

		private float m_buffTime_FarOutMan;

		private bool m_isHurting;

		private bool m_isDamMult;

		private bool m_isDamPowerDown;

		private bool m_isAmmoDrain;

		private bool m_isSuperVisible;

		private bool m_isWeakMeat;

		private bool m_isBiClops;

		private bool m_isMoleEye;

		private bool m_isDlort;

		private bool m_isBadTrip;

		private float m_debuffTime_Hurt;

		private float m_debuffTime_DamMult;

		private float m_debuffTime_DamPowerDown;

		private float m_debuffTime_AmmoDrain;

		private float m_debuffTime_SuperVisible;

		private float m_debuffTime_WeakMeat;

		private float m_debuffTime_BiClops;

		private float m_debuffTime_MoleEye;

		private float m_debuffTime_Dlort;

		private float m_debuffTime_BadTrip;

		private float m_buffIntensity_HealHarm = 20f;

		private float m_damageResist;

		private float m_damageMult = 1f;

		private float m_regenMult = 1f;

		private float m_muscleMeatPower = 1f;

		private float m_cyclopsPower = 1f;

		private GameObject[] BuffSystems_LeftHand = new GameObject[14];

		private GameObject[] BuffSystems_RightHand = new GameObject[14];

		private GameObject[] DeBuffSystems_LeftHand = new GameObject[14];

		private GameObject[] DeBuffSystems_RightHand = new GameObject[14];

		public bool IsHealing => m_isHealing;

		public bool IsDamResist => m_isDamResist;

		public bool isDamPowerUp => m_isDamPowerUp;

		public bool IsInfiniteAmmo => m_isInfiniteAmmo;

		public bool IsGhosted => m_isGhosted;

		public bool IsMuscleMeat => m_isMuscleMeat;

		public bool IsCyclops => m_isCyclops;

		public bool IsBlort => m_isBlort;

		public bool IsHurting => m_isHurting;

		public bool IsDamMult => m_isDamMult;

		public bool IsDamPowerDown => m_isDamPowerDown;

		public bool IsAmmoDrain => m_isAmmoDrain;

		public bool IsSuperVisible => m_isSuperVisible;

		public bool IsWeakMeat => m_isWeakMeat;

		public bool IsBiClops => m_isBiClops;

		public bool IsMoleEye => m_isMoleEye;

		public bool IsDlort => m_isDlort;

		public bool IsBadTrip => m_isBadTrip;

		private void Start()
		{
			UpdateSosigPlayerBodyState();
		}

		public void SetOutfit(SosigEnemyTemplate tem)
		{
			if (m_sosigPlayerBody == null)
			{
				return;
			}
			GM.Options.ControlOptions.MBClothing = tem.SosigEnemyID;
			SosigEnemyID mBClothing = GM.Options.ControlOptions.MBClothing;
			if (mBClothing != SosigEnemyID.None)
			{
				SosigEnemyTemplate sosigEnemyTemplate = ManagerSingleton<IM>.Instance.odicSosigObjsByID[mBClothing];
				if (sosigEnemyTemplate.OutfitConfig.Count > 0)
				{
					SosigOutfitConfig o = sosigEnemyTemplate.OutfitConfig[UnityEngine.Random.Range(0, sosigEnemyTemplate.OutfitConfig.Count)];
					m_sosigPlayerBody.ApplyOutfit(o);
				}
			}
		}

		public void UpdateSosigPlayerBodyState()
		{
			if (GM.Options.ControlOptions.MBMode == ControlOptions.MeatBody.Enabled)
			{
				if (!(m_sosigPlayerBody == null))
				{
					return;
				}
				GameObject gameObject = UnityEngine.Object.Instantiate(PlayerSosigBodyPrefab.GetGameObject(), GM.CurrentPlayerBody.Torso.position, GM.CurrentPlayerBody.Torso.rotation);
				m_sosigPlayerBody = gameObject.GetComponent<PlayerSosigBody>();
				SosigEnemyID mBClothing = GM.Options.ControlOptions.MBClothing;
				Debug.Log("Setting to:" + mBClothing);
				if (mBClothing != SosigEnemyID.None)
				{
					SosigEnemyTemplate sosigEnemyTemplate = ManagerSingleton<IM>.Instance.odicSosigObjsByID[mBClothing];
					if (sosigEnemyTemplate.OutfitConfig.Count > 0)
					{
						SosigOutfitConfig o = sosigEnemyTemplate.OutfitConfig[UnityEngine.Random.Range(0, sosigEnemyTemplate.OutfitConfig.Count)];
						m_sosigPlayerBody.ApplyOutfit(o);
					}
				}
			}
			else if (m_sosigPlayerBody != null)
			{
				UnityEngine.Object.Destroy(m_sosigPlayerBody.gameObject);
				m_sosigPlayerBody = null;
			}
		}

		public void Update()
		{
			UpdatePowerUps();
			if (m_blindAmount > 0f)
			{
				m_blindAmount -= Time.deltaTime;
				GlobalHearingTarget = 1f - m_blindAmount;
				GlobalHearingTarget = Mathf.Clamp(GlobalHearingTarget, 0.1f, 1f);
			}
			else
			{
				GlobalHearingTarget = 1f;
			}
			if (GlobalHearingTarget < GlobalHearing)
			{
				GlobalHearing = Mathf.Lerp(GlobalHearing, GlobalHearingTarget, Time.deltaTime * 8f);
			}
			else
			{
				GlobalHearing = Mathf.Lerp(GlobalHearing, GlobalHearingTarget, Time.deltaTime * 3f);
			}
			AudioListener.volume = GlobalHearing;
			PlayerEntityUpdate();
		}

		public void BlindPlayer(float f)
		{
			m_blindAmount = Mathf.Max(m_blindAmount, f + 0.5f);
			if (m_blindAmount >= 1f)
			{
				PSystemBlind.Emit(1);
			}
		}

		private void PlayerEntityUpdate()
		{
			m_highVisibilityEventValue = Mathf.MoveTowards(m_highVisibilityEventValue, 0f, Time.deltaTime * 1f);
			float num = 1.5f;
			float topSpeedInLastSecond = GM.CurrentMovementManager.GetTopSpeedInLastSecond();
			float num2 = Mathf.Clamp(topSpeedInLastSecond * 0.2f, 0f, 0.2f);
			num -= num2;
			float num3 = Mathf.Clamp(GetBodyMovementSpeed() * 0.01f, 0f, 0.3f);
			num -= num3;
			num -= m_highVisibilityEventValue;
			float y = Head.transform.localPosition.y;
			float num4 = 0f;
			if (y < 1.2f)
			{
				num4 = (y - 1.2f) * 1f;
			}
			num += Mathf.Clamp(num4, 0f, 0.6f);
			VisibilityMult = Mathf.Clamp(num, 0.9f, 2.1f);
			DebugString = "Total:" + VisibilityMult.ToString("#.##") + " TopSpeedPenalty:" + num2 + " HandPenalty:" + num3 + " VisibleEvent:" + m_highVisibilityEventValue + " HeightBonus:" + num4 + " Occlusion:" + GetBodyOcclusion();
			float num5 = 0.1f;
			if (!HasAGunInHand())
			{
				num5 += 0.03f;
			}
			if (!HasAWeaponInHand())
			{
				num5 += 0.1f;
			}
			if (GetPlayerHealth() < 0.1f)
			{
				num5 += 0.5f;
			}
			else if (GetPlayerHealth() < 0.4f)
			{
				num5 += 0.1f;
			}
			for (int i = 0; i < PlayerEntities.Count; i++)
			{
				AIEntity aIEntity = PlayerEntities[i];
				if (m_isGhosted)
				{
					aIEntity.VisibilityMultiplier = 2.2f;
				}
				else if (m_isSuperVisible)
				{
					aIEntity.VisibilityMultiplier = 0.1f;
				}
				else
				{
					aIEntity.VisibilityMultiplier = VisibilityMult;
				}
				aIEntity.DangerMultiplier = num5;
			}
			if (Physics.Raycast(Head.position, UnityEngine.Random.onUnitSphere, out hit, 5f, LM_OcclusionTesting, QueryTriggerInteraction.Ignore))
			{
				MergeOcclusionDistance(hit.distance);
			}
			else
			{
				MergeOcclusionDistance(5f);
			}
		}

		private bool HasAWeaponInHand()
		{
			for (int i = 0; i < GM.CurrentMovementManager.Hands.Length; i++)
			{
				if (GM.CurrentMovementManager.Hands[i].CurrentInteractable != null && (GM.CurrentMovementManager.Hands[i].CurrentInteractable is FVRFireArm || GM.CurrentMovementManager.Hands[i].CurrentInteractable is FVRMeleeWeapon || GM.CurrentMovementManager.Hands[i].CurrentInteractable is SosigWeaponPlayerInterface))
				{
					return true;
				}
			}
			return false;
		}

		private bool HasAGunInHand()
		{
			for (int i = 0; i < GM.CurrentMovementManager.Hands.Length; i++)
			{
				if (GM.CurrentMovementManager.Hands[i].CurrentInteractable != null && (GM.CurrentMovementManager.Hands[i].CurrentInteractable is FVRFireArm || (GM.CurrentMovementManager.Hands[i].CurrentInteractable is SosigWeaponPlayerInterface && (GM.CurrentMovementManager.Hands[i].CurrentInteractable as SosigWeaponPlayerInterface).W.Type == SosigWeapon.SosigWeaponType.Gun)))
				{
					return true;
				}
			}
			return false;
		}

		public void VisibleEvent(float intensity)
		{
			m_highVisibilityEventValue = Mathf.Max(m_highVisibilityEventValue, intensity);
		}

		private float GetBodyMovementSpeed()
		{
			float magnitude = GM.CurrentMovementManager.Hands[0].Input.VelLinearLocal.magnitude;
			return Mathf.Max(magnitude, GM.CurrentMovementManager.Hands[1].Input.VelLinearLocal.magnitude);
		}

		private void MergeOcclusionDistance(float distance)
		{
			m_averageOcclusionDistance = Mathf.Lerp(m_averageOcclusionDistance, distance, 0.05f);
		}

		private float GetBodyOcclusion()
		{
			if (m_averageOcclusionDistance < 3.5f)
			{
				return 1f - m_averageOcclusionDistance / 3.5f;
			}
			return 0f;
		}

		public FVRSoundEnvironment GetCurrentSoundEnvironment()
		{
			return GM.CurrentSceneSettings.DefaultSoundEnvironment;
		}

		public int GetPlayerIFF()
		{
			return m_playerIFF;
		}

		public void SetPlayerIFF(int iff)
		{
			m_playerIFF = iff;
			for (int i = 0; i < PlayerEntities.Count; i++)
			{
				PlayerEntities[i].IFFCode = m_playerIFF;
			}
		}

		public float GetDamageResist()
		{
			return m_damageResist;
		}

		public float GetDamageMult()
		{
			return m_damageMult;
		}

		public float GetRegenMult()
		{
			return m_regenMult;
		}

		public float GetMuscleMeatPower()
		{
			return m_muscleMeatPower;
		}

		public float GetCyclopsPower()
		{
			return m_cyclopsPower;
		}

		private void ActivateBuff(int i, bool isInverted)
		{
			if (PUM.HasEffectPlayer(i, isInverted))
			{
				if (!isInverted && BuffSystems_LeftHand[i] == null)
				{
					GameObject gameObject = UnityEngine.Object.Instantiate(PUM.GetEffectPlayer(i, isInverted), GM.CurrentPlayerBody.LeftHand.transform.position, GM.CurrentPlayerBody.LeftHand.transform.rotation);
					gameObject.transform.SetParent(GM.CurrentPlayerBody.LeftHand.transform);
					BuffSystems_LeftHand[i] = gameObject;
					GameObject gameObject2 = UnityEngine.Object.Instantiate(PUM.GetEffectPlayer(i, isInverted), GM.CurrentPlayerBody.RightHand.transform.position, GM.CurrentPlayerBody.RightHand.transform.rotation);
					gameObject2.transform.SetParent(GM.CurrentPlayerBody.RightHand.transform);
					BuffSystems_RightHand[i] = gameObject2;
				}
				else if (isInverted && DeBuffSystems_LeftHand[i] == null)
				{
					GameObject gameObject3 = UnityEngine.Object.Instantiate(PUM.GetEffectPlayer(i, isInverted), GM.CurrentPlayerBody.LeftHand.transform.position, GM.CurrentPlayerBody.LeftHand.transform.rotation);
					gameObject3.transform.SetParent(GM.CurrentPlayerBody.LeftHand.transform);
					DeBuffSystems_LeftHand[i] = gameObject3;
					GameObject gameObject4 = UnityEngine.Object.Instantiate(PUM.GetEffectPlayer(i, isInverted), GM.CurrentPlayerBody.RightHand.transform.position, GM.CurrentPlayerBody.RightHand.transform.rotation);
					gameObject4.transform.SetParent(GM.CurrentPlayerBody.RightHand.transform);
					DeBuffSystems_RightHand[i] = gameObject4;
				}
			}
		}

		private void DeActivateBuff(int i)
		{
			if (BuffSystems_LeftHand[i] != null)
			{
				UnityEngine.Object.Destroy(BuffSystems_LeftHand[i]);
				BuffSystems_LeftHand[i] = null;
			}
			if (BuffSystems_RightHand[i] != null)
			{
				UnityEngine.Object.Destroy(BuffSystems_RightHand[i]);
				BuffSystems_RightHand[i] = null;
			}
		}

		private void DeActivateDeBuff(int i)
		{
			if (DeBuffSystems_LeftHand[i] != null)
			{
				UnityEngine.Object.Destroy(DeBuffSystems_LeftHand[i]);
				DeBuffSystems_LeftHand[i] = null;
			}
			if (DeBuffSystems_RightHand[i] != null)
			{
				UnityEngine.Object.Destroy(DeBuffSystems_RightHand[i]);
				DeBuffSystems_RightHand[i] = null;
			}
		}

		private void DeActivateAllBuffSystems()
		{
			for (int i = 0; i < 13; i++)
			{
				DeActivateBuff(i);
				DeActivateDeBuff(i);
			}
		}

		public void DeActivatePowerIfActive(PowerupType type)
		{
		}

		public void ActivatePower(PowerupType type, PowerUpIntensity intensity, PowerUpDuration d, bool isPuke, bool isInverted, float DurationOverride = -1f)
		{
			float b = 1f;
			if (isPuke && GM.ZMaster != null)
			{
				int num = UnityEngine.Random.Range(3, 5);
				for (int i = 0; i < num; i++)
				{
					GM.ZMaster.VomitRandomThing();
				}
			}
			switch (d)
			{
			case PowerUpDuration.Full:
				b = 30f;
				break;
			case PowerUpDuration.SuperLong:
				b = 40f;
				break;
			case PowerUpDuration.Short:
				b = 20f;
				break;
			case PowerUpDuration.VeryShort:
				b = 10f;
				break;
			case PowerUpDuration.Blip:
				b = 2f;
				break;
			}
			if (DurationOverride > 0f)
			{
				b = DurationOverride;
			}
			switch (type)
			{
			case PowerupType.Health:
			{
				float f = 0f;
				switch (intensity)
				{
				case PowerUpIntensity.High:
					f = 1f;
					break;
				case PowerUpIntensity.Medium:
					f = 0.5f;
					break;
				case PowerUpIntensity.Low:
					f = 0.25f;
					break;
				}
				if (!isInverted)
				{
					HealPercent(f);
				}
				else
				{
					HarmPercent(f);
				}
				break;
			}
			case PowerupType.QuadDamage:
				if (!isInverted)
				{
					switch (intensity)
					{
					case PowerUpIntensity.High:
						m_damageMult = 4f;
						break;
					case PowerUpIntensity.Medium:
						m_damageMult = 3f;
						break;
					case PowerUpIntensity.Low:
						m_damageMult = 2f;
						break;
					}
					m_isDamPowerUp = true;
					m_isDamPowerDown = false;
					DeActivateDeBuff(1);
					m_buffTime_DamPowerUp = Mathf.Max(m_buffTime_DamPowerUp, b);
				}
				else
				{
					switch (intensity)
					{
					case PowerUpIntensity.High:
						m_damageMult = 0.125f;
						break;
					case PowerUpIntensity.Medium:
						m_damageMult = 0.25f;
						break;
					case PowerUpIntensity.Low:
						m_damageMult = 0.5f;
						break;
					}
					m_isDamPowerDown = true;
					m_isDamPowerUp = false;
					DeActivateBuff(1);
					m_debuffTime_DamPowerDown = Mathf.Max(m_debuffTime_DamPowerDown, b);
				}
				break;
			case PowerupType.InfiniteAmmo:
				if (!isInverted)
				{
					m_isInfiniteAmmo = true;
					m_isAmmoDrain = false;
					DeActivateDeBuff(2);
					m_buffTime_InfiniteAmmo = Mathf.Max(m_buffTime_InfiniteAmmo, b);
				}
				else
				{
					m_isAmmoDrain = true;
					m_isInfiniteAmmo = false;
					DeActivateBuff(2);
					m_debuffTime_AmmoDrain = Mathf.Max(m_debuffTime_AmmoDrain, b);
				}
				break;
			case PowerupType.Invincibility:
				if (!isInverted)
				{
					switch (intensity)
					{
					case PowerUpIntensity.High:
						m_isDamResist = true;
						m_isDamMult = false;
						DeActivateDeBuff(3);
						m_damageResist = 0f;
						m_buffTime_DamResist = Mathf.Max(m_buffTime_DamResist, b);
						break;
					case PowerUpIntensity.Medium:
						m_isDamResist = true;
						m_isDamMult = false;
						DeActivateDeBuff(3);
						m_damageResist = 0.5f;
						m_buffTime_DamResist = Mathf.Max(m_buffTime_DamResist, b);
						break;
					case PowerUpIntensity.Low:
						m_isDamResist = true;
						m_isDamMult = false;
						DeActivateDeBuff(3);
						m_damageResist = 0.75f;
						m_buffTime_DamResist = Mathf.Max(m_buffTime_DamResist, b);
						break;
					}
				}
				else
				{
					switch (intensity)
					{
					case PowerUpIntensity.High:
						m_isDamMult = true;
						m_isDamResist = false;
						DeActivateBuff(3);
						m_damageResist = 4f;
						m_debuffTime_DamMult = Mathf.Max(m_debuffTime_DamMult, b);
						break;
					case PowerUpIntensity.Medium:
						m_isDamMult = true;
						m_isDamResist = false;
						DeActivateBuff(3);
						m_damageResist = 3f;
						m_debuffTime_DamMult = Mathf.Max(m_debuffTime_DamMult, b);
						break;
					case PowerUpIntensity.Low:
						m_isDamResist = true;
						m_isDamMult = false;
						DeActivateBuff(3);
						m_damageResist = 2f;
						m_debuffTime_DamMult = Mathf.Max(m_debuffTime_DamMult, b);
						break;
					}
				}
				break;
			case PowerupType.Ghosted:
				if (!isInverted)
				{
					m_isGhosted = true;
					m_isSuperVisible = false;
					DeActivateDeBuff(4);
					m_buffTime_Ghosted = Mathf.Max(m_buffTime_Ghosted, b);
				}
				else
				{
					m_isSuperVisible = true;
					m_isGhosted = false;
					DeActivateBuff(4);
					m_debuffTime_SuperVisible = Mathf.Max(m_debuffTime_SuperVisible, b);
				}
				break;
			case PowerupType.MuscleMeat:
				if (!isInverted)
				{
					switch (intensity)
					{
					case PowerUpIntensity.High:
						m_muscleMeatPower = 4f;
						break;
					case PowerUpIntensity.Medium:
						m_muscleMeatPower = 3f;
						break;
					case PowerUpIntensity.Low:
						m_muscleMeatPower = 2f;
						break;
					}
					m_isMuscleMeat = true;
					m_isWeakMeat = false;
					DeActivateDeBuff(6);
					m_buffTime_MuscleMeat = Mathf.Max(m_buffTime_MuscleMeat, b);
				}
				else
				{
					switch (intensity)
					{
					case PowerUpIntensity.High:
						m_muscleMeatPower = 0.125f;
						break;
					case PowerUpIntensity.Medium:
						m_muscleMeatPower = 0.25f;
						break;
					case PowerUpIntensity.Low:
						m_muscleMeatPower = 0.5f;
						break;
					}
					m_isWeakMeat = true;
					m_isMuscleMeat = false;
					DeActivateBuff(6);
					m_debuffTime_WeakMeat = Mathf.Max(m_debuffTime_WeakMeat, b);
				}
				break;
			case PowerupType.Regen:
				if (!isInverted)
				{
					switch (intensity)
					{
					case PowerUpIntensity.High:
						m_buffIntensity_HealHarm = 0.2f;
						break;
					case PowerUpIntensity.Medium:
						m_buffIntensity_HealHarm = 0.1f;
						break;
					case PowerUpIntensity.Low:
						m_buffIntensity_HealHarm = 0.05f;
						break;
					}
					m_isHealing = true;
					m_isHurting = false;
					DeActivateDeBuff(10);
					m_buffTime_Heal = Mathf.Max(m_buffTime_Heal, b);
				}
				else
				{
					switch (intensity)
					{
					case PowerUpIntensity.High:
						m_buffIntensity_HealHarm = 0.2f;
						break;
					case PowerUpIntensity.Medium:
						m_buffIntensity_HealHarm = 0.1f;
						break;
					case PowerUpIntensity.Low:
						m_buffIntensity_HealHarm = 0.05f;
						break;
					}
					m_isHurting = true;
					m_isHealing = false;
					DeActivateBuff(10);
					m_debuffTime_Hurt = Mathf.Max(m_debuffTime_Hurt, b);
				}
				break;
			case PowerupType.Cyclops:
				switch (intensity)
				{
				case PowerUpIntensity.High:
					m_cyclopsPower = 4f;
					break;
				case PowerUpIntensity.Medium:
					m_cyclopsPower = 3f;
					break;
				case PowerUpIntensity.Low:
					m_cyclopsPower = 2f;
					break;
				}
				if (!isInverted)
				{
					m_isCyclops = true;
					m_isBiClops = false;
					DeActivateDeBuff(11);
					VFX_Biclops.SetActive(value: false);
					m_buffTime_Cyclops = Mathf.Max(m_buffTime_Cyclops, b);
				}
				else
				{
					m_isBiClops = true;
					m_isCyclops = false;
					DeActivateBuff(11);
					VFX_Cyclops.SetActive(value: false);
					m_debuffTime_BiClops = Mathf.Max(m_debuffTime_BiClops, b);
				}
				if (!isInverted)
				{
					VFX_Cyclops.SetActive(value: true);
				}
				else
				{
					VFX_Biclops.SetActive(value: true);
				}
				break;
			case PowerupType.HomeTown:
				if (!isInverted)
				{
					GM.CurrentMovementManager.TeleportToPoint(GM.CurrentSceneSettings.PowerupPoint_HomeTown.position, isAbsolute: true, GM.CurrentSceneSettings.PowerupPoint_HomeTown.forward);
				}
				else
				{
					GM.CurrentMovementManager.TeleportToPoint(GM.CurrentSceneSettings.PowerupPoint_InverseHomeTown.position, isAbsolute: true, GM.CurrentSceneSettings.PowerupPoint_InverseHomeTown.forward);
				}
				break;
			case PowerupType.WheredIGo:
				if (GM.CurrentMovementManager.Mode == FVRMovementManager.MovementMode.Teleport || GM.CurrentMovementManager.Mode == FVRMovementManager.MovementMode.Dash || GM.CurrentMovementManager.Mode == FVRMovementManager.MovementMode.JoystickTeleport || GM.CurrentMovementManager.Mode == FVRMovementManager.MovementMode.SlideToTarget)
				{
					int index = UnityEngine.Random.Range(0, GM.CurrentSceneSettings.PowerPoints_WheredIGo_TP.Count);
					GM.CurrentMovementManager.TeleportToPoint(GM.CurrentSceneSettings.PowerPoints_WheredIGo_TP[index].position, isAbsolute: false, GM.CurrentSceneSettings.PowerPoints_WheredIGo_TP[index].forward);
				}
				else
				{
					int index2 = UnityEngine.Random.Range(0, GM.CurrentSceneSettings.PowerPoints_WheredIGo_Grav.Count);
					GM.CurrentMovementManager.TeleportToPoint(GM.CurrentSceneSettings.PowerPoints_WheredIGo_Grav[index2].position, isAbsolute: false, GM.CurrentSceneSettings.PowerPoints_WheredIGo_Grav[index2].forward);
				}
				break;
			case PowerupType.SnakeEye:
				if (!isInverted)
				{
					GM.CurrentSceneSettings.SetSnakeEyes(b: true);
					GM.CurrentSceneSettings.SetMoleEye(b: false);
					m_buffTime_SnakeEyes = Mathf.Max(m_buffTime_SnakeEyes, b);
					m_isSnakeEyes = true;
					m_isMoleEye = false;
				}
				else
				{
					GM.CurrentSceneSettings.SetMoleEye(b: true);
					GM.CurrentSceneSettings.SetSnakeEyes(b: false);
					m_debuffTime_MoleEye = Mathf.Max(m_debuffTime_MoleEye, b);
					m_isMoleEye = true;
					m_isSnakeEyes = false;
				}
				break;
			case PowerupType.Blort:
				if (!isInverted)
				{
					GM.CurrentSceneSettings.SetBlort(b: true);
					GM.CurrentSceneSettings.SetDlort(b: false);
					m_buffTime_Blort = Mathf.Max(m_buffTime_Blort, b);
					m_isBlort = true;
					m_isDlort = false;
				}
				else
				{
					GM.CurrentSceneSettings.SetDlort(b: true);
					GM.CurrentSceneSettings.SetBlort(b: false);
					m_debuffTime_Dlort = Mathf.Max(m_debuffTime_Dlort, b);
					m_isDlort = true;
					m_isBlort = false;
				}
				break;
			case PowerupType.FarOutMeat:
				if (!isInverted)
				{
					GM.CurrentSceneSettings.SetFarOutMan(b: true);
					GM.CurrentSceneSettings.SetBadTrip(b: false);
					m_buffTime_FarOutMan = Mathf.Max(m_buffTime_FarOutMan, b);
					m_isFarOutMan = true;
					m_isBadTrip = false;
				}
				else
				{
					GM.CurrentSceneSettings.SetBadTrip(b: true);
					GM.CurrentSceneSettings.SetFarOutMan(b: false);
					m_debuffTime_BadTrip = Mathf.Max(m_debuffTime_BadTrip, b);
					m_isBadTrip = true;
					m_isFarOutMan = false;
				}
				break;
			}
			ActivateBuff((int)type, isInverted);
		}

		private void UpdatePowerUps()
		{
			if (m_isHealing && m_buffTime_Heal > 0f)
			{
				m_buffTime_Heal -= Time.deltaTime;
				if (m_buffTime_Heal <= 0f)
				{
					DeActivateBuff(10);
					m_isHealing = false;
				}
			}
			if (m_isDamResist && m_buffTime_DamResist > 0f)
			{
				m_buffTime_DamResist -= Time.deltaTime;
				if (m_buffTime_DamResist <= 0f)
				{
					DeActivateBuff(3);
					m_isDamResist = false;
				}
			}
			if (m_isDamPowerUp && m_buffTime_DamPowerUp > 0f)
			{
				m_buffTime_DamPowerUp -= Time.deltaTime;
				if (m_buffTime_DamPowerUp <= 0f)
				{
					DeActivateBuff(1);
					m_isDamPowerUp = false;
				}
			}
			if (m_isInfiniteAmmo && m_buffTime_InfiniteAmmo > 0f)
			{
				m_buffTime_InfiniteAmmo -= Time.deltaTime;
				if (m_buffTime_InfiniteAmmo <= 0f)
				{
					DeActivateBuff(2);
					m_isInfiniteAmmo = false;
				}
			}
			if (m_isGhosted && m_buffTime_Ghosted > 0f)
			{
				m_buffTime_Ghosted -= Time.deltaTime;
				if (m_buffTime_Ghosted <= 0f)
				{
					DeActivateBuff(4);
					m_isGhosted = false;
				}
			}
			if (m_isMuscleMeat && m_buffTime_MuscleMeat > 0f)
			{
				m_buffTime_MuscleMeat -= Time.deltaTime;
				if (m_buffTime_MuscleMeat <= 0f)
				{
					DeActivateBuff(6);
					m_isMuscleMeat = false;
				}
			}
			if (m_isCyclops && m_buffTime_Cyclops > 0f)
			{
				m_buffTime_Cyclops -= Time.deltaTime;
				if (m_buffTime_Cyclops <= 0f)
				{
					DeActivateBuff(11);
					m_isCyclops = false;
					VFX_Cyclops.SetActive(value: false);
				}
			}
			if (m_isSnakeEyes && m_buffTime_SnakeEyes > 0f)
			{
				m_buffTime_SnakeEyes -= Time.deltaTime;
				if (m_buffTime_SnakeEyes <= 0f)
				{
					m_isSnakeEyes = false;
					GM.CurrentSceneSettings.SetSnakeEyes(b: false);
				}
			}
			if (m_isMoleEye && m_debuffTime_MoleEye > 0f)
			{
				m_debuffTime_MoleEye -= Time.deltaTime;
				if (m_debuffTime_MoleEye <= 0f)
				{
					m_isMoleEye = false;
					GM.CurrentSceneSettings.SetMoleEye(b: false);
				}
			}
			if (m_isBlort && m_buffTime_Blort > 0f)
			{
				m_buffTime_Blort -= Time.deltaTime;
				if (m_buffTime_Blort <= 0f)
				{
					DeActivateBuff(9);
					m_isBlort = false;
					GM.CurrentSceneSettings.SetBlort(b: false);
				}
			}
			if (m_isDlort && m_debuffTime_Dlort > 0f)
			{
				m_debuffTime_Dlort -= Time.deltaTime;
				if (m_debuffTime_Dlort <= 0f)
				{
					DeActivateBuff(9);
					m_isDlort = false;
					GM.CurrentSceneSettings.SetDlort(b: false);
				}
			}
			if (m_isFarOutMan && m_buffTime_FarOutMan > 0f)
			{
				m_buffTime_FarOutMan -= Time.deltaTime;
				if (m_buffTime_FarOutMan <= 0f)
				{
					m_isFarOutMan = false;
					GM.CurrentSceneSettings.SetFarOutMan(b: false);
				}
			}
			if (m_isBadTrip && m_debuffTime_BadTrip > 0f)
			{
				m_debuffTime_BadTrip -= Time.deltaTime;
				if (m_debuffTime_BadTrip <= 0f)
				{
					m_isBadTrip = false;
					GM.CurrentSceneSettings.SetBadTrip(b: false);
				}
			}
			if (m_isHurting && m_debuffTime_Hurt > 0f)
			{
				m_debuffTime_Hurt -= Time.deltaTime;
				if (m_debuffTime_Hurt <= 0f)
				{
					DeActivateDeBuff(10);
					m_isHurting = false;
				}
			}
			if (m_isDamMult && m_debuffTime_DamMult > 0f)
			{
				m_debuffTime_DamMult -= Time.deltaTime;
				if (m_debuffTime_DamMult <= 0f)
				{
					DeActivateDeBuff(3);
					m_isDamMult = false;
				}
			}
			if (m_isDamPowerDown && m_debuffTime_DamPowerDown > 0f)
			{
				m_debuffTime_DamPowerDown -= Time.deltaTime;
				if (m_debuffTime_DamPowerDown <= 0f)
				{
					DeActivateDeBuff(1);
					m_isDamPowerDown = false;
				}
			}
			if (m_isAmmoDrain && m_debuffTime_AmmoDrain > 0f)
			{
				m_debuffTime_AmmoDrain -= Time.deltaTime;
				if (m_debuffTime_AmmoDrain <= 0f)
				{
					DeActivateDeBuff(2);
					m_isAmmoDrain = false;
				}
			}
			if (m_isSuperVisible && m_debuffTime_SuperVisible > 0f)
			{
				m_debuffTime_SuperVisible -= Time.deltaTime;
				if (m_debuffTime_SuperVisible <= 0f)
				{
					DeActivateDeBuff(4);
					m_isSuperVisible = false;
				}
			}
			if (m_isWeakMeat && m_debuffTime_WeakMeat > 0f)
			{
				m_debuffTime_WeakMeat -= Time.deltaTime;
				if (m_debuffTime_WeakMeat <= 0f)
				{
					DeActivateDeBuff(6);
					m_isWeakMeat = false;
				}
			}
			if (m_isBiClops && m_debuffTime_BiClops > 0f)
			{
				m_debuffTime_BiClops -= Time.deltaTime;
				if (m_debuffTime_BiClops <= 0f)
				{
					DeActivateDeBuff(11);
					m_isBiClops = false;
					VFX_Biclops.SetActive(value: false);
				}
			}
			if (m_isHealing)
			{
				HealPercent(m_buffIntensity_HealHarm * Time.deltaTime);
			}
			if (m_isHurting)
			{
				HarmPercent(m_buffIntensity_HealHarm * Time.deltaTime);
			}
			if (m_isCyclops)
			{
				VFX_Cyclops.SetActive(value: true);
			}
			else
			{
				VFX_Cyclops.SetActive(value: false);
			}
			if (m_isBiClops)
			{
				VFX_Biclops.SetActive(value: true);
			}
			else
			{
				VFX_Biclops.SetActive(value: false);
			}
		}

		public void UpdateCameraPost()
		{
			if (PostLayer != null)
			{
				if (GM.Options.PerformanceOptions.IsPostEnabled_AO || GM.Options.PerformanceOptions.IsPostEnabled_Bloom || GM.Options.PerformanceOptions.IsPostEnabled_CC)
				{
					PostLayer.enabled = true;
				}
				else
				{
					PostLayer.enabled = false;
				}
			}
		}

		public void Init(FVRSceneSettings SceneSettings)
		{
			SetPlayerIFF(SceneSettings.DefaultPlayerIFF);
			for (int i = 0; i < Hitboxes.Length; i++)
			{
				if (Hitboxes[i] != null)
				{
					Hitboxes[i].IsActivated = SceneSettings.AreHitboxesEnabled;
				}
			}
			if (SceneSettings.AreQuickbeltSlotsEnabled)
			{
				ConfigureQuickbelt(GM.Options.QuickbeltOptions.QuickbeltPreset);
			}
			GameObject gameObject = UnityEngine.Object.Instantiate(WristMenuPrefab, Vector3.zero, Quaternion.identity);
			gameObject.GetComponent<FVRWristMenu>().SetHandsAndFace(RightHand.GetComponent<FVRViveHand>(), LeftHand.GetComponent<FVRViveHand>(), EyeCam.transform);
			if (GM.CurrentSceneSettings.DoesUseHealthBar)
			{
				HealthBar = UnityEngine.Object.Instantiate(HealthBarPrefab, Vector3.zero, Quaternion.identity);
			}
			m_startingHealth = Health;
		}

		public bool RegisterPlayerHit(float DamagePoints, bool FromSelf)
		{
			GM.CurrentSceneSettings.OnPlayerTookDamage(DamagePoints / m_startingHealth);
			if (GM.CurrentSceneSettings.DoesDamageGetRegistered && GM.CurrentSceneSettings.DeathResetPoint != null && !GM.IsDead())
			{
				Health -= DamagePoints;
				HitEffect();
				if (Health <= 0f)
				{
					if (GM.CurrentMovementManager.Hands[0].CurrentInteractable != null && !(GM.CurrentMovementManager.Hands[0].CurrentInteractable is FVRPhysicalObject))
					{
						GM.CurrentMovementManager.Hands[0].CurrentInteractable.ForceBreakInteraction();
					}
					if (GM.CurrentMovementManager.Hands[1].CurrentInteractable != null && !(GM.CurrentMovementManager.Hands[1].CurrentInteractable is FVRPhysicalObject))
					{
						GM.CurrentMovementManager.Hands[1].CurrentInteractable.ForceBreakInteraction();
					}
					ManagerSingleton<GM>.Instance.KillPlayer(FromSelf);
					return true;
				}
			}
			return false;
		}

		public void KillPlayer(bool FromSelf)
		{
			if (!GM.IsDead())
			{
				if (GM.CurrentMovementManager.Hands[0].CurrentInteractable != null && !(GM.CurrentMovementManager.Hands[0].CurrentInteractable is FVRPhysicalObject))
				{
					GM.CurrentMovementManager.Hands[0].CurrentInteractable.ForceBreakInteraction();
				}
				if (GM.CurrentMovementManager.Hands[1].CurrentInteractable != null && !(GM.CurrentMovementManager.Hands[1].CurrentInteractable is FVRPhysicalObject))
				{
					GM.CurrentMovementManager.Hands[1].CurrentInteractable.ForceBreakInteraction();
				}
				ManagerSingleton<GM>.Instance.KillPlayer(FromSelf);
			}
		}

		public float GetPlayerHealth()
		{
			return Mathf.Clamp(Health / m_startingHealth, 0f, 1f);
		}

		public void HealPercent(float f)
		{
			Health += m_startingHealth * f;
			Health = Mathf.Clamp(Health, 0f, m_startingHealth);
		}

		public void HarmPercent(float f)
		{
			Health -= m_startingHealth * f;
			if (Health <= 0f)
			{
				KillPlayer(FromSelf: false);
			}
		}

		public void ResetHealth()
		{
			Health = m_startingHealth;
		}

		public void SetHealthThreshold(float h)
		{
			m_startingHealth = h;
			Health = h;
		}

		public int GetPlayerHealthRaw()
		{
			return (int)Health;
		}

		public int GetMaxHealthPlayerRaw()
		{
			return (int)m_startingHealth;
		}

		public void DisableHands()
		{
			LeftHand.gameObject.SetActive(value: false);
			RightHand.gameObject.SetActive(value: false);
		}

		public void EnableHands()
		{
			LeftHand.gameObject.SetActive(value: true);
			RightHand.gameObject.SetActive(value: true);
		}

		public void UpdatePlayerBodyPositions()
		{
			MoveBodyTargets();
			MoveHitBoxes();
		}

		public void MoveBodyTargets()
		{
			MoveTransformRelativeToTarget(NeckJointTransform, NeckJointTargetsThis, tracksPosition: false);
			MoveTransformRelativeToTarget(NeckTransform, NeckTargetsThis, tracksPosition: true);
			MoveTransformRelativeToTarget(TorsoTransform, TorsoTargetsThis, tracksPosition: true);
		}

		public void MoveHitBoxes()
		{
			for (int i = 0; i < Hitboxes.Length; i++)
			{
				Hitboxes[i].UpdatePositions();
			}
		}

		public void EnableHitBoxes()
		{
			for (int i = 0; i < Hitboxes.Length; i++)
			{
				Hitboxes[i].IsActivated = true;
			}
		}

		public void DisableHitBoxes()
		{
			for (int i = 0; i < Hitboxes.Length; i++)
			{
				Hitboxes[i].IsActivated = false;
			}
		}

		public void HitEffect()
		{
			PSystemDamage.Emit(1);
		}

		private void MoveTransformRelativeToTarget(Transform trans, Transform target, bool tracksPosition)
		{
			if (tracksPosition)
			{
				trans.position = target.position;
			}
			Vector3 forward = target.forward;
			Vector3 vector = forward;
			vector.y = 0f;
			vector.Normalize();
			Vector3 a = Vector3.zero;
			a = ((!(forward.y > 0f)) ? target.up : (-target.up));
			a.y = 0f;
			a.Normalize();
			float num = Mathf.Clamp(Vector3.Dot(vector, forward), 0f, 1f);
			Vector3 forward2 = Vector3.Lerp(a, vector, num * num);
			trans.rotation = Quaternion.LookRotation(forward2, Vector3.up);
		}

		public void MoveQuickbeltContents(Vector3 dir)
		{
			for (int i = 0; i < QuickbeltSlots.Count; i++)
			{
				QuickbeltSlots[i].MoveContentsInstant(dir);
			}
		}

		public void MoveQuickbeltContentsCheap(Vector3 dir)
		{
			for (int i = 0; i < QuickbeltSlots.Count; i++)
			{
				QuickbeltSlots[i].MoveContentsCheap(dir);
			}
		}

		public void ConfigureQuickbelt(int index)
		{
			if (QuickbeltSlots.Count > 0)
			{
				for (int num = QuickbeltSlots.Count - 1; num >= 0; num--)
				{
					if (QuickbeltSlots[num] == null)
					{
						QuickbeltSlots.RemoveAt(num);
					}
					else if (QuickbeltSlots[num].IsPlayer)
					{
						if (QuickbeltSlots[num].CurObject != null)
						{
							QuickbeltSlots[num].CurObject.ClearQuickbeltState();
						}
						UnityEngine.Object.Destroy(QuickbeltSlots[num].gameObject);
						QuickbeltSlots.RemoveAt(num);
					}
				}
			}
			int num2 = Mathf.Clamp(index, 0, ManagerSingleton<GM>.Instance.QuickbeltConfigurations.Length);
			GameObject gameObject = UnityEngine.Object.Instantiate(ManagerSingleton<GM>.Instance.QuickbeltConfigurations[num2], Torso.position, Torso.rotation);
			gameObject.transform.SetParent(Torso.transform);
			gameObject.transform.localPosition = Vector3.zero;
			IEnumerator enumerator = gameObject.transform.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Transform transform = (Transform)enumerator.Current;
					if (transform.gameObject.tag == "QuickbeltSlot")
					{
						FVRQuickBeltSlot component = transform.GetComponent<FVRQuickBeltSlot>();
						if (GM.Options.QuickbeltOptions.QuickbeltHandedness > 0)
						{
							Vector3 forward = component.PoseOverride.forward;
							Vector3 up = component.PoseOverride.up;
							forward = Vector3.Reflect(forward, component.transform.right);
							up = Vector3.Reflect(up, component.transform.right);
							component.PoseOverride.rotation = Quaternion.LookRotation(forward, up);
						}
						QuickbeltSlots.Add(component);
					}
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = enumerator as IDisposable) != null)
				{
					disposable.Dispose();
				}
			}
			for (int i = 0; i < QuickbeltSlots.Count; i++)
			{
				if (QuickbeltSlots[i].IsPlayer)
				{
					QuickbeltSlots[i].transform.SetParent(Torso);
					QuickbeltSlots[i].QuickbeltRoot = null;
					if (GM.Options.QuickbeltOptions.QuickbeltHandedness > 0)
					{
						QuickbeltSlots[i].transform.localPosition = new Vector3(0f - QuickbeltSlots[i].transform.localPosition.x, QuickbeltSlots[i].transform.localPosition.y, QuickbeltSlots[i].transform.localPosition.z);
					}
				}
			}
			PlayerBackPack[] array = UnityEngine.Object.FindObjectsOfType<PlayerBackPack>();
			for (int j = 0; j < array.Length; j++)
			{
				array[j].RegisterQuickbeltSlots();
			}
			UnityEngine.Object.Destroy(gameObject);
		}
	}
}
