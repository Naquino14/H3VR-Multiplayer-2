using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class LeverActionFirearm : FVRFireArm
	{
		public enum ZPos
		{
			Forward,
			Middle,
			Rear
		}

		[Serializable]
		public class LeverActuatedPiece
		{
			public Transform Piece;

			public Vector3 PosBack;

			public Vector3 PosForward;

			public InterpStyle InterpStyle;

			public ActuationType ActuationType;
		}

		public enum ActuationType
		{
			Lever,
			Hammer
		}

		[Header("LeverAction Config")]
		public FVRFireArmChamber Chamber;

		public bool UsesSecondChamber;

		public FVRFireArmChamber Chamber2;

		public Transform SecondEjectionSpot;

		public Transform SecondMuzzle;

		private bool m_isHammerCocked2;

		private bool m_isSecondaryMuzzlePos;

		public Transform Lever;

		public Transform LeverRoot;

		public Transform Hammer;

		public Transform LoadingGate;

		public Transform Trigger;

		public Vector2 TriggerRotRange;

		public FVRAlternateGrip ForeGrip;

		public Vector2 LeverAngleRange = new Vector2(-68f, 0f);

		public Vector2 HammerAngleRange = new Vector2(-36f, 0f);

		public Vector2 LoadingGateAngleRange = new Vector2(-24f, 0f);

		public Vector3 EjectionDir = new Vector3(0f, 2f, 0f);

		public Vector3 EjectionSpin = new Vector3(80f, 0f, 0f);

		private bool m_isLeverReleasePressed;

		private float m_curLeverRot;

		private float m_tarLeverRot;

		private float m_leverRotSpeed = 700f;

		private bool m_isActionMovingForward;

		private ZPos m_curLeverPos = ZPos.Rear;

		private ZPos m_lastLeverPos = ZPos.Rear;

		private FVRFirearmMovingProxyRound m_proxy;

		private FVRFirearmMovingProxyRound m_proxy2;

		[Header("Round Positions Config")]
		public Transform ReceiverLowerPathForward;

		public Transform ReceiverLowerPathRearward;

		public Transform ReceiverUpperPathForward;

		public Transform ReceiverUpperPathRearward;

		public Transform ReceiverEjectionPathForward;

		public Transform ReceiverEjectionPathRearward;

		public Transform ReceiverEjectionPoint;

		private bool m_isHammerCocked;

		[Header("Spinning Config")]
		public Transform PoseSpinHolder;

		public bool CanSpin;

		private bool m_isSpinning;

		public LeverActuatedPiece[] ActuatedPieces;

		private bool useLinearRacking = true;

		private float baseDistance = 1f;

		private float BaseAngleOffset;

		private bool m_wasLeverLocked;

		private Vector3 m_baseSpinPosition = Vector3.zero;

		private float curDistanceBetweenGrips = 1f;

		private float lastDistanceBetweenGrips = -1f;

		private float m_rackingDisplacement;

		private float xSpinRot;

		private float xSpinVel;

		public bool IsHammerCocked => m_isHammerCocked;

		protected override void Awake()
		{
			base.Awake();
			GameObject gameObject = new GameObject("m_proxyRound");
			m_proxy = gameObject.AddComponent<FVRFirearmMovingProxyRound>();
			m_proxy.Init(base.transform);
			if (UsesSecondChamber)
			{
				GameObject gameObject2 = new GameObject("m_proxyRound2");
				m_proxy2 = gameObject2.AddComponent<FVRFirearmMovingProxyRound>();
				m_proxy2.Init(base.transform);
			}
			m_baseSpinPosition = PoseSpinHolder.localPosition;
		}

		public override Transform GetMuzzle()
		{
			if (!UsesSecondChamber)
			{
				return base.GetMuzzle();
			}
			if (m_isSecondaryMuzzlePos)
			{
				return SecondMuzzle;
			}
			return MuzzlePos;
		}

		public override void BeginInteraction(FVRViveHand hand)
		{
			base.BeginInteraction(hand);
			SetBaseHandAngle(hand);
		}

		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);
			UpdateLever();
			Trigger.localEulerAngles = new Vector3(Mathf.Lerp(TriggerRotRange.x, TriggerRotRange.y, hand.Input.TriggerFloat), 0f, 0f);
			if (hand.Input.TriggerDown && !IsAltHeld && m_curLeverPos == ZPos.Rear && m_hasTriggeredUpSinceBegin && (m_isHammerCocked || m_isHammerCocked2))
			{
				Fire();
			}
			float t = Mathf.InverseLerp(LeverAngleRange.y, LeverAngleRange.x, m_curLeverRot);
			if (Hammer != null)
			{
				if (m_isHammerCocked)
				{
					Hammer.localEulerAngles = new Vector3(HammerAngleRange.x, 0f, 0f);
				}
				else
				{
					Hammer.localEulerAngles = new Vector3(Mathf.Lerp(HammerAngleRange.y, HammerAngleRange.x, t), 0f, 0f);
				}
			}
		}

		public override void EndInteraction(FVRViveHand hand)
		{
			Trigger.localEulerAngles = new Vector3(0f, 0f, 0f);
			base.EndInteraction(hand);
		}

		private void SetBaseHandAngle(FVRViveHand hand)
		{
			Vector3 normalized = Vector3.ProjectOnPlane(m_hand.PoseOverride.forward, LeverRoot.right).normalized;
			Vector3 forward = LeverRoot.forward;
			float num = (BaseAngleOffset = Mathf.Atan2(Vector3.Dot(LeverRoot.right, Vector3.Cross(forward, normalized)), Vector3.Dot(forward, normalized)) * 57.29578f);
		}

		private void UpdateLever()
		{
			bool flag = false;
			bool flag2 = false;
			if (base.IsHeld)
			{
				if (m_hand.IsInStreamlinedMode)
				{
					flag = m_hand.Input.BYButtonPressed;
					flag2 = m_hand.Input.BYButtonUp;
				}
				else
				{
					flag = m_hand.Input.TouchpadPressed;
					flag2 = m_hand.Input.TouchpadUp;
				}
			}
			m_isLeverReleasePressed = false;
			bool flag3 = false;
			if (!IsAltHeld && ForeGrip.m_hand != null)
			{
				if (ForeGrip.m_hand.Input.TriggerPressed && ForeGrip.m_hasTriggeredUpSinceBegin)
				{
					flag3 = true;
				}
				m_isLeverReleasePressed = true;
				curDistanceBetweenGrips = Vector3.Distance(m_hand.PalmTransform.position, base.AltGrip.m_hand.PalmTransform.position);
				if (lastDistanceBetweenGrips < 0f)
				{
					lastDistanceBetweenGrips = curDistanceBetweenGrips;
				}
			}
			else
			{
				lastDistanceBetweenGrips = -1f;
			}
			m_isSpinning = false;
			if (!IsAltHeld && CanSpin && flag)
			{
				m_isSpinning = true;
			}
			bool flag4 = false;
			if ((m_isHammerCocked || m_isHammerCocked2) && !m_isSpinning && m_curLeverPos == ZPos.Rear)
			{
				flag4 = true;
			}
			if (flag3)
			{
				flag4 = false;
			}
			if (base.AltGrip == null && !IsAltHeld)
			{
				flag4 = true;
			}
			if (flag4 && useLinearRacking)
			{
				SetBaseHandAngle(m_hand);
			}
			m_wasLeverLocked = flag4;
			if (flag2)
			{
				m_tarLeverRot = 0f;
				PoseSpinHolder.localPosition = m_baseSpinPosition;
				lastDistanceBetweenGrips = curDistanceBetweenGrips;
				m_rackingDisplacement = 0f;
			}
			else if (m_isLeverReleasePressed && !flag4)
			{
				if (useLinearRacking)
				{
					curDistanceBetweenGrips = Vector3.Distance(m_hand.PalmTransform.position, base.AltGrip.m_hand.PalmTransform.position);
					float num = 0f;
					if (curDistanceBetweenGrips < lastDistanceBetweenGrips)
					{
						num = lastDistanceBetweenGrips - curDistanceBetweenGrips;
						m_rackingDisplacement += num;
					}
					else
					{
						num = curDistanceBetweenGrips - lastDistanceBetweenGrips;
						m_rackingDisplacement -= num;
					}
					m_rackingDisplacement = Mathf.Clamp(m_rackingDisplacement, 0f, 0.04f);
					if (m_rackingDisplacement < 0.005f)
					{
						m_rackingDisplacement = 0f;
					}
					if (m_rackingDisplacement > 0.035f)
					{
						m_rackingDisplacement = 0.04f;
					}
					PoseSpinHolder.localPosition = m_baseSpinPosition + Vector3.forward * m_rackingDisplacement * 2f;
					m_tarLeverRot = Mathf.Lerp(LeverAngleRange.y, LeverAngleRange.x, m_rackingDisplacement * 25f);
					lastDistanceBetweenGrips = curDistanceBetweenGrips;
				}
				else
				{
					Vector3 normalized = Vector3.ProjectOnPlane(m_hand.PoseOverride.forward, LeverRoot.right).normalized;
					Vector3 forward = LeverRoot.forward;
					float num2 = Mathf.Atan2(Vector3.Dot(LeverRoot.right, Vector3.Cross(forward, normalized)), Vector3.Dot(forward, normalized)) * 57.29578f;
					num2 -= BaseAngleOffset;
					num2 *= 3f;
					num2 = (m_tarLeverRot = Mathf.Clamp(num2, LeverAngleRange.x, LeverAngleRange.y));
				}
			}
			else if (m_isSpinning)
			{
				float num3 = Mathf.Clamp(m_hand.Input.VelLinearWorld.magnitude - 1f, 0f, 3f);
				float value = num3 * 120f;
				float num4 = Mathf.Repeat(Mathf.Abs(xSpinRot), 360f);
				value = Mathf.Clamp(value, 0f, num4 * 0.5f);
				m_tarLeverRot = Mathf.Clamp(0f - value, LeverAngleRange.x, LeverAngleRange.y);
				PoseSpinHolder.localPosition = m_baseSpinPosition;
			}
			if (Mathf.Abs(m_curLeverRot - LeverAngleRange.y) < 1f)
			{
				if (m_lastLeverPos == ZPos.Forward)
				{
					m_curLeverPos = ZPos.Middle;
				}
				else
				{
					m_curLeverPos = ZPos.Rear;
					IsBreachOpenForGasOut = false;
				}
			}
			else if (Mathf.Abs(m_curLeverRot - LeverAngleRange.x) < 1f)
			{
				if (m_lastLeverPos == ZPos.Rear)
				{
					m_curLeverPos = ZPos.Middle;
				}
				else
				{
					m_curLeverPos = ZPos.Forward;
					IsBreachOpenForGasOut = true;
				}
			}
			else
			{
				m_curLeverPos = ZPos.Middle;
				IsBreachOpenForGasOut = true;
			}
			if (m_curLeverPos == ZPos.Rear && m_lastLeverPos != ZPos.Rear)
			{
				m_tarLeverRot = LeverAngleRange.y;
				m_curLeverRot = LeverAngleRange.y;
				if (m_isActionMovingForward && m_proxy.IsFull && !Chamber.IsFull)
				{
					m_hand.Buzz(m_hand.Buzzer.Buzz_OnHoverInteractive);
					Chamber.SetRound(m_proxy.Round);
					m_proxy.ClearProxy();
					PlayAudioEvent(FirearmAudioEventType.HandleBack);
				}
				else
				{
					PlayAudioEvent(FirearmAudioEventType.HandleBackEmpty);
				}
				if (UsesSecondChamber && m_isActionMovingForward && m_proxy2.IsFull && !Chamber2.IsFull)
				{
					Chamber2.SetRound(m_proxy2.Round);
					m_proxy2.ClearProxy();
				}
				m_isActionMovingForward = false;
			}
			else if (m_curLeverPos == ZPos.Forward && m_lastLeverPos != 0)
			{
				m_tarLeverRot = LeverAngleRange.x;
				m_curLeverRot = LeverAngleRange.x;
				m_isHammerCocked = true;
				if (UsesSecondChamber)
				{
					m_isHammerCocked2 = true;
				}
				PlayAudioEvent(FirearmAudioEventType.Prefire);
				if (!m_isActionMovingForward && Chamber.IsFull)
				{
					m_hand.Buzz(m_hand.Buzzer.Buzz_OnHoverInteractive);
					Chamber.EjectRound(ReceiverEjectionPoint.position, base.transform.right * EjectionDir.x + base.transform.up * EjectionDir.y + base.transform.forward * EjectionDir.z, base.transform.right * EjectionSpin.x + base.transform.up * EjectionSpin.y + base.transform.forward * EjectionSpin.z);
					PlayAudioEvent(FirearmAudioEventType.HandleForward);
				}
				else
				{
					PlayAudioEvent(FirearmAudioEventType.HandleForwardEmpty);
				}
				if (UsesSecondChamber && !m_isActionMovingForward && Chamber2.IsFull)
				{
					Chamber2.EjectRound(SecondEjectionSpot.position, base.transform.right * EjectionDir.x + base.transform.up * EjectionDir.y + base.transform.forward * EjectionDir.z, base.transform.right * EjectionSpin.x + base.transform.up * EjectionSpin.y + base.transform.forward * EjectionSpin.z);
				}
				m_isActionMovingForward = true;
			}
			else if (m_curLeverPos == ZPos.Middle && m_lastLeverPos == ZPos.Rear)
			{
				if (Magazine != null && !m_proxy.IsFull && Magazine.HasARound())
				{
					GameObject fromPrefabReference = Magazine.RemoveRound(b: false);
					m_proxy.SetFromPrefabReference(fromPrefabReference);
				}
				if (UsesSecondChamber && Magazine != null && !m_proxy2.IsFull && Magazine.HasARound())
				{
					GameObject fromPrefabReference2 = Magazine.RemoveRound(b: false);
					m_proxy2.SetFromPrefabReference(fromPrefabReference2);
				}
			}
			float t = Mathf.InverseLerp(LeverAngleRange.y, LeverAngleRange.x, m_curLeverRot);
			if (m_proxy.IsFull)
			{
				if (m_isActionMovingForward)
				{
					m_proxy.ProxyRound.position = Vector3.Lerp(ReceiverUpperPathForward.position, ReceiverUpperPathRearward.position, t);
					m_proxy.ProxyRound.rotation = Quaternion.Slerp(ReceiverUpperPathForward.rotation, ReceiverUpperPathRearward.rotation, t);
					if (LoadingGate != null)
					{
						LoadingGate.localEulerAngles = new Vector3(LoadingGateAngleRange.x, 0f, 0f);
					}
				}
				else
				{
					m_proxy.ProxyRound.position = Vector3.Lerp(ReceiverLowerPathForward.position, ReceiverLowerPathRearward.position, t);
					m_proxy.ProxyRound.rotation = Quaternion.Slerp(ReceiverLowerPathForward.rotation, ReceiverLowerPathRearward.rotation, t);
					if (LoadingGate != null)
					{
						LoadingGate.localEulerAngles = new Vector3(LoadingGateAngleRange.y, 0f, 0f);
					}
				}
			}
			else if (LoadingGate != null)
			{
				LoadingGate.localEulerAngles = new Vector3(LoadingGateAngleRange.y, 0f, 0f);
			}
			if (Chamber.IsFull)
			{
				Chamber.ProxyRound.position = Vector3.Lerp(ReceiverEjectionPathForward.position, ReceiverEjectionPathRearward.position, t);
				Chamber.ProxyRound.rotation = Quaternion.Slerp(ReceiverEjectionPathForward.rotation, ReceiverEjectionPathRearward.rotation, t);
			}
			if (UsesSecondChamber && Chamber2.IsFull)
			{
				Chamber2.ProxyRound.position = Vector3.Lerp(ReceiverEjectionPathForward.position, ReceiverEjectionPathRearward.position, t);
				Chamber2.ProxyRound.rotation = Quaternion.Slerp(ReceiverEjectionPathForward.rotation, ReceiverEjectionPathRearward.rotation, t);
			}
			if (m_curLeverPos != ZPos.Rear && !m_proxy.IsFull)
			{
				Chamber.IsAccessible = true;
			}
			else
			{
				Chamber.IsAccessible = false;
			}
			if (UsesSecondChamber)
			{
				if (m_curLeverPos != ZPos.Rear && !m_proxy2.IsFull)
				{
					Chamber2.IsAccessible = true;
				}
				else
				{
					Chamber2.IsAccessible = false;
				}
			}
			for (int i = 0; i < ActuatedPieces.Length; i++)
			{
				if (ActuatedPieces[i].InterpStyle == InterpStyle.Translate)
				{
					ActuatedPieces[i].Piece.localPosition = Vector3.Lerp(ActuatedPieces[i].PosBack, ActuatedPieces[i].PosForward, t);
				}
				else
				{
					ActuatedPieces[i].Piece.localEulerAngles = Vector3.Lerp(ActuatedPieces[i].PosBack, ActuatedPieces[i].PosForward, t);
				}
			}
			m_lastLeverPos = m_curLeverPos;
		}

		private void Fire()
		{
			if (m_isHammerCocked)
			{
				m_isHammerCocked = false;
			}
			else if (m_isHammerCocked2)
			{
				m_isHammerCocked2 = false;
			}
			PlayAudioEvent(FirearmAudioEventType.HammerHit);
			bool flag = false;
			bool flag2 = true;
			if (Chamber.Fire())
			{
				flag = true;
				flag2 = true;
				m_isSecondaryMuzzlePos = false;
			}
			else if (UsesSecondChamber && Chamber2.Fire())
			{
				flag = true;
				flag2 = false;
				m_isSecondaryMuzzlePos = true;
			}
			if (flag)
			{
				if (flag2)
				{
					base.Fire(Chamber, GetMuzzle(), doBuzz: true);
				}
				else
				{
					base.Fire(Chamber2, SecondMuzzle, doBuzz: true);
				}
				FireMuzzleSmoke();
				bool twoHandStabilized = IsTwoHandStabilized();
				bool foregripStabilized = base.AltGrip != null;
				bool shoulderStabilized = IsShoulderStabilized();
				Recoil(twoHandStabilized, foregripStabilized, shoulderStabilized);
				if (flag2)
				{
					PlayAudioGunShot(Chamber.GetRound(), GM.CurrentPlayerBody.GetCurrentSoundEnvironment());
				}
				else
				{
					PlayAudioGunShot(Chamber2.GetRound(), GM.CurrentPlayerBody.GetCurrentSoundEnvironment());
				}
			}
		}

		protected override void FVRFixedUpdate()
		{
			base.FVRFixedUpdate();
			m_curLeverRot = Mathf.MoveTowards(m_curLeverRot, m_tarLeverRot, m_leverRotSpeed * Time.deltaTime);
			Lever.localEulerAngles = new Vector3(m_curLeverRot, 0f, 0f);
			UpdateSpinning();
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
				xSpinVel = Mathf.Clamp(xSpinVel, -200f, 200f);
				xSpinRot += xSpinVel * Time.deltaTime * 5f;
				PoseSpinHolder.localEulerAngles = new Vector3(xSpinRot, 0f, 0f);
				xSpinVel = Mathf.Lerp(xSpinVel, 0f, Time.deltaTime * 0.6f);
			}
			else
			{
				xSpinRot = 0f;
				xSpinVel = 0f;
				PoseSpinHolder.localEulerAngles = new Vector3(xSpinRot, 0f, 0f);
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
