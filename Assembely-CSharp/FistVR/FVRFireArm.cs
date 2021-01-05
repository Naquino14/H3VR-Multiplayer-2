using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class FVRFireArm : FVRPhysicalObject
	{
		public enum MuzzleState
		{
			None,
			Suppressor,
			MuzzleBreak
		}

		[Serializable]
		public class GasOutEffect
		{
			public GameObject EffectPrefab;

			public ParticleSystem PSystem;

			public Transform Parent;

			public bool FollowsMuzzle;

			public float MaxGasRate;

			[Tooltip("x = unsuppressed, y = suppresed")]
			public Vector2 GasPerEvent = Vector2.zero;

			public float GasDownRate = 0.5f;

			private float m_currentGasRate;

			private float m_timeSinceLastEvent;

			public void GasUpdate(bool BreachOpen)
			{
				if (m_timeSinceLastEvent < 1f)
				{
					m_timeSinceLastEvent += Time.deltaTime;
				}
				if (FollowsMuzzle)
				{
					m_currentGasRate -= GasDownRate * Time.deltaTime;
				}
				else if (BreachOpen)
				{
					m_currentGasRate -= GasDownRate * Time.deltaTime;
				}
				else
				{
					m_currentGasRate -= GasDownRate * Time.deltaTime * 0.02f;
				}
				m_currentGasRate = Mathf.Clamp(m_currentGasRate, 0f, MaxGasRate);
				ParticleSystem.EmissionModule emission = PSystem.emission;
				ParticleSystem.MinMaxCurve rateOverTime = emission.rateOverTime;
				rateOverTime.mode = ParticleSystemCurveMode.Constant;
				rateOverTime.constantMax = m_currentGasRate;
				rateOverTime.constantMin = m_currentGasRate;
				if (FollowsMuzzle)
				{
					if (m_timeSinceLastEvent < 0.25f)
					{
						rateOverTime.constantMax = 0f;
						rateOverTime.constantMin = 0f;
					}
					else
					{
						rateOverTime.constantMax = m_currentGasRate;
						rateOverTime.constantMin = m_currentGasRate;
					}
				}
				else if (BreachOpen)
				{
					rateOverTime.constantMax = m_currentGasRate;
					rateOverTime.constantMin = m_currentGasRate;
					emission.enabled = true;
				}
				else
				{
					rateOverTime.constantMax = 0f;
					rateOverTime.constantMin = 0f;
					emission.enabled = false;
				}
				emission.rateOverTime = rateOverTime;
			}

			public void AddGas(bool isSuppressed)
			{
				m_timeSinceLastEvent = 0f;
				if (isSuppressed)
				{
					m_currentGasRate += GasPerEvent.y;
				}
				else
				{
					m_currentGasRate += GasPerEvent.x;
				}
				m_currentGasRate = Mathf.Clamp(m_currentGasRate, 0f, MaxGasRate);
			}
		}

		[Header("FireArm Config")]
		[SearchableEnum]
		public FireArmMagazineType MagazineType;

		public FireArmClipType ClipType;

		public FireArmRoundType RoundType;

		public FVRFireArmMechanicalAccuracyClass AccuracyClass;

		[Header("Muzzle Params")]
		public Transform MuzzlePos;

		public GameObject Foregrip;

		[Header("NewAudioImplementation")]
		public FVRFirearmAudioSet AudioClipSet;

		protected SM.AudioSourcePool m_pool_shot;

		protected SM.AudioSourcePool m_pool_tail;

		protected SM.AudioSourcePool m_pool_mechanics;

		protected SM.AudioSourcePool m_pool_handling;

		protected SM.AudioSourcePool m_pool_belt;

		private bool m_fireWeaponHandlingPerceivableSounds;

		[Header("Firing/Suppression Params")]
		public MuzzleState DefaultMuzzleState;

		public FVRFireArmMechanicalAccuracyClass DefaultMuzzleDamping;

		protected bool m_isSuppressed;

		protected bool m_isBraked;

		public Suppressor CurrentSuppressor;

		public MuzzleBrake CurrentMuzzleBrake;

		public List<MuzzleDevice> MuzzleDevices = new List<MuzzleDevice>();

		public Transform CurrentMuzzle;

		public AttachableMeleeWeapon CurrentAttachableMeleeWeapon;

		[Header("Magazine Params")]
		public bool UsesMagazines;

		public bool UsesBeltBoxes;

		public Transform MagazineMountPos;

		public Transform MagazineEjectPos;

		public FVRFireArmMagazine Magazine;

		public Transform BeltBoxMountPos;

		public Transform BeltBoxEjectPos;

		public bool UsesBelts;

		public FVRFirearmBeltDisplayData BeltDD;

		public bool ConnectedToBox;

		public bool HasBelt;

		public bool UsesTopCover;

		public bool IsTopCoverUp;

		public bool RequiresBoltBackToSeatBelt;

		[Header("Clip Params")]
		public bool UsesClips;

		public Transform ClipMountPos;

		public Transform ClipEjectPos;

		public FVRFireArmClip Clip;

		public GameObject ClipTrigger;

		private float m_clipEjectDelay;

		[Header("Recoil Params")]
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

		protected float m_ejectDelay;

		private FVRFireArmMagazine m_lastEjectedMag;

		[Header("Muzzle Smoke Params")]
		public MuzzleEffectSize DefaultMuzzleEffectSize = MuzzleEffectSize.Standard;

		public MuzzleEffect[] MuzzleEffects;

		private List<MuzzlePSystem> m_muzzleSystems = new List<MuzzlePSystem>();

		public GasOutEffect[] GasOutEffects;

		public bool ControlsOwnGasOut = true;

		[HideInInspector]
		public bool IsBreachOpenForGasOut = true;

		[Header("Firing/Suppression Params")]
		private AttachableFirearm m_integratedAttachedFirearm;

		private Quaternion m_storedLocalPoseOverrideRot;

		public FVRSceneSettings SceneSettings;

		private float m_internalMechanicalMOA;

		public float StockDist;

		public float ClipEjectDelay => m_clipEjectDelay;

		public float EjectDelay
		{
			get
			{
				return m_ejectDelay;
			}
			set
			{
				m_ejectDelay = value;
			}
		}

		public FVRFireArmMagazine LastEjectedMag => m_lastEjectedMag;

		public bool IsSuppressed()
		{
			if (m_isSuppressed || (DefaultMuzzleState == MuzzleState.Suppressor && MuzzleDevices.Count == 0))
			{
				return true;
			}
			return false;
		}

		public bool IsBraked()
		{
			if (m_isBraked || (DefaultMuzzleState == MuzzleState.MuzzleBreak && MuzzleDevices.Count == 0))
			{
				return true;
			}
			return false;
		}

		public Transform GetMagMountPos(bool isBeltBox)
		{
			if (isBeltBox)
			{
				return BeltBoxMountPos;
			}
			return MagazineMountPos;
		}

		public Transform GetMagEjectPos(bool isBeltBox)
		{
			if (isBeltBox)
			{
				return BeltBoxEjectPos;
			}
			return MagazineEjectPos;
		}

		public float GetRecoilZ()
		{
			return m_recoilLinearZ;
		}

		public AttachableFirearm GetIntegratedAttachableFirearm()
		{
			return m_integratedAttachedFirearm;
		}

		public void SetIntegratedAttachableFirearm(AttachableFirearm a)
		{
			m_integratedAttachedFirearm = a;
		}

		protected override void Awake()
		{
			base.Awake();
			CurrentMuzzle = MuzzlePos;
			m_internalMechanicalMOA = AM.GetFireArmMechanicalSpread(AccuracyClass);
			SceneSettings = GM.CurrentSceneSettings;
			m_recoilPoseHolderLocalPosStart = RecoilingPoseHolder.localPosition;
			m_recoilLinearXYBase = new Vector2(m_recoilPoseHolderLocalPosStart.x, m_recoilPoseHolderLocalPosStart.y);
			m_recoilLinearZ = m_recoilPoseHolderLocalPosStart.z;
			RegenerateMuzzleEffects(null);
			for (int i = 0; i < MuzzleEffects.Length; i++)
			{
			}
			for (int j = 0; j < GasOutEffects.Length; j++)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(GasOutEffects[j].EffectPrefab, GasOutEffects[j].Parent.position, GasOutEffects[j].Parent.rotation);
				gameObject.transform.SetParent(GasOutEffects[j].Parent.transform);
				GasOutEffects[j].PSystem = gameObject.GetComponent<ParticleSystem>();
			}
			m_storedLocalPoseOverrideRot = PoseOverride.transform.localRotation;
			m_pool_shot = SM.CreatePool(3, 3, FVRPooledAudioType.GunShot);
			if (AudioClipSet == null)
			{
				Debug.Log("Missing audio" + base.gameObject.name);
			}
			m_pool_tail = SM.CreatePool(AudioClipSet.TailConcurrentLimit, AudioClipSet.TailConcurrentLimit, FVRPooledAudioType.GunTail);
			m_pool_handling = SM.CreatePool(3, 3, FVRPooledAudioType.GunHand);
			m_pool_mechanics = SM.CreatePool(3, 3, FVRPooledAudioType.GunMech);
			if (AudioClipSet.BeltSettlingLimit > 0)
			{
				m_pool_belt = SM.CreatePool(AudioClipSet.BeltSettlingLimit, AudioClipSet.BeltSettlingLimit, FVRPooledAudioType.GunMech);
			}
			if (GM.CurrentSceneSettings.UsesWeaponHandlingAISound)
			{
				m_fireWeaponHandlingPerceivableSounds = true;
			}
		}

		protected override void FVRUpdate()
		{
			base.FVRUpdate();
			if (ControlsOwnGasOut)
			{
				for (int i = 0; i < GasOutEffects.Length; i++)
				{
					GasOutEffects[i].GasUpdate(IsBreachOpenForGasOut);
				}
			}
			if (UsesBelts)
			{
				BeltDD.UpdateBelt();
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

		public virtual void FireMuzzleSmoke()
		{
			if (GM.CurrentSceneSettings.IsSceneLowLight)
			{
				if (IsSuppressed())
				{
					FXM.InitiateMuzzleFlash(GetMuzzle().position, GetMuzzle().forward, 0.25f, new Color(1f, 0.9f, 0.77f), 0.5f);
				}
				else
				{
					FXM.InitiateMuzzleFlash(GetMuzzle().position, GetMuzzle().forward, 1f, new Color(1f, 0.9f, 0.77f), 1f);
				}
			}
			for (int i = 0; i < m_muzzleSystems.Count; i++)
			{
				if (m_muzzleSystems[i].OverridePoint == null)
				{
					m_muzzleSystems[i].PSystem.transform.position = GetMuzzle().position;
				}
				m_muzzleSystems[i].PSystem.Emit(m_muzzleSystems[i].NumParticlesPerShot);
			}
			for (int j = 0; j < GasOutEffects.Length; j++)
			{
				GasOutEffects[j].AddGas(IsSuppressed());
			}
		}

		public virtual void FireMuzzleSmoke(int i)
		{
			if (GM.CurrentSceneSettings.IsSceneLowLight)
			{
				if (IsSuppressed())
				{
					FXM.InitiateMuzzleFlash(GetMuzzle().position, GetMuzzle().forward, 0.25f, new Color(1f, 0.9f, 0.77f), 0.5f);
				}
				else
				{
					FXM.InitiateMuzzleFlash(GetMuzzle().position, GetMuzzle().forward, 1f, new Color(1f, 0.9f, 0.77f), 1f);
				}
			}
			if (m_muzzleSystems[i].OverridePoint == null)
			{
				m_muzzleSystems[i].PSystem.transform.position = GetMuzzle().position;
			}
			m_muzzleSystems[i].PSystem.Emit(m_muzzleSystems[i].NumParticlesPerShot);
		}

		public virtual void AddGas(int i)
		{
			GasOutEffects[i].AddGas(IsSuppressed());
		}

		public void RattleSuppresor()
		{
			if (!(CurrentSuppressor != null))
			{
				return;
			}
			CurrentSuppressor.ShotEffect();
			if (CurrentSuppressor.CatchRot < 90f && !CurrentSuppressor.IsIntegrate)
			{
				CurrentSuppressor.CatchRot -= UnityEngine.Random.Range(1f, 9f);
				CurrentSuppressor.CatchRot = Mathf.Clamp(CurrentSuppressor.CatchRot, 0f, 360f);
				CurrentSuppressor.transform.localEulerAngles = new Vector3(0f, 0f, CurrentSuppressor.CatchRot);
				if (CurrentSuppressor.CatchRot < 1f)
				{
					Suppressor currentSuppressor = CurrentSuppressor;
					CurrentSuppressor.DetachFromMount();
					currentSuppressor.RootRigidbody.AddForceAtPosition((MuzzlePos.forward + UnityEngine.Random.onUnitSphere * 0.02f) * 0.3f, MuzzlePos.position + UnityEngine.Random.onUnitSphere * 0.002f, ForceMode.Impulse);
				}
			}
		}

		public virtual Transform GetMuzzle()
		{
			return CurrentMuzzle;
		}

		public void RegisterMuzzleDevice(MuzzleDevice m)
		{
			if (m != null && !MuzzleDevices.Contains(m))
			{
				MuzzleDevices.Add(m);
				UpdateCurrentMuzzle();
			}
		}

		public void DeRegisterMuzzleDevice(MuzzleDevice m)
		{
			if (m != null && MuzzleDevices.Contains(m))
			{
				MuzzleDevices.Remove(m);
				UpdateCurrentMuzzle();
			}
		}

		public float GetCombinedMuzzleDeviceAccuracy()
		{
			float num = 0f;
			for (int i = 0; i < MuzzleDevices.Count; i++)
			{
				num += MuzzleDevices[i].GetMechanicalAccuracy();
			}
			return num;
		}

		public float GetCombinedFixedDrop(FVRFireArmMechanicalAccuracyClass c)
		{
			if (MuzzleDevices.Count < 1)
			{
				return 0f;
			}
			float num = 1f;
			if (IsBraked() || IsSuppressed())
			{
				num = AM.GetDropMult(c);
			}
			for (int i = 0; i < MuzzleDevices.Count; i++)
			{
				num *= MuzzleDevices[i].GetDropMult(this);
			}
			return Mathf.Clamp(num - 1f, 0f, num);
		}

		public Vector2 GetCombinedFixedDrift(FVRFireArmMechanicalAccuracyClass c)
		{
			if (MuzzleDevices.Count < 1)
			{
				return Vector2.zero;
			}
			Vector2 vector = new Vector2(1f, 1f);
			if (IsBraked() || IsSuppressed())
			{
				vector = new Vector2(AM.GetDriftMult(c), AM.GetDriftMult(c));
			}
			Vector2 driftMult = MuzzleDevices[MuzzleDevices.Count - 1].GetDriftMult(this);
			return new Vector2(vector.x * driftMult.x, vector.y * driftMult.y);
		}

		public float GetReductionFactor()
		{
			float num = 1f;
			if (MuzzleDevices.Count > 0)
			{
				return AM.GetRecoilMult(MuzzleDevices[MuzzleDevices.Count - 1].MechanicalAccuracy);
			}
			return AM.GetRecoilMult(DefaultMuzzleDamping);
		}

		public void RegisterAttachedMeleeWeapon(AttachableMeleeWeapon a)
		{
			CurrentAttachableMeleeWeapon = a;
			if (a == null)
			{
				MP.CanNewStab = false;
				MP.HighDamageBCP = Vector3.zero;
				MP.StabDamageBCP = Vector3.zero;
				MP.TearDamageBCP = Vector3.zero;
				MP.HighDamageColliders.Clear();
				MP.HighDamageVectors.Clear();
				MP.StabColliders.Clear();
				MP.StabDirection = null;
				return;
			}
			MP.HighDamageBCP = a.MP.HighDamageBCP;
			MP.StabDamageBCP = a.MP.StabDamageBCP;
			MP.TearDamageBCP = a.MP.TearDamageBCP;
			MP.HighDamageColliders.Clear();
			MP.HighDamageVectors.Clear();
			MP.StabColliders.Clear();
			MP.StabDirection = a.MP.StabDirection;
			MP.CanNewStab = a.MP.CanNewStab;
			MP.BladeLength = a.MP.BladeLength;
			MP.MassWhileStabbed = a.MP.MassWhileStabbed;
			MP.StabAngularThreshold = a.MP.StabAngularThreshold;
			MP.StabVelocityRequirement = a.MP.StabVelocityRequirement;
			MP.CanTearOut = a.MP.CanTearOut;
			MP.TearOutVelThreshold = a.MP.TearOutVelThreshold;
			for (int i = 0; i < a.MP.HighDamageColliders.Count; i++)
			{
				MP.HighDamageColliders.Add(a.MP.HighDamageColliders[i]);
			}
			for (int j = 0; j < a.MP.HighDamageVectors.Count; j++)
			{
				MP.HighDamageVectors.Add(a.MP.HighDamageVectors[j]);
			}
			for (int k = 0; k < a.MP.StabColliders.Count; k++)
			{
				MP.StabColliders.Add(a.MP.StabColliders[k]);
			}
		}

		public void RegisterSuppressor(Suppressor s)
		{
			CurrentSuppressor = s;
			if (s == null)
			{
				m_isSuppressed = false;
			}
			else
			{
				m_isSuppressed = true;
			}
		}

		public void RegisterMuzzleBrake(MuzzleBrake m)
		{
			CurrentMuzzleBrake = m;
			if (m == null)
			{
				m_isBraked = false;
			}
			else
			{
				m_isBraked = true;
			}
		}

		private void UpdateCurrentMuzzle()
		{
			if (MuzzleDevices.Count == 0)
			{
				CurrentMuzzle = MuzzlePos;
				RegenerateMuzzleEffects(null);
			}
			else
			{
				CurrentMuzzle = MuzzleDevices[MuzzleDevices.Count - 1].Muzzle;
				RegenerateMuzzleEffects(MuzzleDevices[MuzzleDevices.Count - 1]);
			}
			UpdateGasOutLocation();
		}

		private void RegenerateMuzzleEffects(MuzzleDevice m)
		{
			for (int i = 0; i < m_muzzleSystems.Count; i++)
			{
				UnityEngine.Object.Destroy(m_muzzleSystems[i].PSystem);
			}
			m_muzzleSystems.Clear();
			bool flag = false;
			MuzzleEffect[] muzzleEffects = MuzzleEffects;
			if (m != null)
			{
				muzzleEffects = m.MuzzleEffects;
				if (!m.ForcesEffectSize)
				{
					flag = true;
				}
			}
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
					GameObject gameObject = ((!GM.CurrentSceneSettings.IsSceneLowLight) ? UnityEngine.Object.Instantiate(muzzleConfig.Prefabs_Highlight[(int)muzzleEffectSize], MuzzlePos.position, MuzzlePos.rotation) : UnityEngine.Object.Instantiate(muzzleConfig.Prefabs_Lowlight[(int)muzzleEffectSize], MuzzlePos.position, MuzzlePos.rotation));
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

		private void UpdateGasOutLocation()
		{
			for (int i = 0; i < GasOutEffects.Length; i++)
			{
				if (GasOutEffects[i].FollowsMuzzle)
				{
					GasOutEffects[i].PSystem.transform.position = GetMuzzle().position;
				}
			}
		}

		public override void SetQuickBeltSlot(FVRQuickBeltSlot slot)
		{
			if (slot != null && !base.IsHeld)
			{
				if (Magazine != null)
				{
					Magazine.SetAllCollidersToLayer(triggersToo: false, "NoCol");
				}
			}
			else if (Magazine != null)
			{
				Magazine.SetAllCollidersToLayer(triggersToo: false, "Default");
			}
			base.SetQuickBeltSlot(slot);
		}

		public virtual bool IsTwoHandStabilized()
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

		public virtual bool IsShoulderStabilized()
		{
			if (!HasActiveShoulderStock || StockPos == null)
			{
				return false;
			}
			Vector3 vA = GM.CurrentPlayerBody.Torso.position + GM.CurrentPlayerBody.Torso.right * 0.15f + GM.CurrentPlayerBody.Torso.forward * 0.1f - GM.CurrentPlayerBody.Torso.up * 0.1f;
			Vector3 vB = GM.CurrentPlayerBody.Torso.position + GM.CurrentPlayerBody.Torso.right * -0.15f + GM.CurrentPlayerBody.Torso.forward * 0.1f - GM.CurrentPlayerBody.Torso.up * 0.1f;
			Vector3 closestValidPoint = GetClosestValidPoint(vA, vB, StockPos.position);
			Vector3 vA2 = GM.CurrentPlayerBody.Torso.position + GM.CurrentPlayerBody.Torso.right * 0.15f + GM.CurrentPlayerBody.Torso.forward * 0.1f + GM.CurrentPlayerBody.Torso.up * 0.1f;
			Vector3 vB2 = GM.CurrentPlayerBody.Torso.position + GM.CurrentPlayerBody.Torso.right * -0.15f + GM.CurrentPlayerBody.Torso.forward * 0.1f + GM.CurrentPlayerBody.Torso.up * 0.1f;
			Vector3 closestValidPoint2 = GetClosestValidPoint(vA2, vB2, StockPos.position);
			Vector3 closestValidPoint3 = GetClosestValidPoint(closestValidPoint, closestValidPoint2, StockPos.position);
			if ((StockDist = Vector3.Distance(closestValidPoint3, StockPos.position)) <= 0.2f)
			{
				return true;
			}
			return false;
		}

		public virtual bool IsForegripStabilized()
		{
			if (base.AltGrip == null)
			{
				return false;
			}
			return true;
		}

		public virtual void Recoil(bool twoHandStabilized, bool foregripStabilized, bool shoulderStabilized, FVRFireArmRecoilProfile overrideprofile = null, float VerticalRecoilMult = 1f)
		{
			float num = 0f;
			float num2 = 0f;
			float num3 = 0f;
			float num4 = 0f;
			FVRFireArmRecoilProfile fVRFireArmRecoilProfile = null;
			fVRFireArmRecoilProfile = ((!(overrideprofile != null)) ? GetRecoilProfile() : overrideprofile);
			float num5 = fVRFireArmRecoilProfile.MaxVerticalRot;
			float num6 = fVRFireArmRecoilProfile.MaxHorizontalRot;
			if (base.Bipod != null && base.Bipod.IsBipodActive)
			{
				num2 = fVRFireArmRecoilProfile.VerticalRotPerShot_Bipodded * UnityEngine.Random.Range(0.8f, 1f);
				num = fVRFireArmRecoilProfile.HorizontalRotPerShot_Bipodded * UnityEngine.Random.Range(0.8f, 1f);
				num3 = fVRFireArmRecoilProfile.ZLinearPerShot * UnityEngine.Random.Range(0.8f, 1f);
				num4 = 0f;
				num5 = fVRFireArmRecoilProfile.MaxVerticalRot_Bipodded;
				num6 = fVRFireArmRecoilProfile.MaxHorizontalRot_Bipodded;
			}
			else if (foregripStabilized)
			{
				num2 = (num2 = fVRFireArmRecoilProfile.VerticalRotPerShot * UnityEngine.Random.Range(0.4f, 0.45f));
				num = UnityEngine.Random.Range(0f - fVRFireArmRecoilProfile.HorizontalRotPerShot, fVRFireArmRecoilProfile.HorizontalRotPerShot) * UnityEngine.Random.Range(0.4f, 0.45f);
				num3 = fVRFireArmRecoilProfile.ZLinearPerShot * UnityEngine.Random.Range(0.8f, 0.9f);
				num4 = fVRFireArmRecoilProfile.MassDriftFactors.x;
				num5 *= 0.6f;
				num6 *= 0.9f;
			}
			else if (twoHandStabilized)
			{
				num2 = fVRFireArmRecoilProfile.VerticalRotPerShot * UnityEngine.Random.Range(0.5f, 0.6f);
				num = UnityEngine.Random.Range(0f - fVRFireArmRecoilProfile.HorizontalRotPerShot, fVRFireArmRecoilProfile.HorizontalRotPerShot) * UnityEngine.Random.Range(0.6f, 0.8f);
				num3 = fVRFireArmRecoilProfile.ZLinearPerShot * UnityEngine.Random.Range(0.8f, 0.9f);
				num4 = fVRFireArmRecoilProfile.MassDriftFactors.y;
				num5 *= 0.8f;
				num6 *= 0.95f;
			}
			else
			{
				num2 = fVRFireArmRecoilProfile.VerticalRotPerShot * UnityEngine.Random.Range(0.9f, 1f);
				num = UnityEngine.Random.Range(0f - fVRFireArmRecoilProfile.HorizontalRotPerShot, fVRFireArmRecoilProfile.HorizontalRotPerShot) * UnityEngine.Random.Range(0.9f, 1f);
				num3 = fVRFireArmRecoilProfile.ZLinearPerShot * UnityEngine.Random.Range(0.9f, 1f);
				num4 = fVRFireArmRecoilProfile.MassDriftFactors.z;
			}
			if (shoulderStabilized)
			{
				if (foregripStabilized || twoHandStabilized)
				{
					num2 *= 0.3f;
					num = UnityEngine.Random.Range(0f - fVRFireArmRecoilProfile.HorizontalRotPerShot, fVRFireArmRecoilProfile.HorizontalRotPerShot) * UnityEngine.Random.Range(0.2f, 0.3f);
					num3 *= 0.5f;
				}
				else
				{
					num2 *= 0.75f;
					num3 *= 0.7f;
				}
				num5 *= 0.5f;
				num6 *= 0.6f;
				num4 *= fVRFireArmRecoilProfile.MassDriftFactors.w;
			}
			float reductionFactor = GetReductionFactor();
			num2 *= reductionFactor;
			num *= reductionFactor;
			num3 *= reductionFactor;
			num2 *= VerticalRecoilMult;
			m_massDriftInstability += num4 * fVRFireArmRecoilProfile.MassDriftIntensity;
			float num7 = m_recoilX + num2;
			float num8 = 0f;
			if (num7 > num5)
			{
				num8 = (num7 - num5) * 0.25f;
				num7 = num5 - UnityEngine.Random.Range(num2 * 0.2f, num2 * 0.8f);
			}
			m_recoilX = Mathf.Clamp(num7, 0f - num5, num5);
			float value = m_recoilY + num + num8 * Mathf.Sign(num);
			m_recoilY = Mathf.Clamp(value, 0f - num6, num6);
			m_recoilLinearZ += num3;
			m_recoilLinearZ = Mathf.Clamp(m_recoilLinearZ, m_recoilPoseHolderLocalPosStart.z, m_recoilPoseHolderLocalPosStart.z + fVRFireArmRecoilProfile.ZLinearMax);
			float x = Mathf.Abs(m_recoilY) / fVRFireArmRecoilProfile.MaxHorizontalRot * (Mathf.Abs(num3) / fVRFireArmRecoilProfile.ZLinearMax) * Mathf.Sign(m_recoilY) * fVRFireArmRecoilProfile.XYLinearPerShot;
			float y = (0f - Mathf.Abs(m_recoilX) / fVRFireArmRecoilProfile.MaxVerticalRot) * (Mathf.Abs(num3) / fVRFireArmRecoilProfile.ZLinearMax) * Mathf.Sign(m_recoilX) * fVRFireArmRecoilProfile.XYLinearPerShot * 0.1f;
			Vector2 vector = new Vector2(x, y);
			float num9 = fVRFireArmRecoilProfile.XYLinearMax;
			if (shoulderStabilized)
			{
				num9 *= 0.35f;
			}
			m_recoilLinearXY += vector;
			m_recoilLinearXY = Vector2.ClampMagnitude(m_recoilLinearXY, num9);
		}

		public virtual void Fire(FVRFireArmChamber chamber, Transform muzzle, bool doBuzz, float velMult = 1f)
		{
			if (doBuzz && m_hand != null)
			{
				m_hand.Buzz(m_hand.Buzzer.Buzz_GunShot);
				if (base.AltGrip != null && base.AltGrip.m_hand != null)
				{
					base.AltGrip.m_hand.Buzz(m_hand.Buzzer.Buzz_GunShot);
				}
			}
			GM.CurrentSceneSettings.OnShotFired(this);
			if (IsSuppressed())
			{
				GM.CurrentPlayerBody.VisibleEvent(0.1f);
			}
			else
			{
				GM.CurrentPlayerBody.VisibleEvent(2f);
			}
			float chamberVelMult = AM.GetChamberVelMult(chamber.RoundType, Vector3.Distance(chamber.transform.position, muzzle.position));
			float num = GetCombinedFixedDrop(AccuracyClass) * 0.0166667f;
			Vector2 vector = GetCombinedFixedDrift(AccuracyClass) * 0.0166667f;
			for (int i = 0; i < chamber.GetRound().NumProjectiles; i++)
			{
				GameObject gameObject = null;
				float num2 = chamber.GetRound().ProjectileSpread + m_internalMechanicalMOA + GetCombinedMuzzleDeviceAccuracy();
				if (chamber.GetRound().BallisticProjectilePrefab != null)
				{
					Vector3 vector2 = muzzle.forward * 0.005f;
					gameObject = UnityEngine.Object.Instantiate(chamber.GetRound().BallisticProjectilePrefab, muzzle.position - vector2, muzzle.rotation);
					Vector2 vector3 = (UnityEngine.Random.insideUnitCircle + UnityEngine.Random.insideUnitCircle + UnityEngine.Random.insideUnitCircle) * 0.333333343f * num2;
					gameObject.transform.Rotate(new Vector3(vector3.x + vector.y + num, vector3.y + vector.x, 0f));
					BallisticProjectile component = gameObject.GetComponent<BallisticProjectile>();
					component.Fire(component.MuzzleVelocityBase * chamber.ChamberVelocityMultiplier * velMult * chamberVelMult, gameObject.transform.forward, this);
				}
			}
		}

		public override void BeginInteraction(FVRViveHand hand)
		{
			base.BeginInteraction(hand);
			if (GM.Options.ControlOptions.UseGunRigMode2)
			{
				PoseOverride.localRotation = Quaternion.Inverse(hand.m_storedInitialPointingTransformDir);
			}
			else if ((hand.CMode == ControlMode.Oculus || hand.CMode == ControlMode.Index) && PoseOverride_Touch != null)
			{
				PoseOverride.localRotation = PoseOverride_Touch.localRotation;
			}
			else
			{
				PoseOverride.localRotation = m_storedLocalPoseOverrideRot;
			}
		}

		public override void EndInteraction(FVRViveHand hand)
		{
			base.EndInteraction(hand);
		}

		public override void EndInteractionIntoInventorySlot(FVRViveHand hand, FVRQuickBeltSlot slot)
		{
			base.EndInteractionIntoInventorySlot(hand, slot);
		}

		protected override void FVRFixedUpdate()
		{
			base.FVRFixedUpdate();
			if (RecoilingPoseHolder != null && RecoilProfile != null)
			{
				FVRFireArmRecoilProfile recoilProfile = GetRecoilProfile();
				bool flag = IsTwoHandStabilized();
				bool flag2 = IsForegripStabilized();
				bool flag3 = IsShoulderStabilized();
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
				m_recoilX = Mathf.Lerp(m_recoilX, vector.y, Time.deltaTime * y * recoilProfile.VerticalRotRecovery * UnityEngine.Random.Range(0.9f, 1.1f));
				m_recoilY = Mathf.Lerp(m_recoilY, vector.x, Time.deltaTime * x * recoilProfile.HorizontalRotRecovery * UnityEngine.Random.Range(0.9f, 1.1f));
				m_recoilLinearZ = Mathf.Lerp(m_recoilLinearZ, m_recoilPoseHolderLocalPosStart.z, Time.deltaTime * recoilProfile.ZLinearRecovery * num4 * UnityEngine.Random.Range(0.9f, 1.1f));
				m_recoilLinearXY = Vector2.Lerp(m_recoilLinearXY, Vector2.zero, Time.deltaTime * recoilProfile.XYLinearRecovery * num3 * UnityEngine.Random.Range(0.9f, 1.1f));
				if (base.Bipod != null)
				{
					base.Bipod.RecoilFactor = m_recoilLinearZ;
				}
			}
			if (m_ejectDelay > 0f)
			{
				m_ejectDelay -= Time.deltaTime;
			}
			else
			{
				m_ejectDelay = 0f;
			}
			if (m_clipEjectDelay > 0f)
			{
				m_clipEjectDelay -= Time.deltaTime;
			}
			else
			{
				m_clipEjectDelay = 0f;
			}
		}

		public virtual void LoadMag(FVRFireArmMagazine mag)
		{
			if (!(Magazine == null) || !(mag != null))
			{
				return;
			}
			m_lastEjectedMag = null;
			Magazine = mag;
			if (m_hand != null)
			{
				m_hand.Buzz(m_hand.Buzzer.Buzz_BeginInteraction);
				if (Magazine.m_hand != null)
				{
					Magazine.m_hand.Buzz(m_hand.Buzzer.Buzz_BeginInteraction);
				}
			}
			if (mag.UsesOverrideInOut)
			{
				PlayAudioEventHandling(mag.ProfileOverride.MagazineIn);
			}
			else
			{
				PlayAudioEvent(FirearmAudioEventType.MagazineIn);
			}
		}

		public virtual Transform GetMagMountingTransform()
		{
			return base.transform;
		}

		public virtual void EjectMag()
		{
			if (Magazine != null)
			{
				if (Magazine.UsesOverrideInOut)
				{
					PlayAudioEventHandling(Magazine.ProfileOverride.MagazineOut);
				}
				else
				{
					PlayAudioEvent(FirearmAudioEventType.MagazineOut);
				}
				m_lastEjectedMag = Magazine;
				m_ejectDelay = 0.4f;
				if (m_hand != null)
				{
					m_hand.Buzz(m_hand.Buzzer.Buzz_BeginInteraction);
				}
				Magazine.Release();
				if (Magazine.m_hand != null)
				{
					Magazine.m_hand.Buzz(m_hand.Buzzer.Buzz_BeginInteraction);
				}
				Magazine = null;
			}
		}

		public virtual void LoadClip(FVRFireArmClip clip)
		{
			if (!(Clip == null) || !(clip != null))
			{
				return;
			}
			Clip = clip;
			if (m_hand != null)
			{
				m_hand.Buzz(m_hand.Buzzer.Buzz_BeginInteraction);
				if (Clip.m_hand != null)
				{
					Clip.m_hand.Buzz(m_hand.Buzzer.Buzz_BeginInteraction);
				}
			}
		}

		public virtual void EjectClip()
		{
			if (Clip != null)
			{
				m_clipEjectDelay = 0.4f;
				if (m_hand != null)
				{
					m_hand.Buzz(m_hand.Buzzer.Buzz_BeginInteraction);
				}
				Clip.Release();
				if (Clip.m_hand != null)
				{
					Clip.m_hand.Buzz(m_hand.Buzzer.Buzz_BeginInteraction);
				}
				Clip = null;
			}
		}

		public void PlayShotTail(FVRTailSoundClass tailClass, FVRSoundEnvironment TailEnvironment, float globalLoudnessMultiplier = 1f)
		{
			AudioEvent tailSet = SM.GetTailSet(tailClass, TailEnvironment);
			Vector3 position = base.transform.position;
			m_pool_tail.PlayClipVolumePitchOverride(tailSet, position, tailSet.VolumeRange * globalLoudnessMultiplier, AudioClipSet.TailPitchMod_Suppressed * tailSet.PitchRange.x);
		}

		public void PlayAudioGunShot(FVRFireArmRound round, FVRSoundEnvironment TailEnvironment, float globalLoudnessMultiplier = 1f)
		{
			Vector3 position = base.transform.position;
			FVRTailSoundClass tailClass = FVRTailSoundClass.Tiny;
			if (IsSuppressed())
			{
				PlayAudioEvent(FirearmAudioEventType.Shots_Suppressed);
				if (AudioClipSet.UsesTail_Suppressed)
				{
					tailClass = round.TailClassSuppressed;
					AudioEvent tailSet = SM.GetTailSet(round.TailClassSuppressed, TailEnvironment);
					m_pool_tail.PlayClipVolumePitchOverride(tailSet, position, tailSet.VolumeRange * globalLoudnessMultiplier, AudioClipSet.TailPitchMod_Suppressed * tailSet.PitchRange.x);
				}
			}
			else if (AudioClipSet.UsesLowPressureSet)
			{
				if (round.IsHighPressure)
				{
					PlayAudioEvent(FirearmAudioEventType.Shots_Main);
					if (AudioClipSet.UsesTail_Main)
					{
						tailClass = round.TailClass;
						AudioEvent tailSet2 = SM.GetTailSet(round.TailClass, TailEnvironment);
						m_pool_tail.PlayClipVolumePitchOverride(tailSet2, position, tailSet2.VolumeRange * globalLoudnessMultiplier, AudioClipSet.TailPitchMod_Main * tailSet2.PitchRange.x);
					}
				}
				else
				{
					PlayAudioEvent(FirearmAudioEventType.Shots_LowPressure);
					if (AudioClipSet.UsesTail_Main)
					{
						tailClass = round.TailClass;
						AudioEvent tailSet3 = SM.GetTailSet(round.TailClass, TailEnvironment);
						m_pool_tail.PlayClipVolumePitchOverride(tailSet3, position, tailSet3.VolumeRange * globalLoudnessMultiplier, AudioClipSet.TailPitchMod_LowPressure * tailSet3.PitchRange.x);
					}
				}
			}
			else
			{
				PlayAudioEvent(FirearmAudioEventType.Shots_Main);
				if (AudioClipSet.UsesTail_Main)
				{
					tailClass = round.TailClass;
					AudioEvent tailSet4 = SM.GetTailSet(round.TailClass, TailEnvironment);
					m_pool_tail.PlayClipVolumePitchOverride(tailSet4, position, tailSet4.VolumeRange * globalLoudnessMultiplier, AudioClipSet.TailPitchMod_Main * tailSet4.PitchRange.x);
				}
			}
			float soundTravelDistanceMultByEnvironment = SM.GetSoundTravelDistanceMultByEnvironment(TailEnvironment);
			int playerIFF = GM.CurrentPlayerBody.GetPlayerIFF();
			if (IsSuppressed())
			{
				GM.CurrentSceneSettings.OnPerceiveableSound(AudioClipSet.Loudness_Suppressed, AudioClipSet.Loudness_Suppressed * soundTravelDistanceMultByEnvironment * 0.5f * globalLoudnessMultiplier, base.transform.position, playerIFF);
			}
			else if (AudioClipSet.UsesLowPressureSet && !round.IsHighPressure)
			{
				GM.CurrentSceneSettings.OnPerceiveableSound(AudioClipSet.Loudness_Primary * 0.6f, AudioClipSet.Loudness_Primary * 0.6f * soundTravelDistanceMultByEnvironment * globalLoudnessMultiplier, base.transform.position, playerIFF);
			}
			else
			{
				GM.CurrentSceneSettings.OnPerceiveableSound(AudioClipSet.Loudness_Primary, AudioClipSet.Loudness_Primary * soundTravelDistanceMultByEnvironment * globalLoudnessMultiplier, base.transform.position, playerIFF);
			}
			if (!IsSuppressed())
			{
				SceneSettings.PingReceivers(MuzzlePos.position);
			}
			RattleSuppresor();
			for (int i = 0; i < MuzzleDevices.Count; i++)
			{
				MuzzleDevices[i].OnShot(this, tailClass);
			}
		}

		public void PlayAudioGunShot(bool IsHighPressure, FVRTailSoundClass TailClass, FVRTailSoundClass TailClassSuppressed, FVRSoundEnvironment TailEnvironment)
		{
			Vector3 position = base.transform.position;
			FVRTailSoundClass tailClass = FVRTailSoundClass.Tiny;
			if (IsSuppressed())
			{
				PlayAudioEvent(FirearmAudioEventType.Shots_Suppressed);
				if (AudioClipSet.UsesTail_Suppressed)
				{
					tailClass = TailClassSuppressed;
					AudioEvent tailSet = SM.GetTailSet(TailClassSuppressed, TailEnvironment);
					m_pool_tail.PlayClipPitchOverride(tailSet, position, AudioClipSet.TailPitchMod_Suppressed * tailSet.PitchRange.x);
				}
			}
			else
			{
				float num = 1f;
				if (IsBraked())
				{
					num = 0.92f;
				}
				if (IsHighPressure)
				{
					PlayAudioEvent(FirearmAudioEventType.Shots_Main, num);
					if (AudioClipSet.UsesTail_Main)
					{
						tailClass = TailClass;
						AudioEvent tailSet2 = SM.GetTailSet(TailClass, TailEnvironment);
						m_pool_tail.PlayClipPitchOverride(tailSet2, position, AudioClipSet.TailPitchMod_Main * tailSet2.PitchRange.x * num);
					}
				}
				else
				{
					PlayAudioEvent(FirearmAudioEventType.Shots_LowPressure, num);
					if (AudioClipSet.UsesTail_Main)
					{
						tailClass = TailClass;
						AudioEvent tailSet3 = SM.GetTailSet(TailClass, TailEnvironment);
						m_pool_tail.PlayClipPitchOverride(tailSet3, position, AudioClipSet.TailPitchMod_LowPressure * tailSet3.PitchRange.x * num);
					}
				}
			}
			float soundTravelDistanceMultByEnvironment = SM.GetSoundTravelDistanceMultByEnvironment(TailEnvironment);
			int playerIFF = GM.CurrentPlayerBody.GetPlayerIFF();
			if (IsSuppressed())
			{
				GM.CurrentSceneSettings.OnPerceiveableSound(AudioClipSet.Loudness_Suppressed, AudioClipSet.Loudness_Suppressed * soundTravelDistanceMultByEnvironment * 0.4f, base.transform.position, playerIFF);
			}
			else if (AudioClipSet.UsesLowPressureSet && !IsHighPressure)
			{
				GM.CurrentSceneSettings.OnPerceiveableSound(AudioClipSet.Loudness_Primary * 0.6f, AudioClipSet.Loudness_Primary * 0.6f * soundTravelDistanceMultByEnvironment, base.transform.position, playerIFF);
			}
			else
			{
				GM.CurrentSceneSettings.OnPerceiveableSound(AudioClipSet.Loudness_Primary, AudioClipSet.Loudness_Primary * soundTravelDistanceMultByEnvironment, base.transform.position, playerIFF);
			}
			if (!IsSuppressed())
			{
				SceneSettings.PingReceivers(MuzzlePos.position);
			}
			RattleSuppresor();
			for (int i = 0; i < MuzzleDevices.Count; i++)
			{
				MuzzleDevices[i].OnShot(this, tailClass);
			}
		}

		public void PlayAudioEventHandling(AudioEvent e)
		{
			Vector3 position = base.transform.position;
			m_pool_handling.PlayClip(e, position);
		}

		public FVRPooledAudioSource PlayAudioAsHandling(AudioEvent ev, Vector3 pos)
		{
			float num = 1f;
			if (m_fireWeaponHandlingPerceivableSounds)
			{
				num = SM.GetSoundTravelDistanceMultByEnvironment(GM.CurrentPlayerBody.GetCurrentSoundEnvironment());
			}
			int playerIFF = GM.CurrentPlayerBody.GetPlayerIFF();
			if (m_fireWeaponHandlingPerceivableSounds)
			{
				GM.CurrentSceneSettings.OnPerceiveableSound(10f * AudioClipSet.Loudness_OperationMult, 10f * AudioClipSet.Loudness_OperationMult * num * 0.5f, base.transform.position, playerIFF);
			}
			return m_pool_handling.PlayClip(ev, pos);
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
				m_pool_shot.PlayClipPitchOverride(AudioClipSet.Shots_Main, GetMuzzle().position, new Vector2(AudioClipSet.Shots_Main.PitchRange.x * pitchmod, AudioClipSet.Shots_Main.PitchRange.y * pitchmod));
				if (base.IsHeld)
				{
					m_hand.ForceTubeKick(AudioClipSet.FTP.Kick_Shot);
					m_hand.ForceTubeRumble(AudioClipSet.FTP.Rumble_Shot_Intensity, AudioClipSet.FTP.Rumble_Shot_Duration);
				}
				break;
			case FirearmAudioEventType.Shots_Suppressed:
				m_pool_shot.PlayClipPitchOverride(AudioClipSet.Shots_Suppressed, GetMuzzle().position, new Vector2(AudioClipSet.Shots_Suppressed.PitchRange.x * pitchmod, AudioClipSet.Shots_Suppressed.PitchRange.y * pitchmod));
				if (base.IsHeld)
				{
					m_hand.ForceTubeKick(AudioClipSet.FTP.Kick_Shot);
					m_hand.ForceTubeRumble(AudioClipSet.FTP.Rumble_Shot_Intensity, AudioClipSet.FTP.Rumble_Shot_Duration);
				}
				break;
			case FirearmAudioEventType.Shots_LowPressure:
				m_pool_shot.PlayClipPitchOverride(AudioClipSet.Shots_LowPressure, GetMuzzle().position, new Vector2(AudioClipSet.Shots_LowPressure.PitchRange.x * pitchmod, AudioClipSet.Shots_LowPressure.PitchRange.y * pitchmod));
				if (base.IsHeld)
				{
					m_hand.ForceTubeKick(AudioClipSet.FTP.Kick_Shot);
					m_hand.ForceTubeRumble(AudioClipSet.FTP.Rumble_Shot_Intensity, AudioClipSet.FTP.Rumble_Shot_Duration);
				}
				break;
			case FirearmAudioEventType.BoltRelease:
				m_pool_mechanics.PlayClip(AudioClipSet.BoltRelease, position);
				if (base.IsHeld)
				{
					m_hand.ForceTubeRumble(AudioClipSet.FTP.Rumble_Handling_Intensity, AudioClipSet.FTP.Rumble_Handling_Duration);
				}
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
				if (base.IsHeld)
				{
					m_hand.ForceTubeRumble(AudioClipSet.FTP.Rumble_Handling_Intensity, AudioClipSet.FTP.Rumble_Handling_Duration);
				}
				break;
			case FirearmAudioEventType.HandleForward:
				m_pool_handling.PlayClip(AudioClipSet.HandleForward, position);
				if (m_fireWeaponHandlingPerceivableSounds)
				{
					GM.CurrentSceneSettings.OnPerceiveableSound(20f * AudioClipSet.Loudness_OperationMult, 20f * AudioClipSet.Loudness_OperationMult * num * 0.5f, base.transform.position, playerIFF);
				}
				if (base.IsHeld)
				{
					m_hand.ForceTubeRumble(AudioClipSet.FTP.Rumble_Handling_Intensity, AudioClipSet.FTP.Rumble_Handling_Duration);
				}
				break;
			case FirearmAudioEventType.HandleBackEmpty:
				m_pool_handling.PlayClip(AudioClipSet.HandleBackEmpty, position);
				if (m_fireWeaponHandlingPerceivableSounds)
				{
					GM.CurrentSceneSettings.OnPerceiveableSound(20f * AudioClipSet.Loudness_OperationMult, 20f * AudioClipSet.Loudness_OperationMult * num * 0.5f, base.transform.position, playerIFF);
				}
				if (base.IsHeld)
				{
					m_hand.ForceTubeRumble(AudioClipSet.FTP.Rumble_Handling_Intensity, AudioClipSet.FTP.Rumble_Handling_Duration);
				}
				break;
			case FirearmAudioEventType.HandleForwardEmpty:
				m_pool_handling.PlayClip(AudioClipSet.HandleForwardEmpty, position);
				if (m_fireWeaponHandlingPerceivableSounds)
				{
					GM.CurrentSceneSettings.OnPerceiveableSound(20f * AudioClipSet.Loudness_OperationMult, 20f * AudioClipSet.Loudness_OperationMult * num * 0.5f, base.transform.position, playerIFF);
				}
				if (base.IsHeld)
				{
					m_hand.ForceTubeRumble(AudioClipSet.FTP.Rumble_Handling_Intensity, AudioClipSet.FTP.Rumble_Handling_Duration);
				}
				break;
			case FirearmAudioEventType.HandleUp:
				m_pool_handling.PlayClip(AudioClipSet.HandleUp, position);
				if (m_fireWeaponHandlingPerceivableSounds)
				{
					GM.CurrentSceneSettings.OnPerceiveableSound(10f * AudioClipSet.Loudness_OperationMult, 10f * AudioClipSet.Loudness_OperationMult * num * 0.5f, base.transform.position, playerIFF);
				}
				if (base.IsHeld)
				{
					m_hand.ForceTubeRumble(AudioClipSet.FTP.Rumble_Handling_Intensity, AudioClipSet.FTP.Rumble_Handling_Duration);
				}
				break;
			case FirearmAudioEventType.HandleDown:
				m_pool_handling.PlayClip(AudioClipSet.HandleDown, position);
				if (m_fireWeaponHandlingPerceivableSounds)
				{
					GM.CurrentSceneSettings.OnPerceiveableSound(20f * AudioClipSet.Loudness_OperationMult, 20f * AudioClipSet.Loudness_OperationMult * num * 0.5f, base.transform.position, playerIFF);
				}
				if (base.IsHeld)
				{
					m_hand.ForceTubeRumble(AudioClipSet.FTP.Rumble_Handling_Intensity, AudioClipSet.FTP.Rumble_Handling_Duration);
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
				if (base.IsHeld)
				{
					m_hand.ForceTubeRumble(AudioClipSet.FTP.Rumble_Handling_Intensity, AudioClipSet.FTP.Rumble_Handling_Duration);
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
				if (base.IsHeld)
				{
					m_hand.ForceTubeRumble(AudioClipSet.FTP.Rumble_Handling_Intensity, AudioClipSet.FTP.Rumble_Handling_Duration);
				}
				break;
			case FirearmAudioEventType.BreachClose:
				m_pool_handling.PlayClip(AudioClipSet.BreachClose, position);
				if (m_fireWeaponHandlingPerceivableSounds)
				{
					GM.CurrentSceneSettings.OnPerceiveableSound(20f * AudioClipSet.Loudness_OperationMult, 20f * AudioClipSet.Loudness_OperationMult * num * 0.5f, base.transform.position, playerIFF);
				}
				if (base.IsHeld)
				{
					m_hand.ForceTubeRumble(AudioClipSet.FTP.Rumble_Handling_Intensity, AudioClipSet.FTP.Rumble_Handling_Duration);
				}
				break;
			case FirearmAudioEventType.MagazineIn:
				m_pool_handling.PlayClip(AudioClipSet.MagazineIn, position);
				if (m_fireWeaponHandlingPerceivableSounds)
				{
					GM.CurrentSceneSettings.OnPerceiveableSound(20f * AudioClipSet.Loudness_OperationMult, 20f * AudioClipSet.Loudness_OperationMult * num * 0.5f, base.transform.position, playerIFF);
				}
				if (base.IsHeld)
				{
					m_hand.ForceTubeRumble(AudioClipSet.FTP.Rumble_Handling_Intensity, AudioClipSet.FTP.Rumble_Handling_Duration);
				}
				break;
			case FirearmAudioEventType.MagazineOut:
				m_pool_handling.PlayClip(AudioClipSet.MagazineOut, position);
				if (m_fireWeaponHandlingPerceivableSounds)
				{
					GM.CurrentSceneSettings.OnPerceiveableSound(10f * AudioClipSet.Loudness_OperationMult, 10f * AudioClipSet.Loudness_OperationMult * num * 0.5f, base.transform.position, playerIFF);
				}
				if (base.IsHeld)
				{
					m_hand.ForceTubeRumble(AudioClipSet.FTP.Rumble_Handling_Intensity, AudioClipSet.FTP.Rumble_Handling_Duration);
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
				if (base.IsHeld)
				{
					m_hand.ForceTubeRumble(AudioClipSet.FTP.Rumble_Handling_Intensity, AudioClipSet.FTP.Rumble_Handling_Duration);
				}
				break;
			case FirearmAudioEventType.TopCoverDown:
				m_pool_handling.PlayClip(AudioClipSet.TopCoverDown, position);
				if (m_fireWeaponHandlingPerceivableSounds)
				{
					GM.CurrentSceneSettings.OnPerceiveableSound(10f * AudioClipSet.Loudness_OperationMult, 10f * AudioClipSet.Loudness_OperationMult * num * 0.5f, base.transform.position, playerIFF);
				}
				if (base.IsHeld)
				{
					m_hand.ForceTubeRumble(AudioClipSet.FTP.Rumble_Handling_Intensity, AudioClipSet.FTP.Rumble_Handling_Duration);
				}
				break;
			case FirearmAudioEventType.StockOpen:
				m_pool_handling.PlayClip(AudioClipSet.StockOpen, position);
				if (m_fireWeaponHandlingPerceivableSounds)
				{
					GM.CurrentSceneSettings.OnPerceiveableSound(10f * AudioClipSet.Loudness_OperationMult, 10f * AudioClipSet.Loudness_OperationMult * num * 0.5f, base.transform.position, playerIFF);
				}
				if (base.IsHeld)
				{
					m_hand.ForceTubeRumble(AudioClipSet.FTP.Rumble_Handling_Intensity, AudioClipSet.FTP.Rumble_Handling_Duration);
				}
				break;
			case FirearmAudioEventType.StockClosed:
				m_pool_handling.PlayClip(AudioClipSet.StockClosed, position);
				if (m_fireWeaponHandlingPerceivableSounds)
				{
					GM.CurrentSceneSettings.OnPerceiveableSound(10f * AudioClipSet.Loudness_OperationMult, 10f * AudioClipSet.Loudness_OperationMult * num * 0.5f, base.transform.position, playerIFF);
				}
				if (base.IsHeld)
				{
					m_hand.ForceTubeRumble(AudioClipSet.FTP.Rumble_Handling_Intensity, AudioClipSet.FTP.Rumble_Handling_Duration);
				}
				break;
			case FirearmAudioEventType.BipodOpen:
				m_pool_handling.PlayClip(AudioClipSet.BipodOpen, position);
				if (m_fireWeaponHandlingPerceivableSounds)
				{
					GM.CurrentSceneSettings.OnPerceiveableSound(10f * AudioClipSet.Loudness_OperationMult, 10f * AudioClipSet.Loudness_OperationMult * num * 0.5f, base.transform.position, playerIFF);
				}
				if (base.IsHeld)
				{
					m_hand.ForceTubeRumble(AudioClipSet.FTP.Rumble_Handling_Intensity, AudioClipSet.FTP.Rumble_Handling_Duration);
				}
				break;
			case FirearmAudioEventType.BipodClosed:
				m_pool_handling.PlayClip(AudioClipSet.BipodClosed, position);
				if (m_fireWeaponHandlingPerceivableSounds)
				{
					GM.CurrentSceneSettings.OnPerceiveableSound(10f * AudioClipSet.Loudness_OperationMult, 10f * AudioClipSet.Loudness_OperationMult * num * 0.5f, base.transform.position, playerIFF);
				}
				if (base.IsHeld)
				{
					m_hand.ForceTubeRumble(AudioClipSet.FTP.Rumble_Handling_Intensity, AudioClipSet.FTP.Rumble_Handling_Duration);
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
			case FirearmAudioEventType.BeltSettle:
				m_pool_belt.PlayClip(AudioClipSet.BeltSettle, position);
				break;
			}
		}

		public virtual List<FireArmRoundClass> GetChamberRoundList()
		{
			return null;
		}

		public virtual List<string> GetFlagList()
		{
			return null;
		}

		public virtual void SetLoadedChambers(List<FireArmRoundClass> rounds)
		{
		}

		public virtual void SetFromFlagList(List<string> flags)
		{
		}

		public override Dictionary<string, string> GetFlagDic()
		{
			return new Dictionary<string, string>();
		}

		[ContextMenu("ConfigureForMelee")]
		public void ConfigureForMelee()
		{
			MP.IsMeleeWeapon = true;
			if (StockPos != null)
			{
				MP.HandPoint = StockPos;
			}
			else
			{
				MP.HandPoint = base.transform;
			}
			MP.EndPoint = MuzzlePos;
			MP.BaseDamageBCP = new Vector3(100f, 0f, 0f);
		}
	}
}
