using UnityEngine;

namespace FistVR
{
	public class TubeFedShotgunBolt : FVRInteractiveObject
	{
		public enum BoltPos
		{
			Forward,
			ForwardToMid,
			Locked,
			LockedToRear,
			Rear
		}

		[Header("Shotgun Bolt")]
		public TubeFedShotgun Shotgun;

		public float Speed_Forward;

		public float Speed_Rearward;

		public float Speed_Held;

		public float SpringStiffness = 5f;

		public BoltPos CurPos;

		public BoltPos LastPos;

		public Transform Point_Bolt_Forward;

		public Transform Point_Bolt_LockPoint;

		public Transform Point_Bolt_Rear;

		public bool HasLastRoundBoltHoldOpen = true;

		private float m_curBoltSpeed;

		private float m_boltZ_current;

		private float m_boltZ_heldTarget;

		private float m_boltZ_forward;

		private float m_boltZ_lock;

		private float m_boltZ_rear;

		private bool m_isBoltLocked;

		private bool m_isHandleHeld;

		private float m_HandleLerp;

		[Header("Reciprocating Barrel")]
		public bool HasReciprocatingBarrel;

		public Transform Barrel;

		public Vector3 BarrelForward;

		public Vector3 BarrelRearward;

		private bool m_isBarrelReciprocating;

		[Header("Elevator")]
		public bool HasElevator;

		public Transform Elevator;

		public Vector3 ElevatorForward;

		public Vector3 ElevatorRearward;

		[Header("Hammer")]
		public bool HasHammer;

		public Transform Hammer;

		public Vector3 HammerForward;

		public Vector3 HammerRearward;

		protected override void Awake()
		{
			base.Awake();
			m_boltZ_current = base.transform.localPosition.z;
			m_boltZ_forward = Point_Bolt_Forward.localPosition.z;
			m_boltZ_lock = Point_Bolt_LockPoint.localPosition.z;
			m_boltZ_rear = Point_Bolt_Rear.localPosition.z;
		}

		public override bool IsInteractable()
		{
			if (Shotgun.Mode == TubeFedShotgun.ShotgunMode.Automatic)
			{
				return base.IsInteractable();
			}
			return false;
		}

		public float GetBoltLerpBetweenLockAndFore()
		{
			return Mathf.InverseLerp(m_boltZ_lock, m_boltZ_forward, m_boltZ_current);
		}

		public float GetBoltLerpBetweenRearAndFore()
		{
			return Mathf.InverseLerp(m_boltZ_rear, m_boltZ_forward, m_boltZ_current);
		}

		public void LockBolt()
		{
			if (!m_isBoltLocked)
			{
				m_isBoltLocked = true;
			}
		}

		public void ReleaseBolt()
		{
			if (m_isBoltLocked)
			{
				Shotgun.PlayAudioEvent(FirearmAudioEventType.BoltRelease);
				m_isBoltLocked = false;
			}
		}

		public void UpdateHandleHeldState(bool state, float lerp)
		{
			m_isHandleHeld = state;
			m_HandleLerp = lerp;
		}

		public void ImpartFiringImpulse()
		{
			m_curBoltSpeed = Speed_Rearward;
			if (CurPos == BoltPos.Forward)
			{
				m_isBarrelReciprocating = true;
			}
		}

