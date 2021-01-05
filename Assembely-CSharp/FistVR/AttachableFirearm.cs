using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class AttachableFirearm : MonoBehaviour
	{
		public AttachableFirearmPhysicalObject Attachment;

		public AttachableFirearmInterface Interface;

		public Transform MuzzlePos;

		public FireArmRoundType RoundType;

		public FVRFireArmMechanicalAccuracyClass AccuracyClass;

		public FVRFireArm OverrideFA;

		[Header("Audio")]
		public FVRFirearmAudioSet AudioClipSet;

		protected SM.AudioSourcePool m_pool_shot;

		protected SM.AudioSourcePool m_pool_tail;

		protected SM.AudioSourcePool m_pool_mechanics;

		protected SM.AudioSourcePool m_pool_handling;

		private bool m_fireWeaponHandlingPerceivableSounds;

		[Header("Recoil")]
		public Transform RecoilingPoseHolder;

		public FVRFireArmRecoilProfile RecoilProfile;

		public bool UsesStockedRecoilProfile;

		public FVRFireArmRecoilProfile RecoilProfileStocked;

		private Vector3 m_recoilPoseHolderLocalPosStart;

		private float m_recoilX;

		private float m_recoilY;

		private float m_recoilLinearZ;

		private Vector2 m_recoilLinearXY = new Vector2(0f, 0f);

		private Vector2 m_recoilLinearXYBase = new Vector2(0f, 0f);

		public bool HasActiveShoulderStock;

		public Transform StockPos;

		private float m_massDriftInstability;

		private Vector2 m_curRecoilMassDrift = Vector2.zero;

		private Vector2 m_tarRecoilMassDrift = Vector2.zero;

		[Header("Muzzle Effects")]
		public MuzzleEffectSize DefaultMuzzleEffectSize = MuzzleEffectSize.Standard;

		public MuzzleEffect[] MuzzleEffects;

		private List<MuzzlePSystem> m_muzzleSystems = new List<MuzzlePSystem>();

		private Quaternion m_storedLocalPoseOverrideRot;

		private FVRSceneSettings m_sceneSettings;

		public FVRSceneSettings SceneSettings => m_sceneSettings;

		public float GetRecoilZ()
		{
			return m_recoilLinearZ;
		}

		public virtual void Awake()
		{
			m_sceneSettings = GM.CurrentSceneSettings;
			m_recoilPoseHolderLocalPosStart = RecoilingPoseHolder.localPosition;
			m_recoilLinearXYBase = new Vector2(m_recoilPoseHolderLocalPosStart.x, m_recoilPoseHolderLocalPosStart.y);
			m_recoilLinearZ = m_recoilPoseHolderLocalPosStart.z;
			if (Attachment != null)
			{
				m_storedLocalPoseOverrideRot = Attachment.PoseOverride.transform.localRotation;
			}
			m_pool_shot = SM.CreatePool(3, 3, FVRPooledAudioType.GunShot);
			if (AudioClipSet == null)
			{
				Debug.Log("Missing audio" + base.gameObject.name);
			}
			m_pool_tail = SM.CreatePool(AudioClipSet.TailConcurrentLimit, AudioClipSet.TailConcurrentLimit, FVRPooledAudioType.GunTail);
			m_pool_handling = SM.CreatePool(3, 3, FVRPooledAudioType.GunHand);
			m_pool_mechanics = SM.CreatePool(3, 3, FVRPooledAudioType.GunMech);
			RegenerateMuzzleEffects();
			if (OverrideFA != null)
			{
				OverrideFA.SetIntegratedAttachableFirearm(this);
			}
			if (m_sceneSettings.UsesWeaponHandlingAISound)
			{
				m_fireWeaponHandlingPerceivableSounds = true;
			}
		}

		public FVRFireArmRecoilProfile GetRecoilProfile()
		{
			if (UsesStockedRecoilProfile)
			{
				if (HasActiveShoulderStock)
				{
					return RecoilProfileStocked;
				}
				return RecoilProfile;
			}
			return RecoilProfile;
		}

		public virtual void ProcessInput(FVRViveHand hand, bool fromInterface, FVRInteractiveObject o)
		{
		}

		public virtual void Fire(FVRFireArmChamber chamber, Transform muzzle, bool doBuzz, FVRFireArm fa, float velMult = 1f)
		{
			if (doBuzz)
			{
			}
			if (fa != null)
			{
				GM.CurrentSceneSettings.OnShotFired(fa);
			}
			GM.CurrentPlayerBody.VisibleEvent(2f);
			for (int i = 0; i < chamber.GetRound().NumProjectiles; i++)
			{
				GameObject gameObject = null;
				float num = chamber.GetRound().ProjectileSpread + AM.GetFireArmMechanicalSpread(AccuracyClass);
				if (chamber.GetRound().BallisticProjectilePrefab != null)
				{
					Vector3 vector = muzzle.forward * 0.005f;
					gameObject = Object.Instantiate(chamber.GetRound().BallisticProjectilePrefab, muzzle.position - vector, muzzle.rotation);
					Vector2 vector2 = Random.insideUnitCircle * num;
					gameObject.transform.Rotate(new Vector3(vector2.x, vector2.y, 0f));
					BallisticProjectile component = gameObject.GetComponent<BallisticProjectile>();
					component.SetSource_IFF(GM.CurrentPlayerBody.GetPlayerIFF());
					component.Fire(component.MuzzleVelocityBase * chamber.ChamberVelocityMultiplier * velMult, gameObject.transform.forward, null);
				}
			}
		}

		public virtual void FireMuzzleSmoke()
		{
			for (int i = 0; i < m_muzzleSystems.Count; i++)
			{
				if (m_muzzleSystems[i].OverridePoint == null)
				{
					m_muzzleSystems[i].PSystem.transform.position = MuzzlePos.position;
				}
				m_muzzleSystems[i].PSystem.Emit(m_muzzleSystems[i].NumParticlesPerShot);
			}
		}

		private void RegenerateMuzzleEffects()
		{
			for (int i = 0; i < m_muzzleSystems.Count; i++)
			{
				Object.Destroy(m_muzzleSystems[i].PSystem);
			}
			m_muzzleSystems.Clear();
			bool flag = false;
			MuzzleEffect[] muzzleEffects = MuzzleEffects;
			for (int j = 0; j < muzzleEffects.Length; j++)
			{
				if (muzzleEffects[j].Entry != 0)
				{
					MuzzleEffectConfig muzzleConfig = FXM.GetMuzzleConfig(muzzleEffects[j].Entry);
					MuzzleEffectSize muzzleEffectSize = muzzleEffects[j].Size;
					if (flag)
					{
						muzzleEffectSize = DefaultMuzzleEffectSize;
					}
					GameObject gameObject = ((!GM.CurrentSceneSettings.IsSceneLowLight) ? Object.Instantiate(muzzleConfig.Prefabs_Highlight[(int)muzzleEffectSize], MuzzlePos.position, MuzzlePos.rotation) : Object.Instantiate(muzzleConfig.Prefabs_Lowlight[(int)muzzleEffectSize], MuzzlePos.position, MuzzlePos.rotation));
					if (muzzleEffects[j].OverridePoint == null)
					{
						gameObject.transform.SetParent(MuzzlePos.transform);
					}
					else
					{
						gameObject.transform.SetParent(muzzleEffects[j].OverridePoint);
						gameObject.transform.localPosition = Vector3.zero;
						gameObject.transform.localEulerAngles = Vector3.zero;
					}
					MuzzlePSystem muzzlePSystem = new MuzzlePSystem();
					muzzlePSystem.PSystem = gameObject.GetComponent<ParticleSystem>();
					muzzlePSystem.OverridePoint = muzzleEffects[j].OverridePoint;
					int index = (int)muzzleEffectSize;
					if (GM.CurrentSceneSettings.IsSceneLowLight)
					{
						muzzlePSystem.NumParticlesPerShot = muzzleConfig.NumParticles_Lowlight[index];
					}
					else
					{
						muzzlePSystem.NumParticlesPerShot = muzzleConfig.NumParticles_Highlight[index];
					}
					m_muzzleSystems.Add(muzzlePSystem);
				}
			}
		}

		public virtual bool IsTwoHandStabilized(FVRViveHand m_hand)
		{
			bool result = false;
			if (m_hand != null && m_hand.OtherHand != null && (m_hand.OtherHand.CurrentInteractable == null || m_hand.OtherHand.CurrentInteractable is Flashlight || m_hand.OtherHand.CurrentInteractable is FVRFireArmMagazine))
			{
				float num = Vector3.Distance(m_hand.PalmTransform.position, m_hand.OtherHand.PalmTransform.position);
				if (num < 0.15f)
				{
					result = true;
				}
			}
			return result;
		}

		protected virtual void Recoil(bool twoHandStabilized, bool foregripStabilized, bool shoulderStabilized)
		{
			float num = 0f;
			float num2 = 0f;
			float num3 = 0f;
			float num4 = 0f;
			FVRFireArmRecoilProfile recoilProfile = GetRecoilProfile();
			float num5 = recoilProfile.MaxVerticalRot;
			float num6 = recoilProfile.MaxHorizontalRot;
			if (foregripStabilized)
			{
				num2 = (num2 = recoilProfile.VerticalRotPerShot * Random.Range(0.4f, 0.45f));
				num = Random.Range(0f - recoilProfile.HorizontalRotPerShot, recoilProfile.HorizontalRotPerShot) * Random.Range(0.4f, 0.45f);
				num3 = recoilProfile.ZLinearPerShot * Random.Range(0.8f, 0.9f);
				num4 = recoilProfile.MassDriftFactors.x;
				num5 *= 0.6f;
				num6 *= 0.9f;
			}
			else if (twoHandStabilized)
			{
				num2 = recoilProfile.VerticalRotPerShot * Random.Range(0.5f, 0.6f);
				num = Random.Range(0f - recoilProfile.HorizontalRotPerShot, recoilProfile.HorizontalRotPerShot) * Random.Range(0.6f, 0.8f);
				num3 = recoilProfile.ZLinearPerShot * Random.Range(0.8f, 0.9f);
				num4 = recoilProfile.MassDriftFactors.y;
				num5 *= 0.8f;
				num6 *= 0.95f;
			}
			else
			{
				num2 = recoilProfile.VerticalRotPerShot * Random.Range(0.9f, 1f);
				num = Random.Range(0f - recoilProfile.HorizontalRotPerShot, recoilProfile.HorizontalRotPerShot) * Random.Range(0.9f, 1f);
				num3 = recoilProfile.ZLinearPerShot * Random.Range(0.9f, 1f);
				num4 = recoilProfile.MassDriftFactors.z;
			}
			if (shoulderStabilized)
			{
				if (foregripStabilized || twoHandStabilized)
				{
					num2 *= 0.3f;
					num = Random.Range(0f - recoilProfile.HorizontalRotPerShot, recoilProfile.HorizontalRotPerShot) * Random.Range(0.2f, 0.3f);
					num3 *= 0.5f;
				}
				else
				{
					num2 *= 0.75f;
					num3 *= 0.7f;
				}
				num5 *= 0.5f;
				num6 *= 0.6f;
				num4 *= recoilProfile.MassDriftFactors.w;
			}
			m_massDriftInstability += num4 * recoilProfile.MassDriftIntensity;
			float num7 = m_recoilX + num2;
			float num8 = 0f;
			if (num7 > num5)
			{
				num8 = (num7 - num5) * 0.25f;
				num7 = num5 - Random.Range(num2 * 0.2f, num2 * 0.8f);
			}
			m_recoilX = Mathf.Clamp(num7, 0f - num5, num5);
			float value = m_recoilY + num + num8 * Mathf.Sign(num);
			m_recoilY = Mathf.Clamp(value, 0f - num6, num6);
			m_recoilLinearZ += num3;
			m_recoilLinearZ = Mathf.Clamp(m_recoilLinearZ, m_recoilPoseHolderLocalPosStart.z, m_recoilPoseHolderLocalPosStart.z + recoilProfile.ZLinearMax);
			float x = Mathf.Abs(m_recoilY) / recoilProfile.MaxHorizontalRot * (Mathf.Abs(num3) / recoilProfile.ZLinearMax) * Mathf.Sign(m_recoilY) * recoilProfile.XYLinearPerShot;
			float y = (0f - Mathf.Abs(m_recoilX) / recoilProfile.MaxVerticalRot) * (Mathf.Abs(num3) / recoilProfile.ZLinearMax) * Mathf.Sign(m_recoilX) * recoilProfile.XYLinearPerShot * 0.1f;
			Vector2 vector = new Vector2(x, y);
			float num9 = recoilProfile.XYLinearMax;
			if (shoulderStabilized)
			{
				num9 *= 0.35f;
			}
			m_recoilLinearXY += vector;
			m_recoilLinearXY = Vector2.ClampMagnitude(m_recoilLinearXY, num9);
		}

		public virtual void Tick(float t, FVRViveHand m_hand)
		{
			if (RecoilingPoseHolder != null && RecoilProfile != null)
			{
				FVRFireArmRecoilProfile recoilProfile = GetRecoilProfile();
				bool flag = IsTwoHandStabilized(m_hand);
				bool flag2 = false;
				bool flag3 = false;
				RecoilingPoseHolder.localEulerAngles = new Vector3(m_recoilX, m_recoilY, 0f);
				RecoilingPoseHolder.localPosition = new Vector3(m_recoilLinearXYBase.x + m_recoilLinearXY.x, m_recoilLinearXYBase.y + m_recoilLinearXY.y, m_recoilLinearZ);
				m_massDriftInstability = Mathf.Clamp(m_massDriftInstability, 0f, recoilProfile.MaxMassDriftMagnitude);
				float num = 0f;
				num = (flag2 ? (recoilProfile.MassDriftFactors.x * recoilProfile.MaxMassMaxRotation) : ((!flag) ? (recoilProfile.MassDriftFactors.z * recoilProfile.MaxMassMaxRotation) : (recoilProfile.MassDriftFactors.y * recoilProfile.MaxMassMaxRotation)));
				if (flag3)
				{
					num *= recoilProfile.MassDriftFactors.w;
				}
				if (m_hand != null)
				{
					m_tarRecoilMassDrift += new Vector2(0f - m_hand.Input.VelAngularLocal.y, 0f - m_hand.Input.VelAngularLocal.x) * Time.deltaTime * 10f;
					m_tarRecoilMassDrift = Vector2.ClampMagnitude(m_tarRecoilMassDrift, recoilProfile.MaxMassMaxRotation);
				}
				else
				{
					m_tarRecoilMassDrift = Vector2.zero;
				}
				m_massDriftInstability = Mathf.Lerp(m_massDriftInstability, 0f, Time.deltaTime * recoilProfile.MassDriftRecoveryFactor);
				m_curRecoilMassDrift = Vector2.Lerp(m_curRecoilMassDrift, m_tarRecoilMassDrift, Time.deltaTime * recoilProfile.MassDriftRecoveryFactor);
				float num2 = 1f;
				float x;
				float y;
				float num3;
				float num4;
				if (flag2)
				{
					x = recoilProfile.RecoveryStabilizationFactors_Foregrip.x;
					y = recoilProfile.RecoveryStabilizationFactors_Foregrip.y;
					num3 = recoilProfile.RecoveryStabilizationFactors_Foregrip.z;
					num4 = recoilProfile.RecoveryStabilizationFactors_Foregrip.w;
					num2 = 1.5f;
				}
				else if (flag)
				{
					x = recoilProfile.RecoveryStabilizationFactors_Twohand.x;
					y = recoilProfile.RecoveryStabilizationFactors_Twohand.y;
					num3 = recoilProfile.RecoveryStabilizationFactors_Twohand.z;
					num4 = recoilProfile.RecoveryStabilizationFactors_Twohand.w;
					num2 = 1.2f;
				}
				else
				{
					x = recoilProfile.RecoveryStabilizationFactors_None.x;
					y = recoilProfile.RecoveryStabilizationFactors_None.y;
					num3 = recoilProfile.RecoveryStabilizationFactors_None.z;
					num4 = recoilProfile.RecoveryStabilizationFactors_None.w;
				}
				if (flag3)
				{
					num3 = 1f;
					num4 = 1f;
					num2 = 4f;
				}
				m_tarRecoilMassDrift = Vector2.Lerp(m_tarRecoilMassDrift, Vector2.zero, Time.deltaTime * recoilProfile.MassDriftRecoveryFactor * num2);
				Vector2 vector = Vector2.ClampMagnitude(m_curRecoilMassDrift * m_massDriftInstability, recoilProfile.MaxMassMaxRotation);
				m_recoilX = Mathf.Lerp(m_recoilX, vector.y, Time.deltaTime * y * recoilProfile.VerticalRotRecovery * Random.Range(0.9f, 1.1f));
				m_recoilY = Mathf.Lerp(m_recoilY, vector.x, Time.deltaTime * x * recoilProfile.HorizontalRotRecovery * Random.Range(0.9f, 1.1f));
				m_recoilLinearZ = Mathf.Lerp(m_recoilLinearZ, m_recoilPoseHolderLocalPosStart.z, Time.deltaTime * recoilProfile.ZLinearRecovery * num4 * Random.Range(0.9f, 1.1f));
				m_recoilLinearXY = Vector2.Lerp(m_recoilLinearXY, Vector2.zero, Time.deltaTime * recoilProfile.XYLinearRecovery * num3 * Random.Range(0.9f, 1.1f));
			}
		}

		public void PlayAudioGunShot(FVRFireArmRound round, FVRSoundEnvironment TailEnvironment)
		{
			Vector3 position = base.transform.position;
			if (AudioClipSet.UsesLowPressureSet)
			{
				if (round.IsHighPressure)
				{
					PlayAudioEvent(FirearmAudioEventType.Shots_Main);
					if (AudioClipSet.UsesTail_Main)
					{
						AudioEvent tailSet = SM.GetTailSet(round.TailClass, TailEnvironment);
						m_pool_tail.PlayClipPitchOverride(tailSet, position, AudioClipSet.TailPitchMod_Main * tailSet.PitchRange.x);
					}
				}
				else
				{
					PlayAudioEvent(FirearmAudioEventType.Shots_LowPressure);
					if (AudioClipSet.UsesTail_Main)
					{
						AudioEvent tailSet2 = SM.GetTailSet(round.TailClass, TailEnvironment);
						m_pool_tail.PlayClipPitchOverride(tailSet2, position, AudioClipSet.TailPitchMod_LowPressure * tailSet2.PitchRange.x);
					}
				}
			}
			else
			{
				PlayAudioEvent(FirearmAudioEventType.Shots_Main);
				if (AudioClipSet.UsesTail_Main)
				{
					AudioEvent tailSet3 = SM.GetTailSet(round.TailClass, TailEnvironment);
					m_pool_tail.PlayClipPitchOverride(tailSet3, position, AudioClipSet.TailPitchMod_Main * tailSet3.PitchRange.x);
				}
			}
			float soundTravelDistanceMultByEnvironment = SM.GetSoundTravelDistanceMultByEnvironment(TailEnvironment);
			int playerIFF = GM.CurrentPlayerBody.GetPlayerIFF();
			if (AudioClipSet.UsesLowPressureSet && !round.IsHighPressure)
			{
				GM.CurrentSceneSettings.OnPerceiveableSound(AudioClipSet.Loudness_Primary * 0.6f, AudioClipSet.Loudness_Primary * 0.6f * soundTravelDistanceMultByEnvironment, base.transform.position, playerIFF);
			}
			else
			{
				GM.CurrentSceneSettings.OnPerceiveableSound(AudioClipSet.Loudness_Primary, AudioClipSet.Loudness_Primary * soundTravelDistanceMultByEnvironment, base.transform.position, playerIFF);
			}
		}

		public void PlayAudioGunShot(bool IsHighPressure, FVRTailSoundClass TailClass, FVRTailSoundClass TailClassSuppressed, FVRSoundEnvironment TailEnvironment)
		{
			Vector3 position = base.transform.position;
			float num = 1f;
			if (IsHighPressure)
			{
				PlayAudioEvent(FirearmAudioEventType.Shots_Main, num);
				if (AudioClipSet.UsesTail_Main)
				{
					AudioEvent tailSet = SM.GetTailSet(TailClass, TailEnvironment);
					m_pool_tail.PlayClipPitchOverride(tailSet, position, AudioClipSet.TailPitchMod_Main * tailSet.PitchRange.x * num);
				}
			}
			else
			{
				PlayAudioEvent(FirearmAudioEventType.Shots_LowPressure, num);
				if (AudioClipSet.UsesTail_Main)
				{
					AudioEvent tailSet2 = SM.GetTailSet(TailClass, TailEnvironment);
					m_pool_tail.PlayClipPitchOverride(tailSet2, position, AudioClipSet.TailPitchMod_LowPressure * tailSet2.PitchRange.x * num);
				}
			}
			float soundTravelDistanceMultByEnvironment = SM.GetSoundTravelDistanceMultByEnvironment(TailEnvironment);
			int playerIFF = GM.CurrentPlayerBody.GetPlayerIFF();
			if (AudioClipSet.UsesLowPressureSet && !IsHighPressure)
			{
				GM.CurrentSceneSettings.OnPerceiveableSound(AudioClipSet.Loudness_Primary * 0.6f, AudioClipSet.Loudness_Primary * 0.6f * soundTravelDistanceMultByEnvironment, base.transform.position, playerIFF);
			}
			else
			{
				GM.CurrentSceneSettings.OnPerceiveableSound(AudioClipSet.Loudness_Primary, AudioClipSet.Loudness_Primary * soundTravelDistanceMultByEnvironment, base.transform.position, playerIFF);
			}
		}

		public void PlayAudioEventHandling(AudioEvent e)
		{
			Vector3 position = base.transform.position;
			m_pool_handling.PlayClip(e, position);
		}

		public void PlayAudioEvent(FirearmAudioEventType eType, float pitchmod = 1f)
		{
			Vector3 position = base.transform.position;
			float num = 1f;
			if (m_fireWeaponHandlingPerceivableSounds)
			{
				num = SM.GetSoundTravelDistanceMultByEnvironment(GM.CurrentPlayerBody.GetCurrentSoundEnvironment());
			}
			int playerIFF = GM.CurrentPlayerBody.GetPlayerIFF();
			switch (eType)
			{
			case FirearmAudioEventType.Shots_Main:
				m_pool_shot.PlayClipPitchOverride(AudioClipSet.Shots_Main, MuzzlePos.position, new Vector2(AudioClipSet.Shots_Main.PitchRange.x * pitchmod, AudioClipSet.Shots_Main.PitchRange.y * pitchmod));
				break;
			case FirearmAudioEventType.Shots_Suppressed:
				m_pool_shot.PlayClip(AudioClipSet.Shots_Suppressed, MuzzlePos.position);
				break;
			case FirearmAudioEventType.Shots_LowPressure:
				m_pool_shot.PlayClip(AudioClipSet.Shots_LowPressure, MuzzlePos.position);
				break;
			case FirearmAudioEventType.BoltRelease:
				m_pool_mechanics.PlayClip(AudioClipSet.BoltRelease, position);
				break;
			case FirearmAudioEventType.BoltSlideBackHeld:
				m_pool_mechanics.PlayClip(AudioClipSet.BoltSlideBackHeld, position);
				if (m_fireWeaponHandlingPerceivableSounds)
				{
					GM.CurrentSceneSettings.OnPerceiveableSound(20f * AudioClipSet.Loudness_OperationMult, 20f * AudioClipSet.Loudness_OperationMult * num * 0.5f, base.transform.position, playerIFF);
				}
				break;
			case FirearmAudioEventType.BoltSlideForwardHeld:
				m_pool_mechanics.PlayClip(AudioClipSet.BoltSlideForwardHeld, position);
				if (m_fireWeaponHandlingPerceivableSounds)
				{
					GM.CurrentSceneSettings.OnPerceiveableSound(20f * AudioClipSet.Loudness_OperationMult, 20f * AudioClipSet.Loudness_OperationMult * num * 0.5f, base.transform.position, playerIFF);
				}
				break;
			case FirearmAudioEventType.BoltSlideBack:
				m_pool_mechanics.PlayClip(AudioClipSet.BoltSlideBack, position);
				if (m_fireWeaponHandlingPerceivableSounds)
				{
					GM.CurrentSceneSettings.OnPerceiveableSound(40f * AudioClipSet.Loudness_OperationMult, 20f * AudioClipSet.Loudness_OperationMult * num * 0.5f, base.transform.position, playerIFF);
				}
				break;
			case FirearmAudioEventType.BoltSlideBackLocked:
				m_pool_mechanics.PlayClip(AudioClipSet.BoltSlideBackLocked, position);
				if (m_fireWeaponHandlingPerceivableSounds)
				{
					GM.CurrentSceneSettings.OnPerceiveableSound(20f * AudioClipSet.Loudness_OperationMult, 20f * AudioClipSet.Loudness_OperationMult * num * 0.5f, base.transform.position, playerIFF);
				}
				break;
			case FirearmAudioEventType.BoltSlideForward:
				m_pool_mechanics.PlayClip(AudioClipSet.BoltSlideForward, position);
				if (m_fireWeaponHandlingPerceivableSounds)
				{
					GM.CurrentSceneSettings.OnPerceiveableSound(20f * AudioClipSet.Loudness_OperationMult, 20f * AudioClipSet.Loudness_OperationMult * num * 0.5f, base.transform.position, playerIFF);
				}
				break;
			case FirearmAudioEventType.CatchOnSear:
				m_pool_mechanics.PlayClip(AudioClipSet.CatchOnSear, position);
				if (m_fireWeaponHandlingPerceivableSounds)
				{
					GM.CurrentSceneSettings.OnPerceiveableSound(20f * AudioClipSet.Loudness_OperationMult, 20f * AudioClipSet.Loudness_OperationMult * num * 0.5f, base.transform.position, playerIFF);
				}
				break;
			case FirearmAudioEventType.ChamberManual:
				m_pool_mechanics.PlayClip(AudioClipSet.ChamberManual, position);
				if (m_fireWeaponHandlingPerceivableSounds)
				{
					GM.CurrentSceneSettings.OnPerceiveableSound(20f * AudioClipSet.Loudness_OperationMult, 20f * AudioClipSet.Loudness_OperationMult * num * 0.5f, base.transform.position, playerIFF);
				}
				break;
			case FirearmAudioEventType.HammerHit:
				m_pool_mechanics.PlayClip(AudioClipSet.HammerHit, position);
				if (m_fireWeaponHandlingPerceivableSounds)
				{
					GM.CurrentSceneSettings.OnPerceiveableSound(40f * AudioClipSet.Loudness_OperationMult, 20f * AudioClipSet.Loudness_OperationMult * num * 0.5f, base.transform.position, playerIFF);
				}
				break;
			case FirearmAudioEventType.Prefire:
				m_pool_mechanics.PlayClip(AudioClipSet.Prefire, position);
				if (m_fireWeaponHandlingPerceivableSounds)
				{
					GM.CurrentSceneSettings.OnPerceiveableSound(15f * AudioClipSet.Loudness_OperationMult, 15f * AudioClipSet.Loudness_OperationMult * num * 0.5f, base.transform.position, playerIFF);
				}
				break;
			case FirearmAudioEventType.HandleGrab:
				m_pool_handling.PlayClip(AudioClipSet.HandleGrab, position);
				if (m_fireWeaponHandlingPerceivableSounds)
				{
					GM.CurrentSceneSettings.OnPerceiveableSound(10f * AudioClipSet.Loudness_OperationMult, 10f * AudioClipSet.Loudness_OperationMult * num * 0.5f, base.transform.position, playerIFF);
				}
				break;
			case FirearmAudioEventType.HandleBack:
				m_pool_handling.PlayClip(AudioClipSet.HandleBack, position);
				if (m_fireWeaponHandlingPerceivableSounds)
				{
					GM.CurrentSceneSettings.OnPerceiveableSound(20f * AudioClipSet.Loudness_OperationMult, 20f * AudioClipSet.Loudness_OperationMult * num * 0.5f, base.transform.position, playerIFF);
				}
				break;
			case FirearmAudioEventType.HandleForward:
				m_pool_handling.PlayClip(AudioClipSet.HandleForward, position);
				if (m_fireWeaponHandlingPerceivableSounds)
				{
					GM.CurrentSceneSettings.OnPerceiveableSound(20f * AudioClipSet.Loudness_OperationMult, 20f * AudioClipSet.Loudness_OperationMult * num * 0.5f, base.transform.position, playerIFF);
				}
				break;
			case FirearmAudioEventType.HandleBackEmpty:
				m_pool_handling.PlayClip(AudioClipSet.HandleBackEmpty, position);
				if (m_fireWeaponHandlingPerceivableSounds)
				{
					GM.CurrentSceneSettings.OnPerceiveableSound(20f * AudioClipSet.Loudness_OperationMult, 20f * AudioClipSet.Loudness_OperationMult * num * 0.5f, base.transform.position, playerIFF);
				}
				break;
			case FirearmAudioEventType.HandleForwardEmpty:
				m_pool_handling.PlayClip(AudioClipSet.HandleForwardEmpty, position);
				if (m_fireWeaponHandlingPerceivableSounds)
				{
					GM.CurrentSceneSettings.OnPerceiveableSound(20f * AudioClipSet.Loudness_OperationMult, 20f * AudioClipSet.Loudness_OperationMult * num * 0.5f, base.transform.position, playerIFF);
				}
				break;
			case FirearmAudioEventType.HandleUp:
				m_pool_handling.PlayClip(AudioClipSet.HandleUp, position);
				if (m_fireWeaponHandlingPerceivableSounds)
				{
					GM.CurrentSceneSettings.OnPerceiveableSound(10f * AudioClipSet.Loudness_OperationMult, 10f * AudioClipSet.Loudness_OperationMult * num * 0.5f, base.transform.position, playerIFF);
				}
				break;
			case FirearmAudioEventType.HandleDown:
				m_pool_handling.PlayClip(AudioClipSet.HandleDown, position);
				if (m_fireWeaponHandlingPerceivableSounds)
				{
					GM.CurrentSceneSettings.OnPerceiveableSound(20f * AudioClipSet.Loudness_OperationMult, 20f * AudioClipSet.Loudness_OperationMult * num * 0.5f, base.transform.position, playerIFF);
				}
				break;
			case FirearmAudioEventType.Safety:
				m_pool_handling.PlayClip(AudioClipSet.Safety, position);
				if (m_fireWeaponHandlingPerceivableSounds)
				{
					GM.CurrentSceneSettings.OnPerceiveableSound(8f * AudioClipSet.Loudness_OperationMult, 8f * AudioClipSet.Loudness_OperationMult * num * 0.5f, base.transform.position, playerIFF);
				}
				break;
			case FirearmAudioEventType.FireSelector:
				m_pool_handling.PlayClip(AudioClipSet.FireSelector, position);
				if (m_fireWeaponHandlingPerceivableSounds)
				{
					GM.CurrentSceneSettings.OnPerceiveableSound(8f * AudioClipSet.Loudness_OperationMult, 8f * AudioClipSet.Loudness_OperationMult * num * 0.5f, base.transform.position, playerIFF);
				}
				break;
			case FirearmAudioEventType.TriggerReset:
				m_pool_handling.PlayClip(AudioClipSet.TriggerReset, position);
				if (m_fireWeaponHandlingPerceivableSounds)
				{
					GM.CurrentSceneSettings.OnPerceiveableSound(3f * AudioClipSet.Loudness_OperationMult, 3f * AudioClipSet.Loudness_OperationMult * num * 0.5f, base.transform.position, playerIFF);
				}
				break;
			case FirearmAudioEventType.BreachOpen:
				m_pool_handling.PlayClip(AudioClipSet.BreachOpen, position);
				if (m_fireWeaponHandlingPerceivableSounds)
				{
					GM.CurrentSceneSettings.OnPerceiveableSound(20f * AudioClipSet.Loudness_OperationMult, 20f * AudioClipSet.Loudness_OperationMult * num * 0.5f, base.transform.position, playerIFF);
				}
				break;
			case FirearmAudioEventType.BreachClose:
				m_pool_handling.PlayClip(AudioClipSet.BreachClose, position);
				if (m_fireWeaponHandlingPerceivableSounds)
				{
					GM.CurrentSceneSettings.OnPerceiveableSound(20f * AudioClipSet.Loudness_OperationMult, 20f * AudioClipSet.Loudness_OperationMult * num * 0.5f, base.transform.position, playerIFF);
				}
				break;
			case FirearmAudioEventType.MagazineIn:
				m_pool_handling.PlayClip(AudioClipSet.MagazineIn, position);
				if (m_fireWeaponHandlingPerceivableSounds)
				{
					GM.CurrentSceneSettings.OnPerceiveableSound(20f * AudioClipSet.Loudness_OperationMult, 20f * AudioClipSet.Loudness_OperationMult * num * 0.5f, base.transform.position, playerIFF);
				}
				break;
			case FirearmAudioEventType.MagazineOut:
				m_pool_handling.PlayClip(AudioClipSet.MagazineOut, position);
				if (m_fireWeaponHandlingPerceivableSounds)
				{
					GM.CurrentSceneSettings.OnPerceiveableSound(10f * AudioClipSet.Loudness_OperationMult, 10f * AudioClipSet.Loudness_OperationMult * num * 0.5f, base.transform.position, playerIFF);
				}
				break;
			case FirearmAudioEventType.MagazineInsertRound:
				m_pool_handling.PlayClip(AudioClipSet.MagazineInsertRound, position);
				if (m_fireWeaponHandlingPerceivableSounds)
				{
					GM.CurrentSceneSettings.OnPerceiveableSound(10f * AudioClipSet.Loudness_OperationMult, 10f * AudioClipSet.Loudness_OperationMult * num * 0.5f, base.transform.position, playerIFF);
				}
				break;
			case FirearmAudioEventType.MagazineEjectRound:
				m_pool_handling.PlayClip(AudioClipSet.MagazineEjectRound, position);
				if (m_fireWeaponHandlingPerceivableSounds)
				{
					GM.CurrentSceneSettings.OnPerceiveableSound(10f * AudioClipSet.Loudness_OperationMult, 10f * AudioClipSet.Loudness_OperationMult * num * 0.5f, base.transform.position, playerIFF);
				}
				break;
			case FirearmAudioEventType.TopCoverRelease:
				m_pool_handling.PlayClip(AudioClipSet.TopCoverRelease, position);
				break;
			case FirearmAudioEventType.TopCoverUp:
				m_pool_handling.PlayClip(AudioClipSet.TopCoverUp, position);
				if (m_fireWeaponHandlingPerceivableSounds)
				{
					GM.CurrentSceneSettings.OnPerceiveableSound(10f * AudioClipSet.Loudness_OperationMult, 10f * AudioClipSet.Loudness_OperationMult * num * 0.5f, base.transform.position, playerIFF);
				}
				break;
			case FirearmAudioEventType.TopCoverDown:
				m_pool_handling.PlayClip(AudioClipSet.TopCoverDown, position);
				if (m_fireWeaponHandlingPerceivableSounds)
				{
					GM.CurrentSceneSettings.OnPerceiveableSound(10f * AudioClipSet.Loudness_OperationMult, 10f * AudioClipSet.Loudness_OperationMult * num * 0.5f, base.transform.position, playerIFF);
				}
				break;
			case FirearmAudioEventType.StockOpen:
				m_pool_handling.PlayClip(AudioClipSet.StockOpen, position);
				if (m_fireWeaponHandlingPerceivableSounds)
				{
					GM.CurrentSceneSettings.OnPerceiveableSound(10f * AudioClipSet.Loudness_OperationMult, 10f * AudioClipSet.Loudness_OperationMult * num * 0.5f, base.transform.position, playerIFF);
				}
				break;
			case FirearmAudioEventType.StockClosed:
				m_pool_handling.PlayClip(AudioClipSet.StockClosed, position);
				if (m_fireWeaponHandlingPerceivableSounds)
				{
					GM.CurrentSceneSettings.OnPerceiveableSound(10f * AudioClipSet.Loudness_OperationMult, 10f * AudioClipSet.Loudness_OperationMult * num * 0.5f, base.transform.position, playerIFF);
				}
				break;
			case FirearmAudioEventType.BipodOpen:
				m_pool_handling.PlayClip(AudioClipSet.BipodOpen, position);
				if (m_fireWeaponHandlingPerceivableSounds)
				{
					GM.CurrentSceneSettings.OnPerceiveableSound(10f * AudioClipSet.Loudness_OperationMult, 10f * AudioClipSet.Loudness_OperationMult * num * 0.5f, base.transform.position, playerIFF);
				}
				break;
			case FirearmAudioEventType.BipodClosed:
				m_pool_handling.PlayClip(AudioClipSet.BipodClosed, position);
				if (m_fireWeaponHandlingPerceivableSounds)
				{
					GM.CurrentSceneSettings.OnPerceiveableSound(10f * AudioClipSet.Loudness_OperationMult, 10f * AudioClipSet.Loudness_OperationMult * num * 0.5f, base.transform.position, playerIFF);
				}
				break;
			case FirearmAudioEventType.BeltGrab:
				m_pool_handling.PlayClip(AudioClipSet.BeltGrab, position);
				if (m_fireWeaponHandlingPerceivableSounds)
				{
					GM.CurrentSceneSettings.OnPerceiveableSound(10f * AudioClipSet.Loudness_OperationMult, 10f * AudioClipSet.Loudness_OperationMult * num * 0.5f, base.transform.position, playerIFF);
				}
				break;
			case FirearmAudioEventType.BeltRelease:
				m_pool_handling.PlayClip(AudioClipSet.BeltRelease, position);
				if (m_fireWeaponHandlingPerceivableSounds)
				{
					GM.CurrentSceneSettings.OnPerceiveableSound(10f * AudioClipSet.Loudness_OperationMult, 10f * AudioClipSet.Loudness_OperationMult * num * 0.5f, base.transform.position, playerIFF);
				}
				break;
			case FirearmAudioEventType.BeltSeat:
				m_pool_handling.PlayClip(AudioClipSet.BeltSeat, position);
				if (m_fireWeaponHandlingPerceivableSounds)
				{
					GM.CurrentSceneSettings.OnPerceiveableSound(10f * AudioClipSet.Loudness_OperationMult, 10f * AudioClipSet.Loudness_OperationMult * num * 0.5f, base.transform.position, playerIFF);
				}
				break;
			}
		}
	}
}
