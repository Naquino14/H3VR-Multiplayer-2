using System;
using UnityEngine;

namespace Valve.VR.InteractionSystem.Sample
{
	public class BuggyBuddy : MonoBehaviour
	{
		public Transform turret;

		private float turretRot;

		[Tooltip("Maximum steering angle of the wheels")]
		public float maxAngle = 30f;

		[Tooltip("Maximum Turning torque")]
		public float maxTurnTorque = 30f;

		[Tooltip("Maximum torque applied to the driving wheels")]
		public float maxTorque = 300f;

		[Tooltip("Maximum brake torque applied to the driving wheels")]
		public float brakeTorque = 30000f;

		[Tooltip("If you need the visual wheels to be attached automatically, drag the wheel shape here.")]
		public GameObject[] wheelRenders;

		[Tooltip("The vehicle's speed when the physics engine can use different amount of sub-steps (in m/s).")]
		public float criticalSpeed = 5f;

		[Tooltip("Simulation sub-steps when the speed is above critical.")]
		public int stepsBelow = 5;

		[Tooltip("Simulation sub-steps when the speed is below critical.")]
		public int stepsAbove = 1;

		private WheelCollider[] m_Wheels;

		public AudioSource au_motor;

		[HideInInspector]
		public float mvol;

		public AudioSource au_skid;

		private float svol;

		public WheelDust skidsample;

		private float skidSpeed = 3f;

		public Vector3 localGravity;

		[HideInInspector]
		public Rigidbody body;

		public float rapidfireTime;

		private float shootTimer;

		[HideInInspector]
		public Vector2 steer;

		[HideInInspector]
		public float throttle;

		[HideInInspector]
		public float handBrake;

		[HideInInspector]
		public Transform controllerReference;

		[HideInInspector]
		public float speed;

		public Transform centerOfMass;

		private void Start()
		{
			body = GetComponent<Rigidbody>();
			m_Wheels = GetComponentsInChildren<WheelCollider>();
			body.centerOfMass = body.transform.InverseTransformPoint(centerOfMass.position) * body.transform.lossyScale.x;
		}

		private void Update()
		{
			m_Wheels[0].ConfigureVehicleSubsteps(criticalSpeed, stepsBelow, stepsAbove);
			float num = maxTorque * throttle;
			if (steer.y < -0.5f)
			{
				num *= -1f;
			}
			float num2 = maxAngle * steer.x;
			speed = base.transform.InverseTransformVector(body.velocity).z;
			float num3 = Mathf.Abs(speed);
			num2 /= 1f + num3 / 20f;
			float num4 = Mathf.Abs(num);
			mvol = Mathf.Lerp(mvol, Mathf.Pow(num4 / maxTorque, 0.8f) * Mathf.Lerp(0.4f, 1f, Mathf.Abs(m_Wheels[2].rpm) / 200f) * Mathf.Lerp(1f, 0.5f, handBrake), Time.deltaTime * 9f);
			au_motor.volume = Mathf.Clamp01(mvol);
			float value = Mathf.Lerp(0.8f, 1f, mvol);
			au_motor.pitch = Mathf.Clamp01(value);
			svol = Mathf.Lerp(svol, skidsample.amt / skidSpeed, Time.deltaTime * 9f);
			au_skid.volume = Mathf.Clamp01(svol);
			float value2 = Mathf.Lerp(0.9f, 1f, svol);
			au_skid.pitch = Mathf.Clamp01(value2);
			for (int i = 0; i < wheelRenders.Length; i++)
			{
				WheelCollider wheelCollider = m_Wheels[i];
				if (wheelCollider.transform.localPosition.z > 0f)
				{
					wheelCollider.steerAngle = num2;
					wheelCollider.motorTorque = num;
				}
				if (wheelCollider.transform.localPosition.z < 0f)
				{
				}
				wheelCollider.motorTorque = num;
				if (wheelCollider.transform.localPosition.x < 0f)
				{
				}
				if (wheelCollider.transform.localPosition.x >= 0f)
				{
				}
				if (wheelRenders[i] != null && m_Wheels[0].enabled)
				{
					wheelCollider.GetWorldPose(out var pos, out var quat);
					Transform transform = wheelRenders[i].transform;
					transform.position = pos;
					transform.rotation = quat;
				}
			}
			steer = Vector2.Lerp(steer, Vector2.zero, Time.deltaTime * 4f);
		}

		private void FixedUpdate()
		{
			body.AddForce(localGravity, ForceMode.Acceleration);
		}

		public static float FindAngle(Vector3 fromVector, Vector3 toVector, Vector3 upVector)
		{
			if (toVector == Vector3.zero)
			{
				return 0f;
			}
			float num = Vector3.Angle(fromVector, toVector);
			Vector3 lhs = Vector3.Cross(fromVector, toVector);
			num *= Mathf.Sign(Vector3.Dot(lhs, upVector));
			return num * ((float)Math.PI / 180f);
		}
	}
}