		public void UpdateBolt()
		{
			bool flag = false;
			if (base.IsHeld || m_isHandleHeld)
			{
				flag = true;
			}
			if (base.IsHeld)
			{
				Vector3 closestValidPoint = GetClosestValidPoint(Point_Bolt_Forward.position, Point_Bolt_Rear.position, m_hand.Input.Pos);
				m_boltZ_heldTarget = Shotgun.transform.InverseTransformPoint(closestValidPoint).z;
			}
			else if (m_isHandleHeld)
			{
				m_boltZ_heldTarget = Mathf.Lerp(m_boltZ_forward, m_boltZ_rear, m_HandleLerp);
			}
			Vector2 vector = new Vector2(m_boltZ_rear, m_boltZ_forward);
			if (m_boltZ_current <= m_boltZ_lock && m_isBoltLocked)
			{
				vector = new Vector2(m_boltZ_rear, m_boltZ_lock);
			}
			if (flag)
			{
				m_curBoltSpeed = 0f;
			}
			else if (Shotgun.Mode == TubeFedShotgun.ShotgunMode.Automatic && ((CurPos < BoltPos.LockedToRear && m_curBoltSpeed >= 0f) || LastPos >= BoltPos.Rear))
			{
				m_curBoltSpeed = Mathf.MoveTowards(m_curBoltSpeed, Speed_Forward, Time.deltaTime * SpringStiffness);
			}
			float value = m_boltZ_current;
			float boltZ_current = m_boltZ_current;
			if (flag)
			{
				boltZ_current = m_boltZ_heldTarget;
				value = Mathf.MoveTowards(m_boltZ_current, boltZ_current, Speed_Held * Time.deltaTime);
			}
			else if (Shotgun.Mode == TubeFedShotgun.ShotgunMode.Automatic)
			{
				value = m_boltZ_current + m_curBoltSpeed * Time.deltaTime;
			}
			value = Mathf.Clamp(value, vector.x, vector.y);
			if (Mathf.Abs(value - m_boltZ_current) > Mathf.Epsilon)
			{
				m_boltZ_current = value;
				base.transform.localPosition = new Vector3(base.transform.localPosition.x, base.transform.localPosition.y, m_boltZ_current);
				if (HasElevator)
				{
					float boltLerpBetweenLockAndFore = GetBoltLerpBetweenLockAndFore();
					Elevator.localEulerAngles = Vector3.Lerp(ElevatorRearward, ElevatorForward, boltLerpBetweenLockAndFore);
				}
			}
			else
			{
				m_curBoltSpeed = 0f;
			}
			if (HasHammer)
			{
				if (Shotgun.IsHammerCocked)
				{
					Hammer.localEulerAngles = HammerRearward;
				}
				else
				{
					float boltLerpBetweenLockAndFore2 = GetBoltLerpBetweenLockAndFore();
					Hammer.localEulerAngles = Vector3.Lerp(HammerRearward, HammerForward, boltLerpBetweenLockAndFore2);
				}
			}
			if (HasReciprocatingBarrel && m_isBarrelReciprocating)
			{
				float t = 0f;
				if (!m_isBoltLocked && !base.IsHeld)
				{
					t = 1f - GetBoltLerpBetweenLockAndFore();
				}
				Barrel.localPosition = Vector3.Lerp(BarrelForward, BarrelRearward, t);
			}
			BoltPos curPos = CurPos;
			curPos = ((!(Mathf.Abs(m_boltZ_current - m_boltZ_forward) < 0.0015f)) ? ((Mathf.Abs(m_boltZ_current - m_boltZ_lock) < 0.0015f) ? BoltPos.Locked : ((Mathf.Abs(m_boltZ_current - m_boltZ_rear) < 0.0015f) ? BoltPos.Rear : ((m_boltZ_current > m_boltZ_lock) ? BoltPos.ForwardToMid : BoltPos.LockedToRear))) : BoltPos.Forward);
			int curPos2 = (int)CurPos;
			CurPos = (BoltPos)Mathf.Clamp((int)curPos, curPos2 - 1, curPos2 + 1);
			if (CurPos >= BoltPos.Locked)
			{
				Shotgun.Chamber.IsAccessible = true;
			}
			else
			{
				Shotgun.Chamber.IsAccessible = false;
			}
			if (CurPos >= BoltPos.ForwardToMid)
			{
				Shotgun.IsBreachOpenForGasOut = true;
			}
			else
			{
				Shotgun.IsBreachOpenForGasOut = false;
			}
			if (CurPos == BoltPos.Forward && LastPos != 0)
			{
				BoltEvent_ArriveAtFore();
			}
			else if (CurPos == BoltPos.ForwardToMid && LastPos == BoltPos.Forward)
			{
				BoltEvent_ExtractRoundFromMag();
			}
			else if (CurPos == BoltPos.Locked && LastPos == BoltPos.ForwardToMid)
			{
				BoltEvent_EjectRound();
			}
			else if (CurPos != BoltPos.ForwardToMid || LastPos != BoltPos.Locked)
			{
				if (CurPos == BoltPos.Locked && LastPos == BoltPos.LockedToRear)
				{
					BoltEvent_BoltCaught();
				}
				else if (CurPos == BoltPos.Rear && LastPos != BoltPos.Rear)
				{
					BoltEvent_SmackRear();
				}
			}
			if (CurPos >= BoltPos.Locked && Shotgun.Mode == TubeFedShotgun.ShotgunMode.Automatic && HasLastRoundBoltHoldOpen && Shotgun.Magazine != null && !Shotgun.HasExtractedRound() && !Shotgun.Magazine.HasARound() && !Shotgun.Chamber.IsFull && !Shotgun.IsSlideReleaseButtonHeld)
			{
				LockBolt();
			}
			LastPos = CurPos;
		}

		private void BoltEvent_ArriveAtFore()
		{
			Shotgun.ChamberRound();
			Shotgun.ReturnCarrierRoundToMagazineIfRelevant();
			if (HasReciprocatingBarrel && m_isBarrelReciprocating)
			{
				m_isBarrelReciprocating = false;
				Barrel.localPosition = BarrelForward;
			}
			if (base.IsHeld)
			{
				Shotgun.PlayAudioEvent(FirearmAudioEventType.BoltSlideForwardHeld);
			}
			else
			{
				Shotgun.PlayAudioEvent(FirearmAudioEventType.BoltSlideForward);
			}
		}

		private void BoltEvent_EjectRound()
		{
			Shotgun.EjectExtractedRound();
			Shotgun.TransferShellToUpperTrack();
			Shotgun.CockHammer();
		}

		private void BoltEvent_ExtractRoundFromMag()
		{
			Shotgun.ExtractRound();
		}

		private void BoltEvent_SmackRear()
		{
			if (base.IsHeld || m_isHandleHeld)
			{
				ReleaseBolt();
			}
			if (base.IsHeld)
			{
				Shotgun.PlayAudioEvent(FirearmAudioEventType.BoltSlideBackHeld);
			}
			else
			{
				Shotgun.PlayAudioEvent(FirearmAudioEventType.BoltSlideBack);
			}
		}

		private void BoltEvent_BoltCaught()
		{
			if (m_isBoltLocked)
			{
				if (HasReciprocatingBarrel && m_isBarrelReciprocating)
				{
					m_isBarrelReciprocating = false;
				}
				Shotgun.PlayAudioEvent(FirearmAudioEventType.BoltSlideBackLocked);
			}
		}
	}
}
