using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class BAP : FVRFireArm
	{
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

		public BAPHandle Handle;

		public Transform Muzzle;

		public GameObject ReloadTriggerWell;

		public bool HasMagEjectionButton = true;

		private FVRFirearmMovingProxyRound m_proxy;

		public Transform Extraction_MagazinePos;

		public Transform Extraction_ChamberPos;

		public Transform Extraction_Ejecting;

		public Transform EjectionPos;

		public float UpwardEjectionForce;

		public float RightwardEjectionForce = 2f;

		public float YSpinEjectionTorque = 80f;

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

		public Transform MagReleaseButton_Display;

		public Axis MagReleaseAxis;

		public InterpStyle MagReleaseInterpStyle = InterpStyle.Rotation;

		public float MagReleasePressedValue;

		public float MagReleaseUnpressedValue;

		private float m_magReleaseCurValue;

		private float m_magReleaseTarValue;

		private Vector2 TouchPadAxes = Vector2.zero;

		public bool HasFireSelectorButton;

		public Transform FireSelector_Display;

		public Axis FireSelector_Axis;

		public InterpStyle FireSelector_InterpStyle = InterpStyle.Rotation;

		public FireSelectorMode[] FireSelector_Modes;

		private int m_fireSelectorMode;

		public bool RequiresHammerUncockedToToggleFireSelector;

		[Header("Baffle")]
		public GameObject BaffleRoot;

		public List<GameObject> BaffleStates;

		public GameObject Cap;

		public bool HasCap = true;

		public bool HasBaffle = true;

		public int BaffleState;

		public GameObject Prefab_Baffle;

		public GameObject Prefab_Cap;

		public Transform PPoint_Baffle;

		public Transform PPoint_Cap;

		private float m_timeTilCanDetectPiece = 1f;

		public AudioEvent AudClip_InsertCap;

		public AudioEvent AudClip_RemoveCap;

		public AudioEvent AudClip_InsertBaffle;

		public AudioEvent AudClip_RemoveBaffle;

		private bool m_isHammerCocked;

		private bool BoltMovingForward;

		public bool HasExtractedRound()
		{
			return m_proxy.IsFull;
		}

		public void RemoveThing()
		{
			m_timeTilCanDetectPiece = 1f;
		}

		public bool CanDetectPiece()
		{
			if (m_timeTilCanDetectPiece > 0f)
			{
				return false;
			}
			return true;
		}

		public void SetCapState(bool hasC)
		{
			if (hasC && !HasCap)
			{
				PlayAudioAsHandling(AudClip_InsertCap, Cap.transform.position);
			}
			else if (!hasC && HasCap)
			{
				PlayAudioAsHandling(AudClip_RemoveCap, Cap.transform.position);
			}
			HasCap = hasC;
			Cap.SetActive(HasCap);
		}

		public void SetBaffleState(bool hasB, int s)
		{
			BaffleState = s;
			if (hasB && !HasBaffle)
			{
				PlayAudioAsHandling(AudClip_InsertBaffle, Cap.transform.position);
			}
			else if (!hasB && HasBaffle)
			{
				PlayAudioAsHandling(AudClip_RemoveBaffle, Cap.transform.position);
			}
			HasBaffle = hasB;
			BaffleRoot.SetActive(HasBaffle);
			for (int i = 0; i < BaffleStates.Count; i++)
			{
				if (i == BaffleState)
				{
					BaffleStates[i].SetActive(value: true);
				}
				else
				{
					BaffleStates[i].SetActive(value: false);
				}
			}
			if (!HasBaffle)
			{
				DefaultMuzzleState = MuzzleState.None;
			}
			else
			{
				DefaultMuzzleState = MuzzleState.Suppressor;
			}
		}

		protected override void Awake()
		{
			base.Awake();
			GameObject gameObject = new GameObject("m_proxyRound");
			m_proxy = gameObject.AddComponent<FVRFirearmMovingProxyRound>();
			m_proxy.Init(base.transform);
		}

		public override void UpdateInteraction(FVRViveHand hand)
		{
			TouchPadAxes = hand.Input.TouchpadAxes;
			if (m_hasTriggeredUpSinceBegin && !IsAltHeld)
			{
				m_triggerFloat = hand.Input.TriggerFloat;
			}
			else
			{
				m_triggerFloat = 0f;
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
				if (hand.Input.TouchpadDown && TouchPadAxes.magnitude > 0.25f && Vector2.Angle(TouchPadAxes, Vector2.left) <= 45f && HasFireSelectorButton)
				{
					ToggleFireSelector();
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
			FiringSystem();
			base.UpdateInteraction(hand);
		}

		public override void EndInteraction(FVRViveHand hand)
		{
			m_triggerFloat = 0f;
			m_hasTriggerCycled = false;
			m_isMagReleasePressed = false;
			base.EndInteraction(hand);
		}

		protected override void FVRFixedUpdate()
		{
			base.FVRFixedUpdate();
			UpdateComponentDisplay();
			if (m_timeTilCanDetectPiece > 0f)
			{
				m_timeTilCanDetectPiece -= Time.deltaTime;
			}
			if (!HasCap && HasBaffle && Vector3.Angle(Muzzle.forward, Vector3.down) < 45f)
			{
				RemoveThing();
				GameObject gameObject = UnityEngine.Object.Instantiate(Prefab_Baffle, PPoint_Baffle.position, PPoint_Baffle.rotation);
				BAPBaffle component = gameObject.GetComponent<BAPBaffle>();
				component.SetState(BaffleState);
				component.RootRigidbody.velocity = Muzzle.forward * 2.5f;
				SetBaffleState(hasB: false, 0);
			}
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
			if (Handle.CurHandleSlideState == BAPHandle.HandleSlideState.Forward)
			{
				IsBreachOpenForGasOut = false;
			}
			else
			{
				IsBreachOpenForGasOut = true;
			}
		}

		public void CockHammer()
		{
			if (!m_isHammerCocked)
			{
				m_isHammerCocked = true;
			}
		}

		public void DropHammer()
		{
			if (m_isHammerCocked)
			{
				m_isHammerCocked = false;
				PlayAudioEvent(FirearmAudioEventType.HammerHit);
				Fire();
			}
		}

		private void ToggleFireSelector()
		{
			if (FireSelector_Modes.Length > 1)
			{
				m_fireSelectorMode++;
				if (m_fireSelectorMode >= FireSelector_Modes.Length)
				{
					m_fireSelectorMode -= FireSelector_Modes.Length;
				}
				PlayAudioEvent(FirearmAudioEventType.FireSelector);
				if (FireSelector_Display != null)
				{
					SetAnimatedComponent(FireSelector_Display, FireSelector_Modes[m_fireSelectorMode].SelectorPosition, FireSelector_InterpStyle, FireSelector_Axis);
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

		protected virtual void FiringSystem()
		{
			if (FireSelector_Modes[m_fireSelectorMode].ModeType != 0 && !IsAltHeld && Handle.CurHandleRotState != BAPHandle.HandleRotState.Open && Handle.CurHandleSlideState == BAPHandle.HandleSlideState.Forward && m_hasTriggerCycled)
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
			if (!HasCap && HasBaffle)
			{
				RemoveThing();
				GameObject gameObject = UnityEngine.Object.Instantiate(Prefab_Baffle, PPoint_Baffle.position, PPoint_Baffle.rotation);
				BAPBaffle component = gameObject.GetComponent<BAPBaffle>();
				component.SetState(BaffleState);
				component.RootRigidbody.velocity = Muzzle.forward * 2.5f;
				SetBaffleState(hasB: false, 0);
			}
			PlayWellrodGunShot(Chamber.GetRound(), GM.CurrentPlayerBody.GetCurrentSoundEnvironment());
			BaffleState++;
			if (BaffleState > 10)
			{
				BaffleState = 10;
			}
			SetBaffleState(HasBaffle, BaffleState);
			return true;
		}

		public void PlayWellrodGunShot(FVRFireArmRound round, FVRSoundEnvironment TailEnvironment, float globalLoudnessMultiplier = 1f)
		{
			FVRTailSoundClass tailClass = FVRTailSoundClass.Tiny;
			Vector3 position = base.transform.position;
			float num = (float)BaffleState * 0.1f;
			float num2 = 1f;
			if (HasBaffle)
			{
				m_pool_shot.PlayClipVolumePitchOverride(AudioClipSet.Shots_Suppressed, GetMuzzle().position, new Vector2(AudioClipSet.Shots_Suppressed.VolumeRange.x * num, AudioClipSet.Shots_Suppressed.VolumeRange.y * num), new Vector2(AudioClipSet.Shots_Suppressed.PitchRange.x * num2, AudioClipSet.Shots_Suppressed.PitchRange.y * num2));
				if (base.IsHeld)
				{
					m_hand.ForceTubeKick(AudioClipSet.FTP.Kick_Shot);
					m_hand.ForceTubeRumble(AudioClipSet.FTP.Rumble_Shot_Intensity, AudioClipSet.FTP.Rumble_Shot_Duration);
				}
				if (AudioClipSet.UsesTail_Suppressed)
				{
					tailClass = round.TailClassSuppressed;
					AudioEvent tailSet = SM.GetTailSet(round.TailClassSuppressed, TailEnvironment);
					m_pool_tail.PlayClipVolumePitchOverride(tailSet, position, tailSet.VolumeRange * globalLoudnessMultiplier * num, AudioClipSet.TailPitchMod_Suppressed * tailSet.PitchRange.x);
				}
			}
			else
			{
				PlayAudioEvent(FirearmAudioEventType.Shots_Main);
				if (AudioClipSet.UsesTail_Main)
				{
					tailClass = round.TailClass;
					AudioEvent tailSet2 = SM.GetTailSet(round.TailClass, TailEnvironment);
					m_pool_tail.PlayClipVolumePitchOverride(tailSet2, position, tailSet2.VolumeRange * globalLoudnessMultiplier, AudioClipSet.TailPitchMod_Main * tailSet2.PitchRange.x);
				}
			}
			float soundTravelDistanceMultByEnvironment = SM.GetSoundTravelDistanceMultByEnvironment(TailEnvironment);
			int playerIFF = GM.CurrentPlayerBody.GetPlayerIFF();
			if (HasBaffle)
			{
				GM.CurrentSceneSettings.OnPerceiveableSound(AudioClipSet.Loudness_Suppressed * num, AudioClipSet.Loudness_Suppressed * soundTravelDistanceMultByEnvironment * 0.5f * globalLoudnessMultiplier * num, base.transform.position, playerIFF);
			}
			else
			{
				GM.CurrentSceneSettings.OnPerceiveableSound(AudioClipSet.Loudness_Primary, AudioClipSet.Loudness_Primary * soundTravelDistanceMultByEnvironment * globalLoudnessMultiplier, base.transform.position, playerIFF);
			}
			if (!HasBaffle)
			{
				SceneSettings.PingReceivers(MuzzlePos.position);
			}
			for (int i = 0; i < MuzzleDevices.Count; i++)
			{
				MuzzleDevices[i].OnShot(this, tailClass);
			}
		}

		public void UpdateBolt(float BoltLerp)
		{
			if (Handle.CurHandleSlideState != 0 && !m_proxy.IsFull && !Chamber.IsFull)
			{
				Chamber.IsAccessible = true;
			}
			else
			{
				Chamber.IsAccessible = false;
			}
			if (Handle.CurHandleSlideState == BAPHandle.HandleSlideState.Forward && Handle.LastHandleSlideState != 0)
			{
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
			else if (Handle.CurHandleSlideState == BAPHandle.HandleSlideState.Rear && Handle.LastHandleSlideState != BAPHandle.HandleSlideState.Rear)
			{
				CockHammer();
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
			else if (Handle.CurHandleSlideState == BAPHandle.HandleSlideState.Mid && Handle.LastHandleSlideState == BAPHandle.HandleSlideState.Rear && Magazine != null && !m_proxy.IsFull && Magazine.HasARound() && !Chamber.IsFull)
			{
				GameObject fromPrefabReference = Magazine.RemoveRound(b: false);
				m_proxy.SetFromPrefabReference(fromPrefabReference);
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
