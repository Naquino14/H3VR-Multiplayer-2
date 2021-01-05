using System;
using UnityEngine;

namespace FistVR
{
	public class AIFireArmMount : MonoBehaviour
	{
		public AIFireArm FireArm;

		public bool DoesXRot;

		public Transform XRotMount;

		public Transform XMountedTo;

		public float XMinRot;

		public float XMaxRot;

		public float XRotSpeed;

		public bool DoesYRot;

		public Transform YRotMount;

		public Transform YMountedTo;

		public float YMinRot;

		public float YMaxRot;

		public float YRotSpeed;

		private Vector3 m_targetPoint;

		private bool shouldFire;

		public void Awake()
		{
			m_targetPoint = base.transform.position + base.transform.forward;
		}

		public void SetTargetPoint(Vector3 v)
		{
			m_targetPoint = v;
		}

		public void SetShouldFire(bool b)
		{
			shouldFire = b;
		}

		public void Update()
		{
			float num = 0f;
			if (DoesYRot)
			{
				Vector3 directionToPoint = GetDirectionToPoint(m_targetPoint, YRotMount.position);
				Vector3 to = Vector3.ProjectOnPlane(directionToPoint, YRotMount.up);
				float maxRadiansDelta = Mathf.Abs(YMinRot) * (float)Math.PI / 180f;
				Vector3 vector = Vector3.RotateTowards(YMountedTo.forward, -YMountedTo.right, maxRadiansDelta, 1f);
				float num2 = Mathf.Clamp(Vector3.Angle(vector, to), 0f, Mathf.Abs(YMinRot - YMaxRot));
				maxRadiansDelta = num2 * (float)Math.PI / 180f;
				Vector3 forward = Vector3.RotateTowards(vector, YMountedTo.right, maxRadiansDelta, 1f);
				Quaternion to2 = Quaternion.LookRotation(forward, YMountedTo.up);
				Quaternion rotation = Quaternion.RotateTowards(YRotMount.rotation, to2, YRotSpeed * Time.deltaTime);
				YRotMount.rotation = rotation;
			}
			if (DoesXRot)
			{
				float num3 = 40f;
				Vector2 angles = Vector2.zero;
				float vel = 30f;
				float grav = 9.81f;
				if (FireArm != null)
				{
					vel = FireArm.TrajectoryMuzzleVelocity;
					grav = FireArm.TrajectoryGravityMultiplier * 9.81f;
				}
				if (CalculateInclinationsToTarget(XRotMount.position, m_targetPoint, vel, out angles, grav))
				{
					num3 = angles.x;
				}
				else
				{
					num += 90f;
				}
				Vector3 directionToPoint2 = GetDirectionToPoint(m_targetPoint, XRotMount.position);
				Vector3 vector2 = Vector3.ProjectOnPlane(directionToPoint2, XRotMount.right);
				float maxRadiansDelta2 = Mathf.Abs(XMinRot) * (float)Math.PI / 180f;
				Vector3 vector3 = Vector3.RotateTowards(XMountedTo.forward, -XMountedTo.up, maxRadiansDelta2, 1f);
				float num4 = Vector3.Angle(Vector3.down, XMountedTo.forward) - 90f;
				maxRadiansDelta2 = (num3 - num4) * (float)Math.PI / 180f;
				Vector3 to3 = Vector3.RotateTowards(XMountedTo.forward, Vector3.up, maxRadiansDelta2, 1f);
				float num5 = Mathf.Clamp(Vector3.Angle(vector3, to3), 0f, Mathf.Abs(XMinRot - XMaxRot));
				maxRadiansDelta2 = num5 * (float)Math.PI / 180f;
				Vector3 forward2 = Vector3.RotateTowards(vector3, XMountedTo.up, maxRadiansDelta2, 1f);
				Quaternion to4 = Quaternion.LookRotation(forward2, XMountedTo.up);
				Quaternion rotation2 = Quaternion.RotateTowards(XRotMount.rotation, to4, XRotSpeed * Time.deltaTime);
				XRotMount.rotation = rotation2;
			}
			if (!(FireArm != null))
			{
				return;
			}
			if (shouldFire)
			{
				Vector3 directionToPoint3 = GetDirectionToPoint(m_targetPoint, FireArm.Muzzle.position);
				Vector3 to5 = Vector3.ProjectOnPlane(directionToPoint3, Vector3.up);
				num = Vector3.Angle(Vector3.ProjectOnPlane(FireArm.Muzzle.forward, Vector3.up), to5);
				if (num <= FireArm.FiringAngleThreshold)
				{
					FireArm.SetShouldFire(b: true);
				}
				else
				{
					FireArm.SetShouldFire(b: false);
				}
			}
			else
			{
				FireArm.SetShouldFire(b: false);
			}
			FireArm.UpdateWeaponSystem();
		}

		public Vector3 GetDirectionToPoint(Vector3 point, Vector3 origin)
		{
			return (point - origin).normalized;
		}

		public bool CalculateInclinationsToTarget(Vector3 start, Vector3 end, float vel, out Vector2 angles, float grav)
		{
			angles = Vector2.zero;
			Vector2 a = new Vector2(start.x, start.z);
			Vector2 b = new Vector2(end.x, end.z);
			float num = Vector2.Distance(a, b);
			float num2 = 0f - (start.y - end.y);
			float num3 = num * num;
			float num4 = vel * vel;
			float num5 = vel * vel * vel * vel;
			float num6 = num5 - grav * (grav * num3 + 2f * num2 * num4);
			if (num6 < 0f)
			{
				return false;
			}
			float num7 = num4 - Mathf.Sqrt(num6);
			float num8 = num4 + Mathf.Sqrt(num6);
			float num9 = grav * num;
			float x = Mathf.Atan(num7 / num9) * 57.29578f;
			float y = Mathf.Atan(num8 / num9) * 57.29578f;
			angles = new Vector2(x, y);
			return true;
		}
	}
}
