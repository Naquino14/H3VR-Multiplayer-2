using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class LeverActionTubeACtion : FVRInteractiveObject
	{
		public enum FollowerPos
		{
			Forward,
			Middle,
			ClampedRear,
			Rear
		}

		public FVRFireArm FA;

		public Transform Gun;

		public Transform Root;

		public Transform SwingMag;

		public FVRFireArmMagazine Mag;

		public Transform Spring;

		private int m_cachedCapacity = -1;

		public Transform Pos_Forward;

		public Transform Pos_Rearward;

		public Vector3 SpringScaleForward;

		public Vector3 SpringScaleRearward;

		public List<Vector3> LocalPos_PerRound;

		public List<float> LocalSpringScale_PerRound;

		private float m_zCurrent;

		private float m_zForward;

		private float m_zRearward;

		private float m_zRoundRearClamp;

		private float m_zHeldTarget;

		private bool m_isTubeOpen;

		public float ZClampWhenOpened;

		public float Speed = -5f;

		public FollowerPos FCurPos = FollowerPos.Rear;

		public FollowerPos FLastPos = FollowerPos.Rear;

		private float m_curRot;

		private float m_lastRot;

		public AudioEvent AudEvent_FollowerForward;

		public AudioEvent AudEvent_FollowerRearward;

		public AudioEvent AudEvent_FollowerForwardHeld;

		public AudioEvent AudEvent_FollowerRearwardHeld;

		public AudioEvent AudEvent_SwingHit;

		protected override void Awake()
		{
			base.Awake();
			m_zCurrent = base.transform.localPosition.z;
			m_zHeldTarget = base.transform.localPosition.z;
			m_zForward = Pos_Forward.localPosition.z;
			m_zRearward = Pos_Rearward.localPosition.z;
			m_zRoundRearClamp = Pos_Rearward.localPosition.z;
		}

		protected override void FVRUpdate()
		{
			base.FVRUpdate();
			if (m_cachedCapacity != Mag.m_numRounds)
			{
				m_cachedCapacity = Mag.m_numRounds;
				m_zRoundRearClamp = LocalPos_PerRound[m_cachedCapacity].z;
			}
			bool flag = false;
			if (base.IsHeld)
			{
				flag = true;
			}
			Vector3 zero = Vector3.zero;
			if (base.IsHeld)
			{
				zero = GetClosestValidPoint(Pos_Forward.position, Pos_Rearward.position, m_hand.Input.Pos);
				m_zHeldTarget = Root.InverseTransformPoint(zero).z;
			}
			Vector2 vector = new Vector2(Mathf.Min(m_zRoundRearClamp, m_zForward), Mathf.Max(m_zRoundRearClamp, m_zForward));
			if (m_isTubeOpen)
			{
				vector.x = ZClampWhenOpened;
			}
			float zCurrent = m_zCurrent;
			float zRoundRearClamp = m_zRoundRearClamp;
			if (flag)
			{
				zRoundRearClamp = m_zHeldTarget;
				zCurrent = Mathf.MoveTowards(m_zCurrent, zRoundRearClamp, 5f * Time.deltaTime);
			}
			else
			{
				zCurrent = m_zCurrent + Speed * Time.deltaTime;
			}
			zCurrent = Mathf.Clamp(zCurrent, Mathf.Min(vector.x, vector.y), Mathf.Max(vector.x, vector.y));
			if (Mathf.Abs(zCurrent - m_zCurrent) > Mathf.Epsilon)
			{
				m_zCurrent = zCurrent;
				base.transform.localPosition = new Vector3(base.transform.localPosition.x, base.transform.localPosition.y, m_zCurrent);
				float t = Mathf.InverseLerp(m_zForward, m_zRearward, m_zCurrent);
				Spring.localScale = Vector3.Lerp(SpringScaleForward, SpringScaleRearward, t);
			}
			FollowerPos fCurPos = FCurPos;
			fCurPos = ((!(Mathf.Abs(m_zCurrent - m_zForward) < 0.001f) && !m_isTubeOpen) ? ((Mathf.Abs(m_zCurrent - m_zRoundRearClamp) < 0.001f) ? FollowerPos.ClampedRear : ((!(Mathf.Abs(m_zCurrent - m_zRearward) < 0.001f)) ? FollowerPos.Middle : FollowerPos.Rear)) : FollowerPos.Forward);
			int fCurPos2 = (int)FCurPos;
			FCurPos = fCurPos;
			if ((FCurPos == FollowerPos.Rear || FCurPos == FollowerPos.ClampedRear) && FLastPos != FollowerPos.Rear && FLastPos != FollowerPos.ClampedRear)
			{
				if (base.IsHeld)
				{
					FA.PlayAudioAsHandling(AudEvent_FollowerRearwardHeld, base.transform.position);
				}
				else
				{
					FA.PlayAudioAsHandling(AudEvent_FollowerRearward, base.transform.position);
				}
			}
			else if (FCurPos == FollowerPos.Forward && FLastPos != 0)
			{
				if (base.IsHeld)
				{
					FA.PlayAudioAsHandling(AudEvent_FollowerForwardHeld, base.transform.position);
				}
				else
				{
					FA.PlayAudioAsHandling(AudEvent_FollowerForward, base.transform.position);
				}
			}
			FLastPos = FCurPos;
			if (FCurPos == FollowerPos.Forward && base.IsHeld)
			{
				Vector3 vector2 = m_hand.Input.Pos - SwingMag.position;
				vector2 = Vector3.ProjectOnPlane(vector2, Gun.forward).normalized;
				Vector3 lhs = -Gun.up;
				float value = Mathf.Atan2(Vector3.Dot(Gun.forward, Vector3.Cross(lhs, vector2)), Vector3.Dot(lhs, vector2)) * 57.29578f;
				value = Mathf.Clamp(value, 0f, 90f);
				Debug.Log(value);
				if (Mathf.Abs(value - m_curRot) > Mathf.Epsilon)
				{
					m_curRot = value;
					SwingMag.localEulerAngles = new Vector3(0f, 0f, m_curRot);
				}
				if (m_curRot < 0.001f)
				{
					m_curRot = 0f;
					m_isTubeOpen = false;
				}
				else
				{
					m_isTubeOpen = true;
				}
				if (m_curRot <= 0f && m_lastRot > 0f)
				{
					FA.PlayAudioAsHandling(AudEvent_SwingHit, SwingMag.position);
				}
				else if (m_curRot >= 90f && m_lastRot < 90f)
				{
					FA.PlayAudioAsHandling(AudEvent_SwingHit, SwingMag.position);
				}
			}
			m_lastRot = m_curRot;
		}
	}
}
