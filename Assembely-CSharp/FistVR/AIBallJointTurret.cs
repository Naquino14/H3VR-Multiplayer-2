using System;
using UnityEngine;

namespace FistVR
{
	public class AIBallJointTurret : AIBodyPiece
	{
		public AIClunk Clunk;

		public Transform Root;

		public Transform AimingTransform;

		public Rigidbody RB;

		public AISensorSystem SensorSystem;

		public float RotSpeed;

		private float m_currentRotSpeed;

		public float MaxRotAngle;

		private Vector3 m_targetDirection = Vector3.forward;

		private Vector3 m_lookPoint = Vector3.zero;

		public float TimeTilPlatesReset = 10f;

		public AIFireArmMount[] FireArms;

		private bool IsPermanentlyDisabled;

		public bool DoesRotate = true;

		public bool FireAtWill;

		public float FiringAngularCutoff = 5f;

		private float m_damageReset;

		public void SetTargetPoint(Vector3 v)
		{
			if (IsPlateDamaged)
			{
				v += UnityEngine.Random.onUnitSphere;
			}
			m_lookPoint = v;
			m_targetDirection = GetDirectionToPoint(v);
			for (int i = 0; i < FireArms.Length; i++)
			{
				if (FireArms[i] != null)
				{
					FireArms[i].SetTargetPoint(m_lookPoint);
				}
			}
		}

		public void SetFireAtWill(bool b)
		{
			FireAtWill = b;
			for (int i = 0; i < FireArms.Length; i++)
			{
				if (FireArms[i] != null)
				{
					FireArms[i].SetShouldFire(b);
				}
			}
		}

		public void SetFiringAngleCutoff(float f)
		{
			FiringAngularCutoff = f;
		}

		public override void Awake()
		{
			base.Awake();
			SetTargetPoint(AimingTransform.position + AimingTransform.forward * 2f);
		}

		public override void Update()
		{
			base.Update();
			if (IsPlateDisabled || IsPermanentlyDisabled)
			{
				m_currentRotSpeed = 0f;
				SetFireAtWill(b: false);
				for (int i = 0; i < FireArms.Length; i++)
				{
					FireArms[i] = null;
				}
				Clunk.Die();
			}
			else if (IsPlateDamaged)
			{
				m_currentRotSpeed = UnityEngine.Random.Range(0f, RotSpeed * 0.5f);
				m_damageReset -= Time.deltaTime;
				if (m_damageReset <= 0f)
				{
					ResetAllPlates();
				}
			}
			else
			{
				m_currentRotSpeed = RotSpeed;
			}
			bool flag = true;
			for (int j = 0; j < SensorSystem.Sensors.Length; j++)
			{
				if (SensorSystem.Sensors[j] != null)
				{
					flag = false;
				}
			}
			if (flag)
			{
				IsPermanentlyDisabled = true;
			}
		}

		public override void DestroyEvent()
		{
			IsPermanentlyDisabled = true;
			Debug.Log("Destroyed");
			base.DestroyEvent();
		}

		public override bool SetPlateDamaged(bool b)
		{
			if (base.SetPlateDamaged(b))
			{
				m_damageReset = TimeTilPlatesReset;
				return true;
			}
			return false;
		}

		public override bool SetPlateDisabled(bool b)
		{
			if (base.SetPlateDisabled(b))
			{
				return true;
			}
			return false;
		}

		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (DoesRotate)
			{
				UpdateTurretRot();
			}
		}

		private void UpdateTurretRot()
		{
			Vector3 normalized = Vector3.ProjectOnPlane(m_targetDirection, Root.up).normalized;
			float maxRadiansDelta = MaxRotAngle * (float)Math.PI / 180f;
			Vector3 forward = Vector3.RotateTowards(normalized, m_targetDirection, maxRadiansDelta, 1f);
			Quaternion to = Quaternion.LookRotation(forward, Vector3.up);
			Quaternion rotation = Quaternion.RotateTowards(RB.rotation, to, m_currentRotSpeed * Time.deltaTime);
			base.transform.rotation = rotation;
		}

		public Vector3 GetDirectionToPoint(Vector3 point)
		{
			return (point - AimingTransform.position).normalized;
		}

		public Vector3 GetDirectionToPoint(Vector3 point, Vector3 origin)
		{
			return (point - origin).normalized;
		}
	}
}
