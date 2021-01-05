using UnityEngine;

namespace FistVR
{
	public class AttachableFirearmPhysicalObject : FVRFireArmAttachment
	{
		public AttachableFirearm FA;

		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);
			FA.ProcessInput(hand, fromInterface: false, this);
		}

		protected override void FVRUpdate()
		{
			base.FVRUpdate();
			FA.Tick(Time.deltaTime, m_hand);
		}
	}
}
