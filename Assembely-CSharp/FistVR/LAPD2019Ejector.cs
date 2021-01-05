using UnityEngine;

namespace FistVR
{
	public class LAPD2019Ejector : FVRInteractiveObject
	{
		public LAPD2019 Gun;

		public Transform Ejector;

		public Vector3 ForwardPos;

		public Vector3 RearPos;

		public override bool IsInteractable()
		{
			if (!Gun.isCylinderArmLocked)
			{
				return true;
			}
			return false;
		}

		public override void SimpleInteraction(FVRViveHand hand)
		{
			base.SimpleInteraction(hand);
			Ejector.localPosition = RearPos;
			Gun.EjectChambers();
		}

		protected override void FVRUpdate()
		{
			base.FVRUpdate();
			Ejector.localPosition = Vector3.Lerp(Ejector.localPosition, ForwardPos, Time.deltaTime * 6f);
		}
	}
}
