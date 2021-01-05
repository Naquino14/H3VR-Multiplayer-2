using System;
using UnityEngine;

namespace FistVR
{
	public class RevolvingShotgun : FVRFireArm
	{
		public enum FireSelectorModeType
		{
			Safe,
			Single,
			FullAuto
		}

		[Serializable]
		public class FireSelectorMode
		{
			public float SelectorPosition;

			public FireSelectorModeType ModeType;
		}

		[Header("Revolving Shotgun Config")]
		private bool m_isHammerLocked;

		private bool m_hasTriggerCycled;

		public bool DoesFiringRecock;

		private Vector2 TouchPadAxes = Vector2.zero;

		public Speedloader.SpeedLoaderType SLType;

		[Header("Cylinder Config")]
		public bool CylinderLoaded;

		public Transform ProxyCylinder;

		public bool IsCylinderRotClockwise = true;

		public int NumChambers;

		public FVRFireArmChamber[] Chambers;

		private int m_curChamber;

		private float m_tarChamberLerp;

		private float m_curChamberLerp;

		[Header("Trigger Config")]
		public Transform Trigger;

		public bool HasTrigger;

		public float TriggerFiringThreshold = 0.8f;

		public float TriggerResetThreshold = 0.4f;

		public float Trigger_ForwardValue;

		public float Trigger_RearwardValue;

		public Axis TriggerAxis;

		public InterpStyle TriggerInterpStyle = InterpStyle.Rotation;

		private float m_triggerCurrentRot;

		[Header("Fire Selector Config")]
		public Transform FireSelectorSwitch;

		public InterpStyle FireSelector_InterpStyle = InterpStyle.Rotation;

		public Axis FireSelector_Axis;

		public FireSelectorMode[] FireSelector_Modes;

		private int m_fireSelectorMode;

		public GameObject CylinderPrefab;

		public Transform CyclinderMountPoint;

		private float lastTriggerRot;

		private bool m_shouldRecock;

		public int CurChamber
		{
			get
			{
				return m_curChamber;
			}
			set
			{
				if (value < 0)
				{
					m_curChamber = NumChambers - 1;
				}
				else
				{
					m_curChamber = value % NumChambers;
				}
			}
		}

		public int FireSelectorModeIndex => m_fireSelectorMode;

		protected override void Awake()
		{
			base.Awake();
			ProxyCylinder.gameObject.SetActive(CylinderLoaded);
		}

