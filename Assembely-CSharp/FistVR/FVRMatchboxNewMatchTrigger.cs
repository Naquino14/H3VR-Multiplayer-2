using UnityEngine;

namespace FistVR
{
	public class FVRMatchboxNewMatchTrigger : FVRInteractiveObject
	{
		public GameObject MatchPrefab;

		public override void BeginInteraction(FVRViveHand hand)
		{
			GameObject gameObject = Object.Instantiate(MatchPrefab, hand.transform.position, hand.transform.rotation);
			FVRPhysicalObject component = gameObject.GetComponent<FVRPhysicalObject>();
			hand.ForceSetInteractable(component);
			component.BeginInteraction(hand);
		}
	}
}
