using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class ThirdPersonCamera : MonoBehaviour
	{
		private Transform rig;

		private Transform head;

		private Transform torso;

		private List<Transform> hands;

		public float DistanceBack;

		public Transform Root;

		public Transform VerticalHinge;

		public Transform HorizontalHinge;

		public Transform Dolly;

		public Transform Cam;

		public Rigidbody SosigTorso;

		private float elevationMag;

		private float elevationVel;

		public LayerMask CamMask;

		private RaycastHit m_hit;

		public bool UpdateCam = true;

		private float AttachedRotationMultiplier = 30f;

		private float AttachedPositionMultiplier = 4500f;

		private float AttachedRotationFudge = 500f;

		private float AttachedPositionFudge = 500f;

		private void Start()
		{
			rig = GM.CurrentMovementManager.transform;
			head = GM.CurrentPlayerBody.Head;
			torso = GM.CurrentPlayerBody.Torso;
		}

		private void Update()
		{
			if (UpdateCam)
			{
				float num = Vector3.Distance(rig.position, Root.position);
				if (num > 20f)
				{
					Root.position = rig.position;
				}
				Vector3 b = new Vector3(torso.position.x, rig.position.y, torso.position.z);
				Root.position = Vector3.Lerp(Root.position, b, Time.deltaTime * 8f);
				Vector3 forward = torso.forward;
				VerticalHinge.rotation = Quaternion.Slerp(VerticalHinge.rotation, Quaternion.LookRotation(forward, Vector3.up), Time.deltaTime * 2f);
				float num2 = Vector3.Angle(head.forward, Vector3.up);
				num2 -= 90f;
				num2 *= 0.3f;
				elevationMag = Mathf.SmoothDampAngle(elevationMag, num2, ref elevationVel, 1f);
				HorizontalHinge.localEulerAngles = new Vector3(elevationMag, 0f, 0f);
				float value = DistanceBack;
				if (Physics.Raycast(HorizontalHinge.position, -HorizontalHinge.forward, out m_hit, DistanceBack, CamMask))
				{
					value = m_hit.distance - 0.1f;
				}
				value = Mathf.Clamp(value, 0f, DistanceBack);
				Dolly.localPosition = Vector3.Lerp(Dolly.localPosition, new Vector3(0f, 0f, 0f - value), Time.deltaTime * 8f);
				Cam.position = Vector3.Lerp(Cam.position, Dolly.position, Time.deltaTime * 4f);
				Cam.rotation = HorizontalHinge.rotation;
			}
			if (DistanceFromCoreTarget() > 1f)
			{
				SosigTorso.position = torso.position - torso.up * 0.25f;
			}
		}

		private void FixedUpdate()
		{
			SosigPhys();
		}

		private void SosigPhys()
		{
			Vector3 position = SosigTorso.position;
			Quaternion rotation = SosigTorso.rotation;
			Vector3 vector = torso.position - torso.up * 0.25f;
			Quaternion rotation2 = torso.rotation;
			Vector3 vector2 = vector - position;
			Quaternion quaternion = rotation2 * Quaternion.Inverse(rotation);
			float deltaTime = Time.deltaTime;
			quaternion.ToAngleAxis(out var angle, out var axis);
			if (angle > 180f)
			{
				angle -= 360f;
			}
			if (angle != 0f)
			{
				Vector3 target = deltaTime * angle * axis * AttachedRotationMultiplier;
				SosigTorso.angularVelocity = Vector3.MoveTowards(SosigTorso.angularVelocity, target, AttachedRotationFudge * Time.fixedDeltaTime);
			}
			Vector3 target2 = vector2 * AttachedPositionMultiplier * deltaTime;
			SosigTorso.velocity = Vector3.MoveTowards(SosigTorso.velocity, target2, AttachedPositionFudge * deltaTime);
		}

		public float DistanceFromCoreTarget()
		{
			return Vector3.Distance(SosigTorso.position, torso.position);
		}
	}
}
