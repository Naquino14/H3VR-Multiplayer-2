using UnityEngine;

namespace Valve.VR.InteractionSystem
{
	public class ItemPackage : MonoBehaviour
	{
		public enum ItemPackageType
		{
			Unrestricted,
			OneHanded,
			TwoHanded
		}

		public new string name;

		public ItemPackageType packageType;

		public GameObject itemPrefab;

		public GameObject otherHandItemPrefab;

		public GameObject previewPrefab;

		public GameObject fadedPreviewPrefab;
	}
}
