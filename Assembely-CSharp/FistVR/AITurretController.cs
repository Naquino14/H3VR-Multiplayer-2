using UnityEngine;

namespace FistVR
{
	public class AITurretController : MonoBehaviour
	{
		public Transform root;

		public float XRotSpeed = 1f;

		public float YRotSpeed = 1f;

		public Transform YAxisTransform;

		public bool RotateY;

		public bool ClampY;

		public float RotateYMaxAngle;

		public Transform XAxisTransform;

		public bool RotateX;

		public bool ClampX;

		public float RotateXMaxAngle;

		private Vector3 m_targetDirection = Vector3.forward;

		private Vector3 m_lookPoint = Vector3.zero;

		public AIWeaponSystem WeaponSystem;

		public bool FireAtWill;

		public float AngularCutoff = 5f;

		public void SetFireAtWill(bool b)
		{
			FireAtWill = b;
		}

		public void UpdateTurretController()
		{
			RotateTowardsTarget();
			if (FireAtWill)
			{
				Vector3 directionToPoint = GetDirectionToPoint(m_lookPoint, WeaponSystem.Muzzle.position);
				Debug.DrawLine(m_lookPoint, m_lookPoint + directionToPoint, Color.blue);
				Debug.DrawLine(m_lookPoint, m_lookPoint + WeaponSystem.Muzzle.forward, Color.green);
				if (Vector3.Angle(directionToPoint, WeaponSystem.Muzzle.forward) <= AngularCutoff)
				{
					WeaponSystem.SetShouldFire(b: true);
				}
				else
				{
					WeaponSystem.SetShouldFire(b: false);
				}
			}
			else
			{
				WeaponSystem.SetShouldFire(b: false);
			}
			WeaponSystem.UpdateWeaponSystem();
		}

		private void RotateTowardsTarget()
		{
			if (RotateX)
			{
				Vector3 directionToPoint = GetDirectionToPoint(m_lookPoint, XAxisTransform.position);
				Vector3 forward = Vector3.ProjectOnPlane(directionToPoint, XAxisTransform.right);
				Quaternion to = Quaternion.LookRotation(forward, Vector3.up);
				Quaternion rotation = YAxisTransform.rotation;
				Quaternion to2 = Quaternion.RotateTowards(rotation, to, RotateXMaxAngle);
				if (ClampX)
				{
					XAxisTransform.rotation = Quaternion.RotateTowards(XAxisTransform.rotation, to2, Time.deltaTime * XRotSpeed);
					XAxisTransform.localEulerAngles = new Vector3(XAxisTransform.localEulerAngles.x, 0f, 0f);
				}
				else
				{
					XAxisTransform.rotation = Quaternion.RotateTowards(XAxisTransform.rotation, to, Time.deltaTime * XRotSpeed);
					XAxisTransform.localEulerAngles = new Vector3(XAxisTransform.localEulerAngles.x, 0f, 0f);
				}
			}
			if (RotateY)
			{
				Vector3 directionToPoint2 = GetDirectionToPoint(m_lookPoint, XAxisTransform.position);
				Vector3 forward2 = Vector3.ProjectOnPlane(directionToPoint2, YAxisTransform.up);
				Quaternion to3 = Quaternion.LookRotation(forward2, Vector3.up);
				Quaternion rotation2 = root.rotation;
				Quaternion to4 = Quaternion.RotateTowards(rotation2, to3, RotateYMaxAngle);
				if (ClampY)
				{
					YAxisTransform.rotation = Quaternion.RotateTowards(YAxisTransform.rotation, to4, Time.deltaTime * YRotSpeed);
					YAxisTransform.localEulerAngles = new Vector3(0f, YAxisTransform.localEulerAngles.y, 0f);
				}
				else
				{
					YAxisTransform.rotation = Quaternion.RotateTowards(YAxisTransform.rotation, to3, Time.deltaTime * YRotSpeed);
					YAxisTransform.localEulerAngles = new Vector3(0f, YAxisTransform.localEulerAngles.y, 0f);
				}
			}
		}

		public void SetTargetPoint(Vector3 v)
		{
			m_lookPoint = v;
			m_targetDirection = GetDirectionToPoint(v);
		}

		public Vector3 GetDirectionToPoint(Vector3 point)
		{
			return (point - base.transform.position).normalized;
		}

		public Vector3 GetDirectionToPoint(Vector3 point, Vector3 origin)
		{
			return (point - origin).normalized;
		}
	}
}
