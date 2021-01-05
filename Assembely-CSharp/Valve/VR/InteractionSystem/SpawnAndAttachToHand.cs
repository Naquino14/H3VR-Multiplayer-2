using UnityEngine;

namespace Valve.VR.InteractionSystem
{
	public class SpawnAndAttachToHand : MonoBehaviour
	{
		public Hand hand;

		public GameObject prefab;

		public void SpawnAndAttach(Hand passedInhand)
		{
			Hand hand = passedInhand;
			if (passedInhand == null)
			{
				hand = this.hand;
			}
			if (!(hand == null))
			{
				GameObject objectToAttach = Object.Instantiate(prefab);
				hand.AttachObject(objectToAttach, GrabTypes.Scripted);
			}
		}
	}
}
