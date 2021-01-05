using System;
using UnityEngine;

namespace FistVR
{
	public class OpenBoltReceiverBolt : FVRInteractiveObject
	{
		public enum BoltPos
		{
			Forward,
			ForwardToMid,
			Locked,
			LockedToRear,
			Rear
		}

		[Serializable]
		public class BoltSlidingPiece
		{
			public Transform Piece;

			public float DistancePercent;
		}

		[Header("Bolt Config")]
		public OpenBoltReceiver Receiver;

		public float BoltSpeed_Forward;

		public float BoltSpeed_Rearward;

		public float BoltSpeed_Held;

		public float BoltSpringStiffness = 5f;

		public BoltPos CurPos;

		public BoltPos LastPos;

		public Transform Point_Bolt_Forward;

		public Transform Point_Bolt_LockPoint;

		public Transform Point_Bolt_Rear;

		public Transform Point_Bolt_SafetyCatch;

		public Transform Point_Bolt_SafetyRotLimit;

		public bool HasLastRoundBoltHoldOpen;

		public bool UsesRotatingSafety = true;

		private bool m_doesFiringPinStrikeOnArrivalAtFore = true;

		private float m_curBoltSpeed;

		private float m_boltZ_current;

		private float m_boltZ_heldTarget;

		private float m_boltZ_forward;

		private float m_boltZ_lock;

		private float m_boltZ_rear;

		private float m_boltZ_safetyCatch;

		private float m_boltZ_safetyrotLimit;

		[Header("Safety Catch Config")]
		public float BoltRot_Standard;

		public float BoltRot_Safe;

		public float BoltRot_SlipDistance;

		private bool m_hasSafetyCatch;

		private float m_currentBoltRot;

		[Header("Spring Config")]
		public Transform Spring;

		public Vector2 SpringScales;

		public BoltSlidingPiece[] SlidingPieces;

		private bool m_isChargingHandleHeld;

		private float m_chargingHandleLerp;

