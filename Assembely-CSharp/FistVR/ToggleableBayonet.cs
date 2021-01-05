using UnityEngine;

namespace FistVR
{
	public class ToggleableBayonet : FVRInteractiveObject
	{
		public FVRFireArm FA;

		public Transform Bayonet;

		public float BayonetRot_Closed;

		public float BayonetRot_Open;

		private bool m_bayonetEnabled;

		public override void SimpleInteraction(FVRViveHand hand)
		{
			base.SimpleInteraction(hand);
			ToggleBayonet();
		}

		private void ToggleBayonet()
		{
			if (!FA.MP.IsJointedToObject)
			{
				m_bayonetEnabled = !m_bayonetEnabled;
				if (m_bayonetEnabled)
				{
					Bayonet.localEulerAngles = new Vector3(BayonetRot_Open, 0f, 0f);
					FA.MP.CanNewStab = true;
					FA.PlayAudioEvent(FirearmAudioEventType.BipodOpen);
				}
				else
				{
					Bayonet.localEulerAngles = new Vector3(BayonetRot_Closed, 0f, 0f);
					FA.MP.CanNewStab = false;
					FA.PlayAudioEvent(FirearmAudioEventType.BipodClosed);
				}
			}
		}
	}
}
