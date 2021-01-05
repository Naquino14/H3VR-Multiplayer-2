using UnityEngine;

namespace FistVR
{
	public class ZosigEndingStarter : FVRPhysicalObject
	{
		public AudioEvent AudEvent_Eat;

		public ZosigEndingManager M;

		public void Update()
		{
			if (base.IsHeld)
			{
				FVRViveHand hand = m_hand;
				Vector3 b = GM.CurrentPlayerBody.Head.transform.position + GM.CurrentPlayerBody.Head.transform.up * -0.2f;
				if (Vector3.Distance(base.transform.position, b) < 0.15f)
				{
					EndInteraction(m_hand);
					hand.ForceSetInteractable(null);
					SM.PlayGenericSound(AudEvent_Eat, base.transform.position);
					M.InitEnding();
					Object.Destroy(base.gameObject);
				}
			}
		}
	}
}