		protected override void Awake()
		{
			base.Awake();
			m_boltZ_current = base.transform.localPosition.z;
			m_boltZ_forward = Point_Bolt_Forward.localPosition.z;
			m_boltZ_lock = Point_Bolt_LockPoint.localPosition.z;
			m_boltZ_rear = Point_Bolt_Rear.localPosition.z;
			if (Point_Bolt_SafetyCatch != null && UsesRotatingSafety)
			{
				m_boltZ_safetyCatch = Point_Bolt_SafetyCatch.localPosition.z;
				m_boltZ_safetyrotLimit = Point_Bolt_SafetyRotLimit.localPosition.z;
				m_hasSafetyCatch = true;
				m_currentBoltRot = BoltRot_Standard;
			}
		}

		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);
		}

		public override void EndInteraction(FVRViveHand hand)
		{
			base.EndInteraction(hand);
		}

		public void ChargingHandleHeld(float l)
		{
			m_isChargingHandleHeld = true;
			m_chargingHandleLerp = l;
		}

		public void ChargingHandleReleased()
		{
			m_isChargingHandleHeld = false;
			m_chargingHandleLerp = 0f;
		}

		public float GetBoltLerpBetweenLockAndFore()
		{
			return Mathf.InverseLerp(m_boltZ_lock, m_boltZ_forward, m_boltZ_current);
		}

		public void SetBoltToRear()
		{
			m_boltZ_current = m_boltZ_rear;
		}

		public void UpdateBolt()
		{
			bool flag = false;
			if (base.IsHeld || m_isChargingHandleHeld)
			{
				flag = true;
			}
			if (base.IsHeld)
			{
				Vector3 closestValidPoint = GetClosestValidPoint(Point_Bolt_Forward.position, Point_Bolt_Rear.position, m_hand.Input.Pos);
				m_boltZ_heldTarget = Receiver.transform.InverseTransformPoint(closestValidPoint).z;
			}
			else if (m_isChargingHandleHeld)
			{
				m_boltZ_heldTarget = Mathf.Lerp(m_boltZ_forward, m_boltZ_rear, m_chargingHandleLerp);
			}
			Vector2 vector = new Vector2(m_boltZ_rear, m_boltZ_forward);
			if (m_boltZ_current <= m_boltZ_lock && Receiver.IsBoltCatchEngaged())
			{
				vector = new Vector2(m_boltZ_rear, m_boltZ_lock);
			}
			if (m_hasSafetyCatch)
			{
				float num = m_currentBoltRot;
				float num2 = Mathf.InverseLerp(Mathf.Min(BoltRot_Standard, BoltRot_Safe), Mathf.Max(BoltRot_Standard, BoltRot_Safe), num);
				if (base.IsHeld)
				{
					if (m_boltZ_current < m_boltZ_safetyrotLimit)
					{
						Vector3 vector2 = m_hand.Input.Pos - base.transform.position;
						vector2 = Vector3.ProjectOnPlane(vector2, base.transform.forward).normalized;
						Vector3 up = Receiver.transform.up;
						num = Mathf.Atan2(Vector3.Dot(base.transform.forward, Vector3.Cross(up, vector2)), Vector3.Dot(up, vector2)) * 57.29578f;
						num = Mathf.Clamp(num, Mathf.Min(BoltRot_Standard, BoltRot_Safe), Mathf.Max(BoltRot_Standard, BoltRot_Safe));
					}
				}
				else if (!m_isChargingHandleHeld)
				{
					num = ((!(num2 <= 0.5f)) ? Mathf.Max(BoltRot_Standard, BoltRot_Safe) : Mathf.Min(BoltRot_Standard, BoltRot_Safe));
				}
				if (Mathf.Abs(num - BoltRot_Safe) < BoltRot_SlipDistance)
				{
					vector = new Vector2(m_boltZ_rear, m_boltZ_safetyCatch);
				}
				else if (!(Mathf.Abs(num - BoltRot_Standard) < BoltRot_SlipDistance))
				{
					vector = new Vector2(m_boltZ_rear, m_boltZ_safetyrotLimit);
				}
				if (Mathf.Abs(num - m_currentBoltRot) > 0.1f)
				{
					base.transform.localEulerAngles = new Vector3(0f, 0f, num);
				}
				m_currentBoltRot = num;
			}
			if (flag)
			{
				m_curBoltSpeed = 0f;
			}
			else if (m_curBoltSpeed >= 0f || CurPos >= BoltPos.Locked)
			{
				m_curBoltSpeed = Mathf.MoveTowards(m_curBoltSpeed, BoltSpeed_Forward, Time.deltaTime * BoltSpringStiffness);
			}
			float boltZ_current = m_boltZ_current;
			float target = m_boltZ_current;
			if (flag)
			{
				target = m_boltZ_heldTarget;
			}
			boltZ_current = ((!flag) ? (m_boltZ_current + m_curBoltSpeed * Time.deltaTime) : Mathf.MoveTowards(m_boltZ_current, target, BoltSpeed_Held * Time.deltaTime));
			boltZ_current = Mathf.Clamp(boltZ_current, vector.x, vector.y);
			if (Mathf.Abs(boltZ_current - m_boltZ_current) > Mathf.Epsilon)
			{
				m_boltZ_current = boltZ_current;
				base.transform.localPosition = new Vector3(base.transform.localPosition.x, base.transform.localPosition.y, m_boltZ_current);
				if (SlidingPieces.Length > 0)
				{
					float z = Point_Bolt_Rear.localPosition.z;
					for (int i = 0; i < SlidingPieces.Length; i++)
					{
						Vector3 localPosition = SlidingPieces[i].Piece.localPosition;
						float z2 = Mathf.Lerp(m_boltZ_current, z, SlidingPieces[i].DistancePercent);
						SlidingPieces[i].Piece.localPosition = new Vector3(localPosition.x, localPosition.y, z2);
					}
				}
				if (Spring != null)
				{
					float t = Mathf.InverseLerp(m_boltZ_rear, m_boltZ_forward, m_boltZ_current);
					Spring.localScale = new Vector3(1f, 1f, Mathf.Lerp(SpringScales.x, SpringScales.y, t));
				}
			}
			else
			{
				m_curBoltSpeed = 0f;
			}
			BoltPos curPos = CurPos;
			curPos = ((!(Mathf.Abs(m_boltZ_current - m_boltZ_forward) < 0.001f)) ? ((Mathf.Abs(m_boltZ_current - m_boltZ_lock) < 0.001f) ? BoltPos.Locked : ((Mathf.Abs(m_boltZ_current - m_boltZ_rear) < 0.001f) ? BoltPos.Rear : ((m_boltZ_current > m_boltZ_lock) ? BoltPos.ForwardToMid : BoltPos.LockedToRear))) : BoltPos.Forward);
			int curPos2 = (int)CurPos;
			CurPos = curPos;
			if (CurPos == BoltPos.Rear && LastPos != BoltPos.Rear)
			{
				BoltEvent_BoltSmackRear();
			}
			if (CurPos == BoltPos.Locked && LastPos != BoltPos.Locked)
			{
				BoltEvent_BoltCaught();
			}
			if (CurPos >= BoltPos.Locked && LastPos < BoltPos.Locked)
			{
				BoltEvent_EjectRound();
			}
			if (CurPos < BoltPos.Locked && LastPos > BoltPos.ForwardToMid)
			{
				BoltEvent_BeginChambering();
			}
			if (CurPos == BoltPos.Forward && LastPos != 0)
			{
				BoltEvent_ArriveAtFore();
			}
			LastPos = CurPos;
		}

		private void BoltEvent_ArriveAtFore()
		{
			if (Receiver.ChamberRound())
			{
			}
			if (m_doesFiringPinStrikeOnArrivalAtFore && Receiver.Fire())
			{
				ImpartFiringImpulse();
			}
			if (base.IsHeld || m_isChargingHandleHeld)
			{
				Receiver.PlayAudioEvent(FirearmAudioEventType.BoltSlideForwardHeld);
			}
			else
			{
				Receiver.PlayAudioEvent(FirearmAudioEventType.BoltSlideForward);
			}
		}

		public void ImpartFiringImpulse()
		{
			m_curBoltSpeed = BoltSpeed_Rearward;
		}

		private void BoltEvent_BoltCaught()
		{
			if (Receiver.IsBoltCatchEngaged())
			{
				Receiver.PlayAudioEvent(FirearmAudioEventType.CatchOnSear);
			}
		}

		private void BoltEvent_EjectRound()
		{
			Receiver.EjectExtractedRound();
		}

		private void BoltEvent_BeginChambering()
		{
			Receiver.BeginChamberingRound();
		}

		private void BoltEvent_BoltSmackRear()
		{
			if (base.IsHeld || m_isChargingHandleHeld)
			{
				Receiver.PlayAudioEvent(FirearmAudioEventType.BoltSlideBackHeld);
			}
			else
			{
				Receiver.PlayAudioEvent(FirearmAudioEventType.BoltSlideBack);
			}
		}
	}
}
