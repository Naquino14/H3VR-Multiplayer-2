using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class Handgun : FVRFireArm
	{
		public enum FireSelectorModeType
		{
			Single = 1,
			FullAuto,
			Safe,
			Burst
		}

		[Serializable]
		public class FireSelectorMode
		{
			public float SelectorPosition;

			public FireSelectorModeType ModeType;

			public int BurstAmount = 3;
		}

		public enum TriggerStyle
		{
			SA,
			SADA,
			DAO
		}

		public enum HeldTouchpadAction
		{
			None,
			SlideRelease,
			Hammer,
			MagRelease
		}

		[Header("Handgun Params")]
		public HandgunSlide Slide;

		public FVRFireArmChamber Chamber;

		[Header("Component Connections")]
		public Transform Trigger;

		public Transform Safety;

		public Transform Barrel;

		public Transform Hammer;

		public Transform FireSelector;

		public Transform SlideRelease;

		public GameObject ReloadTriggerWell;

		[Header("Round Positions")]
		public Transform RoundPos_Ejecting;

		public Transform RoundPos_Ejection;

		public Transform RoundPos_Magazine;

		public Vector3 RoundEjectionSpeed;

		public Vector3 RoundEjectionSpin;

		private FVRFirearmMovingProxyRound m_proxy;

		[Header("Trigger Params")]
		public bool HasTrigger;

		public InterpStyle TriggerInterp;

		public Axis TriggerAxis;

		public float TriggerUnheld;

		public float TriggerHeld;

		public float TriggerResetThreshold = 0.45f;

		public float TriggerBreakThreshold = 0.85f;

		public TriggerStyle TriggerType;

		public float TriggerSpeed = 20f;

		public bool HasManualDecocker;

		private float m_triggerTarget;

		private float m_triggerFloat;

		private bool m_isSeerReady = true;

		[Header("Slide Release Params")]
		public bool HasSlideRelease;

		public bool HasSlideReleaseControl = true;

		public InterpStyle SlideReleaseInterp;

		public Axis SlideReleaseAxis;

		public float SlideReleaseUp;

		public float SlideReleaseDown;

		public bool HasSlideLockFunctionality;

		public float SlideLockRot;

		private bool m_isSlideLockMechanismEngaged;

		[Header("Safety Params")]
		public bool HasSafety;

		public bool HasSafetyControl = true;

		public InterpStyle Safety_Interp;

		public Axis SafetyAxis;

		public float SafetyOff;

		public float SafetyOn;

		public bool DoesSafetyRequireSlideForward;

		public bool DoesSafetyLockSlide;

		public bool HasMagazineSafety;

		public bool DoesSafetyRequireCockedHammer;

		public bool DoesSafetyEngagingDecock;

		public bool DoesSafetyDisengageCockHammer;

		private bool m_isSafetyEngaged;

		[Header("Hammer Params")]
		public bool HasHammer;

		public bool HasHammerControl = true;

		public InterpStyle Hammer_Interp;

		public Axis HammerAxis;

		public float HammerForward;

		public float HammerRearward;

		private bool m_isHammerCocked;

		private float m_hammerDALerp;

		[Header("Barrel Params")]
		public bool HasTiltingBarrel;

		public InterpStyle BarrelInterp;

		public Axis BarrelAxis;

		public float BarrelUntilted;

		public float BarrelTilted;

		[Header("FireSelector Params")]
		public bool HasFireSelector;

		public InterpStyle FireSelectorInterpStyle = InterpStyle.Rotation;

		public Axis FireSelectorAxis;

		public FireSelectorMode[] FireSelectorModes;

		private int m_fireSelectorMode;

		private int m_CamBurst;

		[Header("Misc Control Vars")]
		public bool HasMagReleaseInput = true;

		[HideInInspector]
		public bool IsSlideLockPushedUp;

		[HideInInspector]
		public bool IsSlideLockHeldDown;

		[HideInInspector]
		public bool IsMagReleaseHeldDown;

		[HideInInspector]
		public bool IsSafetyOn;

		[HideInInspector]
		public bool HasTriggerReset = true;

		[HideInInspector]
		public bool IsSlideLockUp;

		[HideInInspector]
		public bool IsSlideLockExternalPushedUp;

		[HideInInspector]
		public bool IsSlideLockExternalHeldDown;

		public bool CanPhysicsSlideRack = true;

		private HashSet<Collider> m_slideCols = new HashSet<Collider>();

		private HashSet<Collider> m_unRackCols = new HashSet<Collider>();

		private HeldTouchpadAction m_heldTouchpadAction;

		private Vector2 TouchpadClickInitiation = Vector2.zero;

		private float m_timeSinceFiredShot = 1f;

		public bool IsSLideLockMechanismEngaged => m_isSlideLockMechanismEngaged;

		public bool IsSafetyEngaged => m_isSafetyEngaged;

		public int FireSelectorModeIndex => m_fireSelectorMode;

		protected override void Awake()
		{
			base.Awake();
			m_CamBurst = 1;
			GameObject gameObject = new GameObject("m_proxyRound");
			m_proxy = gameObject.AddComponent<FVRFirearmMovingProxyRound>();
			m_proxy.Init(base.transform);
			if (CanPhysicsSlideRack)
			{
				InitSlideCols();
			}
			if (Chamber != null)
			{
				Chamber.Firearm = this;
			}
		}

		private void InitSlideCols()
		{
			Transform[] componentsInChildren = Slide.gameObject.GetComponentsInChildren<Transform>();
			Transform[] array = componentsInChildren;
			foreach (Transform transform in array)
			{
				Collider component = transform.GetComponent<Collider>();
				if (component != null && !component.isTrigger)
				{
					m_slideCols.Add(component);
				}
			}
		}

		public void ResetCamBurst()
		{
			if (FireSelectorModes.Length > 0)
			{
				FireSelectorMode fireSelectorMode = FireSelectorModes[m_fireSelectorMode];
				m_CamBurst = fireSelectorMode.BurstAmount;
			}
		}

		public override void OnCollisionEnter(Collision c)
		{
			if (base.IsHeld && CanPhysicsSlideRack && c.contacts.Length > 0 && m_slideCols.Contains(c.contacts[0].thisCollider))
			{
				float num = Vector3.Angle(base.transform.forward, c.relativeVelocity);
				if (num > 135f && c.relativeVelocity.magnitude > 3f)
				{
					m_unRackCols.Add(c.collider);
					Slide.KnockToRear();
				}
			}
			base.OnCollisionEnter(c);
		}

		public void OnCollisionExit(Collision c)
		{
			if (base.IsHeld && CanPhysicsSlideRack && m_unRackCols.Contains(c.collider))
			{
				if (c.relativeVelocity.magnitude > 3f)
				{
					IsSlideLockExternalHeldDown = true;
				}
				m_unRackCols.Clear();
			}
		}

		public override int GetTutorialState()
		{
			if (Magazine == null)
			{
				return 0;
			}
			if (m_isSafetyEngaged)
			{
				return 4;
			}
			if (!Chamber.IsFull && Slide.GetSlideSpeed() <= 0f && m_timeSinceFiredShot > 0.25f)
			{
				if (Magazine.HasARound())
				{
					return 1;
				}
				return 3;
			}
			return 2;
		}

		public void EjectExtractedRound()
		{
			if (Chamber.IsFull)
			{
				Chamber.EjectRound(RoundPos_Ejection.position, base.transform.right * RoundEjectionSpeed.x + base.transform.up * RoundEjectionSpeed.y + base.transform.forward * RoundEjectionSpeed.z, base.transform.right * RoundEjectionSpin.x + base.transform.up * RoundEjectionSpin.y + base.transform.forward * RoundEjectionSpin.z);
			}
		}

		public void ExtractRound()
		{
			if (!(Magazine == null) && !m_proxy.IsFull && Magazine.HasARound())
			{
				GameObject fromPrefabReference = Magazine.RemoveRound(b: false);
				m_proxy.SetFromPrefabReference(fromPrefabReference);
			}
		}

		public bool ChamberRound()
		{
			if (m_proxy.IsFull && !Chamber.IsFull)
			{
				Chamber.SetRound(m_proxy.Round);
				m_proxy.ClearProxy();
				return true;
			}
			return false;
		}

		public bool CycleFireSelector()
		{
			if (FireSelectorModes.Length <= 1)
			{
				return false;
			}
			bool flag = true;
			bool flag2 = true;
			int fireSelectorMode = m_fireSelectorMode;
			m_fireSelectorMode++;
			if (m_fireSelectorMode >= FireSelectorModes.Length)
			{
				m_fireSelectorMode -= FireSelectorModes.Length;
			}
			if (HasSafety)
			{
				if (FireSelectorModes[m_fireSelectorMode].ModeType == FireSelectorModeType.Safe)
				{
					flag2 = SetSafetyState(s: true);
				}
				else
				{
					SetSafetyState(s: false);
				}
			}
			if (!flag2)
			{
				flag = false;
				m_fireSelectorMode = fireSelectorMode;
			}
			if (flag)
			{
				PlayAudioEvent(FirearmAudioEventType.FireSelector);
			}
			if (FireSelectorModes.Length > 0)
			{
				FireSelectorMode fireSelectorMode2 = FireSelectorModes[m_fireSelectorMode];
				if (m_triggerFloat < 0.1f)
				{
					m_CamBurst = fireSelectorMode2.BurstAmount;
				}
			}
			return true;
		}

		public bool SetFireSelectorByIndex(int i)
		{
			if (FireSelectorModes.Length <= 1)
			{
				return false;
			}
			m_fireSelectorMode = i;
			if (m_fireSelectorMode >= FireSelectorModes.Length)
			{
				m_fireSelectorMode -= FireSelectorModes.Length;
			}
			if (HasSafety)
			{
				if (FireSelectorModes[m_fireSelectorMode].ModeType == FireSelectorModeType.Safe)
				{
					SetSafetyState(s: true);
				}
				else
				{
					SetSafetyState(s: false);
				}
			}
			return true;
		}

		public bool ToggleSafety()
		{
			if (!HasSafety)
			{
				return false;
			}
			if (DoesSafetyRequireSlideForward && Slide.CurPos != 0)
			{
				return false;
			}
			if (Slide.CurPos == HandgunSlide.SlidePos.Forward || Slide.CurPos >= HandgunSlide.SlidePos.Locked)
			{
				if (m_isSafetyEngaged)
				{
					PlayAudioEvent(FirearmAudioEventType.Safety);
					m_isSafetyEngaged = false;
					if (DoesSafetyDisengageCockHammer)
					{
						CockHammer(isManual: true);
					}
				}
				else
				{
					bool flag = true;
					if (DoesSafetyRequireCockedHammer && !m_isHammerCocked)
					{
						flag = false;
					}
					if (flag)
					{
						m_isSafetyEngaged = true;
						if (DoesSafetyEngagingDecock)
						{
							DeCockHammer(isManual: true, isLoud: true);
						}
						PlayAudioEvent(FirearmAudioEventType.Safety);
					}
				}
				UpdateSafetyPos();
				return true;
			}
			return false;
		}

		public bool SetSafetyState(bool s)
		{
			if (!HasSafety)
			{
				return false;
			}
			if (DoesSafetyRequireSlideForward && Slide.CurPos != 0)
			{
				return false;
			}
			if (Slide.CurPos == HandgunSlide.SlidePos.Forward || Slide.CurPos == HandgunSlide.SlidePos.Locked)
			{
				if (m_isSafetyEngaged && !s)
				{
					PlayAudioEvent(FirearmAudioEventType.Safety);
					m_isSafetyEngaged = false;
					if (DoesSafetyDisengageCockHammer)
					{
						CockHammer(isManual: true);
					}
					UpdateSafetyPos();
					return true;
				}
				if (!m_isSafetyEngaged && s)
				{
					bool flag = true;
					if (DoesSafetyRequireCockedHammer && !m_isHammerCocked)
					{
						flag = false;
					}
					if (flag)
					{
						m_isSafetyEngaged = true;
						if (DoesSafetyEngagingDecock)
						{
							DeCockHammer(isManual: true, isLoud: true);
						}
						PlayAudioEvent(FirearmAudioEventType.Safety);
					}
					UpdateSafetyPos();
					return true;
				}
			}
			return false;
		}

		private void UpdateSafetyPos()
		{
			if (HasSafety)
			{
				float val = SafetyOff;
				if (m_isSafetyEngaged)
				{
					val = SafetyOn;
				}
				SetAnimatedComponent(Safety, val, Safety_Interp, SafetyAxis);
			}
		}

		public void ReleaseSeer()
		{
			HasTriggerReset = false;
			if (m_isHammerCocked && m_isSeerReady)
			{
				if (FireSelectorModes[m_fireSelectorMode].ModeType == FireSelectorModeType.Single || (FireSelectorModes[m_fireSelectorMode].ModeType == FireSelectorModeType.Burst && m_CamBurst < 1))
				{
					m_isSeerReady = false;
				}
				DropHammer(isManual: false);
			}
		}

		public void CockHammer(bool isManual)
		{
			if (isManual && !m_isHammerCocked)
			{
				PlayAudioEvent(FirearmAudioEventType.Prefire);
			}
			m_isHammerCocked = true;
		}

		public void DeCockHammer(bool isManual, bool isLoud)
		{
			if (Slide.CurPos != 0)
			{
				return;
			}
			if (isManual && m_isHammerCocked)
			{
				if (isLoud)
				{
					PlayAudioEvent(FirearmAudioEventType.HammerHit);
				}
				else
				{
					PlayAudioEvent(FirearmAudioEventType.TriggerReset);
				}
			}
			m_isHammerCocked = false;
		}

		public void DropSlideRelease()
		{
			if (IsSlideLockUp)
			{
				PlayAudioEvent(FirearmAudioEventType.BoltRelease);
			}
			IsSlideLockUp = false;
		}

		public void EngageSlideRelease()
		{
			IsSlideLockUp = true;
		}

		private void EngageSlideLockMechanism()
		{
			if (!m_isSlideLockMechanismEngaged)
			{
				m_isSlideLockMechanismEngaged = true;
				PlayAudioEvent(FirearmAudioEventType.FireSelector);
			}
		}

		private void DisEngageSlideLockMechanism()
		{
			if (m_isSlideLockMechanismEngaged)
			{
				m_isSlideLockMechanismEngaged = false;
				PlayAudioEvent(FirearmAudioEventType.FireSelector);
			}
		}

		public bool IsSlideCatchEngaged()
		{
			return IsSlideLockUp;
		}

		public void DropHammer(bool isManual)
		{
			if (Slide.CurPos == HandgunSlide.SlidePos.Forward && m_isHammerCocked)
			{
				m_isHammerCocked = false;
				if (m_CamBurst > 0)
				{
					m_CamBurst--;
				}
				Fire();
				PlayAudioEvent(FirearmAudioEventType.HammerHit);
			}
		}

		public bool Fire()
		{
			if (!Chamber.Fire())
			{
				return false;
			}
			m_timeSinceFiredShot = 0f;
			base.Fire(Chamber, GetMuzzle(), doBuzz: true);
			FireMuzzleSmoke();
			bool twoHandStabilized = IsTwoHandStabilized();
			bool foregripStabilized = IsForegripStabilized();
			bool shoulderStabilized = IsShoulderStabilized();
			float globalLoudnessMultiplier = 1f;
			float verticalRecoilMult = 1f;
			if (m_isSlideLockMechanismEngaged)
			{
				globalLoudnessMultiplier = 0.4f;
				verticalRecoilMult = 1.5f;
			}
			Recoil(twoHandStabilized, foregripStabilized, shoulderStabilized, GetRecoilProfile(), verticalRecoilMult);
			PlayAudioGunShot(Chamber.GetRound(), GM.CurrentPlayerBody.GetCurrentSoundEnvironment(), globalLoudnessMultiplier);
			if (!IsSLideLockMechanismEngaged)
			{
				Slide.ImpartFiringImpulse();
			}
			return true;
		}

		protected override void FVRUpdate()
		{
			base.FVRUpdate();
			UpdateComponents();
			Slide.UpdateSlide();
			UpdateDisplayRoundPositions();
			if (m_timeSinceFiredShot < 1f)
			{
				m_timeSinceFiredShot += Time.deltaTime;
			}
		}

		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);
			UpdateInputAndAnimate(hand);
		}

		private void UpdateInputAndAnimate(FVRViveHand hand)
		{
			IsSlideLockHeldDown = false;
			IsSlideLockPushedUp = false;
			IsMagReleaseHeldDown = false;
			if (IsAltHeld)
			{
				return;
			}
			if (m_hasTriggeredUpSinceBegin)
			{
				m_triggerTarget = hand.Input.TriggerFloat;
			}
			else
			{
				m_triggerTarget = 0f;
			}
			if (m_triggerTarget > m_triggerFloat)
			{
				m_triggerFloat = Mathf.MoveTowards(m_triggerFloat, m_triggerTarget, Time.deltaTime * TriggerSpeed);
			}
			else
			{
				m_triggerFloat = Mathf.MoveTowards(m_triggerFloat, m_triggerTarget, Time.deltaTime * TriggerSpeed * 2f);
			}
			if (!HasTriggerReset && m_triggerFloat <= TriggerResetThreshold)
			{
				HasTriggerReset = true;
				m_isSeerReady = true;
				PlayAudioEvent(FirearmAudioEventType.TriggerReset);
				if (FireSelectorModes.Length > 0)
				{
					m_CamBurst = FireSelectorModes[m_fireSelectorMode].BurstAmount;
				}
			}
			if (hand.IsInStreamlinedMode)
			{
				if (hand.Input.AXButtonPressed)
				{
					IsMagReleaseHeldDown = true;
				}
				if (hand.Input.BYButtonDown)
				{
					m_heldTouchpadAction = HeldTouchpadAction.None;
					bool flag = true;
					if (HasSlideReleaseControl && IsSlideCatchEngaged())
					{
						DropSlideRelease();
						flag = false;
					}
					if (flag)
					{
						if (HasFireSelector)
						{
							CycleFireSelector();
						}
						else if (HasSafetyControl)
						{
							ToggleSafety();
						}
					}
				}
			}
			else
			{
				Vector2 touchpadAxes = hand.Input.TouchpadAxes;
				if (hand.Input.TouchpadDown)
				{
					TouchpadClickInitiation = touchpadAxes;
					if (touchpadAxes.magnitude > 0.2f)
					{
						if (Vector2.Angle(touchpadAxes, Vector2.up) <= 45f)
						{
							m_heldTouchpadAction = HeldTouchpadAction.None;
							if (HasFireSelector)
							{
								CycleFireSelector();
							}
							else if (HasSafetyControl)
							{
								ToggleSafety();
							}
						}
						else if (Vector2.Angle(touchpadAxes, Vector2.down) <= 45f)
						{
							m_heldTouchpadAction = HeldTouchpadAction.MagRelease;
						}
						else if (Vector2.Angle(touchpadAxes, Vector2.left) <= 45f && HasSlideRelease && HasSlideReleaseControl)
						{
							m_heldTouchpadAction = HeldTouchpadAction.SlideRelease;
						}
						else if (Vector2.Angle(touchpadAxes, Vector2.right) <= 45f && TriggerType != TriggerStyle.DAO && HasHammerControl)
						{
							m_heldTouchpadAction = HeldTouchpadAction.Hammer;
						}
					}
				}
				if (hand.Input.TouchpadPressed)
				{
					if (m_heldTouchpadAction == HeldTouchpadAction.MagRelease)
					{
						if (touchpadAxes.magnitude > 0.2f && Vector2.Angle(touchpadAxes, Vector2.down) <= 45f)
						{
							IsMagReleaseHeldDown = true;
						}
					}
					else if (m_heldTouchpadAction == HeldTouchpadAction.SlideRelease)
					{
						if (touchpadAxes.y >= TouchpadClickInitiation.y + 0.05f)
						{
							IsSlideLockPushedUp = true;
						}
						else if (touchpadAxes.y <= TouchpadClickInitiation.y - 0.05f)
						{
							IsSlideLockHeldDown = true;
						}
					}
					else if (m_heldTouchpadAction == HeldTouchpadAction.Hammer)
					{
						if (touchpadAxes.y <= TouchpadClickInitiation.y - 0.05f && TriggerType != TriggerStyle.DAO && !m_isHammerCocked && Slide.CurPos == HandgunSlide.SlidePos.Forward)
						{
							CockHammer(isManual: true);
						}
						else if (touchpadAxes.y >= TouchpadClickInitiation.y + 0.05f && TriggerType != TriggerStyle.DAO && m_isHammerCocked && (m_triggerFloat > 0.1f || HasManualDecocker) && Slide.CurPos == HandgunSlide.SlidePos.Forward)
						{
							DeCockHammer(isManual: true, isLoud: false);
						}
					}
				}
				if (hand.Input.TouchpadUp)
				{
					m_heldTouchpadAction = HeldTouchpadAction.None;
				}
			}
			if (m_triggerFloat >= TriggerBreakThreshold)
			{
				if (!m_isHammerCocked && (TriggerType == TriggerStyle.SADA || TriggerType == TriggerStyle.DAO) && Slide.CurPos == HandgunSlide.SlidePos.Forward && (!HasMagazineSafety || Magazine != null))
				{
					if (m_hammerDALerp >= 1f && m_isSeerReady)
					{
						CockHammer(isManual: false);
					}
				}
				else if (!m_isSafetyEngaged && (!HasMagazineSafety || Magazine != null))
				{
					ReleaseSeer();
				}
			}
			if (!m_isHammerCocked && !m_isSafetyEngaged && (TriggerType == TriggerStyle.SADA || TriggerType == TriggerStyle.DAO) && Slide.CurPos == HandgunSlide.SlidePos.Forward && m_isSeerReady)
			{
				float num = (m_hammerDALerp = Mathf.InverseLerp(TriggerResetThreshold, TriggerBreakThreshold, m_triggerFloat));
			}
			else
			{
				m_hammerDALerp = 0f;
			}
			if (IsMagReleaseHeldDown && HasMagReleaseInput)
			{
				if (Magazine != null)
				{
					base.EjectMag();
				}
				if (ReloadTriggerWell != null)
				{
					ReloadTriggerWell.SetActive(value: false);
				}
			}
			else if (ReloadTriggerWell != null)
			{
				ReloadTriggerWell.SetActive(value: true);
			}
		}

		public void ReleaseMag()
		{
			if (Magazine != null)
			{
				base.EjectMag();
			}
		}

		private void UpdateComponents()
		{
			if (Slide.CurPos < HandgunSlide.SlidePos.Locked)
			{
				IsSlideLockUp = false;
			}
			else if (IsSlideLockPushedUp || IsSlideLockExternalPushedUp)
			{
				EngageSlideRelease();
			}
			else if ((IsSlideLockHeldDown || IsSlideLockExternalHeldDown) && Slide.m_hand == null)
			{
				DropSlideRelease();
				IsSlideLockExternalHeldDown = false;
			}
			IsSlideLockExternalHeldDown = false;
			if (HasSlideLockFunctionality && Slide.CurPos == HandgunSlide.SlidePos.Forward)
			{
				if (IsSlideLockHeldDown)
				{
					EngageSlideLockMechanism();
				}
				else if (IsSlideLockPushedUp)
				{
					DisEngageSlideLockMechanism();
				}
			}
			if (HasHammer)
			{
				float t = 0f;
				switch (TriggerType)
				{
				case TriggerStyle.SA:
					t = ((!m_isHammerCocked) ? (1f - Slide.GetSlideLerpBetweenLockAndFore()) : 1f);
					break;
				case TriggerStyle.SADA:
					t = ((!m_isHammerCocked) ? ((Slide.CurPos != 0) ? (1f - Slide.GetSlideLerpBetweenLockAndFore()) : m_hammerDALerp) : 1f);
					break;
				case TriggerStyle.DAO:
					t = ((Slide.CurPos != 0 || !m_isSeerReady) ? (1f - Slide.GetSlideLerpBetweenLockAndFore()) : m_hammerDALerp);
					break;
				}
				float val = Mathf.Lerp(HammerForward, HammerRearward, t);
				SetAnimatedComponent(Hammer, val, Hammer_Interp, HammerAxis);
			}
			if (HasTiltingBarrel)
			{
				float num = 1f - Slide.GetSlideLerpBetweenLockAndFore();
				float val2 = Mathf.Lerp(BarrelUntilted, BarrelTilted, num * 4f);
				SetAnimatedComponent(Barrel, val2, BarrelInterp, BarrelAxis);
			}
			if (HasSlideRelease)
			{
				float t2 = 0f;
				if (IsSlideLockUp)
				{
					t2 = 1f;
				}
				float a = SlideReleaseDown;
				float b = SlideReleaseUp;
				if (m_isSlideLockMechanismEngaged)
				{
					a = SlideLockRot;
					b = SlideLockRot;
				}
				switch (SlideReleaseInterp)
				{
				case InterpStyle.Rotation:
				{
					Vector3 localEulerAngles = SlideRelease.localEulerAngles;
					switch (SlideReleaseAxis)
					{
					case Axis.X:
						localEulerAngles.x = Mathf.Lerp(a, b, t2);
						break;
					case Axis.Y:
						localEulerAngles.y = Mathf.Lerp(a, b, t2);
						break;
					case Axis.Z:
						localEulerAngles.z = Mathf.Lerp(a, b, t2);
						break;
					}
					SlideRelease.localEulerAngles = localEulerAngles;
					break;
				}
				case InterpStyle.Translate:
				{
					Vector3 localPosition = SlideRelease.localPosition;
					switch (SlideReleaseAxis)
					{
					case Axis.X:
						localPosition.x = Mathf.Lerp(a, b, t2);
						break;
					case Axis.Y:
						localPosition.y = Mathf.Lerp(a, b, t2);
						break;
					case Axis.Z:
						localPosition.z = Mathf.Lerp(a, b, t2);
						break;
					}
					SlideRelease.localPosition = localPosition;
					break;
				}
				}
			}
			if (HasTrigger)
			{
				switch (TriggerInterp)
				{
				case InterpStyle.Rotation:
				{
					Vector3 localEulerAngles2 = Trigger.localEulerAngles;
					switch (TriggerAxis)
					{
					case Axis.X:
						localEulerAngles2.x = Mathf.Lerp(TriggerUnheld, TriggerHeld, m_triggerFloat);
						break;
					case Axis.Y:
						localEulerAngles2.y = Mathf.Lerp(TriggerUnheld, TriggerHeld, m_triggerFloat);
						break;
					case Axis.Z:
						localEulerAngles2.z = Mathf.Lerp(TriggerUnheld, TriggerHeld, m_triggerFloat);
						break;
					}
					Trigger.localEulerAngles = localEulerAngles2;
					break;
				}
				case InterpStyle.Translate:
				{
					Vector3 localPosition2 = Trigger.localPosition;
					switch (TriggerAxis)
					{
					case Axis.X:
						localPosition2.x = Mathf.Lerp(TriggerUnheld, TriggerHeld, m_triggerFloat);
						break;
					case Axis.Y:
						localPosition2.y = Mathf.Lerp(TriggerUnheld, TriggerHeld, m_triggerFloat);
						break;
					case Axis.Z:
						localPosition2.z = Mathf.Lerp(TriggerUnheld, TriggerHeld, m_triggerFloat);
						break;
					}
					Trigger.localPosition = localPosition2;
					break;
				}
				}
			}
			if (!HasFireSelector)
			{
				return;
			}
			switch (FireSelectorInterpStyle)
			{
			case InterpStyle.Rotation:
			{
				Vector3 zero2 = Vector3.zero;
				switch (FireSelectorAxis)
				{
				case Axis.X:
					zero2.x = FireSelectorModes[m_fireSelectorMode].SelectorPosition;
					break;
				case Axis.Y:
					zero2.y = FireSelectorModes[m_fireSelectorMode].SelectorPosition;
					break;
				case Axis.Z:
					zero2.z = FireSelectorModes[m_fireSelectorMode].SelectorPosition;
					break;
				}
				FireSelector.localEulerAngles = zero2;
				break;
			}
			case InterpStyle.Translate:
			{
				Vector3 zero = Vector3.zero;
				switch (FireSelectorAxis)
				{
				case Axis.X:
					zero.x = FireSelectorModes[m_fireSelectorMode].SelectorPosition;
					break;
				case Axis.Y:
					zero.y = FireSelectorModes[m_fireSelectorMode].SelectorPosition;
					break;
				case Axis.Z:
					zero.z = FireSelectorModes[m_fireSelectorMode].SelectorPosition;
					break;
				}
				FireSelector.localPosition = zero;
				break;
			}
			}
		}

		private void UpdateDisplayRoundPositions()
		{
			float slideLerpBetweenLockAndFore = Slide.GetSlideLerpBetweenLockAndFore();
			if (m_proxy.IsFull)
			{
				m_proxy.ProxyRound.position = Vector3.Lerp(RoundPos_Magazine.position, Chamber.transform.position, slideLerpBetweenLockAndFore);
				m_proxy.ProxyRound.rotation = Quaternion.Slerp(RoundPos_Magazine.rotation, Chamber.transform.rotation, slideLerpBetweenLockAndFore);
			}
			else if (Chamber.IsFull)
			{
				Chamber.ProxyRound.position = Vector3.Lerp(RoundPos_Ejecting.position, Chamber.transform.position, slideLerpBetweenLockAndFore);
				Chamber.ProxyRound.rotation = Quaternion.Slerp(RoundPos_Ejecting.rotation, Chamber.transform.rotation, slideLerpBetweenLockAndFore);
			}
			if (Slide.CurPos == HandgunSlide.SlidePos.Forward)
			{
				Chamber.IsAccessible = false;
			}
			else
			{
				Chamber.IsAccessible = true;
			}
		}

		public override List<FireArmRoundClass> GetChamberRoundList()
		{
			if (Chamber.IsFull && !Chamber.IsSpent)
			{
				List<FireArmRoundClass> list = new List<FireArmRoundClass>();
				list.Add(Chamber.GetRound().RoundClass);
				return list;
			}
			return null;
		}

		public override void SetLoadedChambers(List<FireArmRoundClass> rounds)
		{
			if (rounds.Count > 0)
			{
				Chamber.Autochamber(rounds[0]);
			}
		}

		public override List<string> GetFlagList()
		{
			return null;
		}

		public override void SetFromFlagList(List<string> flags)
		{
		}

		public override void ConfigureFromFlagDic(Dictionary<string, string> f)
		{
			string empty = string.Empty;
			string empty2 = string.Empty;
			empty = "HammerState";
			if (f.ContainsKey(empty))
			{
				empty2 = f[empty];
				if (empty2 == "Cocked")
				{
					CockHammer(isManual: false);
				}
			}
			if (HasSafety)
			{
				empty = "SafetyState";
				if (f.ContainsKey(empty))
				{
					empty2 = f[empty];
					if (empty2 == "On")
					{
						SetSafetyState(s: true);
					}
				}
			}
			if (HasFireSelector)
			{
				empty = "FireSelectorState";
				if (f.ContainsKey(empty))
				{
					empty2 = f[empty];
					int result = 0;
					int.TryParse(empty2, out result);
					SetFireSelectorByIndex(result);
				}
			}
			UpdateComponents();
			UpdateSafetyPos();
		}

		public override Dictionary<string, string> GetFlagDic()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			string key = "HammerState";
			string value = "Uncocked";
			if (m_isHammerCocked)
			{
				value = "Cocked";
			}
			dictionary.Add(key, value);
			if (HasSafety)
			{
				key = "SafetyState";
				value = "Off";
				if (m_isSafetyEngaged)
				{
					value = "On";
				}
				dictionary.Add(key, value);
			}
			if (HasFireSelector)
			{
				key = "FireSelectorState";
				value = m_fireSelectorMode.ToString();
				dictionary.Add(key, value);
			}
			return dictionary;
		}
	}
}
