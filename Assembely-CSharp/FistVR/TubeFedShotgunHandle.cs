using UnityEngine;

namespace FistVR
{
	public class TubeFedShotgunHandle : FVRAlternateGrip
	{
		public enum BoltPos
		{
			Forward,
			ForwardToMid,
			Locked,
			LockedToRear,
			Rear
		}

		[Header("Shotgun Handle")]
		public TubeFedShotgun Shotgun;

		public float Speed_Held;

		public BoltPos CurPos;

		public BoltPos LastPos;

		public Transform Point_Bolt_Forward;

		public Transform Point_Bolt_LockPoint;

		public Transform Point_Bolt_Rear;

		private float m_handZOffset;

		private float m_boltZ_current;

		private float m_boltZ_heldTarget;

		private float m_boltZ_forward;

		private float m_boltZ_lock;

		private float m_boltZ_rear;

		private bool m_isHandleLocked;

		protected override void Awake()
		{
			base.Awake();
			m_boltZ_current = base.transform.localPosition.z;
			m_boltZ_forward = Point_Bolt_Forward.localPosition.z;
			m_boltZ_lock = Point_Bolt_LockPoint.localPosition.z;
			m_boltZ_rear = Point_Bolt_Rear.localPosition.z;
		}

		public override void BeginInteraction(FVRViveHand hand)
		{
			m_handZOffset = base.transform.InverseTransformPoint(hand.Input.Pos).z;
			base.BeginInteraction(hand);
		}

		public float GetBoltLerpBetweenLockAndFore()
		{
			return Mathf.InverseLerp(m_boltZ_lock, m_boltZ_forward, m_boltZ_current);
		}

		public float GetBoltLerpBetweenRearAndFore()
		{
			return Mathf.InverseLerp(m_boltZ_rear, m_boltZ_forward, m_boltZ_current);
		}

		public void LockHandle()
		{
			m_isHandleLocked = true;
		}

		public void UnlockHandle()
		{
			m_isHandleLocked = false;
		}

		public void UpdateHandle()
		{
			bool flag = false;
			if (base.IsHeld || Shotgun.IsAltHeld)
			{
				flag = true;
			}
			if (flag && !m_isHandleLocked)
			{
				Vector3 closestValidPoint;
				if (base.IsHeld)
				{
					closestValidPoint = GetClosestValidPoint(Point_Bolt_Forward.position, Point_Bolt_Rear.position, m_hand.Input.Pos + -base.transform.forward * m_handZOffset * Shotgun.transform.localScale.x);
				}
				else
				{
					if (Shotgun.m_hand.Input.TriggerPressed)
					{
						Vector3 velLinearWorld = Shotgun.m_hand.Input.VelLinearWorld;
						velLinearWorld = Vector3.Project(velLinearWorld, Shotgun.transform.forward);
						velLinearWorld = Shotgun.transform.InverseTransformDirection(velLinearWorld);
						if (Mathf.Abs(velLinearWorld.z) > 1f)
						{
							float z = Shotgun.GrabPointTransform.localPosition.z;
							float num = (0f - velLinearWorld.z) * Time.deltaTime;
							z += num;
							z = Mathf.Clamp(z, Point_Bolt_Rear.localPosition.z - 0.1f, Point_Bolt_Forward.localPosition.z + 0.1f);
							Shotgun.GrabPointTransform.localPosition = new Vector3(Shotgun.GrabPointTransform.localPosition.x, Shotgun.GrabPointTransform.localPosition.y, z);
						}
					}
					closestValidPoint = GetClosestValidPoint(Point_Bolt_Forward.position, Point_Bolt_Rear.position, Shotgun.m_hand.Input.Pos);
				}
				m_boltZ_heldTarget = Shotgun.transform.InverseTransformPoint(closestValidPoint).z;
			}
			float num2 = m_boltZ_current;
			float boltZ_current = m_boltZ_current;
			if (flag && !m_isHandleLocked)
			{
				boltZ_current = m_boltZ_heldTarget;
				num2 = Mathf.MoveTowards(m_boltZ_current, boltZ_current, Speed_Held * Time.deltaTime);
			}
			if (Mathf.Abs(num2 - m_boltZ_current) > Mathf.Epsilon)
			{
				m_boltZ_current = num2;
				base.transform.localPosition = new Vector3(base.transform.localPosition.x, base.transform.localPosition.y, m_boltZ_current);
			}
			BoltPos curPos = CurPos;
			curPos = ((!(Mathf.Abs(m_boltZ_current - m_boltZ_forward) < 0.001f)) ? ((Mathf.Abs(m_boltZ_current - m_boltZ_lock) < 0.001f) ? BoltPos.Locked : ((Mathf.Abs(m_boltZ_current - m_boltZ_rear) < 0.001f) ? BoltPos.Rear : ((m_boltZ_current > m_boltZ_lock) ? BoltPos.ForwardToMid : BoltPos.LockedToRear))) : BoltPos.Forward);
			int curPos2 = (int)CurPos;
			CurPos = (BoltPos)Mathf.Clamp((int)curPos, curPos2 - 1, curPos2 + 1);
			if (CurPos == BoltPos.Forward && LastPos != 0)
			{
				BoltEvent_ArriveAtFore();
			}
			else if (CurPos == BoltPos.Rear && LastPos != BoltPos.Rear)
			{
				BoltEvent_SmackRear();
			}
			LastPos = CurPos;
		}

		private void BoltEvent_ArriveAtFore()
		{
			if (Shotgun.Mode == TubeFedShotgun.ShotgunMode.PumpMode)
			{
				if (Shotgun.IsHammerCocked)
				{
					LockHandle();
				}
				if (Shotgun.Chamber.IsFull || Shotgun.HasExtractedRound())
				{
					Shotgun.PlayAudioEvent(FirearmAudioEventType.HandleForward);
				}
				else
				{
					Shotgun.PlayAudioEvent(FirearmAudioEventType.HandleForwardEmpty);
				}
			}
		}

		private void BoltEvent_SmackRear()
		{
			if (Shotgun.Mode == TubeFedShotgun.ShotgunMode.PumpMode)
			{
				if (Shotgun.Chamber.IsFull || Shotgun.HasExtractedRound())
				{
					Shotgun.PlayAudioEvent(FirearmAudioEventType.HandleBack);
				}
				else
				{
					Shotgun.PlayAudioEvent(FirearmAudioEventType.HandleBackEmpty);
				}
			}
		}

		public override void UpdateInteraction(FVRViveHand hand)
		{
			Vector2 touchpadAxes = hand.Input.TouchpadAxes;
			bool flag = false;
			if (hand.IsInStreamlinedMode && hand.Input.BYButtonDown)
			{
				flag = true;
			}
			else if (hand.Input.TouchpadDown)
			{
				flag = true;
			}
			if (flag && CurPos == BoltPos.Forward && Shotgun.CanModeSwitch)
			{
				Shotgun.ToggleMode();
			}
			base.UpdateInteraction(hand);
		}
	}
}
