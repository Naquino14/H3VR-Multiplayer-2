using UnityEngine;

namespace Valve.VR.InteractionSystem
{
	public class SpawnAndAttachAfterControllerIsTracking : MonoBehaviour
	{
		private Hand hand;

		public GameObject itemPrefab;

		private void Start()
		{
			hand = GetComponentInParent<Hand>();
		}

		private void Update()
		{
			if (itemPrefab != null && hand.isActive && hand.isPoseValid)
			{
				GameObject gameObject = Object.Instantiate(itemPrefab);
				gameObject.SetActive(value: true);
				hand.AttachObject(gameObject, GrabTypes.Scripted);
				hand.TriggerHapticPulse(800);
				Object.Destroy(base.gameObject);
				gameObject.transform.localScale = itemPrefab.transform.localScale;
			}
		}
	}
}
