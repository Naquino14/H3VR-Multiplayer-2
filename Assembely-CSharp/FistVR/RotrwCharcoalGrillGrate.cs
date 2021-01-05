using UnityEngine;

namespace FistVR
{
	public class RotrwCharcoalGrillGrate : FVRPhysicalObject
	{
		public RotrwCharcoalGrill Grill;

		public bool m_isMountedOnGrill;

		public AudioEvent AudEvent_MountDemount;

		public override bool IsInteractable()
		{
			return Grill.CanPickupGrate();
		}

		public override bool IsDistantGrabbable()
		{
			if (m_isMountedOnGrill)
			{
				return false;
			}
			return true;
		}

		protected override void FVRFixedUpdate()
		{
			base.FVRFixedUpdate();
			if (base.IsHeld && Vector3.Distance(base.transform.position, Grill.GrillGrateSpot.position) < 0.2f && Grill.CanPickupGrate())
			{
				FVRViveHand hand = m_hand;
				EndInteraction(hand);
				hand.ForceSetInteractable(null);
				MountGrate();
				Grill.MountGrate();
			}
		}

		private void MountGrate()
		{
			base.transform.position = Grill.GrillGrateSpot.position;
			base.transform.rotation = Grill.GrillGrateSpot.rotation;
			m_isMountedOnGrill = true;
			base.RootRigidbody.isKinematic = true;
			SM.PlayGenericSound(AudEvent_MountDemount, base.transform.position);
		}

		public override void BeginInteraction(FVRViveHand hand)
		{
			base.BeginInteraction(hand);
			base.RootRigidbody.isKinematic = false;
			if (m_isMountedOnGrill)
			{
				SM.PlayGenericSound(AudEvent_MountDemount, base.transform.position);
				m_isMountedOnGrill = false;
				Grill.DemountGrate();
			}
		}
	}
}
