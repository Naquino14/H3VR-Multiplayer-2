using UnityEngine;

namespace FistVR
{
	public class HandgunSlide : FVRInteractiveObject
	{
		public enum SlidePos
		{
			Forward,
			ForwardToMid,
			Locked,
			LockedToRear,
			Rear
		}

		public Handgun Handgun;

		public float Speed_Forward;

		public float Speed_Rearward;

		public float Speed_Held;

		public float SpringStiffness = 5f;

		public SlidePos CurPos;

		public SlidePos LastPos;

		public Transform Point_Slide_Forward;

		public Transform Point_Slide_LockPoint;

		public Transform Point_Slide_Rear;

		public bool HasLastRoundSlideHoldOpen = true;

		private float m_curSlideSpeed;

		private float m_slideZ_current;

		private float m_slideZ_heldTarget;

		private float m_slideZ_forward;

		private float m_slideZ_lock;

		private float m_slideZ_rear;

		private float m_handZOffset;

		protected override void Awake()
		{
			base.Awake();
			m_slideZ_current = base.transform.localPosition.z;
			m_slideZ_forward = Point_Slide_Forward.localPosition.z;
			m_slideZ_lock = Point_Slide_LockPoint.localPosition.z;
			m_slideZ_rear = Point_Slide_Rear.localPosition.z;
		}

		public override bool IsInteractable()
		{
			if (Handgun.IsSLideLockMechanismEngaged)
			{
				return false;
			}
			return base.IsInteractable();
		}

		public float GetSlideSpeed()
		{
			return m_curSlideSpeed;
		}

		public float GetSlideLerpBetweenLockAndFore()
		{
			return Mathf.InverseLerp(m_slideZ_lock, m_slideZ_forward, m_slideZ_current);
		}

		public float GetSlideLerpBetweenRearAndFore()
		{
			return Mathf.InverseLerp(m_slideZ_rear, m_slideZ_forward, m_slideZ_current);
		}

		public void ImpartFiringImpulse()
		{
			m_curSlideSpeed = Speed_Rearward;
		}

		public override void BeginInteraction(FVRViveHand hand)
		{
			base.BeginInteraction(hand);
			if (Handgun.Clip != null)
			{
				Handgun.EjectClip();
			}
			m_handZOffset = base.transform.InverseTransformPoint(hand.Input.Pos).z;
		}

		public override void EndInteraction(FVRViveHand hand)
		{
			if (CurPos >= SlidePos.Locked && !Handgun.IsSlideLockUp)
			{
				Handgun.PlayAudioEvent(FirearmAudioEventType.BoltRelease);
			}
			base.EndInteraction(hand);
		}

		public void KnockToRear()
		{
			ImpartFiringImpulse();
		}

