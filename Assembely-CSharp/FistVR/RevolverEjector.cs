using UnityEngine;

namespace FistVR
{
	public class RevolverEjector : FVRInteractiveObject
	{
		public Revolver Magnum;

		public Transform Ejector;

		public Vector3 ForwardPos;

		public Vector3 RearPos;

		public override bool IsInteractable()
		{
			if (!Magnum.isCylinderArmLocked)
			{
				return true;
			}
			return false;
		}

		public override void SimpleInteraction(FVRViveHand hand)
		{
			base.SimpleInteraction(hand);
			if (Ejector != null)
			{
				Ejector.localPosition = RearPos;
			}
			Magnum.EjectChambers();
		}

		protected override void FVRUpdate()
		{
			base.FVRUpdate();
			if (Ejector != null)
			{
				Ejector.localPosition = Vector3.Lerp(Ejector.localPosition, ForwardPos, Time.deltaTime * 6f);
			}
		}
	}
}
