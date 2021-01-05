using UnityEngine;

namespace FistVR
{
	public class ClosedBoltHandle : FVRInteractiveObject
	{
		public enum HandlePos
		{
			Forward,
			ForwardToMid,
			Locked,
			LockedToRear,
			Rear
		}

		[Header("Bolt Handle")]
		public ClosedBoltWeapon Weapon;

		public float Speed_Forward;

		public float Speed_Held;

		public float SpringStiffness = 100f;

		public HandlePos CurPos;

		public HandlePos LastPos;

		public Transform Point_Forward;

		public Transform Point_LockPoint;

		public Transform Point_Rear;

		public Transform Point_SafetyRotLimit;

		private float m_curSpeed;

		private float m_posZ_current;

		private float m_posZ_heldTarget;

		private float m_posZ_forward;

		private float m_posZ_lock;

		private float m_posZ_rear;

		private float m_posZ_safetyrotLimit;

		[Header("Safety Catch Config")]
		public bool UsesRotation = true;

		public float Rot_Standard;

		public float Rot_Safe;

		public float Rot_SlipDistance;

		public bool IsSlappable;

		public Transform SlapPoint;

		public float SlapDistance = 0.1f;

		private bool m_hasRotCatch;

		private float m_currentRot;

		[Header("Rotating Bit")]
		public bool HasRotatingPart;

		public Transform RotatingPart;

		public Vector3 RotatingPartNeutralEulers;

		public Vector3 RotatingPartLeftEulers;

		public Vector3 RotatingPartRightEulers;

		public bool StaysRotatedWhenBack;

		public bool UsesSoundOnGrab;

		private bool m_isHandleHeld;

		private float m_HandleLerp;

		private bool m_isAtLockAngle;

		protected override void Awake()
		{
			base.Awake();
			m_posZ_current = base.transform.localPosition.z;
			m_posZ_forward = Point_Forward.localPosition.z;
			m_posZ_lock = Point_LockPoint.localPosition.z;
			m_posZ_rear = Point_Rear.localPosition.z;
			if (Point_SafetyRotLimit != null && UsesRotation)
			{
				m_posZ_safetyrotLimit = Point_SafetyRotLimit.localPosition.z;
				m_hasRotCatch = true;
				m_currentRot = Rot_Standard;
			}
		}

		public float GetBoltLerpBetweenLockAndFore()
		{
			return Mathf.InverseLerp(m_posZ_lock, m_posZ_forward, m_posZ_current);
		}

		public float GetBoltLerpBetweenRearAndFore()
		{
			return Mathf.InverseLerp(m_posZ_rear, m_posZ_forward, m_posZ_current);
		}

		public bool ShouldControlBolt()
		{
			if (!UsesRotation)
			{
				return base.IsHeld;
			}
			if (base.IsHeld)
			{
				return true;
			}
			if (m_isAtLockAngle)
			{
				return true;
			}
			return false;
		}