		protected override void FVRUpdate()
		{
			base.FVRUpdate();
		}

		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);
			TouchPadAxes = hand.Input.TouchpadAxes;
			UpdateTriggerHammer();
			UpdateCylinderRelease();
			if (IsAltHeld)
			{
				return;
			}
			if (hand.IsInStreamlinedMode)
			{
				if (hand.Input.BYButtonDown)
				{
					ToggleFireSelector();
				}
			}
			else if (hand.Input.TouchpadDown && TouchPadAxes.magnitude > 0.2f && Vector2.Angle(TouchPadAxes, Vector2.left) <= 45f)
			{
				ToggleFireSelector();
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
				FireSelectorMode fireSelectorMode = FireSelector_Modes[m_fireSelectorMode];
				PlayAudioEvent(FirearmAudioEventType.FireSelector);
				if (FireSelectorSwitch != null)
				{
					SetAnimatedComponent(FireSelectorSwitch, fireSelectorMode.SelectorPosition, FireSelector_InterpStyle, FireSelector_Axis);
				}
			}
		}

		private void UpdateTriggerHammer()
		{
			float num = 0f;
			bool flag = true;
			FireSelectorMode fireSelectorMode = FireSelector_Modes[m_fireSelectorMode];
			if (fireSelectorMode.ModeType != 0)
			{
				flag = false;
			}
			if (m_hasTriggeredUpSinceBegin && !IsAltHeld && !flag)
			{
				num = m_hand.Input.TriggerFloat;
			}
			if (m_isHammerLocked)
			{
				num += 0.8f;
			}
			m_triggerCurrentRot = Mathf.Lerp(Trigger_ForwardValue, Trigger_RearwardValue, num);
			if (Mathf.Abs(m_triggerCurrentRot - lastTriggerRot) > 0.0001f)
			{
				SetAnimatedComponent(Trigger, m_triggerCurrentRot, TriggerInterpStyle, TriggerAxis);
			}
			lastTriggerRot = m_triggerCurrentRot;
			if (m_shouldRecock)
			{
				m_shouldRecock = false;
				m_isHammerLocked = true;
				PlayAudioEvent(FirearmAudioEventType.Prefire);
			}
			if (!m_hasTriggerCycled)
			{
				if (!(num >= 0.98f))
				{
					return;
				}
				m_hasTriggerCycled = true;
				m_isHammerLocked = false;
				if (!IsCylinderRotClockwise)
				{
					CurChamber--;
				}
				else
				{
					CurChamber++;
				}
				m_curChamberLerp = 0f;
				m_tarChamberLerp = 0f;
				PlayAudioEvent(FirearmAudioEventType.HammerHit);
				if (CylinderLoaded && Chambers[CurChamber].IsFull && !Chambers[CurChamber].IsSpent)
				{
					Chambers[CurChamber].Fire();
					Fire();
					if (GM.CurrentSceneSettings.IsAmmoInfinite || GM.CurrentPlayerBody.IsInfiniteAmmo)
					{
						Chambers[CurChamber].IsSpent = false;
						Chambers[CurChamber].UpdateProxyDisplay();
					}
					if (DoesFiringRecock)
					{
						m_shouldRecock = true;
					}
				}
			}
			else if (m_hasTriggerCycled && m_hand.Input.TriggerFloat <= 0.1f)
			{
				m_hasTriggerCycled = false;
				PlayAudioEvent(FirearmAudioEventType.TriggerReset);
			}
		}

		private void UpdateCylinderRelease()
		{
			float num = 0f;
			bool flag = true;
			FireSelectorMode fireSelectorMode = FireSelector_Modes[m_fireSelectorMode];
			if (fireSelectorMode.ModeType != 0)
			{
				flag = false;
			}
			if (m_hasTriggeredUpSinceBegin && !IsAltHeld && !flag)
			{
				num = m_hand.Input.TriggerFloat;
			}
			if (m_isHammerLocked)
			{
				m_tarChamberLerp = 1f;
			}
			else if (!m_hasTriggerCycled)
			{
				m_tarChamberLerp = num * 1.4f;
			}
			m_curChamberLerp = Mathf.Lerp(m_curChamberLerp, m_tarChamberLerp, Time.deltaTime * 16f);
			int num2 = 0;
			num2 = ((!IsCylinderRotClockwise) ? ((CurChamber - 1) % NumChambers) : ((CurChamber + 1) % NumChambers));
			ProxyCylinder.transform.localRotation = Quaternion.Slerp(GetLocalRotationFromCylinder(CurChamber), GetLocalRotationFromCylinder(num2), m_curChamberLerp);
		}

		private void Fire()
		{
			FVRFireArmChamber fVRFireArmChamber = Chambers[CurChamber];
			base.Fire(fVRFireArmChamber, GetMuzzle(), doBuzz: true);
			FireMuzzleSmoke();
			if (fVRFireArmChamber.GetRound().IsHighPressure)
			{
				bool twoHandStabilized = IsTwoHandStabilized();
				bool foregripStabilized = base.AltGrip != null;
				bool shoulderStabilized = IsShoulderStabilized();
				Recoil(twoHandStabilized, foregripStabilized, shoulderStabilized);
			}
			PlayAudioGunShot(fVRFireArmChamber.GetRound(), GM.CurrentPlayerBody.GetCurrentSoundEnvironment());
		}

		public bool LoadCylinder(Speedloader s)
		{
			if (CylinderLoaded)
			{
				return false;
			}
			CylinderLoaded = true;
			ProxyCylinder.gameObject.SetActive(CylinderLoaded);
			PlayAudioEvent(FirearmAudioEventType.MagazineIn);
			m_curChamber = 0;
			ProxyCylinder.localRotation = GetLocalRotationFromCylinder(m_curChamber);
			for (int i = 0; i < Chambers.Length; i++)
			{
				if (s.Chambers[i].IsLoaded)
				{
					Chambers[i].Autochamber(s.Chambers[i].LoadedClass);
					if (s.Chambers[i].IsSpent)
					{
						Chambers[i].IsSpent = true;
					}
					else
					{
						Chambers[i].IsSpent = false;
					}
				}
				else
				{
					Chambers[i].Unload();
				}
				Chambers[i].UpdateProxyDisplay();
			}
			return true;
		}

		public Speedloader EjectCylinder()
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(CylinderPrefab, CyclinderMountPoint.position, CyclinderMountPoint.rotation);
			Speedloader component = gameObject.GetComponent<Speedloader>();
			PlayAudioEvent(FirearmAudioEventType.MagazineOut);
			for (int i = 0; i < component.Chambers.Count; i++)
			{
				if (!Chambers[i].IsFull)
				{
					component.Chambers[i].Unload();
				}
				else if (Chambers[i].IsSpent)
				{
					component.Chambers[i].LoadEmpty(Chambers[i].GetRound().RoundClass);
				}
				else
				{
					component.Chambers[i].Load(Chambers[i].GetRound().RoundClass);
				}
				Chambers[i].UpdateProxyDisplay();
			}
			base.EjectDelay = 0.4f;
			CylinderLoaded = false;
			ProxyCylinder.gameObject.SetActive(CylinderLoaded);
			return component;
		}

		public Quaternion GetLocalRotationFromCylinder(int cylinder)
		{
			float t = (float)cylinder * (360f / (float)NumChambers) * -1f;
			t = Mathf.Repeat(t, 360f);
			return Quaternion.Euler(new Vector3(0f, 0f, t));
		}

		public int GetClosestChamberIndex()
		{
			float num = 0f - base.transform.localEulerAngles.z;
			num += 360f / (float)NumChambers * 0.5f;
			num = Mathf.Repeat(num, 360f);
			return Mathf.CeilToInt(num / (360f / (float)NumChambers)) - 1;
		}
	}
}
