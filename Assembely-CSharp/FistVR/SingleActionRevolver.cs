using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class SingleActionRevolver : FVRFireArm
	{
		[Header("Single Action Revolver")]
		public bool AllowsSuppressor;

		public Transform Hammer;

		public Transform LoadingGate;

		public Transform Trigger;

		public Transform EjectorRod;

		public SingleActionRevolverCylinder Cylinder;

		public Transform HammerFanDir;

		private int m_curChamber;

		private float m_curChamberLerp;

		private float m_tarChamberLerp;

		[Header("Component Movement Params")]
		public float Hammer_Rot_Uncocked;

		public float Hammer_Rot_Halfcocked;

		public float Hammer_Rot_Cocked;

		public float LoadingGate_Rot_Closed;

		public float LoadingGate_Rot_Open;

		public float Trigger_Rot_Forward;

		public float Trigger_Rot_Rearward;

		public Vector3 EjectorRod_Pos_Forward;

		public Vector3 EjectorRod_Pos_Rearward;

		public bool DoesCylinderTranslateForward;

		public bool DoesHalfCockHalfRotCylinder;

		public bool HasTransferBarSafety;

		public Vector3 CylinderBackPos;

		public Vector3 CylinderFrontPos;

		[Header("Spinning Config")]
		public Transform PoseSpinHolder;

		public bool CanSpin = true;

		private bool m_isSpinning;

		[Header("StateToggling")]
		public bool StateToggles = true;

		private bool m_isStateToggled;

		public Transform Pose_Main;

		public Transform Pose_Toggled;

		public float TriggerThreshold = 0.9f;

		private float m_triggerFloat;

		private bool m_isHammerCocking;

		private bool m_isHammerCocked;

		private float m_hammerCockLerp;

		private float m_hammerCockSpeed = 10f;

		private float xSpinRot;

		private float xSpinVel;

		private float timeSinceColFire;

		public int CurChamber
		{
			get
			{
				return m_curChamber;
			}
			set
			{
				m_curChamber = value % Cylinder.NumChambers;
			}
		}

		public int NextChamber => (m_curChamber + 1) % Cylinder.NumChambers;

		public int PrevChamber
		{
			get
			{
				int num = m_curChamber - 1;
				if (num < 0)
				{
					return Cylinder.NumChambers - 1;
				}
				return num;
			}
		}

		public int PrevChamber2
		{
			get
			{
				int num = m_curChamber - 2;
				if (num < 0)
				{
					return Cylinder.NumChambers + num;
				}
				return num;
			}
		}

		protected override void Awake()
		{
			base.Awake();
			if (PoseOverride_Touch != null)
			{
				Pose_Main.localPosition = PoseOverride_Touch.localPosition;
				Pose_Main.localRotation = PoseOverride_Touch.localRotation;
			}
		}

		public override void BeginInteraction(FVRViveHand hand)
		{
			base.BeginInteraction(hand);
			if (IsAltHeld)
			{
				return;
			}
			if (!m_isStateToggled)
			{
				PoseOverride.localPosition = Pose_Main.localPosition;
				PoseOverride.localRotation = Pose_Main.localRotation;
				if (m_grabPointTransform != null)
				{
					m_grabPointTransform.localPosition = Pose_Main.localPosition;
					m_grabPointTransform.localRotation = Pose_Main.localRotation;
				}
			}
			else
			{
				PoseOverride.localPosition = Pose_Toggled.localPosition;
				PoseOverride.localRotation = Pose_Toggled.localRotation;
				if (m_grabPointTransform != null)
				{
					m_grabPointTransform.localPosition = Pose_Toggled.localPosition;
					m_grabPointTransform.localRotation = Pose_Toggled.localRotation;
				}
			}
		}

		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);
			m_isSpinning = false;
			if (!IsAltHeld)
			{
				if (!m_isStateToggled)
				{
					if (hand.Input.TouchpadPressed && !hand.IsInStreamlinedMode && Vector2.Angle(hand.Input.TouchpadAxes, Vector2.up) < 45f)
					{
						m_isSpinning = true;
					}
					if (hand.IsInStreamlinedMode)
					{
						if (hand.Input.AXButtonDown)
						{
							CockHammer(5f);
						}
						if (hand.Input.BYButtonDown && StateToggles)
						{
							ToggleState();
							PlayAudioEvent(FirearmAudioEventType.BreachOpen);
						}
					}
					else if (hand.Input.TouchpadDown)
					{
						if (Vector2.Angle(hand.Input.TouchpadAxes, Vector2.down) < 45f)
						{
							CockHammer(5f);
						}
						else if (Vector2.Angle(hand.Input.TouchpadAxes, Vector2.left) < 45f && StateToggles)
						{
							ToggleState();
							PlayAudioEvent(FirearmAudioEventType.BreachOpen);
						}
					}
				}
				else
				{
					if (hand.IsInStreamlinedMode)
					{
						if (hand.Input.AXButtonDown)
						{
							AdvanceCylinder();
						}
						if (hand.Input.BYButtonDown && StateToggles)
						{
							ToggleState();
							PlayAudioEvent(FirearmAudioEventType.BreachOpen);
						}
					}
					else if (hand.Input.TouchpadDown)
					{
						if (Vector2.Angle(hand.Input.TouchpadAxes, Vector2.left) < 45f && StateToggles)
						{
							ToggleState();
							PlayAudioEvent(FirearmAudioEventType.BreachClose);
						}
						else if (Vector2.Angle(hand.Input.TouchpadAxes, Vector2.right) < 45f)
						{
							AdvanceCylinder();
						}
					}
					if (hand.Input.TriggerDown)
					{
						EjectPrevCylinder();
					}
				}
			}
			UpdateTriggerHammer();
			UpdateCylinderRot();
			if (!base.IsHeld)
			{
				m_isSpinning = false;
			}
		}

		public override void EndInteraction(FVRViveHand hand)
		{
			m_triggerFloat = 0f;
			base.EndInteraction(hand);
			base.RootRigidbody.AddRelativeTorque(new Vector3(xSpinVel, 0f, 0f), ForceMode.Impulse);
		}

		protected override void FVRFixedUpdate()
		{
			UpdateSpinning();
			if (timeSinceColFire < 3f)
			{
				timeSinceColFire += Time.deltaTime;
			}
			base.FVRFixedUpdate();
		}

		private void UpdateTriggerHammer()
		{
			if (base.IsHeld && !m_isStateToggled && !m_isHammerCocked && !m_isHammerCocking && m_hand.OtherHand != null)
			{
				Vector3 velLinearWorld = m_hand.OtherHand.Input.VelLinearWorld;
				float num = Vector3.Distance(m_hand.OtherHand.PalmTransform.position, HammerFanDir.position);
				if (num < 0.15f && Vector3.Angle(velLinearWorld, HammerFanDir.forward) < 60f && velLinearWorld.magnitude > 1f)
				{
					CockHammer(10f);
				}
			}
			if (m_isHammerCocking)
			{
				if (m_hammerCockLerp < 1f)
				{
					m_hammerCockLerp += Time.deltaTime * m_hammerCockSpeed;
				}
				else
				{
					m_hammerCockLerp = 1f;
					m_isHammerCocking = false;
					m_isHammerCocked = true;
					CurChamber++;
					m_curChamberLerp = 0f;
					m_tarChamberLerp = 0f;
				}
			}
			if (!m_isStateToggled)
			{
				Hammer.localEulerAngles = new Vector3(Mathf.Lerp(Hammer_Rot_Uncocked, Hammer_Rot_Cocked, m_hammerCockLerp), 0f, 0f);
			}
			else
			{
				Hammer.localEulerAngles = new Vector3(Hammer_Rot_Halfcocked, 0f, 0f);
			}
			if (LoadingGate != null)
			{
				if (!m_isStateToggled)
				{
					LoadingGate.localEulerAngles = new Vector3(0f, 0f, LoadingGate_Rot_Closed);
				}
				else
				{
					LoadingGate.localEulerAngles = new Vector3(0f, 0f, LoadingGate_Rot_Open);
				}
			}
			m_triggerFloat = 0f;
			if (m_hasTriggeredUpSinceBegin && !m_isSpinning && !m_isStateToggled)
			{
				m_triggerFloat = m_hand.Input.TriggerFloat;
			}
			Trigger.localEulerAngles = new Vector3(Mathf.Lerp(Trigger_Rot_Forward, Trigger_Rot_Rearward, m_triggerFloat), 0f, 0f);
			if (m_triggerFloat > TriggerThreshold)
			{
				DropHammer();
			}
		}

		private void DropHammer()
		{
			if (m_isHammerCocked)
			{
				m_isHammerCocked = false;
				m_isHammerCocking = false;
				m_hammerCockLerp = 0f;
				Fire();
			}
		}

		private void AdvanceCylinder()
		{
			CurChamber++;
			PlayAudioEvent(FirearmAudioEventType.FireSelector);
		}

		public void EjectPrevCylinder()
		{
			if (m_isStateToggled)
			{
				FVRFireArmChamber fVRFireArmChamber = Cylinder.Chambers[PrevChamber];
				if (fVRFireArmChamber.IsFull)
				{
					PlayAudioEvent(FirearmAudioEventType.MagazineEjectRound);
				}
				fVRFireArmChamber.EjectRound(fVRFireArmChamber.transform.position + fVRFireArmChamber.transform.forward * 0.0025f, -fVRFireArmChamber.transform.forward, Vector3.zero);
			}
		}

		private void Fire()
		{
			PlayAudioEvent(FirearmAudioEventType.HammerHit);
			if (Cylinder.Chambers[CurChamber].Fire())
			{
				FVRFireArmChamber fVRFireArmChamber = Cylinder.Chambers[CurChamber];
				base.Fire(fVRFireArmChamber, GetMuzzle(), doBuzz: true);
				FireMuzzleSmoke();
				bool twoHandStabilized = IsTwoHandStabilized();
				bool foregripStabilized = base.AltGrip != null;
				bool shoulderStabilized = IsShoulderStabilized();
				Recoil(twoHandStabilized, foregripStabilized, shoulderStabilized);
				PlayAudioGunShot(fVRFireArmChamber.GetRound(), GM.CurrentPlayerBody.GetCurrentSoundEnvironment());
				if (GM.CurrentSceneSettings.IsAmmoInfinite || GM.CurrentPlayerBody.IsInfiniteAmmo)
				{
					fVRFireArmChamber.IsSpent = false;
					fVRFireArmChamber.UpdateProxyDisplay();
				}
			}
		}

		private void UpdateCylinderRot()
		{
			if (m_isStateToggled)
			{
				for (int i = 0; i < Cylinder.Chambers.Length; i++)
				{
					if (i == PrevChamber)
					{
						Cylinder.Chambers[i].IsAccessible = true;
					}
					else
					{
						Cylinder.Chambers[i].IsAccessible = false;
					}
				}
				if (DoesHalfCockHalfRotCylinder)
				{
					int cylinder = (CurChamber + 1) % Cylinder.NumChambers;
					Cylinder.transform.localRotation = Quaternion.Slerp(Cylinder.GetLocalRotationFromCylinder(CurChamber), Cylinder.GetLocalRotationFromCylinder(cylinder), 0.5f);
				}
				else
				{
					Cylinder.transform.localRotation = Cylinder.GetLocalRotationFromCylinder(CurChamber);
				}
				if (DoesCylinderTranslateForward)
				{
					Cylinder.transform.localPosition = CylinderBackPos;
				}
			}
			else
			{
				for (int j = 0; j < Cylinder.Chambers.Length; j++)
				{
					Cylinder.Chambers[j].IsAccessible = false;
				}
				if (m_isHammerCocking)
				{
					m_tarChamberLerp = m_hammerCockLerp;
				}
				else
				{
					m_tarChamberLerp = 0f;
				}
				m_curChamberLerp = Mathf.Lerp(m_curChamberLerp, m_tarChamberLerp, Time.deltaTime * 16f);
				int cylinder2 = (CurChamber + 1) % Cylinder.NumChambers;
				Cylinder.transform.localRotation = Quaternion.Slerp(Cylinder.GetLocalRotationFromCylinder(CurChamber), Cylinder.GetLocalRotationFromCylinder(cylinder2), m_curChamberLerp);
				if (DoesCylinderTranslateForward)
				{
					Cylinder.transform.localPosition = Vector3.Lerp(CylinderBackPos, CylinderFrontPos, m_hammerCockLerp);
				}
			}
		}

		private void UpdateSpinning()
		{
			if (!base.IsHeld)
			{
				m_isSpinning = false;
			}
			if (m_isSpinning)
			{
				Vector3 vector = Vector3.zero;
				if (m_hand != null)
				{
					vector = m_hand.Input.VelLinearLocal;
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
				PoseSpinHolder.localRotation = Quaternion.RotateTowards(PoseSpinHolder.localRotation, Quaternion.identity, Time.deltaTime * 500f);
				PoseSpinHolder.localEulerAngles = new Vector3(PoseSpinHolder.localEulerAngles.x, 0f, 0f);
			}
		}

		private void CockHammer(float speed)
		{
			if (!m_isHammerCocked && !m_isHammerCocking)
			{
				m_hammerCockSpeed = speed;
				m_isHammerCocking = true;
				PlayAudioEvent(FirearmAudioEventType.Prefire);
			}
		}

		private void ToggleState()
		{
			m_isStateToggled = !m_isStateToggled;
			if (!IsAltHeld)
			{
				if (!m_isStateToggled)
				{
					PoseOverride.localPosition = Pose_Main.localPosition;
					PoseOverride.localRotation = Pose_Main.localRotation;
					if (m_grabPointTransform != null)
					{
						m_grabPointTransform.localPosition = Pose_Main.localPosition;
						m_grabPointTransform.localRotation = Pose_Main.localRotation;
					}
				}
				else
				{
					PoseOverride.localPosition = Pose_Toggled.localPosition;
					PoseOverride.localRotation = Pose_Toggled.localRotation;
					if (m_grabPointTransform != null)
					{
						m_grabPointTransform.localPosition = Pose_Toggled.localPosition;
						m_grabPointTransform.localRotation = Pose_Toggled.localRotation;
					}
				}
			}
			m_isHammerCocking = false;
			m_isHammerCocked = false;
			m_hammerCockLerp = 0f;
			if (!m_isStateToggled)
			{
			}
		}

		public override void OnCollisionEnter(Collision col)
		{
			base.OnCollisionEnter(col);
			if (!HasTransferBarSafety && col.collider.attachedRigidbody == null && Cylinder.Chambers[CurChamber].IsFull && !Cylinder.Chambers[CurChamber].IsSpent && !m_isHammerCocked && !m_isHammerCocking && !m_isStateToggled && col.relativeVelocity.magnitude > 2f && timeSinceColFire > 2.9f)
			{
				timeSinceColFire = 0f;
				Fire();
			}
		}

		public override List<FireArmRoundClass> GetChamberRoundList()
		{
			bool flag = false;
			List<FireArmRoundClass> list = new List<FireArmRoundClass>();
			for (int i = 0; i < Cylinder.Chambers.Length; i++)
			{
				if (Cylinder.Chambers[i].IsFull)
				{
					list.Add(Cylinder.Chambers[i].GetRound().RoundClass);
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
			for (int i = 0; i < Cylinder.Chambers.Length; i++)
			{
				if (i < rounds.Count)
				{
					Cylinder.Chambers[i].Autochamber(rounds[i]);
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
