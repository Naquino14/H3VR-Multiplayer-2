using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class Revolver : FVRFireArm
	{
		[Serializable]
		public class TriggerPiece
		{
			public Transform TPiece;

			public Axis TAxis;

			public Vector2 TRange;

			public InterpStyle TInterp;
		}

		[Header("Revolver Config")]
		public bool AllowsSuppressor;

		public bool isChiappa;

		public bool isChiappaHammer;

		public Transform Hammer;

		public bool CanManuallyCockHammer = true;

		public bool IsDoubleActionTrigger = true;

		private float m_hammerForwardRot;

		public float m_hammerBackwardRot = -49f;

		private float m_hammerCurrentRot;

		public Transform Trigger;

		private float m_triggerForwardRot;

		public float m_triggerBackwardRot = 30f;

		private float m_triggerCurrentRot;

		private bool m_isHammerLocked;

		private bool m_hasTriggerCycled;

		public bool DoesFiringRecock;

		public Transform CylinderReleaseButton;

		public bool isCyclinderReleaseARot;

		public Vector3 CylinderReleaseButtonForwardPos;

		public Vector3 CylinderReleaseButtonRearPos;

		private bool m_isCylinderReleasePressed;

		private float m_curCyclinderReleaseRot;

		[Header("Cylinder Config")]
		public bool UsesCylinderArm = true;

		public Transform CylinderArm;

		private bool m_isCylinderArmLocked = true;

		private bool m_wasCylinderArmLocked = true;

		private float CylinderArmRot;

		public bool IsCylinderRotClockwise = true;

		public Vector2 CylinderRotRange = new Vector2(0f, 105f);

		public bool IsCylinderArmZ;

		public bool AngInvert;

		public bool GravityRotsCylinderPositive = true;

		public RevolverCylinder Cylinder;

		private int m_curChamber;

		private float m_tarChamberLerp;

		private float m_curChamberLerp;

		[Header("Chambers Config")]
		public FVRFireArmChamber[] Chambers;

		[Header("Spinning Config")]
		public Transform PoseSpinHolder;

		public bool CanSpin = true;

		private bool m_isSpinning;

		public Transform Muzzle;

		public bool UsesAltPoseSwitch = true;

		public Transform Pose_Main;

		public Transform Pose_Reloading;

		private bool m_isInMainpose = true;

		private Vector2 TouchPadAxes = Vector2.zero;

		private bool m_hasEjectedSinceOpening;

		public List<TriggerPiece> TPieces;

		private float xSpinRot;

		private float xSpinVel;

		private float lastTriggerRot;

		private bool m_shouldRecock;

		private float m_CylCloseVel;

		public bool isCylinderArmLocked => m_isCylinderArmLocked;

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
					m_curChamber = Cylinder.numChambers - 1;
				}
				else
				{
					m_curChamber = value % Cylinder.numChambers;
				}
			}
		}

		protected override void Awake()
		{
			base.Awake();
		}

		public override int GetTutorialState()
		{
			int num = 0;
			int num2 = 0;
			for (int i = 0; i < Chambers.Length; i++)
			{
				if (Chambers[i].IsFull)
				{
					num++;
					if (!Chambers[i].IsSpent)
					{
						num2++;
					}
				}
			}
			if (num <= 0)
			{
				if (m_isCylinderArmLocked)
				{
					return 0;
				}
				return 1;
			}
			if (num2 > 0)
			{
				if (m_isCylinderArmLocked)
				{
					return 3;
				}
				return 2;
			}
			if (m_isCylinderArmLocked)
			{
				return 0;
			}
			return 4;
		}

		protected override void FVRUpdate()
		{
			base.FVRUpdate();
		}

		public override void BeginInteraction(FVRViveHand hand)
		{
			base.BeginInteraction(hand);
			if (!IsAltHeld)
			{
				if (m_isInMainpose)
				{
					PoseOverride.localPosition = Pose_Main.localPosition;
					PoseOverride.localRotation = Pose_Main.localRotation;
					m_grabPointTransform.localPosition = Pose_Main.localPosition;
					m_grabPointTransform.localRotation = Pose_Main.localRotation;
				}
				else
				{
					PoseOverride.localPosition = Pose_Reloading.localPosition;
					PoseOverride.localRotation = Pose_Reloading.localRotation;
					m_grabPointTransform.localPosition = Pose_Reloading.localPosition;
					m_grabPointTransform.localRotation = Pose_Reloading.localRotation;
				}
			}
			base.RootRigidbody.maxAngularVelocity = 40f;
		}

		public override void EndInteraction(FVRViveHand hand)
		{
			base.EndInteraction(hand);
			base.RootRigidbody.AddRelativeTorque(new Vector3(xSpinVel, 0f, 0f), ForceMode.Impulse);
		}

		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);
			if (IsAltHeld && !m_isInMainpose)
			{
				m_isInMainpose = true;
				PoseOverride.localPosition = Pose_Main.localPosition;
				PoseOverride.localRotation = Pose_Main.localRotation;
				m_grabPointTransform.localPosition = Pose_Main.localPosition;
				m_grabPointTransform.localRotation = Pose_Main.localRotation;
			}
			TouchPadAxes = hand.Input.TouchpadAxes;
			m_isSpinning = false;
			if (!IsAltHeld && !hand.IsInStreamlinedMode)
			{
				if (hand.Input.TouchpadPressed && Vector2.Angle(TouchPadAxes, Vector2.up) < 45f)
				{
					m_isSpinning = true;
				}
				if (hand.Input.TouchpadDown && Vector2.Angle(TouchPadAxes, Vector2.right) < 45f && UsesAltPoseSwitch)
				{
					m_isInMainpose = !m_isInMainpose;
					if (m_isInMainpose)
					{
						PoseOverride.localPosition = Pose_Main.localPosition;
						PoseOverride.localRotation = Pose_Main.localRotation;
						m_grabPointTransform.localPosition = Pose_Main.localPosition;
						m_grabPointTransform.localRotation = Pose_Main.localRotation;
					}
					else
					{
						PoseOverride.localPosition = Pose_Reloading.localPosition;
						PoseOverride.localRotation = Pose_Reloading.localRotation;
						m_grabPointTransform.localPosition = Pose_Reloading.localPosition;
						m_grabPointTransform.localRotation = Pose_Reloading.localRotation;
					}
				}
			}
			UpdateTriggerHammer();
			UpdateCylinderRelease();
			if (!base.IsHeld || IsAltHeld || base.AltGrip != null)
			{
				m_isSpinning = false;
			}
		}

		protected override void FVRFixedUpdate()
		{
			UpdateSpinning();
			base.FVRFixedUpdate();
		}

		public void EjectChambers()
		{
			bool flag = false;
			for (int i = 0; i < Chambers.Length; i++)
			{
				if (Chambers[i].IsFull)
				{
					flag = true;
					if (AngInvert)
					{
						Chambers[i].EjectRound(Chambers[i].transform.position + Chambers[i].transform.forward * Cylinder.CartridgeLength, Chambers[i].transform.forward, UnityEngine.Random.onUnitSphere, ForceCaseLessEject: true);
					}
					else
					{
						Chambers[i].EjectRound(Chambers[i].transform.position + -Chambers[i].transform.forward * Cylinder.CartridgeLength, -Chambers[i].transform.forward, UnityEngine.Random.onUnitSphere, ForceCaseLessEject: true);
					}
				}
			}
			if (flag)
			{
				PlayAudioEvent(FirearmAudioEventType.MagazineOut);
			}
		}

		private void UpdateSpinning()
		{
			if (!base.IsHeld || IsAltHeld || base.AltGrip != null)
			{
				m_isSpinning = false;
			}
			if (m_isSpinning)
			{
				Vector3 vector = Vector3.zero;
				if (m_hand != null)
				{
					vector = m_hand.Input.VelLinearWorld;
				}
				float value = Vector3.Dot(vector.normalized, base.transform.up);
				value = Mathf.Clamp(value, 0f - vector.magnitude, vector.magnitude);
				if (Mathf.Abs(xSpinVel) < 90f)
				{
					xSpinVel += value * Time.deltaTime * 600f;
				}
				else if (Mathf.Sign(value) == Mathf.Sign(xSpinVel))
				{
					xSpinVel += value * Time.deltaTime * 600f;
				}
				if (Mathf.Abs(xSpinVel) < 90f)
				{
					if (Vector3.Dot(base.transform.up, Vector3.down) >= 0f && Mathf.Sign(xSpinVel) == 1f)
					{
						xSpinVel += Time.deltaTime * 50f;
					}
					if (Vector3.Dot(base.transform.up, Vector3.down) < 0f && Mathf.Sign(xSpinVel) == -1f)
					{
						xSpinVel -= Time.deltaTime * 50f;
					}
				}
				xSpinVel = Mathf.Clamp(xSpinVel, -500f, 500f);
				xSpinRot += xSpinVel * Time.deltaTime * 5f;
				PoseSpinHolder.localEulerAngles = new Vector3(xSpinRot, 0f, 0f);
				xSpinVel = Mathf.Lerp(xSpinVel, 0f, Time.deltaTime * 0.6f);
			}
			else
			{
				xSpinRot = 0f;
				xSpinVel = 0f;
			}
		}

		private void UpdateTriggerHammer()
		{
			float num = 0f;
			if (m_hasTriggeredUpSinceBegin && !m_isSpinning && !IsAltHeld && isCylinderArmLocked)
			{
				num = m_hand.Input.TriggerFloat;
			}
			if (m_isHammerLocked)
			{
				num += 0.8f;
				m_triggerCurrentRot = Mathf.Lerp(m_triggerForwardRot, m_triggerBackwardRot, num);
			}
			else
			{
				m_triggerCurrentRot = Mathf.Lerp(m_triggerForwardRot, m_triggerBackwardRot, num);
			}
			if (Mathf.Abs(m_triggerCurrentRot - lastTriggerRot) > 0.01f)
			{
				if (Trigger != null)
				{
					Trigger.localEulerAngles = new Vector3(m_triggerCurrentRot, 0f, 0f);
				}
				for (int i = 0; i < TPieces.Count; i++)
				{
					SetAnimatedComponent(TPieces[i].TPiece, Mathf.Lerp(TPieces[i].TRange.x, TPieces[i].TRange.y, num), TPieces[i].TInterp, TPieces[i].TAxis);
				}
			}
			lastTriggerRot = m_triggerCurrentRot;
			if (m_shouldRecock)
			{
				m_shouldRecock = false;
				m_isHammerLocked = true;
				PlayAudioEvent(FirearmAudioEventType.Prefire);
			}
			if (!m_hasTriggerCycled || !IsDoubleActionTrigger)
			{
				if (num >= 0.98f && (m_isHammerLocked || IsDoubleActionTrigger) && !m_hand.Input.TouchpadPressed)
				{
					if (m_isCylinderArmLocked)
					{
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
						if (Chambers[CurChamber].IsFull && !Chambers[CurChamber].IsSpent)
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
					else
					{
						m_hasTriggerCycled = true;
						m_isHammerLocked = false;
					}
				}
				else if ((num <= 0.08f || !IsDoubleActionTrigger) && !m_isHammerLocked && CanManuallyCockHammer && !IsAltHeld)
				{
					if (m_hand.IsInStreamlinedMode)
					{
						if (m_hand.Input.AXButtonDown)
						{
							m_isHammerLocked = true;
							PlayAudioEvent(FirearmAudioEventType.Prefire);
						}
					}
					else if (m_hand.Input.TouchpadDown && Vector2.Angle(TouchPadAxes, Vector2.down) < 45f)
					{
						m_isHammerLocked = true;
						PlayAudioEvent(FirearmAudioEventType.Prefire);
					}
				}
			}
			else if (m_hasTriggerCycled && m_hand.Input.TriggerFloat <= 0.08f)
			{
				m_hasTriggerCycled = false;
				PlayAudioEvent(FirearmAudioEventType.TriggerReset);
			}
			if (!isChiappaHammer)
			{
				if (m_hasTriggerCycled || !IsDoubleActionTrigger)
				{
					if (m_isHammerLocked)
					{
						m_hammerCurrentRot = Mathf.Lerp(m_hammerCurrentRot, m_hammerBackwardRot, Time.deltaTime * 10f);
					}
					else
					{
						m_hammerCurrentRot = Mathf.Lerp(m_hammerCurrentRot, m_hammerForwardRot, Time.deltaTime * 30f);
					}
				}
				else if (m_isHammerLocked)
				{
					m_hammerCurrentRot = Mathf.Lerp(m_hammerCurrentRot, m_hammerBackwardRot, Time.deltaTime * 10f);
				}
				else
				{
					m_hammerCurrentRot = Mathf.Lerp(m_hammerForwardRot, m_hammerBackwardRot, num);
				}
			}
			if (isChiappaHammer)
			{
				bool flag = false;
				if (m_hand.IsInStreamlinedMode && m_hand.Input.AXButtonPressed)
				{
					flag = true;
				}
				else if (Vector2.Angle(m_hand.Input.TouchpadAxes, Vector2.down) < 45f && m_hand.Input.TouchpadPressed)
				{
					flag = true;
				}
				if (num <= 0.02f && !IsAltHeld && flag)
				{
					m_hammerCurrentRot = Mathf.Lerp(m_hammerCurrentRot, m_hammerBackwardRot, Time.deltaTime * 15f);
				}
				else
				{
					m_hammerCurrentRot = Mathf.Lerp(m_hammerCurrentRot, m_hammerForwardRot, Time.deltaTime * 6f);
				}
			}
			if (Hammer != null)
			{
				Hammer.localEulerAngles = new Vector3(m_hammerCurrentRot, 0f, 0f);
			}
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
			if (fVRFireArmChamber.GetRound().IsCaseless)
			{
				fVRFireArmChamber.SetRound(null);
			}
		}

		public void AddCylinderCloseVel(float f)
		{
			m_CylCloseVel = f;
		}

		private void UpdateCylinderRelease()
		{
			float num = 0f;
			if (m_hasTriggeredUpSinceBegin && !m_isSpinning && !IsAltHeld && isCylinderArmLocked)
			{
				num = m_hand.Input.TriggerFloat;
			}
			m_isCylinderReleasePressed = false;
			if (!IsAltHeld && (!m_isHammerLocked || DoesFiringRecock))
			{
				if (m_hand.IsInStreamlinedMode)
				{
					if (m_hand.Input.BYButtonPressed)
					{
						m_isCylinderReleasePressed = true;
					}
				}
				else if (m_hand.Input.TouchpadPressed && Vector2.Angle(TouchPadAxes, Vector2.left) < 45f)
				{
					m_isCylinderReleasePressed = true;
				}
			}
			if (CylinderReleaseButton != null)
			{
				if (isCyclinderReleaseARot)
				{
					if (!m_isCylinderReleasePressed)
					{
						m_curCyclinderReleaseRot = Mathf.Lerp(m_curCyclinderReleaseRot, CylinderReleaseButtonForwardPos.x, Time.deltaTime * 3f);
					}
					else
					{
						m_curCyclinderReleaseRot = Mathf.Lerp(m_curCyclinderReleaseRot, CylinderReleaseButtonRearPos.x, Time.deltaTime * 3f);
					}
					CylinderReleaseButton.localEulerAngles = new Vector3(m_curCyclinderReleaseRot, 0f, 0f);
				}
				else if (m_isCylinderReleasePressed)
				{
					CylinderReleaseButton.localPosition = Vector3.Lerp(CylinderReleaseButton.localPosition, CylinderReleaseButtonForwardPos, Time.deltaTime * 3f);
				}
				else
				{
					CylinderReleaseButton.localPosition = Vector3.Lerp(CylinderReleaseButton.localPosition, CylinderReleaseButtonRearPos, Time.deltaTime * 3f);
				}
			}
			if (m_isCylinderReleasePressed)
			{
				m_isCylinderArmLocked = false;
			}
			else
			{
				float f = CylinderArm.localEulerAngles.z;
				if (IsCylinderArmZ)
				{
					f = CylinderArm.localEulerAngles.x;
				}
				if (Mathf.Abs(f) <= 1f && !m_isCylinderArmLocked)
				{
					m_isCylinderArmLocked = true;
					CylinderArm.localEulerAngles = Vector3.zero;
				}
			}
			float num2 = 160f;
			if (!GravityRotsCylinderPositive)
			{
				num2 *= -1f;
			}
			float num3 = 0f;
			if (!m_isCylinderArmLocked)
			{
				float num4 = base.transform.InverseTransformDirection(m_hand.Input.VelAngularWorld).z;
				float num5 = base.transform.InverseTransformDirection(m_hand.Input.VelLinearWorld).x;
				if (IsCylinderArmZ)
				{
					num4 = base.transform.InverseTransformDirection(m_hand.Input.VelAngularWorld).x;
					num5 = base.transform.InverseTransformDirection(m_hand.Input.VelLinearWorld).y;
				}
				if (AngInvert)
				{
					num4 = 0f - num4;
					num5 = 0f - num5;
				}
				num2 += num4 * 70f;
				num2 += num5 * -350f;
				num2 += m_CylCloseVel;
				m_CylCloseVel = 0f;
				num3 = CylinderArmRot + num2 * Time.deltaTime;
				num3 = Mathf.Clamp(num3, CylinderRotRange.x, CylinderRotRange.y);
				if (num3 != CylinderArmRot)
				{
					CylinderArmRot = num3;
					if (IsCylinderArmZ)
					{
						CylinderArm.localEulerAngles = new Vector3(num3, 0f, 0f);
					}
					else
					{
						CylinderArm.localEulerAngles = new Vector3(0f, 0f, num3);
					}
				}
			}
			float f2 = CylinderArm.localEulerAngles.z;
			if (IsCylinderArmZ)
			{
				f2 = CylinderArm.localEulerAngles.x;
			}
			if (Mathf.Abs(f2) > 30f)
			{
				for (int i = 0; i < Chambers.Length; i++)
				{
					Chambers[i].IsAccessible = true;
				}
			}
			else
			{
				for (int j = 0; j < Chambers.Length; j++)
				{
					Chambers[j].IsAccessible = false;
				}
			}
			if (Mathf.Abs(f2) < 1f && IsCylinderArmZ)
			{
				m_hasEjectedSinceOpening = false;
			}
			if (Mathf.Abs(f2) > 45f && IsCylinderArmZ && !m_hasEjectedSinceOpening)
			{
				m_hasEjectedSinceOpening = true;
				EjectChambers();
			}
			if (!IsCylinderArmZ && Mathf.Abs(CylinderArm.localEulerAngles.z) > 75f && Vector3.Angle(base.transform.forward, Vector3.up) <= 120f)
			{
				float num6 = base.transform.InverseTransformDirection(m_hand.Input.VelLinearWorld).z;
				if (AngInvert)
				{
					num6 = 0f - num6;
				}
				if (num6 < -2f)
				{
					EjectChambers();
				}
			}
			if (m_isCylinderArmLocked && !m_wasCylinderArmLocked)
			{
				m_curChamber = Cylinder.GetClosestChamberIndex();
				Cylinder.transform.localRotation = Cylinder.GetLocalRotationFromCylinder(m_curChamber);
				m_curChamberLerp = 0f;
				m_tarChamberLerp = 0f;
				PlayAudioEvent(FirearmAudioEventType.BreachClose);
			}
			if (!m_isCylinderArmLocked && m_wasCylinderArmLocked)
			{
				PlayAudioEvent(FirearmAudioEventType.BreachOpen);
			}
			if (m_isHammerLocked)
			{
				m_tarChamberLerp = 1f;
			}
			else if (!m_hasTriggerCycled && IsDoubleActionTrigger)
			{
				m_tarChamberLerp = num * 1.4f;
			}
			m_curChamberLerp = Mathf.Lerp(m_curChamberLerp, m_tarChamberLerp, Time.deltaTime * 16f);
			int num7 = 0;
			num7 = ((!IsCylinderRotClockwise) ? ((CurChamber - 1) % Cylinder.numChambers) : ((CurChamber + 1) % Cylinder.numChambers));
			if (isCylinderArmLocked)
			{
				Cylinder.transform.localRotation = Quaternion.Slerp(Cylinder.GetLocalRotationFromCylinder(CurChamber), Cylinder.GetLocalRotationFromCylinder(num7), m_curChamberLerp);
			}
			m_wasCylinderArmLocked = m_isCylinderArmLocked;
		}

		public override List<FireArmRoundClass> GetChamberRoundList()
		{
			bool flag = false;
			List<FireArmRoundClass> list = new List<FireArmRoundClass>();
			for (int i = 0; i < Chambers.Length; i++)
			{
				if (Chambers[i].IsFull)
				{
					list.Add(Chambers[i].GetRound().RoundClass);
					flag = true;
				}
			}
			if (flag)
			{
				return list;
			}
			return null;
		}

		public override void SetLoadedChambers(List<FireArmRoundClass> rounds)
		{
			if (rounds.Count <= 0)
			{
				return;
			}
			for (int i = 0; i < Chambers.Length; i++)
			{
				if (i < rounds.Count)
				{
					Chambers[i].Autochamber(rounds[i]);
				}
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
