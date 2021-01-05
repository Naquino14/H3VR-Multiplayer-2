using UnityEngine;

namespace Valve.VR.InteractionSystem
{
	[RequireComponent(typeof(Interactable))]
	public class DestroyOnDetachedFromHand : MonoBehaviour
	{
		private void OnDetachedFromHand(Hand hand)
		{
			Object.Destroy(base.gameObject);
		}
	}
}
