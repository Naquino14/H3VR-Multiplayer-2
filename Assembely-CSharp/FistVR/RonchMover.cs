using System;
using UnityEngine;

namespace FistVR
{
	public class RonchMover : MonoBehaviour
	{
		[Header("Body Connections")]
		public Transform Torso;

		[Header("Ground Sensing")]
		public Transform CastLeft_Forward;

		public Transform CastLeft_Rearward;

		public Transform CastRight_Forward;

		public Transform CastRight_Rearward;

		public Transform IK_LeftFoot;

		public Transform IK_RightFoot;

		private Vector3 m_curLeftFootPoint;

		private Vector3 m_tarLeftFootPoint;

		private Vector3 m_curRightFootPoint;

		private Vector3 m_tarRightFootPoint;

		public LayerMask LM_GroundCast;

		private RaycastHit m_hit;

		private float m_walkingLerp;

		private float m_torsoLerp_LeftRight = 0.5f;

		private float m_footLerp_Left = 0.5f;

		private float m_footLerp_Right = 0.5f;

		public AnimationCurve CastDistanceCurve_Walking;

		public AudioEvent FootSteps;

		private bool wasLeftFootLerpGoingUp;

		private float m_lastLeftFootLerp;

		private float m_curWorldY;

		private float m_tarWorldY;

		private void Start()
		{
			m_curWorldY = Torso.position.y;
			m_curLeftFootPoint = IK_LeftFoot.position;
			m_tarLeftFootPoint = IK_LeftFoot.position;
			m_curRightFootPoint = IK_RightFoot.position;
			m_curRightFootPoint = IK_RightFoot.position;
		}

		public void SetFacing(Vector3 facing)
		{
			base.transform.rotation = Quaternion.LookRotation(facing, Vector3.up);
		}

		public bool MoveTowardsPosition(RonchWaypoint tarPoint, float speed)
		{
			Vector3 position = base.transform.position;
			position.y = 0f;
			Vector3 position2 = tarPoint.transform.position;
			position2.y = 0f;
			Vector3 vector = Vector3.MoveTowards(position, position2, speed * Time.deltaTime);
			bool result = false;
			if (Vector3.Distance(position2, vector) < 0.001f)
			{
				result = true;
			}
			vector.y = base.transform.position.y;
			if (Physics.Raycast(new Vector3(vector.x, vector.y + 5f, vector.z), -Vector3.up, out m_hit, 25f, LM_GroundCast, QueryTriggerInteraction.Ignore))
			{
				vector.y = m_hit.point.y;
			}
			base.transform.position = vector;
			return result;
		}

		private void Update()
		{
		}

		public void Shudder()
		{
			Torso.Rotate(new Vector3(UnityEngine.Random.Range(-5f, 5f), 0f, UnityEngine.Random.Range(-5f, 5f)));
		}

		public void SetToDeathPose()
		{
			Torso.localPosition = new Vector3(0f, 2f, -4f);
			Torso.localEulerAngles = new Vector3(-33f, 0f, -4f);
		}

		public float UpdateWalking(float t)
		{
			m_walkingLerp += t;
			m_walkingLerp = Mathf.Repeat(m_walkingLerp, (float)Math.PI * 2f);
			m_footLerp_Left = (Mathf.Sin(m_walkingLerp) + 1f) * 0.5f;
			m_footLerp_Right = (Mathf.Sin(m_walkingLerp + (float)Math.PI) + 1f) * 0.5f;
			if (m_footLerp_Left > m_lastLeftFootLerp)
			{
				if (!wasLeftFootLerpGoingUp)
				{
					float delay = Vector3.Distance(IK_LeftFoot.position, GM.CurrentPlayerBody.Head.position) / 343f;
					SM.PlayCoreSoundDelayed(FVRPooledAudioType.GenericLongRange, FootSteps, IK_LeftFoot.position + Vector3.up * 2f, delay);
				}
				wasLeftFootLerpGoingUp = true;
			}
			else
			{
				if (wasLeftFootLerpGoingUp)
				{
					float delay2 = Vector3.Distance(IK_RightFoot.position, GM.CurrentPlayerBody.Head.position) / 343f;
					SM.PlayCoreSoundDelayed(FVRPooledAudioType.GenericLongRange, FootSteps, IK_RightFoot.position + Vector3.up * 2f, delay2);
				}
				wasLeftFootLerpGoingUp = false;
			}
			m_lastLeftFootLerp = m_footLerp_Left;
			m_torsoLerp_LeftRight = Mathf.Sin(m_walkingLerp);
			float time = m_walkingLerp / (float)Math.PI;
			float time2 = m_walkingLerp / (float)Math.PI + 1f;
			return MoveIK(m_footLerp_Left, m_footLerp_Right, 0f - m_torsoLerp_LeftRight, CastDistanceCurve_Walking.Evaluate(time), CastDistanceCurve_Walking.Evaluate(time2));
		}

		private float MoveIK(float lerpLeft, float lerpRight, float lerpLeftRight, float castDistanceLeft, float castDistanceRight)
		{
			Vector3 direction = Vector3.Lerp(CastLeft_Forward.forward, CastLeft_Rearward.forward, lerpLeft);
			Vector3 direction2 = Vector3.Lerp(CastRight_Forward.forward, CastRight_Rearward.forward, lerpRight);
			if (Physics.Raycast(CastLeft_Forward.position, direction, out m_hit, castDistanceLeft, LM_GroundCast, QueryTriggerInteraction.Ignore))
			{
				m_tarLeftFootPoint = m_hit.point;
			}
			else
			{
				m_tarLeftFootPoint = CastLeft_Forward.position + direction.normalized * castDistanceLeft;
			}
			if (Physics.Raycast(CastRight_Forward.position, direction2, out m_hit, castDistanceRight, LM_GroundCast, QueryTriggerInteraction.Ignore))
			{
				m_tarRightFootPoint = m_hit.point;
			}
			else
			{
				m_tarRightFootPoint = CastRight_Forward.position + direction2.normalized * castDistanceRight;
			}
			m_tarWorldY = Mathf.Lerp(IK_LeftFoot.position.y, IK_RightFoot.position.y, 0.5f) + 6f;
			m_curWorldY = Mathf.MoveTowards(m_curWorldY, m_tarWorldY, Time.deltaTime * 2f);
			Vector3 vector = base.transform.InverseTransformPoint(new Vector3(0f, m_curWorldY, 0f));
			Torso.localPosition = new Vector3(lerpLeftRight, vector.y, Mathf.Lerp(IK_LeftFoot.localPosition.z, IK_RightFoot.localPosition.z, 0.5f));
			float a = Vector3.Distance(new Vector3(m_curLeftFootPoint.x, 0f, m_curLeftFootPoint.z), new Vector3(m_tarLeftFootPoint.x, 0f, m_tarLeftFootPoint.z));
			a = Mathf.Max(a, Vector3.Distance(new Vector3(m_curRightFootPoint.x, 0f, m_curRightFootPoint.z), new Vector3(m_tarRightFootPoint.x, 0f, m_tarRightFootPoint.z)));
			m_curLeftFootPoint = Vector3.MoveTowards(m_curLeftFootPoint, m_tarLeftFootPoint, Time.deltaTime * 50f);
			m_curRightFootPoint = Vector3.MoveTowards(m_curRightFootPoint, m_tarRightFootPoint, Time.deltaTime * 50f);
			IK_LeftFoot.position = m_curLeftFootPoint;
			IK_RightFoot.position = m_curRightFootPoint;
			return a;
		}
	}
}
