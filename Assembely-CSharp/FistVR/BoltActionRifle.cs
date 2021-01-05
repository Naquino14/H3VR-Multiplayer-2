using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class BoltActionRifle : FVRFireArm
	{
		public enum ZPos
		{
			Forward,
			Middle,
			Rear
		}

		public enum HammerCockType
		{
			OnBack,
			OnUp,
			OnClose,
			OnForward
		}

		public enum FireSelectorModeType
		{
			Safe,
			Single
		}

		[Serializable]
		public class FireSelectorMode
		{
			public float SelectorPosition;

			public FireSelectorModeType ModeType;

			public bool IsBoltLocked;
		}

		[Header("BoltActionRifle Config")]
		public FVRFireArmChamber Chamber;

		public bool HasMagEjectionButton = true;

		public bool HasFireSelectorButton = true;

		public BoltActionRifle_Handle BoltHandle;

		public float BoltLerp;

		public bool BoltMovingForward;

		public BoltActionRifle_Handle.BoltActionHandleState CurBoltHandleState;

		public BoltActionRifle_Handle.BoltActionHandleState LastBoltHandleState;

		[Header("Hammer Config")]
		public bool HasVisualHammer;

		public Transform Hammer;

		public float HammerUncocked;

		public float HammerCocked;

		private bool m_isHammerCocked;

		public HammerCockType CockType;

		private FVRFirearmMovingProxyRound m_proxy;

		[Header("Round Positions Config")]
		public Transform Extraction_MagazinePos;

		public Transform Extraction_ChamberPos;

		public Transform Extraction_Ejecting;

		public Transform EjectionPos;

		public float UpwardEjectionForce;

		public float RightwardEjectionForce = 2f;

		public float YSpinEjectionTorque = 80f;

		public Transform Muzzle;

		public GameObject ReloadTriggerWell;

		[Header("Control Config")]
		public float TriggerResetThreshold = 0.1f;

		public float TriggerFiringThreshold = 0.8f;

		private float m_triggerFloat;

		private bool m_hasTriggerCycled;

		private bool m_isMagReleasePressed;

		public Transform Trigger_Display;

		public float Trigger_ForwardValue;

		public float Trigger_RearwardValue;

		public InterpStyle TriggerInterpStyle = InterpStyle.Rotation;

		public Transform Trigger_Display2;

		public float Trigger_ForwardValue2;

		public float Trigger_RearwardValue2;

		public InterpStyle TriggerInterpStyle2 = InterpStyle.Rotation;

		public Transform MagReleaseButton_Display;

		public Axis MagReleaseAxis;

		public InterpStyle MagReleaseInterpStyle = InterpStyle.Rotation;

		public float MagReleasePressedValue;

		public float MagReleaseUnpressedValue;

		private float m_magReleaseCurValue;

		private float m_magReleaseTarValue;

		private Vector2 TouchPadAxes = Vector2.zero;

		public Transform FireSelector_Display;

		public Axis FireSelector_Axis;

		public InterpStyle FireSelector_InterpStyle = InterpStyle.Rotation;

		public FireSelectorMode[] FireSelector_Modes;

		private int m_fireSelectorMode;

		public bool RequiresHammerUncockedToToggleFireSelector;

		public bool UsesSecondFireSelectorChange;

		public Transform FireSelector_Display_Secondary;

		public Axis FireSelector_Axis_Secondary;

		public InterpStyle FireSelector_InterpStyle_Secondary = InterpStyle.Rotation;

		public FireSelectorMode[] FireSelector_Modes_Secondary;

		[Header("Special Features")]
		public bool EjectsMagazineOnEmpty;

		public bool PlaysExtraTailOnShot;

		public FVRTailSoundClass ExtraTail = FVRTailSoundClass.Explosion;

		[Header("Reciprocating Barrel")]
		public bool HasReciprocatingBarrel;

		public G11RecoilingSystem RecoilSystem;

		private bool m_isQuickboltTouching;

		private Vector2 lastTPTouchPoint = Vector2.zero;

		public bool IsHammerCocked => m_isHammerCocked;

		public bool HasExtractedRound()
		{
			return m_proxy.IsFull;
		}

		protected override void Awake()
		{
			base.Awake();
			if (UsesClips && ClipTrigger != null)
			{
				if (CurBoltHandleState == BoltActionRifle_Handle.BoltActionHandleState.Rear)
				{
					if (!ClipTrigger.activeSelf)
					{
						ClipTrigger.SetActive(value: true);
					}
				}
				else if (ClipTrigger.activeSelf)
				{
					ClipTrigger.SetActive(value: false);
				}
			}
			GameObject gameObject = new GameObject("m_proxyRound");
			m_proxy = gameObject.AddComponent<FVRFirearmMovingProxyRound>();
			m_proxy.Init(base.transform);
		}

		public bool CanBoltMove()
		{
			if (FireSelector_Modes.Length < 1)
			{
				return true;
			}
			if (FireSelector_Modes[m_fireSelectorMode].IsBoltLocked)
			{
				return false;
			}
			return true;
		}

		public override int GetTutorialState()
		{
			if (FireSelector_Modes[m_fireSelectorMode].ModeType == FireSelectorModeType.Safe)
			{
				return 4;
			}
			if (Chamber.IsFull)
			{
				if (Chamber.IsSpent)
				{
					return 0;
				}
				if (base.AltGrip != null)
				{
					return 6;
				}
				return 5;
			}
			if (Magazine != null && !Magazine.HasARound())
			{
				if (CurBoltHandleState == BoltActionRifle_Handle.BoltActionHandleState.Forward || CurBoltHandleState == BoltActionRifle_Handle.BoltActionHandleState.Mid)
				{
					return 0;
				}
				if (Clip != null)
				{
					return 2;
				}
				if (!Magazine.IsFull())
				{
					return 1;
				}
				return 3;
			}
			return 3;
		}

		public override void BeginInteraction(FVRViveHand hand)
		{
			base.BeginInteraction(hand);
		}

		public override void UpdateInteraction(FVRViveHand hand)
		{
			TouchPadAxes = hand.Input.TouchpadAxes;
			if (m_hasTriggeredUpSinceBegin)
			{
				m_triggerFloat = hand.Input.TriggerFloat;
			}
			if (!m_hasTriggerCycled)
			{
				if (m_triggerFloat >= TriggerFiringThreshold)
				{
					m_hasTriggerCycled = true;
				}
			}
			else if (m_triggerFloat <= TriggerResetThreshold)
			{
				m_hasTriggerCycled = false;
			}
			m_isMagReleasePressed = false;
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			if (hand.IsInStreamlinedMode)
			{
				if (HasFireSelectorButton && hand.Input.BYButtonDown)
				{
					ToggleFireSelector();
				}
				if (HasMagEjectionButton && hand.Input.AXButtonPressed)
				{
					m_isMagReleasePressed = true;
				}
			}
			else
			{
				if (HasMagEjectionButton && hand.Input.TouchpadPressed && m_hasTriggeredUpSinceBegin && TouchPadAxes.magnitude > 0.3f && Vector2.Angle(TouchPadAxes, Vector2.down) <= 45f)
				{
					m_isMagReleasePressed = true;
				}
				if (GM.Options.QuickbeltOptions.BoltActionModeSetting == QuickbeltOptions.BoltActionMode.Quickbolting)
				{
					flag3 = true;
				}
				if (GM.Options.QuickbeltOptions.BoltActionModeSetting == QuickbeltOptions.BoltActionMode.Slidebolting)
				{
					flag2 = true;
				}
				if (GM.Options.ControlOptions.UseGunRigMode2)
				{
					flag2 = true;
					flag3 = false;
				}
				if (base.Bipod != null && base.Bipod.IsBipodActive)
				{
					flag2 = true;
					flag3 = false;
				}
				if (!CanBoltMove())
				{
					flag2 = false;
					flag3 = false;
				}
				if (IsHammerCocked && BoltHandle.HandleState == BoltActionRifle_Handle.BoltActionHandleState.Forward && BoltHandle.HandleRot == BoltActionRifle_Handle.BoltActionHandleRot.Down)
				{
					flag2 = false;
				}
				if (hand.Input.TouchpadDown && TouchPadAxes.magnitude > 0.1f)
				{
					if (flag3 && Vector2.Angle(TouchPadAxes, Vector2.right) <= 45f && BoltHandle.UsesQuickRelease && BoltHandle.HandleState == BoltActionRifle_Handle.BoltActionHandleState.Forward)
					{
						flag = true;
					}
					else if (Vector2.Angle(TouchPadAxes, Vector2.left) <= 45f && HasFireSelectorButton)
					{
						ToggleFireSelector();
					}
				}
			}
			if (m_isMagReleasePressed)
			{
				ReleaseMag();
				if (ReloadTriggerWell != null)
				{
					ReloadTriggerWell.SetActive(value: false);
				}
			}
			else if (ReloadTriggerWell != null)
			{
				ReloadTriggerWell.SetActive(value: true);
			}
			if (m_hasTriggeredUpSinceBegin && !flag && flag2)
			{
				if ((base.AltGrip != null && !IsAltHeld) || GM.Options.ControlOptions.UseGunRigMode2 || (base.Bipod != null && base.Bipod.IsBipodActive))
				{
					if (hand.Input.TouchpadTouched)
					{
						Vector2 touchpadAxes = hand.Input.TouchpadAxes;
						if (touchpadAxes.magnitude > 0.1f)
						{
							bool isQuickboltTouching = m_isQuickboltTouching;
							if (Vector2.Angle(touchpadAxes, Vector2.right + Vector2.up) < 90f && !m_isQuickboltTouching)
							{
								m_isQuickboltTouching = true;
							}
							if (m_isQuickboltTouching && isQuickboltTouching)
							{
								float sAngle = GetSAngle(touchpadAxes, lastTPTouchPoint, hand.CMode);
								BoltHandle.DriveBolt((0f - sAngle) / 90f);
							}
							lastTPTouchPoint = touchpadAxes;
						}
						else
						{
							lastTPTouchPoint = Vector2.zero;
						}
					}
					else
					{
						lastTPTouchPoint = Vector2.zero;
					}
				}
				if (m_isQuickboltTouching)
				{
					Debug.DrawLine(BoltHandle.BoltActionHandleRoot.transform.position, BoltHandle.BoltActionHandleRoot.transform.position + 0.1f * new Vector3(lastTPTouchPoint.x, lastTPTouchPoint.y, 0f), Color.blue);
				}
			}
			if (hand.Input.TouchpadTouchUp)
			{
				m_isQuickboltTouching = false;
				lastTPTouchPoint = Vector2.zero;
			}
			FiringSystem();
			base.UpdateInteraction(hand);
			if (flag && !IsAltHeld && base.AltGrip != null)
			{
				m_isQuickboltTouching = false;
				lastTPTouchPoint = Vector2.zero;
				hand.Buzz(hand.Buzzer.Buzz_BeginInteraction);
				hand.HandMadeGrabReleaseSound();
				hand.EndInteractionIfHeld(this);
				EndInteraction(hand);
				BoltHandle.BeginInteraction(hand);
				hand.ForceSetInteractable(BoltHandle);
				BoltHandle.TPInitiate();
			}
		}

		public float GetSignedAngle(Vector2 from, Vector2 to)
		{
			Vector2 normalized = new Vector2(from.y, 0f - from.x).normalized;
			float num = Mathf.Sign(Vector2.Dot(from, normalized));
			float num2 = Vector2.Angle(from, to);
			return num2 * num;
		}

		private float GetSAngle(Vector2 v1, Vector2 v2, ControlMode m)
		{
			if (m == ControlMode.Index)
			{
				return (v1.y - v2.y) * 130f;
			}
			float num = Mathf.Sign(v1.x * v2.y - v1.y * v2.x);
			return Vector2.Angle(v1, v2) * num;
		}

		public override void EndInteraction(FVRViveHand hand)
		{
			m_triggerFloat = 0f;
			m_hasTriggerCycled = false;
			m_isMagReleasePressed = false;
			m_isQuickboltTouching = false;
			lastTPTouchPoint = Vector2.zero;
			base.EndInteraction(hand);
		}

		public void SetHasTriggeredUp()
		{
			m_hasTriggeredUpSinceBegin = true;
		}

		public void CockHammer()
		{
			if (!m_isHammerCocked)
			{
				m_isHammerCocked = true;
				if (HasVisualHammer)
				{
					SetAnimatedComponent(Hammer, HammerCocked, InterpStyle.Translate, Axis.Z);
				}
			}
		}

		public void DropHammer()
		{
			if (IsHammerCocked)
			{
				m_isHammerCocked = false;
				PlayAudioEvent(FirearmAudioEventType.HammerHit);
				Fire();
				if (HasVisualHammer)
				{
					SetAnimatedComponent(Hammer, HammerUncocked, InterpStyle.Translate, Axis.Z);
				}
			}
		}

		protected virtual void ToggleFireSelector()
		{
			if ((RequiresHammerUncockedToToggleFireSelector && IsHammerCocked) || FireSelector_Modes.Length <= 1)
			{
				return;
			}
			m_fireSelectorMode++;
			if (m_fireSelectorMode >= FireSelector_Modes.Length)
			{
				m_fireSelectorMode -= FireSelector_Modes.Length;
			}
			PlayAudioEvent(FirearmAudioEventType.FireSelector);
			if (FireSelector_Display != null)
			{
				SetAnimatedComponent(FireSelector_Display, FireSelector_Modes[m_fireSelectorMode].SelectorPosition, FireSelector_InterpStyle, FireSelector_Axis);
				if (UsesSecondFireSelectorChange)
				{
					SetAnimatedComponent(FireSelector_Display_Secondary, FireSelector_Modes_Secondary[m_fireSelectorMode].SelectorPosition, FireSelector_InterpStyle_Secondary, FireSelector_Axis_Secondary);
				}
			}
		}

		public void ReleaseMag()
		{
			if (Magazine != null)
			{
				m_magReleaseCurValue = MagReleasePressedValue;
				base.EjectMag();
			}
		}

		public FireSelectorMode GetFiringMode()
		{
			return FireSelector_Modes[m_fireSelectorMode];
		}

		protected virtual void FiringSystem()
		{
			if (FireSelector_Modes[m_fireSelectorMode].ModeType != 0 && !IsAltHeld && BoltHandle.HandleState == BoltActionRifle_Handle.BoltActionHandleState.Forward && BoltHandle.HandleRot != 0 && m_hasTriggerCycled)
			{
				DropHammer();
			}
		}

		public bool Fire()
		{
			FireSelectorMode fireSelectorMode = FireSelector_Modes[m_fireSelectorMode];
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
			FVRSoundEnvironment currentSoundEnvironment = GM.CurrentPlayerBody.GetCurrentSoundEnvironment();
			PlayAudioGunShot(Chamber.GetRound(), currentSoundEnvironment);
			if (PlaysExtraTailOnShot)
			{
				AudioEvent tailSet = SM.GetTailSet(ExtraTail, currentSoundEnvironment);
				m_pool_tail.PlayClipVolumePitchOverride(tailSet, base.transform.position, tailSet.VolumeRange * 1f, AudioClipSet.TailPitchMod_Main * tailSet.PitchRange.x);
			}
			if (HasReciprocatingBarrel)
			{
				RecoilSystem.Recoil(isPowerful: false);
			}
			return true;
		}

		protected override void FVRFixedUpdate()
		{
			base.FVRFixedUpdate();
			UpdateComponentDisplay();
		}

		private void UpdateComponentDisplay()
		{
			if (Trigger_Display != null)
			{
				if (TriggerInterpStyle == InterpStyle.Translate)
				{
					Trigger_Display.localPosition = new Vector3(0f, 0f, Mathf.Lerp(Trigger_ForwardValue, Trigger_RearwardValue, m_triggerFloat));
				}
				else if (TriggerInterpStyle == InterpStyle.Rotation)
				{
					Trigger_Display.localEulerAngles = new Vector3(Mathf.Lerp(Trigger_ForwardValue, Trigger_RearwardValue, m_triggerFloat), 0f, 0f);
				}
			}
			if (Trigger_Display2 != null)
			{
				if (TriggerInterpStyle == InterpStyle.Translate)
				{
					Trigger_Display2.localPosition = new Vector3(0f, 0f, Mathf.Lerp(Trigger_ForwardValue2, Trigger_RearwardValue2, m_triggerFloat));
				}
				else if (TriggerInterpStyle == InterpStyle.Rotation)
				{
					Trigger_Display2.localEulerAngles = new Vector3(Mathf.Lerp(Trigger_ForwardValue2, Trigger_RearwardValue2, m_triggerFloat), 0f, 0f);
				}
			}
			if (MagReleaseButton_Display != null)
			{
				Vector3 zero = Vector3.zero;
				if (m_isMagReleasePressed)
				{
					m_magReleaseTarValue = MagReleasePressedValue;
				}
				else
				{
					m_magReleaseTarValue = MagReleaseUnpressedValue;
				}
				m_magReleaseCurValue = Mathf.Lerp(m_magReleaseCurValue, m_magReleaseTarValue, Time.deltaTime * 4f);
				float magReleaseCurValue = m_magReleaseCurValue;
				switch (MagReleaseAxis)
				{
				case Axis.X:
					zero.x = magReleaseCurValue;
					break;
				case Axis.Y:
					zero.y = magReleaseCurValue;
					break;
				case Axis.Z:
					zero.z = magReleaseCurValue;
					break;
				}
				switch (MagReleaseInterpStyle)
				{
				case InterpStyle.Translate:
					MagReleaseButton_Display.localPosition = zero;
					break;
				case InterpStyle.Rotation:
					MagReleaseButton_Display.localEulerAngles = zero;
					break;
				}
			}
			if (CurBoltHandleState == BoltActionRifle_Handle.BoltActionHandleState.Forward)
			{
				IsBreachOpenForGasOut = false;
			}
			else
			{
				IsBreachOpenForGasOut = true;
			}
		}

		public void UpdateBolt(BoltActionRifle_Handle.BoltActionHandleState State, float lerp)
		{
			CurBoltHandleState = State;
			BoltLerp = lerp;
			if (CurBoltHandleState != 0 && !m_proxy.IsFull && !Chamber.IsFull)
			{
				Chamber.IsAccessible = true;
			}
			else
			{
				Chamber.IsAccessible = false;
			}
			if (UsesClips && ClipTrigger != null)
			{
				if (CurBoltHandleState == BoltActionRifle_Handle.BoltActionHandleState.Rear)
				{
					if (!ClipTrigger.activeSelf)
					{
						ClipTrigger.SetActive(value: true);
					}
				}
				else if (ClipTrigger.activeSelf)
				{
					ClipTrigger.SetActive(value: false);
				}
			}
			if (CurBoltHandleState == BoltActionRifle_Handle.BoltActionHandleState.Rear && LastBoltHandleState != BoltActionRifle_Handle.BoltActionHandleState.Rear)
			{
				if (CockType == HammerCockType.OnBack)
				{
					CockHammer();
				}
				if (Chamber.IsFull)
				{
					Chamber.EjectRound(EjectionPos.position, base.transform.right * RightwardEjectionForce + base.transform.up * UpwardEjectionForce, base.transform.up * YSpinEjectionTorque);
					PlayAudioEvent(FirearmAudioEventType.HandleBack);
				}
				else
				{
					PlayAudioEvent(FirearmAudioEventType.HandleBackEmpty);
				}
				BoltMovingForward = true;
			}
			else if (CurBoltHandleState == BoltActionRifle_Handle.BoltActionHandleState.Forward && LastBoltHandleState != 0)
			{
				if (CockType == HammerCockType.OnForward)
				{
					CockHammer();
				}
				if (m_proxy.IsFull && !Chamber.IsFull)
				{
					Chamber.SetRound(m_proxy.Round);
					m_proxy.ClearProxy();
					PlayAudioEvent(FirearmAudioEventType.HandleForward);
				}
				else
				{
					PlayAudioEvent(FirearmAudioEventType.HandleForwardEmpty);
				}
				BoltMovingForward = false;
			}
			else if (CurBoltHandleState == BoltActionRifle_Handle.BoltActionHandleState.Mid && LastBoltHandleState == BoltActionRifle_Handle.BoltActionHandleState.Rear && Magazine != null)
			{
				if (!m_proxy.IsFull && Magazine.HasARound() && !Chamber.IsFull)
				{
					GameObject fromPrefabReference = Magazine.RemoveRound(b: false);
					m_proxy.SetFromPrefabReference(fromPrefabReference);
				}
				if (EjectsMagazineOnEmpty && !Magazine.HasARound())
				{
					EjectMag();
				}
			}
			if (m_proxy.IsFull)
			{
				m_proxy.ProxyRound.position = Vector3.Lerp(Extraction_ChamberPos.position, Extraction_MagazinePos.position, BoltLerp);
				m_proxy.ProxyRound.rotation = Quaternion.Slerp(Extraction_ChamberPos.rotation, Extraction_MagazinePos.rotation, BoltLerp);
			}
			if (Chamber.IsFull)
			{
				Chamber.ProxyRound.position = Vector3.Lerp(Extraction_ChamberPos.position, Extraction_Ejecting.position, BoltLerp);
				Chamber.ProxyRound.rotation = Quaternion.Slerp(Extraction_ChamberPos.rotation, Extraction_Ejecting.rotation, BoltLerp);
			}
			LastBoltHandleState = CurBoltHandleState;
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
				if (HasVisualHammer)
				{
					SetAnimatedComponent(Hammer, HammerCocked, InterpStyle.Translate, Axis.Z);
				}
			}
			if (FireSelector_Modes.Length > 1)
			{
				empty = "FireSelectorState";
				if (f.ContainsKey(empty))
				{
					empty2 = f[empty];
					int.TryParse(empty2, out m_fireSelectorMode);
				}
				if (FireSelector_Display != null)
				{
					FireSelectorMode fireSelectorMode = FireSelector_Modes[m_fireSelectorMode];
					SetAnimatedComponent(FireSelector_Display, fireSelectorMode.SelectorPosition, FireSelector_InterpStyle, FireSelector_Axis);
				}
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
			if (FireSelector_Modes.Length > 1)
			{
				key = "FireSelectorState";
				value = m_fireSelectorMode.ToString();
				dictionary.Add(key, value);
			}
			return dictionary;
		}
	}
}
