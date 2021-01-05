using UnityEngine;

namespace Valve.VR.InteractionSystem.Sample
{
	public class Grenade : MonoBehaviour
	{
		public GameObject explodePartPrefab;

		public int explodeCount = 10;

		public float minMagnitudeToExplode = 1f;

		private Interactable interactable;

		private void Start()
		{
			interactable = GetComponent<Interactable>();
		}

		private void OnCollisionEnter(Collision collision)
		{
			if ((!(interactable != null) || !(interactable.attachedToHand != null)) && collision.impulse.magnitude > minMagnitudeToExplode)
			{
				for (int i = 0; i < explodeCount; i++)
				{
					GameObject gameObject = Object.Instantiate(explodePartPrefab, base.transform.position, base.transform.rotation);
					gameObject.GetComponentInChildren<MeshRenderer>().material.SetColor("_TintColor", Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f));
				}
				Object.Destroy(base.gameObject);
			}
		}
	}
}
