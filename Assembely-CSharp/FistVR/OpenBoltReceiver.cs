using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class OpenBoltReceiver : FVRFireArm
	{
		public new enum InterpStyle
		{
			Translate,
			Rotation
		}

		public new enum Axis
		{
			X,
			Y,
			Z
		}

		public enum FireSelectorModeType
		{
			Safe,
			Single,
			FullAuto,
			SuperFastBurst
		}

		[Serializable]
		public class FireSelectorMode
		{
			public float SelectorPosition;

			public FireSelectorModeType ModeType;
		}

		[Header("OpenBoltWeapon Config")]
		public bool HasTriggerButton = true;

		public bool HasFireSelectorButton = true;

		public bool HasMagReleaseButton = true;

		public bool DoesForwardBoltDisableReloadWell;

		[Header("Component Connections")]
		public OpenBoltReceiverBolt Bolt;

		public FVRFireArmChamber Chamber;

		public Transform Trigger;

		public Transform MagReleaseButton;

		public Transform FireSelectorSwitch;

		public Transform FireSelectorSwitch2;

		public GameObject ReloadTriggerWell;

		[Header("Round Positions")]
		public Transform RoundPos_Ejecting;

		public Transform RoundPos_Ejection;

		public Transform RoundPos_MagazinePos;

		private FVRFirearmMovingProxyRound m_proxy;

		public Vector3 EjectionSpeed;

		public Vector3 EjectionSpin;

		public bool UsesDelinker;

		public ParticleSystem DelinkerSystem;

		[Header("Trigger Config")]
		public float TriggerFiringThreshold = 0.8f;

		public float TriggerResetThreshold = 0.4f;

		public float Trigger_ForwardValue;

		public float Trigger_RearwardValue;

		public InterpStyle TriggerInterpStyle = InterpStyle.Rotation;

		private float m_triggerFloat;

		private bool m_hasTriggerCycled;

		private bool m_isSeerEngaged = true;

		private bool m_isHammerCocked;

		private bool m_isCamSet = true;

		private int m_CamBurst;

		public int SuperBurstAmount = 3;

		private int m_fireSelectorMode;

		[Header("Fire Selector Config")]
		public InterpStyle FireSelector_InterpStyle = InterpStyle.Rotation;

		public Axis FireSelector_Axis;

		public FireSelectorMode[] FireSelector_Modes;

		[Header("Secondary Fire Selector Config")]
		public InterpStyle FireSelector_InterpStyle2 = InterpStyle.Rotation;

		public Axis FireSelector_Axis2;

		public FireSelectorMode[] FireSelector_Modes2;

		private float m_timeSinceFiredShot = 1f;

		[Header("SpecialFeatures")]
		public bool UsesRecoilingSystem;

		public G11RecoilingSystem RecoilingSystem;

		public bool UsesMagMountTransformOverride;

		public Transform MagMountTransformOverride;

		public bool IsSeerEngaged => m_isSeerEngaged;

		public bool IsHammerCocked => m_isHammerCocked;

		public int FireSelectorModeIndex => m_fireSelectorMode;

		public bool HasExtractedRound()
		{
			return m_proxy.IsFull;
		}

		protected override void Awake()
		{
			base.Awake();
			GameObject gameObject = new GameObject("m_proxyRound");
			m_proxy = gameObject.AddComponent<FVRFirearmMovingProxyRound>();
			m_proxy.Init(base.transform);
		}

		public override int GetTutorialState()
		{
			if (Magazine == null)
			{
				return 0;
			}
			if (Magazine != null && !Magazine.HasARound())
			{
				return 5;
			}
			if (FireSelector_Modes[m_fireSelectorMode].ModeType == FireSelectorModeType.Safe)
			{
				return 1;
			}
			if ((Bolt.CurPos == OpenBoltReceiverBolt.BoltPos.Forward) & (m_timeSinceFiredShot > 0.4f))
			{
				return 2;
			}
			if (base.AltGrip == null)
			{
				return 3;
			}
			return 4;
		}

		public void SecondaryFireSelectorClicked()
		{
			PlayAudioEvent(FirearmAudioEventType.FireSelector);
		}

		public bool IsBoltCatchEngaged()
		{
			return m_isSeerEngaged;
		}

		public void ReleaseSeer()
		{
			if (m_isSeerEngaged && Bolt.CurPos == OpenBoltReceiverBolt.BoltPos.Locked)
			{
				PlayAudioEvent(FirearmAudioEventType.Prefire);
			}
			m_isSeerEngaged = false;
		}

		public void EngageSeer()
		{
			m_isSeerEngaged = true;
		}

		protected virtual void ToggleFireSelector()
		{
			if (FireSelector_Modes.Length <= 1)
			{
				return;
			}
			m_fireSelectorMode++;
			if (m_fireSelectorMode >= FireSelector_Modes.Length)
			{
				m_fireSelectorMode -= FireSelector_Modes.Length;
			}
			PlayAudioEvent(FirearmAudioEventType.FireSelector);
			if (FireSelectorSwitch != null)
			{
				switch (FireSelector_InterpStyle)
				{
				case InterpStyle.Rotation:
				{
					Vector3 zero2 = Vector3.zero;
					switch (FireSelector_Axis)
					{
					case Axis.X:
						zero2.x = FireSelector_Modes[m_fireSelectorMode].SelectorPosition;
						break;
					case Axis.Y:
						zero2.y = FireSelector_Modes[m_fireSelectorMode].SelectorPosition;
						break;
					case Axis.Z:
						zero2.z = FireSelector_Modes[m_fireSelectorMode].SelectorPosition;
						break;
					}
					FireSelectorSwitch.localEulerAngles = zero2;
					break;
				}
				case InterpStyle.Translate:
				{
					Vector3 zero = Vector3.zero;
					switch (FireSelector_Axis)
					{
					case Axis.X:
						zero.x = FireSelector_Modes[m_fireSelectorMode].SelectorPosition;
						break;
					case Axis.Y:
						zero.y = FireSelector_Modes[m_fireSelectorMode].SelectorPosition;
						break;
					case Axis.Z:
						zero.z = FireSelector_Modes[m_fireSelectorMode].SelectorPosition;
						break;
					}
					FireSelectorSwitch.localPosition = zero;
					break;
				}
				}
			}
			if (!(FireSelectorSwitch2 != null))
			{
				return;
			}
			switch (FireSelector_InterpStyle2)
			{
			case InterpStyle.Rotation:
			{
				Vector3 zero4 = Vector3.zero;
				switch (FireSelector_Axis2)
				{
				case Axis.X:
					zero4.x = FireSelector_Modes2[m_fireSelectorMode].SelectorPosition;
					break;
				case Axis.Y:
					zero4.y = FireSelector_Modes2[m_fireSelectorMode].SelectorPosition;
					break;
				case Axis.Z:
					zero4.z = FireSelector_Modes2[m_fireSelectorMode].SelectorPosition;
					break;
				}
				FireSelectorSwitch2.localEulerAngles = zero4;
				break;
			}
			case InterpStyle.Translate:
			{
				Vector3 zero3 = Vector3.zero;
				switch (FireSelector_Axis2)
				{
				case Axis.X:
					zero3.x = FireSelector_Modes2[m_fireSelectorMode].SelectorPosition;
					break;
				case Axis.Y:
					zero3.y = FireSelector_Modes2[m_fireSelectorMode].SelectorPosition;
					break;
				case Axis.Z:
					zero3.z = FireSelector_Modes2[m_fireSelectorMode].SelectorPosition;
					break;
				}
				FireSelectorSwitch2.localPosition = zero3;
				break;
			}
			}
		}

		public void EjectExtractedRound()
		{
			if (Chamber.IsFull)
			{
				Chamber.EjectRound(RoundPos_Ejection.position, base.transform.right * EjectionSpeed.x + base.transform.up * EjectionSpeed.y + base.transform.forward * EjectionSpeed.z, base.transform.right * EjectionSpin.x + base.transform.up * EjectionSpin.y + base.transform.forward * EjectionSpin.z);
			}
		}

		public void BeginChamberingRound()
		{
			FireSelectorModeType modeType = FireSelector_Modes[m_fireSelectorMode].ModeType;
			FireSelectorMode fireSelectorMode = FireSelector_Modes[m_fireSelectorMode];
			if (modeType == FireSelectorModeType.Single || modeType == FireSelectorModeType.SuperFastBurst)
			{
				EngageSeer();
			}
			bool flag = false;
			GameObject fromPrefabReference = null;
			if (HasBelt)
			{
				if (!m_proxy.IsFull && BeltDD.HasARound())
				{
					if (AudioClipSet.BeltSettlingLimit > 0)
					{
						PlayAudioEvent(FirearmAudioEventType.BeltSettle);
					}
					flag = true;
					fromPrefabReference = BeltDD.RemoveRound(b: false);
				}
			}
			else if (!m_proxy.IsFull && Magazine != null && !Magazine.IsBeltBox && Magazine.HasARound())
			{
				flag = true;
				fromPrefabReference = Magazine.RemoveRound(b: false);
			}
			if (flag)
			{
				if (flag)
				{
					m_proxy.SetFromPrefabReference(fromPrefabReference);
				}
				if (Bolt.HasLastRoundBoltHoldOpen && Magazine != null && !Magazine.HasARound() && Magazine.DoesFollowerStopBolt && !Magazine.IsBeltBox)
				{
					EngageSeer();
				}
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

		public override Transform GetMagMountingTransform()
		{
			if (UsesMagMountTransformOverride)
			{
				return MagMountTransformOverride;
			}
			return base.GetMagMountingTransform();
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
			if (UsesDelinker && HasBelt)
			{
				DelinkerSystem.Emit(1);
			}
			if (HasBelt)
			{
				BeltDD.AddJitter();
			}
			bool twoHandStabilized = IsTwoHandStabilized();
			bool foregripStabilized = base.AltGrip != null;
			bool shoulderStabilized = IsShoulderStabilized();
			Recoil(twoHandStabilized, foregripStabilized, shoulderStabilized);
			bool flag = false;
			FireSelectorMode fireSelectorMode = FireSelector_Modes[m_fireSelectorMode];
			if (fireSelectorMode.ModeType == FireSelectorModeType.SuperFastBurst)
			{
				for (int i = 1; i < SuperBurstAmount; i++)
				{
					if (Magazine.HasARound())
					{
						Magazine.RemoveRound();
						base.Fire(Chamber, GetMuzzle(), doBuzz: false);
						flag = true;
						FireMuzzleSmoke();
						Recoil(twoHandStabilized, foregripStabilized, shoulderStabilized);
					}
				}
			}
			if (UsesRecoilingSystem)
			{
				if (flag)
				{
					RecoilingSystem.Recoil(isPowerful: true);
				}
				else
				{
					RecoilingSystem.Recoil(isPowerful: false);
				}
			}
			if (flag)
			{
				PlayAudioGunShot(IsHighPressure: false, Chamber.GetRound().TailClass, Chamber.GetRound().TailClassSuppressed, GM.CurrentPlayerBody.GetCurrentSoundEnvironment());
			}
			else
			{
				PlayAudioGunShot(Chamber.GetRound(), GM.CurrentPlayerBody.GetCurrentSoundEnvironment());
			}
			return true;
		}

		protected override void FVRUpdate()
		{
			base.FVRUpdate();
			UpdateControls();
			Bolt.UpdateBolt();
			UpdateDisplayRoundPositions();
			if (m_timeSinceFiredShot < 1f)
			{
				m_timeSinceFiredShot += Time.deltaTime;
			}
		}

		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);
			if (Trigger != null)
			{
				if (TriggerInterpStyle == InterpStyle.Translate)
				{
					Trigger.localPosition = new Vector3(0f, 0f, Mathf.Lerp(Trigger_ForwardValue, Trigger_RearwardValue, m_triggerFloat));
				}
				else if (TriggerInterpStyle == InterpStyle.Rotation)
				{
					Trigger.localEulerAngles = new Vector3(Mathf.Lerp(Trigger_ForwardValue, Trigger_RearwardValue, m_triggerFloat), 0f, 0f);
				}
			}
		}

		public override void EndInteraction(FVRViveHand hand)
		{
			EngageSeer();
			m_hasTriggerCycled = false;
			m_triggerFloat = 0f;
			if (Trigger != null)
			{
				if (TriggerInterpStyle == InterpStyle.Translate)
				{
					Trigger.localPosition = new Vector3(0f, 0f, Mathf.Lerp(Trigger_ForwardValue, Trigger_RearwardValue, m_triggerFloat));
				}
				else if (TriggerInterpStyle == InterpStyle.Rotation)
				{
					Trigger.localEulerAngles = new Vector3(Mathf.Lerp(Trigger_ForwardValue, Trigger_RearwardValue, m_triggerFloat), 0f, 0f);
				}
			}
			base.EndInteraction(hand);
		}

		public override void EndInteractionIntoInventorySlot(FVRViveHand hand, FVRQuickBeltSlot slot)
		{
			EngageSeer();
			m_hasTriggerCycled = false;
			base.EndInteractionIntoInventorySlot(hand, slot);
		}

		private void UpdateControls()
		{
			if (base.IsHeld)
			{
				if (HasTriggerButton && m_hasTriggeredUpSinceBegin && !IsAltHeld && FireSelector_Modes[m_fireSelectorMode].ModeType != 0)
				{
					m_triggerFloat = m_hand.Input.TriggerFloat;
				}
				else
				{
					m_triggerFloat = 0f;
				}
				bool flag = false;
				if (Bolt.HasLastRoundBoltHoldOpen && Magazine != null && !Magazine.HasARound() && !Magazine.IsBeltBox)
				{
					flag = true;
				}
				if (!m_hasTriggerCycled)
				{
					if (m_triggerFloat >= TriggerFiringThreshold)
					{
						m_hasTriggerCycled = true;
						if (!flag)
						{
							ReleaseSeer();
						}
					}
				}
				else if (m_triggerFloat <= TriggerResetThreshold && m_hasTriggerCycled)
				{
					EngageSeer();
					m_hasTriggerCycled = false;
					PlayAudioEvent(FirearmAudioEventType.TriggerReset);
				}
				if (IsAltHeld)
				{
					return;
				}
				if (m_hand.IsInStreamlinedMode)
				{
					if (m_hand.Input.BYButtonDown && HasFireSelectorButton)
					{
						ToggleFireSelector();
					}
					if (m_hand.Input.AXButtonDown && HasMagReleaseButton)
					{
						EjectMag();
					}
				}
				else if (m_hand.Input.TouchpadDown && m_hand.Input.TouchpadAxes.magnitude > 0.1f)
				{
					if (HasFireSelectorButton && Vector2.Angle(m_hand.Input.TouchpadAxes, Vector2.left) <= 45f)
					{
						ToggleFireSelector();
					}
					else if (HasMagReleaseButton && Vector2.Angle(m_hand.Input.TouchpadAxes, Vector2.down) <= 45f)
					{
						EjectMag();
					}
				}
			}
			else
			{
				m_triggerFloat = 0f;
			}
		}

		private void UpdateDisplayRoundPositions()
		{
			float boltLerpBetweenLockAndFore = Bolt.GetBoltLerpBetweenLockAndFore();
			if (m_proxy.IsFull)
			{
				m_proxy.ProxyRound.position = Vector3.Lerp(RoundPos_MagazinePos.position, Chamber.transform.position, boltLerpBetweenLockAndFore);
				m_proxy.ProxyRound.rotation = Quaternion.Slerp(RoundPos_MagazinePos.rotation, Chamber.transform.rotation, boltLerpBetweenLockAndFore);
			}
			else if (Chamber.IsFull)
			{
				Chamber.ProxyRound.position = Vector3.Lerp(RoundPos_Ejecting.position, Chamber.transform.position, boltLerpBetweenLockAndFore);
				Chamber.ProxyRound.rotation = Quaternion.Slerp(RoundPos_Ejecting.rotation, Chamber.transform.rotation, boltLerpBetweenLockAndFore);
			}
			if (!DoesForwardBoltDisableReloadWell)
			{
				return;
			}
			if (Bolt.CurPos >= OpenBoltReceiverBolt.BoltPos.Locked)
			{
				if (!ReloadTriggerWell.activeSelf)
				{
					ReloadTriggerWell.SetActive(value: true);
				}
			}
			else if (ReloadTriggerWell.activeSelf)
			{
				ReloadTriggerWell.SetActive(value: false);
			}
		}

		public void ReleaseMag()
		{
			if (Magazine != null)
			{
				base.EjectMag();
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
	}
}
