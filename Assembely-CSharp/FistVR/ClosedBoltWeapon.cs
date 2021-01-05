using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class ClosedBoltWeapon : FVRFireArm
	{
		public enum FireSelectorModeType
		{
			Safe,
			Single,
			Burst,
			FullAuto,
			SuperFastBurst
		}

		[Serializable]
		public class FireSelectorMode
		{
			public float SelectorPosition;

			public FireSelectorModeType ModeType;

			public int BurstAmount = 3;
		}

		[Header("ClosedBoltWeapon Config")]
		public bool HasFireSelectorButton = true;

		public bool HasMagReleaseButton = true;

		public bool HasBoltReleaseButton = true;

		public bool HasBoltCatchButton = true;

		public bool HasHandle = true;

		[Header("Component Connections")]
		public ClosedBolt Bolt;

		public ClosedBoltHandle Handle;

		public FVRFireArmChamber Chamber;

		public Transform Trigger;

		public Transform FireSelectorSwitch;

		public Transform FireSelectorSwitch2;

		[Header("Round Positions")]
		public Transform RoundPos_Ejecting;

		public Transform RoundPos_Ejection;

		public Transform RoundPos_MagazinePos;

		private FVRFirearmMovingProxyRound m_proxy;

		public Vector3 EjectionSpeed = new Vector3(4f, 2.5f, -1.2f);

		public Vector3 EjectionSpin = new Vector3(20f, 180f, 30f);

		public bool UsesDelinker;

		public ParticleSystem DelinkerSystem;

		[Header("Trigger Config")]
		public bool HasTrigger;

		public float TriggerFiringThreshold = 0.8f;

		public float TriggerResetThreshold = 0.4f;

		public float TriggerDualStageThreshold = 0.95f;

		public float Trigger_ForwardValue;

		public float Trigger_RearwardValue;

		public Axis TriggerAxis;

		public InterpStyle TriggerInterpStyle = InterpStyle.Rotation;

		public bool UsesDualStageFullAuto;

		private float m_triggerFloat;

		private bool m_hasTriggerReset;

		private int m_CamBurst;

		private bool m_isHammerCocked;

		private int m_fireSelectorMode;

		[Header("Fire Selector Config")]
		public InterpStyle FireSelector_InterpStyle = InterpStyle.Rotation;

		public Axis FireSelector_Axis;

		public FireSelectorMode[] FireSelector_Modes;

		[Header("Secondary Fire Selector Config")]
		public InterpStyle FireSelector_InterpStyle2 = InterpStyle.Rotation;

		public Axis FireSelector_Axis2;

		public FireSelectorMode[] FireSelector_Modes2;

		[Header("SpecialFeatures")]
		public bool EjectsMagazineOnEmpty;

		public bool BoltLocksWhenNoMagazineFound;

		public bool DoesClipEntryRequireBoltBack = true;

		public bool UsesMagMountTransformOverride;

		public Transform MagMountTransformOverride;

		public bool ReciprocatesOnShot = true;

		[Header("StickySystem")]
		public bool UsesStickyDetonation;

		public AudioEvent StickyDetonateSound;

		public Transform StickyTrigger;

		public Vector2 StickyRotRange = new Vector2(0f, 20f);

		private float m_stickyChargeUp;

		public float StickyChargeUpSpeed = 1f;

		public AudioSource m_chargeSound;

		public Renderer StickyScreen;

		public float StickyMaxMultBonus = 1.3f;

		public float StickyForceMult = 1f;

		[HideInInspector]
		public bool IsMagReleaseButtonHeld;

		[HideInInspector]
		public bool IsBoltReleaseButtonHeld;

		[HideInInspector]
		public bool IsBoltCatchButtonHeld;

		private float m_timeSinceFiredShot = 1f;

		private bool m_hasStickTriggerDown;

		private List<MF2_StickyBomb> m_primedBombs = new List<MF2_StickyBomb>();

		public bool IsHammerCocked => m_isHammerCocked;

		public int FireSelectorModeIndex => m_fireSelectorMode;

		public bool HasExtractedRound()
		{
			return m_proxy.IsFull;
		}

		protected override void Awake()
		{
			base.Awake();
			m_CamBurst = 1;
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
			if (!Chamber.IsFull & (m_timeSinceFiredShot > 0.4f))
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

		public void CockHammer()
		{
			if (!m_isHammerCocked)
			{
				m_isHammerCocked = true;
				PlayAudioEvent(FirearmAudioEventType.Prefire);
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

		public bool IsWeaponOnSafe()
		{
			if (FireSelector_Modes.Length == 0)
			{
				return false;
			}
			if (FireSelector_Modes[m_fireSelectorMode].ModeType == FireSelectorModeType.Safe)
			{
				return true;
			}
			return false;
		}

		public void ResetCamBurst()
		{
			FireSelectorMode fireSelectorMode = FireSelector_Modes[m_fireSelectorMode];
			m_CamBurst = fireSelectorMode.BurstAmount;
		}

		protected virtual void ToggleFireSelector()
		{
			if (FireSelector_Modes.Length <= 1)
			{
				return;
			}
			if (Bolt.UsesAKSafetyLock && !Bolt.IsBoltForwardOfSafetyLock())
			{
				int num = m_fireSelectorMode + 1;
				if (num >= FireSelector_Modes.Length)
				{
					num -= FireSelector_Modes.Length;
				}
				if (FireSelector_Modes[num].ModeType == FireSelectorModeType.Safe)
				{
					return;
				}
			}
			m_fireSelectorMode++;
			if (m_fireSelectorMode >= FireSelector_Modes.Length)
			{
				m_fireSelectorMode -= FireSelector_Modes.Length;
			}
			FireSelectorMode fireSelectorMode = FireSelector_Modes[m_fireSelectorMode];
			if (m_triggerFloat < 0.1f)
			{
				m_CamBurst = fireSelectorMode.BurstAmount;
			}
			PlayAudioEvent(FirearmAudioEventType.FireSelector);
			if (FireSelectorSwitch != null)
			{
				SetAnimatedComponent(FireSelectorSwitch, fireSelectorMode.SelectorPosition, FireSelector_InterpStyle, FireSelector_Axis);
			}
			if (FireSelectorSwitch2 != null)
			{
				FireSelectorMode fireSelectorMode2 = FireSelector_Modes2[m_fireSelectorMode];
				SetAnimatedComponent(FireSelectorSwitch2, fireSelectorMode2.SelectorPosition, FireSelector_InterpStyle2, FireSelector_Axis2);
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
			bool flag = false;
			GameObject fromPrefabReference = null;
			if (HasBelt)
			{
				if (!m_proxy.IsFull && BeltDD.HasARound())
				{
					flag = true;
					fromPrefabReference = BeltDD.RemoveRound(b: false);
				}
			}
			else if (!m_proxy.IsFull && Magazine != null && !Magazine.IsBeltBox && Magazine.HasARound())
			{
				flag = true;
				fromPrefabReference = Magazine.RemoveRound(b: false);
			}
			if (flag && flag)
			{
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

		public override Transform GetMagMountingTransform()
		{
			if (UsesMagMountTransformOverride)
			{
				return MagMountTransformOverride;
			}
			return base.GetMagMountingTransform();
		}

		protected override void FVRFixedUpdate()
		{
			base.FVRFixedUpdate();
			if (UsesStickyDetonation && m_stickyChargeUp > 0f)
			{
				base.RootRigidbody.velocity += UnityEngine.Random.onUnitSphere * 0.2f * m_stickyChargeUp * StickyForceMult;
				base.RootRigidbody.angularVelocity += UnityEngine.Random.onUnitSphere * 1f * m_stickyChargeUp * StickyForceMult;
			}
		}

		public bool Fire()
		{
			if (!Chamber.Fire())
			{
				return false;
			}
			m_timeSinceFiredShot = 0f;
			float velMult = 1f;
			if (UsesStickyDetonation)
			{
				velMult = 1f + Mathf.Lerp(0f, StickyMaxMultBonus, m_stickyChargeUp);
			}
			base.Fire(Chamber, GetMuzzle(), doBuzz: true, velMult);
			bool twoHandStabilized = IsTwoHandStabilized();
			bool foregripStabilized = base.AltGrip != null;
			bool shoulderStabilized = IsShoulderStabilized();
			Recoil(twoHandStabilized, foregripStabilized, shoulderStabilized);
			bool flag = false;
			FireSelectorMode fireSelectorMode = FireSelector_Modes[m_fireSelectorMode];
			if (fireSelectorMode.ModeType == FireSelectorModeType.SuperFastBurst)
			{
				for (int i = 0; i < fireSelectorMode.BurstAmount - 1; i++)
				{
					if (Magazine.HasARound())
					{
						Magazine.RemoveRound();
						base.Fire(Chamber, GetMuzzle(), doBuzz: false);
						flag = true;
						Recoil(twoHandStabilized, foregripStabilized, shoulderStabilized);
					}
				}
			}
			FireMuzzleSmoke();
			if (UsesDelinker && HasBelt)
			{
				DelinkerSystem.Emit(1);
			}
			if (HasBelt)
			{
				BeltDD.AddJitter();
			}
			if (flag)
			{
				PlayAudioGunShot(IsHighPressure: false, Chamber.GetRound().TailClass, Chamber.GetRound().TailClassSuppressed, GM.CurrentPlayerBody.GetCurrentSoundEnvironment());
			}
			else
			{
				PlayAudioGunShot(Chamber.GetRound(), GM.CurrentPlayerBody.GetCurrentSoundEnvironment());
			}
			if (ReciprocatesOnShot)
			{
				Bolt.ImpartFiringImpulse();
			}
			return true;
		}

		protected override void FVRUpdate()
		{
			base.FVRUpdate();
			UpdateComponents();
			if (HasHandle)
			{
				Handle.UpdateHandle();
				Bolt.UpdateHandleHeldState(Handle.ShouldControlBolt(), 1f - Handle.GetBoltLerpBetweenLockAndFore());
			}
			Bolt.UpdateBolt();
			if (UsesClips && DoesClipEntryRequireBoltBack)
			{
				if (Bolt.CurPos >= ClosedBolt.BoltPos.Locked)
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
			UpdateDisplayRoundPositions();
			if (m_timeSinceFiredShot < 1f)
			{
				m_timeSinceFiredShot += Time.deltaTime;
			}
		}

		public override void LoadMag(FVRFireArmMagazine mag)
		{
			base.LoadMag(mag);
			if (BoltLocksWhenNoMagazineFound && mag != null && Bolt.IsBoltLocked())
			{
				Bolt.ReleaseBolt();
			}
		}

		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);
			UpdateInputAndAnimate(hand);
		}

		private void UpdateInputAndAnimate(FVRViveHand hand)
		{
			IsBoltReleaseButtonHeld = false;
			IsBoltCatchButtonHeld = false;
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
				if (FireSelector_Modes.Length > 0)
				{
					m_CamBurst = FireSelector_Modes[m_fireSelectorMode].BurstAmount;
				}
				PlayAudioEvent(FirearmAudioEventType.TriggerReset);
			}
			if (hand.IsInStreamlinedMode)
			{
				if (hand.Input.BYButtonDown)
				{
					ToggleFireSelector();
					if (UsesStickyDetonation)
					{
						Detonate();
					}
				}
				if (hand.Input.AXButtonDown && HasMagReleaseButton && (!EjectsMagazineOnEmpty || (Bolt.CurPos >= ClosedBolt.BoltPos.Locked && Bolt.IsHeld && !m_proxy.IsFull)))
				{
					ReleaseMag();
				}
				if (UsesStickyDetonation)
				{
					if (hand.Input.BYButtonDown)
					{
						SetAnimatedComponent(StickyTrigger, StickyRotRange.y, InterpStyle.Rotation, Axis.X);
					}
					else if (hand.Input.BYButtonUp)
					{
						SetAnimatedComponent(StickyTrigger, StickyRotRange.x, InterpStyle.Rotation, Axis.X);
					}
				}
			}
			else
			{
				Vector2 touchpadAxes = hand.Input.TouchpadAxes;
				if (hand.Input.TouchpadDown)
				{
					if (UsesStickyDetonation)
					{
						Detonate();
					}
					if (touchpadAxes.magnitude > 0.2f)
					{
						if (Vector2.Angle(touchpadAxes, Vector2.left) <= 45f)
						{
							ToggleFireSelector();
						}
						else if (Vector2.Angle(touchpadAxes, Vector2.up) <= 45f)
						{
							if (HasBoltReleaseButton)
							{
								Bolt.ReleaseBolt();
							}
						}
						else if (Vector2.Angle(touchpadAxes, Vector2.down) <= 45f && HasMagReleaseButton && (!EjectsMagazineOnEmpty || (Bolt.CurPos >= ClosedBolt.BoltPos.Locked && Bolt.IsHeld && !m_proxy.IsFull)))
						{
							ReleaseMag();
						}
					}
				}
				if (UsesStickyDetonation)
				{
					if (hand.Input.TouchpadDown)
					{
						SetAnimatedComponent(StickyTrigger, StickyRotRange.y, InterpStyle.Rotation, Axis.X);
					}
					else if (hand.Input.TouchpadUp)
					{
						SetAnimatedComponent(StickyTrigger, StickyRotRange.x, InterpStyle.Rotation, Axis.X);
					}
				}
				if (hand.Input.TouchpadPressed && touchpadAxes.magnitude > 0.2f)
				{
					if (Vector2.Angle(touchpadAxes, Vector2.up) <= 45f)
					{
						if (HasBoltReleaseButton)
						{
							IsBoltReleaseButtonHeld = true;
						}
					}
					else if (Vector2.Angle(touchpadAxes, Vector2.right) <= 45f && HasBoltCatchButton)
					{
						IsBoltCatchButtonHeld = true;
					}
				}
			}
			FireSelectorModeType modeType = FireSelector_Modes[m_fireSelectorMode].ModeType;
			if (modeType == FireSelectorModeType.Safe || !m_hasTriggeredUpSinceBegin)
			{
				return;
			}
			if (UsesStickyDetonation)
			{
				if (Bolt.CurPos != 0 || !Chamber.IsFull || Chamber.IsSpent)
				{
					return;
				}
				if (hand.Input.TriggerPressed && m_hasTriggerReset)
				{
					m_hasStickTriggerDown = true;
					m_stickyChargeUp += Time.deltaTime * 0.25f * StickyChargeUpSpeed;
					m_stickyChargeUp = Mathf.Clamp(m_stickyChargeUp, 0f, 1f);
					if (m_stickyChargeUp > 0.05f && !m_chargeSound.isPlaying)
					{
						m_chargeSound.Play();
					}
				}
				else
				{
					if (m_chargeSound.isPlaying)
					{
						m_chargeSound.Stop();
					}
					m_stickyChargeUp = 0f;
				}
				if (m_hasStickTriggerDown && (hand.Input.TriggerUp || m_stickyChargeUp >= 1f))
				{
					m_hasStickTriggerDown = false;
					DropHammer();
					EndStickyCharge();
				}
			}
			else if (m_triggerFloat >= TriggerFiringThreshold && Bolt.CurPos == ClosedBolt.BoltPos.Forward && (m_hasTriggerReset || (modeType == FireSelectorModeType.FullAuto && !UsesDualStageFullAuto) || (modeType == FireSelectorModeType.FullAuto && UsesDualStageFullAuto && m_triggerFloat > TriggerDualStageThreshold) || (modeType == FireSelectorModeType.Burst && m_CamBurst > 0)))
			{
				DropHammer();
				m_hasTriggerReset = false;
				if (m_CamBurst > 0)
				{
					m_CamBurst--;
				}
			}
		}

		public override void EndInteraction(FVRViveHand hand)
		{
			if (UsesStickyDetonation)
			{
				EndStickyCharge();
			}
			base.EndInteraction(hand);
		}

		private void EndStickyCharge()
		{
			m_chargeSound.Stop();
			m_stickyChargeUp = 0f;
		}

		private void UpdateComponents()
		{
			if (HasTrigger)
			{
				SetAnimatedComponent(Trigger, Mathf.Lerp(Trigger_ForwardValue, Trigger_RearwardValue, m_triggerFloat), TriggerInterpStyle, TriggerAxis);
			}
			if (UsesStickyDetonation)
			{
				float t = Mathf.Clamp((float)m_primedBombs.Count / 8f, 0f, 1f);
				float y = Mathf.Lerp(0.56f, 0.23f, t);
				float num = Mathf.Lerp(5f, 15f, t);
				StickyScreen.material.SetTextureOffset("_IncandescenceMap", new Vector2(0f, y));
			}
		}

		private void UpdateDisplayRoundPositions()
		{
			float boltLerpBetweenLockAndFore = Bolt.GetBoltLerpBetweenLockAndFore();
			if (Chamber.IsFull)
			{
				Chamber.ProxyRound.position = Vector3.Lerp(RoundPos_Ejecting.position, Chamber.transform.position, boltLerpBetweenLockAndFore);
				Chamber.ProxyRound.rotation = Quaternion.Slerp(RoundPos_Ejecting.rotation, Chamber.transform.rotation, boltLerpBetweenLockAndFore);
			}
			if (m_proxy.IsFull)
			{
				m_proxy.ProxyRound.position = Vector3.Lerp(RoundPos_MagazinePos.position, Chamber.transform.position, boltLerpBetweenLockAndFore);
				m_proxy.ProxyRound.rotation = Quaternion.Slerp(RoundPos_MagazinePos.rotation, Chamber.transform.rotation, boltLerpBetweenLockAndFore);
			}
		}

		public void ReleaseMag()
		{
			if (Magazine != null)
			{
				base.EjectMag();
			}
		}

		public void RegisterStickyBomb(MF2_StickyBomb sb)
		{
			if (sb != null)
			{
				m_primedBombs.Add(sb);
			}
		}

		public void Detonate()
		{
			bool flag = false;
			if (m_primedBombs.Count > 0)
			{
				for (int num = m_primedBombs.Count - 1; num >= 0; num--)
				{
					if (m_primedBombs[num] != null && m_primedBombs[num].m_hasStuck && !m_primedBombs[num].m_hasExploded)
					{
						flag = true;
						m_primedBombs[num].Detonate();
						m_primedBombs.RemoveAt(num);
					}
				}
			}
			if (flag)
			{
				SM.PlayCoreSound(FVRPooledAudioType.GenericClose, StickyDetonateSound, base.transform.position);
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
				if (FireSelectorSwitch != null)
				{
					FireSelectorMode fireSelectorMode = FireSelector_Modes[m_fireSelectorMode];
					SetAnimatedComponent(FireSelectorSwitch, fireSelectorMode.SelectorPosition, FireSelector_InterpStyle, FireSelector_Axis);
				}
				if (FireSelectorSwitch2 != null)
				{
					FireSelectorMode fireSelectorMode2 = FireSelector_Modes2[m_fireSelectorMode];
					SetAnimatedComponent(FireSelectorSwitch2, fireSelectorMode2.SelectorPosition, FireSelector_InterpStyle2, FireSelector_Axis2);
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
