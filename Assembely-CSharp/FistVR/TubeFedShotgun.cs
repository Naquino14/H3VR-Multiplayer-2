using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class TubeFedShotgun : FVRFireArm
	{
		public enum ShotgunMode
		{
			PumpMode,
			Automatic
		}

		[Header("Shotgun Params")]
		public TubeFedShotgunBolt Bolt;

		public TubeFedShotgunHandle Handle;

		public FVRFireArmChamber Chamber;

		public bool HasHandle;

		[Header("Component Connections")]
		public Transform Trigger;

		public Transform Safety;

		public GameObject ReloadTriggerWell;

		[Header("Carrier System")]
		public bool UsesAnimatedCarrier;

		public Transform Carrier;

		public Vector2 CarrierRots;

		public Transform CarrierComparePoint1;

		public Transform CarrierComparePoint2;

		public float CarrierDetectDistance;

		private float m_curCarrierRot;

		private float m_tarCarrierRot;

		[Header("Round Positions")]
		public Transform RoundPos_LowerPath_Forward;

		public Transform RoundPos_LowerPath_Rearward;

		public Transform RoundPos_UpperPath_Forward;

		public Transform RoundPos_UpperPath_Rearward;

		public Transform RoundPos_Ejecting;

		public Transform RoundPos_Ejection;

		public Vector3 RoundEjectionSpeed;

		public Vector3 RoundEjectionSpin;

		private FVRFirearmMovingProxyRound m_proxy;

		private bool m_isExtractedRoundOnLowerPath = true;

		[Header("Trigger Params")]
		public bool HasTrigger;

		public InterpStyle TriggerInterp;

		public Axis TriggerAxis;

		public float TriggerUnheld;

		public float TriggerHeld;

		public float TriggerResetThreshold = 0.45f;

		public float TriggerBreakThreshold = 0.85f;

		private float m_triggerFloat;

		private bool m_hasTriggerReset = true;

		public bool UsesSlamFireTrigger;

		[Header("Safety Params")]
		public bool HasSafety;

		public InterpStyle Safety_Interp;

		public Axis SafetyAxis;

		public float SafetyOff;

		public float SafetyOn;

		private bool m_isSafetyEngaged = true;

		private bool m_isHammerCocked;

		[Header("Control Params")]
		public bool HasSlideReleaseButton;

		[HideInInspector]
		public bool IsSlideReleaseButtonHeld;

		[Header("Mode Params")]
		public bool CanModeSwitch;

		public ShotgunMode Mode;

		private bool m_isChamberRoundOnExtractor;

		public bool IsSafetyEngaged => m_isSafetyEngaged;

		public bool IsHammerCocked => m_isHammerCocked;

		public bool HasExtractedRound()
		{
			return m_proxy.IsFull;
		}

		protected override void Awake()
		{
			base.Awake();
			if (Mode == ShotgunMode.Automatic)
			{
				if (HasHandle)
				{
					Handle.LockHandle();
				}
			}
			else if (HasHandle)
			{
				Handle.UnlockHandle();
			}
			if (!HasSafety)
			{
				m_isSafetyEngaged = false;
			}
			GameObject gameObject = new GameObject("m_proxyRound");
			m_proxy = gameObject.AddComponent<FVRFirearmMovingProxyRound>();
			m_proxy.Init(base.transform);
		}

		public override int GetTutorialState()
		{
			if (m_isSafetyEngaged)
			{
				return 2;
			}
			if (Chamber.IsFull && !Chamber.IsSpent)
			{
				return 3;
			}
			if (Magazine == null || !Magazine.HasARound())
			{
				return 0;
			}
			return 1;
		}

		public void BoltReleasePressed()
		{
			if (Mode == ShotgunMode.Automatic)
			{
				Bolt.ReleaseBolt();
			}
		}

		public bool CanCycleMagState()
		{
			if (Handle.CurPos != 0)
			{
				return false;
			}
			if (HasExtractedRound())
			{
				return false;
			}
			return true;
		}

		public void ToggleSafety()
		{
			if (HasSafety)
			{
				m_isSafetyEngaged = !m_isSafetyEngaged;
				PlayAudioEvent(FirearmAudioEventType.Safety);
				UpdateSafetyGeo();
			}
		}

		private void UpdateSafetyGeo()
		{
			float num = 0f;
			SetAnimatedComponent(val: (!m_isSafetyEngaged) ? SafetyOff : SafetyOn, t: Safety, interp: Safety_Interp, axis: SafetyAxis);
		}

		public void EjectExtractedRound()
		{
			if (m_isChamberRoundOnExtractor)
			{
				m_isChamberRoundOnExtractor = false;
				if (Chamber.IsFull)
				{
					Chamber.EjectRound(RoundPos_Ejection.position, base.transform.right * RoundEjectionSpeed.x + base.transform.up * RoundEjectionSpeed.y + base.transform.forward * RoundEjectionSpeed.z, base.transform.right * RoundEjectionSpin.x + base.transform.up * RoundEjectionSpin.y + base.transform.forward * RoundEjectionSpin.z);
				}
			}
		}

		public void ExtractRound()
		{
			if (!(Magazine == null) && !m_proxy.IsFull && !m_proxy.IsFull && Magazine.HasARound())
			{
				GameObject fromPrefabReference = Magazine.RemoveRound(b: false);
				m_proxy.SetFromPrefabReference(fromPrefabReference);
				m_isExtractedRoundOnLowerPath = true;
			}
		}

		public bool ChamberRound()
		{
			if (Chamber.IsFull)
			{
				m_isChamberRoundOnExtractor = true;
			}
			if (m_proxy.IsFull && !Chamber.IsFull && !m_isExtractedRoundOnLowerPath)
			{
				m_isChamberRoundOnExtractor = true;
				Chamber.SetRound(m_proxy.Round);
				m_proxy.ClearProxy();
				return true;
			}
			return false;
		}

		public bool ReturnCarrierRoundToMagazineIfRelevant()
		{
			if (m_proxy.IsFull && m_isExtractedRoundOnLowerPath)
			{
				Magazine.AddRound(m_proxy.Round.RoundClass, makeSound: false, updateDisplay: true);
				m_proxy.ClearProxy();
				return true;
			}
			return false;
		}

		public void TransferShellToUpperTrack()
		{
			if (m_proxy.IsFull && m_isExtractedRoundOnLowerPath && !Chamber.IsFull)
			{
				m_isExtractedRoundOnLowerPath = false;
			}
		}

		public void ToggleMode()
		{
			if (Bolt.CurPos != 0)
			{
				Debug.Log("not forward");
				return;
			}
			if (HasHandle && Handle.CurPos != 0)
			{
				Debug.Log("not forward");
				return;
			}
			if (m_isHammerCocked)
			{
				Debug.Log("hammer cocked");
				return;
			}
			PlayAudioEvent(FirearmAudioEventType.FireSelector);
			if (Mode == ShotgunMode.PumpMode)
			{
				Mode = ShotgunMode.Automatic;
				if (HasHandle)
				{
					Handle.LockHandle();
				}
			}
			else
			{
				Mode = ShotgunMode.PumpMode;
				if (HasHandle)
				{
					Handle.UnlockHandle();
				}
			}
		}

		public void CockHammer()
		{
			if (!m_isHammerCocked)
			{
				m_isHammerCocked = true;
				PlayAudioEvent(FirearmAudioEventType.Prefire);
			}
		}

		public void ReleaseHammer()
		{
			if (m_isHammerCocked && Bolt.CurPos == TubeFedShotgunBolt.BoltPos.Forward)
			{
				PlayAudioEvent(FirearmAudioEventType.HammerHit);
				Fire();
				m_isHammerCocked = false;
				if (HasHandle && Mode == ShotgunMode.PumpMode)
				{
					Handle.UnlockHandle();
				}
			}
		}

		public bool Fire()
		{
			if (!Chamber.Fire())
			{
				return false;
			}
			base.Fire(Chamber, GetMuzzle(), doBuzz: true);
			FireMuzzleSmoke();
			bool twoHandStabilized = IsTwoHandStabilized();
			bool foregripStabilized = IsForegripStabilized();
			bool shoulderStabilized = IsShoulderStabilized();
			Recoil(twoHandStabilized, foregripStabilized, shoulderStabilized);
			PlayAudioGunShot(Chamber.GetRound(), GM.CurrentPlayerBody.GetCurrentSoundEnvironment());
			if (Mode == ShotgunMode.Automatic && Chamber.GetRound().IsHighPressure)
			{
				Bolt.ImpartFiringImpulse();
			}
			return true;
		}

		protected override void FVRUpdate()
		{
			base.FVRUpdate();
			UpdateComponents();
			UpdateCarrier();
			if (HasHandle)
			{
				Handle.UpdateHandle();
				bool state = false;
				if ((Handle.IsHeld || IsAltHeld) && Mode == ShotgunMode.PumpMode)
				{
					state = true;
				}
				Bolt.UpdateHandleHeldState(state, 1f - Handle.GetBoltLerpBetweenRearAndFore());
			}
			Bolt.UpdateBolt();
			UpdateDisplayRoundPositions();
			if (HasExtractedRound() && m_isExtractedRoundOnLowerPath)
			{
				if (Magazine != null)
				{
					Magazine.IsDropInLoadable = false;
				}
			}
			else if (Magazine != null)
			{
				Magazine.IsDropInLoadable = true;
			}
		}

		private void UpdateCarrier()
		{
			if (!UsesAnimatedCarrier)
			{
				return;
			}
			if (base.IsHeld)
			{
				if (m_hand.OtherHand.CurrentInteractable != null)
				{
					if (m_hand.OtherHand.CurrentInteractable is FVRFireArmRound)
					{
						float num = Vector3.Distance(m_hand.OtherHand.CurrentInteractable.transform.position, GetClosestValidPoint(CarrierComparePoint1.position, CarrierComparePoint2.position, m_hand.OtherHand.CurrentInteractable.transform.position));
						if (num < CarrierDetectDistance)
						{
							m_tarCarrierRot = CarrierRots.y;
						}
						else
						{
							m_tarCarrierRot = CarrierRots.x;
						}
					}
					else
					{
						m_tarCarrierRot = CarrierRots.x;
					}
				}
				else
				{
					m_tarCarrierRot = CarrierRots.x;
				}
			}
			else
			{
				m_tarCarrierRot = CarrierRots.x;
			}
			if (HasExtractedRound() && !m_isExtractedRoundOnLowerPath)
			{
				m_tarCarrierRot = CarrierRots.y;
			}
			if (Mathf.Abs(m_curCarrierRot - m_tarCarrierRot) > 0.001f)
			{
				m_curCarrierRot = Mathf.MoveTowards(m_curCarrierRot, m_tarCarrierRot, 270f * Time.deltaTime);
				Carrier.localEulerAngles = new Vector3(m_curCarrierRot, 0f, 0f);
			}
		}

		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);
			UpdateInputAndAnimate(hand);
		}

		private void UpdateInputAndAnimate(FVRViveHand hand)
		{
			IsSlideReleaseButtonHeld = false;
			if (IsAltHeld)
			{
				return;
			}
			if (m_hasTriggeredUpSinceBegin)
			{
				m_triggerFloat = hand.Input.TriggerFloat;
			}
			else
			{
				m_triggerFloat = 0f;
			}
			if (!m_hasTriggerReset && m_triggerFloat <= TriggerResetThreshold)
			{
				m_hasTriggerReset = true;
				PlayAudioEvent(FirearmAudioEventType.TriggerReset);
			}
			Vector2 touchpadAxes = hand.Input.TouchpadAxes;
			if (hand.IsInStreamlinedMode)
			{
				if (hand.Input.BYButtonDown)
				{
					ToggleSafety();
				}
				if (hand.Input.AXButtonPressed)
				{
					IsSlideReleaseButtonHeld = true;
					if (HasHandle && Mode == ShotgunMode.PumpMode)
					{
						Handle.UnlockHandle();
					}
				}
			}
			else
			{
				if (hand.Input.TouchpadDown && touchpadAxes.magnitude > 0.2f)
				{
					if (Vector2.Angle(touchpadAxes, Vector2.left) <= 45f)
					{
						ToggleSafety();
					}
					else if (Vector2.Angle(touchpadAxes, Vector2.up) <= 45f && Mode == ShotgunMode.Automatic)
					{
						Bolt.ReleaseBolt();
					}
				}
				if (hand.Input.TouchpadPressed && touchpadAxes.magnitude > 0.2f && Vector2.Angle(touchpadAxes, Vector2.up) <= 45f)
				{
					IsSlideReleaseButtonHeld = true;
					if (HasHandle && Mode == ShotgunMode.PumpMode)
					{
						Handle.UnlockHandle();
					}
				}
			}
			if (m_triggerFloat >= TriggerBreakThreshold && m_isHammerCocked && !m_isSafetyEngaged)
			{
				if (m_hasTriggerReset || UsesSlamFireTrigger)
				{
					ReleaseHammer();
				}
				m_hasTriggerReset = false;
			}
		}

		private void UpdateComponents()
		{
			if (HasTrigger)
			{
				SetAnimatedComponent(Trigger, Mathf.Lerp(TriggerUnheld, TriggerHeld, m_triggerFloat), TriggerInterp, TriggerAxis);
			}
		}

		private void UpdateDisplayRoundPositions()
		{
			float boltLerpBetweenLockAndFore = Bolt.GetBoltLerpBetweenLockAndFore();
			if (Chamber.IsFull)
			{
				if (m_isChamberRoundOnExtractor)
				{
					Chamber.ProxyRound.position = Vector3.Lerp(RoundPos_Ejecting.position, Chamber.transform.position, boltLerpBetweenLockAndFore);
					Chamber.ProxyRound.rotation = Quaternion.Slerp(RoundPos_Ejecting.rotation, Chamber.transform.rotation, boltLerpBetweenLockAndFore);
				}
				else
				{
					Chamber.ProxyRound.position = Chamber.transform.position;
					Chamber.ProxyRound.rotation = Chamber.transform.rotation;
				}
			}
			if (m_proxy.IsFull)
			{
				if (m_isExtractedRoundOnLowerPath || Chamber.IsFull)
				{
					m_proxy.ProxyRound.position = Vector3.Lerp(RoundPos_LowerPath_Rearward.position, RoundPos_LowerPath_Forward.position, boltLerpBetweenLockAndFore);
					m_proxy.ProxyRound.rotation = Quaternion.Slerp(RoundPos_LowerPath_Rearward.rotation, RoundPos_LowerPath_Forward.rotation, boltLerpBetweenLockAndFore);
				}
				else
				{
					m_proxy.ProxyRound.position = Vector3.Lerp(RoundPos_UpperPath_Rearward.position, RoundPos_UpperPath_Forward.position, boltLerpBetweenLockAndFore);
					m_proxy.ProxyRound.rotation = Quaternion.Slerp(RoundPos_UpperPath_Rearward.rotation, RoundPos_UpperPath_Forward.rotation, boltLerpBetweenLockAndFore);
				}
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
					m_isHammerCocked = true;
				}
			}
			if (!HasSafety)
			{
				return;
			}
			empty = "SafetyState";
			if (f.ContainsKey(empty))
			{
				empty2 = f[empty];
				if (empty2 == "On")
				{
					m_isSafetyEngaged = true;
				}
				UpdateSafetyGeo();
			}
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
			return dictionary;
		}
	}
}