		public void UpdateSlide()
		{
			bool flag = false;
			if (base.IsHeld)
			{
				flag = true;
			}
			if ((Handgun.DoesSafetyLockSlide && Handgun.IsSafetyEngaged) || Handgun.IsSLideLockMechanismEngaged)
			{
				return;
			}
			if (base.IsHeld)
			{
				Vector3 closestValidPoint = GetClosestValidPoint(Point_Slide_Forward.position, Point_Slide_Rear.position, m_hand.Input.Pos + -base.transform.forward * m_handZOffset * Handgun.transform.localScale.x);
				m_slideZ_heldTarget = Handgun.transform.InverseTransformPoint(closestValidPoint).z;
			}
			Vector2 vector = new Vector2(m_slideZ_rear, m_slideZ_forward);
			if (m_slideZ_current <= m_slideZ_lock && Handgun.IsSlideCatchEngaged())
			{
				vector = new Vector2(m_slideZ_rear, m_slideZ_lock);
			}
			if (Handgun.Clip != null)
			{
				vector = new Vector2(m_slideZ_rear, m_slideZ_lock);
			}
			if (flag)
			{
				m_curSlideSpeed = 0f;
			}
			else if ((CurPos < SlidePos.LockedToRear && m_curSlideSpeed >= 0f) || LastPos >= SlidePos.Rear)
			{
				m_curSlideSpeed = Mathf.MoveTowards(m_curSlideSpeed, Speed_Forward, Time.deltaTime * SpringStiffness);
			}
			float slideZ_current = m_slideZ_current;
			float slideZ_current2 = m_slideZ_current;
			if (flag)
			{
				slideZ_current2 = m_slideZ_heldTarget;
				slideZ_current = Mathf.MoveTowards(m_slideZ_current, slideZ_current2, Speed_Held * Time.deltaTime);
			}
			else
			{
				slideZ_current = m_slideZ_current + m_curSlideSpeed * Time.deltaTime;
			}
			slideZ_current = Mathf.Clamp(slideZ_current, vector.x, vector.y);
			if (Mathf.Abs(slideZ_current - m_slideZ_current) > Mathf.Epsilon)
			{
				m_slideZ_current = slideZ_current;
				base.transform.localPosition = new Vector3(base.transform.localPosition.x, base.transform.localPosition.y, m_slideZ_current);
			}
			else
			{
				m_curSlideSpeed = 0f;
			}
			SlidePos curPos = CurPos;
			curPos = ((!(Mathf.Abs(m_slideZ_current - m_slideZ_forward) < 0.001f)) ? ((Mathf.Abs(m_slideZ_current - m_slideZ_lock) < 0.001f) ? SlidePos.Locked : ((Mathf.Abs(m_slideZ_current - m_slideZ_rear) < 0.001f) ? SlidePos.Rear : ((m_slideZ_current > m_slideZ_lock) ? SlidePos.ForwardToMid : SlidePos.LockedToRear))) : SlidePos.Forward);
			int curPos2 = (int)CurPos;
			CurPos = (SlidePos)Mathf.Clamp((int)curPos, curPos2 - 1, curPos2 + 1);
			if (CurPos == SlidePos.Forward && LastPos != 0)
			{
				SlideEvent_ArriveAtFore();
			}
			else if (CurPos != SlidePos.ForwardToMid || LastPos != 0)
			{
				if (CurPos == SlidePos.Locked && LastPos == SlidePos.ForwardToMid)
				{
					SlideEvent_EjectRound();
				}
				else if (CurPos == SlidePos.ForwardToMid && LastPos == SlidePos.Locked)
				{
					SlideEvent_ExtractRoundFromMag();
				}
				else if (CurPos == SlidePos.Locked && LastPos == SlidePos.LockedToRear)
				{
					SlideEvent_SlideCaught();
				}
				else if (CurPos == SlidePos.Rear && LastPos != SlidePos.Rear)
				{
					SlideEvent_SmackRear();
				}
			}
			if (CurPos >= SlidePos.Locked && HasLastRoundSlideHoldOpen && Handgun.Magazine != null && !Handgun.Magazine.HasARound() && !Handgun.IsSlideCatchEngaged())
			{
				Handgun.EngageSlideRelease();
			}
			LastPos = CurPos;
		}

		private void SlideEvent_ArriveAtFore()
		{
			Handgun.ChamberRound();
			if (base.IsHeld)
			{
				Handgun.PlayAudioEvent(FirearmAudioEventType.BoltSlideForwardHeld);
			}
			else
			{
				Handgun.PlayAudioEvent(FirearmAudioEventType.BoltSlideForward);
			}
		}

		private void SlideEvent_EjectRound()
		{
			Handgun.EjectExtractedRound();
			if (Handgun.TriggerType != Handgun.TriggerStyle.DAO)
			{
				Handgun.CockHammer(isManual: false);
			}
		}

		private void SlideEvent_ExtractRoundFromMag()
		{
			Handgun.ExtractRound();
		}

		private void SlideEvent_SmackRear()
		{
			Handgun.DropSlideRelease();
			if (base.IsHeld)
			{
				Handgun.PlayAudioEvent(FirearmAudioEventType.BoltSlideBackHeld);
			}
			else
			{
				Handgun.PlayAudioEvent(FirearmAudioEventType.BoltSlideBack);
			}
		}

		private void SlideEvent_SlideCaught()
		{
			if (Handgun.IsSlideCatchEngaged())
			{
				Handgun.PlayAudioEvent(FirearmAudioEventType.BoltSlideBackLocked);
			}
		}
	}
}
