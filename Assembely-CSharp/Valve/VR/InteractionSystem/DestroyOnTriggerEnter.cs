using UnityEngine;

namespace Valve.VR.InteractionSystem
{
	public class DestroyOnTriggerEnter : MonoBehaviour
	{
		public string tagFilter;

		private bool useTag;

		private void Start()
		{
			if (!string.IsNullOrEmpty(tagFilter))
			{
				useTag = true;
			}
		}

		private void OnTriggerEnter(Collider collider)
		{
			if (!useTag || (useTag && collider.gameObject.tag == tagFilter))
			{
				Object.Destroy(collider.gameObject.transform.root.gameObject);
			}
		}
	}
}
