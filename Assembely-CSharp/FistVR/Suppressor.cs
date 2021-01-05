using UnityEngine;

namespace FistVR
{
	public class Suppressor : MuzzleDevice
	{
		[Header("Suppressor Settings")]
		public float CatchRot;

		private float m_smoothedCatchRotDelta;

		public AudioSource AudSourceScrewOnOff;

		public AudioClip[] AudClipsScrewOnOff;

		public bool IsIntegrate;

		private Vector3 lastHandForward = Vector3.zero;

		private Vector3 lastMountForward = Vector3.zero;

		public override void BeginInteraction(FVRViveHand hand)
		{
			if (!IsIntegrate)
			{
				base.BeginInteraction(hand);
			}
		}

		public void CatchRotDeltaAdd(float f)
		{
			m_smoothedCatchRotDelta += Mathf.Abs(f);
		}

		protected override void FVRUpdate()
		{
			base.FVRUpdate();
			if (m_smoothedCatchRotDelta > 0f)
			{
				if (!AudSourceScrewOnOff.isPlaying)
				{
					AudSourceScrewOnOff.clip = AudClipsScrewOnOff[Random.Range(0, AudClipsScrewOnOff.Length)];
					AudSourceScrewOnOff.Play();
					AudSourceScrewOnOff.volume = Mathf.Clamp(m_smoothedCatchRotDelta * 0.03f, 0f, 1.2f);
				}
				else
				{
					AudSourceScrewOnOff.volume = Mathf.Clamp(m_smoothedCatchRotDelta * 0.03f, 0f, 1.2f);
				}
				m_smoothedCatchRotDelta -= 400f * Time.deltaTime;
			}
			else if (AudSourceScrewOnOff != null)
			{
				AudSourceScrewOnOff.volume = 0f;
				if (AudSourceScrewOnOff.isPlaying)
				{
					AudSourceScrewOnOff.Stop();
				}
			}
		}

		protected override Quaternion GetRotTarget()
		{
			if (Sensor.CurHoveredMount != null)
			{
				Vector3 up = Sensor.CurHoveredMount.transform.up;
				up = Quaternion.AngleAxis(CatchRot, Sensor.CurHoveredMount.transform.forward) * up;
				return Quaternion.LookRotation(Sensor.CurHoveredMount.transform.forward, up);
			}
			return base.GetRotTarget();
		}

		protected override Vector3 GetPosTarget()
		{
			if (Sensor.CurHoveredMount != null)
			{
				Vector3 closestValidPoint = GetClosestValidPoint(Sensor.CurHoveredMount.Point_Front.position, Sensor.CurHoveredMount.Point_Rear.position, base.m_handPos);
				if (Vector3.Distance(closestValidPoint, base.m_handPos) < 0.15f || CatchRot > 1f)
				{
					return closestValidPoint;
				}
				return base.GetPosTarget();
			}
			return base.GetPosTarget();
		}

		protected override void UpdateSnappingBasedOnDistance()
		{
			if (Sensor.CurHoveredMount != null)
			{
				Vector3 closestValidPoint = GetClosestValidPoint(Sensor.CurHoveredMount.Point_Front.position, (Sensor.CurHoveredMount.GetRootMount().MyObject as FVRFireArm).MuzzlePos.position, base.transform.position);
				if (Vector3.Distance(closestValidPoint, base.transform.position) < 0.08f || CatchRot > 1f)
				{
					SetSnapping(b: true);
				}
				else
				{
					SetSnapping(b: false);
				}
			}
			else
			{
				SetSnapping(b: false);
			}
		}

		protected override void FVRFixedUpdate()
		{
			if (base.IsHeld && m_isInSnappingMode && Sensor.CurHoveredMount != null)
			{
				float catchRot = CatchRot;
				Vector3 lhs = Vector3.ProjectOnPlane(m_hand.transform.up, base.transform.forward);
				Vector3 rhs = Vector3.ProjectOnPlane(lastHandForward, base.transform.forward);
				float num = Mathf.Atan2(Vector3.Dot(base.transform.forward, Vector3.Cross(lhs, rhs)), Vector3.Dot(lhs, rhs)) * 57.29578f;
				CatchRot -= num;
				Vector3 lhs2 = Vector3.ProjectOnPlane(Sensor.CurHoveredMount.transform.up, base.transform.forward);
				Vector3 rhs2 = Vector3.ProjectOnPlane(lastMountForward, base.transform.forward);
				num = Mathf.Atan2(Vector3.Dot(base.transform.forward, Vector3.Cross(lhs2, rhs2)), Vector3.Dot(lhs2, rhs2)) * 57.29578f;
				CatchRot += num;
				CatchRot = Mathf.Clamp(CatchRot, 0f, 360f);
				lastHandForward = m_hand.transform.up;
				lastMountForward = Sensor.CurHoveredMount.transform.up;
				CatchRotDeltaAdd(Mathf.Abs(CatchRot - catchRot));
			}
			base.FVRFixedUpdate();
		}

		public void AutoMountWell()
		{
			CatchRot = 360f;
			base.transform.localEulerAngles = new Vector3(0f, 0f, CatchRot);
		}

		protected override void SetSnapping(bool b)
		{
			if (m_isInSnappingMode == b)
			{
				return;
			}
			m_isInSnappingMode = b;
			if (m_isInSnappingMode)
			{
				SetAllCollidersToLayer(triggersToo: false, "NoCol");
				if (m_hand != null)
				{
					lastHandForward = m_hand.transform.up;
				}
				lastMountForward = Sensor.CurHoveredMount.transform.up;
			}
			else
			{
				SetAllCollidersToLayer(triggersToo: false, "Default");
			}
		}

		public override void AttachToMount(FVRFireArmAttachmentMount m, bool playSound)
		{
			base.AttachToMount(m, playSound);
			base.transform.localEulerAngles = new Vector3(0f, 0f, CatchRot);
		}

		public virtual void ShotEffect()
		{
		}
	}
}