		public override void BeginInteraction(FVRViveHand hand)
		{
			if (UsesSoundOnGrab)
			{
				Weapon.PlayAudioEvent(FirearmAudioEventType.HandleGrab);
			}
			base.BeginInteraction(hand);
		}

		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);
			if (HasRotatingPart)
			{
				Vector3 normalized = (base.transform.position - m_hand.PalmTransform.position).normalized;
				if (Vector3.Dot(normalized, base.transform.right) > 0f)
				{
					RotatingPart.localEulerAngles = RotatingPartLeftEulers;
				}
				else
				{
					RotatingPart.localEulerAngles = RotatingPartRightEulers;
				}
			}
		}

		public override void EndInteraction(FVRViveHand hand)
		{
			if (HasRotatingPart && !StaysRotatedWhenBack)
			{
				RotatingPart.localEulerAngles = RotatingPartNeutralEulers;
			}
			if (!Weapon.Bolt.IsBoltLocked())
			{
				Weapon.PlayAudioEvent(FirearmAudioEventType.BoltRelease);
			}
			base.EndInteraction(hand);
		}

		public void UpdateHandle()
		{
			bool flag = false;
			if (base.IsHeld)
			{
				flag = true;
			}
			if (flag)
			{
				Vector3 closestValidPoint = GetClosestValidPoint(Point_Forward.position, Point_Rear.position, m_hand.Input.Pos);
				m_posZ_heldTarget = Weapon.transform.InverseTransformPoint(closestValidPoint).z;
			}
			Vector2 vector = new Vector2(m_posZ_rear, m_posZ_forward);
			if (m_hasRotCatch)
			{
				float num = m_currentRot;
				if (!base.IsHeld && IsSlappable && Weapon.IsHeld && m_isAtLockAngle)
				{
					FVRViveHand otherHand = Weapon.m_hand.OtherHand;
					float num2 = Vector3.Distance(SlapPoint.position, otherHand.Input.Pos);
					float num3 = Vector3.Dot(SlapPoint.forward, otherHand.Input.VelLinearWorld.normalized);
					if (num2 < SlapDistance && num3 > 0.3f)
					{
						float magnitude = otherHand.Input.VelLinearWorld.magnitude;
						if (magnitude > 1f)
						{
							num = Rot_Standard;
							Weapon.Bolt.ReleaseBolt();
							if (HasRotatingPart)
							{
								RotatingPart.localEulerAngles = RotatingPartNeutralEulers;
							}
						}
					}
				}
				float num4 = Mathf.InverseLerp(Mathf.Min(Rot_Standard, Rot_Safe), Mathf.Max(Rot_Standard, Rot_Safe), num);
				if (!base.IsHeld)
				{
					num = ((!(num4 <= 0.5f)) ? Mathf.Max(Rot_Standard, Rot_Safe) : Mathf.Min(Rot_Standard, Rot_Safe));
				}
				else if (m_posZ_current < m_posZ_safetyrotLimit)
				{
					Vector3 vector2 = m_hand.Input.Pos - base.transform.position;
					vector2 = Vector3.ProjectOnPlane(vector2, base.transform.forward).normalized;
					Vector3 up = Weapon.transform.up;
					num = Mathf.Atan2(Vector3.Dot(base.transform.forward, Vector3.Cross(up, vector2)), Vector3.Dot(up, vector2)) * 57.29578f;
					num = Mathf.Clamp(num, Mathf.Min(Rot_Standard, Rot_Safe), Mathf.Max(Rot_Standard, Rot_Safe));
				}
				if (Mathf.Abs(num - Rot_Safe) < Rot_SlipDistance)
				{
					vector = new Vector2(m_posZ_rear, m_posZ_lock);
					m_isAtLockAngle = true;
				}
				else if (Mathf.Abs(num - Rot_Standard) < Rot_SlipDistance)
				{
					m_isAtLockAngle = false;
				}
				else
				{
					vector = new Vector2(m_posZ_rear, m_posZ_safetyrotLimit);
					m_isAtLockAngle = true;
				}
				if (Mathf.Abs(num - m_currentRot) > 0.1f)
				{
					base.transform.localEulerAngles = new Vector3(0f, 0f, num);
				}
				m_currentRot = num;
			}
			if (flag)
			{
				m_curSpeed = 0f;
			}
			else if (m_curSpeed >= 0f || CurPos > HandlePos.Forward)
			{
				m_curSpeed = Mathf.MoveTowards(m_curSpeed, Speed_Forward, Time.deltaTime * SpringStiffness);
			}
			float posZ_current = m_posZ_current;
			float posZ_current2 = m_posZ_current;
			if (flag)
			{
				posZ_current2 = m_posZ_heldTarget;
				posZ_current = Mathf.MoveTowards(m_posZ_current, posZ_current2, Speed_Held * Time.deltaTime);
			}
			else
			{
				posZ_current = m_posZ_current + m_curSpeed * Time.deltaTime;
			}
			posZ_current = Mathf.Clamp(posZ_current, vector.x, vector.y);
			if (Mathf.Abs(posZ_current - m_posZ_current) > Mathf.Epsilon)
			{
				m_posZ_current = posZ_current;
				base.transform.localPosition = new Vector3(base.transform.localPosition.x, base.transform.localPosition.y, m_posZ_current);
			}
			else
			{
				m_curSpeed = 0f;
			}
			HandlePos curPos = CurPos;
			curPos = ((!(Mathf.Abs(m_posZ_current - m_posZ_forward) < 0.001f)) ? ((Mathf.Abs(m_posZ_current - m_posZ_lock) < 0.001f) ? HandlePos.Locked : ((Mathf.Abs(m_posZ_current - m_posZ_rear) < 0.001f) ? HandlePos.Rear : ((m_posZ_current > m_posZ_lock) ? HandlePos.ForwardToMid : HandlePos.LockedToRear))) : HandlePos.Forward);
			int curPos2 = (int)CurPos;
			CurPos = (HandlePos)Mathf.Clamp((int)curPos, curPos2 - 1, curPos2 + 1);
			if (CurPos == HandlePos.Forward && LastPos != 0)
			{
				Event_ArriveAtFore();
			}
			else if ((CurPos != HandlePos.ForwardToMid || LastPos != 0) && (CurPos != HandlePos.Locked || LastPos != HandlePos.ForwardToMid) && (CurPos != HandlePos.ForwardToMid || LastPos != HandlePos.Locked))
			{
				if (CurPos == HandlePos.Locked && LastPos == HandlePos.LockedToRear && m_isAtLockAngle)
				{
					Event_HitLockPosition();
				}
				else if (CurPos == HandlePos.Rear && LastPos != HandlePos.Rear)
				{
					Event_SmackRear();
				}
			}
			LastPos = CurPos;
		}

		private void Event_ArriveAtFore()
		{
			Weapon.PlayAudioEvent(FirearmAudioEventType.HandleForward);
			if (HasRotatingPart)
			{
				RotatingPart.localEulerAngles = RotatingPartNeutralEulers;
			}
		}

		private void Event_HitLockPosition()
		{
			Weapon.PlayAudioEvent(FirearmAudioEventType.HandleForward);
		}

		private void Event_SmackRear()
		{
			Weapon.PlayAudioEvent(FirearmAudioEventType.HandleBack);
		}
	}
}
