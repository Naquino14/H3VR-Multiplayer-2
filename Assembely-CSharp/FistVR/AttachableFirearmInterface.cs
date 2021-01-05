using UnityEngine;

namespace FistVR
{
	public class AttachableFirearmInterface : AttachableForegrip
	{
		public AttachableFirearm FA;

		public override void UpdateInteraction(FVRViveHand hand)
		{
			FA.ProcessInput(hand, fromInterface: true, this);
			base.UpdateInteraction(hand);
		}

		public override void PassHandInput(FVRViveHand hand, FVRInteractiveObject o)
		{
			FA.ProcessInput(hand, fromInterface: true, o);
			base.PassHandInput(hand, o);
		}

		protected override void FVRUpdate()
		{
			base.FVRUpdate();
			FA.Tick(Time.deltaTime, m_hand);
		}
	}
}
