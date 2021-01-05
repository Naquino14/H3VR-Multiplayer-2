using UnityEngine;

namespace FistVR
{
	public class wwHorseShoeGamePickup : FVRInteractiveObject
	{
		[Header("Pickup Stuff")]
		public wwHorseShoePlinth Plinth;

		public GameObject HorseshoePrefab;

		public override void BeginInteraction(FVRViveHand hand)
		{
			GameObject gameObject = Object.Instantiate(HorseshoePrefab, base.transform.position, base.transform.rotation);
			wwHorseShoe component = gameObject.GetComponent<wwHorseShoe>();
			component.Plinth = Plinth;
			hand.ForceSetInteractable(component);
			component.BeginInteraction(hand);
			Plinth.GrabbedHorseshoe();
		}
	}
}
