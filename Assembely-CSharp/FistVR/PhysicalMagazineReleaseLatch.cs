using UnityEngine;

namespace FistVR
{
	public class PhysicalMagazineReleaseLatch : MonoBehaviour
	{
		public FVRFireArm FireArm;

		public HingeJoint Joint;

		private float timeSinceLastCollision = 6f;

		private void FixedUpdate()
		{
			if (timeSinceLastCollision < 5f)
			{
				timeSinceLastCollision += Time.deltaTime;
			}
			if (FireArm.Magazine != null && timeSinceLastCollision < 0.03f && Joint.angle < -35f)
			{
				FireArm.EjectMag();
			}
		}

		private void OnCollisionEnter(Collision col)
		{
			if (col.collider.attachedRigidbody != null && col.collider.attachedRigidbody != FireArm.RootRigidbody && col.collider.attachedRigidbody.gameObject.GetComponent<FVRPhysicalObject>() != null && col.collider.attachedRigidbody.gameObject.GetComponent<FVRPhysicalObject>().IsHeld)
			{
				timeSinceLastCollision = 0f;
			}
		}
	}
}
