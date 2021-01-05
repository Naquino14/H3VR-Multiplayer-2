using UnityEngine;

namespace FistVR
{
	public class EdibleThing : FVRPhysicalObject
	{
		public AudioEvent EatSound;

		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);
			Vector3 b = GM.CurrentPlayerBody.Head.transform.position + GM.CurrentPlayerBody.Head.transform.up * -0.2f;
			if (Vector3.Distance(base.transform.position, b) < 0.15f)
			{
				EndInteraction(hand);
				hand.ForceSetInteractable(null);
				SM.PlayGenericSound(EatSound, base.transform.position);
				Object.Destroy(base.gameObject);
			}
		}
	}
}
