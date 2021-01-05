using UnityEngine;

namespace FistVR
{
	public class FVRKnife : FVRPhysicalObject
	{
		public Collider Blade;

		public Collider Handle;

		protected override void FVRFixedUpdate()
		{
			base.FVRFixedUpdate();
			if (!base.IsHeld && !(base.RootRigidbody.velocity.magnitude > 0f))
			{
			}
		}

		private new void OnCollisionEnter(Collision col)
		{
			if (!base.IsHeld && col.contacts[0].thisCollider == Blade && Vector3.Angle(col.contacts[0].normal, base.transform.forward) > 95f && col.impulse.magnitude > 0.05f)
			{
				base.RootRigidbody.isKinematic = true;
				base.transform.SetParent(col.contacts[0].otherCollider.transform);
			}
		}

		public override void BeginInteraction(FVRViveHand hand)
		{
			base.BeginInteraction(hand);
			base.transform.SetParent(null);
		}
	}
}
